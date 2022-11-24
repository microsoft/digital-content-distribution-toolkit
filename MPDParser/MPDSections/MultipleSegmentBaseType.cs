// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
 ///<para>C# class for MultipleSegmentBaseType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="MultipleSegmentBaseType">
 ///  <complexContent>
 ///    <extension base="{urn:mpeg:dash:schema:mpd:2011}SegmentBaseType">
 ///      <sequence>
 ///        <element name="SegmentTimeline" type="{urn:mpeg:dash:schema:mpd:2011}SegmentTimelineType" minOccurs="0"/>
 ///        <element name="BitstreamSwitching" type="{urn:mpeg:dash:schema:mpd:2011}URLType" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute name="duration" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="startNumber" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "MultipleSegmentBaseType")]
    public class MultipleSegmentBaseType : SegmentBaseType
    {
        private SegmentTimelineType segmentTimeline;
        private URLType bitstreamSwitching;
        private uint duration;
        private uint startNumber;

        public MultipleSegmentBaseType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "SegmentTimeline")]
        public SegmentTimelineType SegmentTimeline
        {
            get { return segmentTimeline; }
            set { segmentTimeline = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "BitstreamSwitching")]
        public URLType BitstreamSwitching
        {
            get { return bitstreamSwitching; }
            set { bitstreamSwitching = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "duration", DataType = "unsignedInt")]
        public uint Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "startNumber", DataType = "unsignedInt")]
        public uint StartNumber
        {
            get { return startNumber; }
            set { startNumber = value; }
        }

    }
}
