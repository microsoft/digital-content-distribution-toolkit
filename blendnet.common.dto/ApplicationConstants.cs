using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto
{
    public class ApplicationConstants
    {
        public struct CosmosContainers
        {
            public const string ContentProvider = "ContentProvider";
        }

        public struct StorageContainerSuffix
        {
            public const string Raw = "-raw";
            public const string Mezzanine = "-mezzanine";
            public const string Processed = "-processed";
            public const string Cdn = "-cdn";
        }

        public struct StorageInstanceNames
        {
            public const string CMSStorage = "CMSStorage";
            public const string CMSCDNStorage = "CMSCDNStorage";
            
        }
    }
}
