// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    class utils
    {
        public static Stream getResourceStream(string embeddedResourceName)
        {
            Stream embeddedResourceStream = null;
            Assembly assembly = typeof(MPDParser).Assembly;
            try
            {
                string[] resouces = assembly.GetManifestResourceNames();
                foreach (string resource in resouces)
                {
                    if (resource.Contains(embeddedResourceName))
                    {
                        System.Diagnostics.Debug.WriteLine("MPDParser::utils::getResourceStream::Compelete Schema Path::" + resource);
                        embeddedResourceStream = assembly.GetManifestResourceStream(resource);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return embeddedResourceStream;
        }
    }
}
