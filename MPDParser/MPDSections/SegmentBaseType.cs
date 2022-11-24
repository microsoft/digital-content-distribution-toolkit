// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
     ///<para>C# class for SegmentBaseType complex type.
     ///
     ///<para>The following schema fragment specifies the expected content contained within this class.
     ///
     ///<example>
     ///<complexType name="SegmentBaseType">
     ///  <complexContent>
     ///    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
     ///      <sequence>
     ///        <element name="Initialization" type="{urn:mpeg:dash:schema:mpd:2011}URLType" minOccurs="0"/>
     ///        <element name="RepresentationIndex" type="{urn:mpeg:dash:schema:mpd:2011}URLType" minOccurs="0"/>
     ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
     ///      </sequence>
     ///      <attribute name="timescale" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
     ///      <attribute name="presentationTimeOffset" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" />
     ///      <attribute name="timeShiftBufferDepth" type="{http://www.w3.org/2001/XMLSchema}duration" />
     ///      <attribute name="indexRange" type="{http://www.w3.org/2001/XMLSchema}string" />
     ///      <attribute name="indexRangeExact" type="{http://www.w3.org/2001/XMLSchema}boolean" default="false" />
     ///      <anyAttribute processContents='lax' namespace='##other'/>
     ///    </restriction>
     ///  </complexContent>
     ///</complexType>
     ///</example>

     
    [System.Xml.Serialization.XmlType(TypeName = "SegmentBaseType")]
    public class SegmentBaseType
    {

        private URLType initialization;
        private URLType representationIndex;
        private List<XmlElement> anies;
        private uint timescale;
        private ulong presentationTimeOffset;
        private string timeShiftBufferDepth;
        private String indexRange;
        private Boolean indexRangeExact;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public SegmentBaseType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "Initialization")]
        public URLType Initialization
        {
            get { return initialization; }
            set { initialization = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "RepresentationIndex")]
        public URLType RepresentationIndex
        {
            get { return representationIndex; }
            set { representationIndex = value; }
        }

        [System.Xml.Serialization.XmlAnyElement()]
        public List<XmlElement> Anies
        {
            get
            {
                if (this.anies == null)
                {
                    this.anies = new List<XmlElement>();
                }
                return anies;
            }
            set { anies = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "timescale", DataType = "unsignedInt")]
        public uint Timescale
        {
            get { return timescale; }
            set { timescale = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "presentationTimeOffset", DataType = "unsignedLong")]
        public ulong PresentationTimeOffset
        {
            get { return presentationTimeOffset; }
            set { presentationTimeOffset = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "timeShiftBufferDepth", DataType = "duration")]
        public string TimeShiftBufferDepth
        {
            get { return timeShiftBufferDepth; }
            set { timeShiftBufferDepth = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "indexRange")]
        public String IndexRange
        {
            get { return indexRange; }
            set { indexRange = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "indexRangeExact")]
        public Boolean IndexRangeExact
        {
            get { return indexRangeExact; }
            set { indexRangeExact = value; }
        }

        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }
    }
}
