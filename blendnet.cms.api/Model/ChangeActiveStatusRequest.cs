using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.api.Model
{
    public class ChangeActiveStatusRequest
    {
        [Required]
        public bool Status { get; set; }
    }
}
