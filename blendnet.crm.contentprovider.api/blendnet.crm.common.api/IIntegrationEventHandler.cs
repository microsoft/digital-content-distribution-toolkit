using blendnet.crm.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.crm.common.api
{
    /// <summary>
    /// To be implemented by the event handlers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIntegrationEventHandler<T>
        where T : IntegrationEvent
    {
        Task Handle(T integrationEvent);
    }
}
