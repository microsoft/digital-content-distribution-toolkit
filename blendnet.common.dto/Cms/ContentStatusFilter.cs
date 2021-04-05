using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Content Status Filter
    /// </summary>
    public class ContentStatusFilter
    {
        public string[] ContentUploadStatuses { get; set; }

        public string[] ContentTransformStatuses { get; set; }

        public string[] ContentBroadcastStatuses { get; set; }
    }
    
}
