using blendnet.common.dto;
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
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string UserName { get; set; }

        /// <summary>
        /// The channel id who creates the user
        /// </summary>
        [Required]
        public Channel ChannelId { get; set; }
    }
}
