using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.user.api.Model
{
    public class AppSettings
    {
        public string GraphClientId { get; set; }

        public string GraphClientSecret { get; set; }

        public string GraphClientTenant { get; set; }
    }
}
