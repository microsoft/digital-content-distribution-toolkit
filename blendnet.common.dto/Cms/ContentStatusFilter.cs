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

        public bool IsContentUploadStatusesPresent
        {
            get
            {
                if (ContentUploadStatuses != null &&
                   ContentUploadStatuses.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsContentTransformStatusesPresent
        {
            get
            {
                if (ContentTransformStatuses != null &&
                   ContentTransformStatuses.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsContentBroadcastStatusesPresent
        {
            get
            {
                if (ContentBroadcastStatuses != null &&
                   ContentBroadcastStatuses.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
    
}
