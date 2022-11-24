// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;
using blendnet.common.dto.Oms;

namespace blendnet.common.dto.Events
{
    public class OrderCompletedIntegrationEvent : IntegrationEvent
    {
        public Order Order { get; set; }
    }
}
