
using blendnet.common.dto.Cms;
using blendnet.common.dto.Oms;
using System.Collections.Generic;

namespace blendnet.oms.api.Model
{
    public class CreatedOrderResponse
    {
        public Order Order { get; set; }  

        // List of content which is not present in retailer device for an order
        public List<ContentInfo> Contents { get; set; }
    }
}
