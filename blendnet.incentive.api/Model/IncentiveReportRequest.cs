// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.incentive.api.Model
{
    /// <summary>
    /// Incentive Report Request
    /// </summary>
    public class IncentiveReportRequest
    {
        /// <summary>
        /// Partner Code
        /// </summary>
        [Required]
        public string PartnerCode { get; set; }

        /// <summary>
        /// Partner Code
        /// </summary>
        [Required]
        public string[] PartnerIds { get; set; }

        /// <summary>
        /// Date Time
        /// </summary>
        [Required]
        public DateOnly ReportingDate { get; set; }

    }

    public class DateOnly
    {
        [Required]
        [Range(2000, 2099)]
        public int Year { get; set; }

        [Required]
        [Range(1,12)]
        public int Month { get; set; }

        [Required]
        [Range(1, 31)]
        public int Day { get; set; }

    }
}
