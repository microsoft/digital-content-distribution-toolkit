using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.Util
{
    public class IncentiveUtil
    {
        public static PlanDetail GetPlanDetailForEvent(IncentivePlan incentivePlan, EventType eventType)
        {
            if (incentivePlan == null) return null;

            List<PlanDetail> planDetails = incentivePlan.PlanDetails.Where(plan => plan.EventType == eventType).ToList();

            if (planDetails.Count == 0)
            {
                return null;
            }

            return planDetails[0]; //Ideally only one plan should be present
        }

        public static double GetComputedValue(double originalValue, Formula formula)
        {
            switch (formula.FormulaType)
            {
                case FormulaType.PLUS:
                    return originalValue + formula.RightOperand;
                case FormulaType.MINUS:
                    return originalValue - formula.RightOperand;
                case FormulaType.MULTIPLY:
                    return originalValue * formula.RightOperand;
                case FormulaType.PERCENTAGE:
                    return originalValue * formula.RightOperand / 100.0;
            }

            return 0;
        }
    }
}
