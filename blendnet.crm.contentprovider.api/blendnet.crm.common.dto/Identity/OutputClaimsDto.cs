using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.crm.common.dto.Identity
{
    public class OutputClaimsDto
    {
        //List of security groups the user is member of
        public List<string> groups { get; set; }

    }
}
