using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A sound that should play alongside an animation.
    /// </summary>
    [DocumentAsJson]
    public class AnimationSound
    {
        /// <summary>
        /// The sound will play when this frame is reached. Should be set if <see cref="Looping"/> is false.
        /// </summary>
        [DocumentAsJson("Recommended", "0")]
        public int Frame;

        [JsonIgnore]
        public SoundAttributes Attributes = new SoundAttributes() { Type = EnumSoundType.Entity };

        /// <summary>
        /// The chance to play this sound.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        public float Chance = 1;

        /// <summary>
        /// Should this sound loop whilst the animation is playing? 
        /// </summary>
        [DocumentAsJson("Optional", "False")]
        public bool Looping = false;

        // For reading from json without needing a separate json object for the SoundAttributes
        /// <summary>
        /// The location of the sound file.
        /// </summary>
        [DocumentAsJson("Required")]
        [JsonProperty]
        private AssetLocation Location { set => Attributes.Location = value; }

        /// <summary>
        /// Alternative form for <see cref="Location"/>.
        /// </summary>
        [DocumentAsJson("Optional")]
        [JsonProperty]
        private AssetLocation Path { set => Attributes.Location = value; }

        /// <summary>
        /// Controls a random pitch for the sound.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        [JsonProperty]
        private NatFloat Pitch { set => Attributes.Pitch = value; }

        /// <summary>
        /// Controls a random volume for the sound.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        [JsonProperty]
        private NatFloat Volume { set => Attributes.Volume = value; }

        /// <summary>
        /// At this many meters away, the sound will be 1% of the volume.
        /// </summary>
        [DocumentAsJson("Optional", "32")]
        [JsonProperty]
        private float Range { set => Attributes.Range = value; }

        /// <summary>
        /// If true, sets the pitch to be randomized on sound play between 0.75 and 1.25. If false, the pitch will be 1. If not set, then the pitch is provided by <see cref="Pitch"/>.
        /// </summary>
        [DocumentAsJson("Optional", "False")]
        [JsonProperty]
        private bool RandomizePitch { set => Attributes.Pitch = value ? SoundAttributes.RandomPitch : SoundAttributes.NatFloatOne; }


        public AnimationSound Clone()
        {
            return new AnimationSound()
            {
                Frame = Frame,
                Attributes = Attributes.Clone(),
                Chance = Chance,
                Looping = Looping
            };
        }

        public void ToBytes(BinaryWriter writer)
        {
            Attributes.ToBytes(writer);
            writer.Write(Chance);
            writer.Write(Looping);
            writer.Write(Frame);
        }

        public static AnimationSound CreateFromBytes(BinaryReader reader, string version)
        {
            if (GameVersion.IsAtLeastVersion(version, "1.22.0-dev.5"))
            {
                return new AnimationSound()
                {
                    Attributes = SoundAttributes.FromBytes(reader, version),
                    Chance = reader.ReadSingle(),
                    Looping = reader.ReadBoolean(),
                    Frame = reader.ReadInt32(),
                };
            }
            else if (GameVersion.IsAtLeastVersion(version, "1.22.0-dev.2"))
            {
                AnimationSound sound = new AnimationSound();
                sound.Location = AssetLocation.Create(reader.ReadString());
                sound.Range = reader.ReadSingle();
                bool randomizePitch = reader.ReadBoolean();
                sound.Pitch = randomizePitch ? SoundAttributes.RandomPitch : SoundAttributes.NatFloatOne;
                sound.Looping = GameVersion.IsAtLeastVersion(version, "1.22.0-dev.3") ? reader.ReadBoolean() : false;
                return sound;
            }
            else
            {
                return new AnimationSound()
                {
                    Attributes = new SoundAttributes 
                    {
                        Location = AssetLocation.Create(reader.ReadString()),
                        Range = reader.ReadSingle()
                    },
                    Frame = reader.ReadInt32(),
                    Pitch = reader.ReadBoolean() ? SoundAttributes.RandomPitch : SoundAttributes.NatFloatOne
                };
            }
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            // Needed because we don't have a SoundAttributeConverter set up for the parsing
            Attributes.Location?.WithPathPrefixOnce("sounds/");
        }
    }
}
