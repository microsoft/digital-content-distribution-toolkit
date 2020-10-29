using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.bdd.Drivers
{
    /// <summary>
    /// Provides the helper methods to make HTTP Calls
    /// </summary>
    public class HttpClientDriver
    {
        HttpClient _httpClient;

        public HttpClientDriver()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        /// <summary>
        /// Makes the GET call to return the data from given URL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpClientResponse<T>> Get<T>(string url)
        {
            HttpClientResponse<T> httpClientResponse = new HttpClientResponse<T>();

            var result = await _httpClient.GetAsync(url);

            httpClientResponse.RawMessage = result;

            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();

                httpClientResponse.Data = JsonSerializer.Deserialize<T>(resultContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                httpClientResponse.Data =  default(T);
            }

            return httpClientResponse;
        }

        /// <summary>
        /// Makes the POST Call
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="url"></param>
        /// <param name="payLoad"></param>
        /// <returns></returns>
        public async Task<HttpClientResponse<O>> Post<I,O>(string url, I payLoad)
        {
            HttpClientResponse<O> httpClientResponse = new HttpClientResponse<O>();

            HttpResponseMessage result;
            
            if (payLoad != null)
            {
                string jsonData = System.Text.Json.JsonSerializer.Serialize(payLoad);

                StringContent requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

                result = await _httpClient.PostAsync(url, requestContent);
            }
            else
            {
                result = await _httpClient.PostAsync(url,null);
            }

            httpClientResponse.RawMessage = result;

            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(resultContent))
                {
                    httpClientResponse.Data = JsonSerializer.Deserialize<O>(resultContent);
                }
            }
            else
            {
                httpClientResponse.Data = default(O);
            }

            return httpClientResponse;
        }

        /// <summary>
        /// Makes the delete call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpClientResponse<T>> Delete<T>(string url)
        {
            HttpClientResponse<T> httpClientResponse = new HttpClientResponse<T>();

            var result = await _httpClient.DeleteAsync(url);

            httpClientResponse.RawMessage = result;

            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(resultContent))
                {
                    httpClientResponse.Data = JsonSerializer.Deserialize<T>(resultContent);
                }
            }
            else
            {
                httpClientResponse.Data = default(T);
            }

            return httpClientResponse;
        }

    }
}
