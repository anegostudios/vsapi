using System;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class EntityBehaviorPassivePhysics : EntityBehavior, IRenderer
    {
        float accumulator;
        Vec3d outposition = new Vec3d();
        Vec3d prevPos = new Vec3d();

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

        
        protected bool duringRenderFrame;
        public double RenderOrder => 0;
        public int RenderRange => 9999;
        ICoreClientAPI capi;

        
        public override void OnEntityDespawn(EntityDespawnReason despawn)
        {
            (entity.World.Api as ICoreClientAPI)?.Event.UnregisterRenderer(this, EnumRenderStage.Before);
        }

        public EntityBehaviorPassivePhysics(Entity entity) : base(entity)
        {
            
        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            waterDragValue = 1 - (1 - GlobalConstants.WaterDrag) * (float)attributes["waterDragFactor"].AsDouble(1);

            airDragValue = 1 - (1 - GlobalConstants.AirDragAlways) * (float)attributes["airDragFallingFactor"].AsDouble(1);

            groundDragFactor = 0.3 * (float)attributes["groundDragFactor"].AsDouble(1);

            gravityPerSecond = GlobalConstants.GravityPerSecond * (float)attributes["gravityFactor"].AsDouble(1f);

            if (entity.World.Side == EnumAppSide.Client)
            {
                capi = (entity.World.Api as ICoreClientAPI);
                duringRenderFrame = true;
                capi.Event.RegisterRenderer(this, EnumRenderStage.Before, "passivephysics");
            }
        }

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            if (capi.IsGamePaused) return;

            onPhysicsTick(deltaTime);
        }


        public override void OnGameTick(float deltaTime)
        {
            if (!duringRenderFrame)
            {
                onPhysicsTick(deltaTime);
            }
        }


        public void onPhysicsTick(float deltaTime)
        {
            EntityPos pos = entity.LocalPos;

            accumulator += deltaTime;

            if (accumulator > 1)
            {
                accumulator = 1;
            }

            while (accumulator >= GlobalConstants.PhysicsFrameTime)
            {
                prevPos.Set(pos.X, pos.Y, pos.Z);
                DoPhysics(GlobalConstants.PhysicsFrameTime, pos);
                accumulator -= GlobalConstants.PhysicsFrameTime;
            }

            entity.PhysicsUpdateWatcher?.Invoke(accumulator, prevPos);
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
            Block aboveblock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y + 1), (int)pos.Z);

            if (entity.FeetInLiquid)
            {
                Vec3d pushvec = inblock.PushVector;
                if (pushvec != null)
                {
                    float pushstrength = 0.3f * 1000f / GameMath.Clamp(entity.MaterialDensity, 750, 2500);

                    pos.Motion.Add(
                        pushvec.X * pushstrength,
                        pushvec.Y * pushstrength,
                        pushvec.Z * pushstrength
                    );
                }
            }
            

            // Gravity
            if (pos.Y > -100 && entity.ApplyGravity)
            {
                double gravStrength = gravityPerSecond * dt + Math.Max(0, -0.015f * pos.Motion.Y);
                if (entity.Swimming)
                {

                    // above 0 => floats
                    // below 0 => sinks
                    float baseboyancy = GameMath.Clamp(1 - entity.MaterialDensity / inblock.MaterialDensity, -1, 1);

                    float waterY = (int)pos.Y + inblock.LiquidLevel / 8f + (aboveblock.IsLiquid() ? 9/8f : 0);
                    
                    float bottomSubmergedness = waterY - (float)pos.Y;
                    
                    // 0 = at swim line
                    // 1 = completely submerged
                    float swimlineSubmergedness = GameMath.Clamp(bottomSubmergedness - (entity.CollisionBox.Y2 - (float)entity.SwimmingOffsetY), 0, 1);

                    double boyancyStrength = GameMath.Clamp(60 * baseboyancy * swimlineSubmergedness, -1.5f, 1.5f);

                    double waterDrag = GameMath.Clamp(100 * Math.Abs(pos.Motion.Y) - 0.02f, 1, 1.25f);

                    pos.Motion.Y += gravStrength * (boyancyStrength - 1);
                    pos.Motion.Y /= waterDrag;

                } else
                {
                    pos.Motion.Y -= gravStrength;
                }   
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

       

            Block block = entity.World.BlockAccessor.GetBlock(pos.XInt, pos.YInt, pos.ZInt);

            entity.FeetInLiquid = block.MatterState == EnumMatterState.Liquid;

            if (entity.FeetInLiquid)
            {
                float waterY = (int)pos.Y + block.LiquidLevel / 8f + (aboveblock.IsLiquid() ? 9 / 8f : 0);
                float bottomSubmergedness = waterY - (float)pos.Y;

                // 0 = at swim line
                // 1 = completely submerged
                float swimlineSubmergedness = bottomSubmergedness - (entity.CollisionBox.Y2 - (float)entity.SwimmingOffsetY);

                entity.Swimming = swimlineSubmergedness > 0;
            } else
            {
                entity.Swimming = false;
            }

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
        public void Dispose()
        {

        }

    }
}
