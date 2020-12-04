using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class ContentProviderDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ContainerBaseName { get; set; }

        public AddressDto Address { get; set; }

        public List<ContentAdministratorDto> ContentAdministrators { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// Resets identifiers
        /// </summary>
        public void ResetIdentifiers()
        {
            this.Id = null;

            if (this.ContentAdministrators != null && this.ContentAdministrators.Count > 0)
            {
                foreach (ContentAdministratorDto contentAdministrator in this.ContentAdministrators)
                {
                    contentAdministrator.Id = null;
                }
            }
        }
    }
}