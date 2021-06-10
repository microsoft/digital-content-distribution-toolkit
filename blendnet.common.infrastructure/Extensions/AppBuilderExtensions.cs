using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace blendnet.common.infrastructure.Extensions
{
    public static class AppBuilderExtensions
    {
        public static void RunCustomGlobalExceptionHandler( this IApplicationBuilder app, ILogger logger)
        {
            //Globar error handler
            app.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                context.Response.ContentType = "application/json";

                logger.LogError(exceptionHandlerPathFeature.Error, exceptionHandlerPathFeature.Error.Message, exceptionHandlerPathFeature.Path);

                var result = JsonSerializer.Serialize(new { error = exceptionHandlerPathFeature.Error.Message });

                await context.Response.WriteAsync(result);
            });
        }
    }
}
