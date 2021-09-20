using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Retailer
{
    /// <summary>
    /// Device assignment record for retailer
    /// </summary>
    public class RetailerDeviceAssignment
    {
        /// <summary>
        /// Device ID
        /// </summary>
        [Required]
        public string DeviceId { get; set; }

        /// <summary>
        /// Assignment start date
        /// </summary>
        [Required]
        public DateTime AssignmentStartDate { get; set; }

        /// <summary>
        /// Assignment end date
        /// </summary>
        [Required]
        public DateTime AssignmentEndDate { get; set; }

        /// <summary>
        /// Tells whether the current record is active, as per the input date
        /// </summary>
        /// <param name="dateTime">date for checking active (defaults to current time)</param>
        /// <returns></returns>
        public bool IsActive(DateTime? dateTime = null)
        {
            if (!dateTime.HasValue)
            {
                dateTime = DateTime.UtcNow;
            }

            return AssignmentStartDate <= dateTime.Value && dateTime.Value <= AssignmentEndDate;
        }
    }
}