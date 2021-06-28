using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.user.api.Models
{
    public class UpdateProfileRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
