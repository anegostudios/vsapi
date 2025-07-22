using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Allows for definitions of behaviors of a block that can be applied to any block
    /// See the derived types for a list of all documented block behaviors.
    /// </summary>
    [DocumentAsJson]
    public abstract class BlockBehavior : CollectibleBehavior
    {
        /// <summary>
        /// The block for this behavior instance.
        /// </summary>
        public Block block;


        public BlockBehavior(Block block) : base(block)
        {
            this.block = block;
        }


        /// <summary>
        /// Called when a survival player has broken the block. The default behavior removes the block and spawns the block drops.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
        /// <param name="handling"></param>
        public virtual void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }

        /// <summary>
        /// When the player has presed the middle mouse click on the block. The default behavior returns an itemstack with the block itself
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return null;
        }

        /// <summary>
        /// Is called before a block is broken, should return what items this block should drop. Return null or empty array for no drops. The default behavior drops whatever block.Drops is set to.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
        /// <param name="dropChanceMultiplier"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropChanceMultiplier, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return null;
        }

        /// <summary>
        /// Called when any of it's 6 neighbour blocks has been changed
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="neibpos"></param>
        /// <param name="handling"></param>
        public virtual void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }

        /// <summary>
        /// Used by torches and other blocks to check if it can attach itself to that block. The default behavior tests for SideSolid[blockFace.Index]
        /// </summary>
        /// <param name="world"></param>
        /// <param name="block"></param>
        /// <param name="pos"></param>
        /// <param name="blockFace"></param>
        /// <param name="handling"></param>
        /// <param name="attachmentArea"></param>
        /// <returns></returns>
        public virtual bool CanAttachBlockAt(IBlockAccessor world, Block block, BlockPos pos, BlockFacing blockFace, ref EnumHandling handling, Cuboidi attachmentArea = null)
        {
            handling = EnumHandling.PassThrough;

            return false;
        }


        /// <summary>
        /// Should return if supplied entitytype is allowed to spawn on this block
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="sc"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool CanCreatureSpawnOn(IBlockAccessor blockAccessor, BlockPos pos, EntityProperties type, BaseSpawnConditions sc, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return false;
        }


        /// <summary>
        /// For any block that can be rotated, this method should be implemented to return the correct rotated block code. It is used by the world edit tool for allowing block data rotations
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual AssetLocation GetRotatedBlockCode(int angle, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return null;
        }

        /// <summary>
        /// For any block that can be flipped upside down, this method should be implemented to return the correctly flipped block code. It is used by the world edit tool for allowing block data rotations
        /// </summary>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual AssetLocation GetVerticallyFlippedBlockCode(ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return null;
        }

        /// <summary>
        /// For any block that can be flipped vertically, this method should be implemented to return the correctly flipped block code. It is used by the world edit tool for allowing block data rotations
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual AssetLocation GetHorizontallyFlippedBlockCode(EnumAxis axis, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return null;
        }

        /// <summary>
        /// Used to determine if a block should be treated like air when placing blocks. (e.g. used for tallgrass)
        /// </summary>
        /// <param name="block"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool IsReplacableBy(Block block, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return false;
        }

        /// <summary>
        /// Everytime the player moves by 8 blocks (or rather leaves the current 8-grid), a scan of all blocks 32x32x32 blocks around the player is initiated
        /// and this method is called. If the method returns true, the block is registered to a client side game ticking for spawning particles and such.
        /// This method will be called everytime the player left his current 8-grid area. 
        /// 
        /// The default behavior is to return true if block.ParticleProperties are set
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="pos"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool ShouldReceiveClientParticleTicks(IWorldAccessor world, IPlayer byPlayer, BlockPos pos, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return false;
        }


        public virtual void OnAsyncClientParticleTick(IAsyncParticleManager manager, BlockPos pos, float windAffectednessAtPos, float secondsTicking)
        {

        }   

        /// <summary>
        /// Always called when a block has been removed through whatever method, except during worldgen or via ExchangeBlock()
        /// For Worldgen you might be able to use TryPlaceBlockForWorldGen() to attach custom behaviors during placement/removal
        /// 
        /// The default behavior is to delete the block entity, if this block has any
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="handling"></param>
        public virtual void OnBlockRemoved(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }


        public virtual void Activate(IWorldAccessor world, Caller caller, BlockSelection blockSel, ITreeAttribute activationArgs, ref EnumHandling handled)
        {

        }

        /// <summary>
        /// When a player does a right click while targeting this placed block. Should return true if the event is handled, so that other events can occur, e.g. eating a held item if the block is not interactable with.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Called by the block info HUD for displaying additional information
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="forPlayer"></param>
        /// <returns></returns>
        public virtual string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {
            return "";
        }

        

        public virtual void OnBlockExploded(IWorldAccessor world, BlockPos pos, BlockPos explosionCenter, EnumBlastType blastType, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }

        public virtual WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return Array.Empty<WorldInteraction>();
        }


        public virtual void OnBlockInteractStop(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }

        public virtual bool OnBlockInteractStep(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        public virtual bool OnBlockInteractCancel(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }


        /// <summary>
        /// Step 1: Called when the player attempts to place this block. The default behavior calls Block.DoPlaceBlock().
        /// If returned true and default behavior has not been prevented, the game will next call CanPlaceBlock(). If that method also returns true and default behavior has not been overriden, DoPlaceBlock() will get called.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="itemstack"></param>
        /// <param name="blockSel"></param>
        /// <param name="handling"></param>
        /// <param name="failureCode"></param>
        /// <returns></returns>
        public virtual bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Step 2: Test if the block can be placed
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <param name="handling"></param>
        /// <param name="failureCode"></param>
        /// <returns></returns>
        public virtual bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Step 3: Place the block. Return false if it cannot be placed (but you should rather return false in CanPlaceBlock).
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="blockSel"></param>
        /// <param name="byItemStack"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Step 4: Block was placed. Always called when a block has been placed through whatever method, except during worldgen or via ExchangeBlock()
        /// Until 1.20: The BlockEntity (if the block has one) does not exist yet at this point and will not be generated if handling is not PassThrough
        /// 1.21+: The BlockEntity (if the block has one) exists at this point. Setting handling to PreventDefault or PreventSubsequent has no effect on BlockEntity creation, but may affect/prevent later behaviors
        /// </summary>
        /// <param name="world"></param>
        /// <param name="blockPos"></param>
        /// <param name="handling"></param>
        public virtual void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }

        public override void OnCreatedByCrafting(ItemSlot[] allInputslots, ItemSlot outputSlot, GridRecipe byRecipe, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
        }

        public virtual string GetHeldBlockInfo(IWorldAccessor world, ItemSlot inSlot)
        {
            return "";
        }

        public virtual AssetLocation GetSnowCoveredBlockCode(float snowLevel)
        {
            return null;
        }

        /// <summary>
        /// If this is less than 1.0, will slow down mining of the given block  (e.g. used for reinforced blocks)
        /// </summary>
        public virtual float GetMiningSpeedModifier(IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
        {
            return 1.0f;
        }

        public virtual void GetPlacedBlockName(StringBuilder sb, IWorldAccessor world, BlockPos pos)
        {
            
        }

        [Obsolete("Use GetRetention() instead")]
        public virtual int GetHeatRetention(BlockPos pos, BlockFacing facing, ref EnumHandling handled)
        {
            return 0;
        }

        public virtual int GetRetention(BlockPos pos, BlockFacing facing, EnumRetentionType type, ref EnumHandling handled)
        {
            return 0;
        }

        public virtual float GetLiquidBarrierHeightOnSide(BlockFacing face, BlockPos pos, ref EnumHandling handled)
        {
            return 0;
        }
    }
}
