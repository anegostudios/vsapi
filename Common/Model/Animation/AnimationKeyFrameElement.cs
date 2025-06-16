using Newtonsoft.Json;

#nullable disable

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AnimationKeyFrameElement
    {
        [JsonProperty]
        public double? OffsetX = null;
        [JsonProperty]
        public double? OffsetY = null;
        [JsonProperty]
        public double? OffsetZ = null;

        [JsonProperty]
        public double? StretchX = null;
        [JsonProperty]
        public double? StretchY = null;
        [JsonProperty]
        public double? StretchZ = null;
        [JsonProperty]
        public double? RotationX = null;
        [JsonProperty]
        public double? RotationY = null;
        [JsonProperty]
        public double? RotationZ = null;
        [JsonProperty]
        public double? OriginX = null;
        [JsonProperty]
        public double? OriginY = null;
        [JsonProperty]
        public double? OriginZ = null;
        [JsonProperty]
        public bool RotShortestDistanceX = false;
        [JsonProperty]
        public bool RotShortestDistanceY = false;
        [JsonProperty]
        public bool RotShortestDistanceZ = false;

        internal int Frame;
        public ShapeElement ForElement;


        public bool AnySet
        {
            get { return PositionSet || StretchSet || RotationSet || OriginSet; }
        }

        public bool PositionSet
        {
            get { return OffsetX != null || OffsetY != null || OffsetZ != null; }
        }
        public bool StretchSet
        {
            get { return StretchX != null || StretchY != null || StretchZ != null; }
        }
        public bool RotationSet
        {
            get { return RotationX != null || RotationY != null || RotationZ != null; }
        }
        public bool OriginSet
        {
            get { return OriginX != null || OriginY != null || OriginZ != null; }
        }

        internal bool IsSet(int flag)
        {
            switch (flag)
            {
                case 0: return PositionSet;
                case 1: return RotationSet;
                case 2: return StretchSet;
                case 3: return OriginSet;
            }

            return false;
        }



        
    }
}
