using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.Model
{
    public class Address
    {
        public string StreetName { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public MapLocation MapLocation { get; set; }
    }
}
