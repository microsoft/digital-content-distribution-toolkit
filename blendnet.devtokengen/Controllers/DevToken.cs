using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace blendnet.devtokengen.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DevToken : ControllerBase
    {
        private readonly ILogger<DevToken> _logger;

        public DevToken(ILogger<DevToken> logger)
        {
            _logger = logger;
        }

        [HttpGet("superadmin")]
        public async Task<string> SuperAdminToken()
        {
            string serviceId = "0583d21a-1245-4158-8b62-629b24a08797"; // super admin
            string accessToken = await Util.FetchToken(serviceId, "https://api-alpha.kaiza.la", "cloudapi-svc-account", "kv-blendnet-dev");
            
            return accessToken;
        }

        [HttpGet("TSTP")]
        public async Task<string> TestPartnerToken()
        {
            string serviceId = "404002c4-3b2b-4255-b838-ed38ce2696f3"; // test partner
            string accessToken = await Util.FetchToken(serviceId, "https://api-alpha.kaiza.la", "cloudapi-svc-account", "kv-blendnet-dev");
            return accessToken;
        }

        [HttpGet("NOVO")]
        public async Task<string> NovoPartnerToken()
        {
            string serviceId = "a19f4345-876c-4ef2-a154-0c32a38b5ed2"; // NOVO-internal
            string accessToken = await Util.FetchToken(serviceId, "https://api-alpha.kaiza.la", "cloudapi-svc-account", "kv-blendnet-dev");
            return accessToken;
        }

        [HttpGet("device1")]
        public async Task<string> Device1()
        {
            string serviceId = "7f4bc54d-847b-4922-b756-4b6ad8d84624"; // device 1
            string accessToken = await Util.FetchToken(serviceId, "https://api-alpha.kaiza.la", "hubdevice", "shwg");
            return accessToken;
        }

        [HttpGet("analyticsReporter")]
        public async Task<string> AnalyticsReporter()
        {
            string serviceId = "01f61b00-6fc2-4b3c-be21-6b26a2a0dc11"; // Analytics Reporter
            string accessToken = await Util.FetchToken(serviceId, "https://api-alpha.kaiza.la", "cloudapi-svc-account", "kv-blendnet-dev");
            return accessToken;
        }
    }
}
