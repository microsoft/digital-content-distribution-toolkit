// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Class to capture Command
    /// </summary>
    public class UserCommand : BaseDto
    {
        private UserCommandType _userCommandType;

        public UserCommand(UserCommandType userCommandType)
        {
            _userCommandType = userCommandType;
        }

        /// <summary>
        /// User Command type
        /// </summary>
        public UserCommandType UserCommandType
        {
            get
            {
                return _userCommandType;
            }
            set
            {
                _userCommandType = value;
            }
        }

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
        public UserContainerType Type => UserContainerType.Command;

        /// <summary>
        /// Status
        /// </summary>
        public DataExportRequestStatus DataExportRequestStatus { get; set; }

        /// <summary>
        /// Data Export Result
        /// </summary>
        public DataExportResult DataExportResult { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public DataUpdateRequestStatus DataUpdateRequestStatus { get; set; }

        /// <summary>
        /// Execution Details
        /// </summary>
        public List<CommandExecutionDetails> ExecutionDetails { get; set; } = new List<CommandExecutionDetails>();

        /// <summary>
        /// Details 
        /// </summary>
        public List<StatusByEachServiceDetails> StatusByEachServiceDetails { get; set; } = new List<StatusByEachServiceDetails>();


        /// <summary>
        /// If the user to which Command is attached is deleted
        /// </summary>
        public bool IsUserDeleted { get; set; }

        /// <summary>
        /// Is Command Complete
        /// </summary>
        /// <returns></returns>
        public bool IsCommandComplete()
        {
            string[] statusesToLookFor = new string[] { ApplicationConstants.BlendNetServices.OMSService,
                                                        ApplicationConstants.BlendNetServices.IncentiveService,
                                                        ApplicationConstants.BlendNetServices.UserService};

            string[] completedServices = StatusByEachServiceDetails.Select(s => s.CompletedByService).ToArray();

            Array.Sort(statusesToLookFor);

            Array.Sort(completedServices);

            return statusesToLookFor.SequenceEqual(completedServices);

        }

        /// <summary>
        /// Checks if any command failed
        /// </summary>
        /// <returns></returns>
        public bool IsAnyCommandFailed()
        {
            return StatusByEachServiceDetails.Where(s=>s.IsFailed).Any();
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataUpdateRequestStatus
    {
        NotInitialized = 0,
        Submitted = 1,
        UpdateInProgress = 2,
        Updated = 3,
        Failed = 4,
    }


    /// <summary>
    /// Command Execution Details
    /// </summary>
    public class CommandExecutionDetails
    {
        public string EventName { get; set; }

        public DateTime EventDateTime { get; set; }

    }

    /// <summary>
    /// Status of each service
    /// </summary>
    public class StatusByEachServiceDetails
    {
        public string CompletedByService { get; set; }

        public DateTime CompletionDateTime { get; set; }

        public bool NoDataToOperate { get; set; }

        public bool IsFailed { get; set; }

        public string FailureReason { get; set; }

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

    /// <summary>
    /// Command Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserCommandType
    {
        Export = 0,
        Update = 1
    }

}