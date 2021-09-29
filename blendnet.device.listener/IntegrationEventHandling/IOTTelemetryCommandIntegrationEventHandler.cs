using blendnet.common.dto.Device;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.device.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using blendnet.common.dto.Extensions;
using blendnet.common.dto.Cms;
using System.Linq;
using blendnet.api.proxy.Cms;
using blendnet.common.dto;

namespace blendnet.device.listener.IntegrationEventHandling
{
    /// <summary>
    /// We are going to recieve action to be performed in the telemetry data.
    /// Here is the event handler to listen the action sent in the telemetry data.
    /// </summary>
    public class IOTTelemetryCommandIntegrationEventHandler : IIntegrationEventHandler<IOTTelemetryCommandIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly DeviceAppSettings _appSettings;

        private IDeviceRepository _deviceRepository;

        private ContentProxy _contentProxy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="deviceRepository"></param>
        public IOTTelemetryCommandIntegrationEventHandler(ILogger<IOTTelemetryCommandIntegrationEventHandler> logger,
                                                      TelemetryClient tc,
                                                      IOptionsMonitor<DeviceAppSettings> optionsMonitor,
                                                      IDeviceRepository deviceRepository,
                                                      ContentProxy contentProxy)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _deviceRepository = deviceRepository;

            _contentProxy = contentProxy;

        }

        /// <summary>
        /// To handle the command recieved via IOT Central
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(IOTTelemetryCommandIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("IOTTelemetryCommandIntegrationEventHandler.Handle"))
                {
                    if (integrationEvent.Telemetry == null ||
                        integrationEvent.Telemetry.TelemetryCommandData == null ||
                        !integrationEvent.Telemetry.TelemetryCommandData.CommandName.HasValue ||
                        string.IsNullOrEmpty(integrationEvent.Telemetry.TelemetryCommandData.CommandData))
                    {
                        _logger.LogInformation($"No valid telemetry details found. Make sure to set the right data export in IOT Central. Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module}");

                        return;
                    }

                    _logger.LogInformation($"Handle Process starting for Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} Message Body : {integrationEvent.Body}");

                    switch (integrationEvent.Telemetry.TelemetryCommandData.CommandName.Value)
                    {
                        case IOTTelemetryCommandName.CompleteCommand:
                            {
                                await CompleteCommand(integrationEvent);
                                break;
                            };
                        case IOTTelemetryCommandName.ProvisionDevice:
                            {
                                await ProvisionDevice(integrationEvent);
                                break;
                            }
                        case IOTTelemetryCommandName.ContentDeleted:
                            {
                                await ProcessDeviceContentRequest(integrationEvent, true);
                                break;
                            }
                        case IOTTelemetryCommandName.ContentDownloaded:
                            {
                                await ProcessDeviceContentRequest(integrationEvent, false);
                                break;
                            }
                        default:
                            {
                                _logger.LogInformation($"Invalid Command Name Passed {integrationEvent.Telemetry.TelemetryCommandData.CommandName.Value}  for : Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module}");
                                break;
                            }
                    }

                    _logger.LogInformation($"Handle complete  for : Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }


        /// <summary>
        /// Marks the command as complete based on the data recieved in command.
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        private async Task CompleteCommand(IOTTelemetryCommandIntegrationEvent integrationEvent)
        {
            try
            {
                _logger.LogInformation($"Complete command data : {integrationEvent.Telemetry.TelemetryCommandData.CommandData}." +
                    $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                DeviceCommandUpdateRequest deviceCommandUpdateRequest =
                    JsonSerializer.Deserialize<DeviceCommandUpdateRequest>(integrationEvent.Telemetry.TelemetryCommandData.CommandData,
                                                                           Utilties.GetJsonSerializerOptions());

                //perform the data validation
                if (!deviceCommandUpdateRequest.CommandId.HasValue ||
                    string.IsNullOrEmpty(deviceCommandUpdateRequest.DeviceId) ||
                    !deviceCommandUpdateRequest.IsFailed.HasValue)
                {
                    _logger.LogError($"Invalid data recieved for Complete Command. " +
                        $"Command Id, Device Id , Is Failed are mandatory. Command Data {integrationEvent.Telemetry.TelemetryCommandData.CommandData} Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //get the device command
                DeviceCommand deviceCommand = await _deviceRepository.GetDeviceCommandById(deviceCommandUpdateRequest.CommandId.Value,
                                                                                            deviceCommandUpdateRequest.DeviceId);

                if (deviceCommand == null)
                {
                    _logger.LogError($"Invalid command id - {deviceCommandUpdateRequest.CommandId.Value} and Device Id {deviceCommandUpdateRequest.DeviceId} " +
                        $" combination recieved for Complete Command. " +
                        $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //validate the device command status. It should be in "Pushed to Device" state.
                if (deviceCommand.DeviceCommandStatus != DeviceCommandStatus.DeviceCommandPushedToDevice)
                {
                    _logger.LogError($"Invalid device command status. Command Id {deviceCommand.Id} Current status should be set to {DeviceCommandStatus.DeviceCommandPushedToDevice} " +
                        $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //get the device id
                Device device = await _deviceRepository.GetDeviceById(deviceCommandUpdateRequest.DeviceId);

                if (device == null)
                {
                    _logger.LogError($"Invalid device id - {deviceCommandUpdateRequest.DeviceId} recieved for Complete Command. " +
                        $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //if command has failed
                if (deviceCommandUpdateRequest.IsFailed.Value)
                {
                    device.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandFailed;

                    deviceCommand.DeviceCommandStatus = DeviceCommandStatus.DeviceCommandFailed;

                    if (!string.IsNullOrEmpty(deviceCommandUpdateRequest.FailureReason))
                    {
                        if (deviceCommand.FailureDetails == null)
                        {
                            deviceCommand.FailureDetails = new List<string>();
                        }

                        deviceCommand.FailureDetails.Add(deviceCommandUpdateRequest.FailureReason);
                    }
                }
                else
                {
                    device.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandComplete;

                    deviceCommand.DeviceCommandStatus = DeviceCommandStatus.DeviceCommandComplete;

                    device.FilterUpdatedBy = deviceCommand.Id;
                }

                DateTime currentDateTime = DateTime.UtcNow;

                device.ModifiedDate = currentDateTime;

                deviceCommand.ModifiedDate = currentDateTime;

                DeviceCommandExecutionDetails deviceCommandExecutionDetails =
                        new DeviceCommandExecutionDetails() { EventName = deviceCommand.DeviceCommandStatus.ToString(), EventDateTime = currentDateTime };

                deviceCommand.ExecutionDetails.Add(deviceCommandExecutionDetails);

                await _deviceRepository.UpdateInBatch(device, deviceCommand);

                return;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to complete command. Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} Exception {ex.Message}";

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Changes the device state to provisioned
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        private async Task ProvisionDevice(IOTTelemetryCommandIntegrationEvent integrationEvent)
        {
            try
            {
                _logger.LogInformation($"Provision Device data : {integrationEvent.Telemetry.TelemetryCommandData.CommandData}." +
                    $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                ProvisionDeviceRequest provisionDeviceRequest =
                    JsonSerializer.Deserialize<ProvisionDeviceRequest>(integrationEvent.Telemetry.TelemetryCommandData.CommandData,
                                                                           Utilties.GetJsonSerializerOptions());

                //perform the data validation
                if (string.IsNullOrEmpty(provisionDeviceRequest.DeviceId))
                {
                    _logger.LogError($"Invalid data recieved for Provision Device. Command Data : {integrationEvent.Telemetry.TelemetryCommandData.CommandData} " +
                        $" Device Id is mandatory. Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //get device id
                var device = await _deviceRepository.GetDeviceById(provisionDeviceRequest.DeviceId);

                if (device == null)
                {
                    _logger.LogError($"Invalid device id - {provisionDeviceRequest.DeviceId} recieved for Provision Device. " +
                        $"Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                device.DeviceStatus = DeviceStatus.Provisioned;
                device.DeviceStatusUpdatedOn = DateTime.UtcNow;
                device.ModifiedDate = DateTime.UtcNow;

                await _deviceRepository.UpdateDevice(device);

                return;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to provision device. Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} Exception {ex.Message}";

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Maintaines the device to content mapping
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        private async Task ProcessDeviceContentRequest(IOTTelemetryCommandIntegrationEvent integrationEvent,
                                                                    bool isDeleted)
        {
            try
            {
                _logger.LogInformation($"Process Device Content data : {integrationEvent.Telemetry.TelemetryCommandData.CommandData}." +
                    $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                DeviceContentUpdateRequest deviceContentRequest =
                    JsonSerializer.Deserialize<DeviceContentUpdateRequest>(integrationEvent.Telemetry.TelemetryCommandData.CommandData,
                                                                           Utilties.GetJsonSerializerOptions());

                //perform the data validation
                if (string.IsNullOrEmpty(deviceContentRequest.DeviceId) ||
                    deviceContentRequest.Contents == null ||
                    deviceContentRequest.Contents.Count <= 0)
                {
                    _logger.LogError($"Invalid data recieved for Device Content Mapping. " +
                        $"Device Id and Content Id(s) is mandatory. Command Data : {integrationEvent.Telemetry.TelemetryCommandData.CommandData} Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //Get Device Id
                var device = await _deviceRepository.GetDeviceById(deviceContentRequest.DeviceId);

                if (device == null)
                {
                    _logger.LogError($"Invalid device id - {deviceContentRequest.DeviceId} recieved for Device Content Mapping. " +
                        $"Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");

                    return;
                }

                //make a call to content proxy to validate content id and get the content provider id
                List<ContentInfo> contentInfos = await _contentProxy.GetContentProviderIds(deviceContentRequest.Contents.Select(x => x.ContentId).ToList());

                List<string> failedContents = new List<string>();

                foreach (var contentdata in deviceContentRequest.Contents)
                {
                    Guid contentProviderId = contentInfos.Where(x => x.ContentId == contentdata.ContentId).Select(x => x.ContentProviderId).FirstOrDefault();

                    if (contentProviderId == default(Guid))
                    {
                        failedContents.Add($"Invalid Content Id {contentdata.ContentId}");

                        continue;
                    }

                    try
                    {
                        DeviceContent deviceContent = await _deviceRepository.GetDeviceContent(contentdata.ContentId, deviceContentRequest.DeviceId);

                        var curDate = DateTime.UtcNow;

                        //if there is no existing record for deviceid and content id, insert
                        if (deviceContent == null)
                        {
                            deviceContent = new DeviceContent()
                            {
                                DeviceId = deviceContentRequest.DeviceId,
                                ContentId = contentdata.ContentId,
                                ContentProviderId = contentProviderId,
                                IsDeleted = isDeleted,
                                OperationTimeStamp = contentdata.OperationTime,
                                CreatedDate = curDate,
                                ModifiedDate = curDate
                            };

                            //insert record
                            await _deviceRepository.CreateDeviceContent(deviceContent);
                        }
                        else
                        {
                            //if the record already exists. update only if the data base record is not latest than the given one
                            if (deviceContent.OperationTimeStamp < contentdata.OperationTime)
                            {
                                deviceContent.IsDeleted = isDeleted;
                                deviceContent.ContentProviderId = contentProviderId;
                                deviceContent.OperationTimeStamp = contentdata.OperationTime;
                                deviceContent.ModifiedDate = curDate;

                                //update the record
                                await _deviceRepository.UpdateDeviceContent(deviceContent);
                            }
                            else
                            {
                                failedContents.Add($"Skipping device to content mapping for {contentdata.ContentId} and {deviceContent.DeviceId}. Database operation time is latest. " +
                                    $" Recieved operation time " +
                                    $" {contentdata.OperationTime.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDDHHmmssfff)} " +
                                    $" Database operation time {deviceContent.OperationTimeStamp.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDDHHmmssfff)} ");
                            }
                        }
                    }
                    catch (Exception ex) //swallow the exception and continue for other content ids
                    {
                        failedContents.Add($"Exception occurred for {contentdata.ContentId}. Ex: {ex.Message}");

                        _logger.LogError($"Exception occurred for - {contentdata.ContentId}. Ex: {ex.ToString()} " +
                                                $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");
                    }

                }

                //in case there are failed mappings, report it to the application insights
                if (failedContents.Count >= 0)
                {
                    _logger.LogError($"Failed or skipped device content mapping for - {string.Join("|", failedContents)}." +
                                                $" Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} ");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to update content to device mapping. Device Id : {integrationEvent.DeviceId} Template Id : {integrationEvent.TemplateId} Module Id : {integrationEvent.Module} Exception {ex.Message}";

                _logger.LogError(ex, $"{errorMessage}");
            }

        }
    }
}
