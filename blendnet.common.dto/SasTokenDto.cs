using System;

namespace blendnet.common.dto
{
    public class SasTokenDto
    {
        public string StorageAccount { get; set; }

        public string ContainerName { get; set; }
        
        public string PolicyName { get; set; }
        
        public Uri SasUri { get; set; }

        public int ExpiryInMinutes { get; set; }
    }
}