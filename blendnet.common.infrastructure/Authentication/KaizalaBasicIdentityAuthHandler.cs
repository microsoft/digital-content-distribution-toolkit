// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Authentication
{
    /// <summary>
    /// Kaizala Basic Identity Auth Handler, does not check the existence of user in blendnet user collection
    /// </summary>
    public class KaizalaBasicIdentityAuthHandler : BaseKiazalaIdentityAuthHandler
    {

        public KaizalaBasicIdentityAuthHandler( IOptionsMonitor<KaizalaIdentityAuthOptions> options,
                                                ILoggerFactory logger,
                                                UrlEncoder encoder,
                                                ISystemClock clock,
                                                KaizalaIdentityProxy kaizalaIdentityProxy,
                                                ILogger<KaizalaBasicIdentityAuthHandler> authLogger) : base(options, logger, encoder, clock, kaizalaIdentityProxy,authLogger)
        { }
    }
}
