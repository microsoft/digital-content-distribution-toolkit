// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// RetailerCreatedIntegrationEvent
    /// </summary>
    public class RetailerCreatedIntegrationEvent: IntegrationEvent
    {
        public Retailer.RetailerDto Retailer { get; set; }
    }
}
