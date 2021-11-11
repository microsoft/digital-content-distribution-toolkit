using blendnet.common.dto.Common;
using blendnet.common.dto.Device;
using blendnet.device.api.Model;
using blendnet.device.repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.device.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class BrowseDeviceController : ControllerBase
    {
        private readonly ILogger _logger;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private IDeviceRepository _deviceRepository;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceRepository"></param>
        /// <param name="logger"></param>
        /// <param name="stringLocalizer"></param>
        public BrowseDeviceController(IDeviceRepository deviceRepository,
                                    ILogger<DeviceContentController> logger,
                                    IStringLocalizer<SharedResource> stringLocalizer)
        {
            _deviceRepository = deviceRepository;

            _logger = logger;

            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Get List of Content by the given device id and content provider id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        [HttpPost("{deviceId}/{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<ResultData<Guid>>> GetDeviceContent(   string deviceId, 
                                                                              Guid contentProviderId, 
                                                                              string continuationToken)
        {
            List<string> errorInfo = new List<string>();

            Device device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["DVC_ERR_0006"], deviceId));

                return BadRequest(errorInfo);
            }

            ResultData<DeviceContent> deviceApiResult = await _deviceRepository.GetContentByDeviceId(deviceId, contentProviderId, continuationToken, true);

            ResultData<Guid> result;

            if (deviceApiResult.Data != null && deviceApiResult.Data.Count > 0)
            {
                var contentIds = deviceApiResult.Data.Select(dc => dc.ContentId).ToList();

                result = new ResultData<Guid>(contentIds, deviceApiResult.ContinuationToken);

                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }


        /// <summary>
        /// Checks if content exists on device
        /// </summary>
        /// <param name="contentAvailabilityRequest"></param>
        /// <returns></returns>
        [HttpPost("contentavailability", Name = nameof(GetContentAvailability))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<List<string>>> GetContentAvailability(ContentAvailabilityRequest contentAvailabilityRequest)
        {
            List<string> errorInfo = new List<string>();

            if (contentAvailabilityRequest == null ||
                contentAvailabilityRequest.ContentId == default(Guid) ||
                contentAvailabilityRequest.DeviceIds == null ||
                contentAvailabilityRequest.DeviceIds.Count() <= 0 )
            {
                errorInfo.Add(_stringLocalizer["DVC_ERR_0010"]);

                return BadRequest(errorInfo);
            }

            List<DeviceContent> deviceContents = await _deviceRepository.GetContentAvailability(contentAvailabilityRequest.ContentId, contentAvailabilityRequest.DeviceIds);

            if (deviceContents != null && deviceContents.Count > 0)
            {
                var deviceIds = deviceContents.Select(dc => dc.DeviceId).ToList();

                return Ok(deviceIds);
            }
            else
            {
                return NotFound();
            }

        }


        /// <summary>
        /// Returns content availability count on device
        /// This is an approximate count.
        /// In case, a broadcast has been cancelled, it will take some time to get reflected here.
        /// Currently there is a limitation from SES if device is Powered OFF for 24 hours after cancellation, it will never get notified here
        /// </summary>
        /// <param name="contentAvailabilityRequest"></param>
        /// <returns></returns>
        [HttpPost("contentavailabilitycount", Name = nameof(GetContentAvailabilityCount))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<List<DeviceContentAvailability>>> GetContentAvailabilityCount(ContentAvailabilityCountRequest contentAvailabilityCountRequest)
        {
            List<string> errorInfo = new List<string>();

            if (contentAvailabilityCountRequest == null ||
                contentAvailabilityCountRequest.DeviceIds == null ||
                contentAvailabilityCountRequest.DeviceIds.Count() <= 0)
            {
                errorInfo.Add(_stringLocalizer["DVC_ERR_0015"]);

                return BadRequest(errorInfo);
            }

            List<DeviceContentAvailability> deviceContentAvailability = await _deviceRepository.GetContentAvailabilityCount(contentAvailabilityCountRequest.DeviceIds);

            if (deviceContentAvailability != null && deviceContentAvailability.Count > 0)
            {
                return Ok(deviceContentAvailability);
            }
            else
            {
                return NotFound();
            }

        }
    }
}
