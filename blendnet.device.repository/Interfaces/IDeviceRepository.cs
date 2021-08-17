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
    interface IDeviceRepository
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
        /// Update Device
        /// </summary>
        /// <param name="updatedDevice"></param>
        /// <returns></returns>
        Task<int> UpdateDevice(Device updatedDevice);

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
    }
}
