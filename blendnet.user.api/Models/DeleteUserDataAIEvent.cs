// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// AI Event for Delete User Data Request
    /// </summary>
    public class DeleteUserDataAIEvent : BaseAIEvent
    {
        public Guid UserId { get; set; }

        public Guid RequestId { get; set; }
    }
}
