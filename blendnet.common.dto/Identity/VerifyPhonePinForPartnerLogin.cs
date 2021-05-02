using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Identity
{
    public class VerifyPhonePinForPartnerLoginRequest
    {
        public string PhoneNumber { get; set; }

        public int Pin { get; set; }
        
        public List<string> Permissions { get; set; }
    }

    public class VerifyPhonePinForPartnerLoginResponse
    {
        public string UserId { get; set; }
        
        public string ClientId { get; set; }
        
        public bool IsNewUser { get; set; }
        
        public string AuthDetails { get; set; }
        
        public string AuthVerificationDetail { get; set; }
        
        public string AuthenticationToken { get; set; }
        
        public long TokenCreationTime { get; set; }
        
        public string HomeDSUrl { get; set; }
        
        public string HomeKMSUrl { get; set; }
        
        public string HomeAPIUrl { get; set; }
        
        public List<object> HideBottomBarInChatCanvas { get; set; }
        
        public List<object> HelpIconInActionPalette { get; set; }
    }
}
