using blendnet.common.dto.cms;
using blendnet.common.infrastructure.Ams;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace blendnet.cms.api.Common
{
    /// <summary>
    /// Ams Helper Class to Generate the Token
    /// </summary>
    public class AmsHelper
    {
        private readonly ILogger _logger;

        AppSettings _appSettings;

        public AmsHelper(ILogger<AmsHelper> logger,
                        IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;

            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Generates the token for the Content and Command Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public async Task<string> GetContentToken(Guid contentId, Guid commandId)
        {
            AmsData amsData = new AmsData();

            amsData.AmsAccountName = _appSettings.AmsAccountName;
            amsData.AmsArmEndPoint = _appSettings.AmsArmEndPoint;
            amsData.AmsClientId = _appSettings.AmsClientId;
            amsData.AmsClientSecret = _appSettings.AmsClientSecret;
            amsData.AmsResourceGroupName = _appSettings.AmsResourceGroupName;
            amsData.AmsSubscriptionId = _appSettings.AmsSubscriptionId;
            amsData.AmsTenantId = _appSettings.AmsTenantId;
            amsData.AmsTokenAudience = _appSettings.AmsTokenAudience;
            amsData.AmsTokenExpiryInMts = _appSettings.AmsTokenExpiryInMts;
            amsData.AmsTokenIssuer = _appSettings.AmsTokenIssuer;
            amsData.AmsTokenSigningKey = _appSettings.AmsTokenSigningKey;

            string token = await AmsUtilities.GetContentToken(amsData, contentId, commandId);

            return token;
        }

    }
}
