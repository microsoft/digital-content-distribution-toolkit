using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for BaseURLType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="BaseURLType">
 ///  <simpleContent>
 ///    <extension base="<http:///www.w3.org/2001/XMLSchema>anyURI">
 ///      <attribute name="serviceLocation" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="byteRange" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </simpleContent>
 ///</complexType>
 ///</example>
 ///
 ///
 
    public class BaseURLType
    {
        private String txtValue;
        private String serviceLocation;
        private String byteRange;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public BaseURLType() { }

        [System.Xml.Serialization.XmlText(DataType="anyURI")]
        public String Value{
            get{
                return this.txtValue;
            }
            set{
                this.txtValue = value;
            }
        }

    [System.Xml.Serialization.XmlAttribute()]
    public String ServiceLocation{
        get{
            return this.serviceLocation;
        }
        set{
            this.serviceLocation = value;
        }
    }
    [System.Xml.Serialization.XmlAttribute()]
    public String ByteRange{
        get{
            return this.byteRange;
        }

        set{
            this.byteRange = value;
        }
    }

    [System.Xml.Serialization.XmlAttribute()]
    private IDictionary<System.Xml.XmlQualifiedName , String> OtherAttributes
    {
        get
        {
            return this.otherAttributes;
        }
    }
    }
}
