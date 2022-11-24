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
    /// Updates the user data for privacy requirement. Updates the User Phone Number with User Id
    /// </summary>
    public class UpdateUserDataIntegrationEvent: IntegrationEvent
    {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// User Phone Number
        /// </summary>
        public string UserPhoneNumber { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid CommandId { get; set; }
    }
}
