// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.incentive.api.Model
{
    public class UserEventRequest
    {
        /// <summary>
        /// UTC Time when the event originally occured. To be recorded by client app at the time of actual event
        /// </summary>
        [Required]
        public DateTime OriginalTime { get; set; }

        /// <summary>
        /// Content ID. Required only in cases of events dealing with content consumption
        /// </summary>
        public Guid? ContentId {get; set; }

        /// <summary>
        /// Device ID. Required only in cases of events dealing with device/downloads
        /// </summary>
        public string DeviceId { get; set; }
    }
}