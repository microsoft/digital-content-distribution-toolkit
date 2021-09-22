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
    /// AI Event for Create User Data Export Request
    /// </summary>
    public class CreateUserDataExportCommandAIEvent : BaseAIEvent
    {
        public Guid UserId { get; set; }

        public Guid RequestId { get; set; }
    }
}
