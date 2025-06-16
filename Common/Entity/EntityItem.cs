using System;
using System.IO;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class EntityItemSlot : DummySlot
    {
        public EntityItem Ei;

        public EntityItemSlot(EntityItem ei)
        {
            this.Ei = ei;
        }
    }

    public class EntityItem : Entity
    {
        public EntityItemSlot Slot;
        
        public long itemSpawnedMilliseconds;

        /// <summary>
        /// The itemstack attached to this Item Entity.
        /// </summary>
        public ItemStack Itemstack
        {
            get { return WatchedAttributes.GetItemstack("itemstack"); }
            set { WatchedAttributes.SetItemstack("itemstack", value); Slot.Itemstack = value; }
        }

        /// <summary>
        /// The UID of the player that dropped this itemstack.
        /// </summary>
        public string ByPlayerUid
        {
            get { return WatchedAttributes.GetString("byPlayerUid"); }
            set { WatchedAttributes.SetString("byPlayerUid", value); }
        }

        /// <summary>
        /// Returns the material density of the item.
        /// </summary>
        public override float MaterialDensity
        {
            get { return (Slot.Itemstack?.Collectible != null) ? Slot.Itemstack.Collectible.MaterialDensity : 2000; }
        }

        /// <summary>
        /// Whether or not the EntityItem is interactable.
        /// </summary>
        public override bool IsInteractable
        {
            get { return false; }
        }

        /// <summary>
        /// Get the HSV colors for the lighting.
        /// </summary>
        public override byte[] LightHsv
        {
            get
            {
                return Slot.Itemstack?.Collectible?.GetLightHsv(World.BlockAccessor, null, Slot.Itemstack);
            }
        }



        public EntityItem() : base(GlobalConstants.DefaultSimulationRange * 3 / 4)   // we call a parameterised constructor instead of the parameterless base constructor
        {
            Stats = new EntityStats(this);
            Slot = new EntityItemSlot(this);
        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long chunkindex3d)
        {
            this.World = api.World;
            this.Api = api;
            this.Properties = properties;
            this.Class = properties.Class;
            this.InChunkIndex3d = chunkindex3d;

            if (Itemstack == null || Itemstack.StackSize == 0 || !Itemstack.ResolveBlockOrItem(World))
            {
                Die();
                this.Itemstack = null;
                return;
            }

            // ----- The following code is a reduced version of Entity.Initialize() -----

            alive = WatchedAttributes.GetInt("entityDead", 0) == 0;

            WatchedAttributes.RegisterModifiedListener("onFire", updateOnFire);

            if (Properties.CollisionBoxSize != null || properties.SelectionBoxSize != null)
            {
                updateColSelBoxes();
            }

            DoInitialActiveCheck(api);

            this.Properties.Initialize(this, api);

            LocalEyePos.Y = Properties.EyeHeight;

            TriggerOnInitialized();

            // ----- The following code is specific to Entity Item -----

            // If attribute was modified and resent to client, make sure we still have the resolved thing in memory
            WatchedAttributes.RegisterModifiedListener("itemstack", () => {
                if (Itemstack != null && Itemstack.Collectible == null) Itemstack.ResolveBlockOrItem(World);
                Slot.Itemstack = Itemstack;
            });

            JsonObject gravityFactor = Itemstack.Collectible.Attributes?["gravityFactor"];
            if (gravityFactor?.Exists == true)
            {
                WatchedAttributes.SetDouble("gravityFactor", gravityFactor.AsDouble(1));
            }
            JsonObject airdragFactor = Itemstack.Collectible.Attributes?["airDragFactor"];
            if (airdragFactor?.Exists == true)
            {
                WatchedAttributes.SetDouble("airDragFactor", airdragFactor.AsDouble(1));
            }

            itemSpawnedMilliseconds = World.ElapsedMilliseconds;
            Swimming = FeetInLiquid = World.BlockAccessor.GetBlock(Pos.AsBlockPos, BlockLayersAccess.Fluid).IsLiquid();

            tmpPos.Set(Pos.XInt, Pos.YInt, Pos.ZInt);
            windLoss = World.BlockAccessor.GetDistanceToRainFall(tmpPos) / 4f;
        }

        long lastPlayedSizzlesTotalMs;
        float getWindSpeedAccum = 0.25f;
        Vec3d windSpeed = new Vec3d();
        BlockPos tmpPos = new BlockPos();
        float windLoss;


        public override void OnGameTick(float dt)
        {
            if (World.Side == EnumAppSide.Client)
            {
                try
                {
                    base.OnGameTick(dt);
                }
                catch (Exception e)
                {
                    if (World == null) throw new NullReferenceException("'World' was null for EntityItem; entity is " + (alive ? "alive" : "post-lifetime"));
                    Api.Logger.Error("Erroring EntityItem tick: please report this as a bug!");
                    Api.Logger.Error(e);
                }
            }
            else
            {
                // simplified server tick
                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    behavior.OnGameTick(dt);
                }

                if (InLava) Ignite();

                if (IsOnFire)
                {
                    Block fluidBlock = World.BlockAccessor.GetBlock(Pos.AsBlockPos, BlockLayersAccess.Fluid);
                    if (fluidBlock.IsLiquid() && fluidBlock.LiquidCode != "lava" || World.ElapsedMilliseconds - OnFireBeginTotalMs > 12000)
                    {
                        IsOnFire = false;
                    }
                    else
                    {
                        ApplyFireDamage(dt);

                        if (!alive && InLava)
                        {
                            DieInLava();
                        }
                    }
                }
            }

            if (!this.Alive) return;

            if (Itemstack != null)
            {
                if (!Collided && !Swimming && World.Side == EnumAppSide.Server)
                {
                    getWindSpeedAccum += dt;
                    if (getWindSpeedAccum > 0.25)
                    {
                        getWindSpeedAccum = 0;
                        tmpPos.Set(Pos.XInt, Pos.YInt, Pos.ZInt);
                        windSpeed = World.BlockAccessor.GetWindSpeedAt(tmpPos);

                        windSpeed.X = Math.Max(0, Math.Abs(windSpeed.X) - windLoss) * Math.Sign(windSpeed.X);
                        windSpeed.Y = Math.Max(0, Math.Abs(windSpeed.Y) - windLoss) * Math.Sign(windSpeed.Y);
                        windSpeed.Z = Math.Max(0, Math.Abs(windSpeed.Z) - windLoss) * Math.Sign(windSpeed.Z);
                    }

                    float fac = GameMath.Clamp(1000f / Itemstack.Collectible.MaterialDensity, 1f, 10);

                    SidedPos.Motion.X += windSpeed.X / 1000.0 * fac * GameMath.Clamp(1f / (1 + Math.Abs(SidedPos.Motion.X)), 0, 1);
                    SidedPos.Motion.Y += windSpeed.Y / 1000.0 * fac * GameMath.Clamp(1f / (1 + Math.Abs(SidedPos.Motion.Y)), 0, 1);
                    SidedPos.Motion.Z += windSpeed.Z / 1000.0 * fac * GameMath.Clamp(1f / (1 + Math.Abs(SidedPos.Motion.Z)), 0, 1);
                }

                Itemstack.Collectible.OnGroundIdle(this);

                if (FeetInLiquid && !InLava)
                {
                    float temp = Itemstack.Collectible.GetTemperature(World, Itemstack);

                    if (temp > 20)
                    {
                        Itemstack.Collectible.SetTemperature(World, Itemstack, Math.Max(20, temp - 5));

                        if (temp > 90)
                        {
                            double width = SelectionBox.XSize;
                            SplashParticleProps.BasePos.Set(Pos.X - width / 2, Pos.Y - 0.75, Pos.Z - width / 2);
                            SplashParticleProps.AddVelocity.Set(0, 0, 0);
                            SplashParticleProps.QuantityMul = 0.1f;
                            World.SpawnParticles(SplashParticleProps);
                        }

                        if (temp > 200 && World.Side == EnumAppSide.Client && World.ElapsedMilliseconds - lastPlayedSizzlesTotalMs > 10000)
                        {
                            World.PlaySoundAt(new AssetLocation("sounds/sizzle"), this, null);
                            lastPlayedSizzlesTotalMs = World.ElapsedMilliseconds;
                        }
                    }
                }

            }
            else
            {
                Die();
            }

            World.FrameProfiler.Mark("entity-tick-droppeditems");
        }

        public override void Ignite()
        {
            var stack = this.Itemstack;
            if (InLava || (stack != null && stack.Collectible.CombustibleProps != null && (stack.Collectible.CombustibleProps.MeltingPoint < 700 || stack.Collectible.CombustibleProps.BurnTemperature > 0)))
            {
                base.Ignite();
            }
        }

        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            if (SidedProperties == null) return;
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnEntityDespawn(despawn);
            }

            WatchedAttributes.OnModified.Clear();
        }

        public override void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            // no animations
        }

        public override void UpdateDebugAttributes()
        {
            // no animations
        }

        public override void StartAnimation(string code)
        {
        }

        public override void StopAnimation(string code)
        {
        }

        public override void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            if (!Alive) return;

            Alive = false;

            DespawnReason = new EntityDespawnData()
            {
                Reason = reason,
                DamageSourceForDeath = damageSourceForDeath
            };
        }


        /// <summary>
        /// Builds and spawns an EntityItem from a provided ItemStack.
        /// </summary>
        /// <param name="itemstack">The contents of the EntityItem</param>
        /// <param name="position">The position of the EntityItem</param>
        /// <param name="velocity">The velocity of the EntityItem</param>
        /// <param name="world">The world the EntityItems preside in.</param>
        /// <returns>A freshly baked EntityItem to introduce to the world.</returns>
        public static EntityItem FromItemstack(ItemStack itemstack, Vec3d position, Vec3d velocity, IWorldAccessor world)
        {
            EntityItem item = new EntityItem();
            item.Code = GlobalConstants.EntityItemTypeCode;
            item.SimulationRange = (int)(0.75f * GlobalConstants.DefaultSimulationRange);
            item.Itemstack = itemstack;

            item.ServerPos.SetPosWithDimension(position);

            if (velocity == null)
            {
                velocity = new Vec3d((float)world.Rand.NextDouble() * 0.1f - 0.05f, (float)world.Rand.NextDouble() * 0.1f - 0.05f, (float)world.Rand.NextDouble() * 0.1f - 0.05f);
            }

            item.ServerPos.Motion = velocity;


            item.Pos.SetFrom(item.ServerPos);

            return item;
        }


        public override bool CanCollect(Entity byEntity)
        {
            return Alive && World.ElapsedMilliseconds - itemSpawnedMilliseconds > 1000;
        }

        public override ItemStack OnCollected(Entity byEntity)
        {
            return Slot.Itemstack;
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            return false;
        }

        float fireDamage;

        public override bool ReceiveDamage(DamageSource damageSource, float damage)
        {
            if (damageSource.Source == EnumDamageSource.Internal && damageSource.Type == EnumDamageType.Fire) fireDamage += damage;
            if (fireDamage > 4) Die();

            return base.ReceiveDamage(damageSource, damage);
        }

        public override void FromBytes(BinaryReader reader, bool forClient)
        {
            base.FromBytes(reader, forClient);

            if (Itemstack != null)
            {
                Slot.Itemstack = Itemstack;
            }

            if (World != null)
            {
                if (Slot.Itemstack?.ResolveBlockOrItem(World) != true)
                {
                    this.Itemstack = null;
                    Die();
                }
            }
        }

        public override double SwimmingOffsetY
        {
            get { return base.SwimmingOffsetY; }
        }
    }
}
