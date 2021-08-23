using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class DeviceCommand:BaseDto
    {
        /// <summary>
        /// Unique Command Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Device Id
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Device Container Type
        /// </summary>
        public DeviceContainerType DeviceContainerType 
        {
            get
            {
                return DeviceContainerType.Command;
            }
        }

        /// <summary>
        /// Failure Details
        /// </summary>
        public List<string> FailureDetails { get; set; }

        /// <summary>
        /// Device Command Execution Details
        /// </summary>
        public List<DeviceCommandExecutionDetails> ExecutionDetails { get; set; }

        /// <summary>
        /// Device Command Type
        /// </summary>
        public DeviceCommandType DeviceCommandType { get; set; }
                
        /// <summary>
        /// Command Status
        /// </summary>
        public DeviceCommandStatus DeviceCommandStatus { get; set; }

        /// <summary>
        /// Details about Filter Update Request
        /// </summary>
        public FilterUpdateRequest FilterUpdateRequest { get; set; }
    }


    /// <summary>
    /// Device Command Execution Details
    /// </summary>
    public class DeviceCommandExecutionDetails
    {
        public string EventName { get; set; }

        public DateTime EventDateTime { get; set; }

    }

    /// <summary>
    /// Command Type
    /// Added the attribute so that cosmos uses string for enums
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceCommandType
    {
        FilterUpdate = 0,
        ForceDelete = 1
    }

    /// <summary>
    /// Filter Update Request
    /// </summary>

    public class FilterUpdateRequest
    {
        public List<string> Filters { get; set; }
    }
}
