using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// User Id of user who requested
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Phone number of user from whom data needs to be exported
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
        /// Execution Details
        /// </summary>
        public List<DataExportCommandExecutionDetails> ExecutionDetails { get; set; } = new List<DataExportCommandExecutionDetails>();

        /// <summary>
        /// Details 
        /// </summary>
        public List<DataExportByEachServiceDetails> DataExportByEachServiceDetails { get; set; } = new List<DataExportByEachServiceDetails>();

        /// <summary>
        /// Data Export Result
        /// </summary>
        public DataExportResult DataExportResult { get; set; }

        /// <summary>
        /// Is Export Complete
        /// </summary>
        /// <returns></returns>
        public bool IsExportComplete ()
        {
            string[] statusesToLookFor = new string[] { ApplicationConstants.BlendNetServices.OMSService,
                                                        ApplicationConstants.BlendNetServices.IncentiveService,
                                                        ApplicationConstants.BlendNetServices.UserService};

            string[] completedServices = DataExportByEachServiceDetails.Select(s => s.CompletedByService).ToArray();

            Array.Sort(statusesToLookFor);

            Array.Sort(completedServices);

            return statusesToLookFor.SequenceEqual(completedServices);

        }

    }

    

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataExportRequestStatus
    {
        NotInitialized = 0,
        Submitted = 1,
        ExportInProgress = 2,
        Exported = 3,
        ExportedDataNotified = 4,
    }

    /// <summary>
    /// Data Export Command Execution Details
    /// </summary>
    public class DataExportCommandExecutionDetails
    {
        public DataExportRequestStatus DataExportRequestStatus { get; set; }

        public DateTime EventDateTime { get; set; }

    }

    /// <summary>
    /// Data ExportCompletion Details
    /// </summary>
    public class DataExportByEachServiceDetails
    {
        public string CompletedByService { get; set; }

        public DateTime CompletionDateTime { get; set; }

        public bool NoDataToExport { get; set; }

    }

    /// <summary>
    /// Data Export Result
    /// </summary>
    public class DataExportResult
    {
        /// </summary>
        /// <summary>
        /// URL to exported data
        /// </summary>
        public string ExportedDataUrl { get; set; }

        /// <summary>
        /// Expiry
        /// </summary>
        public DateTime ExpiresOn { get; set; }

        /// <summary>
        /// In case any error sending push notification
        /// </summary>
        public bool NotificationSent { get; set; }
    }

}