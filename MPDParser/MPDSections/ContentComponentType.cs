// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
    /// c# class for ContentComponentType complex type.
    ///
    ///<para>The following schema fragment specifies the expected content contained within this class.
    ///
    ///<example>
    ///<complexType name="ContentComponentType">
    ///  <complexContent>
    ///    <restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
    ///      <sequence>
    ///        <element name="Accessibility" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
    ///        <element name="Role" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
    ///        <element name="Rating" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
    ///        <element name="Viewpoint" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
    ///        <any processContents='lax' namespace='##other' maxOccurs="unbounded" minOccurs="0"/>
    ///      </sequence>
    ///      <attribute name="id" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
    ///      <attribute name="lang" type="{http://www.w3.org/2001/XMLSchema}language" />
    ///      <attribute name="contentType" type="{http://www.w3.org/2001/XMLSchema}string" />
    ///      <attribute name="par" type="{urn:mpeg:dash:schema:mpd:2011}RatioType" />
    ///      <anyAttribute processContents='lax' namespace='##other'/>
    ///    </restriction>
    ///  </complexContent>
    ///</complexType>
    ///</example>
    
    
    

    [System.Xml.Serialization.XmlType(TypeName = "ContentComponentType")]
    public class ContentComponentType
    {
        [System.Xml.Serialization.XmlElement(ElementName = "Accessibility")]
        private List<DescriptorType> accessibilities;
        [System.Xml.Serialization.XmlElement(ElementName = "Role")]
        private List<DescriptorType> roles;
        [System.Xml.Serialization.XmlElement(ElementName = "Rating")]
        private List<DescriptorType> ratings;
        [System.Xml.Serialization.XmlElement(ElementName = "Viewpoint")]
        private List<DescriptorType> viewpoints;
        [System.Xml.Serialization.XmlAnyElement()]
        private List<System.Xml.XmlElement> anies;
        [System.Xml.Serialization.XmlAttribute(AttributeName = "id", DataType = "unsignedInt")]
        private long id;
        [System.Xml.Serialization.XmlAttribute(AttributeName = "lang", DataType = "language")]
        //@XmlJavaTypeAdapter(CollapsedStringAdapter.class)
        private String lang;
        [System.Xml.Serialization.XmlAttribute(AttributeName = "conntentType")]
        private String contentType;
        [System.Xml.Serialization.XmlAttribute(AttributeName = "par")]
        private String par;
        [System.Xml.Serialization.XmlAnyAttribute()]
        private IDictionary<System.Xml.XmlQualifiedName, String> otherAttributes = new Dictionary<System.Xml.XmlQualifiedName, String>();


        public ContentComponentType() { }


        public List<DescriptorType> Accessibilities
        {
            get
            {

                if (this.accessibilities == null)
                {
                    this.accessibilities = new List<DescriptorType>();
                }
                return accessibilities;
            }
            set { accessibilities = value; }
        }

        public List<DescriptorType> Roles
        {
            get
            {
                if (this.roles == null)
                {
                    this.roles = new List<DescriptorType>();
                }
                return roles;
            }
            set { roles = value; }
        }
        public List<DescriptorType> Ratings
        {
            get
            {

                if (this.ratings == null)
                {
                    this.ratings = new List<DescriptorType>();
                }
                return ratings;
            }
            set { ratings = value; }
        }
        public List<DescriptorType> Viewpoints
        {
            get
            {
                if (this.viewpoints == null)
                {
                    viewpoints = new List<DescriptorType>();
                }
                return viewpoints;
            }
            set { viewpoints = value; }
        }
        public List<System.Xml.XmlElement> Anies
        {
            get {
                if (anies == null)
                {
                    anies = new List<System.Xml.XmlElement>();
                }
                return anies; }
            set { anies = value; }
        }

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        public String Lang
        {
            get { return lang; }
            set { lang = value; }
        }

        public String ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        public String Par
        {
            get { return par; }
            set { par = value; }
        }

        public IDictionary<System.Xml.XmlQualifiedName, String> OtherAttributes
        {
            get { return otherAttributes; }
        }


    }
}
