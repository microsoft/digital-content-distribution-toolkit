using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Class to capture Data Export requests from end users
    /// </summary>
    public class UserDataExportCommand : BaseDto
    {
        /// <summary>
        /// Request ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Phone number of user who requested
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Type
        /// </summary>
        public UserContainerType Type => UserContainerType.UserDataExportCommand;

        /// <summary>
        /// Status
        /// </summary>
        public DataExportRequestStatus Status { get; set; }

        /// <summary>
        /// Data export Result
        /// </summary>
        public UserDataExportResult Result { get; set; }
    }

    /// <summary>
    /// Class to store the Data Export Result
    /// </summary>
    public class UserDataExportResult
    {
        /// <summary>
        /// Date of completion
        /// </summary>
        public DateTime DateCompleted { get; set; }

        /// <summary>
        /// URL to exported data
        /// </summary>
        public string ExportedDataUrl { get; set; }

        /// <summary>
        /// Validity (end) date of exported data being available
        /// </summary>
        public DateTime ExportedDataValidity { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataExportRequestStatus
    {
        Active = 0,
        Completed = 1,
    }
}