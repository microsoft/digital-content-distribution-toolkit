using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Identity
{
    /// <summary>
    /// Api Response from the service generating token for service accounts
    /// </summary>
    public class GetAccessTokenForPartnerServiceResponse
    {
        public string AccessToken { get; set; }
    }
}
