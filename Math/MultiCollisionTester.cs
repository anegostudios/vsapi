using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class MultiCollisionTester
    {
        public CachedCuboidList CollisionBoxList = new();

        public Cuboidd[] entityBox = ArrayUtil.CreateFilled(10, (i) => new Cuboidd());
        protected int count;
        Cuboidf[] collBox = new Cuboidf[10];

        // Use class level fields to reduce garbage collection
        public BlockPos tmpPos = new();
        public Vec3d tmpPosDelta = new();

        public BlockPos minPos = new();
        public BlockPos maxPos = new();

        public Vec3d pos = new();

        readonly Cuboidd tmpBox = new();
        readonly BlockPos blockPos = new();
        readonly Vec3d blockPosVec = new();

        public void ApplyTerrainCollision(Cuboidf[] collisionBoxes, int collBoxCount, Entity entity, EntityPos entityPos, float dtFactor, ref Vec3d newPosition, float stepHeight = 1, float yExtra = 1)
        {
            this.count = collBoxCount;
            var world = entity.World;

            pos.X = entityPos.X;
            pos.Y = entityPos.Y;
            pos.Z = entityPos.Z;
            
            EnumPushDirection pushDirection = EnumPushDirection.None;

            for (int i = 0; i < collBoxCount; i++)
            {
                entityBox[i].SetAndTranslate(collisionBoxes[i], pos.X, pos.Y, pos.Z);
                //entityBox[i].RemoveRoundingErrors(); // Necessary to prevent unwanted clipping through blocks when there is knockback
            }

            double motionX = entityPos.Motion.X * dtFactor;
            double motionY = entityPos.Motion.Y * dtFactor;
            double motionZ = entityPos.Motion.Z * dtFactor;

            GenerateCollisionBoxList(world.BlockAccessor, motionX, motionY, motionZ, stepHeight, yExtra);

            bool collided = false;
            int collisionBoxListCount = CollisionBoxList.Count;

            // ---------- Y COLLISION. Call events and set collided vertically.
            for (int i = 0; i < collisionBoxListCount; i++)
            {
                for (int j = 0; j < collBoxCount; j++)
                {
                    motionY = CollisionBoxList.cuboids[i].pushOutY(entityBox[j], motionY, ref pushDirection);
                    if (pushDirection != EnumPushDirection.None)
                    {
                        CollisionBoxList.blocks[i].OnEntityCollide(
                            world,
                            entity,
                            CollisionBoxList.positions[i],
                            pushDirection == EnumPushDirection.Negative ? BlockFacing.UP : BlockFacing.DOWN,
                            tmpPosDelta.Set(motionX, motionY, motionZ),
                            !entity.CollidedVertically
                        );

                        collided = true;
                    }
                }

            }

            for (int j = 0; j < collBoxCount; j++) entityBox[j].Translate(0, motionY, 0);

            entity.CollidedVertically = collided;

            // Check if horizontal collision is possible.
            bool horizontallyBlocked = false;
            for (int j = 0; j < collBoxCount; j++) entityBox[j].Translate(motionX, 0, motionZ);
            foreach (var cuboid in CollisionBoxList)
            {
                bool blocked = false;
                for (int j = 0; j < collBoxCount; j++)
                {
                    if (cuboid.Intersects(entityBox[j]))
                    {
                        horizontallyBlocked = true;
                        blocked = true;
                        break;
                    }
                }
                if (blocked) break;
            }

            for (int j = 0; j < collBoxCount; j++)
            {
                entityBox[j].Translate(-motionX, 0, -motionZ);  // cheaper than creating a new Cuboidd
            }

            // No collisions for the entity found when testing horizontally, so skip this.
            // This allows entities to move around corners without falling down on a certain axis.
            collided = false;
            if (horizontallyBlocked)
            {

                // X - Collision (Horizontal)
                for (int i = 0; i < collisionBoxListCount; i++)
                {
                    bool pushed = false;
                    for (int j = 0; j < collBoxCount; j++)
                    {
                        motionX = CollisionBoxList.cuboids[i].pushOutX(entityBox[j], motionX, ref pushDirection);
                        if (pushDirection != EnumPushDirection.None)
                        {
                            CollisionBoxList.blocks[i].OnEntityCollide(
                                world,
                                entity,
                                CollisionBoxList.positions[i],
                                pushDirection == EnumPushDirection.Negative ? BlockFacing.EAST : BlockFacing.WEST,
                                tmpPosDelta.Set(motionX, motionY, motionZ),
                                !entity.CollidedHorizontally
                            );
                        }
                        pushed |= pushDirection != EnumPushDirection.None;
                    }

                    collided = pushed;
                }
                for (int j = 0; j < collBoxCount; j++) entityBox[j].Translate(motionX, 0, 0);

                // Z - Collision (Horizontal)

                for (int i = 0; i < collisionBoxListCount; i++)
                {
                    bool pushed = false;
                    for (int j = 0; j < collBoxCount; j++)
                    {
                        motionZ = CollisionBoxList.cuboids[i].pushOutZ(entityBox[j], motionZ, ref pushDirection);
                        if (pushDirection != EnumPushDirection.None)
                        {
                            CollisionBoxList.blocks[i].OnEntityCollide(
                                world,
                                entity,
                                CollisionBoxList.positions[i],
                                pushDirection == EnumPushDirection.Negative ? BlockFacing.SOUTH : BlockFacing.NORTH,
                                tmpPosDelta.Set(motionX, motionY, motionZ),
                                !entity.CollidedHorizontally
                            );
                        }
                        pushed |= pushDirection != EnumPushDirection.None;
                    }

                    collided = pushed;
                }
            }

            entity.CollidedHorizontally = collided;


            newPosition.Set(pos.X + motionX, pos.Y + motionY, pos.Z + motionZ);
        }

        protected virtual void GenerateCollisionBoxList(IBlockAccessor blockAccessor, double motionX, double motionY, double motionZ, float stepHeight, float yExtra)
        {
            double minx = double.MaxValue, miny = double.MaxValue, minz = double.MaxValue;
            double maxx = double.MinValue, maxy = double.MinValue, maxz = double.MinValue;

            for (int i = 0; i < count; i++)
            {
                var ebox = entityBox[i];
                minx = Math.Min(minx, ebox.X1);
                miny = Math.Min(miny, ebox.Y1);
                minz = Math.Min(minz, ebox.Z1);

                maxx = Math.Max(maxx, ebox.X2);
                maxy = Math.Max(maxy, ebox.Y2);
                maxz = Math.Max(maxz, ebox.Z2);
            }

            // Check if the min and max positions of the collision test are unchanged and use the old list if they are.
            minPos.Set(
                (int)(minx + Math.Min(0, motionX)),
                (int)(miny + Math.Min(0, motionY) - yExtra), // yExtra looks at blocks below to allow for the extra high collision box of fences.
                (int)(minz + Math.Min(0, motionZ))
            );

            double y2 = Math.Max(miny + stepHeight, maxy);

             maxPos.Set(
                (int)(maxx + Math.Max(0, motionX)),
                (int)(y2 + Math.Max(0, motionY)),
                (int)(maxz + Math.Max(0, motionZ))
            );


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
        /// If given cuboidf collides with the terrain, returns the collision box it collides with. By default also checks if the cuboid is merely touching the terrain, set alsoCheckTouch to disable that.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="ecollisionBoxes"></param>
        /// <param name="collBoxCount"></param>
        /// <param name="pos"></param>
        /// <param name="alsoCheckTouch"></param>
        /// <returns></returns>
        public Cuboidd GetCollidingCollisionBox(IBlockAccessor blockAccessor, Cuboidf[] ecollisionBoxes, int collBoxCount, Vec3d pos, bool alsoCheckTouch = true)
        {
            for (int j = 0; j < collBoxCount; j++)
            {
                var entityBoxRel = ecollisionBoxes[j];
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
                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int z = minZ; z <= maxZ; z++)
                        {
                            Block block = blockAccessor.GetMostSolidBlock(x, y, z);
                            blockPos.Set(x, y, z);
                            blockPosVec.Set(x, y, z);

                            Cuboidf[] collisionBoxes = block.GetCollisionBoxes(blockAccessor, blockPos);

                            if (collisionBoxes == null) continue;
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
            }

            return null;
        }


    }
}
