using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.infrastructure.ServiceBus
{
    /// <summary>
    /// Connection Details for Service Bus
    /// </summary>
    public class EventBusConnectionData
    {
        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public int MaxConcurrentCalls { get; set; } = 5;

    }
}
