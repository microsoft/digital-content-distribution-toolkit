using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.cms.listener.Model
{
    public class AppSettings
    {
        public string AccountEndPoint { get; set; }
        
        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string  ServiceBusTopicName { get; set; }

        public string ServiceBusSubscriptionName { get; set; }

        public int ServiceBusMaxConcurrentCalls { get; set; }
        
        public string CMSStorageConnectionString { get; set; }

        public string ContentAdministratorGroupId { get; set; }
    }
}
