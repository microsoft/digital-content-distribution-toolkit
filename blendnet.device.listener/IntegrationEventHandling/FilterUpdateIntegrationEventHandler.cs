using blendnet.api.proxy.IOTCentral;
using blendnet.common.dto;
using blendnet.common.dto.Device;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.device.repository.CosmosRepository;
using blendnet.device.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using blendnet.common.dto.Extensions;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.device.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the Filter Update Integration Event Handler
    /// </summary>
    public class FilterUpdateIntegrationEventHandler: IIntegrationEventHandler<FilterUpdateIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly DeviceAppSettings _appSettings;

        private IOTCentralProxy _iotCentralProxy;

        private IDeviceRepository _deviceRepository;

        public FilterUpdateIntegrationEventHandler(ILogger<FilterUpdateIntegrationEventHandler> logger,
                                                       TelemetryClient tc,
                                                       IOptionsMonitor<DeviceAppSettings> optionsMonitor,
                                                       IDeviceRepository deviceRepository,
                                                       IOTCentralProxy iotCentralProxy)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _deviceRepository = deviceRepository;

            _iotCentralProxy = iotCentralProxy;

        }

        /// <summary>
        /// Handles the Broadcast event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(FilterUpdateIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("FilterUpdateIntegrationEventHandler.Handle"))
                {
                    if (integrationEvent.FilterUpdateCommand == null ||
                        string.IsNullOrEmpty(integrationEvent.FilterUpdateCommand.DeviceId) ||
                        integrationEvent.FilterUpdateCommand.FilterUpdateRequest == null)
                    {
                        _logger.LogInformation($"No device details or Filter details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for device id: {integrationEvent.FilterUpdateCommand.DeviceId}");

                    DeviceCommand filterUpdateCommand = integrationEvent.FilterUpdateCommand;

                    Device device = await _deviceRepository.GetDeviceById(filterUpdateCommand.DeviceId);

                    if (device == null)
                    {
                        _logger.LogInformation($"No device details found in database for device id: {integrationEvent.FilterUpdateCommand.DeviceId}");

                        return;
                    }

                    DateTime currentTime = DateTime.UtcNow;

                    PopulateDeviceCommand(filterUpdateCommand, currentTime);

                    _logger.LogInformation($"Updating filter for device id: {integrationEvent.FilterUpdateCommand.DeviceId} Command Id {filterUpdateCommand.Id.Value}");

                    //Create a command record with in progress status. It will use the command id as ID and Device Id and partition key
                    Guid commandId = await _deviceRepository.CreateDeviceCommand(filterUpdateCommand);

                    device.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandInProcess;

                    device.ModifiedDate = currentTime;

                    device.FilterUpdateStatusUpdatedBy = commandId;

                    await _deviceRepository.UpdateDevice(device);

                    //Perform the filter update
                    await ExecuteProxyCommand(device, filterUpdateCommand);

                    //Update the command status. In case of any error, mark it to failure state.
                    if (filterUpdateCommand.FailureDetails.Count > 0)
                    {
                        await UpdateFailedStatus(device, filterUpdateCommand);
                    }
                    else
                    {
                        await UpdateInProgressStatus(device, filterUpdateCommand);
                    }

                    _logger.LogInformation($"Filter update command complete for content id: {integrationEvent.FilterUpdateCommand.DeviceId} command id : {filterUpdateCommand.Id.Value}");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Executes proxy command on device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceCommand"></param>
        /// <returns></returns>
        private async Task ExecuteProxyCommand(Device device, DeviceCommand deviceCommand)
        {
            string errorMessage;

            try
            {
                //proxy module request
                IOTModuleCommandRequest<ProxyModuleCommandRequest> proxyModuleCommandRequest = new IOTModuleCommandRequest<ProxyModuleCommandRequest>();
                proxyModuleCommandRequest.ConnectionTimeout = _appSettings.IOTCAPIConnectionTimeOutInSecs;
                proxyModuleCommandRequest.ResponseTimeout = _appSettings.IOTCAPIResponseTimeOutInSecs;
                proxyModuleCommandRequest.Request = new ProxyModuleCommandRequest();
                proxyModuleCommandRequest.Request.CommandName = ApplicationConstants.HubProxyModuleCommandNames.FilterUpdate;
                proxyModuleCommandRequest.Request.ConnectionTimeOutInMts = _appSettings.IOTCAPIProxyTimeOutInMts;
                proxyModuleCommandRequest.Request.ModuleClientName = ApplicationConstants.HubProxyModuleClientNames.HubServer;
                
                //this is passed to hub server module as pay load
                ProxyModuleFilterUpdatePayload filterUpdatePayload = new ProxyModuleFilterUpdatePayload();
                filterUpdatePayload.CommandId = deviceCommand.Id.Value;
                filterUpdatePayload.DeviceId = deviceCommand.DeviceId;
                filterUpdatePayload.Filters = deviceCommand.FilterUpdateRequest.Filters;
                proxyModuleCommandRequest.Request.Payload = JsonSerializer.Serialize(filterUpdatePayload,Utilties.GetJsonSerializerOptions());

                RunModuleCommandRequest<ProxyModuleCommandRequest> runModuleRequest = new RunModuleCommandRequest<ProxyModuleCommandRequest>();
                runModuleRequest.APIToken = _appSettings.IOTCAPIToken;
                runModuleRequest.APIVersion = _appSettings.IOTCAPIVersion;
                runModuleRequest.CommandName = _appSettings.IOTCAPIProxyCommandName;
                runModuleRequest.ModuleName = _appSettings.IOTCAPIProxyModuleName;
                runModuleRequest.DeviceName = deviceCommand.DeviceId;
                runModuleRequest.IOTModuleCommandRequest = proxyModuleCommandRequest;
                
                IOTModuleCommandResponse<ProxyModuleCommandRequest, ProxyModuleCommandResponse> runModuleResponse =
                                                    await _iotCentralProxy.RunModuleCommand<ProxyModuleCommandRequest,ProxyModuleCommandResponse>(runModuleRequest);

                //It means command has failed on hub server or proxy is not able to connect to hub server.
                if (runModuleResponse.Response.Status == 0)
                {
                    errorMessage = $"Filter update Command failed for device id: {deviceCommand.DeviceId} " +
                        $"                              Command Id : {deviceCommand.Id} " +
                        $"                              Result : {runModuleResponse.Response.Result} " +
                        $"                              Response Code : {runModuleResponse.ResponseCode}";

                    _logger.LogError(errorMessage);

                    deviceCommand.FailureDetails.Add(errorMessage);
                }else
                {
                    _logger.LogInformation($"Filter update Command success on device id: {deviceCommand.DeviceId} Command Id : {deviceCommand.Id} Result : {runModuleResponse.Response.Result} Response Code : {runModuleResponse.ResponseCode}");
                }

            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to execute command for {device.DeviceId} Command {deviceCommand.Id.Value} Exception {ex.Message}";

                deviceCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Populate device command object with required attributes
        /// </summary>
        /// <param name="contentCommand"></param>
        private void PopulateDeviceCommand(DeviceCommand deviceCommand, DateTime currentDateTime)
        {
            deviceCommand.Id = Guid.NewGuid();
            deviceCommand.DeviceCommandType = DeviceCommandType.FilterUpdate;
            deviceCommand.DeviceCommandStatus = DeviceCommandStatus.DeviceCommandInProcess;
            deviceCommand.CreatedDate = currentDateTime;
            deviceCommand.ModifiedDate = currentDateTime;
            deviceCommand.FailureDetails = new List<string>();

            DeviceCommandExecutionDetails commandExecutionDetails = new DeviceCommandExecutionDetails();

            commandExecutionDetails.EventName = DeviceCommandStatus.DeviceCommandInProcess.ToString();

            commandExecutionDetails.EventDateTime = currentDateTime;

            deviceCommand.ExecutionDetails = new List<DeviceCommandExecutionDetails>();

            deviceCommand.ExecutionDetails.Add(commandExecutionDetails);
        }


        /// <summary>
        /// Update Failed Status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <returns></returns>

        private async Task UpdateFailedStatus(Device device, DeviceCommand filterUpdateCommand)
        {
            device.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandPushToDeviceFailed;

            filterUpdateCommand.DeviceCommandStatus = DeviceCommandStatus.DeviceCommandPushToDeviceFailed;

            await UpdateStatus(device, filterUpdateCommand);
        }


        /// <summary>
        /// UpdateInProgressStatus
        /// </summary>
        /// <param name="device"></param>
        /// <param name="filterUpdateCommand"></param>
        /// <returns></returns>
        private async Task UpdateInProgressStatus(Device device, DeviceCommand filterUpdateCommand)
        {
            device.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandPushedToDevice;

            filterUpdateCommand.DeviceCommandStatus = DeviceCommandStatus.DeviceCommandPushedToDevice;

            await UpdateStatus(device, filterUpdateCommand);
        }

        /// <summary>
        /// UpdateStatus
        /// </summary>
        /// <param name="device"></param>
        /// <param name="filterUpdateCommand"></param>
        /// <returns></returns>
        private async Task UpdateStatus(Device device, DeviceCommand filterUpdateCommand)
        {
            DateTime currentTime = DateTime.UtcNow;

            device.ModifiedDate = currentTime;

            device.FilterUpdateStatusUpdatedBy = filterUpdateCommand.Id;

            filterUpdateCommand.ModifiedDate = currentTime;

            DeviceCommandExecutionDetails commandExecutionDetails = new DeviceCommandExecutionDetails();

            commandExecutionDetails.EventName = device.FilterUpdateStatus.Value.ToString();

            commandExecutionDetails.EventDateTime = currentTime;

            filterUpdateCommand.ExecutionDetails.Add(commandExecutionDetails);

            await _deviceRepository.UpdateInBatch(device, filterUpdateCommand);
        }
    }
}
