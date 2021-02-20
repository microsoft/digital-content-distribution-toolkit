using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IIS.Media.DASH.MPDParser;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
 ///<para>C# class for AdaptationSetType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="AdaptationSetType">
 ///  <complexContent>
 ///    <extension base="{urn:mpeg:dash:schema:mpd:2011}RepresentationBaseType">
 ///      <sequence>
 ///        <element name="Accessibility" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="Role" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="Rating" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="Viewpoint" type="{urn:mpeg:dash:schema:mpd:2011}DescriptorType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="ContentComponent" type="{urn:mpeg:dash:schema:mpd:2011}ContentComponentType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="BaseURL" type="{urn:mpeg:dash:schema:mpd:2011}BaseURLType" maxOccurs="unbounded" minOccurs="0"/>
 ///        <element name="SegmentBase" type="{urn:mpeg:dash:schema:mpd:2011}SegmentBaseType" minOccurs="0"/>
 ///        <element name="SegmentList" type="{urn:mpeg:dash:schema:mpd:2011}SegmentListType" minOccurs="0"/>
 ///        <element name="SegmentTemplate" type="{urn:mpeg:dash:schema:mpd:2011}SegmentTemplateType" minOccurs="0"/>
 ///        <element name="Representation" type="{urn:mpeg:dash:schema:mpd:2011}RepresentationType" maxOccurs="unbounded" minOccurs="0"/>
 ///      </sequence>
 ///      <attribute ref="{http:///www.w3.org/1999/xlink}href"/>
 ///      <attribute ref="{http:///www.w3.org/1999/xlink}actuate"/>
 ///      <attribute name="id" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="group" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="lang" type="{http:///www.w3.org/2001/XMLSchema}language" />
 ///      <attribute name="contentType" type="{http:///www.w3.org/2001/XMLSchema}string" />
 ///      <attribute name="par" type="{urn:mpeg:dash:schema:mpd:2011}RatioType" />
 ///      <attribute name="minBandwidth" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="maxBandwidth" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="minWidth" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="maxWidth" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="minHeight" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="maxHeight" type="{http:///www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="minFrameRate" type="{urn:mpeg:dash:schema:mpd:2011}FrameRateType" />
 ///      <attribute name="maxFrameRate" type="{urn:mpeg:dash:schema:mpd:2011}FrameRateType" />
 ///      <attribute name="segmentAlignment" type="{urn:mpeg:dash:schema:mpd:2011}ConditionalUintType" default="false" />
 ///      <attribute name="subsegmentAlignment" type="{urn:mpeg:dash:schema:mpd:2011}ConditionalUintType" default="false" />
 ///      <attribute name="subsegmentStartsWithSAP" type="{urn:mpeg:dash:schema:mpd:2011}SAPType" default="0" />
 ///      <attribute name="bitstreamSwitching" type="{http:///www.w3.org/2001/XMLSchema}boolean" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </complexContent>
 ///</complexType>
 ///</example>
 ///
 ///
 
    [System.Xml.Serialization.XmlType()]
    public class AdaptationSetType:RepresentationBaseType
    {
    protected List<DescriptorType> accessibilities;
    protected List<DescriptorType> roles;
    protected List<DescriptorType> ratings;
    protected List<DescriptorType> viewpoints;
    protected List<ContentComponentType> contentComponents;
    protected List<BaseURLType> baseURLs;
    protected SegmentBaseType segmentBase;
    protected SegmentListType segmentList;
    protected SegmentTemplateType segmentTemplate;
    protected List<RepresentationType> representations;
    protected String href;
    protected ActuateType actuate;
    protected uint id;
    protected uint group;
    protected String lang;
    protected String contentType;
    protected String par;
    protected uint minBandwidth;
    protected uint maxBandwidth;
    protected uint minWidth;
    protected uint maxWidth;
    protected uint minHeight;
    protected uint maxHeight;
    protected String minFrameRate;
    protected String maxFrameRate;
    protected String segmentAlignment;
    protected String subsegmentAlignment;
    protected long subsegmentStartsWithSAP;
    protected Boolean bitstreamSwitching;

    public AdaptationSetType() { }
        
           [System.Xml.Serialization.XmlElement(ElementName="Accessibility")]
    public List<DescriptorType> Accessibilities{
               get{
                   if(this.accessibilities == null){
                       accessibilities = new List<DescriptorType>();
                   }
                   return this.accessibilities;
               }set{
                   this.accessibilities = value;
               }
          }
   [System.Xml.Serialization.XmlElement(ElementName="Role")]
    public List<DescriptorType> Roles{
       get{ 
           if(roles == null){
               roles= new List<DescriptorType>();
           }
           return this.roles;
       } 
       set{
           this.roles = value;
       }
   }
   [System.Xml.Serialization.XmlElement(ElementName="Rating")]
    public List<DescriptorType> Ratings{
       get{
           if(this.ratings == null){
            this.ratings= new List<DescriptorType>();
           }
           return this.ratings;
       } set{
       this.ratings = value;
       }}
    [System.Xml.Serialization.XmlElement(ElementName="Viewpoint")]
    public List<DescriptorType> Viewpoints{
        
        get{ 
            if(this.viewpoints == null){
                this.viewpoints = new List<DescriptorType>();
            }

            return this.viewpoints;
        
        } set{
            this.viewpoints = value;
        }}
    [System.Xml.Serialization.XmlElement(ElementName="ContentComponent")]
    public List<ContentComponentType> ContentComponents{
        get{ 
            if(this.contentComponents == null){
                this.contentComponents = new List<ContentComponentType>();
            }
            return this.contentComponents;
        } set{
            this.contentComponents = value;
        }}
    [System.Xml.Serialization.XmlElement(ElementName="BaseURL")]
    public List<BaseURLType> BaseURLs{
        get{ 
            if(baseURLs == null){
                this.baseURLs = new List<BaseURLType>();
            }
            return this.baseURLs;
        } set{
            this.baseURLs = value;
        }}
    [System.Xml.Serialization.XmlElement(ElementName="SegmentBase")]
    public SegmentBaseType SegmentBase{
        get{ return this.segmentBase;
        } 
        set{
            this.segmentBase = value;
        }}
    [System.Xml.Serialization.XmlElement(ElementName="SegmentList")]
    public SegmentListType SegmentList{
        get{ 
            return this.segmentList;
        } 
        set{
            this.segmentList = value;
        }}
    [System.Xml.Serialization.XmlElement(ElementName="SegmentTemplate")]
    public SegmentTemplateType SegmentTemplate{
        get{ 
            return this.segmentTemplate;
        } 
        set{
            this.segmentTemplate = value;
        }}
    [System.Xml.Serialization.XmlElement(ElementName="Representation")]
    public List<RepresentationType> Representations{
        get{ 
            if(representations == null){
                representations = new List<RepresentationType>();
            }
            return this.representations;
        } 
        set{
            this.representations = value;
        }}

    [System.Xml.Serialization.XmlAttribute(AttributeName="href",Namespace = "http:///www.w3.org/1999/xlink")]
    public String Href{
        get{ 
            
            return this.href;
        } 
        set{
            this.href = value;
        }}

        /// <info>
        /// This attribute is throwing an exception as there is a default value assigned to this variable in XSD but it is not included in the MPD
        /// </info>
    [System.Xml.Serialization.XmlAttribute(AttributeName="actuate", Namespace = "http:///www.w3.org/1999/xlink")]
        public string ActuateAsString{
        get{ 
            return this.actuate.ToString();
        }
        set{
            if(string.IsNullOrEmpty(value)){
                //this.Actuate = default(ActuateType);
                this.Actuate = ActuateType.ON_REQUEST;
            }
            else{
                this.actuate = (ActuateType)Enum.Parse(typeof(ActuateType),value);
            }
        }}

        [System.Xml.Serialization.XmlIgnore()]
        public ActuateType Actuate
        {
            get;set;
        }

    [System.Xml.Serialization.XmlAttribute(AttributeName="id",DataType="unsignedInt")]
    public uint Id{
        get{ 
            return this.id;
        }
        set{
         this.id=value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="group",DataType = "unsignedInt")]
    public uint Group{
        get{
            return this.group;
        }
        set{
            this.group = value;
        }}
    
     [System.Xml.Serialization.XmlAttribute(AttributeName="lang",DataType="language")]
    ///@XmlJavaTypeAdapter(CollapsedStringAdapter.class)
    public String Lang{
         
         get{ return this.lang;
         
         } 
         set{
            this.lang = value;
         }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="contentType")]
    public String ContentType{
        
        get{ 
            return this.contentType;
        } 
        set{
            this.contentType = value;
        }}


    [System.Xml.Serialization.XmlAttribute(AttributeName="par")]
    public String Par{
        get{
            return this.par;
        } 
        
        set{
            this.par = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="minBandwidth", DataType="unsignedInt")]
    public uint MinBandwidth{
        get{
            return this.minBandwidth;
        } 
        
        set{
            this.minBandwidth = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="maxBandwidth", DataType="unsignedInt")]
    public uint MaxBandwidth
    {
        get{ 
            return this.maxBandwidth;
        } 
        set{
            this.maxBandwidth = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="minWidth", DataType="unsignedInt")]
    public uint MinWidth
    {
        get{ 
            return this.minWidth;
        } set{
            this.minWidth = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="maxWidth", DataType="unsignedInt")]
    public uint MaxWidth
    {
        get{
            return this.maxWidth;
        } 
        set{
            this.maxWidth = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="minHeight", DataType="unsignedInt")]
    public uint MinHeight
    {
        get{ 
            return this.minHeight;
            
            } 
        set{
            this.minHeight = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="maxHeight", DataType="unsignedInt")]
    public uint MaxHeight
    {
        get{ 
            return this.minHeight;
            
            } 
        set{
            this.minHeight = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="minFrameRate")]
    public String MinFrameRate{get{ 
            return this.minFrameRate;
            
            } 
        set{
            this.minFrameRate = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="maxFrameRate")]
    public String MaxFrameRate{get{ 
            return this.maxFrameRate;
            
            } 
        set{
            this.maxFrameRate = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="segmentAlignment")]
    public String SegmentAlignment{get{ 
            return this.segmentAlignment;
            
            } 
        set{
            this.segmentAlignment = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="subsegmentAlignment")]
    public String SubsegmentAlignment{
        get{ 
            return this.subsegmentAlignment;
            
            } 
        set{
            this.subsegmentAlignment = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="subsegmentStartsWithSAP")]
    public long SubsegmentStartsWithSAP{
        get{ 
            return this.subsegmentStartsWithSAP;
            
            } 
        set{
            this.subsegmentStartsWithSAP = value;
        }}
    [System.Xml.Serialization.XmlAttribute(AttributeName="bitstreamSwitching")]
    public Boolean BitstreamSwitching{get{ 
            return this.bitstreamSwitching;
            
            } 
        set{
            this.bitstreamSwitching = value;
        }}
    }
}
