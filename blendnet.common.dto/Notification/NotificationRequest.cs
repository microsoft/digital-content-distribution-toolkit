using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class NotificationRequest
    {
        public string PartnerName { get; set; }

        public string Payload { get; set; }

        public List<UserData> UserData { get; set; }
    }

    public class KaizalaNotificationRequest
    {
        public string PartnerName { get; set; }

        public string Payload { get; set; }

        public List<Guid> UserIds { get; set; }

        public int ScaleUnit { get; set; }
    }

    public class UserData
    {
        public string PhoneNumber { get; set; }

        public Guid UserId { get; set; }
    }

    
    public class NotificationData
    {
        
        public string Title { get; set; }

        public string Body { get; set; }

        public string? AttachmentUrl { get; set; }

        public int Type { get; set; }

    }
    
}
