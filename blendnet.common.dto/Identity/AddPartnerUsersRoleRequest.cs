// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Identity
{
    /// <summary>
    /// Add Partner User Role Request
    /// </summary>
    public class AddPartnerUsersRoleRequest
    {
        /// <summary>
        /// Application Name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Phone Role List
        /// </summary>
        public List<PhoneRole> PhoneRoleList { get; set; }

    }

    public class DeletePartnerUsersRoleRequest
    {
        /// <summary>
        /// Application Name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Phone Role List
        /// </summary>
        public List<PhoneRole> PhoneRoleList { get; set; }

    }

    /// <summary>
    /// Phone Role List
    /// </summary>
    public class PhoneRole
    {
        /// <summary>
        /// Phone Number
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }
    }
}
