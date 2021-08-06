﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace blendnet.common.dto.Notification
{
    public class NotificationDto : BaseDto
    {
        public Guid? Id { get; set; }

        public Guid? NotificationId => Id;

        public string Title { get; set; }

        public string Body { get; set; }

        public string AttachmentUrl { get; set; }

        public PushNotificationType Type { get; set; }

        public string Topic { get; set; }

        public string Tags { get; set; }

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PushNotificationType
    {
        OrderComplete,
        NewArrival
    }

}