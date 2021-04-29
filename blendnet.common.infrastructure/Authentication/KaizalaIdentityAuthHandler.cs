using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Authentication
{
    /// <summary>
    /// Kaizala Authentication handler
    /// </summary>
    public class KaizalaIdentityAuthHandler : AuthenticationHandler<KaizalaIdentityAuthOptions>
    {
        public KaizalaIdentityAuthHandler(  IOptionsMonitor<KaizalaIdentityAuthOptions> options, 
                                            ILoggerFactory logger, 
                                            UrlEncoder encoder, 
                                            ISystemClock clock) : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class KaizalaIdentityAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "KaizalaIdentityScheme";

        public string Scheme => DefaultScheme;
    }
}
