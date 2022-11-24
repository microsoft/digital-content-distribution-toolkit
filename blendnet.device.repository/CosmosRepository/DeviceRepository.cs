// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Common;
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
            try
            {
                await this._container.CreateItemAsync<Device>(device, new PartitionKey(device.DeviceId));

                return device.Id;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return string.Empty;
            }
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
        /// Returns all the devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Device>> GetDevices()
        {
            var queryString = $"select * from c where c.deviceContainerType = @type order by c.createdDate desc";
            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@type", DeviceContainerType.Device);

            var devices = await this._container.ExtractDataFromQueryIterator<Device>(queryDef);

            return devices;
        }

        /// <summary>
        /// Get Device By Ids
        /// https://github.com/Azure/azure-cosmosdb-node/issues/156
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        public async Task<List<Device>> GetDeviceByIds(List<string> deviceIds)
        {
            List<Device> deviceList = new List<Device>();

            var queryString = $"SELECT * FROM c WHERE ARRAY_CONTAINS(@deviceIds, c.deviceId) AND c.deviceContainerType = @type";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", DeviceContainerType.Device);

            queryDef.WithParameter("@deviceIds", deviceIds);

            deviceList = await this._container.ExtractDataFromQueryIterator<Device>(queryDef);

            return deviceList;
        }

        #region Device Commands
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
        /// Returns the list of given commands based on device and command type
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceCommandType"></param>
        /// <returns></returns>
        public async Task<List<DeviceCommand>> GetDeviceCommands(string deviceId, DeviceCommandType deviceCommandType)
        {
            List<DeviceCommand> deviceCommandList = new List<DeviceCommand>();

            var queryString = $"SELECT * FROM c WHERE c.deviceId = @deviceId AND c.deviceContainerType = @type AND c.deviceCommandType = @commandType";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@deviceId", deviceId);
            queryDef.WithParameter("@type", DeviceContainerType.Command);
            queryDef.WithParameter("@commandType", deviceCommandType);

            deviceCommandList = await this._container.ExtractDataFromQueryIterator<DeviceCommand>(queryDef);

            return deviceCommandList;
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

                    throw batchResponse.GetTransactionalBatchException(errorMessage);
                }
            }
        }

        #endregion

        #region Device Content
        
        /// <summary>
        /// Gets content by device id and content id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceContent> GetContentByDeviceIdContentId(Guid contentId, string deviceId)
        {
            try
            {
                ItemResponse<DeviceContent> response = await this._container.ReadItemAsync<DeviceContent>(contentId.ToString(), new PartitionKey(deviceId));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets list of device contents by device id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<List<DeviceContent>> GetContentByDeviceId(string deviceId)
        {
            var query = $"SELECT * FROM c WHERE c.deviceId = @deviceId AND c.deviceContainerType = @deviceContainerType AND c.isDeleted = false ";

            var queryDef = new QueryDefinition(query);

            queryDef.WithParameter("@deviceId", deviceId);

            queryDef.WithParameter("@deviceContainerType", DeviceContainerType.DeviceContent.ToString());

            var deviceContentList = await this._container.ExtractDataFromQueryIterator<DeviceContent>(queryDef);

            return deviceContentList;
        }

        /// <summary>
        /// Returns the Content by device id, content provider id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public async Task<ResultData<DeviceContent>> GetContentByDeviceId(string deviceId, 
                                                                            List<Guid> contentProviderIds, 
                                                                            string continuationToken,
                                                                            int pageSize,
                                                                            bool activeOnly = true)
        {
            ResultData<DeviceContent> contentResult;

            QueryDefinition queryDef = GetContentByDeviceIdQueryDef(deviceId, contentProviderIds, activeOnly);

            continuationToken = String.IsNullOrEmpty(continuationToken) ? null : continuationToken;

            contentResult = await this._container.ExtractDataFromQueryIteratorWithToken<DeviceContent>(queryDef, continuationToken, pageSize);

            return contentResult;
        }

        /// <summary>
        /// Returns all the Content by device id, content provider id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        public async Task<List<DeviceContent>> GetContentByDeviceId(  string deviceId,
                                                                List<Guid> contentProviderIds,
                                                                bool activeOnly = true)
        {
            List<DeviceContent> contentResult;

            QueryDefinition queryDef = GetContentByDeviceIdQueryDef(deviceId, contentProviderIds, activeOnly);

            contentResult = await this._container.ExtractDataFromQueryIterator<DeviceContent>(queryDef);

            return contentResult;
        }


        /// <summary>
        /// Returns query definition
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        private QueryDefinition GetContentByDeviceIdQueryDef(string deviceId, 
                                                    List<Guid> contentProviderIds,
                                                    bool activeOnly)
        {
            string queryString = $"SELECT * FROM c where c.deviceId = @deviceId " +
                                  " AND ARRAY_CONTAINS(@contentProviderIds, c.contentProviderId)" +
                                  " AND c.deviceContainerType = @deviceContainerType ";

            if (activeOnly)
            {
                queryString = $" {queryString} AND c.isDeleted = @isDeleted";
            }

            QueryDefinition queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@deviceId", deviceId);

            queryDef.WithParameter("@contentProviderIds", contentProviderIds);

            queryDef.WithParameter("@deviceContainerType", DeviceContainerType.DeviceContent.ToString());

            if (activeOnly)
            {
                queryDef.WithParameter("@isDeleted", !activeOnly);
            }

            return queryDef;
        }


        /// <summary>
        /// Returns the content availability on given device ids
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        public async Task<List<DeviceContent>> GetContentAvailability(Guid contentId, List<string> deviceIds)
        {
            var query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(@deviceIds, c.deviceId) " +
                        $" AND c.id = @contentId " +
                        $" AND c.deviceContainerType = @deviceContainerType" +
                        $" AND c.isDeleted = false";

            var queryDef = new QueryDefinition(query);

            queryDef.WithParameter("@deviceIds", deviceIds);

            queryDef.WithParameter("@contentId", contentId);

            queryDef.WithParameter("@deviceContainerType", DeviceContainerType.DeviceContent.ToString());

            var deviceContentList = await this._container.ExtractDataFromQueryIterator<DeviceContent>(queryDef);

            return deviceContentList;
        }

        /// <summary>
        /// Returns the device content availability count on given device ids
        /// This is an approximate count.
        /// Case where Broadcast has cancelled and content is not deleted, then it will appear in this list
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        public async Task<List<DeviceContentAvailability>> GetContentAvailabilityCount(List<string> deviceIds)
        {
            var query = $"SELECT COUNT(c) AS ActiveContentCount, c.deviceId FROM c " +
                        $" WHERE ARRAY_CONTAINS(@deviceIds, c.deviceId) " +
                        $" AND c.deviceContainerType = @deviceContainerType " +
                        $" AND c.isDeleted = false " +
                        $" Group by c.deviceId ";

            var queryDef = new QueryDefinition(query);

            queryDef.WithParameter("@deviceIds", deviceIds);

            queryDef.WithParameter("@deviceContainerType", DeviceContainerType.DeviceContent.ToString());

            var deviceContentAvailabilityList = await this._container.ExtractDataFromQueryIterator<DeviceContentAvailability>(queryDef);

            return deviceContentAvailabilityList;
        }

        /// <summary>
        /// Returns list of failed upsert device contents 
        /// </summary>
        /// <param name="deviceContent"></param>
        /// <returns></returns>
        public async Task<int> UpsertDeviceContent(DeviceContent deviceContent)
        {
            var response = await _container.UpsertItemAsync<DeviceContent>(deviceContent, new PartitionKey(deviceContent.DeviceId));
            return (int)response.StatusCode;
        }

        /// <summary>
        /// Creates Device Content Mapping
        /// </summary>
        /// <param name="deviceContent"></param>
        /// <returns></returns>
        public async Task<Guid> CreateDeviceContent(DeviceContent deviceContent)
        {
            await this._container.CreateItemAsync<DeviceContent>(deviceContent, new PartitionKey(deviceContent.DeviceId.ToString()));

            return deviceContent.ContentId;
        }

        /// <summary>
        /// Updates Device Content
        /// </summary>
        /// <param name="deviceContent"></param>
        /// <returns></returns>
        public async Task<int> UpdateDeviceContent(DeviceContent deviceContent)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<DeviceContent>(deviceContent,
                                                                                        deviceContent.ContentId.ToString(),
                                                                                        new PartitionKey(deviceContent.DeviceId));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }

        }

        /// <summary>
        /// Get Device Content
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceContent> GetDeviceContent(Guid contentId, string deviceId)
        {
            try
            {
                ItemResponse<DeviceContent> response = await this._container.ReadItemAsync<DeviceContent>(contentId.ToString(), 
                                                                                                            new PartitionKey(deviceId));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Get Exception.
        /// </summary>
        /// <param name="batchResponse"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private BlendNetCosmosTransactionalBatchException GetTransactionalBatchException(TransactionalBatchResponse batchResponse,
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
