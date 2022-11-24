// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    
 ///<para>C# class for SubRepresentationType complex type.
 ///
 ///<para>The following schema fragment specifies the expected content contained within this class.
 ///
 ///<example>
 ///<complexType name="SubRepresentationType">
 ///  <complexContent>
 ///    <extension base="{urn:mpeg:dash:schema:mpd:2011}RepresentationBaseType">
 ///      <attribute name="level" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="dependencyLevel" type="{urn:mpeg:dash:schema:mpd:2011}UIntVectorType" />
 ///      <attribute name="bandwidth" type="{http://www.w3.org/2001/XMLSchema}unsignedInt" />
 ///      <attribute name="contentComponent" type="{urn:mpeg:dash:schema:mpd:2011}StringVectorType" />
 ///      <anyAttribute processContents='lax' namespace='##other'/>
 ///    </extension>
 ///  </complexContent>
 ///</complexType>
 ///</example>

 
    [System.Xml.Serialization.XmlType(TypeName = "SubRepresentationType")]
    public class SubRepresentationType : RepresentationBaseType
    {
        private uint level;
        private List<Int64> dependencyLevels;
        private uint bandwidth;
        private List<String> contentComponents;

        public SubRepresentationType() { }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "level", DataType = "unsignedInt")]
        public uint Level
        {
            get { return level; }
            set { level = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "dependencyLevel")]
        public List<Int64> DependencyLevels
        {
            get
            {
                if (dependencyLevels == null)
                {
                    dependencyLevels = new List<Int64>();
                }
                return dependencyLevels;
            }
            set { dependencyLevels = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "bandwidth", DataType = "unsignedInt")]
        public uint Bandwidth
        {
            get { return bandwidth; }
            set { bandwidth = value; }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "contentComponent")]
        public List<String> ContentComponents
        {
            get
            {
                if (contentComponents == null)
                {
                    this.contentComponents = new List<string>();
                }
                return contentComponents;
            }
            set { contentComponents = value; }
        }
    }
}
