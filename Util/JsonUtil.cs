using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Common
{
    public static class JsonUtil
    {
        public static void Populate<T>(this JToken value, T target) where T : class
        {
            using (var sr = value.CreateReader())
            {
                JsonSerializer.CreateDefault().Populate(sr, target); // Uses the system default JsonSerializerSettings
            }
        }

        /// <summary>
        /// Reads a Json object, and converts it to the designated type.
        /// </summary>
        /// <typeparam name="T">The designated type</typeparam>
        /// <param name="data">The json object.</param>
        /// <returns></returns>
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

        public static T FromString<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// Converts the object to json.
        /// </summary>
        /// <typeparam name="T">The type to convert</typeparam>
        /// <param name="obj">The object to convert</param>
        /// <returns></returns>
        public static byte[] ToBytes<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static string ToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string ToPrettyString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }


        public static void PopulateObject(object toPopulate, string text, string domain, JsonSerializerSettings settings = null)
        {
            if (domain != GlobalConstants.DefaultDomain)
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }
            JsonConvert.PopulateObject(text, toPopulate, settings);
        }

        public static JsonSerializer CreateSerializerForDomain(string domain, JsonSerializerSettings settings = null)
        {
            if (domain != GlobalConstants.DefaultDomain)
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }
            return JsonSerializer.CreateDefault(settings);
        }

        public static void PopulateObject(object toPopulate, JToken token, JsonSerializer js)
        {
            using (JsonReader reader = token.CreateReader())
            {
                js.Populate(reader, toPopulate);
            }
        }

        /// <summary>
        /// Converts a Json object to a typed object.
        /// </summary>
        /// <typeparam name="T">The type to convert.</typeparam>
        /// <param name="text">The text to deserialize</param>
        /// <param name="domain">The domain of the text.</param>
        /// <param name="settings">The settings of the deserializer. (default: Null)</param>
        /// <returns></returns>
        public static T ToObject<T>(string text, string domain, JsonSerializerSettings settings = null)
        {
            if (domain != GlobalConstants.DefaultDomain)
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }   
            
            return JsonConvert.DeserializeObject<T>(text, settings);
        }

        /// <summary>
        /// Converts a Json token to a typed object.
        /// </summary>
        /// <typeparam name="T">The type to convert.</typeparam>
        /// <param name="token">The token to deserialize</param>
        /// <param name="domain">The domain of the text.</param>
        /// <param name="settings">The settings of the deserializer. (default: Null)</param>
        /// <returns></returns>
        public static T ToObject<T>(this JToken token, string domain, JsonSerializerSettings settings = null)
        {
            if (domain != GlobalConstants.DefaultDomain)
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                settings.Converters.Add(new AssetLocationJsonParser(domain));
            }

            return token.ToObject<T>(JsonSerializer.Create(settings));
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
                return AssetLocation.Create(reader.Value as string, domain);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
