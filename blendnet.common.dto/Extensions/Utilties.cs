using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace blendnet.common.dto.Extensions
{
    public class Utilties
    {
        /// <summary>
        /// GetJsonSerializerOptions
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetJsonSerializerOptions(bool ignoreNull = false, bool prettyPrint = false)
        {
            JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            if (ignoreNull)
            {
                _jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
            
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            if (prettyPrint)
            {
                _jsonSerializerOptions.WriteIndented = true;
            }
            
            return _jsonSerializerOptions;
        }
    }
}
