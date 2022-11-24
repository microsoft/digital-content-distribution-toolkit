// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Cms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility.BroadCastMigration
{
    /// <summary>
    /// State of Content before the change
    /// </summary>
    public class OldContent : BaseDto
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
        public string Title { get; set; }

        /// <summary>
        /// Short Description of the media content
        /// </summary>
        [Required]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Long Description of the media content
        /// </summary>
        [Required]
        public string LongDescription { get; set; }

        /// <summary>
        /// Additional Description of the media content
        /// </summary>
        public string AdditionalDescription1 { get; set; }

        /// <summary>
        /// AdditionalDescription2
        /// </summary>
        public string AdditionalDescription2 { get; set; }

        /// <summary>
        /// Genre
        /// </summary>
        [Required]
        public string Genre { get; set; }

        /// <summary>
        /// Age Appropriateness
        /// </summary>
        [Required]
        public string AgeAppropriateness { get; set; }

        /// <summary>
        /// ContentAdvisory
        /// </summary>
        [Required]
        public string ContentAdvisory { get; set; }

        /// <summary>
        /// Year of Release
        /// </summary>
        public string YearOfRelease { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Content Duration
        /// </summary>
        public float DurationInMts { get; set; }

        /// <summary>
        /// Content Rating
        /// </summary>
        public string Rating { get; set; }

        /// <summary>
        /// Media File Name
        /// </summary>
        [Required]
        public string MediaFileName { get; set; }

        /// <summary>
        /// To Resolve Episode
        /// </summary
        public string Hierarchy { get; set; }

        /// <summary>
        /// Dash Url
        /// </summary>
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
        public List<People> People { get; set; }

        /// <summary>
        /// List of Attachments
        /// </summary>
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
        public ContentTransformStatus? ContentTransformStatus { get; set; } = blendnet.common.dto.Cms.ContentTransformStatus.TransformNotInitialized;

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid? ContentTransformStatusUpdatedBy { get; set; }

        /// <summary>
        /// Content Broadcast Status
        /// </summary>
        public ContentBroadcastStatus? ContentBroadcastStatus { get; set; } = blendnet.common.dto.Cms.ContentBroadcastStatus.BroadcastNotInitialized;

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid? ContentBroadcastStatusUpdatedBy { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid? ContentBroadcastedBy { get; set; }

        public void SetIdentifiers()
        {
            this.Id = Guid.NewGuid();
        }

    }
}
