using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public enum EnumSoundType
    {
        Sound, 
        Music, 
        Ambient,
        AmbientGlitchunaffected,
        SoundGlitchunaffected
    }

    /// <summary>
    /// The sound paramaters used for loading sounds on the client side
    /// </summary>
    public class SoundParams
    {
        /// <summary>
        /// The specific sound to be played
        /// </summary>
        public AssetLocation Location;

        /// <summary>
        /// The position of the sound
        /// </summary>
        public Vec3f Position;

        /// <summary>
        /// If true then Position is added relative to the players current position
        /// </summary>
        public bool RelativePosition;

        /// <summary>
        /// If the sound should start again when finished
        /// </summary>
        public bool ShouldLoop;

        /// <summary>
        /// Probably want to set this to false on looping sounds. But remember to dispose it yourself when you no longer need it
        /// </summary>
        public bool DisposeOnFinish = true;

        /// <summary>
        /// The sounds intial pitch. 
        /// </summary>
        public float Pitch = 1f;

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
        float volume = 1f;
        
        /// <summary>
        /// The sounds initial range
        /// </summary>
        public float Range = 32;

        /// <summary>
        /// Determines whether to apply the music or sound volumne level to the Volume
        /// </summary>
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
