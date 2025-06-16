using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Contains all the information for a players block selection event
    /// </summary>
    public class BlockSelection
    {
        /// <summary>
        /// The position the player wants to place/break something at
        /// </summary>
        public BlockPos Position;

        /// <summary>
        /// The face the player aimed at
        /// </summary>
        public BlockFacing Face;

        /// <summary>
        /// The coordinate of the exact aimed position, relative to the Block Position
        /// </summary>
        public Vec3d HitPosition;

        /// <summary>
        /// Which selection box was aimed at. The index corresponds to the array returned by Block.GetSelectionBoxes()
        /// </summary>
        public int SelectionBoxIndex;

        /// <summary>
        /// Always false during block use. True during placement if the Position value was offseted. Example:
        /// - When trying to place planks while aiming at rock, the Position is the one in front of the Rock and DidOffset is True
        /// - When trying to place planks while aiming at tallgrass, the Position is where the tall grass is and DidOffset is false (because tallgrass is replacable)
        /// </summary>
        public bool DidOffset;

        /// <summary>
        /// The block actually being looked at!
        /// </summary>
        public Block Block;

        public BlockSelection()
        {
        }

        public Vec3d FullPosition => new Vec3d(Position.X + HitPosition.X, Position.InternalY + HitPosition.Y, Position.Z + HitPosition.Z);

        /// <summary>
        /// Creates a basic BlockSelection from limited data
        /// </summary>
        public BlockSelection(BlockPos pos, BlockFacing face, Block block)
        {
            Position = pos;
            Face = face;
            HitPosition = new Vec3d(1, 1, 1).Offset(face.Normald).Scale(0.5);
            Block = block;
        }

        public BlockSelection SetPos(int x, int y, int z)
        {
            Position.Set(x, y, z);
            return this;
        }

        public BlockSelection AddPosCopy(int x, int y, int z)
        {
            var cloned = Clone();
            cloned.Position.Add(x, y, z);
            return cloned;
        }
        public BlockSelection AddPosCopy(Vec3i vec)
        {
            var cloned = Clone();
            cloned.Position.Add(vec);
            return cloned;
        }


        /// <summary>
        /// Creates a deep copy 
        /// </summary>
        /// <returns></returns>
        public BlockSelection Clone()
        {
            return new BlockSelection()
            {
                Face = this.Face,
                HitPosition = this.HitPosition?.Clone(),
                SelectionBoxIndex = this.SelectionBoxIndex,
                Position = this.Position?.Copy(),
                DidOffset = DidOffset
            };
        }

        /// <summary>
        /// Returns a subposition index for use addressing decor subpositions on a block
        /// </summary>
        /// <returns></returns>
        public int ToDecorIndex()
        {
            int x = (int)(HitPosition.X * 16);
            int y = 15 - (int)(HitPosition.Y * 16);
            int z = (int)(HitPosition.Z * 16);
            return new DecorBits(Face, x, y, z);
        }

        [System.Obsolete("Use (int)new DecorBits(face, x, y, z) instead, which has the same functionality")]
        public static int GetDecorIndex(BlockFacing face, int x, int y, int z)
        {
            return new DecorBits(face, x, y, z);
        }

        [System.Obsolete("Use (int)new DecorBits(face) instead, which has the same functionality")]
        public static int GetDecorIndex(BlockFacing face)
        {
            return new DecorBits(face);
        }
    }

}
