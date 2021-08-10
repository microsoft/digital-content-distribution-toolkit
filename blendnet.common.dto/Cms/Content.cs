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
        public string Genre { get; set; }

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
        public Guid? ContentBroadcastedBy { get; set; }

        public void SetIdentifiers()
        {
            this.Id = Guid.NewGuid();
        }

    }

    public class People
    {
        public string Name { get; set; }
        
        public Role Role { get; set; }
    }

    public class Attachment
    {
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
        LargeThumbnail = 2
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
        MusicDirector = 3
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
