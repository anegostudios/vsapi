using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class JsonUtil
    {
        public static T FromBytes<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var sr = new StreamReader(stream, Encoding.UTF8, true))
                {
                    return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                }
            }
        }

        public static byte[] ToBytes<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }


        public static void PopulateObject(object toPopulate, string text, string domain, JsonSerializerSettings settings = null)
        {
            if (domain != "game")
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }
            JsonConvert.PopulateObject(text, toPopulate, settings);
        }

        public static T ToObject<T>(string text, string domain, JsonSerializerSettings settings = null)
        {
            if (domain != "game")
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }
            
            return JsonConvert.DeserializeObject<T>(text, settings);
        }
    }

    public class AssetLocationJsonParser : JsonConverter
    {
        string domain;

        public AssetLocationJsonParser(string domain)
        {
            this.domain = domain;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AssetLocation);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string)
            {
                AssetLocation location = new AssetLocation(reader.Value as string);
                if (!location.HasDomain())
                {
                    location.Domain = domain;
                }
                return location;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
