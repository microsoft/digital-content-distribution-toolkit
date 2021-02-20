using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
 ///<para>C# class for actuateType.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///<para>
 ///<example>
 ///<simpleType name="actuateType">
 ///  <restriction base="{http:///www.w3.org/2001/XMLSchema}token">
 ///    <enumeration value="onLoad"/>
 ///    <enumeration value="onRequest"/>
 ///  </restriction>
 ///</simpleType>
 ///</example>
 ///
 
    [XmlType(TypeName = "actuateType", Namespace = "http:///www.w3.org/1999/xlink")]
    public enum ActuateType
    {
        [XmlEnum("onRequest")]
        ON_REQUEST=0,
        [XmlEnum("onLoad")]
        ON_LOAD
    }
}
