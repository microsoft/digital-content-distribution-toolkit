using blendnet.common.dto.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.notification.api.Model
{
    public class NotificationRequest : BaseNotificationRequest
    {
        [Required]
        public List<UserData> UserData { get; set; }

    }
}
