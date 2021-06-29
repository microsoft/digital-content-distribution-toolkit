using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.common.infrastructure.Extensions;
using blendnet.retailer.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.retailer.repository.CosmosRepository
{
    public class RetailerProviderRepository : IRetailerProviderRepository
    {
        RetailerAppSettings _appSettings;
        private readonly ILogger _logger;
        private Container _container;
        
        public RetailerProviderRepository(CosmosClient dbClient,
                                            IOptionsMonitor<RetailerAppSettings> optionsMonitor,
                                            ILogger<RetailerProviderRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.RetailerProvider);
        }

        /// <summary>
        /// Creates a retailer provider in database
        /// </summary>
        /// <param name="retailerProvider"></param>
        /// <returns>Service Account ID of the created retailer</returns>
        async Task<Guid> IRetailerProviderRepository.CreateRetailerProvider(RetailerProviderDto retailerProvider)
        {
            await _container.CreateItemAsync<RetailerProviderDto>(retailerProvider);
            return retailerProvider.ServiceAccountId;
        }

        /// <summary>
        /// Gets Retailer Provider by Partner Code
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <returns></returns>
        async Task<RetailerProviderDto> IRetailerProviderRepository.GetRetailerProviderByPartnerCode(string partnerCode)
        {
            const string queryString = "SELECT TOP 1 VALUE root FROM root WHERE root.partnerCode = @partnerCode";
            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@partnerCode", partnerCode);
            
            var list = await _container.ExtractDataFromQueryIterator<RetailerProviderDto>(queryDef);
            var result = list.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Gets Retailer Provider by Service Account ID
        /// </summary>
        /// <param name="serviceAccountId"></param>
        /// <returns></returns>
        async Task<RetailerProviderDto> IRetailerProviderRepository.GetRetailerProviderByServiceAccountId(Guid serviceAccountId)
        {
            const string queryString = "SELECT TOP 1 VALUE root FROM root WHERE root.serviceAccountId = @serviceAccountId";
            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@serviceAccountId", serviceAccountId);

            var list = await _container.ExtractDataFromQueryIterator<RetailerProviderDto>(queryDef);
            var result = list.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Gets all Retailer Providers
        /// </summary>
        /// <returns></returns>
        async Task<List<RetailerProviderDto>> IRetailerProviderRepository.GetAllRetailerProviders()
        {
            const string queryString = "SELECT VALUE root FROM root";
            var queryDef = new QueryDefinition(queryString);

            var list = await _container.ExtractDataFromQueryIterator<RetailerProviderDto>(queryDef);
            return list;
        }
    }
}
