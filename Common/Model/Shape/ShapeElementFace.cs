using System.Collections.Generic;

namespace Vintagestory.API.Common
{
    public class ShapeElementFace
    {
        /// <summary>
        /// The texture of the face.
        /// </summary>
        public string Texture;

        /// <summary>
        /// The UV array of the face.
        /// </summary>
        public float[] Uv;

        /// <summary>
        /// The rotation of the face.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The glow on the face.
        /// </summary>
        public int Glow;

        /// <summary>
        /// Whether or not the element is enabled.
        /// </summary>
        public bool Enabled = true;
    }
}