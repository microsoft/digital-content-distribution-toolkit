using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for SegmentListType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="SegmentListType">
 ///  <complexContent>
 ///    <extension base="{urn:mpeg:dash:schema:mpd:2011}MultipleSegmentBaseType">
 ///      <sequence>
 ///        <element name="SegmentURL" type="{urn:mpeg:dash:schema:mpd:2011}SegmentURLType" maxOccurs="unbounded" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute ref="{http://www.w3.org/1999/xlink}href"/>
 ///      <attribute ref="{http://www.w3.org/1999/xlink}actuate"/>
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "SegmentListType")]
    public class SegmentListType : MultipleSegmentBaseType
    {
        private List<SegmentURLType> segmentURLs;
        private String href;
        private ActuateType actuate;

        public SegmentListType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "SegmentURL")]
        public List<SegmentURLType> SegmentURLs
        {
            get
            {
                if (this.segmentURLs == null)
                {
                    this.segmentURLs = new List<SegmentURLType>();
                } 
                return segmentURLs;
            }
            set { segmentURLs = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public String Href
        {
            get { return href; }
            set { href = value; }
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
            get;set;
        }
    }
}
