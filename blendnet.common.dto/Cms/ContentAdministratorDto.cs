using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class ContentAdministratorDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string IdentityProviderId { get; set; }
        
    }
}