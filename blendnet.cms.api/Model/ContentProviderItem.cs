// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Holds information about content provider id and name used by consumer app
    /// </summary>
    public class ContentProviderItem
    {
        /// <summary>
        /// Content provider id
        /// </summary>
        public Guid ContentProviderId { get; set; }

        /// <summary>
        /// Content provider name
        /// </summary>
        public string Name { get; set; }
    }
}
