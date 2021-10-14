using blendnet.api.proxy.Cms;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Device;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.device.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement)]
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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> UpdateDownloaded(DeviceContentUpdateRequest updateDownloadedRequest)
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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> UpdateDeleted(DeviceContentUpdateRequest updateDeletedRequest)
        {
            return await ProcessUpdateRequest(updateDeletedRequest, true /*isDeleted*/);
        }

        /// <summary>
        /// Returns the details about : 
        ///     content broadcasted, 
        ///     total valid content broadcasted for this device, 
        ///     total valid available content on device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpPost("{deviceId}/{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement)]
        public async Task<ActionResult<DeviceContentValidity>> GetDeviceContentValidity(string deviceId,Guid contentProviderId)
        {
            List<string> errorInfo = new List<string>();

            Device device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["DVC_ERR_0006"], deviceId));

                return BadRequest(errorInfo);
            }

            if (device.FilterUpdatedBy == null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["DVC_ERR_0014"], deviceId));

                return BadRequest(errorInfo);
            }

            ContentStatusFilter contentStatusFilter = new ContentStatusFilter();

            contentStatusFilter.ContentTransformStatuses = new string[] { ContentTransformStatus.TransformComplete.ToString() };

            contentStatusFilter.ContentBroadcastStatuses = new string[] { ContentBroadcastStatus.BroadcastOrderComplete.ToString() };

            //Get Valid BroadCasted Content for given content provider
            List<Content> broadCastedContents = await _contentProxy.GetContentByContentProviderId(contentProviderId, contentStatusFilter);

            if (broadCastedContents == null || broadCastedContents.Count() <= 0)
            {
                errorInfo.Add(string.Format(_stringLocalizer["DVC_ERR_0011"], deviceId));

                return NotFound(errorInfo);
            }

            //We are concerned with the content which is in active state.
            List<Content> activeBroadCastedContents = broadCastedContents.Where(bc => bc.IsBroadCastActive).ToList();

            if (activeBroadCastedContents == null || activeBroadCastedContents.Count() <= 0)
            {
                errorInfo.Add(string.Format(_stringLocalizer["DVC_ERR_0012"], deviceId));

                return NotFound(errorInfo);
            }

            //Get the list of available content on device for the given content provider
            List<DeviceContent> deviceContents = await _deviceRepository.GetContentByDeviceId(deviceId, contentProviderId, true);

            return GetDeviceContentAvailability(device, activeBroadCastedContents, deviceContents);

        }

        #endregion

        #region private methods

        /// <summary>
        /// Processes update request based on whether it is deleted or downloaded content
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        private async Task<ActionResult> ProcessUpdateRequest(DeviceContentUpdateRequest updateRequest, bool isDeleted)
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

        /// <summary>
        /// 1) find out if the active broadcasted content is valid for device.
        /// 2) IF yes, the does it exists on device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="activeBroadcastContentList"></param>
        /// <param name="contentAvailableOnDevice"></param>
        /// <returns></returns>
        private DeviceContentValidity GetDeviceContentAvailability(Device device,
                                            List<Content> activeBroadcastContentList,  
                                            List<DeviceContent> contentAvailableOnDevice )
        {
            DeviceContentValidity deviceContentValidity = new DeviceContentValidity();

            deviceContentValidity.DeviceId = device.DeviceId;
            deviceContentValidity.DeviceFiltersUsed = device.FilterUpdatedBy.FilterUpdateRequest.Filters;
            deviceContentValidity.TotalActiveBroacastedContent = activeBroadcastContentList.Count();
            deviceContentValidity.ValidActiveBroadcastedContentList = new List<ContentValidity>();

            ContentValidity contentValidity;

            // 1) find out if the active broadcasted content is valid for device.
            // 2) IF yes, the does it exists on device.
            foreach (Content activeContent in activeBroadcastContentList)
            {
                List<string> broadCastedFilters = activeContent.ContentBroadcastedBy.BroadcastRequest.Filters;

                //check if it is a validate broadcast for device.
                if (device.FilterUpdatedBy.FilterUpdateRequest.Filters.Intersect(broadCastedFilters).Any())
                {
                    contentValidity = new ContentValidity();

                    deviceContentValidity.ValidActiveBroadcastedContentList.Add(contentValidity);

                    //since one of the broadast and device filter matches it is eligible to be called as valid content
                    contentValidity.ValidActiveBroadcastedContent = activeContent;

                    //if there is no content available on device, no point checking if it available on device
                    if (contentAvailableOnDevice != null && contentAvailableOnDevice.Count() > 0)
                    {
                        //now check if the valid broadcasted content has been downloaded on device or not
                        DeviceContent contentOnDevice = contentAvailableOnDevice.
                                                        Where(ca => (ca.ContentId == activeContent.ContentId
                                                                        && ca.ContentProviderId == activeContent.ContentProviderId))
                                                        .FirstOrDefault();

                        if (contentOnDevice != default(DeviceContent))
                        {
                            contentValidity.DeviceContent = contentOnDevice;
                        }
                    }
                }
            }

            return deviceContentValidity;
        }

        #endregion

    }
}
