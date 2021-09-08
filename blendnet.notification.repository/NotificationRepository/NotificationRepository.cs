using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.Notification;
using blendnet.common.infrastructure.Extensions;
using blendnet.notification.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.notification.repository
{
    public class NotificationRepository : INotificationRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        NotificationAppSettings _appSettings;

        public NotificationRepository(CosmosClient dbClient,
                                IOptionsMonitor<NotificationAppSettings> optionsMonitor,
                                ILogger<NotificationRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.Notification);
        }

        /// <summary>
        /// Creates new notification entry in database
        /// </summary>
        /// <param name="notificationDto"></param>
        /// <returns></returns>
        public async Task<Guid> CreateNotification(NotificationDto notificationDto)
        {
            await this._container.CreateItemAsync<NotificationDto>(notificationDto, new PartitionKey(notificationDto.NotificationId.Value.ToString()));
            return notificationDto.NotificationId.Value;
        }

        /// <summary>
        /// Returns list of notifications based on continuation token
        /// </summary>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public async Task<ResultData<NotificationDto>> GetNotifications(string continuationToken)
        {
            string queryString = $"SELECT * FROM c ORDER BY c.createdDate desc";

            var queryDef = new QueryDefinition(queryString);

            var notificationResult = await _container.ExtractDataFromQueryIteratorWithToken<NotificationDto>(queryDef, continuationToken);

            return notificationResult;

        }
    }
}
