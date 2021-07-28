using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class NotificationAppSettings
    {
        public string DatabaseName { get; set; }

        public int NotificationTitleMaxLength { get; set; }

        public int NotificationBodyMaxLength { get; set; }

        public string KaizalaIdentityAppName { get; set; }
    }
}
