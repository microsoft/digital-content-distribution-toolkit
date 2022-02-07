using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace blendnet.common.dto.Incentive
{

    public class PlanDetail:ICloneable
    {
        /// <summary>
        /// Unique id associated with detail
        /// </summary>
        public Guid? DetailId { get; set; }

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
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
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

        /// <summary>
        /// Result after application of Rule Type and Formula
        /// </summary>
        public Result Result { get; set; }

        public object Clone()
        {
            PlanDetail planDetail = (PlanDetail)MemberwiseClone();

            planDetail.Formula = Formula != null ? (Formula)Formula.Clone() : null ;

            planDetail.Result = Result != null ? (Result)Result.Clone() : null;

            return planDetail;
        }
    }

    public class Result:ICloneable
    {
        public double Value { get; set; }

        public double ResidualValue { get; set; }

        public RawData RawData { get; set; }

        public object Clone()
        {
            Result result = (Result)MemberwiseClone();

            result.RawData = RawData != null ? (RawData)RawData.Clone() : null;

            return result;
        }
    }

    public class Formula : ICloneable
    {
        /// <summary>
        /// Formula type
        /// </summary>
        [Required]
        public FormulaType FormulaType { get; set; }

        /// <summary>
        /// First Operand to use during calculation of summary
        /// </summary>
        public double? FirstOperand { get; set; }

        /// <summary>
        /// Second Operand to use during calculation of summary
        /// </summary>
        public double? SecondOperand { get; set; }

        /// <summary>
        /// List of ranges to decide the value
        /// </summary>
        public List<RangeValue> RangeOperand { get; set; }

        public object Clone()
        {
            Formula formula =  (Formula) MemberwiseClone();

            if (RangeOperand != null && RangeOperand.Count > 0)
            {
                formula.RangeOperand = RangeOperand.Select(item => (RangeValue)item.Clone()).ToList();
            }

            return formula;
        }
    }


    public class RangeValue : ICloneable
    {
        /// <summary>
        /// Start number of range
        /// </summary>
        public double StartRange { get; set; }

        /// <summary>
        /// End number of the range
        /// </summary>
        public double EndRange { get; set; }

        /// <summary>
        /// Value associated with the range
        /// </summary>
        public double Output { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormulaType
    {
        PLUS = 0,
        MINUS = 1,
        MULTIPLY = 2,
        PERCENTAGE = 3,
        DIVIDE_AND_MULTIPLY = 4,
        RANGE = 5
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
            return HashCode.Combine(item.EventType, item.EventSubType);

        }
    }

    public class RawData: ICloneable
    {
        /// <summary>
        /// Aggregrate Sum of Calculated Value
        /// </summary>
        public double AggregratedCalculatedValue { get; set; }

        /// <summary>
        /// Aggregrated Sum of Original Value
        /// </summary>
        public double AggregratedOriginalValue { get; set; }

        /// <summary>
        /// Aggregrated Count
        /// </summary>
        public double AggregratedCount { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
