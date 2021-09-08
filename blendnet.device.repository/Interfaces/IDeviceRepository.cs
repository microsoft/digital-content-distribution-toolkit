using blendnet.common.dto.Common;
using blendnet.common.dto.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.device.repository.Interfaces
{
    /// <summary>
    /// Device Repository Deals with Device Collection.
    /// The same collection stores the Device and DeviceCommand and DeviceContent.
    /// Id is Device Id or Command Id.
    /// Partition key is Device Id.
    /// </summary>
    public interface IDeviceRepository
    {
        /// <summary>
        /// Create Device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task<string> CreateDevice(Device device);

        /// <summary>
        /// Delete Device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<int> DeleteDevice(string deviceId);

        /// <summary>
        /// Get Device by Device Id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<Device> GetDeviceById(string deviceId);

        /// <summary>
        /// Returns the list of device by Ids
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        Task<List<Device>> GetDeviceByIds(List<string> deviceIds);

        /// <summary>
        /// Update Device
        /// </summary>
        /// <param name="updatedDevice"></param>
        /// <returns></returns>
        Task<int> UpdateDevice(Device updatedDevice);

        /// <summary>
        /// Returns the list of all devices
        /// </summary>
        /// <returns></returns>
        Task<List<Device>> GetDevices();

        /// <summary>
        /// Create Device Command. Id is command id, partion key is device id
        /// </summary>
        /// <param name="deviceCommand"></param>
        /// <returns></returns>
        Task<Guid> CreateDeviceCommand(DeviceCommand deviceCommand);

        /// <summary>
        ///  Update Device Command
        /// </summary>
        /// <param name="updatedDevice"></param>
        /// <returns></returns>
        Task<int> UpdateDeviceCommand(DeviceCommand updatedDeviceCommand);

        /// <summary>
        /// Get device command by command id and device id
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<DeviceCommand> GetDeviceCommandById(Guid commandId, string deviceId);

        /// <summary>
        /// Returns the list of commands based on given device id and command type
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceCommandType"></param>
        /// <returns></returns>
        Task<List<DeviceCommand>> GetDeviceCommands(string deviceId, DeviceCommandType deviceCommandType);

        /// <summary>
        /// Updates both in once batch / transaction
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceCommand"></param>
        /// <returns></returns>
        Task UpdateInBatch(Device device, DeviceCommand deviceCommand);

        /// <summary>
        /// Adds or updates content for a particular device
        /// </summary>
        /// <param name="deviceContent"></param>
        /// <returns></returns>
        Task<int> UpsertDeviceContent(DeviceContent deviceContents);

        /// <summary>
        /// Returns list of contents available for this device id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<List<DeviceContent>> GetContentByDeviceId(string deviceId);


        /// <summary>
        /// Return the content on device for the given device and content provider id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="continuationToken"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        Task<ResultData<DeviceContent>> GetContentByDeviceId(string deviceId,
                                                              Guid contentProviderId,
                                                              string continuationToken,
                                                              bool activeOnly);

        /// <summary>
        /// Gets device content by device id and content id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<DeviceContent> GetContentByDeviceIdContentId(Guid contentId, string deviceId);

        /// <summary>
        /// Returns the content availability on given device ids
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        Task<List<DeviceContent>> GetContentAvailability(Guid contentId, List<string> deviceIds);
    }
}
