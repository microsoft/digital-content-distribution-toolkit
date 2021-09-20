using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class DeviceContentUpdateRequest
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public List<ContentData> Contents { get; set; }

    }

    public class ContentData
    {
        [Required]
        public Guid ContentId { get; set; }

        [Required]
        public DateTime OperationTime { get; set; }
    }
}
