using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.api.Model
{
    public class BroadcastContentRequest
    {
        [Required]
        public List<Guid> ContentIds { get; set; }

        [Required]
        public List<string> Filters { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
