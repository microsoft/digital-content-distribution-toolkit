using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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
        /// Tells whether the current record is an active assignment
        /// </summary>
        [JsonIgnore] // do not put in DB
        public bool IsActive => AssignmentStartDate <= DateTime.UtcNow && DateTime.UtcNow <= AssignmentEndDate;
    }
}