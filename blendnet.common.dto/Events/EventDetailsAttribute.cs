using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventDetailsAttribute:Attribute
    {
        public string EventName { get; set; }

        public bool PerformJsonSerialization { get; set; } = true;
    }
}
