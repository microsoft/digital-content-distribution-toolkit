using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class ContentProviderDto
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<ContentAdministratorDto> ContentAdministrators { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }

        public string LogoUrl { get; set; }

        /// <summary>
        /// Resets identifiers
        /// </summary>
        public void SetIdentifiers()
        {
            this.Id = Guid.NewGuid();

            if (this.ContentAdministrators != null && this.ContentAdministrators.Count > 0)
            {
                foreach (ContentAdministratorDto contentAdministrator in this.ContentAdministrators)
                {
                    contentAdministrator.Id = Guid.NewGuid();
                }
            }
        }
    }
}