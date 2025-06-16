using System;
using System.Runtime.CompilerServices;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// An actual instance of this struct is the 'faceAndSubposition' data.
    /// <br/>The struct also provides various static methods to convert elements to and from a PackedIndex used in WorldChunk storage
    /// </summary>
    public struct DecorBits
    {
        #region Struct and properties to represent a FaceAndSubposition (also including rotation data, from 1.20-rc.2)

        private int faceAndSubposition;      // This struct is wrapping this int value

        public static implicit operator int(DecorBits a)     // For backwards compatibility and not too much change to API, we convert this to an (int) for use elsewhere
        {
            return a.faceAndSubposition;
        }

        public DecorBits(int value)
        {
            faceAndSubposition = value;
        }

        /// <summary>
        /// Simplest case, we supply just a face  (no subposition for cave-art, and no rotation)
        /// </summary>
        /// <param name="face"></param>
        public DecorBits(BlockFacing face)
        {
            faceAndSubposition = face.Index;
        }

        /// <summary>
        /// Turn both face and local voxel position to a decor faceAndSubposition index
        /// </summary>
        /// <param name="face"></param>
        /// <param name="vx">0..15</param>
        /// <param name="vy">0..15</param>
        /// <param name="vz">0..15</param>
        public DecorBits(BlockFacing face, int vx, int vy, int vz)
        {
            int offset = 0;
            switch (face.Index)
            {
                case 0:
                    offset = (15 - vx) + vy * 16;
                    break;
                case 1:
                    offset = (15 - vz) + vy * 16;
                    break;
                case 2:
                    offset = vx + vy * 16;
                    break;
                case 3:
                    offset = vz + vy * 16;
                    break;
                case 4:
                    offset = vx + vz * 16;
                    break;
                case 5:
                    offset = vx + (15 - vz) * 16;
                    break;
            }

            faceAndSubposition = face.Index + 6 * (1 + offset);
        }



        public int Face { get { return faceAndSubposition % 6; } }

        public int SubPosition { get { return faceAndSubposition / 6 & 0xFFF; } }    // Subposition is normally in the range 0-256 but we give it 12 bits here for future-proofing

        public int Rotation                 // Rotation data is 3 additional bits at bit positions 14-12 within the (faceAndSubposition / 6)
        {            
            get {
                return faceAndSubposition / 6 >> 12;
            }
            set {
                int newSubPositionAndRotation = SubPosition + (value << 12);
                faceAndSubposition = faceAndSubposition % 6 + newSubPositionAndRotation * 6;
            }
        }

        #endregion




        #region Static methods for PackedIndex handling

        /// <summary>
        /// A bit mask to select bits 0-14, i.e. the chunk's index3d
        /// </summary>
        const int Index3dMask = 0x7FFF;
        /// <summary>
        /// A bit mask to select the three most significant bits of a byte, 0b11100000 or 0xE0
        /// </summary>
        const int mask3Bits = 0xE0;
        /// <summary>
        /// A bit mask to select the five least significant bits of a byte, 0b00011111 or 0x1F
        /// </summary>
        const int mask5Bits = 0x1F;
        /// <summary>
        /// A bit mask to select the three rotation data bits; this is also the maxvalue of the rotationData
        /// </summary>
        public const int maskRotationData = 0x7;

        /// <summary>
        ///The packedIndex works like this: [radfast 7 Dec 2024, 1.20-rc.2]
        /// <code>
        /// The packedIndex has four components:
        ///     index3d for the block's local x,y,z value within the chunk, each in the range 0-31, for 15 bits in total
        ///     faceindex for the face of the block this decor is on (corresponding to BlockFacing.Index), range 0-5
        ///     optionally, a subposition in the range 0-256, where 0 means no subposition, and values 1-256 give a subposition in the 16x16 subgrid, used for ArtPigment or similar
        ///     optionally, 3 bits of rotation data
        ///     
        /// These are packed into bits in the following way, it has to be this way for backwards compatibility reasons (assuming we do not want to add a new chunk dataversion)
        /// 31 - 24  (the five Least Significant Bits of the subposition) * 6 + faceindex
        /// 23 - 21  (the three Most Significant Bits of the subposition)
        /// 20 - 19  (unused)
        /// 18 - 16  rotation data
        /// 15       (unused)  
        /// 14 - 0   index3d
        /// 
        /// (Exceptionally, the value in bits 31-24 has the magic value of (0x20 * 6 + faceindex), and the value in bits 23-16 is 0xE0, if a subposition value of 256 is intended: this works within the existing algorithms because 0xE0 + 0x20 == 0x100 i.e. 256.   If necessary we can have values up to 0x2A there, so the range of possible subpositions is up to 266)
        /// 
        /// 0000 0000 0000 0000 0000 0000 0000 0000
        /// </code>
        ///
        /// </summary>
        public static int FaceAndSubpositionToIndex(int faceAndSubposition)
        {
            int subPosition = faceAndSubposition / 6;
            int rotationData = (subPosition >> 12) & maskRotationData;
            subPosition &= 0xFFF;   // Exclude the rotationData
            var decorFaceIndex = faceAndSubposition % 6;
            if (subPosition < 256)
            {
                faceAndSubposition = decorFaceIndex + (subPosition & mask5Bits) * 6;
                subPosition &= mask3Bits;   // Three most significant bits of 8-bit subPosition are at index bits 21-23
            }
            else
            {
                faceAndSubposition = decorFaceIndex + 0xC0;    // 0xC0 is 0x20 * 6
                subPosition = mask3Bits;    // 0x20 and 0xE0 will add together to make 256 in ChunkTesselator.BuildDecorPolygons()
            }

            return (faceAndSubposition << 24) + ((subPosition + rotationData) << 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FaceAndSubpositionFromIndex(int packedIndex)
        {
            int bits16to23 = packedIndex >> 16;
            return (packedIndex >> 24 & 0xFF) + ((bits16to23 & mask3Bits) + ((bits16to23 & maskRotationData) << 12)) * 6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FaceToIndex(BlockFacing face)
        {
            return face.Index << 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FaceFromIndex(int packedIndex)
        {
            return (packedIndex >> 24 & 0xFF) % 6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index3dFromIndex(int packedIndex)
        {
            return packedIndex & Index3dMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SubpositionFromIndex(int packedIndex)
        {
            return (packedIndex >> 24 & 0xFF) / 6 + (packedIndex >> 16 & mask3Bits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotationFromIndex(int packedIndex)
        {
            return packedIndex >> 16 & maskRotationData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockFacing FacingFromIndex(int packedIndex)
        {
            return BlockFacing.ALLFACES[DecorBits.FaceFromIndex(packedIndex)];
        }

        #endregion
    }
}

