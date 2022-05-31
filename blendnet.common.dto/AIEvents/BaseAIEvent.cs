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

        /// <summary>
        /// Creates a dictionary based on Property Name and Values.
        /// Assuming we will not have complex object properties.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            //add a default property depecting it is a blendnet custom event. Based on this, adaptive sampling will be skipped for this.
            ret.Add(ApplicationConstants.ApplicationInsightsDefaultEventProperty.BlendNetCustomEvent, 
                    ApplicationConstants.ApplicationInsightsDefaultEventProperty.BlendNetCustomEvent);

            //add a unique event id to every event
            ret.Add(ApplicationConstants.ApplicationInsightsDefaultEventProperty.BlendNetCustomEventId,
                    Guid.NewGuid().ToString());

            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                string propName = prop.Name;

                var val = this.GetType().GetProperty(propName).GetValue(this, null);

                if (val != null)
                {
                    // flatten dictionary
                    if (val is Dictionary<string, string> valDict)
                    {
                        valDict.Select(kv =>
                        {
                            string newKey = $"{propName}_{kv.Key}";
                            return new KeyValuePair<string, string>(newKey, kv.Value);
                        }).Aggregate(ret, (dict, kv) =>
                        {
                            dict.Add(kv.Key, kv.Value);
                            return dict;
                        });
                    }
                    else // simple value
                    {
                        ret.Add(propName, val.ToString());
                    }
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
