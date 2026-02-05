using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    [DocumentAsJson]
    public class AnimationSound
    {
        public int Frame;
        [JsonIgnore]
        public SoundAttributes Attributes = new SoundAttributes() { Type = EnumSoundType.Entity };
        public float Chance = 1;
        public bool Looping = false;

        // For reading from json without needing a separate json object for the SoundAttributes
        [JsonProperty]
        private AssetLocation Location { set => Attributes.Location = value; }
        [JsonProperty]
        private AssetLocation Path { set => Attributes.Location = value; } // Synonym of 'location'
        [JsonProperty]
        private NatFloat Pitch { set => Attributes.Pitch = value; }
        [JsonProperty]
        private NatFloat Volume { set => Attributes.Volume = value; }
        [JsonProperty]
        private float Range { set => Attributes.Range = value; }
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
