using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.AIEvents
{
    /// <summary>
    /// Represents the Base AI Event Name
    /// </summary>
    public abstract class BaseAIEvent
    {
        /// <summary>
        /// Returns the default Event Name
        /// </summary>
        /// <returns></returns>
        public virtual string GetAIEventName()
        {
            return this.GetType().Name;
            
        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                string propName = prop.Name;

                var val = this.GetType().GetProperty(propName).GetValue(this, null);

                if (val != null)
                {
                    ret.Add(propName, val.ToString());
                }
                else
                {
                    ret.Add(propName, null);
                }
            }

            return ret;
        }
    }
}
