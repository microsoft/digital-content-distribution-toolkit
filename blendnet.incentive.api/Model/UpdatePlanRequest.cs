using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.api.Model
{
    public class UpdatePlanRequest
    {

        /// <summary>
        /// Unique id of the plan
        /// </summary>
        [Required]
        public PlanType PlanType { get; set; }

        /// <summary>
        /// SubType name of the audience of the plan
        /// </summary>
        [Required]
        public string SubTypeName { get; set; }

        /// <summary>
        /// End date of the plan
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
    }
}
