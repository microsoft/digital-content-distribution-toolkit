using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.Model
{
    /// <summary>
    /// Content Administrator
    /// </summary>
    public class ContentAdministrator:Person
    {
        public Guid? ContentProviderId { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime? ActivationDate { get; set; }
        
        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// Reset Identifiers
        /// </summary>
        public void ResetIdentifiers()
        {
            this.Id = null;
        }

    }
}
