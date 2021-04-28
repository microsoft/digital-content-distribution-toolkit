using blendnet.common.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy
{
    public class SubscriptionProxy
    {
        private static SubscriptionProxy instance = null;

        private SubscriptionProxy()
        {

        }

        public static SubscriptionProxy Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SubscriptionProxy();
                }

                return instance;
            }
        }


        public ContentProviderSubscriptionDto GetSubscription(Guid contentProviderId, Guid subscriptionId)
        {
            // correct this later
            return new ContentProviderSubscriptionDto();
        }
    }
}
