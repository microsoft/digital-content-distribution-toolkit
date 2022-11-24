// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class FilterUpdatedBy
    {
        /// <summary>
        /// Command Id
        /// </summary>
        public Guid CommandId { get; set; }

        /// <summary>
        /// Details about filter update request
        /// </summary>
        public FilterUpdateRequest FilterUpdateRequest { get; set; }
    }
}
