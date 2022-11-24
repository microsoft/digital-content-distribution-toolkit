// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blendnet.common.dto.User;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Event raised in retailer referral code assigned to user.
    /// </summary>
    public class RetailerAssignedIntegrationEvent : IntegrationEvent
    {
        public User.User User { get; set; }
    }
}
