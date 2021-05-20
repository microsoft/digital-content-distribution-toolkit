using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace blendnet.common.infrastructure.Util
{
    
    public class DateTimeConverter : JsonConverter
    {
        private const string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            var s = reader.Value.ToString();
            DateTime result;
            if (DateTime.TryParseExact(s, format, CultureInfo.InstalledUICulture, DateTimeStyles.None, out result))
                return result;

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(format));
        }
    }
}
