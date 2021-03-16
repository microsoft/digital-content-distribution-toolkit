using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Content Upload Command
    /// </summary>
    public class ContentUploadCommand
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
        /// Content Container Type
        /// </summary>
        public ContentContainerType Type { get; set; }

        /// <summary>
        /// Content Command Type
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Failure Details
        /// </summary>
        public List<string> FailureDetails { get; set; }
    }

    /// <summary>
    /// Command Type
    /// </summary>
    public enum CommandType
    {
        UploadContent = 0,
        DeleteContent = 0,
        TransformContent = 1,
        BroadcastContent = 2,
    }
}
