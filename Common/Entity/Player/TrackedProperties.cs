using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common.Entities
{
    public class TrackedPlayerProperties
    {
        public int EyesInWaterColorShift = 0;
        public int EyesInLavaColorShift = 0;
        public float EyesInLavaDepth = 0;
        public float EyesInWaterDepth = 0;

        public float DayLight = 1;

        public float DistanceToSpawnPoint;
        

        public float MoonLight = 0;
        public double FallSpeed = 0;
        public BlockPos PlayerChunkPos = new BlockPos();

        public BlockPos PlayerPosDiv8 = new BlockPos();

        /// <summary>
        /// Relative value. bottom 0...1 sealevel, 1 .... 2 max-y
        /// </summary>
        public float posY;

        /// <summary>
        /// 0...32
        /// </summary>
        public float sunSlight = 21;

        
        /// <summary>
        /// The servers playstyle
        /// </summary>
        public string Playstyle;

        public string PlayListCode;


        public TrackedPlayerProperties()
        {

        }



    }
}
