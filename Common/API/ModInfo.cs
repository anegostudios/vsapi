using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

#nullable disable

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
    public class ModInfo : IComparable<ModInfo>
    {
        private IReadOnlyList<string> _authors = Array.Empty<string>();

        
        /// <summary> The type of this mod. Can be "Theme", "Content" or "Code". </summary>
        [JsonRequired]
        public EnumModType Type;

        /// <summary>
        /// If the mod is a texture pack that changes topsoil grass textures, define the texture size here
        /// </summary>
        [JsonProperty]
        public int TextureSize = 32;

        /// <summary> The name of this mod. For example "My Example Mod". </summary>
        [JsonRequired]
        public string Name;

        /// <summary>
        /// The mod id (domain) of this mod. For example "myexamplemod".
        /// (Optional. Uses mod name (converted to lowercase, stripped of
        /// whitespace and special characters) if missing.)
        /// </summary>
        [JsonProperty]
        public string ModID { get; set; }

        /// <summary> The version of this mod. For example "2.10.4". (optional) </summary>
        [JsonProperty]
        public string Version = "";

        /// <summary>
        /// The network version of this mod. Change this number when a user that has an older version of your mod should not be allowed to connected to server with a newer version. If not set, the version value is used.
        /// </summary>
        [JsonProperty]
        public string NetworkVersion = null;

        /// <summary>
        /// The path relative to the mod root to load the icon from.
        /// If this is not set, the game will also try to load "./modicon.png" before giving up.
        /// </summary>
        [JsonProperty]
        public string IconPath = null;

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
        public IReadOnlyList<string> Contributors { get; set; }
            = new List<string>().AsReadOnly();


        /// <summary>
        /// Which side(s) this mod runs on. Can be "Server", "Client" or "Universal".
        /// (Optional. Universal (both server and client) by default.)
        /// </summary>
        [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
        public EnumAppSide Side { get; set; } = EnumAppSide.Universal;

        /// <summary>
        /// If set to false and the mod is universal, clients don't need the mod
        /// to join. (Optional. True by default.)
        /// </summary>
        [JsonProperty]
        public bool RequiredOnClient { get; set; } = true;

        /// <summary>
        /// If set to false and the mod is universal, the mod is not disabled
        /// if it's not present on the server. (Optional. True by default.)
        /// </summary>
        [JsonProperty]
        public bool RequiredOnServer { get; set; } = true;

        /// <summary> List of mods (and versions) this mod depends on. </summary>
        [JsonProperty, JsonConverter(typeof(DependenciesConverter))]
        public IReadOnlyList<ModDependency> Dependencies { get; set; }
            = new List<ModDependency>().AsReadOnly();

        /// <summary> Not exposed as a JsonProperty, only coded mods can set this to true </summary>
        public bool CoreMod = false;

        // Parameterless constructor is needed for JSON conversion.
        public ModInfo() {  }

        public ModInfo(EnumModType type, string name, string modID, string version,
                         string description, IEnumerable<string> authors, IEnumerable<string> contributors, string website,
                         EnumAppSide side, bool requiredOnClient, bool requiredOnServer,
                         IEnumerable<ModDependency> dependencies)
        {
            Type = type;
            Name    = name ?? throw new ArgumentNullException(nameof(name));
            ModID   = modID ?? throw new ArgumentNullException(nameof(modID));
            Version = version ?? "";

            Description  = description ?? "";
            Authors      = ReadOnlyCopy(authors);
            Contributors = ReadOnlyCopy(contributors);
            Website      = website ?? "";

            Side             = side;
            RequiredOnClient = requiredOnClient;
            RequiredOnServer = requiredOnServer;
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


        public void Init()
        {
            if (NetworkVersion == null)
            {
                NetworkVersion = Version;
            }
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

        public int CompareTo(ModInfo other)
        {
            int r = ModID.CompareOrdinal(other.ModID);
            if (r != 0) return r;

            if (GameVersion.IsNewerVersionThan(Version, other.Version)) return -1;
            if (GameVersion.IsLowerVersionThan(Version, other.Version)) return 1;
            return 0;
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
                return listType.GetMethod("AsReadOnly").Invoke(list, Array.Empty<object>());
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
