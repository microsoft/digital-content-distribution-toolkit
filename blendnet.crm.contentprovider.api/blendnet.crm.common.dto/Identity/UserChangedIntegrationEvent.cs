using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Identity
{
    public class UserChangedIntegrationEvent:IntegrationEvent
    {
        /// <summary>
        /// Azure AD B2C Object Id
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Given Name
        /// </summary>
        public string GivenName { get; set; }

    }
}
