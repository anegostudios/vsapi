using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

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
    }

}
