using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Incentive
{
    /// <summary>
    /// Class to carry criteria to enable search on events collection
    /// </summary>
    public class EventCriteriaRequest
    {
        /// <summary>
        /// Event generator id . It is must to pass
        /// </summary>
        public string EventGeneratorId { get; set; }

        /// <summary>
        /// Audience Type. It is must to pass
        /// </summary>
        public AudienceType AudienceType { get; set; }

        /// <summary>
        /// Event Types
        /// </summary>
        public List<EventType> EventTypes { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        public DateTime? StartDate { get; set; }
            
        /// <summary>
        /// End Date
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
