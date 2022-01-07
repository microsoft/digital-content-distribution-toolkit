using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class ContentProviderDto:BaseDto
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Same as Id
        /// </summary>
        public Guid? ContentProviderId
        {
            get
            {
                return this.Id;
            }
            set
            {
                // no op
            }
        }

        public ContentProviderContainerType Type { get; set; } = ContentProviderContainerType.ContentProvider;

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length,MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Name { get; set; }

        public List<ContentAdministratorDto> ContentAdministrators { get; set; }

        public DateTime? ActivationDate { get; set; } = null;

        public DateTime? DeactivationDate { get; set; } = null;

        public string LogoUrl { get; set; }

        /// <summary>
        /// Resets identifiers
        /// </summary>
        public void SetIdentifiers()
        {
            this.Id = Guid.NewGuid();

            if (this.ContentAdministrators != null && this.ContentAdministrators.Count > 0)
            {
                foreach (ContentAdministratorDto contentAdministrator in this.ContentAdministrators)
                {
                    contentAdministrator.Id = Guid.NewGuid();
                }
            }
        }
    }
}