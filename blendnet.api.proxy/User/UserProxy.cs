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
using blendnet.common.dto.Extensions;


namespace blendnet.api.proxy
{
    /// <summary>
    /// Proxy to Invoke User Controler API Methods
    /// </summary>
    public class UserProxy: BaseProxy
    {
        private readonly HttpClient _userHttpClient;

        private readonly IDistributedCache _cache;

        public UserProxy(IHttpClientFactory clientFactory,
                          IConfiguration configuration,
                          ILogger<UserProxy> logger,
                          IDistributedCache cache)
             : base(configuration, clientFactory, logger, cache)
        {
            _userHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.USER_HTTP_CLIENT);

            _cache = cache;
        }

        /// <summary>
        /// Read the user details based on given phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            string userByPhoneNoCacheKey = $"{phoneNumber}{ApplicationConstants.DistributedCacheKeySuffix.USERBYPHONEKEY}";

            User userFromCache = await _cache.GetAsync<User>(userByPhoneNoCacheKey);

            if (userFromCache != default(User))
            {
                return userFromCache;
            }
            else
            {
                UserByPhoneRequest request = new UserByPhoneRequest() { PhoneNumber = phoneNumber };

                string url = $"User/user";

                string accessToken = await base.GetServiceAccessToken();

                User user = null;

                try
                {
                    user = await _userHttpClient.Post<UserByPhoneRequest, User>(url, request, true, null, accessToken);

                    //if the user is retrieved store in cache for 24 hours to avoid database calls
                    if (user != null)
                    {
                        await _cache.SetAsync<User>(userByPhoneNoCacheKey,
                                                    user, 
                                                    ApplicationConstants.DistributedCacheDurationsInHrs.Long);
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                { }

                return user;
            }
        }
    }
}
