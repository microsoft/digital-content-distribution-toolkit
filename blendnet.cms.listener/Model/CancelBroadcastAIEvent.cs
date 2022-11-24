// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.Model
{
    /// <summary>
    /// CancelBroadcastAIEvent
    /// </summary>
    public class CancelBroadcastAIEvent: BaseBroadcastAIEvent
    {
        /// <summary>
        /// Cancelation Date
        /// </summary>
        public DateTime CancelationDate { get; set; }
    }
}
