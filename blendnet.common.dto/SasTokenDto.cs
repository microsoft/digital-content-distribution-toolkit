using System;

namespace blendnet.common.dto
{
    public class SasTokenDto
    {
        public string storageAccount { get; set; }
        public string containerName { get; set; }
        public string policyName { get; set; }
        public Uri sasUri { get; set; }
        public int expiryInHours { get; set; }
    }
}