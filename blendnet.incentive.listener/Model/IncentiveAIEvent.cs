﻿using blendnet.common.dto;
using blendnet.common.dto.AIEvents;
using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.Model
{
    /// <summary>
    /// Incentive AI event
    /// </summary>
    public class IncentiveAIEvent: BaseAIEvent
    {
        /// <summary>
        /// Construct the AIEvent based on the incentive event.
        /// </summary>
        /// <param name="incentiveEvent"></param>
        public IncentiveAIEvent(IncentiveEvent incentiveEvent)
        {
            EventId = incentiveEvent.EventId;

            EventCreatedFor = incentiveEvent.EventCreatedFor;

            EventType = incentiveEvent.EventType;

            EventSubType = incentiveEvent.EventSubType;

            EventOccuranceTime = incentiveEvent.EventOccuranceTime;

            CalculatedValue = incentiveEvent.CalculatedValue;

            OriginalValue = incentiveEvent.OriginalValue;

            EventCategoryType = incentiveEvent.EventCategoryType;

            if (incentiveEvent.Audience != null)
            {
                AudienceType = incentiveEvent.Audience.AudienceType;

                AudienceSubTypeName = incentiveEvent.Audience.SubTypeName;
            }

            if (incentiveEvent.Properties != null && incentiveEvent.Properties.Count > 0)
            {
                //converting to dictionary. AI extension takes care of Dictionary<string,string>
                Properties = incentiveEvent.Properties.ToDictionary(p => p.Name, p => p.Value);
            }
        }

        /// <summary>
        /// Event Id
        /// </summary>
        public Guid? EventId { get; set; }

        /// <summary>
        /// User phone number of consumer or Partner id of retailer
        /// </summary>
        public string EventCreatedFor { get; set; }

        /// <summary>
        /// Event type of the event
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Content provider id in case of order completed event
        /// </summary>
        public string EventSubType { get; set; }
        
        
        /// <summary>
        /// List of additional parameters that can be sent
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Time during which event was created. Might differ than CreatedDate when event is 
        /// called from client and there are network delays
        /// </summary>
        public DateTime EventOccuranceTime { get; set; }

        /// <summary>
        /// Computed Value of the event which is later used for computation
        /// </summary>
        public double CalculatedValue { get; set; }

        /// <summary>
        /// Original Value of the event which is later used for reference
        /// </summary>
        public double OriginalValue { get; set; }

        /// <summary>
        /// Category of the event indicating whether it is an income event or expense event
        /// </summary>
        public EventCategoryType EventCategoryType { get; set; }

        /// <summary>
        /// AudienceType
        /// </summary>
        public AudienceType AudienceType { get; set; }

        /// <summary>
        /// "Consumer" for Consumer, Selected Retailer partner code in case of Retailer
        /// </summary>
        public string AudienceSubTypeName { get; set; }

    }
}