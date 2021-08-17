using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class DeviceAppSettings
    {
        public string AccountEndPoint { get; set; }

        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string ServiceBusTopicName { get; set; }

        public Dictionary<string, string> KaizalaIdentity { get; set; }

        public string CloudApiServiceAccountCertName { get; set; }

        public string KaizalaIdentityAppName { get; set; }

    }
}
