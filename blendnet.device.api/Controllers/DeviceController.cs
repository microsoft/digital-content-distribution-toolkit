using blendnet.common.dto;
using blendnet.common.dto.Device;
using blendnet.common.dto.Events;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using blendnet.device.api.Model;
using blendnet.device.repository.Interfaces;
using Microsoft.AspNetCore.Http;
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
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, 
                    ApplicationConstants.KaizalaIdentityRoles.DeviceManagement, 
                    ApplicationConstants.KaizalaIdentityRoles.Device)]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        private IDeviceRepository _deviceRepository;

        IStringLocalizer<SharedResource> _stringLocalizer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceRepository"></param>
        /// <param name="logger"></param>
        /// <param name="eventBus"></param>
        /// <param name="stringLocalizer"></param>
        public DeviceController(IDeviceRepository deviceRepository,
                                    ILogger<DeviceController> logger,
                                    IEventBus eventBus,
                                    IStringLocalizer<SharedResource> stringLocalizer)
        {
            _deviceRepository = deviceRepository;

            _logger = logger;

            _eventBus = eventBus;

            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Submits the Filter Update Request
        /// </summary>
        /// <param name="broadcastContentRequest"></param>
        /// <returns></returns>
        [HttpPost("filterupdate", Name = nameof(FilterUpdate))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.DeviceManagement)]
        public async Task<ActionResult> FilterUpdate(DeviceFilterUpdateRequest filterUpdateRequest)
        {
            List<string> errorInfo = new List<string>();

            if (filterUpdateRequest == null ||
                filterUpdateRequest.DeviceIds == null ||
                filterUpdateRequest.DeviceIds.Count() <= 0 ||
                filterUpdateRequest.Filters == null ||
                filterUpdateRequest.Filters.Count() <= 0 )
            {
                errorInfo.Add(_stringLocalizer["DVC_ERR_0001"]);

                return BadRequest(errorInfo);
            }

            List<Device> devicelist = await _deviceRepository.GetDeviceByIds(filterUpdateRequest.DeviceIds);

            List<string> errorList = new List<string>();

            //adds the invalid id details to the error list
            ValidateDeviceIds(filterUpdateRequest.DeviceIds, devicelist, errorList);

            bool validationFailed;

            if (devicelist != null && devicelist.Count > 0)
            {
                foreach (Device device in devicelist)
                {
                    validationFailed = false;

                    ////if device exists state should be provisioned.
                    if (device.DeviceStatus != DeviceStatus.Provisioned)
                    {
                        errorList.Add(string.Format(_stringLocalizer["DVC_ERR_0002"],device.Id));

                        validationFailed = true;
                    }

                    //Device command should not be in any of the below status
                    if (device.FilterUpdateStatus == DeviceCommandStatus.DeviceCommandSubmitted ||
                        device.FilterUpdateStatus == DeviceCommandStatus.DeviceCommandInProcess ||
                        device.FilterUpdateStatus == DeviceCommandStatus.DeviceCommandPushedToDevice)
                    {
                        errorList.Add(String.Format(_stringLocalizer["DVC_ERR_0003"],
                            device.Id,DeviceCommandStatus.DeviceCommandSubmitted,
                            DeviceCommandStatus.DeviceCommandInProcess,
                            DeviceCommandStatus.DeviceCommandPushedToDevice));

                        validationFailed = true;
                    }

                    //if none of the validation failed for the device
                    if (!validationFailed)
                    {
                        device.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandSubmitted;

                        device.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

                        device.ModifiedDate = DateTime.UtcNow;

                        await _deviceRepository.UpdateDevice(device);

                        DeviceCommand filterUpdateCommand = new DeviceCommand()
                        {
                            DeviceCommandType = DeviceCommandType.FilterUpdate,
                            DeviceId = device.Id,
                            CreatedByUserId = UserClaimData.GetUserId(this.User.Claims),
                            FilterUpdateRequest = new FilterUpdateRequest()
                            {
                                Filters = filterUpdateRequest.Filters
                            }
                        };

                        ////publish the event
                        FilterUpdateIntegrationEvent filterUpdateIntegrationEvent = new FilterUpdateIntegrationEvent()
                        {
                            FilterUpdateCommand = filterUpdateCommand
                        };

                        await _eventBus.Publish(filterUpdateIntegrationEvent);
                    }
                }
            }

            return Ok(errorList);
        }


        #region Private Methods

        /// <summary>
        /// Validates if the give device id exists in database
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="retrievedDevicess"></param>
        /// <param name="errorList"></param>
        private void ValidateDeviceIds(List<string> parentIds, List<Device> retrievedDevicess, List<string> errorList)
        {
            List<string> invalidIds = GetInvalidDeviceIds(parentIds, retrievedDevicess);

            if (invalidIds != null && invalidIds.Count > 0)
            {
                foreach (string invalidId in invalidIds)
                {
                    errorList.Add(String.Format(_stringLocalizer["DVC_ERR_0004"], invalidId));
                }
            }
        }


        /// <summary>
        /// returns the list of invalid device ids
        /// </summary>
        /// <param name="parentList"></param>
        /// <param name="retrievedDevices"></param>
        /// <returns></returns>
        private List<string> GetInvalidDeviceIds(List<string> parentList, List<Device> retrievedDevices)
        {
            List<string> invalidDeviceIds = new List<string>();

            foreach (string deviceId in parentList)
            {
                if (!retrievedDevices.Exists(rd => rd.Id.Equals(deviceId)))
                {
                    invalidDeviceIds.Add(deviceId);
                }
            }

            return invalidDeviceIds;
        }

        #endregion
    }
}
