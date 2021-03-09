using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class ContentAdministratorDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
               
        public string Email { get; set; }

        public long Mobile { get; set; }

        [Required]
        public string IdentityProviderId { get; set; }
        
    }
}