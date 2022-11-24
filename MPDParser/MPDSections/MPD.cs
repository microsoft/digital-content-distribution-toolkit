// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


 //<para>
 //
 //<para>The following schema fragment specifies the expected content contained within this class.
 //
 //<example>
 //
 //</example>
 

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    /// <summary>
    ///     c# class for MPDtype complex type.
    ///     <para>
    ///         <complexType name="MPDtype">
    ///             <complexContent>
    ///                 <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
    ///                 <sequence>
    ///                     <element name="ProgramInformation" type="{urn:mpeg:dash:schema:mpd:2011}ProgramInformationType" maxOccurs="unbounded" minOccurs="0"/>
    ///                     <element name="BaseURL" type="{urn:mpeg:dash:schema:mpd:2011}BaseURLType" maxOccurs="unbounded" minOccurs="0"/>
    ///                     <element name="Location" type="{http://www.w3.org/2001/XMLSchema}anyURI" maxOccurs="unbounded" minOccurs="0"/>
    ///                     <element name="Period" type="{urn:mpeg:dash:schema:mpd:2011}PeriodType" maxOccurs="unbounded"/>
    ///                     <element name="Metrics" type="{urn:mpeg:dash:schema:mpd:2011}MetricsType" maxOccurs="unbounded" minOccurs="0"/>
    ///                     <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
    ///                 </sequence>
    ///                 <attribute name="id" type="{http://www.w3.org/2001/XMLSchema}string" />
    ///                 <attribute name="profiles" use="required" type="{http://www.w3.org/2001/XMLSchema}string" />
    ///                 <attribute name="type" type="{urn:mpeg:dash:schema:mpd:2011}PresentationType" default="static" />
    ///                 <attribute name="availabilityStartTime" type="{http://www.w3.org/2001/XMLSchema}dateTime" />
    ///                 <attribute name="availabilityEndTime" type="{http://www.w3.org/2001/XMLSchema}dateTime" />
    ///                 <attribute name="publishTime" type="{http://www.w3.org/2001/XMLSchema}dateTime" />
    ///                 <attribute name="mediaPresentationDuration" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <attribute name="minimumUpdatePeriod" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <attribute name="minBufferTime" use="required" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <attribute name="timeShiftBufferDepth" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <attribute name="suggestedPresentationDelay" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <attribute name="maxSegmentDuration" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <attribute name="maxSubsegmentDuration" type="{http://www.w3.org/2001/XMLSchema}duration" />
    ///                 <anyAttribute processContents='lax' namespace='##other'/>
    ///                 </restriction>
    ///            </complexContent>
    ///         </complexType>
    ///     </para>
    /// </summary>
    /// 
    /// <see cref="https://microsoft.sharepoint.com/teams/mediaservices/_layouts/15/start.aspx#/Shared%20Documents/Forms/AllItems.aspx?RootFolder=%2fteams%2fmediaservices%2fShared%20Documents%2fSpecs%2fStreaming%20Protocols%2fDASH&FolderCTID=0x012000CEB864DFB278E849912FCEE2F7C76E63"/>
    /// <seealso cref=""/> TODO: Add DASH URL

    
    [System.Xml.Serialization.XmlRoot(Namespace = "urn:mpeg:dash:schema:mpd:2011")]
    public class MPD
    {
        [System.Xml.Serialization.XmlElement(ElementName = "ProgramInformation")]
        public List<ProgramInformationType> programInformations;
        private List<BaseURLType> baseURLs;
        private List<String> locations;
        private List<PeriodType> periods;
        private List<MetricsType> metrics;
        private List<XmlElement> anies;
        private String id;
        private String profiles;
        private PresentationType type;
        private DateTime availabilityStartTime;
        private DateTime availabilityEndTime;
        private DateTime publishTime;
        private string mediaPresentationDuration;
        private string minimumUpdatePeriod;
        private string minBufferTime;
        private string timeShiftBufferDepth;
        private string suggestedPresentationDelay;
        private string maxSegmentDuration;
        private string maxSubsegmentDuration;
        private IDictionary<XmlQualifiedName, String> otherAttributes = new Dictionary<XmlQualifiedName, String>();


        public MPD() { }

        public List<ProgramInformationType> ProgramInformations
        {
            get
            {
                if (programInformations == null)
                {
                    programInformations = new List<ProgramInformationType>();
                }
                return this.programInformations;
            }
            set
            {
                this.programInformations = value;
            }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "BaseURL")]
        public List<BaseURLType> BaseURLs
        {
            get
            {
                if (baseURLs == null)
                {
                    baseURLs = new List<BaseURLType>();
                }
                return this.baseURLs;
            }
            set
            {
                this.baseURLs = value;
            }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "Location", DataType = "anyURI")]
        public List<String> Locations
        {
            get
            {
                if (locations == null)
                {
                    locations = new List<String>();
                }
                return this.locations;
            }


        }

        [System.Xml.Serialization.XmlElement(ElementName = "Period")]
        public List<PeriodType> Periods
        {
            get
            {
                if (this.periods == null)
                {
                    this.periods = new List<PeriodType>();
                }
                return this.periods;
            }
            set
            {
                this.periods = value;
            }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "Metrics")]
        public List<MetricsType> Metrics
        {
            get
            {
                if (this.metrics == null)
                {
                    this.metrics = new List<MetricsType>();
                }
                return this.metrics;
            }

            set
            {
                this.metrics = value;
            }
        }

        [System.Xml.Serialization.XmlAnyElement()]
        public List<System.Xml.XmlElement> Anies
        {
            get
            {
                if (this.anies == null)
                {
                    this.anies = new List<System.Xml.XmlElement>();
                }
                return this.anies;
            }
            set
            {
                this.anies = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "id")]
        public String Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "profiles")]
        public String Profiles
        {
            get
            {
                return this.profiles;
            }
            set
            {
                this.profiles = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "type")]
        public PresentationType Type
        {
            get {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
        public string TypeAsString
        {
            get
            {
                return Type.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.Type = default(PresentationType);
                }
                else
                {
                    this.Type = (PresentationType)Enum.Parse(typeof(PresentationType), value);
                }
            }

        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "availabilityStartTime", DataType = "dateTime")]
        public DateTime AvailabilityStartTime
        {
            get
            {
                return this.availabilityStartTime;
            }

            set
            {
                this.availabilityStartTime = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "availabilityEndTime", DataType = "dateTime")]
        public DateTime AvailabilityEndTime
        {
            get
            {
                return this.availabilityEndTime;
            }
            set
            {
                this.availabilityEndTime = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "publishTime", DataType = "dateTime")]
        public DateTime PublishTime
        {
            get
            {
                return this.publishTime;
            }

            set
            {
                this.publishTime = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "mediaPresentationDuration")]
        public string MediaPresentationDuration
        {
            get
            {
                return this.mediaPresentationDuration;
            }
            set
            {
                this.mediaPresentationDuration = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "minimumUpdatePeriod")]
        public string MinimumUpdatePeriod
        {
            get
            {
                return this.minimumUpdatePeriod;
            }

            set
            {
                this.minimumUpdatePeriod = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "minBufferTime")]
        public string MinBufferTime
        {
            get
            {
                return this.minBufferTime;
            }

            set
            {
                this.minBufferTime = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "timeShiftBufferDepth")]
        public string TimeShiftBufferDepth
        {
            get
            {
                return timeShiftBufferDepth;
            }

            set
            {
                timeShiftBufferDepth = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "suggestedPresentationDelay")]
        public string SuggestedPresentationDelay
        {
            get
            {
                return this.suggestedPresentationDelay;
            }

            set
            {
                this.suggestedPresentationDelay = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "maxSegmentDuration")]
        public string MaxSegmentDuration
        {
            get
            {
                return this.maxSegmentDuration;
            }

            set
            {
                this.maxSegmentDuration = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "maxSubsegmentDuration")]
        public string MaxSubsegmentDuration
        {
            get
            {
                return this.maxSubsegmentDuration;
            }

            set
            {
                this.maxSubsegmentDuration = value;
            }
        }

        private IDictionary<XmlQualifiedName, String> OtherAttributes
        {
            get
            {
                return this.otherAttributes;
            }
        }
    }
}
