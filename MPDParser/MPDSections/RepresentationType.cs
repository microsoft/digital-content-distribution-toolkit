using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for RepresentationType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="RepresentationType">
 ///  <complexContent>
 ///    <extension base="{urn:mpeg:dash:schema:mpd:2011}RepresentationBaseType">
 ///      <sequence>
 ///        <element name="BaseURL" type="{urn:mpeg:dash:schema:mpd:2011}BaseURLType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="SubRepresentation" type="{urn:mpeg:dash:schema:mpd:2011}SubRepresentationType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="SegmentBase" type="{urn:mpeg:dash:schema:mpd:2011}SegmentBaseType" minOccurs="0"/>
 ///        <element name="SegmentList" type="{urn:mpeg:dash:schema:mpd:2011}SegmentListType" minOccurs="0"/>
 ///        <element name="SegmentTemplate" type="{urn:mpeg:dash:schema:mpd:2011}SegmentTemplateType" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute name="id" use="required" type="{urn:mpeg:dash:schema:mpd:2011}StringNoWhitespaceType" />
 ///      <attribute name="bandwidth" use="required" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="qualityRanking" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="dependencyId" type="{urn:mpeg:dash:schema:mpd:2011}StringVectorType" />
 ///      <attribute name="mediaStreamStructureId" type="{urn:mpeg:dash:schema:mpd:2011}StringVectorType" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "RepresentationType")]
    public class RepresentationType : RepresentationBaseType
    {
        protected List<BaseURLType> baseURLs;
        protected List<SubRepresentationType> subRepresentations;
        protected SegmentBaseType segmentBase;
        protected SegmentListType segmentList;
        protected SegmentTemplateType segmentTemplate;
        protected String id;
        protected uint bandwidth;
        protected uint qualityRanking;
        protected List<String> dependencyIds;
        protected List<String> mediaStreamStructureIds;


        public RepresentationType() { }

        [System.Xml.Serialization.XmlElement(ElementName = "BaseURL")]
        public List<BaseURLType> BaseURLs
        {
            get
            {
                if (this.baseURLs == null)
                {
                    this.baseURLs = new List<BaseURLType>();
                }
                return baseURLs;
            }
            set { baseURLs = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "SubRepresentation")]
        public List<SubRepresentationType> SubRepresentations
        {
            get
            {
                if (this.subRepresentations == null)
                {
                    this.subRepresentations = new List<SubRepresentationType>();
                }
                return subRepresentations;
            }
            set { subRepresentations = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "SegmentBase")]
        public SegmentBaseType SegmentBase
        {
            get { return segmentBase; }
            set { segmentBase = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "SegmentList")]
        public SegmentListType SegmentList
        {
            get { return segmentList; }
            set { segmentList = value; }
        }

        [System.Xml.Serialization.XmlElement(ElementName = "SegmentTemplate")]
        public SegmentTemplateType SegmentTemplate
        {
            get { return segmentTemplate; }
            set { segmentTemplate = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "id")]
        public String Id
        {
            get { return id; }
            set { id = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "bandwidth", DataType = "unsignedInt")]
        public uint Bandwidth
        {
            get { return bandwidth; }
            set { bandwidth = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "qualityRanking", DataType = "unsignedInt")]
        public uint QualityRanking
        {
            get { return qualityRanking; }
            set { qualityRanking = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "dependencyId")]
        public List<String> DependencyIds
        {
            get
            {
                if (this.dependencyIds == null)
                {
                    this.dependencyIds = new List<string>();
                }
                return dependencyIds;
            }
            set { dependencyIds = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "mediaStreamStructureId")]
        public List<String> MediaStreamStructureIds
        {
            get
            {
                if (this.mediaStreamStructureIds == null)
                {
                    this.mediaStreamStructureIds = new List<string>();
                }
                return mediaStreamStructureIds;
            }
            set { mediaStreamStructureIds = value; }
        }

    }
}
