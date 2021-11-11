using blendnet.common.dto.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.device.api.Model
{
    public class ContentAvailabilityRequest
    {
        [Required]
        public Guid ContentId { get; set; }

        [Required]
        public List<string> DeviceIds { get; set; }
    }


    public class ContentAvailabilityCountRequest
    {
        [Required]
        public List<string> DeviceIds { get; set; }
    }
}
