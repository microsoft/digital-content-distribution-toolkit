using blendnet.common.dto.AIEvents;
using blendnet.common.dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Class to be used to integrate to 
    /// </summary>
    public class CreateUserAIEvent : BaseAIEvent
    {
        public Guid UserId { get; set; }

        public Guid IdentityId { get; set; }

        public Channel ChannelId { get; set; }
    }
}
