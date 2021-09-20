using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class DeviceCommandUpdateRequest
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public Guid? CommandId { get; set; }

        [Required]
        public bool? IsFailed { get; set; }

        public string FailureReason { get; set; }
    }
}
