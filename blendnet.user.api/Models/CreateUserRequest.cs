using blendnet.common.dto.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.user.api.Models
{
    public class CreateUserRequest 
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The channel id who creates the user
        /// </summary>
        [Required]
        public Channel ChannelId { get; set; }
    }
}
