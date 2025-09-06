
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds arbitrary short data for meshes to be used in the shader.  radfast note: actually interpreted by the shader as ushort
    /// </summary>
    public class CustomMeshDataPartShort : CustomMeshDataPart<short>
    {
        public DataConversion Conversion = DataConversion.NormalizedFloat;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public CustomMeshDataPartShort() : base() { }

        /// <summary>
        /// Size initialization constructor.
        /// </summary>
        /// <param name="size"></param>
        public CustomMeshDataPartShort(int size) : base(size) { }


        /// <summary>
        /// Creates a clone of this collection of data parts.
        /// </summary>
        /// <returns>A clone of this collection of data parts.</returns>
        public CustomMeshDataPartShort Clone()
        {
            CustomMeshDataPartShort cloned = new CustomMeshDataPartShort();
            cloned.SetFrom(this);
            return cloned;
        }

        public CustomMeshDataPartShort EmptyClone()
        {
            return EmptyClone(new CustomMeshDataPartShort()) as CustomMeshDataPartShort;
        }

        public void AddPackedUV(float u, float v, bool isU2, bool isV2)
        {
            Add((short)(ushort)((int)(u * 0x8000 + 0.5f) + (isU2 ? 1 : 0)));  // range 0 - 32768 to correspond with 0.0 - 1.0: that's enough resolution because even in the extremely unlikely event the texture atlas is larger than 16384, uvs for topsoil and other blocks using this are going to be at 32-pixel boundaries (even even pixel or 4-pixel boundaries would be enough resolution more than likely)
            Add((short)(ushort)((int)(v * 0x8000 + 0.5f) + (isV2 ? 1 : 0)));  // then we add 1 to signal to the shader if it is the u2 or v2 value
            // the (short)(ushort) prevents databit loss if the value is 0x8000 or 0x8001 (which is too large for short)
        }
    }
}
