using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Content Command
    /// </summary>
    public class ContentCommand
    {
        /// <summary>
        /// Unique Command Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Content Id
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// To be populated only in case of Delete
        /// </summary>
        public Content Content { get; set; }

        /// <summary>
        /// Content Container Type
        /// </summary>
        public ContentContainerType Type { get; set; } = ContentContainerType.Command;

        /// <summary>
        /// Content Command Type
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Modified Date
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Failure Details
        /// </summary>
        public List<string> FailureDetails { get; set; }

        /// <summary>
        /// Command Status
        /// </summary>
        public CommandStatus CommandStatus { get; set; }

        /// <summary>
        /// Details about broadcast data
        /// </summary>
        public BroadcastRequest BroadcastRequest { get; set; }

        /// <summary>
        /// Execution Details
        /// </summary>
        public List<CommandExecutionDetails> ExecutionDetails { get; set; } = new List<CommandExecutionDetails>();
    }

    /// <summary>
    /// Command Type
    /// Added the attribute so that cosmos uses string for enums
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandType
    {
        UploadContent = 0,
        DeleteContent = 1,
        TransformContent = 2,
        BroadcastContent = 3,
    }

    /// <summary>
    /// Command Status
    /// Added the attribute so that cosmos uses string for enums
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandStatus
    {
        InProgress = 0,
        Complete = 1,
        Failed = 2
    }

    public class CommandExecutionDetails
    {
        public string EventName { get; set; }

        public ContentBroadcastStatus ContentBroadcastStatus { get; set; }

        public DateTime EventDateTime { get; set; }

    }
}
