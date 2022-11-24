// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace blendnet.common.dto.Identity
{
    public class InputClaimsDto
    {
        public string ObjectId { get; set; }

        [JsonPropertyName("signInNames.emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("signInNames.phoneNumber")]
        public string PhoneNumber { get; set; }

        public string GivenName { get; set; }

        public string DisplayName { get; set; }

        public string SurName { get; set; }

        public string SignInName { get; set; }
    }
}
