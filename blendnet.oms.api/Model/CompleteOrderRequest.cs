using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.oms.api.Model
{
    public class CompleteOrderRequest
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public string userPhoneNumber { get; set; }

        [Required]
        public string RetailerPhoneNumber { get; set; }

        [Required]
        public float AmountCollected { get; set; }

        [Required]
        public string PartnerReferenceNumber { get; set;}

    }
}
