using System;
using blendnet.common.dto.AIEvents;

namespace blendnet.retailer.api.Models
{
    public class UnassignDeviceToRetailerAIEvent : BaseAIEvent
    {
        public string PartnerCode { get; set; }

        public string PartnerProvidedId { get; set; }

        public string RetailerId { get; set; }

        public string DeviceId { get; set; }

        public DateTime AssignmentStartDate { get; set; }

        public DateTime AssignmentEndDate { get; set; }
    }
}
