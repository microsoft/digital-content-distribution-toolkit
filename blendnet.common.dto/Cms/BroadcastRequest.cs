// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Broadcast Command Data
    /// </summary>
    public class BroadcastRequest
    {
        public List<string> Filters { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
