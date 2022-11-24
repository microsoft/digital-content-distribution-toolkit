// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace blendnet.common.dto.Incentive
{
    /// <summary>
    /// Plan which indicates how the incentive calculation needs to be done
    /// </summary>
    public class IncentivePlan : BaseDto
    {

        /// <summary>
        /// Unique id of the plan
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Name given to plan
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// Type of the plan
        /// </summary>
        public PlanType PlanType { get; set; }

        /// <summary>
        /// Start date of the plan
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the plan
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Target audience of the plan
        /// </summary>
        public Audience Audience { get; set; }

        /// <summary>
        /// List of events and corresponding formula associated with plan
        /// </summary>
        public List<PlanDetail> PlanDetails { get; set; }

        /// <summary>
        /// Sets the publish mode of the plan
        /// </summary>
        public PublishMode PublishMode { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlanType
    {
        REGULAR,
        MILESTONE
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PublishMode
    {
        DRAFT,
        PUBLISHED
    }
}
