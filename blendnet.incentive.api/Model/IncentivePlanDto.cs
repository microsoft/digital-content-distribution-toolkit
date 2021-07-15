using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.api.Model
{
    public class IncentivePlanDto
    {
        /// <summary>
        /// Unique id of the plan
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Name given to plan
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// Type of the plan
        /// </summary>
        public PlanType PlanType { get; set; }

        /// <summary>
        /// Start date of the plan
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the plan
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Target audience of the plan
        /// </summary>
        public Audience Audience { get; set; }

        /// <summary>
        /// List of events and corresponding formula associated with plan
        /// </summary>
        public List<PlanDetailDto> PlanDetails { get; set; }

    }
}
