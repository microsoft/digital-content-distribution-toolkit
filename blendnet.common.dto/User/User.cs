using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

    }
}
