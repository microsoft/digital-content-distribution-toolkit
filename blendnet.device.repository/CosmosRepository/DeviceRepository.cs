using blendnet.common.dto;
using blendnet.common.dto.Device;
using blendnet.common.dto.Exceptions;
using blendnet.common.infrastructure.Extensions;
using blendnet.device.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.device.repository.CosmosRepository
{
    /// <summary>
    /// Device Repository
    /// </summary>
    public class DeviceRepository:IDeviceRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        DeviceAppSettings _appSettings;

        public DeviceRepository(CosmosClient dbClient,
                                IOptionsMonitor<DeviceAppSettings> optionsMonitor,
                                ILogger<DeviceRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.Device);
        }

        /// <summary>
        /// Create Device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public async Task<string> CreateDevice(Device device)
        {
            await this._container.CreateItemAsync<Device>(device, new PartitionKey(device.DeviceId));

            return device.Id;
        }

        /// <summary>
        /// Delete Device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<int> DeleteDevice(string deviceId)
        {
            try
            {
                var response = await this._container.DeleteItemAsync<Device>(deviceId, new PartitionKey(deviceId));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Get Device by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Device> GetDeviceById(string deviceId)
        {
            try
            {
                ItemResponse<Device> response = await this._container.ReadItemAsync<Device>(deviceId, new PartitionKey(deviceId));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Update Device
        /// </summary>
        /// <param name="updatedDevice"></param>
        /// <returns></returns>
        public async Task<int> UpdateDevice(Device updatedDevice)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<Device>(updatedDevice,
                                                                                        updatedDevice.Id,
                                                                                        new PartitionKey(updatedDevice.DeviceId));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }

        }

        /// <summary>
        /// Create device commad.
        /// Partition key is device id
        /// </summary>
        /// <param name="deviceCommand"></param>
        /// <returns></returns>
        public async Task<Guid> CreateDeviceCommand(DeviceCommand deviceCommand)
        {
            await this._container.CreateItemAsync<DeviceCommand>(deviceCommand, new PartitionKey(deviceCommand.DeviceId));

            return deviceCommand.Id.Value;
        }

        /// <summary>
        /// Update device command by command id and device id
        /// </summary>
        /// <param name="updatedDeviceCommand"></param>
        /// <returns></returns>
        public async Task<int> UpdateDeviceCommand(DeviceCommand updatedDeviceCommand)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<DeviceCommand>(updatedDeviceCommand,
                                                                                        updatedDeviceCommand.Id.Value.ToString(),
                                                                                        new PartitionKey(updatedDeviceCommand.DeviceId));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }

        }

        /// <summary>
        /// Returns the Device Command By Id
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceCommand> GetDeviceCommandById(Guid commandId, string deviceId)
        {
            try
            {
                ItemResponse<DeviceCommand> response = await this._container.ReadItemAsync<DeviceCommand>(commandId.ToString(), new PartitionKey(deviceId));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Device By Ids
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        public async Task<List<Device>> GetDeviceByIds(List<string> deviceIds)
        {
            List<Device> deviceList = new List<Device>();

            string deviceIdsData = string.Join(",", deviceIds.Select(item => "'" + item.ToString() + "'"));

            var queryString = $"SELECT * FROM c WHERE c.deviceId in ({deviceIdsData}) AND c.deviceContainerType = @type";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", DeviceContainerType.Device);

            deviceList = await this._container.ExtractDataFromQueryIterator<Device>(queryDef);

            return deviceList;
        }

        /// <summary>
        /// https://github.com/Azure/azure-cosmos-dotnet-v3/issues/1162
        /// https://docs.microsoft.com/en-us/azure/cosmos-db/transactional-batch
        /// </summary>
        /// <returns></returns>
        public async Task UpdateInBatch(Device device, DeviceCommand deviceCommand)
        {
            TransactionalBatchResponse batchResponse = await this._container.CreateTransactionalBatch(new PartitionKey(device.DeviceId))
                .ReplaceItem<Device>(device.Id, device)
                .ReplaceItem<DeviceCommand>(deviceCommand.Id.Value.ToString(),deviceCommand)
                .ExecuteAsync();
            
            using (batchResponse)
            {
                if (!batchResponse.IsSuccessStatusCode)
                {
                    string errorMessage = $"Batch update failed for Device {device.Id} and Device Command Id {deviceCommand.Id}";

                    throw GetTransactionalBatchException(batchResponse,errorMessage);
                }
            }
        }

        /// <summary>
        /// Get Exception.
        /// </summary>
        /// <param name="batchResponse"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private BlendNetCosmosTransactionalBatchException GetTransactionalBatchException(TransactionalBatchResponse batchResponse ,
                                                                                          string errorMessage)
        {
            List<string> operationStatus = new List<string>();

            for (var i = 0; i < batchResponse.Count; i++)
            {
                var result = batchResponse.GetOperationResultAtIndex<dynamic>(i);

                operationStatus.Add($"Status code for index {i} is {result.StatusCode}");
            }

            BlendNetCosmosTransactionalBatchException batchException =
                            new BlendNetCosmosTransactionalBatchException(errorMessage, 
                                                                          batchResponse.StatusCode, 
                                                                          batchResponse.ErrorMessage, 
                                                                          operationStatus);
            return batchException;
        }
    }
}
