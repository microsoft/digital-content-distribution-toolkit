// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto.Notification
{
    public class NotificationPayloadData : PayloadData
    {
        public List<UserData> UserData { get; set; }
    }

    public class UserData
    {
        public string PhoneNumber { get; set; }

        public Guid UserId { get; set; }
    }

}
