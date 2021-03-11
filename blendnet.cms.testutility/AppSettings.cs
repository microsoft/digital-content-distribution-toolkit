using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.cms.testutility
{
    public class AppSettings
    {
        private readonly IConfiguration _config;

        public AppSettings(IConfiguration config)
        {
            _config = config;
        }

        public string XmlFileName
        {
            get { return _config["XmlFileName"]; }
        }

        public string SubscriptionId
        {
            get { return _config["SubscriptionId"]; }
        }

        public string ResourceGroup
        {
            get { return _config["ResourceGroup"]; }
        }

        public string AccountName
        {
            get { return _config["AccountName"]; }
        }

        public string AadTenantId
        {
            get { return _config["AadTenantId"]; }
        }

        public string AadClientId
        {
            get { return _config["AadClientId"]; }
        }

        public string AadSecret
        {
            get { return _config["AadSecret"]; }
        }

        public Uri ArmAadAudience
        {
            get { return new Uri(_config["ArmAadAudience"]); }
        }

        public Uri AadEndpoint
        {
            get { return new Uri(_config["AadEndpoint"]); }
        }

        public Uri ArmEndpoint
        {
            get { return new Uri(_config["ArmEndpoint"]); }
        }

        public string Location
        {
            get { return _config["Location"]; }
        }

        public string SymmetricKey
        {
            get { return _config["SymmetricKey"]; }
        }

        public string EventHubConnectionString
        {
            get { return _config["EventHubConnectionString"]; }
        }

        public string EventHubName
        {
            get { return _config["EventHubName"]; }
        }

        public string StorageContainerName
        {
            get { return _config["StorageContainerName"]; }
        }

        public string StorageAccountName
        {
            get { return _config["StorageAccountName"]; }
        }

        public string StorageAccountKey
        {
            get { return _config["StorageAccountKey"]; }
        }

        public string BineTransformName
        {
            get { return _config["BineTransformName"]; }
        }

        /// <summary>
        /// Key Frame Interval
        /// </summary>
        public int KeyFrameInterval
        {
            get { return System.Convert.ToInt32( _config["KeyFrameInterval"]); }
        }

        /// <summary>
        /// Bitrate
        /// </summary>
        public int Bitrate
        {
            get { return System.Convert.ToInt32(_config["Bitrate"]); }
        }

        /// <summary>
        /// Width
        /// </summary>
        public int Width
        {
            get { return System.Convert.ToInt32(_config["Width"]); }
        }


        /// <summary>
        /// Height
        /// </summary>
        public int Height
        {
            get { return System.Convert.ToInt32(_config["Height"]); }
        }

        /// <summary>
        /// Label
        /// </summary>
        public string Label
        {
            get { return _config["Label"]; }
        }

        public string TokenIssuer
        {
            get { return _config["TokenIssuer"]; }
        }

        public string TokenAudience
        {
            get { return _config["TokenAudience"]; }
        }

        public string BineContentKeyPolicyName
        {
            get { return _config["BineContentKeyPolicyName"]; }
        }

        

    }

    public struct XMLConstants
    {
        public const string UNIQUE_ID = "{UNIQUE_ID}";
        public const string START_DATE = "{START_DATE}";
        public const string END_DATE = "{END_DATE}";
        public const string MOVIE_NAME ="{MOVIE_NAME}";
        public const string AUDIO_TAR = "{AUDIO_TAR}";
        public const string AUDIO_FILE_SIZE = "{AUDIO_FILE_SIZE}";
        public const string AUDIO_FILE_CHECKSUM = "{AUDIO_FILE_CHECKSUM}";
        public const string VIDEO_TAR = "{VIDEO_TAR}";
        public const string VIDEO_FILE_SIZE = "{VIDEO_FILE_SIZE}";
        public const string VIDEO_FILE_CHECKSUM = "{VIDEO_FILE_CHECKSUM}";
        public const string MPD_FILE = "{MPD_FILE}";
        public const string MPD_FILE_SIZE = "{MPD_FILE_SIZE}";
        public const string MPD_FILE_CHECKSUM = "{MPD_FILE_CHECKSUM}";
    }
}

