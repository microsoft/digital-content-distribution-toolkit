using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace blendnet.common.dto.Incentive
{
    public class Event : BaseDto
    {
        /// <summary>
        /// Unique event id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? EventId { get; set; }

        /// <summary>
        /// User id of consumer or Partner id of retailer
        /// </summary>
        public string EventGeneratorId { get; set; }

        /// <summary>
        /// Event type of the event
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// List of additional parameters that can be sent
        /// </summary>
        public List<Property> Properties { get; set; }

        /// <summary>
        /// UTC Date time value of when the event is created
        /// </summary>
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Date time represented in int format
        /// </summary>
        public int EventDate { get; set; }

        /// <summary>
        /// Value of the event which is later used for computation
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Category of the event indicating whether it is an income event or expense event
        /// </summary>
        public EventCategoryType EventCategoryType { get; set; }

        /// <summary>
        /// Target audience of event. Useful when getting overall summary based on audience
        /// </summary>
        public Audience Audience { get; set; }

    }

    public class Property
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public enum EventCategoryType
    {
        INCOME,
        EXPENSE
    }
}
