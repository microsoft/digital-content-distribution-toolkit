using blendnet.api.proxy.Common;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Notification;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Kaizala
{
    public class NotificationProxy : BaseProxy
    {
        private readonly HttpClient _kaizalaNotificationHttpClient;

        private readonly IConfiguration _configuration;

        ILogger<NotificationProxy> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        public NotificationProxy(IHttpClientFactory clientFactory,
                                    IConfiguration configuration,
                                    ILogger<NotificationProxy> logger,
                                    IDistributedCache cache
                                )
        : base(configuration, clientFactory, logger, cache)
        {
            _kaizalaNotificationHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT);

            _configuration = configuration;

            _logger = logger;

        }

        public async Task SendNotification(NotificationRequest notificationRequest)
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
            var userIdBatch = userData.GroupBy(data => data.PhoneNumber[data.PhoneNumber.Length - 1]
            , (key, val) => (key, val.Select(x => x.UserId).ToList()))
                .ToDictionary(x => x.key, y => y.Item2);

            return userIdBatch;
        }

    }
}
