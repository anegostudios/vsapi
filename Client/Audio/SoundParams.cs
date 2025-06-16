using ProtoBuf;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
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

    /// <summary>
    /// The sound paramaters used for loading sounds on the client side
    /// </summary>
    [ProtoContract]
    public class SoundParams
    {
        /// <summary>
        /// The specific sound to be played
        /// </summary>
        [ProtoMember(1)]
        public AssetLocation Location;

        /// <summary>
        /// The position of the sound
        /// </summary>
        [ProtoMember(2)]
        public Vec3f Position;

        /// <summary>
        /// If true then Position is added relative to the players current position
        /// </summary>
        [ProtoMember(3)]
        public bool RelativePosition;

        /// <summary>
        /// If the sound should start again when finished
        /// </summary>
        [ProtoMember(4)]
        public bool ShouldLoop;

        /// <summary>
        /// Probably want to set this to false on looping sounds. But remember to dispose it yourself when you no longer need it
        /// </summary>
        [ProtoMember(5)]
        public bool DisposeOnFinish = true;

        /// <summary>
        /// The sounds intial pitch. 
        /// </summary>
        [ProtoMember(6)]
        public float Pitch = 1f;

        /// <summary>
        /// 0...1
        /// </summary>
        [ProtoMember(7)]
        public float LowPassFilter = 1f;

        /// <summary>
        /// 0...99f
        /// </summary>
        [ProtoMember(8)]
        public float ReverbDecayTime = 0f;

        /// <summary>
        /// The range in which the sound does not attenuate at all
        /// </summary>
        [ProtoMember(9)]
        public float ReferenceDistance = 3f;

        /// <summary>
        /// The sounds initial volumne (0f - 1f)
        /// </summary>
        public float Volume
        {
            get { return volume; }
            set
            {
                if (value > 1f)
                    volume = 1f;
                else if (value < 0f)
                    volume = 0f;
                else
                    volume = value;
            }
        }

        [ProtoMember(10)]
        float volume = 1f;

        /// <summary>
        /// The sounds initial range (default is 32)
        /// </summary>
        [ProtoMember(11)]
        public float Range = 32;

        /// <summary>
        /// Determines whether to apply the music or sound volumne level to the Volume
        /// </summary>
        [ProtoMember(12)]
        public EnumSoundType SoundType = EnumSoundType.Sound;

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public SoundParams()
        {

        }

        /// <summary>
        /// Constructs the sound based off the asset location.
        /// </summary>
        /// <param name="location">The asset location of the track.</param>
        public SoundParams(AssetLocation location)
        {
            this.Location = location;
            Position = new Vec3f();
            ShouldLoop = false;
            RelativePosition = false;
        }
    }

}
