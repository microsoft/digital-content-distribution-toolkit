using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    [System.Xml.Serialization.XmlType(TypeName = "EventStreamType")]
    public class EventStreamType
    {

        private List<EventType> events;
        private List<System.Xml.XmlElement> anies;
        private String href;
        private ActuateType actuate;
        private String schemeIdUri;
        private String value;
        private uint timescale;

        public EventStreamType() { }

        [System.Xml.Serialization.XmlElement("Event")]
        public List<EventType> Events
        {
            get
            {
                if (this.events == null)
                {
                    events = new List<EventType>();
                }
                return events;
            }
            set { events = value; }
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
                return anies;
            }
            set { anies = value; }
        }

        [System.Xml.Serialization.XmlAttribute(Namespace = "http://www.w3.org/1999/xlink")]
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
            get;
            set;
        }

        //@XmlAttribute(required = true)
        [System.Xml.Serialization.XmlAttribute(DataType = "anyURI")]
        public String SchemeIdUri
        {
            get { return schemeIdUri; }
            set { schemeIdUri = value; }
        }
        [System.Xml.Serialization.XmlAttribute()]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [System.Xml.Serialization.XmlAttribute(DataType = "unsignedInt")]
        public uint Timescale
        {
            get { return timescale; }
            set { timescale = value; }
        }
    }
}
