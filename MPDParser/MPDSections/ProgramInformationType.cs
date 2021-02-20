using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for ProgramInformationType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="ProgramInformationType">
 ///  <complexContent>
 ///    <restriction base="{http:///www.w3.org/2001/XMLSchema}anyType">
 ///      <sequence>
 ///        <element name="Title" type="{http:///www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 ///        <element name="Source" type="{http:///www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 ///        <element name="Copyright" type="{http:///www.w3.org/2001/XMLSchema}string" minOccurs="0"/>
 ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute name="lang" type="{http:///www.w3.org/2001/XMLSchema}language" />
 ///      <attribute name="moreInformationURL" type="{http:///www.w3.org/2001/XMLSchema}anyURI" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </restriction>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.SerializableAttribute()]
    public class ProgramInformationType
    {
        private string title;
        private String source;
        private String copyright;
        private List<System.Xml.XmlElement> anies;
        private String lang;
        private String moreInformationURL;
        private Dictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public ProgramInformationType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "Title")]
        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "Source")]
        public String Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "Copyright")]
        public String Copyright
        {
            get
            {
                return this.copyright;
            }
            set
            {
                this.copyright = value;
            }
        }


        [System.Xml.Serialization.XmlAnyElement()]
        public List<System.Xml.XmlElement> Anies
        {
            get
            {
                if (anies == null)
                {
                    anies = new List<System.Xml.XmlElement>();
                }
                return this.anies;
            }
            set
            {
                this.anies = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "lang", DataType = "language")]
        public String Lang
        {
            get
            {
                return lang;
            }

            set
            {
                this.lang = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "moreInformationURL", DataType = "anyURI")]
        /// @XmlSchemaType(name = "anyURI")
        public String MoreInformationURL
        {
            get
            {
                return this.moreInformationURL;
            }
            set
            {
                this.moreInformationURL = value;
            }
        }

        //[System.Xml.Serialization.XmlAnyAttribute()]
        //public Dictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        //{
        //    get
        //    {
        //        return otherAttributes;
        //    }
        //}

        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
            // set { otherAttributes = value; }
        }
    }
}
