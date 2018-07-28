using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Describes the type of a mod. Allows easy recognition and limiting of
    /// what any particular mod can do.
    /// </summary>
    public enum EnumModType
    {
        /// <summary>
        /// Makes only theme changes (texture, shape, sound, music) to existing
        /// game or mod assets / content without adding new content or code.
        /// </summary>
        Theme,
        /// <summary>
        /// Can modify any existing assets, or add new content, but no code.
        /// </summary>
        Content,
        /// <summary>
        /// Can modify existing assets, add new content and make use of C#
        /// source files (.cs) and pre-compiled assemblies (.dll).
        /// </summary>
        Code
    }

    /// <summary>
    /// Meta data for a specific mod folder, archive, source file or assembly.
    /// Either loaded from a "modinfo.json" or from the assembly's
    /// <see cref="T:Vintagestory.API.Common.ModInfoAttribute"/>.
    /// </summary>
    public class ModInfo
    {
        private IReadOnlyList<string> _authors = new string[0];


        /// <summary> The type of this mod. Can be "Theme", "Content" or "Code". </summary>
        [JsonRequired]
        public EnumModType Type;

        /// <summary> The name of this mod. For example "My Example Mod". </summary>
        [JsonRequired]
        public string Name;

        /// <summary>
        /// The mod id (domain) of this mod. For example "myexamplemod".
        /// (Optional. Uses mod name (converted to lowercase, stripped of
        /// whitespace and special characters) if missing.)
        /// </summary>
        [JsonProperty]
        public string ModID { get; internal set; }

        /// <summary> The version of this mod. For example "2.10.4". (optional) </summary>
        [JsonProperty]
        public string Version = "";


        /// <summary> A short description of what this mod does. (optional) </summary>
        [JsonProperty]
        public string Description = "";

        /// <summary> Location of the website or project site of this mod. (optional) </summary>
        [JsonProperty]
        public string Website = "";

        /// <summary> Names of people working on this mod. (optional) </summary>
        [JsonProperty]
        public IReadOnlyList<string> Authors {
            get { return _authors; }
            set {
                var authors = value ?? Enumerable.Empty<string>();
                _authors = authors.ToList().AsReadOnly();
            }
        }

        /// <summary> Names of people contributing to this mod. (optional) </summary>
        [JsonProperty]
        public IReadOnlyList<string> Contributors { get; private set; }
            = new List<string>().AsReadOnly();


        /// <summary>
        /// Which side(s) this mod runs on. Can be "Server", "Client" or "Universal".
        /// (Optional. Universal (both server and client) by default.)
        /// </summary>
        [JsonProperty]
        public EnumAppSide Side { get; private set; } = EnumAppSide.Universal;

        /// <summary>
        /// If set to false and the mod is universal, clients don't need the mod
        /// to join. (Optional. True by default.)
        /// </summary>
        [JsonProperty]
        public bool RequiredOnClient { get; private set; } = true;

        /// <summary> List of mods (and versions) this mod depends on. </summary>
        [JsonProperty, JsonConverter(typeof(DependenciesConverter))]
        public IReadOnlyList<ModDependency> Dependencies { get; private set; }
            = new List<ModDependency>().AsReadOnly();


        // Parameterless constructor is needed for JSON conversion.
        private ModInfo() {  }

        internal ModInfo(EnumModType type, string name, string modID, string version,
                         string description, IEnumerable<string> authors, IEnumerable<string> contributors, string website,
                         EnumAppSide side, bool requiredOnClient, IEnumerable<ModDependency> dependencies)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (modID == null) throw new ArgumentNullException(nameof(modID));

            Type    = type;
            Name    = name;
            ModID   = modID;
            Version = version ?? "";

            Description  = description ?? "";
            Authors      = ReadOnlyCopy(authors);
            Contributors = ReadOnlyCopy(contributors);
            Website      = website ?? "";

            Side             = side;
            RequiredOnClient = requiredOnClient;
            Dependencies     = ReadOnlyCopy(dependencies);

            // Null-safe helper method which copies the specified elements into a read-only list.
            IReadOnlyList<T> ReadOnlyCopy<T>(IEnumerable<T> elements)
                => (elements ?? Enumerable.Empty<T>()).ToList().AsReadOnly();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            // Automatically generate ID from name if no ID was specified.
            ModID = ModID ?? ToModID(Name);
        }


        /// <summary>
        /// Attempts to convert the specified mod name to a mod ID, stripping any
        /// non-alphanumerical (including spaces and dashes) and lowercasing letters.
        /// </summary>
        public static string ToModID(string name)
        {
            if (name == null) return null;
            var sb = new StringBuilder(name.Length);
            for (var i = 0; i < name.Length; i++)
            {
                var chr = name[i];
                var isLetter = ((chr >= 'a') && (chr <= 'z')) || ((chr >= 'A') && (chr <= 'Z'));
                var isDigit  = ((chr >= '0') && (chr <= '9'));

                if (isLetter || isDigit)
                {
                    sb.Append(char.ToLower(chr));
                }
                // Otherwise, drop the character.

                if (isDigit && (i == 0)) throw new ArgumentException(
                    $"Can't convert '{ name }' to a mod ID automatically, because " +
                    "it starts with a number, which is illegal", nameof(name));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns whether the specified domain is valid.
        ///
        /// Tests if the string is non-null, has a length of at least 1, starts with
        /// a basic lowercase letter and contains only lowercase letters and numbers.
        /// </summary>
        public static bool IsValidModID(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            for (var i = 0; i < str.Length; i++)
            {
                var chr = str[i];
                var isLetter = (chr >= 'a') && (chr <= 'z');
                var isDigit  = (chr >= '0') && (chr <= '9');
                if (isLetter || (isDigit && (i != 0))) continue;
                return false;
            }
            return true;
        }


        private class DependenciesConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(IEnumerable<ModDependency>).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return JObject.Load(reader).Properties()
                              .Select(prop => new ModDependency(prop.Name, (string)prop.Value))
                              .ToList().AsReadOnly();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                foreach (var dependency in (IEnumerable<ModDependency>)value)
                {
                    writer.WritePropertyName(dependency.ModID);
                    writer.WriteValue(dependency.Version);
                }
                writer.WriteEndObject();
            }
        }

        private class ReadOnlyListConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var elementType = objectType.GetGenericArguments()[0];
                var elements    = JArray.Load(reader).Select(e => e.ToObject(elementType));
                var listType    = typeof(List<>).MakeGenericType(elementType);
                var list        = (IList)Activator.CreateInstance(listType);
                foreach (var element in elements) list.Add(element);
                return listType.GetMethod("AsReadOnly").Invoke(list, new object[0]);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var elementType = value.GetType().GetGenericArguments()[0];
                writer.WriteStartArray();
                foreach (var element in (IEnumerable)value)
                {
                    serializer.Serialize(writer, element, elementType);
                }
                writer.WriteEndArray();
            }
        }
    }
}
