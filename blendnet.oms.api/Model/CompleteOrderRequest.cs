using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.oms.api.Model
{
    public class CompleteOrderRequest
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public string UserPhoneNumber { get; set; }

        [Required]
        public string RetailerPartnerProvidedId { get; set; }

        [Required]
        public float AmountCollected { get; set; }

        [Required]
        public string PartnerReferenceNumber { get; set;}

        [Required]
        public DateTime DepositDate { get; set; }

    }
}
