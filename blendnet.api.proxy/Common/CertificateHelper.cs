using Azure.Security.KeyVault.Certificates;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace blendnet.api.proxy.Common
{
    public static class CertificateHelper
    {
        /// <summary>
        /// Signs the data by reading the certificate from keyvault.
        /// Avoid calling this method frequently
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="dataToSign"></param>
        /// <returns></returns>
        public static async Task<string> GetSignedDataFromCertificate(Uri keyVaultUrl,
                                                                      string certificateName,
                                                                      string dataToSign)
        {
            var byteData = Encoding.UTF8.GetBytes(dataToSign);

            CertificateClient client = new CertificateClient(keyVaultUrl, new DefaultAzureCredential());

            X509Certificate2 x509Certificate2 = await client.DownloadCertificateAsync(certificateName);

            var signature = Convert.ToBase64String(x509Certificate2.GetRSAPrivateKey().SignData(byteData, 
                                                                                                HashAlgorithmName.SHA256, 
                                                                                                RSASignaturePadding.Pkcs1));
            return signature;
        }
    }
}
