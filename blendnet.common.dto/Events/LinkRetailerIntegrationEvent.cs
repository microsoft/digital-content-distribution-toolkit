// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// LinkRetailerIntegrationEvent
    /// </summary>
    public class LinkRetailerIntegrationEvent: IntegrationEvent
    {
        public string PartnerProvidedId { get; set; }

        public string PartnerCode { get; set; }

        public User.User User { get; set; }
    }
}
