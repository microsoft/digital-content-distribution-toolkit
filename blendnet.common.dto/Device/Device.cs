// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Represents a HUB Device
    /// </summary>
    public class Device:BaseDto
    {
        private string _id;
        /// <summary>
        /// Unique Content Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumeric_ErrorCode)]
        public string Id 
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value.ToUpper();
            }
        }

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
        public FilterUpdatedBy FilterUpdatedBy { get; set; }

        /// <summary>
        /// Sets the default values
        /// </summary>
        public void SetDefaults()
        {
            this.ModifiedByByUserId = null;

            this.ModifiedDate = null;

            this.FilterUpdatedBy = null;

            this.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandNotInitialized;

            this.FilterUpdateStatusUpdatedBy = null;
        }

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceContainerType
    {
        Device = 0,
        Command = 1,
        Content = 2,
        DeviceContent = 3
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
        DeviceCommandFailed = 6,
        DeviceCommandCancelled = 7,
    }

}
