using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// occurs when the user commands update is completed
    /// </summary>
    public class UserDataUpdateCompletedIntegrationEvent: BaseDataOperationCompletedIntegrationEvent
    {
        /// <summary>
        /// return service name
        /// </summary>
        public override string ServiceName
        {
            get
            {
                return ApplicationConstants.BlendNetServices.UserService;
            }
        }
    }
}
