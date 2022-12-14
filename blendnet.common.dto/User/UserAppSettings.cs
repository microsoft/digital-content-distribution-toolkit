// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.User
{
    public class UserAppSettings
    {
        public string AccountEndPoint { get; set; }

        public string AccountKey { get; set; }

        public string DatabaseName { get; set; }

        public bool AllowWhitelistedUsersOnly { get; set; }

        public int WhitelistedUsersGlobalLimit { get; set; }
        
        public int WhitelistedUsersPerSourceLimit { get; set; }

        public int ExportDataSASExpiryInMts { get; set; }
    }
}
