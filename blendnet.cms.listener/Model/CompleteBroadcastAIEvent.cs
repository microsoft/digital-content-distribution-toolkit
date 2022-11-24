// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.Model
{
    /// <summary>
    /// Complete Broadcast AI Event
    /// </summary>
    public class CompleteBroadcastAIEvent:BaseBroadcastAIEvent
    {
        /// <summary>
        /// Broadcast Start Date
        /// </summary>
        public DateTime BroadcastStartDate { get; set; }

        /// <summary>
        /// Broadcast End Date
        /// </summary>
        public DateTime BroadcastEndDate { get; set; }

        /// <summary>
        /// Filters
        /// </summary>
        public string Filters { get; set; }
    }
}
