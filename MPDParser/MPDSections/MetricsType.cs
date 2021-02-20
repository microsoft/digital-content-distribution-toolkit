using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    public class MetricsType
    {
        private List<DescriptorType> reportings;
        private List<RangeType> ranges;
        private List<System.Xml.XmlElement> anies;
        private String metrics;
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();

        public MetricsType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "Reporting")]
        public List<DescriptorType> Reportings
        {
            get
            {
                if (this.reportings == null)
                {
                    this.reportings = new List<DescriptorType>();
                }
                return reportings;
            }
            set { reportings = value; }
        }
        
        
        [System.Xml.Serialization.XmlElement(ElementName = "Range")]
        public List<RangeType> Ranges
        {
            get
            {
                if (this.ranges == null)
                {
                    this.ranges = new List<RangeType>();
                }
                return ranges;
            }
            set { ranges = value; }
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
        
        [System.Xml.Serialization.XmlAttribute()]
        public String Metrics
        {
            get { return metrics; }
            set { metrics = value; }
        }

        [System.Xml.Serialization.XmlAnyAttribute()]
        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }
    }
}
