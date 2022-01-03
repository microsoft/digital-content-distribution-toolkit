using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Middleware
{
    /// <summary>
    /// Security Header Middleware
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private IConfiguration _configuration;

        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;

            _configuration = configuration;
        }

        /// <summary>
        /// Middleware Logic
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            IHeaderDictionary headers = context.Response.Headers;

            Dictionary<string, string> securityHeaders = _configuration.GetSection("SecurityHeaders").Get<Dictionary<string, string>>();

            if (securityHeaders != null && securityHeaders.Count > 0)
            {
                foreach (string key in securityHeaders.Keys)
                {
                    headers[key] = securityHeaders[key];
                }
            }

            await _next(context);
        }
    }
}
