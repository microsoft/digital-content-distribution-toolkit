using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.incentive.api.Model
{
    public class IncentivePlanRequest
    {
        /// <summary>
        /// Name given to plan
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string PlanName { get; set; }

        /// <summary>
        /// Type of the plan
        /// </summary>
        [Required]
        public PlanType PlanType { get; set; }

        /// <summary>
        /// Start date of the plan
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the plan
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Target audience of the plan
        /// </summary>
        [Required]
        public Audience Audience { get; set; }

        /// <summary>
        /// List of events and corresponding formula associated with plan
        /// </summary>
        [Required]
        public List<PlanDetail> PlanDetails { get; set; }
    }
}
