// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Notification
{
    public class NotificationRequest : BaseNotificationRequest
    {
        [Required]
        public List<UserData> UserData { get; set; }

    }
}
