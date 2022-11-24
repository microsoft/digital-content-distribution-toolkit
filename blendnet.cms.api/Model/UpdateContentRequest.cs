// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Validation;
using System.ComponentModel.DataAnnotations;

namespace blendnet.cms.api.Model
{
    /// <summary>
    /// Update Content Request.
    /// Only allow to update meta data properties.
    /// No edit allowed to update thumbnails, media file or system properties like media file size etc
    /// </summary>
    public class UpdateContentRequest
    {
        /// <summary>
        /// Title
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Title { get; set; }

        /// <summary>
        /// Short Description of the media content
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Description_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Long Description of the media content
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.LongDescription_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.LongDescription_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string LongDescription { get; set; }

        /// <summary>
        /// Additional Description of the media content
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.LongDescription_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string AdditionalDescription1 { get; set; }

        /// <summary>
        /// AdditionalDescription2
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.LongDescription_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string AdditionalDescription2 { get; set; }

        /// <summary>
        /// AdditionalTitle1
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string AdditionalTitle1 { get; set; }

        /// <summary>
        /// AdditionalTitle2
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string AdditionalTitle2 { get; set; }


        /// <summary>
        /// Genre
        /// </summary>
        [Required]
        public Genre Genre { get; set; }

        /// <summary>
        /// Age Appropriateness
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Description_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string AgeAppropriateness { get; set; }

        /// <summary>
        /// ContentAdvisory
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Description_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string ContentAdvisory { get; set; }

        /// <summary>
        /// Year of Release
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string YearOfRelease { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Language { get; set; }

        /// <summary>
        /// Content Duration
        /// </summary>
        public float DurationInMts { get; set; }

        /// <summary>
        /// Content Rating
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Rating { get; set; }

        /// <summary>
        /// To Resolve Episode
        /// </summary
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Hierarchy { get; set; }

        /// <summary>
        /// If it will appear on home page
        /// </summary>
        [Required]
        public bool IsHeaderContent { get; set; }

        /// <summary>
        /// If no subscription purchase is required
        /// </summary>
        [Required]
        public bool IsFreeContent { get; set; }

        /// <summary>
        /// Is Exclusive
        /// </summary>
        [Required]
        public bool IsExclusiveContent { get; set; }


        /// <summary>
        /// Artist list
        /// </summary>
        [ValidateCollection]
        public List<People> People { get; set; }

    }
}
