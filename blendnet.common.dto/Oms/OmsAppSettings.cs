using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Oms
{
    /// <summary>
    /// OMS Service Application Settings
    /// </summary>
    public class OmsAppSettings
    {
        public string AccountEndPoint { get; set; }

        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public string CmsBaseUrl { get; set; }

    }
}
