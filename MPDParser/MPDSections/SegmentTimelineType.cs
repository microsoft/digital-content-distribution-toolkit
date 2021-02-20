using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
     ///<para>C# class for SegmentTimelineType complex type.
     ///
     ///<para>The following schema fragment specifies the expected content contained within this class.
     ///
     ///<example>
     ///<complexType name="SegmentTimelineType">
     ///  <complexContent>
     ///    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
     ///      <sequence>
     ///        <element name="S" maxOccurs="unbounded">
     ///         <complexType>
     ///            <complexContent>
     ///              <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
     ///                <attribute name="t" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" />
     ///                <attribute name="d" use="required" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" />
     ///                <attribute name="r" type="{http://www.w3.org/2001/XMLSchema}integer" default="0" />
     ///                <anyAttribute processContents='lax' namespace='##other'/>
     ///             </restriction>
     ///            </complexContent>
     ///          </complexType>
     ///        </element>
     ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
     ///      </sequence>
     ///      <anyAttribute processContents='lax' namespace='##other'/>
     ///    </restriction>
     ///  </complexContent>
     ///</complexType>
     ///</example>

     
    [System.Xml.Serialization.XmlType(TypeName = "SegmentTimelineType")]
    public class SegmentTimelineType
    {

        private List<SegmentTimelineType.S> ss;
        private List<XmlElement> anies;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public SegmentTimelineType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "S")]
        public List<SegmentTimelineType.S> Ss
        {
            get { return ss; }
            set { ss = value; }
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




        
    //<para>C# class for anonymous complex type.
    //
    //<para>The following schema fragment specifies the expected content contained within this class.
    //
    //<example>
    //<complexType>
    //  <complexContent>
    //    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
    //      <attribute name="t" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" />
    //      <attribute name="d" use="required" type="{http://www.w3.org/2001/XMLSchema}unsignedLong" />
    //      <attribute name="r" type="{http://www.w3.org/2001/XMLSchema}integer" default="0" />
    //      <anyAttribute processContents='lax' namespace='##other'/>
    //    </restriction>
    //  </complexContent>
    //</complexType>
    //</example>
    //
    //
    

        [System.Xml.Serialization.XmlType(TypeName = "")]
        public class S
        {
            private ulong t;
            private ulong d;
            private ulong r;
            private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

            [System.Xml.Serialization.XmlAttribute(AttributeName = "t", DataType = "unsignedLong")]
            public ulong T
            {
                get { return t; }
                set { t = value; }
            }

            [System.Xml.Serialization.XmlAttribute(AttributeName = "d", DataType = "unsignedLong")]
            public ulong D
            {
                get { return d; }
                set { d = value; }
            }

            [System.Xml.Serialization.XmlAttribute(AttributeName = "r")]
            public ulong R
            {
                get {
                    
                    return r; 
                }
                set { r = value; }
            }

            [System.Xml.Serialization.XmlAnyAttribute()]
            public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
            {
                get { return otherAttributes; }
            }
        }
    }
}
