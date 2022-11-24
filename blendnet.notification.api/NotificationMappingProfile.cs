// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.common.dto.Notification;
using blendnet.notification.api.Model;

namespace blendnet.notification.api
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<NotificationDto, BroadcastNotificationRequest>();
            CreateMap<BroadcastNotificationRequest, NotificationDto>();
            CreateMap<NotificationDto, NotificationRequest>();
            CreateMap<NotificationRequest, NotificationDto>();
        }
    }
}
