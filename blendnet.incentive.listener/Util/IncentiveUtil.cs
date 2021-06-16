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

            List<PlanDetail> planDetails = incentivePlan.PlanDetails.Where(plan => plan.EventType == eventType && plan.EventSubType == eventSubType).ToList();

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
                    return originalValue + formula.FirstOperand;
                case FormulaType.MINUS:
                    return originalValue - formula.FirstOperand;
                case FormulaType.MULTIPLY:
                    return originalValue * formula.FirstOperand;
                case FormulaType.PERCENTAGE:
                    return originalValue * formula.FirstOperand / 100.0;
            }

            return 0;
        }

        /// <summary>
        /// Creates basic incentive event 
        /// </summary>
        /// <returns></returns>
        public static IncentiveEvent CreateIncentiveEvent()
        {
            var curDate = DateTime.UtcNow;
            IncentiveEvent incentiveEvent = new IncentiveEvent();
            incentiveEvent.EventId = Guid.NewGuid();
            incentiveEvent.EventDateTime = incentiveEvent.EventOccuranceTime = curDate;
            incentiveEvent.EventDate = Int32.Parse(curDate.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDD));
            incentiveEvent.CreatedDate = curDate;
            incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
            return incentiveEvent;
        }
    }
}
