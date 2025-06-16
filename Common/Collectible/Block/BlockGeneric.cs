using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class BlockGeneric : Block
    {
        public override void GetDecal(IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData)
        {
            bool preventDefault = false;
            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;

                EnumHandling handled = EnumHandling.PassThrough;
                sbh.GetDecal(world, pos, decalTexSource, ref decalModelData, ref blockModelData, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return;
            }

            if (preventDefault) return;

            base.GetDecal(world, pos, decalTexSource, ref decalModelData, ref blockModelData);
        }

        public override void OnDecalTesselation(IWorldAccessor world, MeshData decalMesh, BlockPos pos)
        {
            bool preventDefault = false;
            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;

                EnumHandling handled = EnumHandling.PassThrough;
                sbh.OnDecalTesselation(world, decalMesh, pos, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return;
            }

            if (preventDefault) return;

            base.OnDecalTesselation(world, decalMesh, pos);
        }

        public override Cuboidf GetParticleBreakBox(IBlockAccessor blockAccess, BlockPos pos, BlockFacing facing)
        {
            bool preventDefault = false;
            Cuboidf box = null;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;

                EnumHandling handled = EnumHandling.PassThrough;

                box = sbh.GetParticleBreakBox(blockAccess, pos, facing, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return box;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
            }

            if (preventDefault) return box;

            return base.GetParticleBreakBox(blockAccess, pos, facing);
        }

        public override Cuboidf[] GetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
        {
            bool preventDefault = false;
            List<Cuboidf> allboxes = null;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;


                EnumHandling handled = EnumHandling.PassThrough;

                Cuboidf[] boxes = sbh.GetParticleCollisionBoxes(blockAccessor, pos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return boxes;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
                if (boxes == null) continue;

                if (allboxes == null) allboxes = new List<Cuboidf>();
                allboxes.AddRange(boxes);
            }

            if (preventDefault) return allboxes.ToArray();
            if (allboxes == null) return base.GetParticleCollisionBoxes(blockAccessor, pos);

            allboxes.AddRange(base.GetParticleCollisionBoxes(blockAccessor, pos));

            return allboxes.ToArray();
        }

        public override Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
        {
            bool preventDefault = false;
            List<Cuboidf> allboxes = null;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;

                EnumHandling handled = EnumHandling.PassThrough;

                Cuboidf[] boxes = sbh.GetCollisionBoxes(blockAccessor, pos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return boxes;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
                if (boxes == null) continue;

                if (allboxes == null) allboxes = new List<Cuboidf>();
                allboxes.AddRange(boxes);
            }

            if (preventDefault) return allboxes.ToArray();
            if (allboxes == null) return base.GetCollisionBoxes(blockAccessor, pos);

            allboxes.AddRange(base.GetCollisionBoxes(blockAccessor, pos));

            return allboxes.ToArray();
        }

        public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
        {
            bool preventDefault = false;
            List<Cuboidf> allboxes = null;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;


                EnumHandling handled = EnumHandling.PassThrough;

                Cuboidf[] boxes = sbh.GetSelectionBoxes(blockAccessor, pos, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return boxes;
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
                if (boxes == null) continue;

                if (allboxes == null) allboxes = new List<Cuboidf>();
                allboxes.AddRange(boxes);
            }

            if (preventDefault) return allboxes.ToArray();
            if (allboxes == null) return base.GetSelectionBoxes(blockAccessor, pos);

            allboxes.AddRange(base.GetSelectionBoxes(blockAccessor, pos));

            return allboxes.ToArray();
        }

        public override bool TryPlaceBlockForWorldGen(IBlockAccessor blockAccessor, BlockPos pos, BlockFacing onBlockFace, IRandom worldgenRandom, BlockPatchAttributes attributes = null)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;


                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = sbh.TryPlaceBlockForWorldGen(blockAccessor, pos, onBlockFace, worldgenRandom, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;

            return base.TryPlaceBlockForWorldGen(blockAccessor, pos, onBlockFace, worldgenRandom, attributes);
        }

        public override bool DoParticalSelection(IWorldAccessor world, BlockPos pos)
        {
            bool result = true;
            bool preventDefault = false;

            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                StrongBlockBehavior sbh = behavior as StrongBlockBehavior;
                if (sbh == null) continue;


                EnumHandling handled = EnumHandling.PassThrough;
                bool behaviorResult = sbh.DoParticalSelection(world, pos, ref handled);
                if (handled != EnumHandling.PassThrough)
                {
                    result &= behaviorResult;
                    preventDefault = true;
                }

                if (handled == EnumHandling.PreventSubsequent) return result;
            }

            if (preventDefault) return result;


            return base.DoParticalSelection(world, pos);
        }
    }
}
