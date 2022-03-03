using System;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class EntityBehaviorPassivePhysics : EntityBehavior, IRenderer
    {
        float accumulator;
        Vec3d outposition = new Vec3d();
        Vec3d prevPos = new Vec3d();
        Vec3d moveDelta = new Vec3d();
        Vec3d nextPosition = new Vec3d();

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

        public float clientPhysicsTickTimeThreshold = 0f;
        float accum = 0;

        public float collisionYExtra = 1f;

        /// <summary>
        /// If set, will test for entity collision every tick (expensive)
        /// </summary>
        public Action<float> OnPhysicsTickCallback;
        
        public override void OnEntityDespawn(EntityDespawnReason despawn)
        {
            (entity.World.Api as ICoreClientAPI)?.Event.UnregisterRenderer(this, EnumRenderStage.Before);
            Dispose();
        }

        public EntityBehaviorPassivePhysics(Entity entity) : base(entity)
        {
            
        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            waterDragValue = 1 - (1 - GlobalConstants.WaterDrag) * (float)attributes["waterDragFactor"].AsDouble(1);

            double airDrag = attributes["airDragFactor"].Exists ? attributes["airDragFactor"].AsDouble(1) : attributes["airDragFallingFactor"].AsDouble(1); // airDragFallingFactor is pre1.15
            airDragValue = 1 - (1 - GlobalConstants.AirDragAlways) * airDrag;

            if (entity.WatchedAttributes.HasAttribute("airDragFactor"))
            {
                airDragValue = 1 - (1 - GlobalConstants.AirDragAlways) * (float)entity.WatchedAttributes.GetDouble("airDragFactor");
            }

            groundDragFactor = 0.3 * (float)attributes["groundDragFactor"].AsDouble(1);

            gravityPerSecond = GlobalConstants.GravityPerSecond * (float)attributes["gravityFactor"].AsDouble(1f);
            if (entity.WatchedAttributes.HasAttribute("gravityFactor"))
            {
                gravityPerSecond = GlobalConstants.GravityPerSecond * (float)entity.WatchedAttributes.GetDouble("gravityFactor");
            }

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

            // Graceful degradation of the simulation quality instead of heavily lagging the game
            accum += deltaTime;
            if (accum > clientPhysicsTickTimeThreshold)
            {
                onPhysicsTick(deltaTime);
                accum = 0f;
            }
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
            if (entity.State == EnumEntityState.Inactive)
            {
                return;
            }

            EntityPos pos = entity.SidedPos;

            accumulator += deltaTime;

            if (accumulator > 0.4f)
            {
                accumulator = 0.4f;
            }

            float sliceTime = GlobalConstants.PhysicsFrameTime;

            // Dynamically adapt physics simulation accuracy based on the velocity
            double velo = pos.Motion.Length();
            sliceTime /= GameMath.Clamp((float)velo * 10, 1, 10);

            while (accumulator >= sliceTime)
            {
                prevPos.Set(pos.X, pos.Y, pos.Z);
                DoPhysics(sliceTime, pos);
                accumulator -= sliceTime;
            }

            entity.PhysicsUpdateWatcher?.Invoke(accumulator, prevPos);

            if (pos.Y < -100)
            {
                entity.Die();
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

            float dtFac = 60 * dt;

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
                    float pushstrength = 0.3f * 1000f / GameMath.Clamp(entity.MaterialDensity, 750, 2500) * dtFac;

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
                double gravStrength = gravityPerSecond / 60f * dtFac + Math.Max(0, -0.015f * pos.Motion.Y * dtFac);

                if (entity.Swimming)
                {
                    // above 0 => floats
                    // below 0 => sinks
                    float baseboyancy = GameMath.Clamp(1 - entity.MaterialDensity / inblock.MaterialDensity, -1, 1);

                    float waterY = (int)pos.Y + inblock.LiquidLevel / 8f + (aboveblock.IsLiquid() ? 9/8f : 0);

                    float bottomSubmergedness = waterY - (float)pos.Y;
                    
                    // 0 = at swim line
                    // 1 = completely submerged
                    float swimlineSubmergedness = GameMath.Clamp(bottomSubmergedness - (entity.SelectionBox.Y2 - (float)entity.SwimmingOffsetY), 0, 1);

                    double boyancyStrength = GameMath.Clamp(60 * baseboyancy * swimlineSubmergedness, -1.5f, 1.5f) - 1;

                    double waterDrag = GameMath.Clamp(100 * Math.Abs(pos.Motion.Y * dtFac) - 0.02f, 1, 1.25f);

                    pos.Motion.Y += gravStrength * boyancyStrength;
                    pos.Motion.Y /= waterDrag;
                }
                else
                {
                    pos.Motion.Y -= gravStrength;
                }   
            }


            moveDelta.Set(pos.Motion.X * dtFac, pos.Motion.Y * dtFac, pos.Motion.Z * dtFac);
            nextPosition.Set(pos.X + moveDelta.X, pos.Y + moveDelta.Y, pos.Z + moveDelta.Z);


            bool falling = pos.Motion.Y < 0;

            entity.World.CollisionTester.ApplyTerrainCollision(entity, pos, dtFac, ref outposition, 0, collisionYExtra);

            


            if (entity.World.BlockAccessor.IsNotTraversable((int)(pos.X + moveDelta.X), (int)pos.Y, (int)pos.Z))
            {
                outposition.X = pos.X;
            }
            if (entity.World.BlockAccessor.IsNotTraversable((int)pos.X, (int)(pos.Y + moveDelta.Y), (int)pos.Z))
            {
                outposition.Y = pos.Y;
            }
            if (entity.World.BlockAccessor.IsNotTraversable((int)pos.X, (int)pos.Y, (int)(pos.Z + moveDelta.Z)))
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
            entity.InLava = block.LiquidCode == "lava";
            if (entity.FeetInLiquid)
            {
                float waterY = (int)pos.Y + block.LiquidLevel / 8f + (aboveblock.IsLiquid() ? 9 / 8f : 0);
                float bottomSubmergedness = waterY - (float)pos.Y;

                // 0 = at swim line
                // 1 = completely submerged
                float swimlineSubmergedness = bottomSubmergedness - (entity.SelectionBox.Y2 - (float)entity.SwimmingOffsetY);

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

            OnPhysicsTickCallback?.Invoke(dtFac);
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
