using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Datastructures
{
    public class CachedCuboidList : IEnumerable<Cuboidd>
    {
        public List<Cuboidd> cuboids = new List<Cuboidd>(10);
        public List<BlockPos> positions = new List<BlockPos>(10);
        public List<Block> blocks = new List<Block>(10);
        public int Count;

        public CachedCuboidList()
        {
            
        }

        public void Clear()
        {
            Count = 0;
        }


        public void Add(Cuboidf cuboid, BlockPos offset, Block block = null)
        {
            if (cuboid == null) return;

            if (Count >= cuboids.Count)
            {
                cuboids.Add(cuboid.OffsetCopyDouble(offset));
                positions.Add(new BlockPos(offset.X, offset.Y, offset.Z));
                blocks.Add(block);
            }
            else
            {
                cuboids[Count].Set(
                    cuboid.X1 + offset.X,
                    cuboid.Y1 + offset.Y,
                    cuboid.Z1 + offset.Z,
                    cuboid.X2 + offset.X,
                    cuboid.Y2 + offset.Y,
                    cuboid.Z2 + offset.Z
                );
                positions[Count].Set(offset.X, offset.Y, offset.Z);
                blocks[Count] = block;
            }

            Count++;
        }

        public IEnumerator<Cuboidd> GetEnumerator()
        {
            return cuboids.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cuboids.GetEnumerator();
        }
    }
}
