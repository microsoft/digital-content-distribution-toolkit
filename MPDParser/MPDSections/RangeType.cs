// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{

    
 ///<para>C# class for RangeType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="RangeType">
 ///  <complexContent>
 ///    <restriction base="{http:///www.w3.org/2001/XMLSchema}anyType">
 ///      <attribute name="starttime" type="{http:///www.w3.org/2001/XMLSchema}duration" />
 ///      <attribute name="duration" type="{http:///www.w3.org/2001/XMLSchema}duration" />
 ///    </restriction>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 

    [System.Xml.Serialization.XmlType("RangeType")]
    public class RangeType
    {
        private string starttime;
        private string duration;

        public RangeType() { }

    [System.Xml.Serialization.XmlAttribute(DataType = "duration", AttributeName="starttime")]
    public string Starttime
    {
        get { return starttime; }
        set { starttime = value; }
    }

    [System.Xml.Serialization.XmlAttribute(DataType = "duration", AttributeName = "duration")]
    public string Duration
    {
        get { return duration; }
        set { duration = value; }
    }
    }
}
