using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.bdd.Drivers
{
    public class HttpClientDriver
    {
        HttpClient _httpClient;

        public HttpClientDriver()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
            
        public async Task<HttpClientResponse<T>> Get<T>(string url)
        {
            HttpClientResponse<T> httpClientResponse = new HttpClientResponse<T>();

            var result = await _httpClient.GetAsync(url);

            httpClientResponse.RawMessage = result;

            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();

                httpClientResponse.Data = JsonSerializer.Deserialize<T>(resultContent);
            }
            else
            {
                httpClientResponse.Data =  default(T);
            }

            return httpClientResponse;
        }

        public async Task<HttpClientResponse<O>> Post<I,O>(string url, I payLoad)
        {
            HttpClientResponse<O> httpClientResponse = new HttpClientResponse<O>();

            string jsonData = System.Text.Json.JsonSerializer.Serialize(payLoad);

            StringContent requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync(url, requestContent);

            httpClientResponse.RawMessage = result;

            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();

                httpClientResponse.Data = JsonSerializer.Deserialize<O>(resultContent);
            }
            else
            {
                httpClientResponse.Data = default(O);
            }

            return httpClientResponse;
        }

    }
}
