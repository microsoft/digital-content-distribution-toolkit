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

        /// <summary>
        /// Sets calculated value based on given formula
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="formula"></param>
        /// <param name="incentiveEvent"></param>
        public static void SetComputedValue(Formula formula,IncentiveEvent incentiveEvent)
        {
            switch (formula.FormulaType)
            {
                case FormulaType.PLUS:
                    {
                        incentiveEvent.CalculatedValue = incentiveEvent.OriginalValue + formula.FirstOperand.Value;

                        incentiveEvent.Entity1CalculatedValue = formula.Entity1Operand.HasValue ? incentiveEvent.OriginalValue + formula.Entity1Operand.Value : null;

                        incentiveEvent.Entity2CalculatedValue = formula.Entity2Operand.HasValue ? incentiveEvent.OriginalValue + formula.Entity2Operand.Value : null;

                        incentiveEvent.Entity3CalculatedValue = formula.Entity3Operand.HasValue ? incentiveEvent.OriginalValue + formula.Entity3Operand.Value : null;
                        
                        incentiveEvent.Entity4CalculatedValue = formula.Entity4Operand.HasValue ? incentiveEvent.OriginalValue + formula.Entity4Operand.Value : null;

                        return;
                    }
                case FormulaType.MINUS:
                    {
                        incentiveEvent.CalculatedValue = incentiveEvent.OriginalValue - formula.FirstOperand.Value;

                        incentiveEvent.Entity1CalculatedValue = formula.Entity1Operand.HasValue ? incentiveEvent.OriginalValue - formula.Entity1Operand.Value : null;

                        incentiveEvent.Entity2CalculatedValue = formula.Entity2Operand.HasValue ? incentiveEvent.OriginalValue - formula.Entity2Operand.Value : null;

                        incentiveEvent.Entity3CalculatedValue = formula.Entity3Operand.HasValue ? incentiveEvent.OriginalValue - formula.Entity3Operand.Value : null;

                        incentiveEvent.Entity4CalculatedValue = formula.Entity4Operand.HasValue ? incentiveEvent.OriginalValue - formula.Entity4Operand.Value : null;

                        return;
                    }
                case FormulaType.MULTIPLY:
                    {
                        incentiveEvent.CalculatedValue = incentiveEvent.OriginalValue * formula.FirstOperand.Value;

                        incentiveEvent.Entity1CalculatedValue = formula.Entity1Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity1Operand.Value : null;

                        incentiveEvent.Entity2CalculatedValue = formula.Entity2Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity2Operand.Value : null;

                        incentiveEvent.Entity3CalculatedValue = formula.Entity3Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity3Operand.Value : null;

                        incentiveEvent.Entity4CalculatedValue = formula.Entity4Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity4Operand.Value : null;
                        return;
                    }
                case FormulaType.PERCENTAGE:
                    {
                        incentiveEvent.CalculatedValue = incentiveEvent.OriginalValue * formula.FirstOperand.Value / 100.0;

                        incentiveEvent.Entity1CalculatedValue = formula.Entity1Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity1Operand.Value / 100.0 : null;

                        incentiveEvent.Entity2CalculatedValue = formula.Entity2Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity2Operand.Value / 100.0 : null;

                        incentiveEvent.Entity3CalculatedValue = formula.Entity3Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity3Operand.Value / 100.0 : null;

                        incentiveEvent.Entity4CalculatedValue = formula.Entity4Operand.HasValue ? incentiveEvent.OriginalValue * formula.Entity4Operand.Value / 100.0 : null;

                        return;
                    }
            };

            incentiveEvent.CalculatedValue = 0;
        }

        /// <summary>
        /// Creates basic incentive event 
        /// </summary>
        /// <returns></returns>
        public static IncentiveEvent CreateIncentiveEvent(DateTime curDate, EventCategoryType eventCategoryType = EventCategoryType.INCOME)
        {
            IncentiveEvent incentiveEvent = new IncentiveEvent();
            incentiveEvent.EventId = Guid.NewGuid();
            incentiveEvent.EventOccuranceTime = curDate;
            incentiveEvent.CreatedDate = curDate;
            incentiveEvent.EventCategoryType = eventCategoryType;
            return incentiveEvent;
        }
    }
}
