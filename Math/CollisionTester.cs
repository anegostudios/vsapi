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
        public Vec3d tmpPosDelta = new Vec3d();
        public Vec3d tmpPositionVec = new Vec3d();
        public Cuboidd tempCuboid = new Cuboidd();
        EnumPushDirection pushDirection = EnumPushDirection.None;

        protected BlockPos minPos = new BlockPos();
        protected BlockPos maxPos = new BlockPos();


        /// <summary>
        /// Takes the entity positiona and motion and adds them, respecting any colliding blocks. The resulting new position is put into outposition
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entitypos"></param>
        /// <param name="dtFac"></param>
        /// <param name="outposition"></param>
        /// <param name="stepHeight"></param>
        /// <param name="YExtra">Default 1 for the extra high collision boxes of fences</param>
        public void ApplyTerrainCollision(Entity entity, EntityPos entitypos, float dtFac, ref Vec3d outposition, float stepHeight = 1, float YExtra = 1)
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

          //  entitypos.Motion.X = GameMath.Clamp(entitypos.Motion.X, -1, 1);
          //  entitypos.Motion.Y = GameMath.Clamp(entitypos.Motion.Y, -1, 1);
          //  entitypos.Motion.Z = GameMath.Clamp(entitypos.Motion.Z, -1, 1);

            tmpPositionVec.Set(pos);

            entityBox.Set(
                entity.CollisionBox.X1 + tmpPositionVec.X,
                entity.CollisionBox.Y1 + tmpPositionVec.Y,
                entity.CollisionBox.Z1 + tmpPositionVec.Z,
                entity.CollisionBox.X2 + tmpPositionVec.X,
                entity.CollisionBox.Y2 + tmpPositionVec.Y,
                entity.CollisionBox.Z2 + tmpPositionVec.Z
            );

            double motionX = entitypos.Motion.X * dtFac;
            double motionY = entitypos.Motion.Y * dtFac;
            double motionZ = entitypos.Motion.Z * dtFac;
            outposition.Set(tmpPositionVec.X + motionX, tmpPositionVec.Y + motionY, tmpPositionVec.Z + motionZ);

            GenerateCollisionBoxList(worldaccess.BlockAccessor, motionX, motionY, motionZ, stepHeight, YExtra);

            tmpPosDelta.Set(motionX, motionY, motionZ);


            // Y - Collision (Vertical)
            bool collided = false;

            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                tmpPosDelta.Y = CollisionBoxList.cuboids[i].pushOutY(entityBox, tmpPosDelta.Y, ref pushDirection);
                if (pushDirection == EnumPushDirection.None)
                    continue;

                collided = true;

                CollisionBoxList.blocks[i].OnEntityCollide(
                    worldaccess,
                    entity,
                    CollisionBoxList.positions[i],
                    pushDirection == EnumPushDirection.Negative ? BlockFacing.UP : BlockFacing.DOWN,
                    tmpPosDelta,
                    !entity.CollidedVertically
                );
            }

            entity.CollidedVertically = collided;
            entityBox.Translate(0, tmpPosDelta.Y, 0);


            var horizontalBlocked = false;
            var entityBoxMovedXZ = entityBox.OffsetCopy(tmpPosDelta.X, 0, tmpPosDelta.Z);
            foreach (var cuboid in CollisionBoxList)
            {
                if (cuboid.Intersects(entityBoxMovedXZ))
                {
                    horizontalBlocked = true;
                    break;
                }
            }

            // No collisions for the entity found when testing horizontally, so skip this.
            // This allows entities to move around corners without falling down on a certain axis.
            collided = false;
            if (horizontalBlocked)
            {
                // X - Collision (Horizontal)

                for (int i = 0; i < CollisionBoxList.Count; i++)
                {
                    tmpPosDelta.X = CollisionBoxList.cuboids[i].pushOutX(entityBox, tmpPosDelta.X, ref pushDirection);

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
                        tmpPosDelta,
                        !entity.CollidedHorizontally
                    );
                }

                entityBox.Translate(tmpPosDelta.X, 0, 0);

                // Z - Collision (Horizontal)

                for (int i = 0; i < CollisionBoxList.Count; i++)
                {
                    tmpPosDelta.Z = CollisionBoxList.cuboids[i].pushOutZ(entityBox, tmpPosDelta.Z, ref pushDirection);
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
                        tmpPosDelta,
                        !entity.CollidedHorizontally
                    );
                }

            }

            entity.CollidedHorizontally = collided;

            //fix for player on ladder clipping into block above issue  (caused by the .CollisionBox not always having height precisely 1.85)
            if (entity.CollidedVertically && tmpPosDelta.Y > 0)
            {
                tmpPosDelta.Y -= entity.LadderFixDelta;
            }

            outposition.Set(tmpPositionVec.X + tmpPosDelta.X, tmpPositionVec.Y + tmpPosDelta.Y, tmpPositionVec.Z + tmpPosDelta.Z);
        }

        protected virtual void GenerateCollisionBoxList(IBlockAccessor blockAccessor, double motionX, double motionY, double motionZ, float stepHeight, float yExtra)
        {
            minPos.Set(
                (int)(entityBox.X1 + Math.Min(0, motionX)),
                (int)(entityBox.Y1 + Math.Min(0, motionY) - yExtra), // yExtra for the extra high collision box of fences
                (int)(entityBox.Z1 + Math.Min(0, motionZ))
            );

            double y2 = Math.Max(entityBox.Y1 + stepHeight, entityBox.Y2);

            maxPos.Set(
                (int)(entityBox.X2 + Math.Max(0, motionX)),
                (int)(y2 + Math.Max(0, motionY)),
                (int)(entityBox.Z2 + Math.Max(0, motionZ))
            );

            CollisionBoxList.Clear();
            blockAccessor.WalkBlocks(minPos, maxPos, (block, bpos) => {
                Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, bpos);

                if (collisionBoxes != null)
                {
                    for (int i = 0; i < collisionBoxes.Length; i++)
                    {
                        CollisionBoxList.Add(collisionBoxes[i], bpos, block);
                    }
                }

            }, true);
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
            return GetCollidingBlock(blockAccessor, entityBoxRel, pos, alsoCheckTouch) != null;
        }

        public Block GetCollidingBlock(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, bool alsoCheckTouch = true) 
        { 
            Cuboidd entityBox = tmpBox.SetAndTranslate(entityBoxRel, pos);

            int minX = (int)(entityBox.X1);
            int minY = (int)(entityBox.Y1) - 1;  // -1 for the extra high collision box of fences
            int minZ = (int)(entityBox.Z1);

            int maxX = (int)(entityBox.X2);
            int maxY = (int)(entityBox.Y2);
            int maxZ = (int)(entityBox.Z2);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        Block block = blockAccessor.GetBlock(x, y, z);
                        blockPos.Set(x, y, z);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);
                        if (collisionBoxes == null || collisionBoxes.Length == 0) continue;

                        blockPosVec.Set(x, y, z);
                        for (int i = 0; i < collisionBoxes.Length; i++)
                        {
                            Cuboidf collBox = collisionBoxes[i];
                            if (collBox == null) continue;

                            if (alsoCheckTouch ? entityBox.IntersectsOrTouches(collBox, blockPosVec) : entityBox.Intersects(collBox, blockPosVec)) return block;
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
        public Cuboidd GetCollidingCollisionBox(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, bool alsoCheckTouch = true)
        {
            BlockPos blockPos = new BlockPos();
            Vec3d blockPosVec = new Vec3d();
            Cuboidd entityBox = entityBoxRel.ToDouble().Translate(pos);
            
            entityBox.Y1 = Math.Round(entityBox.Y1, 5); // Fix float/double rounding errors. Only need to fix the vertical because gravity.

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
                                return collBox.ToDouble().Translate(blockPos);
                            }
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

    /// <summary>
    /// Special version of CollisionTester for BehaviorControlledPhysics, which does not re-do the WalkBlocks() call and re-generate the CollisionBoxList more than once in the same entity tick
    /// </summary>
    public class CachingCollisionTester : CollisionTester
    {
        public void NewTick()
        {
            minPos.Set(int.MinValue, int.MinValue, int.MinValue);
        }

        protected override void GenerateCollisionBoxList(IBlockAccessor blockAccessor, double motionX, double motionY, double motionZ, float stepHeight, float yExtra)
        {
            bool minPosIsUnchanged = minPos.SetAndEquals(
                (int)(entityBox.X1 + Math.Min(0, motionX)),
                (int)(entityBox.Y1 + Math.Min(0, motionY) - yExtra), // yExtra for the extra high collision box of fences
                (int)(entityBox.Z1 + Math.Min(0, motionZ))
            );
            double y2 = Math.Max(entityBox.Y1 + stepHeight, entityBox.Y2);

            bool maxPosIsUnchanged = maxPos.SetAndEquals(
                (int)(entityBox.X2 + Math.Max(0, motionX)),
                (int)(y2 + Math.Max(0, motionY)),
                (int)(entityBox.Z2 + Math.Max(0, motionZ))
            );

            if (minPosIsUnchanged && maxPosIsUnchanged)
            {
                return;
            }

            CollisionBoxList.Clear();
            blockAccessor.WalkBlocks(minPos, maxPos, (block, bpos) => {
                Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, bpos);

                if (collisionBoxes != null)
                {
                    for (int i = 0; i < collisionBoxes.Length; i++)
                    {
                        CollisionBoxList.Add(collisionBoxes[i], bpos, block);
                    }
                }

            }, true);
        }
    }
}
