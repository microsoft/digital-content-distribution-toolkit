// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Represent a Media Content and Attachment Files
    /// </summary>
    public class Content:BaseDto
    {
        /// <summary>
        /// Unique Content Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Same as Id
        /// </summary>
        public Guid? ContentId { get; set; }

        /// <summary>
        /// Content provider content id
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string ContentProviderContentId { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public ContentContainerType Type { get; set; }

        /// <summary>
        /// Content Provider Id
        /// </summary>
        [Required]
        public Guid ContentProviderId { get; set; }

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
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string YearOfRelease { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        [Required]
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
        /// Media File Name
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Description_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string MediaFileName { get; set; }

        /// <summary>
        /// To Resolve Episode
        /// </summary
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Hierarchy { get; set; }

        /// <summary>
        /// Dash Url
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Url_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.URL, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.URL_ErrorCode)]
        public string DashUrl { get; set; }

        /// <summary>
        /// Audio Tar File Size
        /// </summary>
        public long? AudioTarFileSize { get; set; }

        /// <summary>
        /// Video Tar File Size
        /// </summary>
        public long? VideoTarFileSize { get; set; }

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
        /// Is Active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Artist list
        /// </summary>
        [ValidateCollection]
        public List<People> People { get; set; }

        /// <summary>
        /// List of Attachments
        /// </summary>
        [Required]
        [ValidateCollection]
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Content Upload Status
        /// </summary>
        public ContentUploadStatus? ContentUploadStatus { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid? ContentUploadStatusUpdatedBy { get; set; }

        /// <summary>
        /// Content Transform Status
        /// </summary>
        public ContentTransformStatus? ContentTransformStatus { get; set; } = Cms.ContentTransformStatus.TransformNotInitialized;

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid? ContentTransformStatusUpdatedBy { get; set; }

        /// <summary>
        /// Content Broadcast Status
        /// </summary>
        public ContentBroadcastStatus? ContentBroadcastStatus { get; set; } = Cms.ContentBroadcastStatus.BroadcastNotInitialized;

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid? ContentBroadcastStatusUpdatedBy { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        public ContentBroadcastedBy ContentBroadcastedBy { get; set; }

        /// <summary>
        /// Returns if broadcast is active
        /// </summary>
        public bool IsBroadCastActive
        {
            get
            {
                DateTime currentDateTime = DateTime.UtcNow;

                if (ContentBroadcastStatus == Cms.ContentBroadcastStatus.BroadcastOrderComplete &&
                   (currentDateTime >= ContentBroadcastedBy.BroadcastRequest.StartDate && 
                    currentDateTime <= ContentBroadcastedBy.BroadcastRequest.EndDate))
                {
                    return true;
                }

                return false;
            }
        }

        public void SetDefaults()
        {
            this.Id = Guid.NewGuid();

            this.ContentBroadcastedBy = null;
            
            this.ContentBroadcastStatusUpdatedBy = null;

            this.ContentTransformStatusUpdatedBy = null;

            this.ContentUploadStatusUpdatedBy = null;

            this.ModifiedByByUserId = null;

            this.ModifiedDate = null;
        }

    }

    public class People
    {
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        [Required]
        public string Name { get; set; }
        
        public Role Role { get; set; }
    }

    public class Attachment
    {
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        [Required]
        public string Name { get; set; }

        public AttachmentType Type { get; set; }

    }

    /// <summary>
    /// Represents the Type of Attachment
    /// </summary>
    public enum AttachmentType
    {
        Thumbnail = 0,
        Teaser = 1,
        LargeThumbnail = 2,
        LandscapeThumbnail = 3
    }

    /// <summary>
    /// Genre
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Genre
    {
        Drama = 0,
        Family = 1,
        Reality = 2,
        Crime = 3,
        Romance = 4,
        Action = 5,
        Thriller = 6,
        Fantasy = 7,
        Mythology = 8,
        Comedy=9
    }

    /// <summary>
    /// Role of artists present
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Role
    {
        Director = 0,
        Actor = 1,
        Singer = 2,
        MusicDirector = 3,
        Other = 4
    }

    /// <summary>
    /// Command Upload Status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentUploadStatus
    {
        UploadSubmitted = 0,
        UploadInProgress = 1,
        UploadComplete = 2,
        UploadFailed = 3
    }

    /// <summary>
    /// Content Transform Status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentTransformStatus
    {
        TransformNotInitialized = 0,
        TransformSubmitted = 1,
        TransformInProgress = 2,
        TransformAMSJobInProgress = 3,
        TransformDownloadInProgress = 4,
        TransformComplete = 5,
        TransformFailed = 6,
        TransformCancelled = 7
    }

    /// <summary>
    /// Content Broadcast Status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentBroadcastStatus
    {
        BroadcastNotInitialized = 0,
        BroadcastSubmitted = 1,
        BroadcastInProgress = 2,
        BroadcastTarPushed = 3,
        BroadcastOrderCreated = 4,
        BroadcastOrderActive = 5,
        BroadcastOrderComplete = 6,
        BroadcastOrderRejected = 7,
        BroadcastOrderFailed = 8,
        BroadcastOrderCancelled = 9,
        BroadcastFailed = 10,
        BroadcastCancelSubmitted = 11,
        BroadcastCancelInProgress = 12,
        BroadcastCancelComplete = 13,
        BroadcastCancelFailed = 14
    }

}
