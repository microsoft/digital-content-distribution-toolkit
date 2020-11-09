

using System;

namespace blendnet.crm.common.dto
{
    public class HubDto
    {
        public Guid? RetailerId { get; set; }

        public Guid? Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }

        public AddressDto Address { get; set; }

        /// <summary>
        /// Reset Identifiers
        /// </summary>
        public void ResetIdentifiers()
        {
            this.Id = null;
        }
    }
}