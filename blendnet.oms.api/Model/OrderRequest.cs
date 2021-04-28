using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Model
{
    public class OrderRequest
    {
        public string PhoneNumber { get; set; }

        public Guid UserId { get; set; }

        public Guid ContentProviderId { get; set; }

        public Guid SubscriptionId { get; set; }
    }
}
