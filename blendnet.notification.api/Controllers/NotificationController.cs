using AutoMapper;
using blendnet.api.proxy.Kaizala;
using blendnet.common.dto.Common;
using blendnet.common.dto.Notification;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using blendnet.notification.api.Model;
using blendnet.notification.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.notification.api
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger _logger;

        private NotificationAppSettings _appSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private TelemetryClient _telemetryClient;

        private KaizalaNotificationProxy _notificationProxy;

        private INotificationRepository _notificationRepository;

        private IMapper _mapper;

        public NotificationController(
                              ILogger<NotificationController> logger,
                              KaizalaNotificationProxy notificationProxy,
                              IOptionsMonitor<NotificationAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              TelemetryClient telemetryClient,
                              INotificationRepository notificationRepository,
                              IMapper mapper
                              )
        {
            _logger = logger;
            _appSettings = optionsMonitor.CurrentValue;
            _stringLocalizer = stringLocalizer;
            _telemetryClient = telemetryClient; 
            _notificationProxy = notificationProxy;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }


        #region notification api

        /// <summary>
        /// Api to send custom notifications based on topic
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        [HttpPost("sendbroadcast", Name = nameof(SendBroadcastNotification))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> SendBroadcastNotification(BroadcastNotificationRequest broadcastNotificationRequest)
        {

            List<string> errorInfo = new List<string>();
            int titleMaxLen = _appSettings.NotificationTitleMaxLength;
            int bodyMaxLen = _appSettings.NotificationBodyMaxLength;

            if (broadcastNotificationRequest.Title.Length > titleMaxLen || broadcastNotificationRequest.Body.Length > bodyMaxLen)
            {
                errorInfo.Add(String.Format(_stringLocalizer["NMS_ERR_001"]));
                return BadRequest(errorInfo);
            }

            NotificationDto notificationDto = GetNotificationFromBroadcastReq(broadcastNotificationRequest);

            var payload = GetNotificationPayload(notificationDto);

            BroadcastNotificationPayloadData payloadData = new BroadcastNotificationPayloadData()
            {
                Payload = payload,
                Topic = broadcastNotificationRequest.Topic,
                PartnerName = _appSettings.KaizalaIdentityAppName
            };

            await _notificationProxy.SendBroadcastNotification(payloadData);
            await _notificationRepository.CreateNotification(notificationDto);

            NotificationAIEvent notificationAIEvent = new NotificationAIEvent()
            {
                NotificationTitle = broadcastNotificationRequest.Title,
                NotificationBody = broadcastNotificationRequest.Body,
                NotificationDateTime = System.Text.Json.JsonSerializer.Serialize(DateTime.UtcNow),
                Topic = broadcastNotificationRequest.Topic,
                PushNotificationType = broadcastNotificationRequest.Type
            };

            _telemetryClient.TrackEvent(notificationAIEvent);

            return Ok(notificationDto.Id);

        }

        /// <summary>
        /// Api to send custom notifications based on topic
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        [HttpPost("send", Name = nameof(SendNotification))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> SendNotification(NotificationRequest notificationRequest)
        {

            List<string> errorInfo = new List<string>();
            int titleMaxLen = _appSettings.NotificationTitleMaxLength;
            int bodyMaxLen = _appSettings.NotificationBodyMaxLength;

            if (notificationRequest.Title.Length > titleMaxLen || notificationRequest.Body.Length > bodyMaxLen)
            {
                errorInfo.Add(String.Format(_stringLocalizer["NMS_ERR_001"]));
                return BadRequest(errorInfo);
            }

            NotificationDto notificationDto = GetNotificationFromNotiReq(notificationRequest);

            var payload = GetNotificationPayload(notificationDto);

            NotificationPayloadData payloadData = new NotificationPayloadData()
            {
                Payload = payload,
                PartnerName = _appSettings.KaizalaIdentityAppName,
                UserData = notificationRequest.UserData
            };

            await _notificationProxy.SendNotification(payloadData);

            NotificationAIEvent notificationAIEvent = new NotificationAIEvent()
            {
                NotificationTitle = notificationRequest.Title,
                NotificationBody = notificationRequest.Body,
                NotificationDateTime = System.Text.Json.JsonSerializer.Serialize(DateTime.UtcNow),
                UserIds = notificationRequest.UserData.Select(x => x.UserId).ToList(),
                PushNotificationType = notificationRequest.Type
            };

            _telemetryClient.TrackEvent(notificationAIEvent);

            return Ok(notificationDto.Id);

        }

        /// <summary>
        /// Returns list of notifications 
        /// </summary>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        [HttpGet("notifications", Name = nameof(GetNotifications))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<ResultData<NotificationDto>>> GetNotifications(string continuationToken)
        {
            List<string> errorInfo = new List<string>();

            var response = await _notificationRepository.GetNotifications(continuationToken);

            if (response == null || response.Data == null || response.Data.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        #endregion

        /// <summary>
        /// Returns notification dto object which is used to store in db
        /// </summary>
        /// <param name="notificationReq"></param>
        /// <returns></returns>
        private NotificationDto GetNotificationFromBroadcastReq(BroadcastNotificationRequest notificationReq)
        {
            NotificationDto notificationDto = _mapper.Map<BroadcastNotificationRequest, NotificationDto>(notificationReq);

            notificationDto.Id = Guid.NewGuid();
            notificationDto.CreatedByUserId = UserClaimData.GetUserId(User.Claims);
            notificationDto.CreatedDate = DateTime.UtcNow;

            return notificationDto;
        }

        /// <summary>
        /// Returns notification dto object
        /// </summary>
        /// <param name="notificationReq"></param>
        /// <returns></returns>
        private NotificationDto GetNotificationFromNotiReq(NotificationRequest notificationReq)
        {
            NotificationDto notificationDto = _mapper.Map<NotificationRequest, NotificationDto>(notificationReq);

            notificationDto.Id = Guid.NewGuid();
            notificationDto.CreatedByUserId = UserClaimData.GetUserId(User.Claims);
            notificationDto.CreatedDate = DateTime.UtcNow;

            return notificationDto;
        }

        /// <summary>
        /// Returns notification payload from given data
        /// </summary>
        /// <param name="notificationData"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        private string GetNotificationPayload(NotificationDto notificationData)
        {
            dynamic gcmObject = new JObject();
            dynamic gcm = new JObject();
            dynamic data = new JObject();

            gcm.pushNotificationKey = "newMsgPushNotification";

            data.message = gcm;
            data.type = notificationData.Type.ToString();
            data.body = notificationData.Body;
            data.title = notificationData.Title;
            data.image_url = notificationData.AttachmentUrl;

            data.message.appname = _appSettings.KaizalaIdentityAppName;

            gcmObject.priority = "high";
            gcmObject.data = data;

            return JsonConvert.SerializeObject(gcmObject);
        }
    }
}
