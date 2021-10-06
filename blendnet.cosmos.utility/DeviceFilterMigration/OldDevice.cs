using blendnet.common.dto;
using blendnet.common.dto.Device;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility.DeviceFilterMigration
{
    public class OldDevice : BaseDto
    {
        private string _id;
        /// <summary>
        /// Unique Content Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        [Required]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumeric)]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value.ToUpper();
            }
        }

        /// <summary>
        /// Same as Id
        /// </summary>
        public string DeviceId
        {
            get
            {
                return Id;
            }
        }

        /// <summary>
        /// Same as Id
        /// </summary>
        public DeviceStatus DeviceStatus { get; set; } = DeviceStatus.Registered;

        /// <summary>
        /// Date when the device state is updated
        /// </summary>
        public DateTime? DeviceStatusUpdatedOn { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public DeviceContainerType DeviceContainerType
        {
            get
            {
                return DeviceContainerType.Device;
            }
        }

        /// <summary>
        /// Current filter update command status
        /// </summary>
        public DeviceCommandStatus? FilterUpdateStatus { get; set; } = DeviceCommandStatus.DeviceCommandNotInitialized;

        /// <summary>
        /// command id which has updated the filter status 
        /// </summary>
        public Guid? FilterUpdateStatusUpdatedBy { get; set; }

        /// <summary>
        /// Command which updated the filter sucessfully
        /// </summary>
        public Guid? FilterUpdatedBy { get; set; }

        /// <summary>
        /// Sets the default values
        /// </summary>
        public void SetDefaults()
        {
            this.ModifiedByByUserId = null;

            this.ModifiedDate = null;

            this.FilterUpdatedBy = null;

            this.FilterUpdateStatus = DeviceCommandStatus.DeviceCommandNotInitialized;

            this.FilterUpdateStatusUpdatedBy = null;
        }

    }
}
