using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Model
{
    public class OrderRequest
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ContentProviderId { get; set; }

        [Required]
        public Guid SubscriptionId { get; set; }
    }
}
