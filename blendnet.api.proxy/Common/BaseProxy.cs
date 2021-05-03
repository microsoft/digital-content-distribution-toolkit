using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Common
{
    /// <summary>
    /// Base Proxy.
    /// </summary>
    public class BaseProxy
    {
        private IConfiguration _configuration;

        public BaseProxy(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get Access Token for service Account
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetServiceAccessToken()
        {
            Uri keyVaultUrl = new Uri($"https://{_configuration["KeyVaultName"]}.vault.azure.net/");

            string certificateName = _configuration["CloudApiServiceAccountCertName"];

            string dataToSign = "";

            string signedData = await CertificateHelper.GetSignedDataFromCertificate(keyVaultUrl, certificateName, dataToSign);

            return "";
        }
    }
}
