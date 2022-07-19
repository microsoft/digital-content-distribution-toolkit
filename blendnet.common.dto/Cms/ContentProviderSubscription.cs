using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto
{
    public class ContentProviderSubscriptionDto : BaseDto
    {
        /// <summary>
        /// Subscription Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Content Provider ID
        /// </summary>
        public Guid ContentProviderId { get; set; }



        public ContentProviderContainerType Type { get; set; } = ContentProviderContainerType.Subscription;

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Title { get; set; }

        public int DurationDays { get; set; }

        public float Price { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsRedeemable { get; set; }

        public int RedemptionValue { get; set; }

        public void SetIdentifiers()
        {
            this.Id = Guid.NewGuid();
        }

        public SubscriptionPublishMode PublishMode { get; set; } = SubscriptionPublishMode.PUBLISHED;

        public SubscriptionType SubscriptionType { get; set; } = SubscriptionType.SVOD;

        /// <summary>
        /// List of contentIds for T-VOD subscription type
        /// </summary>
        public List<Guid> ContentIds { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionPublishMode
    {
        DRAFT,
        PUBLISHED
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionType
    {
        TVOD,
        SVOD
    }
}
