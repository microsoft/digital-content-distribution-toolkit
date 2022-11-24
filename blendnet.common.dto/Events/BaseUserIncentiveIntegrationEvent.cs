// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Incentive;
using blendnet.common.dto.Integration;
using System;
namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Base class for User Incentive events
    /// </summary>
    public abstract class BaseUserIncentiveIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// Incentive Event
        /// </summary>
        /// <value></value>
        public IncentiveEvent IncentiveEvent { get; set; }
    }
}