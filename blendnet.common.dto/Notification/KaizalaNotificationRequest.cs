using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class KaizalaNotificationRequest
    {
        public string PartnerName { get; set; }

        public string Payload { get; set; }

        public List<Guid> UserIds { get; set; }

        public int ScaleUnit { get; set; }
    }
}
