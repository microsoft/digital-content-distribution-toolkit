using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.Model
{
    /// <summary>
    /// Content Provider
    /// </summary>
    public class ContentProvider
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Address Address { get; set; }

        public List<ContentAdministrator> ContentAdministrators { get; set; } 

        public bool IsActive { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// Resets identifiers
        /// </summary>
        public void ResetIdentifiers()
        {
            this.Id = null;

            if (this.ContentAdministrators !=null && this.ContentAdministrators.Count > 0)
            {
                foreach(ContentAdministrator contentAdministrator in this.ContentAdministrators)
                {
                    contentAdministrator.Id = null;
                }
            }
        }
    }
}
