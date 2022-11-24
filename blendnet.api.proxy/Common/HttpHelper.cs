// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Common
{
    /// <summary>
    /// Http Helper
    /// </summary>
    public static class HttpHelper
    {
        public static JsonSerializerOptions _jsonSerializerOptions = Utilties.GetJsonSerializerOptions();

        /// <summary>
        /// Performs the get operation via HttpClient
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<O> Get<O>(this HttpClient httpClient, string url, string accessToken="")
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }
            
            var httpResponse = await httpClient.GetAsync(url);

            await EnsureSuccessStatusCodeAsync(httpResponse);

            var successResponse = await httpResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<O>(successResponse, _jsonSerializerOptions);
        }


        /// <summary>
        /// Performs Post
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="inputrequest"></param>
        /// <param name="parseOutput"></param>
        /// <returns></returns>
        public static async Task<O> Post<I, O>( this HttpClient httpClient, 
                                                string url, 
                                                I inputrequest, 
                                                bool parseOutput = true, 
                                                JsonSerializerOptions jsonSerializerOptions = null,
                                                string accessToken = "")
        {
            HttpResponseMessage httpResponseMessage = null;

            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                       new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            JsonSerializerOptions serializerOptions = _jsonSerializerOptions;

            if (jsonSerializerOptions != null)
            {
                serializerOptions = jsonSerializerOptions;
            }

            if (inputrequest != null)
            {
                var postRequest = new StringContent(
                                           JsonSerializer.Serialize(inputrequest, serializerOptions),
                                           Encoding.UTF8,
                                           "application/json");

                httpResponseMessage = await httpClient.PostAsync(url, postRequest);
            }
            else
            {
                httpResponseMessage = await httpClient.PostAsync(url, null);
            }

            await EnsureSuccessStatusCodeAsync(httpResponseMessage);

            var successResponse = await httpResponseMessage.Content.ReadAsStringAsync();

            if (parseOutput)
            {
                return JsonSerializer.Deserialize<O>(successResponse, serializerOptions);
            }
            else
            {
                return default(O);
            }
        }


        /// <summary>
        /// Default ensure looses the actual message
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = response.Content == null
                    ? ""
                    : await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"{response.StatusCode} (ReasonPhrase: {response.ReasonPhrase}, Content: {responseContent})",
                                                            null, response.StatusCode);

            }
        }
    }
}
