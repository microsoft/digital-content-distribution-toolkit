// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Device App Settings
    /// </summary>
    public class DeviceAppSettings
    {
        public string AccountEndPoint { get; set; }

        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string ServiceBusTopicName { get; set; }

        public Dictionary<string, string> KaizalaIdentity { get; set; }

        public string CloudApiServiceAccountCertName { get; set; }

        public string KaizalaIdentityAppName { get; set; }

        /// <summary>
        /// IOT Central Authentication Token
        /// </summary>
        public string IOTCAPIToken { get; set; }
        
        /// <summary>
        /// IOT Central API Version
        /// </summary>
        public string IOTCAPIVersion { get; set; }

        /// <summary>
        /// IOT Central API Connection Timeout in Secs
        /// </summary>
        public int IOTCAPIConnectionTimeOutInSecs { get; set; }

        /// <summary>
        /// IOT Central API Response Timeout in Secs
        /// </summary>
        public int IOTCAPIResponseTimeOutInSecs { get; set; }

        /// <summary>
        /// IOT Central Proxy Module Name
        /// </summary>
        public string IOTCAPIProxyModuleName { get; set; }

        /// <summary>
        /// IOT Central Proxy Command Name
        /// </summary>
        public string IOTCAPIProxyCommandName { get; set; }

        /// <summary>
        /// Timeout for call from Proxy to Hub for command execution
        /// </summary>
        public int IOTCAPIProxyTimeOutInMts { get; set; }

        /// <summary>
        /// IOT Central Base Url
        /// </summary>
        public string IOTCAPIBaseUrl { get; set; }

        /// <summary>
        /// Custom Message Property Name. This is the name set by IOT central while sending the message to Azure Service Bus
        /// </summary>
        public string IOTCentralPropertyName { get; set; }

        /// <summary>
        /// Custom Message Property Value. This is the value set by IOT central while sending the message to Azure Service Bus
        /// </summary>
        public string IOTCentralPropertyValue { get; set; }

        /// <summary>
        /// CMS Base URL
        /// </summary>
        public string CmsBaseUrl { get; set; }

    }
}
