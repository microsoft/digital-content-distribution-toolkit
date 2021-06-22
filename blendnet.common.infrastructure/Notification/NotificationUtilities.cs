using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.common.infrastructure.Notification
{
    public static class NotificationUtilities
    {
        public static string GetNotificationPayload(string title, string body, string? attachmentUrl, Guid? orderId, int notificationType, string kaizalaIdentityAppName)
        {
         
            dynamic message = new JObject();

            dynamic android = new JObject();

            dynamic notification = new JObject();

            notification.body = body;
            notification.title = title;
            notification.image = attachmentUrl;

            android.notification = notification;

            message.android = android;

            dynamic gcmObject = new JObject();

            dynamic gcm = new JObject();
            dynamic data = new JObject();
            gcm.pushNotificationKey = "newMsgPushNotification";
            gcm.appname = kaizalaIdentityAppName;

            data.message = gcm;
            data.type = notificationType;
            if (notificationType == PushNotificationType.OrderComplete)
            {
                data.orderId = orderId;
            }
            gcmObject.priority = "high";

            gcmObject.data = data;
            gcmObject.message = message;
            string msg = JsonConvert.SerializeObject(gcmObject);
            return msg;
        }
    }
}
