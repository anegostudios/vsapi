using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    public class JsonAttributesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // This causes: Newtonsoft.Json.JsonSerializationException: 'Unexpected token while deserializing object: EndObject.' wtf?
            //if (reader.Value == null) return new JsonObject(null);

            JToken token = JToken.ReadFrom(reader);
            JsonObject var = new JsonObject(token);
            return var;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonObject var = value as JsonObject;
            var.Token.WriteTo(writer);
        }
    }
}
