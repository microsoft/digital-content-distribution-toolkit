// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Whitelisted User
    /// </summary>
    public class WhitelistedUserDto
    {
        /// <summary>
        /// Id of the user (same as phone number)
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id => PhoneNumber;

        /// <summary>
        /// Phone number of the user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email ID associated with this user
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// User ID who whitelisted this user
        /// </summary>
        public Guid WhitelistedByUserId { get ; set; }

        /// <summary>
        /// Role of the user who whitelisted this user (superadmin/retailer/...)
        /// </summary>
        public string WhitelistedByUserRole { get; set; }

        /// <summary>
        /// Partner Code of the retailer who whitelisted this user
        /// </summary>
        public string PartnerCode { get; set; }

        /// <summary>
        /// Partner-provided Retailer ID of the retailer who whitelisted this user
        /// </summary>
        public string PartnerProvidedRetailerId { get; set; }
    }
}
