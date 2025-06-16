using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Same as block behavior but allows even more control over a block, such as the blocks collision boxes and world gen behavior. These are not part of default block behaviors for performance reasons. Requires the block to use the GenericBlock block class (or inherit from it)
    /// </summary>
    public abstract class StrongBlockBehavior : BlockBehavior
    {
        protected StrongBlockBehavior(Block block) : base(block)
        {
        }

        public virtual void GetDecal(IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
        }

        public virtual Cuboidf[] GetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return null;
        }

        public virtual Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return null;
        }

        public virtual Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return null;
        }

        public virtual Cuboidf GetParticleBreakBox(IBlockAccessor blockAccess, BlockPos pos, BlockFacing facing, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return null;
        }

        public virtual void OnDecalTesselation(IWorldAccessor world, MeshData decalMesh, BlockPos pos, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
        }

        public virtual bool TryPlaceBlockForWorldGen(IBlockAccessor blockAccessor, BlockPos pos, BlockFacing onBlockFace, IRandom worldgenRandom, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return false;
        }


        public virtual bool DoParticalSelection(IWorldAccessor world, BlockPos pos, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return false;
        }
    }
}
