// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    public class PeriodType
    {

        private List<BaseURLType> baseURLs;
        private SegmentBaseType segmentBase;
        private SegmentListType segmentList;
        private SegmentTemplateType segmentTemplate;
        private List<EventStreamType> eventStreams;
        private List<AdaptationSetType> adaptationSets;
        private List<SubsetType> subsets;
        private List<System.Xml.XmlElement> anies;
        private String href;
        private ActuateType actuate;
        private String id;
        private string start;
        private string duration;
        private Boolean bitstreamSwitching;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();


        public PeriodType() { }


         [System.Xml.Serialization.XmlElement(ElementName="BaseURL")]
         public List<BaseURLType> BaseURLs{
              get{
                if (baseURLs == null)
                {
                    baseURLs = new List<BaseURLType>();
                }
                return this.baseURLs;
            }
            set{
                this.baseURLs = value;
            }
         }
    [System.Xml.Serialization.XmlElement(ElementName="SegmentBase")]
    public SegmentBaseType SegmentBase{
        get{
            return this.segmentBase;
        }

        set{
            this.segmentBase = value;
        }
    }

    [System.Xml.Serialization.XmlElement(ElementName="SegmentList")]
    public SegmentListType SegmentList{
        get{
            return this.segmentList;
        }
        set{
            this.segmentList = value;
        }
    }


     [System.Xml.Serialization.XmlElement(ElementName="SegmentTemplate")]
    public SegmentTemplateType SegmentTemplate{
        get
        {
            return this.segmentTemplate;
        }
        set
        {
            this.segmentTemplate = value;
        }
     }

     [System.Xml.Serialization.XmlElement(ElementName = "EventStream")]
     public List<EventStreamType> EventStreams
     {
         get
         {
             return this.eventStreams;
         }

         set
         {
             this.eventStreams = value;
         }
     }

     [System.Xml.Serialization.XmlElement(ElementName = "AdaptationSet")]
     public List<AdaptationSetType> AdaptationSets
     {
         get
         {
             if (this.adaptationSets == null)
             {
                 this.adaptationSets = new List<AdaptationSetType>();
             }
             return this.adaptationSets;
         }

         set
         {
             this.adaptationSets = value;
         }
     }

     [System.Xml.Serialization.XmlElement(ElementName = "Subset")]
     public List<SubsetType> Subsets
     {
         get
         {
             if (this.subsets == null)
             {
                 this.subsets = new List<SubsetType>();
             }
             return this.subsets;
         }

         set
         {
             this.subsets = value;
         }
     }


     [System.Xml.Serialization.XmlElement()]
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

     [System.Xml.Serialization.XmlAttribute(Namespace = "http://www.w3.org/1999/xlink")]
     public String Href
     {
         get
         {
             return this.href;
         }
         set
         {
             this.href = value;
         }
     }


     [System.Xml.Serialization.XmlAttribute(AttributeName = "actuate", Namespace = "http://www.w3.org/1999/xlink")]
     public string ActuateAsString
     {
         get
         {
             return this.actuate.ToString();
         }
         set
         {
             if (string.IsNullOrEmpty(value))
             {
                 this.Actuate = default(ActuateType);
             }
             else
             {
                 this.actuate = (ActuateType)Enum.Parse(typeof(ActuateType), value);
             }
         }
     }

     [System.Xml.Serialization.XmlIgnore()]
     public ActuateType Actuate
     {
         get;
         set;
     }

     [System.Xml.Serialization.XmlAttribute()]
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

     [System.Xml.Serialization.XmlAttribute()]
     public string Start
     {
         get
         {
             return this.start;
         }

         set
         {
             this.start = value;
         }
     }

     [System.Xml.Serialization.XmlAttribute()]
     public string Duration
     {
         get
         {
             return this.duration;
         }

         set
         {
             this.duration = value;
         }
     }


     [System.Xml.Serialization.XmlAttribute()]
     public Boolean BitstreamSwitching
     {
         get
         {
             return this.bitstreamSwitching;
         }

         set
         {
             this.bitstreamSwitching = value;
         }
     }

     [System.Xml.Serialization.XmlAttribute()]
     private IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
     {
         get
         {
             return this.otherAttributes;
         }
     }
    }
}
