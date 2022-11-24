// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Identity
{
    public class KiazalaCredentials
    {
        public string PhoneNumber { get; set; }
        
        public string CId { get; set; }
        
        public string TestSender { get; set; }
        
        public string AppName { get; set; }
        
        public string Permissions { get; set; }
        
        public int ApplicationType { get; set; }

        public long TokenValidFrom { get; set; }

    }
}
