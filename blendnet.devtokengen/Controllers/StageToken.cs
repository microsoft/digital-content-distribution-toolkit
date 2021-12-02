using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace blendnet.devtokengen.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StageToken : ControllerBase
    {
        private readonly ILogger<StageToken> _logger;

        public StageToken(ILogger<StageToken> logger)
        {
            _logger = logger;
        }

        [HttpGet("superadmin")]
        public async Task<string> SuperAdminToken()
        {
            string serviceId = "541e925e-079e-4106-a21f-4124950633a3"; // super admin
            string accessToken = await Util.FetchToken(serviceId, "https://api-preprod.kaiza.la", "cloudapi-svc-stg-account", "kv-blendnet-stage");
            return accessToken;
        }

        [HttpGet("TSTP")]
        public async Task<string> TestPartnerToken()
        {
            string serviceId = "8a235288-8adc-468a-811d-c0564befd625"; // test partner
            string accessToken = await Util.FetchToken(serviceId, "https://api-preprod.kaiza.la", "cloudapi-svc-stg-account", "kv-blendnet-stage");
            return accessToken;
        }
    }
}
