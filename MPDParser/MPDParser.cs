// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Xml.Schema;
using System.Reflection;
using System.IO;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    public class MPDParser
    {
 
        public MPDParser()
        {
        }

        /*
            Creating MPD object for MPD passed.
        */  
        /// <summary>
        /// This Method is responsible for deserizing downloaded and XLink Resolved MPD into object structure.
        /// </summary>
        /// <param name="pathToMPD">Path to already downloaded MPD file.</param>
        /// <returns></returns>
        public MPD parse(string pathToMPD)
        {
            MPD mpd = null;
            XmlReaderSettings settings = null;
            XmlReader reader = null;
            XmlSerializer serializer =null;
            XmlResolver resolver;

            try
            {
                settings = new XmlReaderSettings();
                resolver = new XmlUrlResolver();
                resolver.Credentials = CredentialCache.DefaultCredentials;
                settings.XmlResolver = resolver;
                addSchemas(settings);
                settings.Schemas.XmlResolver = resolver;
                settings.Schemas.Compile();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationEventHandler += settingsValidationEventHandler;
                reader = XmlReader.Create(pathToMPD, settings);
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(MPD));
                mpd = (MPD)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("MPDParser::parse::"+e.Message);
                throw;
            }
            finally
            {
                if(reader!=null)
                    reader.Close();
            }
            return mpd;
        }


        private void settingsValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            System.Diagnostics.Debug.WriteLine(e.Exception);
            System.Diagnostics.Debug.WriteLine(e.Severity);
        }

        private XmlReader readSchema(string embeddedSchemaName)
        {
            XmlReader schemaReaderStream = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("MPDParser::readSchema::Schema Path::" + embeddedSchemaName);
                Stream embeddedResourceStream = utils.getResourceStream(embeddedSchemaName);
                schemaReaderStream = XmlReader.Create(embeddedResourceStream);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("MPDParser::readSchema::" + e.Message);
                System.Diagnostics.Debug.WriteLine("MPDParser::readSchema::" + e.StackTrace);
                throw;
            }
            return schemaReaderStream;
        }

        private void addSchemas(XmlReaderSettings settings)
        {
            XmlReader dashMPDSchemaStream = null;
            XmlReader xlinkSchemaStream = null;
            XmlSchema dashSchema;
            XmlSchema xlinkSchema;
            try
            {
                dashMPDSchemaStream = readSchema(Constants.pathToDASHXSD);
                xlinkSchemaStream = readSchema(Constants.pathToXlinkXSD);

                dashSchema = XmlSchema.Read(dashMPDSchemaStream,ValidationCallback);
                xlinkSchema = XmlSchema.Read(xlinkSchemaStream, ValidationCallback);

                XmlSchemaImport import = new XmlSchemaImport();
                import.Namespace = Constants.XLINK_NAMESPACE;
                import.Schema = xlinkSchema;
                dashSchema.Includes.Add(import);
                settings.Schemas.Add(dashSchema);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("MPDParser::addSchemas::" + e.Message);
                throw;
            }
        }

        private void ValidationCallback(object sender, ValidationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MPDParser::addSchemas::ValidationCallback:: " + e.Message);
        }
    }
}
