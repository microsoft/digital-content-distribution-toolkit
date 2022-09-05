using blendnet.common.dto.Incentive;
using blendnet.common.dto.Integration;
using System;

namespace blendnet.common.dto.Events
{
    public class UserMediaDnldCmpltdIncentiveIntegrationEvent : BaseUserIncentiveIntegrationEvent
    {
        public IncentiveEvent RetailerIncentiveEvent { get; set; }
    }
}
