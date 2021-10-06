using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Content Broadcasted By Details
    /// </summary>
    public class ContentBroadcastedBy
    {
        /// <summary>
        /// Command Id
        /// </summary>
        public Guid CommandId { get; set; }

        /// <summary>
        /// Details about broadcast data
        /// </summary>
        public BroadcastRequest BroadcastRequest { get; set; }
    }
}
