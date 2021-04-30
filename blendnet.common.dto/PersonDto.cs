using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace blendnet.common.dto
{
    /// <summary>
    /// Person DTO.
    /// </summary>
    public class PersonDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public AddressDto Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string IdentityProviderId { get; set; }

        public string GetName()
        {
            string name = FirstName;

            if(MiddleName != null)
            {
                name = name + " " + MiddleName;
            }

            if(LastName != null)
            {
                name = name + " " + LastName;
            }

            return name;
        }
    }
}
