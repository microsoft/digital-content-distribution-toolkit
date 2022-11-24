// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Extensions;
using blendnet.common.dto.Notification;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Kaizala
{
    public class KaizalaNotificationProxy : BaseProxy
    {
        private readonly HttpClient _kaizalaNotificationHttpClient;

        private readonly IConfiguration _configuration;

        ILogger<KaizalaNotificationProxy> _logger;

        private readonly UserProxy _userProxy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        public KaizalaNotificationProxy(IHttpClientFactory clientFactory,
                                    IConfiguration configuration,
                                    ILogger<KaizalaNotificationProxy> logger,
                                    UserProxy userProxy,
                                    IDistributedCache cache
                                )
        : base(configuration, clientFactory, logger, cache)
        {
            _kaizalaNotificationHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT);

            _configuration = configuration;

            _logger = logger;

            _userProxy = userProxy;
        }

        public async Task SendNotification(NotificationPayloadData notificationRequest)
        {
            string kaizalaApplicationName = _configuration.GetValue<string>("KaizalaIdentityAppName");

            notificationRequest.PartnerName = kaizalaApplicationName;

            string accessToken = await base.GetServiceAccessToken();

            string urlToInvoke = $"v1/sendExternalNotification";

            var userIdBatches = GetUserIdBatchBasedOnPhoneLastDigit(notificationRequest.UserData);

            foreach(var batch in userIdBatches)
            {
                KaizalaNotificationRequest kzNotificationRequest = new KaizalaNotificationRequest
                {
                    PartnerName = notificationRequest.PartnerName,
                    Payload = notificationRequest.Payload,
                    UserIds = batch.Value,
                    ScaleUnit = GetUserScaleUnitByPhoneDigit(batch.Key.ToString())

                };
                string url = PrepareHttpClient(accessToken, urlToInvoke);

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    JsonSerializerOptions jsonSerializerOptions = Utilties.GetJsonSerializerOptions();

                    jsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

                    await _kaizalaNotificationHttpClient.Post<KaizalaNotificationRequest, object>(url, kzNotificationRequest, false, jsonSerializerOptions);

                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation($"401 from sendExternalNotification - {string.Join(",", batch.Value)}.  Exception {ex} ");
                    throw;
                }

                stopwatch.Stop();

                _logger.LogInformation($" Time taken to send notification (userIds - {string.Join(",", batch.Value)}) is {stopwatch.ElapsedMilliseconds} (ms)");

            }
        }

        public async Task SendBroadcastNotification(BroadcastNotificationPayloadData broadcastNotificationPayloadData)
        {
            string accessToken = await base.GetServiceAccessToken();

            string urlToInvoke = $"v1/sendExternalBroadcastNotification";

            KaizalaBroadcastNotificationRequest kzBroadcastNotificationRequest = new KaizalaBroadcastNotificationRequest
            {
                PartnerName = broadcastNotificationPayloadData.PartnerName,
                Payload = broadcastNotificationPayloadData.Payload,
                Topic = broadcastNotificationPayloadData.Topic,
                ScaleUnits = GetScaleUnitsForCurrentEnv()
            };

            string url = PrepareHttpClient(accessToken, urlToInvoke);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                JsonSerializerOptions jsonSerializerOptions = Utilties.GetJsonSerializerOptions();

                jsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

                await _kaizalaNotificationHttpClient.Post<KaizalaBroadcastNotificationRequest, object>(url, kzBroadcastNotificationRequest, false, jsonSerializerOptions);

            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation($"401 from sendExternalBroadcastNotification - {kzBroadcastNotificationRequest.Topic}  Exception {ex} ");
                throw;
            }

            stopwatch.Stop();

            _logger.LogInformation($" Time taken to send notification {kzBroadcastNotificationRequest.Topic} is {stopwatch.ElapsedMilliseconds} (ms)");
        }

        private string PrepareHttpClient(string accessToken,
                                        string urlToInvoke)
        {
            

            string httpBaseHref = string.Format(_configuration["KaizalaIdentityBaseUrl"], ""); // base url do not need SU

            string url = $"{httpBaseHref}{urlToInvoke}";

            _kaizalaNotificationHttpClient.DefaultRequestHeaders.Remove("accessToken");

            _kaizalaNotificationHttpClient.DefaultRequestHeaders.Add("accessToken", accessToken);

            return url;
        }

        private Dictionary<char, List<Guid>> GetUserIdBatchBasedOnPhoneLastDigit(List<UserData> userData)
        {
            // map the user ID to Kaizala ID 
            // queries each user 
            // TODO: SAMEERA: This can cause a lot of (sequential) User Proxy calls. Need to optimize.
            var userDataWithMappedIds = userData.Select(x => {
                return new UserData() { // creating new, so that original UserData is preserved.
                    PhoneNumber = x.PhoneNumber,
                    UserId = (_userProxy.GetUserByPhoneNumber(x.PhoneNumber)).Result.IdentityId, // explicit wait on userProxy call so that subsequent calls run sequentially
                };
            });

            var userIdBatch = userDataWithMappedIds.GroupBy(data => data.PhoneNumber[data.PhoneNumber.Length - 1]
            , (key, val) => (key, val.Select(x => x.UserId).ToList()))
                .ToDictionary(x => x.key, y => y.Item2);

            return userIdBatch;
        }

    }
}
