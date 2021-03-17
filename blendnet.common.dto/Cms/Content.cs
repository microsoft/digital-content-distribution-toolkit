using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// Represent a Media Content and Attachment Files
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Unique Content Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Same as Id
        /// </summary>
        public Guid ContentId { get; set; }

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
        public int DurationInMts { get; set; }

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
        /// If it will appear on home page
        /// </summary>
        public bool IsHeaderContent { get; set; }

        /// <summary>
        /// If no subscription purchase is required
        /// </summary>
        public bool IsFreeContent { get; set; }

        /// <summary>
        /// Artist list
        /// </summary>
        public List<Artist> Artists { get; set; }

        /// <summary>
        /// List of Attachments
        /// </summary>
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Modified Date
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Content Upload Status
        /// </summary>
        public ContentUploadStatus? ContentUploadStatus { get; set; }

        /// <summary>
        /// Content Transform Status
        /// </summary>
        public ContentUploadStatus? ContentTransformStatus { get; set; }

        /// <summary>
        /// Content Broadcast Status
        /// </summary>
        public ContentUploadStatus? ContentBroadcastStatus { get; set; }

    }

    public class Artist
    {
        public string Name { get; set; }
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
    }

    /// <summary>
    /// Command Upload Status
    /// </summary>
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
    public enum ContentTransformStatus
    {
        
    }

    /// <summary>
    /// Content Broadcast Status
    /// </summary>
    public enum ContentBroadcastStatus
    {
        
    }

}
