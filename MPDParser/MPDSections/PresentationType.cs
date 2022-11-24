// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for PresentationType.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///<para>
 ///<example>
 ///<simpleType name="PresentationType">
 ///  <restriction base="{http:///www.w3.org/2001/XMLSchema}string">
 ///    <enumeration value="static"/>
 ///    <enumeration value="dynamic"/>
 ///  </restriction>
 ///</simpleType>
 ///</example>

 
    [XmlType("PresentationType")]
    public enum PresentationType
    {
        [XmlEnum("static")]
        STATIC=0,
        [XmlEnum("dynamic")]
        DYNAMIC,
    }
}
