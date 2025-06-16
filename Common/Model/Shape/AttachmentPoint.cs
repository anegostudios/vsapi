using Newtonsoft.Json;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// This is a spot on the shape that connects to another shape.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AttachmentPoint
    {
        /// <summary>
        /// The parent element of this attachment point.
        /// </summary>
        public ShapeElement ParentElement;

        /// <summary>
        /// The json code of this attachment point.
        /// </summary>
        [JsonProperty]
        public string Code;

        /// <summary>
        /// The X position of the attachment point.
        /// </summary>
        [JsonProperty]
        public double PosX;

        /// <summary>
        /// The Y position of the attachment point.
        /// </summary>
        [JsonProperty]
        public double PosY;

        /// <summary>
        /// The Z position of the attachment point.
        /// </summary>
        [JsonProperty]
        public double PosZ;

        /// <summary>
        /// The forward vertical rotation of the attachment point.
        /// </summary>
        [JsonProperty]
        public double RotationX;

        /// <summary>
        /// The forward horizontal rotation of the attachment point
        /// </summary>
        [JsonProperty]
        public double RotationY;

        /// <summary>
        /// the left/right tilt of the attachment point
        /// </summary>
        [JsonProperty]
        public double RotationZ;

        public void DeDuplicate()
        {
            Code = Code.DeDuplicate();
        }
    }
}
