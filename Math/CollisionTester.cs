using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.MathTools
{
    public enum EnumIntersect
    {
        NoIntersect,
        IntersectX,
        IntersectY,
        IntersectZ,
        Stuck
    }

    public class CollisionTester
    {
        public CachedCuboidList CollisionBoxList = new CachedCuboidList();

        public Cuboidd entityBox = new Cuboidd();

        // Use class level fields to reduce garbage collection
        public BlockPos tmpPos = new BlockPos();
        public Vec3d tmpMotion = new Vec3d();
        public Vec3d tmpPositionVec = new Vec3d();
        public Cuboidd tempCuboid = new Cuboidd();
        EnumPushDirection pushDirection = EnumPushDirection.None;

        BlockPos minPos = new BlockPos();
        BlockPos maxPos = new BlockPos();

        public void ApplyTerrainCollision(Entity entity, EntityPos entitypos, ref Vec3d outposition, bool blockWhenInside)
        {
            IWorldAccessor worldaccess = entity.World;
            Vec3d pos = entitypos.XYZ;

            // Full stop when already inside a collisionbox, but allow a small margin of error
            // Disabled. Causes too many issues
            /*(if (blockWhenInside && IsColliding(worldaccess.BlockAccessor, entity.CollisionBox, pos, false))
            {
                entity.CollidedVertically = true;
                entity.CollidedVertically = true;
                outposition.Set(pos);
                return;
            }*/



            tmpPositionVec.Set(pos);

            entityBox.Set(
                entity.CollisionBox.X1 + tmpPositionVec.X,
                entity.CollisionBox.Y1 + tmpPositionVec.Y,
                entity.CollisionBox.Z1 + tmpPositionVec.Z,
                entity.CollisionBox.X2 + tmpPositionVec.X,
                entity.CollisionBox.Y2 + tmpPositionVec.Y,
                entity.CollisionBox.Z2 + tmpPositionVec.Z
            );

            CollisionBoxList.Clear();
            outposition.Set(tmpPositionVec.X + entitypos.Motion.X, tmpPositionVec.Y + entitypos.Motion.Y, tmpPositionVec.Z + entitypos.Motion.Z);

            minPos.Set(
                (int)(entityBox.X1 + Math.Min(0, entitypos.Motion.X)),
                (int)(entityBox.Y1 + Math.Min(0, entitypos.Motion.Y) - 1), // -1 for the extra high collision box of fences
                (int)(entityBox.Z1 + Math.Min(0, entitypos.Motion.Z))
            );

            maxPos.Set(
                (int)(entityBox.X2 + Math.Max(0, entitypos.Motion.X)),
                (int)(entityBox.Y2 + Math.Max(0, entitypos.Motion.Y)),
                (int)(entityBox.Z2 + Math.Max(0, entitypos.Motion.Z))
            );

            worldaccess.BlockAccessor.WalkBlocks(minPos, maxPos, (block, bpos) => {
                Cuboidf[] collisionBoxes = block.GetCollisionBoxes(worldaccess.BlockAccessor, bpos);

                for (int i = 0; collisionBoxes != null && i < collisionBoxes.Length; i++)
                {
                    CollisionBoxList.Add(collisionBoxes[i], bpos, block);
                }

            }, true);


            tmpMotion.Set(entitypos.Motion.X, entitypos.Motion.Y, entitypos.Motion.Z);


            // Y - Collision (Vertical)
            bool collided = false;

            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                tmpMotion.Y = CollisionBoxList.cuboids[i].pushOutY(entityBox, tmpMotion.Y, ref pushDirection);
                if (pushDirection == EnumPushDirection.None)
                    continue;

                collided = true;

                CollisionBoxList.blocks[i].OnEntityCollide(
                    worldaccess,
                    entity,
                    CollisionBoxList.positions[i],
                    pushDirection == EnumPushDirection.Negative ? BlockFacing.UP : BlockFacing.DOWN,
                    tmpMotion,
                    !entity.CollidedVertically
                );
            }

            entity.CollidedVertically = collided;
            entityBox.Translate(0, tmpMotion.Y, 0);


            // X - Collision (Horizontal)
            collided = false;

            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                tmpMotion.X = CollisionBoxList.cuboids[i].pushOutX(entityBox, tmpMotion.X, ref pushDirection);

                if (pushDirection == EnumPushDirection.None)
                {
                    continue;
                }

                collided = true;

                CollisionBoxList.blocks[i].OnEntityCollide(
                    worldaccess,
                    entity,
                    CollisionBoxList.positions[i],
                    pushDirection == EnumPushDirection.Negative ? BlockFacing.EAST : BlockFacing.WEST,
                    tmpMotion,
                    !entity.CollidedHorizontally
                );
            }

            entityBox.Translate(tmpMotion.X, 0, 0);

            // Z - Collision (Horizontal)

            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                tmpMotion.Z = CollisionBoxList.cuboids[i].pushOutZ(entityBox, tmpMotion.Z, ref pushDirection);
                if (pushDirection == EnumPushDirection.None)
                {
                    continue;
                }

                collided = true;

                CollisionBoxList.blocks[i].OnEntityCollide(
                    worldaccess,
                    entity,
                    CollisionBoxList.positions[i],
                    pushDirection == EnumPushDirection.Negative ? BlockFacing.SOUTH : BlockFacing.NORTH,
                    tmpMotion,
                    !entity.CollidedHorizontally
                );
            }

            entity.CollidedHorizontally = collided;



            outposition.Set(tmpPositionVec.X + tmpMotion.X, tmpPositionVec.Y + tmpMotion.Y, tmpPositionVec.Z + tmpMotion.Z);
        }

        Cuboidd tmpBox = new Cuboidd();
        BlockPos blockPos = new BlockPos();
        Vec3d blockPosVec = new Vec3d();

        /// <summary>
        /// Tests given cuboidf collides with the terrain. By default also checks if the cuboid is merely touching the terrain, set alsoCheckTouch to disable that.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="entityBoxRel"></param>
        /// <param name="pos"></param>
        /// <param name="alsoCheckTouch"></param>
        /// <returns></returns>
        public bool IsColliding(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, bool alsoCheckTouch = true)
        {
            Cuboidd entityBox = tmpBox.Set(entityBoxRel).Translate(pos);

            int minX = (int)(entityBoxRel.X1 + pos.X);
            int minY = (int)(entityBoxRel.Y1 + pos.Y - 1);  // -1 for the extra high collision box of fences
            int minZ = (int)(entityBoxRel.Z1 + pos.Z);
            // why the eff the ceilings?
            /*int maxX = (int)Math.Ceiling(entityBoxRel.X2 + pos.X);
            int maxY = (int)Math.Ceiling(entityBoxRel.Y2 + pos.Y);
            int maxZ = (int)Math.Ceiling(entityBoxRel.Z2 + pos.Z);*/
            int maxX = (int)(entityBoxRel.X2 + pos.X);
            int maxY = (int)(entityBoxRel.Y2 + pos.Y);
            int maxZ = (int)(entityBoxRel.Z2 + pos.Z);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        Block block = blockAccessor.GetBlock(x, y, z);
                        blockPos.Set(x, y, z);
                        blockPosVec.Set(x, y, z);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);

                        for (int i = 0; collisionBoxes != null && i < collisionBoxes.Length; i++)
                        {
                            Cuboidf collBox = collisionBoxes[i];
                            if (collBox == null) continue;

                            bool colliding = alsoCheckTouch ? entityBox.IntersectsOrTouches(collBox, blockPosVec) : entityBox.Intersects(collBox, blockPosVec);
                            if (colliding) return true;
                        }
                    }
                }
            }

            return false;
        }



        /// <summary>
        /// Tests given cuboidf collides with the terrain. By default also checks if the cuboid is merely touching the terrain, set alsoCheckTouch to disable that.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="entityBoxRel"></param>
        /// <param name="pos"></param>
        /// <param name="alsoCheckTouch"></param>
        /// <returns></returns>
        public Cuboidd GetCollidingCollisionBox(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, bool alsoCheckTouch = true)
        {
            BlockPos blockPos = new BlockPos();
            Vec3d blockPosVec = new Vec3d();
            Cuboidd entityBox = entityBoxRel.ToDouble().Translate(pos);

            int minX = (int)(entityBoxRel.X1 + pos.X);
            int minY = (int)(entityBoxRel.Y1 + pos.Y - 1);  // -1 for the extra high collision box of fences
            int minZ = (int)(entityBoxRel.Z1 + pos.Z);
            int maxX = (int)Math.Ceiling(entityBoxRel.X2 + pos.X);
            int maxY = (int)Math.Ceiling(entityBoxRel.Y2 + pos.Y);
            int maxZ = (int)Math.Ceiling(entityBoxRel.Z2 + pos.Z);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        Block block = blockAccessor.GetBlock(x, y, z);
                        blockPos.Set(x, y, z);
                        blockPosVec.Set(x, y, z);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);

                        for (int i = 0; collisionBoxes != null && i < collisionBoxes.Length; i++)
                        {
                            Cuboidf collBox = collisionBoxes[i];
                            if (collBox == null) continue;

                            bool colliding = alsoCheckTouch ? entityBox.IntersectsOrTouches(collBox, blockPosVec) : entityBox.Intersects(collBox, blockPosVec);
                            if (colliding) return collBox.ToDouble().Translate(blockPos);
                        }
                    }
                }
            }

            return null;
        }





        /// <summary>
        /// Tests given cuboidf collides with the terrain. By default also checks if the cuboid is merely touching the terrain, set alsoCheckTouch to disable that.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="entityBoxRel"></param>
        /// <param name="pos"></param>
        /// <param name="alsoCheckTouch"></param>
        /// <returns></returns>
        public bool GetCollidingCollisionBox(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, ref Cuboidd intoCubuid, bool alsoCheckTouch = true)
        {
            BlockPos blockPos = new BlockPos();
            Vec3d blockPosVec = new Vec3d();
            Cuboidd entityBox = entityBoxRel.ToDouble().Translate(pos);

            int minX = (int)(entityBoxRel.X1 + pos.X);
            int minY = (int)(entityBoxRel.Y1 + pos.Y - 1);  // -1 for the extra high collision box of fences
            int minZ = (int)(entityBoxRel.Z1 + pos.Z);
            int maxX = (int)Math.Ceiling(entityBoxRel.X2 + pos.X);
            int maxY = (int)Math.Ceiling(entityBoxRel.Y2 + pos.Y);
            int maxZ = (int)Math.Ceiling(entityBoxRel.Z2 + pos.Z);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        Block block = blockAccessor.GetBlock(x, y, z);
                        blockPos.Set(x, y, z);
                        blockPosVec.Set(x, y, z);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);

                        for (int i = 0; collisionBoxes != null && i < collisionBoxes.Length; i++)
                        {
                            Cuboidf collBox = collisionBoxes[i];
                            if (collBox == null) continue;

                            bool colliding = alsoCheckTouch ? entityBox.IntersectsOrTouches(collBox, blockPosVec) : entityBox.Intersects(collBox, blockPosVec);
                            if (colliding)
                            {
                                intoCubuid.Set(collBox).Translate(blockPos);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }




        public static bool AabbIntersect(Cuboidf aabb, double x, double y, double z, Cuboidf aabb2, Vec3d pos)
        {
            if (aabb2 == null) return true;

            return
                x + aabb.X1 < aabb2.X2 + pos.X &&
                y + aabb.Y1 < aabb2.Y2 + pos.Y &&
                z + aabb.Z1 < aabb2.Z2 + pos.Z &&
                x + aabb.X2 > aabb2.X1 + pos.X &&
                y + aabb.Y2 > aabb2.Y1 + pos.Y &&
                z + aabb.Z2 > aabb2.Z1 + pos.Z
            ;
        }

        

        public static EnumIntersect AabbIntersect(Cuboidd aabb, Cuboidd aabb2, Vec3d motion)
        {
            bool beforeIntersect = aabb.Intersects(aabb2);

            if (beforeIntersect) return EnumIntersect.Stuck;

            // X
            bool xIntersect =
                aabb.X1 < aabb2.X2 + motion.X &&
                aabb.Y1 < aabb2.Y2 &&
                aabb.Z1 < aabb2.Z2 &&
                aabb.X2 > aabb2.X1 + motion.X &&
                aabb.Y2 > aabb2.Y1 &&
                aabb.Z2 > aabb2.Z1
            ;

            if (xIntersect) return EnumIntersect.IntersectX;

            // Y
            bool yIntersect =
                aabb.X1 < aabb2.X2 &&
                aabb.Y1 < aabb2.Y2 + motion.Y &&
                aabb.Z1 < aabb2.Z2 &&
                aabb.X2 > aabb2.X1 &&
                aabb.Y2 > aabb2.Y1 + motion.Y &&
                aabb.Z2 > aabb2.Z1
            ;

            if (yIntersect) return EnumIntersect.IntersectY;

            // Z
            bool zIntersect =
                aabb.X1 < aabb2.X2 &&
                aabb.Y1 < aabb2.Y2 &&
                aabb.Z1 < aabb2.Z2 + motion.Z &&
                aabb.X2 > aabb2.X1 &&
                aabb.Y2 > aabb2.Y1 &&
                aabb.Z2 > aabb2.Z1 + motion.Z
            ;

            if (zIntersect) return EnumIntersect.IntersectZ;

            return EnumIntersect.NoIntersect;
        }

    }
}
