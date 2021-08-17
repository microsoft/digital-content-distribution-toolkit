using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Represents a HUB Device
    /// </summary>
    public class Device:BaseDto
    {
        /// <summary>
        /// Unique Content Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Same as Id
        /// </summary>
        public string DeviceId
        {
            get
            {
                return Id;
            }
        }

        /// <summary>
        /// Same as Id
        /// </summary>
        public DeviceStatus DeviceStatus { get; set; } = DeviceStatus.Registered;

        /// <summary>
        /// Date when the device state is updated
        /// </summary>
        public DateTime? DeviceStatusUpdatedOn { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public DeviceContainerType DeviceContainerType 
        {
            get
            {
                return DeviceContainerType.Device;
            }
        }

        /// <summary>
        /// Current filter update command status
        /// </summary>
        public DeviceCommandStatus? FilterUpdateStatus { get; set; } = DeviceCommandStatus.DeviceCommandNotInitialized;

        /// <summary>
        /// command id which has updated the filter status 
        /// </summary>
        public Guid? FilterUpdateStatusUpdatedBy { get; set; }

        /// <summary>
        /// Command which updated the filter sucessfully
        /// </summary>
        public Guid? FilterUpdatedBy { get; set; }

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceContainerType
    {
        Device = 0,
        Command = 1,
        Content = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceStatus
    {
        Registered = 0,
        Provisioned = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceCommandStatus
    {
        DeviceCommandNotInitialized = 0,
        DeviceCommandSubmitted = 1,
        DeviceCommandInProcess = 2,
        DeviceCommandPushedToDevice = 3,
        DeviceCommandComplete = 4,
        DeviceCommandPushToDeviceFailed = 5,
        DeviceCommandFailed = 6
    }

}
