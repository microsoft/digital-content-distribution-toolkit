using System;

namespace blendnet.common.dto{
    public class ContentAdministratorDto:PersonDto
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