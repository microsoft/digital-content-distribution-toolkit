using System.ComponentModel.DataAnnotations;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.user.api.Models
{
    public class SendNotificationRequest
    {
        /// <summary>
        /// Title of the notification
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Body of the notification
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// Url of attachment in notification
        /// </summary>
        public string AttachmentUrl { get; set; }


        /// <summary>
        /// Type of Notification
        /// </summary>
        [Required]
        public int Type { get; set; }


    }
}
