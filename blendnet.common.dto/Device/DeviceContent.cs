using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Stores device content details
    /// </summary>
    public class DeviceContent : BaseDto
    {
        /// <summary>
        /// Content id
        /// </summary>
        [JsonProperty(PropertyName ="id")]
        public Guid ContentId { get; set; }

        /// <summary>
        /// device id from device table
        /// </summary>
        public string DeviceId { get; set; }

        public DeviceContainerType DeviceContainerType
        {
            get
            {
                return DeviceContainerType.DeviceContent;
            }
        }

        /// <summary>
        /// Content provider id
        /// </summary>
        public Guid? ContentProviderId { get; set; }

        /// <summary>
        /// Is content deleted status
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Timestamp during which operation was executed from device
        /// </summary>
        public DateTime OperationTimeStamp { get; set; }
    }
}
