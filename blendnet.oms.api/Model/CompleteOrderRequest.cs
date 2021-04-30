using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Model
{
    public class CompleteOrderRequest
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid RetailerId { get; set; }

        [Required]
        public float AmountCollected { get; set; }

        [Required]
        public string PartnerReferenceNumber { get; set;}

        public DateTime PaymentDate { get; set; }
    }
}
