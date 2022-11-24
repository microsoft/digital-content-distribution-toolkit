// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;

namespace blendnet.cms.api.Model
{
    /// <summary>
    /// Content Provider AI Event
    /// </summary>
    public class ContentProviderCreatedAIEvent: BaseContentProviderAIEvent
    {
        /// <summary>
        /// Content provider name
        /// </summary>
        public string Name { get; set; }
    }
}
