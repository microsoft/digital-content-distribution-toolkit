using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto
{
    public class ApplicationConstants
    {
        public struct CosmosContainers
        {
            public const string ContentProvider = "ContentProvider";
            public const string Content = "Content";
        }

        public struct Policy
        {
            public const string PolicyPermissions = "rwlcda";
            public const string ReadOnlyPolicyPermissions = "r";
            public const string ReadWriteAllPolicyPermissions = "rwlcda";
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
    }
}
