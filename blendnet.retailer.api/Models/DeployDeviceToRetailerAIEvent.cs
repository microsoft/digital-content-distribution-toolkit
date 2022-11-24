// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using blendnet.common.dto.AIEvents;

namespace blendnet.retailer.api.Models
{
    public class DeployDeviceToRetailerAIEvent : BaseAIEvent
    {
        public string PartnerCode { get; set; }

        public string PartnerProvidedId { get; set; }

        public string RetailerId { get; set; }

        public string DeviceId { get; set; }

        public DateTime DeploymentDate { get; set; }
    }
}
