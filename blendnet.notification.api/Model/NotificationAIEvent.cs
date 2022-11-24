// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;
using blendnet.common.dto.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.notification.api.Model
{
    public class NotificationAIEvent : BaseAIEvent
    {
        /// <summary>
        /// Push notification type of the event
        /// </summary>
        public PushNotificationType PushNotificationType { get; set; }

        /// <summary>
        /// Notification sent date time
        /// </summary>
        public string NotificationDateTime { get; set; }

        /// <summary>
        /// Notification title
        /// </summary>
        public string NotificationTitle { get; set; }

        /// <summary>
        /// Notification body
        /// </summary>
        public string NotificationBody { get; set; }

        /// <summary>
        /// Topic of notification if it is a broadcast notification
        /// </summary>
        public string Topic { get; set; }


        /// <summary>
        /// List of userids if notification is sent to particular set of users
        /// </summary>
        public List<Guid> UserIds { get; set; }

    }
}
