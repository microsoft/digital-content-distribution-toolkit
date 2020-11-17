using blendnet.common.dto.Integration;
using System;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure
{
    /// <summary>
    /// Interface for Event Bus
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes the event to event bus
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        Task Publish(IntegrationEvent integrationEvent);

        /// <summary>
        /// Subscribe to the event and registers the event handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        /// <summary>
        /// Unsubscribes the event handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void Unsubscribe<T, TH>()
          where TH : IIntegrationEventHandler<T>
          where T : IntegrationEvent;
    }
}
