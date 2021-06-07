using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Oms
{
    /// <summary>
    /// OMS Service Application Settings
    /// </summary>
    public class OmsAppSettings
    {
        public string AccountEndPoint { get; set; }

        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public string CmsBaseUrl { get; set; }

        public string AmsAccountName { get; set; }

        public string AmsArmEndPoint { get; set; }

        public string AmsClientId { get; set; }

        public string AmsClientSecret { get; set; }

        public string AmsResourceGroupName { get; set; }

        public string AmsSubscriptionId { get; set; }

        public string AmsTenantId { get; set; }

        public string AmsTokenAudience { get; set; }

        public string AmsTokenIssuer { get; set; }

        public int AmsTokenExpiryInMts { get; set; }

        public string AmsTokenSigningKey { get; set; }
    }
}
