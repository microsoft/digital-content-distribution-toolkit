using blendnet.common.dto.Incentive;
using System.Collections.Generic;

namespace blendnet.incentive.api.Model
{
    public class CalculatedIncentivePlan
    {
        public IncentivePlan IncentivePlan { get; set; }

        public List<CalculatedPlanDetails> CalculatedPlanDetails  { get;set;}
    }

    public class CalculatedPlanDetails
    {
        public string CalculatedFor { get; set; }  

        public List<PlanDetail> PlanDetails { get; set; }

    }
}
