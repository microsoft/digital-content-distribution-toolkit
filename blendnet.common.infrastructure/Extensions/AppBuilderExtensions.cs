// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
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

                string result;

                //not an ideal way but MalformedContinuationTokenException is private. hence added a check based on type name
                if (exceptionHandlerPathFeature.Error != null &&
                    exceptionHandlerPathFeature.Error.InnerException != null &&
                    exceptionHandlerPathFeature.Error.InnerException.GetType().FullName == "Microsoft.Azure.Cosmos.Query.Core.Exceptions.MalformedContinuationTokenException")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    result = JsonSerializer.Serialize(new { error = "Malformed Continuation Token" });
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    result = JsonSerializer.Serialize(new { error = HttpStatusCode.InternalServerError.ToString() });
                }

                context.Response.ContentType = "application/json";

                logger.LogError(exceptionHandlerPathFeature.Error, exceptionHandlerPathFeature.Error.Message, exceptionHandlerPathFeature.Path);

                await context.Response.WriteAsync(result);
            });
        }
    }
}
