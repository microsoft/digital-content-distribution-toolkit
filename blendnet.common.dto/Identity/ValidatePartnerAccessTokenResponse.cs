// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Identity
{
    public class ValidatePartnerAccessTokenResponse
    {
        public string UserRole { get; set; }

        public KiazalaCredentials KiazalaCredentials { get; set; }

        public string UID { get; set; }
    }
}
