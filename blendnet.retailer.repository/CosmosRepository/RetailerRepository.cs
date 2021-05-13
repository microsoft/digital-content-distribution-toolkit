using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.retailer.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace blendnet.retailer.repository.CosmosRepository
{
    public class RetailerRepository : IRetailerRepository
    {
        private Container _container;
        private readonly ILogger _logger;

        RetailerAppSettings _appSettings;

        private const string RetailerReferralCodesPartitionKey = "RetailerReferralCodes";

        public RetailerRepository(CosmosClient dbClient,
                                    IOptionsMonitor<RetailerAppSettings> optionsMonitor,
                                    ILogger<RetailerRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.Retailer);
        }

        /// <summary>
        /// Create a new Retailer
        /// </summary>
        /// <param name="retailer">Retailer entity</param>
        /// <returns>Partner Provided ID</returns>
        public async Task<string> CreateRetailer(RetailerDto retailer)
        {
            string referralCode = await ReserveReferralCode();
            retailer.ReferralCode = referralCode;

            var response = await this._container.CreateItemAsync<RetailerDto>(retailer, new PartitionKey(retailer.PartnerId));
            return retailer.PartnerProvidedId;
        }

        /// <summary>
        /// Update a retailer entity
        /// </summary>
        /// <param name="retailer"></param>
        /// <returns>status code</returns>
        public async Task<int> UpdateRetailer(RetailerDto updatedRetailer)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<RetailerDto>( updatedRetailer,
                                                                                    updatedRetailer.Id.ToString(),
                                                                                    new PartitionKey(updatedRetailer.PartnerId));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Gets Retailer by RetailerPartnerId (composed)
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <returns>Retailer Entity</returns>
        async Task<RetailerDto> IRetailerRepository.GetRetailerByPartnerId(string retailerPartnerId /* composed*/)
        {
            const string queryString = @"SELECT * FROM r WHERE r.partnerId = @retailerPartnerId AND r.type = @type";
            var query = new QueryDefinition(queryString)
                                .WithParameter("@retailerPartnerId", retailerPartnerId)
                                .WithParameter("@type", RetailerContainerType.Retailer);
            var resultList = await this._container.ExtractDataFromQueryIterator<RetailerDto>(query);
            var result = resultList.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Gets Retailer by Referral Code
        /// </summary>
        /// <param name="referralCode"></param>
        /// <returns>Retailer Entity</returns>
        async Task<RetailerDto> IRetailerRepository.GetRetailerByReferralCode(string referralCode)
        {
            const string queryString = @"SELECT * FROM r WHERE r.referralCode = @referralCode AND r.type = @type";
            var query = new QueryDefinition(queryString)
                                .WithParameter("@referralCode", referralCode)
                                .WithParameter("@type", RetailerContainerType.Retailer);
            var resultList = await this._container.ExtractDataFromQueryIterator<RetailerDto>(query);
            var result = resultList.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Gets nearby retailers for a given location and within distance
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance">in meters</param>
        /// <returns>List of Retailer entities and their distance from the location</returns>
        public async Task<List<RetailerWithDistanceDto>> GetNearbyRetailers(double lat, double lng, double distance)
        {
            const string queryString = 
                @"SELECT r as retailer, ST_DISTANCE({
                    type: ""Point"",
                    coordinates: [r.address.mapLocation.latitude, r.address.mapLocation.longitude]
                },{
                    type: ""Point"",
                    coordinates: [@lat, @lng]
                }) as distanceMeters
                FROM r
                WHERE r.type = @type AND ST_DISTANCE({
                    type: ""Point"",
                    coordinates: [r.address.mapLocation.latitude, r.address.mapLocation.longitude]
                },{
                    type: ""Point"",
                    coordinates: [@lat, @lng]
                }) <= @distance";
            var query = new QueryDefinition(queryString)
                            .WithParameter("@type", RetailerContainerType.Retailer)
                            .WithParameter("@lat", lat)
                            .WithParameter("@lng", lng)
                            .WithParameter("@distance", distance);

            var results = await this._container.ExtractDataFromQueryIterator<RetailerWithDistanceDto>(query);

            // sort ascending by distance
            results.Sort((a, b) => (int)(a.DistanceMeters - b.DistanceMeters));

            return results;
        }

        /// <summary>
        /// Reserves a globally unique referral code for retailer
        /// </summary>
        /// <returns>returns the reserved code</returns>
        private async Task<string> ReserveReferralCode()
        {
            const string PREFIX = "RN";
            const int MAX_ATTEMPTS = 100;

            int attemptCount = 0;
            
            while (attemptCount < MAX_ATTEMPTS)
            {
                int code = GetSecuredRandomNumber(100000, 999999);
                string generatedCode = $"{PREFIX}-{code}";
                RetailerReferralCodeDto codeToReserve = new RetailerReferralCodeDto()
                {
                    ReferralCode = generatedCode,
                    PartnerId = RetailerReferralCodesPartitionKey,
                };
                try
                {
                    var response = await this._container.CreateItemAsync<RetailerReferralCodeDto>(codeToReserve, new PartitionKey(RetailerReferralCodesPartitionKey));
                    return generatedCode;
                }
                catch (CosmosException e)
                {
                    if (e.StatusCode == HttpStatusCode.Conflict)
                    {
                        // retry
                        attemptCount++;
                    }
                    else
                    {
                        // rethrow other exceptions
                        throw;
                    }
                }
            }

            // if reached here, we are out of retries; throw exception
            throw new Exception("Unable to reserve Referral Code");
        }

        /// <summary>
        /// Generate a secure random number within a range
        /// Uses Crypto APIs for more stable, less conflicting and less predictable random numbers
        /// </summary>
        /// <param name="minValue">Minimum of the range</param>
        /// <param name="maxValue">Minimum of the range</param>
        /// <returns>the generated random number</returns>
        private static int GetSecuredRandomNumber(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));

            if (minValue == maxValue)
                return minValue;

            byte[] randomBytes = new byte[4];

            using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rNGCryptoServiceProvider.GetBytes(randomBytes);
            }

            uint scale = BitConverter.ToUInt32(randomBytes, 0);
            return (int)(minValue + (maxValue - minValue) * (scale / ((uint.MaxValue - uint.MinValue) + 0.0)));
        }
    }
}
