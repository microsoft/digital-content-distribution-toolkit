using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.user.api.Models
{
    public class UpdateProfileRequest
    {
        [Required] [StringLength(UserProfile.NameMaxLength)]
        public string Name { get; set; }
    }
}
