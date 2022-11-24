// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Identity
{
    public class OutputClaimsDto
    {
        //List of security groups the user is member of
        public List<string> groups { get; set; }

    }
}
