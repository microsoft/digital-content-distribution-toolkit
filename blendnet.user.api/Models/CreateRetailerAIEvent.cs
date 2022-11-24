// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;
using System.Collections.Generic;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Create Retailer event for Application Insights
    /// </summary>
    public class CreateRetailerAIEvent : BaseCreateRetailerAIEvent
    {
        public Guid UserId { get; internal set; }
    }
}
