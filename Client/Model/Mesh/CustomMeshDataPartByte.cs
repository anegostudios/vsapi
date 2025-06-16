
#nullable disable
namespace Vintagestory.API.Client
{
    public enum DataConversion
    {
        Float,
        NormalizedFloat,
        Integer
    }

    /// <summary>
    /// Holds arbitrary byte data for meshes to be used in the shader
    /// </summary>
    public class CustomMeshDataPartByte : CustomMeshDataPart<byte>
    {
        public DataConversion Conversion = DataConversion.NormalizedFloat;

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public CustomMeshDataPartByte() : base() { }

        /// <summary>
        /// Size initialization constructor.
        /// </summary>
        /// <param name="size"></param>
        public CustomMeshDataPartByte(int size) : base(size) { }

        /// <summary>
        /// adds values to the bytes part per four bytes.
        /// </summary>
        /// <param name="fourbytes">the integer mask of four separate bytes.</param>
        public void AddBytes(int fourbytes)
        {
            if (Count + 4 >= BufferSize)
            {
                GrowBuffer();
            }

            // Direct write of an int to a byte array (safes us 4 iterations and varius masking operations)
            unsafe
            {
                fixed (byte* bytes = Values)
                {
                    int* bytesInt = (int*)bytes;
                    bytesInt[Count / 4] = fourbytes;
                }
            }

            Count += 4;
        }

        /// <summary>
        /// Creates a clone of this collection of data parts.
        /// </summary>
        /// <returns>A clone of this collection of data parts.</returns>
        public CustomMeshDataPartByte Clone()
        {
            CustomMeshDataPartByte cloned = new CustomMeshDataPartByte();
            cloned.SetFrom(this);
            cloned.Conversion = Conversion;
            return cloned;
        }

        public CustomMeshDataPartByte EmptyClone()
        {
            return EmptyClone(new CustomMeshDataPartByte()) as CustomMeshDataPartByte;
        }
    }
}
