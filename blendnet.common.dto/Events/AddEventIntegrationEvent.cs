using blendnet.common.dto.Incentive;
using blendnet.common.dto.Integration;

namespace blendnet.common.dto.Events
{
    public class AddEventIntegrationEvent: IntegrationEvent
    {
        public Event Event { get; set; }
    }
}
