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

        public struct Policy
        {
            public const string PolicyPermissions = "rwlcda";
            
        }

        public struct SaSToken
        {
            public const int expiryInHours = 2;
            
        }
        public struct ContainerConstants
        {
            public const string ContainerSuffix = "-raw";
            
        }

        
    }
}
