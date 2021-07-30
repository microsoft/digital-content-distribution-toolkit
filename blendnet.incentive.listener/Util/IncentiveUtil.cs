using blendnet.common.dto;
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
        public static PlanDetail GetPlanDetailForEvent(IncentivePlan incentivePlan, EventType eventType, string eventSubType)
        {
            if (incentivePlan == null) return null;

            PlanDetail planDetail = incentivePlan.PlanDetails
                                                .Where(plan => plan.EventType == eventType && plan.EventSubType == eventSubType)
                                                .FirstOrDefault();

            return planDetail;
        }

        public static double GetComputedValue(double originalValue, Formula formula)
        {
            switch (formula.FormulaType)
            {
                case FormulaType.PLUS:
                    return originalValue + formula.FirstOperand.Value;
                case FormulaType.MINUS:
                    return originalValue - formula.FirstOperand.Value;
                case FormulaType.MULTIPLY:
                    return originalValue * formula.FirstOperand.Value;
                case FormulaType.PERCENTAGE:
                    return originalValue * formula.FirstOperand.Value / 100.0;
            }

            return 0;
        }

        /// <summary>
        /// Creates basic incentive event 
        /// </summary>
        /// <returns></returns>
        public static IncentiveEvent CreateIncentiveEvent(EventCategoryType eventCategoryType = EventCategoryType.INCOME)
        {
            var curDate = DateTime.UtcNow;
            IncentiveEvent incentiveEvent = new IncentiveEvent();
            incentiveEvent.EventId = Guid.NewGuid();
            incentiveEvent.EventOccuranceTime = curDate;
            incentiveEvent.CreatedDate = curDate;
            incentiveEvent.EventCategoryType = eventCategoryType;
            return incentiveEvent;
        }
    }
}
