using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public enum EnumVelocityState
    {
        /// <summary>
        /// Currently falling
        /// </summary>
        Moving,        
        /// <summary>
        /// Is now outside the world (x/y/z below -30 or x/z above mapsize + 30)
        /// </summary>
        OutsideWorld,   
        /// <summary>
        /// Was falling and has now collided with the terrain
        /// </summary>
        Collided
    }

    [Flags]
    public enum EnumCollideFlags
    {
        CollideX = 1,
        CollideY = 2,
        CollideZ = 4
    }

    public class ParticlePhysics
    {
        public IBlockAccessor BlockAccess;


        // 30 FPS physics for particle physics simulation
        public float PhysicsTickTime = 33f / 1000f;
        public const float AsyncSpawnTime = 33f / 1000f;

        Cuboidd particleCollBox = new Cuboidd();
        Cuboidd blockCollBox = new Cuboidd();
        BlockPos tmpPos = new BlockPos();

        public ParticlePhysics(IBlockAccessor blockAccess)
        {
            this.BlockAccess = blockAccess;
        }

        public Vec3f CollisionStrength(Vec3f velocitybefore, Vec3f velocitynow, float gravityStrength, float deltatime)
        {
            velocitybefore.Y -= gravityStrength * deltatime;

            return new Vec3f(
                velocitybefore.X - velocitynow.X,
                velocitybefore.Y - velocitynow.Y,
                velocitybefore.Z - velocitynow.Z
            );
        }

        public CachedCuboidList CollisionBoxList = new CachedCuboidList();

        public void HandleBoyancy(Vec3d pos, Vec3f velocity, bool boyant, float gravityStrength, float deltatime, float height)
        {
            int xPrev = (int)pos.X;
            int yPrev = (int)pos.Y;
            int zPrev = (int)pos.Z;

            tmpPos.Set(xPrev, yPrev, zPrev);
            Block block = BlockAccess.GetBlock(tmpPos, BlockLayersAccess.Fluid);
            Block prevBlock = block;

            if (boyant)
            {
                if (block.IsLiquid())
                {
                    tmpPos.Set(xPrev, (int)(pos.Y + 1), zPrev);
                    block = BlockAccess.GetBlock(tmpPos, BlockLayersAccess.Fluid);

                    float waterY = (int)pos.Y + prevBlock.LiquidLevel / 8f + (block.IsLiquid() ? 9 / 8f : 0);
                    float bottomSubmergedness = waterY - (float)pos.Y;

                    float swimlineSubmergedness = GameMath.Clamp(bottomSubmergedness + height, 0, 1);

                    float boyancyStrength = GameMath.Clamp(9 * swimlineSubmergedness, -1.25f, 1.25f); // was 3* before. Dunno why it has to be 9* now

                    velocity.Y += gravityStrength * deltatime * boyancyStrength;

                    float waterDrag = (float)GameMath.Clamp(30 * Math.Abs(velocity.Y) - 0.02f, 1, 1.25f);

                    velocity.Y /= waterDrag;
                    velocity.X *= 0.99f;
                    velocity.Z *= 0.99f;

                    if (prevBlock.PushVector != null && swimlineSubmergedness >= 0)
                    {
                        float factor = deltatime / (33f / 1000f);
                        velocity.Add(
                            (float)prevBlock.PushVector.X * 15 * factor,
                            (float)prevBlock.PushVector.Y * 15 * factor,
                            (float)prevBlock.PushVector.Z * 15 * factor
                        );
                    }

                }

            }
            else
            {
                if (block.PushVector != null)
                {
                    velocity.Add(
                        (float)block.PushVector.X * 30 * deltatime,
                        (float)block.PushVector.Y * 30 * deltatime,
                        (float)block.PushVector.Z * 30 * deltatime
                    );
                }

            }
        }

        public float MotionCap = 2;
        BlockPos minPos = new BlockPos();
        BlockPos maxPos = new BlockPos();

        /// <summary>
        /// Updates the velocity vector according to the amount of passed time, gravity and terrain collision.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="motion"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public EnumCollideFlags UpdateMotion(Vec3d pos, Vec3f motion, float size)
        {
            particleCollBox.Set(
                pos.X - size / 2, pos.Y - 0/2, pos.Z - size / 2, 
                pos.X + size / 2, pos.Y + size/2, pos.Z + size / 2
            );

            motion.X = GameMath.Clamp(motion.X, -MotionCap, MotionCap);
            motion.Y = GameMath.Clamp(motion.Y, -MotionCap, MotionCap);
            motion.Z = GameMath.Clamp(motion.Z, -MotionCap, MotionCap);

            EnumCollideFlags flags=0;

            minPos.SetAndCorrectDimension(
                (int)(particleCollBox.X1 + Math.Min(0, motion.X)),
                (int)(particleCollBox.Y1 + Math.Min(0, motion.Y) - 1), // -1 for the extra high collision box of fences
                (int)(particleCollBox.Z1 + Math.Min(0, motion.Z))
            );

            maxPos.SetAndCorrectDimension(
                (int)(particleCollBox.X2 + Math.Max(0, motion.X)),
                (int)(particleCollBox.Y2 + Math.Max(0, motion.Y)),
                (int)(particleCollBox.Z2 + Math.Max(0, motion.Z))
            );

            tmpPos.dimension = minPos.dimension;
            particleCollBox.Y1 %= BlockPos.DimensionBoundary;
            particleCollBox.Y2 %= BlockPos.DimensionBoundary;

            CollisionBoxList.Clear();
            BlockAccess.WalkBlocks(minPos, maxPos, (cblock, x, y, z) => {
                Cuboidf[] collisionBoxes = cblock.GetParticleCollisionBoxes(BlockAccess, tmpPos.Set(x, y, z));
                if (collisionBoxes != null)
                {
                    CollisionBoxList.Add(collisionBoxes, x, y, z, cblock);
                }

            }, false);


            //  Y - Collision (Vertical)
            EnumPushDirection pushDirection = EnumPushDirection.None;

            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                blockCollBox = CollisionBoxList.cuboids[i];

                motion.Y = (float)blockCollBox.pushOutY(particleCollBox, motion.Y, ref pushDirection);

                if (pushDirection != EnumPushDirection.None)
                {
                    flags |= EnumCollideFlags.CollideY;
                }
            }


            particleCollBox.Translate(0, motion.Y, 0);


            // X - Collision (Horizontal)
            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                blockCollBox = CollisionBoxList.cuboids[i];
                
                motion.X = (float)blockCollBox.pushOutX(particleCollBox, motion.X, ref pushDirection);

                if (pushDirection != EnumPushDirection.None)
                {
                    flags |= EnumCollideFlags.CollideX;
                }
            }

            particleCollBox.Translate(motion.X, 0, 0);


            // Z - Collision (Horizontal)
            for (int i = 0; i < CollisionBoxList.Count; i++)
            {
                blockCollBox = CollisionBoxList.cuboids[i];
                
                motion.Z = (float)blockCollBox.pushOutZ(particleCollBox, motion.Z, ref pushDirection);

                if (pushDirection != EnumPushDirection.None)
                {
                    flags |= EnumCollideFlags.CollideZ;
                }
            }


            return flags;
        }

    }
}
