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

        public bool PerformCopyToBroadcastStorage { get; set; }

        public string BroadcastStorageConnectionString { get; set; }

        public string BroadcastStorageContainerName { get; set; }

        public int SasTokenExpiryForContentProviderInMts { get; set; }

        public int SASTokenExpiryToCopyContentInMts { get; set; }

        public string AmsClientId { get; set; }

        public string AmsClientSecret { get; set; }

        public string AmsTenantId { get; set; }

        public string AmsAccountName { get; set; }

        public string AmsResourceGroupName { get; set; }

        public string AmsSubscriptionId { get; set; }

        public string AmsTransformationName { get; set; }

        public string AmsContentPolicyName { get; set; }

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

        public string AmsEventGridPropertyName { get; set; }

        public string AmsEventGridSubscriptionName { get; set; }

        public string AmsTokenSigningKey { get; set; }

        public string AmsTokenIssuer { get; set; }
        
        public string AmsTokenAudience { get; set; }

        public bool AmsWidevineCanPlay { get; set; }

        public bool AmsWidevineCanPersist { get; set; }
        
        public bool AmsWidevineCanRenew { get; set; }

        public int AmsWidevineRentalDurationSeconds { get; set; }

        public int AmsWidevinePlaybackDurationSeconds { get; set; }

        public int AmsWidevineLicenseDurationSeconds { get; set; }

        public int SASTokenExpiryForAmsJobInMts { get; set; }

        public string AmsStreamingEndpointName { get; set; }

        public int AmsTokenExpiryInMts { get; set; }
        
        public int AmsTokenMaxUses { get; set; }

        public string IngestFileName { get; set; }

        public int HttpHandlerLifeTimeInMts { get; set; }

        public int HttpClientRetryCount { get; set; }

        public Dictionary<string,string> KaizalaIdentity { get; set; }
        
        public string CloudApiServiceAccountCertName { get; set; }

        public string KaizalaIdentityAppName { get; set; }

        public string SESServiceBaseUrl { get; set; }

        public string SESServiceUser { get; set; }

        public string SESServicePwd { get; set; }

        public int SubscriptionContentListMaxLimit { get; set; }

        public int MinDaysContentExistInSubscription { get; set; }

    }
}
