using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure
{
    /// <summary>
    /// To be implemented by the event handlers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIntegrationEventHandler<T>
        where T : IntegrationEvent
    {
        /// <summary>
        /// Actual implmentation of the event handler to handle the event raised from service bus.
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        Task Handle(T integrationEvent);
    }
}
