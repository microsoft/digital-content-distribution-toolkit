// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for RepresentationBaseType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="RepresentationBaseType">
 ///  <complexContent>
 ///    <restriction base="{http:///www.w3.org/2001/XMLSchema}anyType">
 ///      <sequence>
 ///        <element name="FramePacking" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="AudioChannelConfiguration" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="ContentProtection" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="EssentialProperty" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="SupplementalProperty" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="InbandEventStream" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute name="profiles" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="width" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="height" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="sar" type="{urn:mpeg:dash:schema:mpd:2011}RatioType" />
 ///      <attribute name="frameRate" type="{urn:mpeg:dash:schema:mpd:2011}FrameRateType" />
 ///      <attribute name="audioSamplingRate" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="mimeType" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="segmentProfiles" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="codecs" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="maximumSAPPeriod" type="{http:///www.w3.org/2001/XMLSchema}double" />
 ///      <attribute name="startWithSAP" type="{urn:mpeg:dash:schema:mpd:2011}SAPType" />
 ///      <attribute name="maxPlayoutRate" type="{http:///www.w3.org/2001/XMLSchema}double" />
 ///      <attribute name="codingDependency" type="{http:///www.w3.org/2001/XMLSchema}boolean" />
 ///      <attribute name="scanType" type="{urn:mpeg:dash:schema:mpd:2011}VideoScanType" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </restriction>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "RepresentationBaseType")]
    public class RepresentationBaseType
    {
        protected List<DescriptorType> framePackings;
        protected List<DescriptorType> audioChannelConfigurations;
        protected List<DescriptorType> contentProtections;
        protected List<DescriptorType> essentialProperties;
        protected List<DescriptorType> supplementalProperties;
        protected List<DescriptorType> inbandEventStreams;
        protected List<System.Xml.XmlElement> anies;
        protected String profiles;
        protected uint width;
        protected uint height;
        protected String sar;
        protected String frameRate;
        protected String audioSamplingRate;
        protected String mimeType;
        protected String segmentProfiles;
        protected String codecs;
        protected Double maximumSAPPeriod;
        protected Int64 startWithSAP;
        protected Double maxPlayoutRate;
        protected Boolean codingDependency;
        protected VideoScanType scanType;
        protected IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();


        public RepresentationBaseType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "FramePacking")]
        public List<DescriptorType> FramePackings
        {
            get
            {
                if (this.framePackings == null)
                {
                    this.framePackings = new List<DescriptorType>();
                }
                return framePackings;
            }
            set { framePackings = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "AudioChannelConfiguration")]
        public List<DescriptorType> AudioChannelConfigurations
        {
            get
            {
                if (this.audioChannelConfigurations == null)
                {
                    this.audioChannelConfigurations = new List<DescriptorType>();
                }
                return audioChannelConfigurations;
            }
            set { audioChannelConfigurations = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "ContentProtection")]
        public List<DescriptorType> ContentProtections
        {
            get
            {
                if (this.contentProtections == null)
                {
                    this.contentProtections = new List<DescriptorType>();
                }
                return contentProtections;
            }
            set { contentProtections = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "EssentialProperty")]
        public List<DescriptorType> EssentialProperties
        {
            get
            {

                if (this.essentialProperties == null)
                {
                    this.essentialProperties = new List<DescriptorType>();
                }
                return essentialProperties;
            }
            set { essentialProperties = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "SupplementalProperty")]
        public List<DescriptorType> SupplementalProperties
        {
            get
            {
                if (supplementalProperties == null)
                {
                    this.supplementalProperties = new List<DescriptorType>();
                }

                return supplementalProperties;
            }
            set { supplementalProperties = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "InbandEventStream")]
        public List<DescriptorType> InbandEventStreams
        {
            get
            {
                if (this.inbandEventStreams == null)
                {
                    this.inbandEventStreams = new List<DescriptorType>();
                }
                return inbandEventStreams;
            }
            set { inbandEventStreams = value; }
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

                return anies;
            }
            set { anies = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "profile")]
        public String Profiles
        {
            get { return profiles; }
            set { profiles = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "width", DataType = "unsignedInt")]
        public uint Width
        {
            get { return width; }
            set { width = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "height", DataType = "unsignedInt")]
        public uint Height
        {
            get { return height; }
            set { height = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "sar")]
        public String Sar
        {
            get { return sar; }
            set { sar = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "frameRate")]
        public String FrameRate
        {
            get { return frameRate; }
            set { frameRate = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "audioSamplingRate")]
        public String AudioSamplingRate
        {
            get { return audioSamplingRate; }
            set { audioSamplingRate = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "mimeType")]
        public String MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "segmentProfiles")]
        public String SegmentProfiles
        {
            get { return segmentProfiles; }
            set { segmentProfiles = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "codecs")]
        public String Codecs
        {
            get { return codecs; }
            set { codecs = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "maximumSAPPeriod")]
        public Double MaximumSAPPeriod
        {
            get { return maximumSAPPeriod; }
            set { maximumSAPPeriod = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "startWithSAP")]
        public Int64 StartWithSAP
        {
            get { return startWithSAP; }
            set { startWithSAP = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "maxPlayoutRate")]
        public Double MaxPlayoutRate
        {
            get { return maxPlayoutRate; }
            set { maxPlayoutRate = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "codingDependency")]
        public Boolean CodingDependency
        {
            get { return codingDependency; }
            set { codingDependency = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "scanType")]
        public VideoScanType ScanType
        {
            get { return scanType; }
            set { scanType = value; }
        }

        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }

    }
}
