using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.devtokengen.Controllers
{
    public static class Util
    {
        public static async Task<string> FetchToken(string serviceId, string baseUrl, string certificateName, string keyVaultName)
        {
            string requestData = $"?serviceId={serviceId}&appName=com.microsoft.mobile.polymer.mishtu&tls=24"; 
            var url = $"{baseUrl}/v1/GetAccessTokenForPartnerService{requestData}";
            
            string signature = await GetSignedDataFromCertificate(keyVaultName, certificateName, requestData);

            using (var c = new HttpClient())
            {
                c.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", signature);
                var response = await c.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var successResponse = await response.Content.ReadAsStringAsync();
                GetAccessTokenForPartnerServiceResponse tokenResponse = JsonSerializer.Deserialize<GetAccessTokenForPartnerServiceResponse>(successResponse);
                return tokenResponse.accessToken;
            }
        }

        private static async Task<string> GetSignedDataFromCertificate(string keyVaultName,
                                                                      string certificateName,
                                                                      string dataToSign)
        {
            var byteData = Encoding.UTF8.GetBytes(dataToSign);

            X509Certificate2 x509Certificate2 = await CertUtil.GetCert(keyVaultName, certificateName);

            var signature = Convert.ToBase64String(x509Certificate2.GetRSAPrivateKey().SignData(byteData, 
                                                                                                HashAlgorithmName.SHA256, 
                                                                                                RSASignaturePadding.Pkcs1));
            return signature;
        }
    }

    static class CertUtil
    {
        private static Dictionary<string, X509Certificate2> _cache = new Dictionary<string, X509Certificate2>();

        public static async Task<X509Certificate2> GetCert(string keyVaultName, string certificateName)
        {
            var cacheKey = $"{keyVaultName}/{certificateName}";
            if (_cache.ContainsKey(cacheKey))
            {
                return _cache[cacheKey];
            }

            Uri keyVaultUrl = new Uri($"https://{keyVaultName}.vault.azure.net/");
            var c = new DefaultAzureCredential();
            CertificateClient client = new CertificateClient(keyVaultUrl, c);
            X509Certificate2 x509Certificate2 = await client.DownloadCertificateAsync(certificateName);

            _cache[cacheKey] = x509Certificate2;
            return x509Certificate2;
        }
    }
}