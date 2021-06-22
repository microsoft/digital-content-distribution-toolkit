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
         
            dynamic message = new JObject();

            dynamic android = new JObject();

            dynamic notification = new JObject();

            notification.body = notificationData.Body;
            notification.title =  notificationData.Title;
            notification.image = notificationData.AttachmentUrl;

            android.notification = notification;

            message.android = android;

            dynamic gcmObject = new JObject();

            dynamic gcm = new JObject();
            dynamic data = new JObject();
            gcm.pushNotificationKey = "newMsgPushNotification";

            data.message = gcm;
            data.type = notificationData.Type;
            gcmObject.priority = "high";

            gcmObject.data = data;
            gcmObject.message = message;
            return gcmObject;
        }
    }
}
