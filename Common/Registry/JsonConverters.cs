using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Vintagestory.API;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Implementation of JsonConverter that converts objects to an instance of a JsonObject
    /// </summary>
    public class JsonObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return new JsonObject(null);

            return new JsonObject(JObject.Load(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }

    public class JsonAttributesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
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
