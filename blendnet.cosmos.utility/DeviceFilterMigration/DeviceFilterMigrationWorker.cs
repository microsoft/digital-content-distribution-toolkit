// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.common.dto.Device;
using blendnet.cosmos.utility.Repository;
using blendnet.device.repository.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility.DeviceFilterMigration
{
    public class DeviceFilterMigrationWorker
    {
        private GenericRepository _genericRepository;

        private IDeviceRepository _deviceRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<DeviceFilterMigrationWorker> _logger;


        public DeviceFilterMigrationWorker(IMapper mapper,
                                        ILogger<DeviceFilterMigrationWorker> logger,
                                        GenericRepository genericRepository,
                                        IDeviceRepository deviceRepository)
        {
            _mapper = mapper;

            _logger = logger;

            _genericRepository = genericRepository;

            _deviceRepository = deviceRepository;

        }

        /// <summary>
        /// Peform the data migration
        /// </summary>
        /// <returns></returns>
        public async Task DoWork()
        {
            List<OldDevice> existingDevices = await GetUpdatedDevices();

            DeviceCommand deviceCommand = null;

            Device newDevice = null;

            foreach (OldDevice exisingDevice in existingDevices)
            {
                try
                {
                    deviceCommand = await _deviceRepository.GetDeviceCommandById(exisingDevice.FilterUpdatedBy.Value,
                                                         exisingDevice.Id);

                    newDevice = _mapper.Map<OldDevice, Device>(exisingDevice);

                    FilterUpdatedBy filterUpdatedBy = new FilterUpdatedBy()
                    {
                        CommandId = deviceCommand.Id.Value,
                        FilterUpdateRequest = deviceCommand.FilterUpdateRequest
                    };

                    newDevice.FilterUpdatedBy = filterUpdatedBy;
                    
                    await _deviceRepository.UpdateDevice(newDevice);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to update DeviceId {exisingDevice.Id} Command Id {deviceCommand.Id}");
                }

            }
        }

        /// <summary>
        /// Get Contents which are in Broadcasted state or Cancelled State
        /// </summary>
        /// <returns></returns>
        private async Task<List<OldDevice>> GetUpdatedDevices()
        {
            string query = "select * from c where c.deviceContainerType = 'Device' AND c.filterUpdateStatus IN ('DeviceCommandComplete')";

            List<OldDevice> existingDevices = await _genericRepository.GetList<OldDevice>("blendnetdev", "Device", query);

            return existingDevices;
        }
    }
}
