using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.cms
{
    /// <summary>
    /// App Settings
    /// </summary>
    public class AppSettings
    {
        public string AccountEndPoint { get; set; }
        
        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string ServiceBusTopicName { get; set; }

        public string CMSStorageConnectionString { get; set; }

        public string CMSCDNStorageConnectionString { get; set; }
        
        public int SasTokenExpiryForContentProviderInMts { get; set; }

        public string ContentAdministratorGroupId { get; set; }

        public int SASTokenExpiryToCopyContentInMts { get; set; }

        public string AmsClientId { get; set; }

        public string AmsClientSecret { get; set; }

        public string AmsTenantId { get; set; }

        public string AmsAccountName { get; set; }

        public string AmsResourceGroupName { get; set; }

        public string AmsSubscriptionId { get; set; }

        public string AmsTransformationName { get; set; }

        public string AmsArmAadAudience { get; set; }

        public string AmsArmEndPoint { get; set; }

        public int AmsAacAudioChannels { get; set; }
        
        public int AmsAacAudioSamplingRate { get; set; }
        
        public int AmsAacAudioBitRate { get; set; }

        public int AmsH264VideoKeyFrameworkIntervalInSec { get; set; }

        public int AmsH264LayerBitRate { get; set; }

        public int AmsH264LayerWidth { get; set; }

        public int AmsH264LayerHeight { get; set; }
        
        public string AmsH264LayerLabel { get; set; }

        public int SASTokenExpiryForAmsJobInMts { get; set; }

    }
}
