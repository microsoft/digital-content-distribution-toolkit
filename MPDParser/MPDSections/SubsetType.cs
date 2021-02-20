using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for SubsetType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="SubsetType">
 ///  <complexContent>
 ///    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 ///      <attribute name="contains" use="required" type="{urn:mpeg:dash:schema:mpd:2011}UIntVectorType" />
 ///      <attribute name="id" type="{http://www.w3.org/2001/XMLSchema}string" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </restriction>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "SubsetType")]
    public class SubsetType
    {

        private List<Int64> contains;
        private String id;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public SubsetType() { }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "contains")]
        public List<Int64> Contains
        {
            get
            {
                if (this.contains == null)
                {
                    this.contains = new List<long>();
                }
                return contains;
            }
            set { contains = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "id")]
        public String Id
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
