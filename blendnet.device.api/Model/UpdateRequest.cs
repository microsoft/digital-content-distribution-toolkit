using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.device.api.Model
{
    public class UpdateRequest
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
