// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Ses
{
    /// <summary>
    /// Class to represent login request to get the SES token
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User Name
        /// </summary>
        public string user { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string pwd { get; set; }
    }
}
