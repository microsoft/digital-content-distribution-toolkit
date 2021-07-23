using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.User;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace blendnet.api.proxy
{
    /// <summary>
    /// Proxy to Invoke User Controler API Methods
    /// </summary>
    public class UserProxy: BaseProxy
    {
        private readonly HttpClient _userHttpClient;

        public UserProxy(IHttpClientFactory clientFactory,
                          IConfiguration configuration,
                          ILogger<UserProxy> logger,
                          IDistributedCache cache)
             : base(configuration, clientFactory, logger, cache)
        {
            _userHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.USER_HTTP_CLIENT);

        }

        /// <summary>
        /// Read the user details based on given phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            string url = $"User/user/{phoneNumber}";

            string accessToken = await base.GetServiceAccessToken();

            User user = null;

            try
            {
                user = await _userHttpClient.Get<User>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {}

            return user;
        }
    }
}
