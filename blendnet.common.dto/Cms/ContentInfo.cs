using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Cms
{
    public class ContentInfo
    {
        /// <summary>
        /// Content id
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Content Provider Id
        /// </summary>
        public Guid ContentProviderId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
    }
}
