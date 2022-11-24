// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Ses
{
    /// <summary>
    /// Cancellation Result
    /// </summary>
    public class BroadcastCancellationResult
    {
        public string Status { get; set; }

        public object Reason { get; set; }
    }
}
