using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class EntityBehaviorPassivePhysics : EntityBehavior
    {
        float accumulator;
        Vec3d outposition = new Vec3d();

        /// <summary>
        /// The amount of drag while travelling through water.
        /// </summary>
        double waterDragValue = GlobalConstants.WaterDrag;

        /// <summary>
        /// The amount of drag while travelling through the air.
        /// </summary>
        double airDragValue = GlobalConstants.AirDragAlways;

        /// <summary>
        /// The amount of drag while travelling on the ground.
        /// </summary>
        double groundDragFactor = 0.7f;

        /// <summary>
        /// The amount of gravity applied per tick to this entity.
        /// </summary>
        double gravityPerSecond = GlobalConstants.GravityPerSecond;


        public EntityBehaviorPassivePhysics(Entity entity) : base(entity)
        {

        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            waterDragValue = 1 - (1 - GlobalConstants.WaterDrag) * (float)attributes["waterDragFactor"].AsDouble(1);

            airDragValue = 1 - (1 - GlobalConstants.AirDragAlways) * (float)attributes["airDragFallingFactor"].AsDouble(1);

            groundDragFactor = 0.3 * (float)attributes["groundDragFactor"].AsDouble(1);

            gravityPerSecond = GlobalConstants.GravityPerSecond * (float)attributes["gravityFactor"].AsDouble(1f);            
        }


        public override void OnGameTick(float deltaTime)
        {
            EntityPos pos = entity.Pos;
            if (entity.World is IServerWorldAccessor)
            {
                pos = entity.ServerPos;
            }

            accumulator += deltaTime;

            if (accumulator > 1)
            {
                accumulator = 1;
            }

            float dt2 = 1f / 75;

            while (accumulator >= dt2)
            {
                DoPhysics(dt2, pos);
                accumulator -= dt2;
            }
        }

        /// <summary>
        /// Performs the physics on the specified entity.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pos"></param>
        public void DoPhysics(float dt, EntityPos pos)
        {
            Vec3d motionBefore = pos.Motion.Clone();
            bool feetInLiquidBefore = entity.FeetInLiquid;
            bool onGroundBefore = entity.OnGround;
            bool swimmingBefore = entity.Swimming;
            bool onCollidedBefore = entity.Collided;

            Block belowBlock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y - 0.05f), (int)pos.Z);

            // On ground drag
            if (entity.OnGround) {
                if (!entity.FeetInLiquid)
                {
                    pos.Motion.X *= (1 - groundDragFactor * belowBlock.DragMultiplier);
                    pos.Motion.Z *= (1 - groundDragFactor * belowBlock.DragMultiplier);
                }
            }
            
            // Water or air drag
            if (entity.FeetInLiquid || entity.Swimming)
            {
                pos.Motion *= (float)Math.Pow(waterDragValue, dt * 33);
            } else
            {
                pos.Motion *= (float)Math.Pow(airDragValue, dt * 33);
            }

            Block inblock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y), (int)pos.Z);

            if (entity.FeetInLiquid)
            {
                string lastcodepart = inblock.LastCodePart(1);
                if (lastcodepart != null)
                {
                    Vec3i normali = Cardinal.FromInitial(lastcodepart)?.Normali;
                    
                    if (normali != null)
                    {
                        float pushstrength = 0.0003f * 1000f / Math.Max(500, entity.MaterialDensity);

                        pos.Motion.Add(
                            normali.X * pushstrength,
                            0,
                            normali.Z * pushstrength
                        );
                    }
                    else
                    {
                        if (lastcodepart == "d")
                        {
                            pos.Motion.Add(0, -0.002f, 0);
                        }
                    }
                }
            }
            

            // Gravity
            if (pos.Y > -100 && entity.ApplyGravity)
            {
                float fact = 1;
                if (entity.Swimming)
                {
                    Block aboveblock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y + entity.SwimmingOffsetY + 1), (int)pos.Z);

                    float swimmingDepth = Math.Min(
                        1, 
                        1 - (float)(pos.Y + entity.SwimmingOffsetY) + (int)(pos.Y + entity.SwimmingOffsetY) + (aboveblock.IsLiquid() ? 1 : 0) - (1 - inblock.LiquidLevel / 7f)
                    );

                    float boyancy = GameMath.Clamp(entity.MaterialDensity / inblock.MaterialDensity - 1, -0.1f, 0.2f);
                    
                    fact = boyancy * (1.5f * swimmingDepth + (1 - swimmingDepth) / 70f);

                    // At very shallow waters, items heavery than water do funny stuff because fact becomes <0
                    if (entity.MaterialDensity > inblock.MaterialDensity)
                    {
                        fact = Math.Max(0, fact);
                    }
                }

                pos.Motion.Y -= fact * (gravityPerSecond * dt + Math.Max(0, -0.015f * pos.Motion.Y));
            }

           
            Vec3d nextPosition = pos.XYZ + pos.Motion;

            bool falling = pos.Motion.Y < 0;

            entity.World.CollisionTester.ApplyTerrainCollision(entity, pos, ref outposition, true);

            


            if (entity.World.BlockAccessor.IsNotTraversable((int)(pos.X + pos.Motion.X), (int)pos.Y, (int)pos.Z))
            {
                outposition.X = pos.X;
            }
            if (entity.World.BlockAccessor.IsNotTraversable((int)pos.X, (int)(pos.Y + pos.Motion.Y), (int)pos.Z))
            {
                outposition.Y = pos.Y;
            }
            if (entity.World.BlockAccessor.IsNotTraversable((int)pos.X, (int)pos.Y, (int)(pos.Z + pos.Motion.Z)))
            {
                outposition.Z = pos.Z;
            }

            entity.OnGround = entity.CollidedVertically && falling;
            pos.SetPos(outposition);

           
            if ((nextPosition.X < outposition.X && pos.Motion.X < 0) || (nextPosition.X > outposition.X && pos.Motion.X > 0))
            {
                pos.Motion.X = 0;
            }

            if ((nextPosition.Y < outposition.Y && pos.Motion.Y < 0) || (nextPosition.Y > outposition.Y && pos.Motion.Y > 0))
            {
                pos.Motion.Y = 0;
            }

            if ((nextPosition.Z < outposition.Z && pos.Motion.Z < 0) || (nextPosition.Z > outposition.Z && pos.Motion.Z > 0))
            {
                pos.Motion.Z = 0;
            }

       

            Block block = entity.World.BlockAccessor.GetBlock(pos.AsBlockPos);
            entity.FeetInLiquid = block.MatterState == EnumMatterState.Liquid;

            block = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y + entity.SwimmingOffsetY), (int)pos.Z);
            entity.Swimming = block.IsLiquid();

            if (!onCollidedBefore && entity.Collided)
            {
                entity.OnCollided();
            }

            if (!onGroundBefore && entity.OnGround)
            {
                entity.OnFallToGround(motionBefore.Y);
            }
            if ((!entity.Swimming && !feetInLiquidBefore && entity.FeetInLiquid) || (!entity.FeetInLiquid && !swimmingBefore && entity.Swimming))
            {
                entity.OnCollideWithLiquid();
            }
            if (!falling || entity.OnGround)
            {
                entity.PositionBeforeFalling.Set(outposition);
            }

            if (GlobalConstants.OutsideWorld(pos.X, pos.Y, pos.Z, entity.World.BlockAccessor))
            {
                entity.DespawnReason = new EntityDespawnReason() { reason = EnumDespawnReason.Death, damageSourceForDeath = new DamageSource() { Source = EnumDamageSource.Fall } };
                return;
            }

            Cuboidd entityBox = entity.World.CollisionTester.entityBox;
            for (int y = (int)entityBox.Y1; y <= (int)entityBox.Y2; y++)
            {
                for (int x = (int)entityBox.X1; x <= (int)entityBox.X2; x++)
                {
                    for (int z = (int)entityBox.Z1; z <= (int)entityBox.Z2; z++)
                    {
                        entity.World.CollisionTester.tmpPos.Set(x, y, z);
                        entity.World.CollisionTester.tempCuboid.Set(x, y, z, x + 1, y + 1, z + 1);
                        if (entity.World.CollisionTester.tempCuboid.IntersectsOrTouches(entityBox))
                        {
                            entity.World.BlockAccessor.GetBlock(x, y, z).OnEntityInside(entity.World, entity, entity.World.CollisionTester.tmpPos);
                        }
                    }
                }
            }
        }


        public override string PropertyName()
        {
            return "passiveentityphysics";
        }
    }
}
