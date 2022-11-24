// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace blendnet.common.dto.Identity
{
    public class B2CResponseDto
    {
        public string version { get; set; }
        public int status { get; set; }
        public string userMessage { get; set; }

        public B2CResponseDto(string message, HttpStatusCode status)
        {
            this.userMessage = message;
            this.status = (int)status;
            this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
