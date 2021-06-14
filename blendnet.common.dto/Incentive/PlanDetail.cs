using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Incentive
{

    public class PlanDetail
    {
        /// <summary>
        /// Unique id associated with detail
        /// </summary>
        public Guid? DetailId { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        [Required]
        public EventGroupType EventGroupType { get; set; }

        /// <summary>
        /// Sub type of event
        /// </summary>
        [Required]
        public EventType EventType { get; set; }

        /// <summary>
        /// Content provider id in case of order completed event
        /// </summary>
        public string EventSubType { get; set; }

        /// <summary>
        /// Title of event
        /// </summary>
        [Required]
        public string EventTitle { get; set; }

        /// <summary>
        /// Rule type indicating whether it is a sum or count event
        /// </summary>
        [Required]
        public RuleType RuleType { get; set; }

        /// <summary>
        /// Formula associated with the event
        /// </summary>
        [Required]
        public Formula Formula { get; set; }
    }

    public class Formula
    {
        /// <summary>
        /// Formula type
        /// </summary>
        [Required]
        public FormulaType FormulaType { get; set; }

        /// <summary>
        /// Left operand to use during calculation of summary
        /// </summary>
        public double? LeftOperand { get; set; }

        /// <summary>
        /// Right operand to use during calculation of summary
        /// </summary>
        [Required]
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormulaType
    {
        PLUS = 0,
        MINUS = 1,
        MULTIPLY = 2,
        PERCENTAGE = 3,
        DIVIDE_AND_MULTIPLY = 4,
        RANGE_AND_MULTIPLY = 5
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RuleType
    {
        SUM,
        COUNT
    }

    public class PlanDetailComparer : IEqualityComparer<PlanDetail>
    {
        public bool Equals(PlanDetail planDetailLeft, PlanDetail planDetailRight)
        {
            return planDetailLeft.EventType == planDetailRight.EventType && string.Equals(planDetailLeft.EventSubType, planDetailRight.EventSubType);
        }

        public int GetHashCode(PlanDetail item)
        {
            return HashCode.Combine(item.EventGroupType, item.EventType, item.Formula.FormulaType);

        }
    }
}
