using blendnet.api.proxy.Cms;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Device;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.device.api.Model;
using blendnet.device.repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace blendnet.device.api.Controllers
{
    /// <summary>
    /// Device content controller
    /// TODO:  If we go with different service principal for each device, take deviceid from token. 
    /// Else current way works fine
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin,
                    ApplicationConstants.KaizalaIdentityRoles.HubDevice)]
    public class DeviceContentController : ControllerBase
    {
        private readonly ILogger _logger;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private IDeviceRepository _deviceRepository;

        private ContentProxy _contentProxy;

        public DeviceContentController(IDeviceRepository deviceRepository,
                                    ILogger<DeviceContentController> logger,
                                    ContentProxy contentProxy,
                                    IStringLocalizer<SharedResource> stringLocalizer)
        {
            _deviceRepository = deviceRepository;

            _logger = logger;

            _stringLocalizer = stringLocalizer;

            _contentProxy = contentProxy;
        }

        #region public apis

        /// <summary>
        /// Updates the deleted contents of device in database
        /// </summary>
        /// <param name="updateDownloadedRequest"></param>
        /// <returns></returns>
        [HttpPost("downloaded", Name = nameof(UpdateDownloaded))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> UpdateDownloaded(UpdateRequest updateDownloadedRequest)
        {
            return await ProcessUpdateRequest(updateDownloadedRequest, false/*isDeleted*/);
        }

        /// <summary>
        /// Updates the downloaded contents of device in database
        /// </summary>
        /// <param name="updateDownloadedRequest"></param>
        /// <returns></returns>
        [HttpPost("deleted", Name = nameof(UpdateDeleted))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> UpdateDeleted(UpdateRequest updateDeletedRequest)
        {
            return await ProcessUpdateRequest(updateDeletedRequest, true /*isDeleted*/);
        }

        #endregion

        #region private methods

        /// <summary>
        /// Processes update request based on whether it is deleted or downloaded content
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        private async Task<ActionResult> ProcessUpdateRequest(UpdateRequest updateRequest, bool isDeleted)
        {
            List<string> failedItems = new List<string>();
            string deviceId = updateRequest.DeviceId;
            //validate device id

            var device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                List<string> errorInfo = new List<string>();
                errorInfo.Add(string.Format(_stringLocalizer["DVC_ERR_0006"], deviceId));
                return BadRequest(errorInfo);
            }

            List<ContentInfo> contentInfos = await _contentProxy.GetContentProviderIds(updateRequest.Contents.Select(x => x.ContentId).ToList());

            foreach (var contentdata in updateRequest.Contents)
            {
                Guid contentProviderId = contentInfos.Where(x => x.ContentId == contentdata.ContentId).Select(x => x.ContentProviderId).FirstOrDefault();

                if (contentProviderId == default(Guid))
                {
                    failedItems.Add(string.Format(_stringLocalizer["DVC_ERR_0008"], contentdata.ContentId));
                    continue;
                }

                var userId = UserClaimData.GetUserId(User.Claims);
                var curDate = DateTime.UtcNow;

                var deviceContent = new DeviceContent()
                {
                    DeviceId = deviceId,
                    ContentId = contentdata.ContentId,
                    ContentProviderId = contentProviderId,
                    IsDeleted = isDeleted,
                    OperationTimeStamp = contentdata.OperationTime,
                    CreatedByUserId = userId,
                    CreatedDate = curDate,
                    ModifiedByByUserId = userId,
                    ModifiedDate = curDate
                };

                try
                {
                    var dbResponse = await _deviceRepository.UpsertDeviceContent(deviceContent);
                }
                catch(Exception)
                {
                    failedItems.Add(string.Format(_stringLocalizer["DVC_ERR_0009"], contentdata.ContentId));
                }
            }

            if(failedItems.Count == 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(failedItems);
            }
        }

        #endregion

    }
}
