using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Authentication
{
    public static class KaizalaIdentityAuthBuilderExtensions
    {
        public static AuthenticationBuilder AddKaizalaIdentityAuth(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<KaizalaIdentityAuthOptions, KaizalaIdentityAuthHandler>(KaizalaIdentityAuthOptions.DefaultScheme, _ => { });
                                     
        }

        public static AuthenticationBuilder AddKaizalaIdentityAuth(this AuthenticationBuilder builder, Action<KaizalaIdentityAuthOptions> configureOptions)
        {
            // Add custom authentication scheme with custom options and custom handler
            return builder.AddScheme<KaizalaIdentityAuthOptions, KaizalaIdentityAuthHandler>(KaizalaIdentityAuthOptions.DefaultScheme, configureOptions);
        }

        public static AuthenticationBuilder AddKaizalaBasicIdentityAuth(this AuthenticationBuilder builder)
        {
            //return builder.AddScheme<KaizalaIdentityAuthOptions, KaizalaBasicIdentityAuthHandler>(KaizalaIdentityAuthOptions.BasicIdentityScheme, _ => { });

            return builder.AddScheme<KaizalaIdentityAuthOptions, KaizalaBasicIdentityAuthHandler>(KaizalaIdentityAuthOptions.BasicIdentityScheme, bia => 
                                                                                                                            { 
                                                                                                                                bia.Scheme = KaizalaIdentityAuthOptions.BasicIdentityScheme;
                                                                                                                            });

        }

        public static AuthenticationBuilder AddKaizalaBasicIdentityAuth(this AuthenticationBuilder builder, Action<KaizalaIdentityAuthOptions> configureOptions)
        {
            // Add custom authentication scheme with custom options and custom handler
            return builder.AddScheme<KaizalaIdentityAuthOptions, KaizalaBasicIdentityAuthHandler>(KaizalaIdentityAuthOptions.BasicIdentityScheme, configureOptions);
        }
    }
}
