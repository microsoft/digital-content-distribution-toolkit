// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for SegmentURLType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="SegmentURLType">
 ///  <complexContent>
 ///    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 ///      <sequence>
 ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute name="media" type="{http://www.w3.org/2001/XMLSchema}anyURI" />
 ///      <attribute name="mediaRange" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="index" type="{http://www.w3.org/2001/XMLSchema}anyURI" />
 ///      <attribute name="indexRange" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </restriction>
 ///  </complexContent>
 ///</complexType>
 //</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "SegmentURLType")]
    public class SegmentURLType
    {

        private List<XmlElement> anies;
        private String media;
        private String mediaRange;
        private String index;
        private String indexRange;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public SegmentURLType() { }

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

        [System.Xml.Serialization.XmlAttribute(AttributeName = "timescale", DataType = "anyURI")]
        public String Media
        {
            get { return media; }
            set { media = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "mediaRange")]
        public String MediaRange
        {
            get { return mediaRange; }
            set { mediaRange = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "index", DataType = "anyURI")]
        public String Index
        {
            get { return index; }
            set { index = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "indexRange")]
        public String IndexRange
        {
            get { return indexRange; }
            set { indexRange = value; }
        }

        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }
    }
}
