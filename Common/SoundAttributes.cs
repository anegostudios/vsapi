using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Sound types, often used to determine specific volume controls.
    /// </summary>
    public enum EnumSoundType
    {
        Sound, 
        Music, 
        Ambient,
        Weather,
        Entity,
        MusicGlitchunaffected,
        AmbientGlitchunaffected,
        SoundGlitchunaffected
    }

    [DocumentAsJson]
    public struct SoundAttributes
    {
        [JsonProperty("path")]
        public AssetLocation? Location;
        public NatFloat Pitch = NatFloatOne;
        public NatFloat Volume = NatFloatOne;
        [JsonProperty("soundType")]
        public EnumSoundType Type = EnumSoundType.Sound;

        /// The range at which the gain will be attenuated to 1% of the supplied volume
        public float Range = 32f;

        public static NatFloat NatFloatOne = NatFloat.One; // Cache for reuse
        public static NatFloat RandomPitch = new NatFloat(1.0f, 0.25f, EnumDistribution.UNIFORM);

        public SoundAttributes() { }

        // The input sound path must be provided without the '.ogg' suffix, and must before use have the sounds/ or other assetcategory prefix.
        public SoundAttributes(AssetLocation assetLocation, bool withRandomPitch)
        {
            Location = assetLocation;
            if (withRandomPitch) this.Pitch = RandomPitch;
        }

        public bool Equals(SoundAttributes other)
        {
            return other.Location == Location 
                && other.Pitch == Pitch 
                && other.Volume == Volume 
                && other.Range == Range 
                && other.Type == Type;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SoundAttributes other) return Equals(other);

            return false;
        }

        public static bool operator ==(SoundAttributes? left, SoundAttributes? right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(SoundAttributes? left, SoundAttributes? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (Location?.GetHashCode() ?? 0) ^ Pitch.GetHashCode() ^ Volume.GetHashCode() ^ Range.GetHashCode() ^ Type.GetHashCode();
        }

        public SoundAttributes Clone()
        {
            SoundAttributes sound = new SoundAttributes();
            sound.Location = Location?.PermanentClone();
            sound.Pitch = Pitch.Clone();
            sound.Volume = Volume.Clone();
            sound.Type = Type;
            sound.Range = Range;
            return sound;
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Location != null);
            if (Location != null) writer.Write(Location.ToShortString());
            writer.Write((int)Type);
            writer.Write(Range);
            Pitch.ToBytes(writer);
            Volume.ToBytes(writer);
        }

        public static SoundAttributes FromBytes(BinaryReader reader, string version)
        {
            return new SoundAttributes()
            {
                Location = reader.ReadBoolean() ? AssetLocation.Create(reader.ReadString()) : null,
                Type = (EnumSoundType)reader.ReadInt32(),
                Range = reader.ReadSingle(),
                Pitch = NatFloat.createFromBytes(reader),
                Volume = NatFloat.createFromBytes(reader)
            };
        }

        public override string ToString() {
            return $"SoundAttributes {{ Location: {Location}, Pitch: {Pitch}, Volume: {Volume}, Range: {Range}, Type: {Type} }}";
        }
    }

    public class SoundAttributeConverter : JsonConverter<SoundAttributes?>
    {
        public float DefaultRange = 32f;
        public bool AddSoundsPrefix = true;
        public bool RandomPitch = false;

        public SoundAttributeConverter(bool randomPitch)
        {
            RandomPitch = randomPitch;
        }

        public SoundAttributeConverter(bool randomPitch, float defaultRange)
        {
            RandomPitch = randomPitch;
            DefaultRange = defaultRange;
        }

        public override bool CanWrite { get => false; }

        public override void WriteJson(JsonWriter writer, SoundAttributes? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override SoundAttributes? ReadJson(JsonReader reader, Type objectType, SoundAttributes? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(string))
            {
                AssetLocationJsonParser converter = serializer.Converters.Select(x => x as AssetLocationJsonParser).Where(x => x != null).FirstOrDefault() 
                    ?? new AssetLocationJsonParser(GlobalConstants.DefaultDomain);
                AssetLocation? location = converter.ReadJson(reader, typeof(AssetLocation), null, false, serializer);
                ArgumentNullException.ThrowIfNull(location); // This should never happen, no matter how the json is set up
                if (AddSoundsPrefix) location.WithPathPrefixOnce("sounds/");

                return new SoundAttributes(location, RandomPitch) { Range = DefaultRange, };
            }

            SoundAttributes sound = new SoundAttributes();
            sound.Range = DefaultRange;

            // Set up so json serializer will create new instances, not populate the existing ones
            sound.Pitch = null!;
            sound.Volume = null!;

            try
            {
                object boxed = sound;
                serializer.Populate(reader, boxed);
                sound = (SoundAttributes)boxed;
                if (AddSoundsPrefix) sound.Location?.WithPathPrefixOnce("sounds/");
            }
            catch (Exception e)
            {
                throw new Exception("Error while parsing " + sound, e);
            }

            sound.Pitch ??= RandomPitch ? SoundAttributes.RandomPitch : SoundAttributes.NatFloatOne;
            sound.Volume ??= SoundAttributes.NatFloatOne;

            return sound.Location == null ? null : sound;
        }
    }
}
