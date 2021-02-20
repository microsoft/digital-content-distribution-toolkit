using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for EventType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="EventType">
 ///  <simpleContent>
 ///    <extension base="<http://www.w3.org/2001/XMLSchema>string">
 ///      <attribute name="presentationTime" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" default="0" />
 ///      <attribute name="duration" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" />
 ///      <attribute name="id" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </simpleContent>
 ///</complexType>
 ///</example>
 
    [System.Xml.Serialization.XmlType(TypeName = "EvenetType")]
    public class EventType
    {
        private String value;
        private ulong presentationTime;
        private ulong duration;
        private uint id;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public EventType() { }

        [System.Xml.Serialization.XmlText()]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        
        [System.Xml.Serialization.XmlAttribute(DataType = "unsignedLong")]
        public ulong PresentationTime
        {
            get { return presentationTime; }
            set { presentationTime = value; }
        }
        
        [System.Xml.Serialization.XmlAttribute(DataType = "unsignedLong")]
        public ulong Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        
        [System.Xml.Serialization.XmlAttribute(DataType = "unsignedInt")]
        public uint Id
        {
            get { return id; }
            set { id = value; }
        }
       
        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }
    }
}
