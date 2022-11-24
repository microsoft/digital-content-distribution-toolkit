// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// UserDataExportCompletedIntegrationEvent
    /// </summary>
    public class UserDataExportCompletedIntegrationEvent: BaseDataOperationCompletedIntegrationEvent
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
