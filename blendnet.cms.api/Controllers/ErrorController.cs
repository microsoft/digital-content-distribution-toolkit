using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace blendnet.cms.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Global exception handler
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        [HttpGet]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (context != null)
            {
                _logger.LogError(context.Error, context.Error.Message);

                return Problem(detail: context.Error.Message,title: context.Error.Message);

            }
            else
            {
                return Problem(detail: "No detail. Exception context is null", "No detail.Exception context is null");
            }

        }
    }
}
