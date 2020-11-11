using blendnet.crm.common.dto.Integration;
using System;
using System.Threading.Tasks;

namespace blendnet.crm.common.api
{
    /// <summary>
    /// Interface for Event Bus
    /// </summary>
    public interface IEventBus
    {
        Task Publish(IntegrationEvent integrationEvent);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
          where TH : IIntegrationEventHandler<T>
          where T : IntegrationEvent;
    }
}
