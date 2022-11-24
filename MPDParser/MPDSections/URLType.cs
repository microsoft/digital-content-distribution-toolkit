// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
 ///<para>C# class for URLType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="URLType">
 ///  <complexContent>
 ///    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 ///      <sequence>
 ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute name="sourceURL" type="{http://www.w3.org/2001/XMLSchema}anyURI" />
 ///      <attribute name="range" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </restriction>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "URLType")]
    public class URLType
    {

        private List<XmlElement> anies;
        private String sourceURL;
        private String range;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public URLType() { }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "sourceURL", DataType = "anyURI")]
        public String SourceURL
        {
            get { return sourceURL; }
            set { sourceURL = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "range")]
        public String Range
        {
            get { return range; }
            set { range = value; }
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

        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }
    }
}
