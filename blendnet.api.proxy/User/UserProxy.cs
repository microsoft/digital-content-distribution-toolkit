using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using blendnet.common.dto;
using blendnet.common.dto.User;

namespace blendnet.api.proxy
{
    public class UserProxy
    {
        private readonly HttpClient _rmsHttpClient;

        public UserProxy(IHttpClientFactory clientFactory)
        {
            _rmsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.USER_HTTP_CLIENT);
        }


        public User GetUserById(Guid userId)
        {
            // correct this later
            return new User();
        }

        public User GetUserByPhoneNumber(string phoneNumber)
        {
            //correct this later
            return new User();
        }


    }
}
