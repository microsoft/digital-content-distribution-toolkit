using System.ComponentModel.DataAnnotations;

namespace blendnet.retailer.api.Models
{
    public class UnassignDeviceRequest
    {
        [Required]
        public string PartnerCode { get; set; }

        [Required]
        public string PartnerProvidedRetailerId { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}