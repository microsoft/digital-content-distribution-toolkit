using blendnet.notification.api.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class BroadcastNotificationRequest : BaseNotificationRequest
    {
        [Required]
        public string Topic { get; set; }

        public string Tags { get; set; }
    }
}
