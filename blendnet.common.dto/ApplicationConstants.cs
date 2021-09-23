using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto
{
    public class ApplicationConstants
    {
        public struct CosmosContainers
        {
            public const string ContentProvider = "ContentProvider";
            public const string ContentProviderPartitionKey = "/contentProviderId";

            public const string Content = "Content";
            public const string ContentPartitionKey = "/contentId";

            public const string Order = "Order";
            public const string OrderPartitionKey = "/phoneNumber";

            public const string User = "User";
            public const string UserPartitionKey = "/phoneNumber";

            public const string WhitelistedUser = "WhitelistedUser";
            public const string WhitelistedUserPartitionKey = "/phoneNumber";

            public const string Retailer = "Retailer";
            public const string RetailerPartitionKey = "/partnerId";

            public const string RetailerProvider = "RetailerProvider";
            public const string RetailerProviderPartitionKey = "/partnerCode";

            public const string IncentivePlan = "IncentivePlan";
            public const string IncentivePlanPartitionKey = "/audience/subTypeName";

            public const string IncentiveEvent = "IncentiveEvent";
            public const string IncentiveEventPartitionKey = "/eventCreatedFor";

            public const string Notification = "Notification";
            public const string NotificationPartitionKey = "/notificationId";

            public const string Device = "Device";
            public const string DevicePartitionKey = "/deviceId";
        }

        public struct Policy
        {
            public const string PolicyPermissions = "rwlcda";
            public const string ReadOnlyPolicyPermissions = "r";
            public const string ReadWriteAllPolicyPermissions = "rwlcda";
        }

        public struct KaizalaIdentityClaims
        {
            public const string AccessToken = "AccessToken";

            public const string PhoneNumber = "PhoneNumber";

            public const string CId = "CId";

            public const string TestSender = "TestSender";

            public const string AppName = "AppName";

            public const string Permissions = "Permissions";

            public const string ApplicationType = "ApplicationType";

            public const string TokenValidFrom = "TokenValidFrom";

            public const string IdentityUId = "IdentityUId";
        }

        public struct BlendNetClaims
        {
            public const string UId = "UId";
        }

        /// <summary>
        /// Identity Roles
        /// </summary>
        public struct KaizalaIdentityRoles
        {
            public const string User = "User";

            public const string Retailer = "Retailer";

            public const string SuperAdmin = "SuperAdmin";

            public const string ContentAdmin = "ContentAdmin";

            public const string RetailerManagement = "RetailerManagement";

            public const string HubDeviceManagement = "HubDeviceManagement";

            public const string HubDevice = "HubDevice";

        }

        /// <summary>
        /// Country Codes
        /// </summary>
        public struct CountryCodes
        {
            public const string India = "+91";
        }

        /// <summary>
        /// Http Client Name
        /// </summary>
        public struct HttpClientKeys
        {
            public const string CMS_HTTP_CLIENT = "cms";
            public const string ORDER_HTTP_CLIENT = "order";
            public const string RETAILER_HTTP_CLIENT = "retailer";
            public const string KAIZALA_HTTP_CLIENT = "kaizala";
            public const string USER_HTTP_CLIENT = "user";
            public const string INCENTIVE_HTTP_CLIENT = "incentive";
            public const string NOTIFICATION_HTTP_CLIENT = "notification";
            public const string SESVODE_HTTP_CLIENT = "ses";
            public const string IOTCENTRAL_HTTP_CLIENT = "iotcentral";
            public const string DEVICE_HTTP_CLIENT = "device";
        }

        public struct StorageContainerPolicyNames
        {
            public const string RawReadOnly = "rawreadonly";
            public const string RawReadWriteAll = "rawreadwriteall";
            public const string MezzanineReadOnly = "mezzaninereadonly";
            public const string ProcessedReadOnly = "processedreadonly";
        }

        public struct StorageContainerSuffix
        {
            public const string Raw = "-raw";
            public const string Mezzanine = "-mezzanine";
            public const string Processed = "-processed";
            public const string Cdn = "-cdn";
        }

        public struct StorageInstanceNames
        {
            public const string CMSStorage = "CMSStorage";
            public const string CMSCDNStorage = "CMSCDNStorage";
            public const string BroadcastStorage = "BroadcastStorage";
        }

        public struct SupportedFileFormats 
        {
            public static List<string> mediaFormats = new List<string>{ "mp4", "wmv", "mpeg","mpd"};
            public static List<string> ThumbnailFormats = new List<string>{"png","jpeg","jpg"};
        }

        public struct AMSJobStatuses
        {
            public const string JobFinished = "Finished";
            public const string JobCanceled = "Canceled";
            public const string JobErrored = "Errored";
        }

        public struct BroadcastJobStatuses
        {
            public const string MOMORDERCREATED = "MOMORDER-CREATED";
            public const string MOMORDERACTIVE = "MOMORDER-ACTIVE";
            public const string MOMORDERCOMPLETED = "MOMORDER-COMPLETED";
            public const string MOMORDERREJECTED = "MOMORDER-REJECTED";
            public const string MOMORDERFAILED = "MOMORDER-FAILED";
            public const string MOMORDERCANCELLED = "MOMORDER-CANCELLED";
        }

        public struct HttpClientNames
        {
            public const string AMS = "AMS";
        }

        /// <summary>
        /// MPD Tokens
        /// </summary>
        public struct MPDTokens
        {
            public const string Bandwidth = "$Bandwidth$";

            public const string Timeline = "$Time$";
        }

        /// <summary>
        /// Folder Names 
        /// </summary>
        public struct DownloadDirectoryNames
        {
            public const string Working = "wrking";

            public const string Final = "fnl";
        }

        public struct DistributedCacheKeySuffix
        {
            public const string SERVICEACCOUNTKEY = "-SVCACCT";
            public const string USERBYPHONEKEY = "-USRBYPH";
        }


        public struct DistributedCacheDurationsInHrs
        {
            public const int Long = 24;
            public const int Medium = 6;
            public const int Short = 1;
        }

        public const string IngestTemplateFileName = "ingest_template.xml";

        public const string BroadcastDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public struct AdaptiveSetTypes
        {
            public const string Audio = "audio";

            public const string Video = "video";
        }


        public struct XMLTokens
        {
            public const string UNIQUE_ID = "{UNIQUE_ID}";
            public const string START_DATE = "{START_DATE}";
            public const string END_DATE = "{END_DATE}";
            public const string AUDIO_TAR = "{AUDIO_TAR}";
            public const string AUDIO_FILE_SIZE = "{AUDIO_FILE_SIZE}";
            public const string AUDIO_FILE_CHECKSUM = "{AUDIO_FILE_CHECKSUM}";
            public const string AUDIO_TAR_FOLDER_NAME = "{AUDIO_TAR_FOLDER_NAME}";
            public const string VIDEO_TAR = "{VIDEO_TAR}";
            public const string VIDEO_FILE_SIZE = "{VIDEO_FILE_SIZE}";
            public const string VIDEO_FILE_CHECKSUM = "{VIDEO_FILE_CHECKSUM}";
            public const string VIDEO_TAR_FOLDER_NAME = "{VIDEO_TAR_FOLDER_NAME}";
            public const string MPD_FILE = "{MPD_FILE}";
            public const string MPD_FILE_SIZE = "{MPD_FILE_SIZE}";
            public const string MPD_FILE_CHECKSUM = "{MPD_FILE_CHECKSUM}";
            public const string CONTENT_ID = "{CONTENT_ID}";
            public const string COMMAND_ID = "{COMMAND_ID}";
            public const string CONTENT_HIERARCHY = "{CONTENT_HIERARCHY}";
            public const string FILTERS = "{FILTERS}";
        }

        /// <summary>
        /// Date and Time Formats
        /// </summary>
        public struct DateTimeFormats
        {
            public const string FormatYYYYMMDD = "yyyyMMdd";
            public const string FormatYYYYMMDDHHmmssfff = "yyyyMMdd HH:mm:ss.fff";
        }
        

        public struct IncentiveEventAdditionalPropertyKeys
        {
            public const string ContentId = "ContentId";
            public const string ContentProviderId = "ContentProviderId";
            public const string UserId = "UserId";
        }

        public struct Common
        {
            public static readonly Guid NIL_GUID = Guid.Empty;

            public const string CONSUMER = "CONSUMER";
        }
        public struct UserProfile
        {
            public const int NameMaxLength = 20;
        }


        public struct HubProxyModuleCommandNames
        {
            public const string FilterUpdate = "FilterUpdate";
        }

        public struct HubProxyModuleClientNames
        {
            public const string HubServer = "HUBServer";
        }

        public struct ValidationRegularExpressions
        {
            public const string AlphaNumeric = "^[a-zA-Z0-9]*$";
        }

        public struct ApplicationInsightsDefaultEventProperty
        {
            public const string BlendNetCustomEvent = "BlendNetCustomEvent";
        }

    }
}
