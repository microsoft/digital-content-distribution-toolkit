using blendnet.notification.api.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Notification
{
    public class BroadcastNotificationRequest : BaseNotificationRequest
    {
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Description_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Topic { get; set; }

        public string Tags { get; set; }
    }
}
