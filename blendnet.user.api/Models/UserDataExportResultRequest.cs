using System;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Request object for updating User Data Export Results
    /// </summary>
    public class UserDataExportResultRequest
    {
        /// <summary>
        /// phone number of user whose export request is fulfilled
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// URL of the exported data file
        /// </summary>
        public string ExportedDataUrl { get; set; }

        /// <summary>
        /// Validity (end) date of the exported data
        /// </summary>
        public DateTime ExportedDataValidity { get; set; }
    }
}