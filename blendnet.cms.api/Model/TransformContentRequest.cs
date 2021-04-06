using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.api.Model
{
    /// <summary>
    /// List of Content Ids to transform
    /// </summary>
    public class TransformContentRequest
    {
        [Required]
        public List<Guid> ContentIds { get; set; }
    }
}
