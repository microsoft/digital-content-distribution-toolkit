using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Identity
{
    /// <summary>
    /// Login With Phone For Partners Request
    /// </summary>
    public class LoginWithPhoneForPartnersRequest
    {
        public string PhoneNumber { get; set; }
        public bool UseVoice { get; set; }
    }

    /// <summary>
    /// Login With Phone For Partners Response
    /// </summary>
    public class LoginWithPhoneForPartnersResponse
    {
        public string Mode { get; set; }

        public string AccountStatus { get; set; }

        public string AuthType { get; set; }

        public int RemainingAttempts { get; set; }

        public int LockEndTime { get; set; }

        public string AuthDetails { get; set; }

        public string SmsProvider { get; set; }

        public bool IsVoiceOtpEnabled { get; set; }
    }
}
