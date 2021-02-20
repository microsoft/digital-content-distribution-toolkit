using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for VideoScanType.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<simpleType name="VideoScanType">
 ///  <restriction base="{http://www.w3.org/2001/XMLSchema}string">
 ///    <enumeration value="progressive"/>
 ///    <enumeration value="interlaced"/>
 ///    <enumeration value="unknown"/>
 ///  </restriction>
 ///</simpleType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "VideoScanType")]
    public enum VideoScanType
    {
        [System.Xml.Serialization.XmlEnum("progressive")]
        PROGRESSIVE,
        [System.Xml.Serialization.XmlEnum("interlaced")]
        INTERLACED,
        [System.Xml.Serialization.XmlEnum("unknown")]
        UNKNOWN,
    }
}
