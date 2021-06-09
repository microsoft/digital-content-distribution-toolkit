using System;
using System.Collections.Generic;

namespace blendnet.common.dto.Incentive
{

    public class PlanDetail
    {
        /// <summary>
        /// Unique id associated with detail
        /// </summary>
        public Guid DetailId { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Title of event
        /// </summary>
        public string EventTitle { get; set; }
        
        /// <summary>
        /// Rule type indicating whether it is a sum or count event
        /// </summary>
        public RuleType RuleType { get; set; }

        /// <summary>
        /// Formula associated with the event
        /// </summary>
        public Formula Formula { get; set; }

    }

    public class Formula
    {
        /// <summary>
        /// Formula type
        /// </summary>
        public FormulaType FormulaType { get; set; }

        /// <summary>
        /// Left operand to use during calculation of summary
        /// </summary>
        public double? LeftOperand { get; set; }

        /// <summary>
        /// Right operand to use during calculation of summary
        /// </summary>
        public double RightOperand { get; set; }

        /// <summary>
        /// List of ranges to decide the value
        /// </summary>
        public List<RangeValue> RangeOperand { get; set; }


    }


    public class RangeValue
    {
        /// <summary>
        /// Start number of range
        /// </summary>
        public int StartRange { get; set; }

        /// <summary>
        /// End number of the range
        /// </summary>
        public int EndRange { get; set; }

        /// <summary>
        /// Value associated with the range
        /// </summary>
        public int Output { get; set; }
    }

    public enum FormulaType
    {
        PLUS = 0,
        MINUS = 1,
        MULTIPLY = 2,
        PERCENTAGE = 3,
        DIVIDE_AND_MULTIPLY = 4,
        RANGE_AND_MULTIPLY = 5
    }

    public enum RuleType
    {
        SUM,
        COUNT
    }
}
