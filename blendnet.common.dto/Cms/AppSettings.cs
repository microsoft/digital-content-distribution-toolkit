﻿using System;
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
    }
}
