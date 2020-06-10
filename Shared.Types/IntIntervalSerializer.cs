using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using Utilities.Extensions;
using Vectors;

namespace SharedT.Types
{
    public class IntIntervalSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var interval = value.To<IntInterval>();

            writer.WriteStartObject();
            writer.WritePropertyName("From");
            writer.WriteValue(interval.From);
            writer.WritePropertyName("To");
            writer.WriteValue(interval.To);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();
            return new IntInterval(
                Convert.ToInt32(properties[0].Value),
                Convert.ToInt32(properties[1].Value));
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IntInterval).IsAssignableFrom(objectType);
        }
    }
}
