using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class BaseNotificationRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public string AttachmentUrl { get; set; }

        [Required]
        public PushNotificationType Type { get; set; }
    }
}
