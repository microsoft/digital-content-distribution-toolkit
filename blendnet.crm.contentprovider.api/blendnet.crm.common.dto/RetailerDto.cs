using System;
using System.Collections.Generic;

namespace blendnet.crm.common.dto{
    public class RetailerDto :PersonDto
    {
        public List<HubDto> Hubs { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// Resets identifiers
        /// </summary>
        public void ResetIdentifiers()
        {
            this.Id = null;

            if (this.Hubs != null && this.Hubs.Count > 0)
            {
                foreach (HubDto hub in this.Hubs)
                {
                    hub.Id = null;
                }
            }
        }
    }
}