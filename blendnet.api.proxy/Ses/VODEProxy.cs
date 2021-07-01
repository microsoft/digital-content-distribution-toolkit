using blendnet.common.dto;
using blendnet.common.dto.Ses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using blendnet.api.proxy.Common;
using System.Net.Http.Headers;

namespace blendnet.api.proxy.Ses
{
    /// <summary>
    /// Responsible for invoking SES Service to peform the cancellation
    /// Since; its an external system call and the authentication mechanism is different, not inheriting from baseproxy
    /// </summary>
    public class VODEProxy
    {
        private IConfiguration _configuration;

        private readonly HttpClient _vodeHttpClient;

        private ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public VODEProxy(IHttpClientFactory clientFactory,
                         IConfiguration configuration,
                         ILogger<VODEProxy> logger)
        {
            _vodeHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.SESVODE_HTTP_CLIENT);

            _configuration = configuration;

            _logger = logger;
        }

        /// <summary>
        /// Performs the cancellation for given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BroadcastCancellationResult> CancelBroadcast(Guid id)
        {
            var token = await Login();

            //set the header on the http client
            _vodeHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //cancel the broad cast
            BroadcastCancellationResult cancellationResult = await Cancel(id);

            //invoke logout
            await Logout();

            return cancellationResult;
        }

        /// <summary>
        /// Performs the login and returns the token
        /// </summary>
        /// <returns></returns>
        private async Task<string> Login()
        {
            string url = $"vod-e/api/login";

            LoginRequest loginRequest = new LoginRequest();

            loginRequest.user = _configuration["SESServiceUser"];

            loginRequest.pwd = _configuration["SESServicePwd"];

            string token = await _vodeHttpClient.Post<LoginRequest,String>(url,loginRequest,true);

            return token;
        }

        /// <summary>
        /// Performs log out
        /// </summary>
        /// <returns></returns>
        private async Task Logout()
        {
            string url = $"vod-e/api/logout";

            await _vodeHttpClient.Get<String>(url);
        }

        /// <summary>
        /// Invokes the cancel end point
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<BroadcastCancellationResult> Cancel(Guid id)
        {
            string url = $"/vod-e/api/content/{id}/cancel ";

            BroadcastCancellationResult broadcastCancellationResult = await _vodeHttpClient.Post<String, BroadcastCancellationResult>(url,null, true);

            return broadcastCancellationResult;
        }

    }
}
