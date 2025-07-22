using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

#nullable disable

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
        public CachedCuboidListFaster CollisionBoxList = new();

        public Cuboidd entityBox = new();

        // Use class level fields to reduce garbage collection
        public BlockPos tmpPos = new();
        public Vec3d tmpPosDelta = new();

        public BlockPos minPos = new();
        public BlockPos maxPos = new();

        public Vec3d pos = new();

        readonly Cuboidd tmpBox = new();
        readonly BlockPos blockPos = new();
        readonly Vec3d blockPosVec = new();
        readonly BlockPos collBlockPos = new();

        /// <summary>
        /// Takes the entity positiona and motion and adds them, respecting any colliding blocks. The resulting new position is put into outposition
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityPos"></param>
        /// <param name="dtFactor"></param>
        /// <param name="newPosition"></param>
        /// <param name="stepHeight"></param>
        /// <param name="yExtra">Default 1 for the extra high collision boxes of fences</param>
        public void ApplyTerrainCollision(Entity entity, EntityPos entityPos, float dtFactor, ref Vec3d newPosition, float stepHeight = 1, float yExtra = 1)
        {
            minPos.dimension = entityPos.Dimension;

            var worldAccessor = entity.World;
            Vec3d pos = this.pos;           // Local copy for efficiency
            Cuboidd entityBox = this.entityBox; // Local copy for efficiency

            pos.X = entityPos.X;
            pos.Y = entityPos.Y;
            pos.Z = entityPos.Z;

            EnumPushDirection pushDirection = EnumPushDirection.None;

            entityBox.SetAndTranslate(entity.CollisionBox, pos.X, pos.Y, pos.Z);

            double motionX = entityPos.Motion.X * dtFactor;
            double motionY = entityPos.Motion.Y * dtFactor;
            double motionZ = entityPos.Motion.Z * dtFactor;

            // We need to make sure that rounding errors do not place us inside a block, because once inside a block, this algorithm no longer pushes the entity out of it
            // So lets collide with blocks a tiny bit earlier - i.e. by the amount of rounding error. In other words, lets push out the entity out of collision boxes once he gets within epsilon meters instead of 0 meters,
            // so that the position+motion addition at the end of the method never ends up being inside a block

            // A double value has ~15 digits. Our max map size of 64mil means we need 8 digits for the non-fractional part, leaving us with 7 digits for the fraction - so the rounding error is on the 8th digit
            // But for some reason we still clip through blocks if we use an epsilon that is less than 0.0001. Not sure why.
            double epsilon = 0.0001;
            double motEpsX = 0, motEpsY = 0, motEpsZ = 0;
            if (motionX > epsilon) motEpsX = epsilon;
            if (motionX < -epsilon) motEpsX = -epsilon;

            if (motionY > epsilon) motEpsY = epsilon;
            if (motionY < -epsilon) motEpsY = -epsilon;

            if (motionZ > epsilon) motEpsZ = epsilon;
            if (motionZ < -epsilon) motEpsZ = -epsilon;

            // We pretend we are by epsilon meters further and push the entity out of it
            // but at the end of the method we do not add this epsilon to the final position
            motionX += motEpsX;
            motionY += motEpsY;
            motionZ += motEpsZ;


            // Generate a cube that encompasses every block between the old and new position.
            // This could also just take the new position and old position without using motion.
            GenerateCollisionBoxList(worldAccessor.BlockAccessor, motionX, motionY, motionZ, stepHeight, yExtra, entityPos.Dimension);

            bool collided = false;

            int collisionBoxListCount = CollisionBoxList.Count;
            Cuboidd[] CollisionBoxListCuboids = CollisionBoxList.cuboids;   // Local reference for efficiency

            collBlockPos.dimension = entityPos.Dimension;
            // ---------- Y COLLISION. Call events and set collided vertically.
            for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
            {
                if (i >= collisionBoxListCount) break;
                motionY = CollisionBoxListCuboids[i].pushOutY(entityBox, motionY, ref pushDirection);
                if (pushDirection == EnumPushDirection.None) continue;

                collided = true;

                collBlockPos.Set(CollisionBoxList.positions[i]);
                CollisionBoxList.blocks[i].OnEntityCollide(
                    worldAccessor,
                    entity,
                    collBlockPos,
                    pushDirection == EnumPushDirection.Negative ? BlockFacing.UP : BlockFacing.DOWN,
                    tmpPosDelta.Set(motionX, motionY, motionZ),
                    !entity.CollidedVertically
                );
            }
            entityBox.Translate(0, motionY, 0);

            entity.CollidedVertically = collided;

            // Check if horizontal collision is possible.
            bool horizontallyBlocked = false;
            entityBox.Translate(motionX, 0, motionZ);
            foreach (var cuboid in CollisionBoxList)
            {
                if (cuboid.Intersects(entityBox))
                {
                    horizontallyBlocked = true;
                    break;
                }
            }
            entityBox.Translate(-motionX, 0, -motionZ);  // cheaper than creating a new Cuboidd

            // No collisions for the entity found when testing horizontally, so skip this.
            // This allows entities to move around corners without falling down on a certain axis.
            collided = false;
            if (horizontallyBlocked)
            {
                // X - Collision (Horizontal)
                for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
                {
                    if (i >= collisionBoxListCount) break;
                    motionX = CollisionBoxListCuboids[i].pushOutX(entityBox, motionX, ref pushDirection);
                    if (pushDirection == EnumPushDirection.None) continue;

                    collided = true;

                    collBlockPos.Set(CollisionBoxList.positions[i]);
                    CollisionBoxList.blocks[i].OnEntityCollide(
                        worldAccessor,
                        entity,
                        collBlockPos,
                        pushDirection == EnumPushDirection.Negative ? BlockFacing.EAST : BlockFacing.WEST,
                        tmpPosDelta.Set(motionX, motionY, motionZ),
                        !entity.CollidedHorizontally
                    );
                }
                entityBox.Translate(motionX, 0, 0);

                // Z - Collision (Horizontal)

                for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
                {
                    if (i >= collisionBoxListCount) break;
                    motionZ = CollisionBoxListCuboids[i].pushOutZ(entityBox, motionZ, ref pushDirection);
                    if (pushDirection == EnumPushDirection.None) continue;

                    collided = true;

                    collBlockPos.Set(CollisionBoxList.positions[i]);
                    CollisionBoxList.blocks[i].OnEntityCollide(
                        worldAccessor,
                        entity,
                        collBlockPos,
                        pushDirection == EnumPushDirection.Negative ? BlockFacing.SOUTH : BlockFacing.NORTH,
                        tmpPosDelta.Set(motionX, motionY, motionZ),
                        !entity.CollidedHorizontally
                    );
                }
            }

            entity.CollidedHorizontally = collided;

            // fix for player on ladder clipping into block above issue  (caused by the .CollisionBox not always having height precisely 1.85)
            if (motionY > 0 && entity.CollidedVertically)
            {
                motionY -= entity.LadderFixDelta;
            }

            motionX -= motEpsX;
            motionY -= motEpsY;
            motionZ -= motEpsZ;

            newPosition.Set(pos.X + motionX, pos.Y + motionY, pos.Z + motionZ);
        }

        protected virtual void GenerateCollisionBoxList(IBlockAccessor blockAccessor, double motionX, double motionY, double motionZ, float stepHeight, float yExtra, int dimension)
        {
            // NEVER CALLED IN 1.20 as all invocations are in CachingCollisionTester.  But we retain this in case a mod calls it.

            // Check if the min and max positions of the collision test are unchanged and use the old list if they are.
            bool minPosIsUnchanged = minPos.SetAndEquals(
                (int)(entityBox.X1 + Math.Min(0, motionX)),
                (int)(entityBox.Y1 + Math.Min(0, motionY) - yExtra), // yExtra looks at blocks below to allow for the extra high collision box of fences.
                (int)(entityBox.Z1 + Math.Min(0, motionZ))
            );

            double y2 = Math.Max(entityBox.Y1 + stepHeight, entityBox.Y2);

            bool maxPosIsUnchanged = maxPos.SetAndEquals(
                (int)(entityBox.X2 + Math.Max(0, motionX)),
                (int)(y2 + Math.Max(0, motionY)),
                (int)(entityBox.Z2 + Math.Max(0, motionZ))
            );

            if (minPosIsUnchanged && maxPosIsUnchanged) return;

            // Clear the list and add every cuboid the block has to it.
            CollisionBoxList.Clear();
            blockAccessor.WalkBlocks(minPos, maxPos, (block, x, y, z) => {
                Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, tmpPos.Set(x, y, z));
                if (collisionBoxes != null)
                {
                    CollisionBoxList.Add(collisionBoxes, x, y, z, block);
                }
            }, true);
        }


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

            int minX = (int)entityBox.X1;
            int minY = (int)entityBox.Y1 - 1; // -1 for the extra high collision box of fences.
            int minZ = (int)entityBox.Z1;

            int maxX = (int)entityBox.X2;
            int maxY = (int)entityBox.Y2;
            int maxZ = (int)entityBox.Z2;

            entityBox.Y1 = Math.Round(entityBox.Y1, 5); // Fix float/double rounding errors. Only need to fix the vertical because gravity.

            BlockPos blockPos = this.blockPos;   // Local reference for efficiency
            Vec3d blockPosVec = this.blockPosVec;   // Local reference for efficiency
            for (int y = minY; y <= maxY; y++)
            {
                blockPos.SetAndCorrectDimension(minX, y, minZ);
                blockPosVec.Set(minX, y, minZ);
                for (int x = minX; x <= maxX; x++)
                {
                    blockPos.X = x;
                    blockPosVec.X = x;
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        blockPos.Z = z;
                        Block block = blockAccessor.GetBlock(blockPos, BlockLayersAccess.MostSolid);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);
                        if (collisionBoxes == null || collisionBoxes.Length == 0) continue;

                        blockPosVec.Z = z;
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
        /// If given cuboidf collides with the terrain, returns the collision box it collides with. By default also checks if the cuboid is merely touching the terrain, set alsoCheckTouch to disable that.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="entityBoxRel"></param>
        /// <param name="pos"></param>
        /// <param name="alsoCheckTouch"></param>
        /// <returns></returns>
        public Cuboidd GetCollidingCollisionBox(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, bool alsoCheckTouch = true)
        {
            BlockPos blockPos = new();
            Vec3d blockPosVec = new();
            Cuboidd entityBox = entityBoxRel.ToDouble().Translate(pos);

            entityBox.Y1 = Math.Round(entityBox.Y1, 5); // Fix float/double rounding errors. Only need to fix the vertical because gravity.

            int minX = (int)(entityBoxRel.X1 + pos.X);
            int minY = (int)(entityBoxRel.Y1 + pos.Y - 1); // -1 for the extra high collision box of fences
            int minZ = (int)(entityBoxRel.Z1 + pos.Z);

            int maxX = (int)Math.Ceiling(entityBoxRel.X2 + pos.X);
            int maxY = (int)Math.Ceiling(entityBoxRel.Y2 + pos.Y);
            int maxZ = (int)Math.Ceiling(entityBoxRel.Z2 + pos.Z);

            for (int y = minY; y <= maxY; y++)
            {
                blockPos.Set(minX, y, minZ);
                blockPosVec.Set(minX, y, minZ);
                for (int x = minX; x <= maxX; x++)
                {
                    blockPos.X = x;
                    blockPosVec.X = x;
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        blockPos.Z = z;
                        Block block = blockAccessor.GetMostSolidBlock(x, y, z);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);
                        if (collisionBoxes == null) continue;

                        blockPosVec.Z = z;
                        for (int i = 0; i < collisionBoxes.Length; i++)
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
        /// <br/>NOTE: currently not dimension-aware unless the supplied Vec3d pos is dimension-aware
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="entityBoxRel"></param>
        /// <param name="pos"></param>
        /// <param name="intoCuboid"></param>
        /// <param name="alsoCheckTouch"></param>
        /// <returns></returns>
        public bool GetCollidingCollisionBox(IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, ref Cuboidd intoCuboid, bool alsoCheckTouch = true, int dimension = 0)
        {
            BlockPos blockPos = new(dimension);
            Vec3d blockPosVec = new();
            Cuboidd entityBox = entityBoxRel.ToDouble().Translate(pos);

            entityBox.Y1 = Math.Round(entityBox.Y1, 5); // Fix float/double rounding errors. Only need to fix the vertical because gravity.

            int minX = (int)(entityBoxRel.X1 + pos.X);
            int minY = (int)(entityBoxRel.Y1 + pos.Y - 1); // -1 for the extra high collision box of fences.
            int minZ = (int)(entityBoxRel.Z1 + pos.Z);

            int maxX = (int)Math.Ceiling(entityBoxRel.X2 + pos.X);
            int maxY = (int)Math.Ceiling(entityBoxRel.Y2 + pos.Y);
            int maxZ = (int)Math.Ceiling(entityBoxRel.Z2 + pos.Z);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    blockPos.Set(x, y, minZ);
                    blockPosVec.Set(x, y, minZ);
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        blockPos.Z = z;
                        Block block = blockAccessor.GetBlock(blockPos, BlockLayersAccess.MostSolid);

                        Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);
                        if (collisionBoxes == null) continue;

                        blockPosVec.Z = z;
                        for (int i = 0; i < collisionBoxes.Length; i++)
                        {
                            Cuboidf collBox = collisionBoxes[i];
                            if (collBox == null) continue;

                            bool colliding = alsoCheckTouch ? entityBox.IntersectsOrTouches(collBox, blockPosVec) : entityBox.Intersects(collBox, blockPosVec);
                            if (colliding)
                            {
                                intoCuboid.Set(collBox).Translate(blockPos);
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
                x + aabb.X2 > aabb2.X1 + pos.X &&
                z + aabb.Z1 < aabb2.Z2 + pos.Z &&
                z + aabb.Z2 > aabb2.Z1 + pos.Z &&
                y + aabb.Y1 < aabb2.Y2 + pos.Y &&
                y + aabb.Y2 > aabb2.Y1 + pos.Y
                ;
        }

        public static EnumIntersect AabbIntersect(Cuboidd aabb, Cuboidd aabb2, Vec3d motion)
        {
            if (aabb.Intersects(aabb2)) return EnumIntersect.Stuck;

            // X
            if (
                aabb.X1 < aabb2.X2 + motion.X &&
                aabb.X2 > aabb2.X1 + motion.X &&
                aabb.Z1 < aabb2.Z2 &&
                aabb.Z2 > aabb2.Z1 &&
                aabb.Y1 < aabb2.Y2 &&
                aabb.Y2 > aabb2.Y1
            ) return EnumIntersect.IntersectX;

            // Y
            if (
                aabb.X1 < aabb2.X2 &&
                aabb.X2 > aabb2.X1 &&
                aabb.Z1 < aabb2.Z2 &&
                aabb.Z2 > aabb2.Z1 &&
                aabb.Y1 < aabb2.Y2 + motion.Y &&
                aabb.Y2 > aabb2.Y1 + motion.Y
            ) return EnumIntersect.IntersectY;

            // Z
            if (
                aabb.X1 < aabb2.X2 &&
                aabb.X2 > aabb2.X1 &&
                aabb.Z1 < aabb2.Z2 + motion.Z &&
                aabb.Z2 > aabb2.Z1 + motion.Z &&
                aabb.Y1 < aabb2.Y2 &&
                aabb.Y2 > aabb2.Y1
            ) return EnumIntersect.IntersectZ;

            return EnumIntersect.NoIntersect;
        }

    }

    /// <summary>
    /// Originally intended to be a special version of CollisionTester for BehaviorControlledPhysics, which does not re-do the WalkBlocks() call and re-generate the CollisionBoxList more than once in the same entity tick
    /// <br/>Currently in 1.20 the caching is not very useful when we loop through all entities sequentially - but empirical testing shows it is actually faster not to cache
    /// </summary>
    public class CachingCollisionTester : CollisionTester
    {
        public void NewTick(EntityPos entityPos)
        {
            minPos.Set(int.MinValue, int.MinValue, int.MinValue);
            minPos.dimension = entityPos.Dimension;
            tmpPos.dimension = entityPos.Dimension;
        }

        public void AssignToEntity(PhysicsBehaviorBase entityPhysics, int dimension)
        {
            minPos.dimension = dimension;
            tmpPos.dimension = dimension;
        }

        protected override void GenerateCollisionBoxList(IBlockAccessor blockAccessor, double motionX, double motionY, double motionZ, float stepHeight, float yExtra, int dimension)
        {
            Cuboidd entityBox = this.entityBox;  // Local reference for efficiency

            bool minPosIsUnchanged = minPos.SetAndEquals(
                (int)(entityBox.X1 + Math.Min(0, motionX)),
                (int)(entityBox.Y1 + Math.Min(0, motionY) - yExtra), // yExtra looks at blocks below to allow for the extra high collision box of fences
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
            blockAccessor.WalkBlocks(minPos, maxPos, (block, x, y, z) => {
                Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, tmpPos.Set(x, y, z));
                if (collisionBoxes != null)
                {
                    CollisionBoxList.Add(collisionBoxes, x, y, z, block);
                }
            }, true);
        }

        public void PushOutFromBlocks(IBlockAccessor blockAccessor, Entity entity, Vec3d tmpVec, float clippingLimit)
        {
            if (IsColliding(blockAccessor, entity.CollisionBox, tmpVec, false))
            {
                Vec3d pos = entity.SidedPos.XYZ;
                entityBox.SetAndTranslate(entity.CollisionBox, pos.X, pos.Y, pos.Z);

                GenerateCollisionBoxList(blockAccessor, 0, 0, 0, 0.5f, 0, entity.SidedPos.Dimension);

                int collisionBoxListCount = CollisionBoxList.Count;
                if (collisionBoxListCount == 0) return;
                Cuboidd[] CollisionBoxListCuboids = CollisionBoxList.cuboids;   // Local reference for efficiency

                double deltaX = 0;
                double deltaZ = 0;
                EnumPushDirection pushDirection = EnumPushDirection.None;
                var reducedBox = entity.CollisionBox.ToDouble();
                reducedBox.Translate(pos.X, pos.Y, pos.Z);
                reducedBox.GrowBy(-clippingLimit, 0, -clippingLimit);

                for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
                {
                    if (i >= collisionBoxListCount) break;
                    deltaX = CollisionBoxListCuboids[i].pushOutX(reducedBox, clippingLimit, ref pushDirection);
                }
                if (deltaX == clippingLimit)
                {
                    for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
                    {
                        if (i >= collisionBoxListCount) break;
                        deltaX = CollisionBoxListCuboids[i].pushOutX(reducedBox, -clippingLimit, ref pushDirection);
                    }
                    deltaX += clippingLimit;
                }
                else deltaX -= clippingLimit;

                for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
                {
                    if (i >= collisionBoxListCount) break;
                    deltaZ = CollisionBoxListCuboids[i].pushOutZ(reducedBox, clippingLimit, ref pushDirection);
                }
                if (deltaZ == clippingLimit)
                {
                    for (int i = 0; i < CollisionBoxListCuboids.Length; i++)
                    {
                        if (i >= collisionBoxListCount) break;
                        deltaZ = CollisionBoxListCuboids[i].pushOutZ(reducedBox, -clippingLimit, ref pushDirection);
                    }
                    deltaZ += clippingLimit;
                }
                else deltaZ -= clippingLimit;

                entity.SidedPos.X = pos.X + deltaX;
                entity.SidedPos.Z = pos.Z + deltaZ;
            }
        }
    }
}
