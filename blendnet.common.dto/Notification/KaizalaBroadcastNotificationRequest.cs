// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class KaizalaBroadcastNotificationRequest
    {
        public string PartnerName { get; set; }

        public string Payload { get; set; }

        public string Topic { get; set; }

        public List<int> ScaleUnits { get; set; }
    }
}
