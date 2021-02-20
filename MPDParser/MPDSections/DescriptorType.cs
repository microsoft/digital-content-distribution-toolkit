using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    public class DescriptorType
    {
        private List<System.Xml.XmlElement> anies;
        private String schemeIdUri;
        private String value;
        private String id;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public DescriptorType() { }

        [System.Xml.Serialization.XmlAnyElement()]
        public List<System.Xml.XmlElement> Anies
        {
            get {
                if (this.anies == null)
                {
                    this.anies = new List<System.Xml.XmlElement>();
                }
                
                return anies; }
            set { anies = value; }
        }

        [System.Xml.Serialization.XmlAttribute(DataType = "anyURI", AttributeName = "schemeIdUri")]
        public String SchemeIdUri
        {
            get { return schemeIdUri; }
            set { schemeIdUri = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "value")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "default_KID")]
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
