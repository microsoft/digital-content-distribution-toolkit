using System.ComponentModel.DataAnnotations;

namespace blendnet.user.api.Models
{
    public class CreateWhitelistedUserRequest
    {
        [Required]
        public string PhoneNumber { get; set; }

        public string EmailId { get; set; }
        
        public string PartnerCode { get; set; }
        
        public string PartnerProvidedRetailerId { get; set; }
    }

    public class DeleteWhitelistedUserRequest
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
