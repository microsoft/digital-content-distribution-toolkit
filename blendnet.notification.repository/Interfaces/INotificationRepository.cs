using blendnet.common.dto.Common;
using blendnet.common.dto.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.notification.repository.Interfaces
{
    public interface INotificationRepository
    {
        /// <summary>
        /// Creates notification entry with given values
        /// </summary>
        /// <param name="notificationDto"></param>
        /// <returns></returns>
        public Task<Guid> CreateNotification(NotificationDto notificationDto);

        /// <summary>
        /// Returns list of notifications. If continuation token is not null, returns list with 
        /// items starting from continuation token
        /// </summary>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public Task<ResultData<NotificationDto>> GetNotifications(string continuationToken);

    }
}
