using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for SegmentTemplateType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="SegmentTemplateType">
 ///  <complexContent>
 ///    <extension base="{urn:mpeg:dash:schema:mpd:2011}MultipleSegmentBaseType">
 ///      <attribute name="media" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="index" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="initialization" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="bitstreamSwitching" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "SegmentTemplateType")]
    public class SegmentTemplateType : MultipleSegmentBaseType
    {
        private String media;
        private String index;
        private String initializationAttribute;
        private String bitstreamSwitchingAttribute;

        public SegmentTemplateType() { }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "media")]
        public String Media
        {
            get { return media; }
            set { media = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "index")]
        public String Index
        {
            get { return index; }
            set { index = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "initialization")]
        public String InitializationAttribute
        {
            get { return initializationAttribute; }
            set { initializationAttribute = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "bitstreamSwitching")]
        public String BitstreamSwitchingAttribute
        {
            get { return bitstreamSwitchingAttribute; }
            set { bitstreamSwitchingAttribute = value; }
        }
    }
}
