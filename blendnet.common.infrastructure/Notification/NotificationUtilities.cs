using blendnet.common.dto.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.common.infrastructure.Notification
{
    public static class NotificationUtilities
    {
        public static dynamic GetNotificationPayload(NotificationData notificationData)
        {
            dynamic gcmObject = new JObject();
            dynamic gcm = new JObject();
            dynamic data = new JObject();

            gcm.pushNotificationKey = "newMsgPushNotification";

            data.message = gcm;
            data.type = notificationData.Type;
            data.body = notificationData.Body;
            data.title = notificationData.Title;
            data.image_url = notificationData.AttachmentUrl;

            gcmObject.priority = "high";
            gcmObject.data = data;
            return gcmObject;
        }
    }
}
