using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class CachedCuboidList : IEnumerable<Cuboidd>
    {
        public Cuboidd[] cuboids = System.Array.Empty<Cuboidd>();   //initialised with size 0 because cuboids.Length test and the enumerator may use this
        public BlockPos[] positions;
        public Block[] blocks;
        public int Count;
        private int populatedSize = 0;

        public CachedCuboidList()
        {
            
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Add(Cuboidf[] cuboids, int x, int y, int z, Block block = null)
        {
            for (int i = 0; i < cuboids.Length; i++)
            {
                Add(cuboids[i], x, y, z, block);
            }
        }

        public void Add(Cuboidf cuboid, int x, int y, int z, Block block = null)
        {
            if (cuboid == null) return;

            if (Count >= populatedSize)
            {
                if (Count >= cuboids.Length) ExpandArrays();
                cuboids[Count] = cuboid.OffsetCopyDouble(x, y % BlockPos.DimensionBoundary, z);
                positions[Count] = new BlockPos(x, y, z);
                blocks[Count] = block;
                populatedSize++;
            }
            else
            {
                cuboids[Count].Set(
                    cuboid.X1 + x,
                    cuboid.Y1 + y % BlockPos.DimensionBoundary,
                    cuboid.Z1 + z,
                    cuboid.X2 + x,
                    cuboid.Y2 + y % BlockPos.DimensionBoundary,
                    cuboid.Z2 + z
                );
                positions[Count].Set(x, y, z);
                blocks[Count] = block;
            }

            Count++;
        }

        public IEnumerator<Cuboidd> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return cuboids[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ExpandArrays()
        {
            int newSize = populatedSize == 0 ? 16 : populatedSize * 3 / 2;
            Cuboidd[] newCuboids = new Cuboidd[newSize];
            BlockPos[] newPositions = new BlockPos[newSize];
            Block[] newBlocks = new Block[newSize];
            for (int i = 0; i < populatedSize; i++)
            {
                newCuboids[i] = cuboids[i];
                newPositions[i] = positions[i];
                newBlocks[i] = blocks[i];
            }
            cuboids = newCuboids;
            positions = newPositions;
            blocks = newBlocks;
        }
    }
}
