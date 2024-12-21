using System;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common;

public class EntityBehaviorPassivePhysics : PhysicsBehaviorBase, IPhysicsTickable, IRemotePhysics
{
    public bool Ticking { get; set; } = true;

    // State info.
    private readonly Vec3d prevPos = new();
    private double motionBeforeY = 0;
    private bool feetInLiquidBefore = false;
    private bool onGroundBefore = false;
    private bool swimmingBefore = false;
    private bool collidedBefore = false;

    // Output of collision tester.
    protected Vec3d newPos = new();

    /// <summary>
    /// The amount of drag while travelling through water.
    /// </summary>
    private double waterDragValue = GlobalConstants.WaterDrag;
    /// <summary>
    /// The amount of drag while travelling through the air.
    /// </summary>
    private double airDragValue = GlobalConstants.AirDragAlways;
    /// <summary>
    /// The amount of drag while travelling on the ground.
    /// </summary>
    private double groundDragValue = 0.7f;
    /// <summary>
    /// The amount of gravity applied per tick to this entity.
    /// </summary>
    private double gravityPerSecond = GlobalConstants.GravityPerSecond;
    /// <summary>
    /// If set, will test for entity collision every tick (expensive)
    /// </summary>
    public Action<float> OnPhysicsTickCallback;

    private volatile int serverPhysicsTickDone = 0;

    public ref int FlagTickDone => ref serverPhysicsTickDone;

    public EntityBehaviorPassivePhysics(Entity entity) : base(entity)
    {
    }

    public void SetState(EntityPos pos)
    {
        prevPos.Set(pos);
        motionBeforeY = pos.Motion.Y;
        onGroundBefore = entity.OnGround;
        feetInLiquidBefore = entity.FeetInLiquid;
        swimmingBefore = entity.Swimming;
        collidedBefore = entity.Collided;
    }

    public virtual void SetProperties(JsonObject attributes)
    {
        waterDragValue = 1 - (1 - waterDragValue) * attributes["waterDragFactor"].AsDouble(1);

        JsonObject airDragFactor = attributes["airDragFactor"];
        double airDrag = airDragFactor.Exists ? airDragFactor.AsDouble(1) : attributes["airDragFallingFactor"].AsDouble(1);
        // airDragFallingFactor is pre1.15
        airDragValue = 1 - (1 - airDragValue) * airDrag;
        if (entity.WatchedAttributes.HasAttribute("airDragFactor"))
        {
            airDragValue = 1 - (1 - GlobalConstants.AirDragAlways) * (float)entity.WatchedAttributes.GetDouble("airDragFactor");
        }

        groundDragValue = 0.3 * attributes["groundDragFactor"].AsDouble(1);

        gravityPerSecond *= attributes["gravityFactor"].AsDouble(1);
        if (entity.WatchedAttributes.HasAttribute("gravityFactor"))
        {
            gravityPerSecond = GlobalConstants.GravityPerSecond * (float)entity.WatchedAttributes.GetDouble("gravityFactor");
        }
    }

    public override void Initialize(EntityProperties properties, JsonObject attributes)
    {
        Init();
        SetProperties(attributes);

        if (entity.Api is ICoreServerAPI esapi)
        {
            esapi.Server.AddPhysicsTickable(this);
        }
        else
        {
            EnumHandling handling = EnumHandling.Handled;
            OnReceivedServerPos(true, ref handling);
        }
    }

    public override void OnReceivedServerPos(bool isTeleport, ref EnumHandling handled)
    {
    }

    public void OnReceivedClientPos(int version)
    {
        if (version > previousVersion)
        {
            previousVersion = version;
            HandleRemotePhysics(clientInterval, true);
            return;
        }

        HandleRemotePhysics(clientInterval, false);
    }

    public void HandleRemotePhysics(float dt, bool isTeleport)
    {
        if (nPos == null)
        {
            nPos = new();
            nPos.Set(entity.ServerPos);
        }

        float dtFactor = dt * 60;

        lPos.SetFrom(nPos);
        nPos.Set(entity.ServerPos);

        if (isTeleport) lPos.SetFrom(nPos);

        lPos.Motion.X = (nPos.X - lPos.X) / dtFactor;
        lPos.Motion.Y = (nPos.Y - lPos.Y) / dtFactor;
        lPos.Motion.Z = (nPos.Z - lPos.Z) / dtFactor;

        if (lPos.Motion.Length() > 20) lPos.Motion.Set(0, 0, 0);

        // Set client motion.
        entity.Pos.Motion.Set(lPos.Motion);
        entity.ServerPos.Motion.Set(lPos.Motion);

        // Set pos for triggering events (interpolation overrides this).
        entity.Pos.SetFrom(entity.ServerPos);

        SetState(lPos);
        RemoteMotionAndCollision(lPos, dtFactor);
        ApplyTests(lPos);
    }

    public void RemoteMotionAndCollision(EntityPos pos, float dtFactor)
    {
        double gravityStrength = (gravityPerSecond / 60f * dtFactor) + Math.Max(0, -0.015f * pos.Motion.Y * dtFactor);
        pos.Motion.Y -= gravityStrength;
        collisionTester.ApplyTerrainCollision(entity, pos, dtFactor, ref newPos, 0, CollisionYExtra);
        bool falling = pos.Motion.Y < 0;
        entity.OnGround = entity.CollidedVertically && falling;
        pos.Motion.Y += gravityStrength;
        pos.SetPos(nPos);
    }

    public void MotionAndCollision(EntityPos pos, float dt)
    {
        float dtFactor = 60 * dt;

        // Apply drag from block below entity.
        if (onGroundBefore)
        {
            if (!feetInLiquidBefore)
            {
                Block belowBlock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y - 0.05f), (int)pos.Z, BlockLayersAccess.Solid);
                pos.Motion.X *= 1 - (groundDragValue * belowBlock.DragMultiplier);
                pos.Motion.Z *= 1 - (groundDragValue * belowBlock.DragMultiplier);
            }
        }

        // Apply water drag and push vector inside liquid, and air drag outside of liquid.
        Block insideFluid = null;
        if (feetInLiquidBefore || swimmingBefore)
        {
            pos.Motion *= Math.Pow(waterDragValue, dt * 33);

            insideFluid = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)pos.Y, (int)pos.Z, BlockLayersAccess.Fluid);

            if (feetInLiquidBefore)
            {
                Vec3d pushVector = insideFluid.PushVector;
                if (pushVector != null)
                {
                    float pushStrength = 0.3f * 1000f / GameMath.Clamp(entity.MaterialDensity, 750, 2500) * dtFactor;

                    pos.Motion.Add(
                        pushVector.X * pushStrength,
                        pushVector.Y * pushStrength,
                        pushVector.Z * pushStrength
                    );
                }
            }
        }
        else
        {
            pos.Motion *= (float)Math.Pow(airDragValue, dt * 33);
        }

        // Apply gravity.
        if (entity.ApplyGravity)
        {
            double gravityStrength = (gravityPerSecond / 60f * dtFactor) + Math.Max(0, -0.015f * pos.Motion.Y * dtFactor);

            if (entity.Swimming)
            {
                // Above 0 => floats.
                // Below 0 => sinks.
                float boyancy = GameMath.Clamp(1 - (entity.MaterialDensity / insideFluid.MaterialDensity), -1, 1);

                Block aboveFluid = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y + 1), (int)pos.Z, BlockLayersAccess.Fluid);
                float waterY = (int)pos.Y + (insideFluid.LiquidLevel / 8f) + (aboveFluid.IsLiquid() ? 9 / 8f : 0);

                // 0 => at swim line.
                // 1 => completely submerged.
                float submergedLevel = waterY - (float)pos.Y;
                float swimLineSubmergedness = GameMath.Clamp(submergedLevel - (entity.SelectionBox.Y2 - (float)entity.SwimmingOffsetY), 0, 1);

                double boyancyStrength = GameMath.Clamp(60 * boyancy * swimLineSubmergedness, -1.5f, 1.5f) - 1;

                double waterDrag = GameMath.Clamp((100 * Math.Abs(pos.Motion.Y * dtFactor)) - 0.02f, 1, 1.25f);

                pos.Motion.Y += gravityStrength * boyancyStrength;
                pos.Motion.Y /= waterDrag;
            }
            else
            {
                pos.Motion.Y -= gravityStrength;
            }
        }

        double nextX = (pos.Motion.X * dtFactor) + pos.X;
        double nextY = (pos.Motion.Y * dtFactor) + pos.Y;
        double nextZ = (pos.Motion.Z * dtFactor) + pos.Z;

        applyCollision(pos, dtFactor);
        

        // Clamp inside the world.
        if (entity.World.BlockAccessor.IsNotTraversable((int)nextX, (int)pos.Y, (int)pos.Z)) newPos.X = pos.X;
        if (entity.World.BlockAccessor.IsNotTraversable((int)pos.X, (int)nextY, (int)pos.Z)) newPos.Y = pos.Y;
        if (entity.World.BlockAccessor.IsNotTraversable((int)pos.X, (int)pos.Y, (int)nextZ)) newPos.Z = pos.Z;

        // Finally set position.
        pos.SetPos(newPos);
        

        // Stop motion if collided.
        if ((nextX < newPos.X && pos.Motion.X < 0) || (nextX > newPos.X && pos.Motion.X > 0)) pos.Motion.X = 0;
        if ((nextY < newPos.Y && pos.Motion.Y < 0) || (nextY > newPos.Y && pos.Motion.Y > 0)) pos.Motion.Y = 0;
        if ((nextZ < newPos.Z && pos.Motion.Z < 0) || (nextZ > newPos.Z && pos.Motion.Z > 0)) pos.Motion.Z = 0;
    }

    protected virtual void applyCollision(EntityPos pos, float dtFactor)
    {
        collisionTester.ApplyTerrainCollision(entity, pos, dtFactor, ref newPos, 0, CollisionYExtra);
    }

    public void ApplyTests(EntityPos pos)
    {
        bool falling = pos.Motion.Y <= 0;
        entity.OnGround = entity.CollidedVertically && falling;

        Block fluidBlock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)pos.Y, (int)pos.Z, BlockLayersAccess.Fluid);
        entity.FeetInLiquid = fluidBlock.MatterState == EnumMatterState.Liquid;
        entity.InLava = fluidBlock.LiquidCode == "lava";

        if (entity.FeetInLiquid)
        {
            Block aboveBlockFluid = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y + 1), (int)pos.Z, BlockLayersAccess.Fluid);
            float waterY = (int)pos.Y + (fluidBlock.LiquidLevel / 8f) + (aboveBlockFluid.IsLiquid() ? 9 / 8f : 0);
            float submergedLevel = waterY - (float)pos.Y;
            float swimlineSubmergedness = submergedLevel - (entity.SelectionBox.Y2 - (float)entity.SwimmingOffsetY);
            entity.Swimming = swimlineSubmergedness > 0;
        }
        else
        {
            entity.Swimming = false;
        }

        if (!onGroundBefore && entity.OnGround)
        {
            entity.OnFallToGround(motionBeforeY);
        }

        if (!feetInLiquidBefore && entity.FeetInLiquid)
        {
            entity.OnCollideWithLiquid();
        }

        if ((swimmingBefore || feetInLiquidBefore ) && !entity.Swimming && !entity.FeetInLiquid)
        {
            entity.OnExitedLiquid();
        }

        if (!collidedBefore && entity.Collided)
        {
            entity.OnCollided();
        }

        if (entity.OnGround)
        {
            entity.PositionBeforeFalling.Set(newPos);
        }

        if (GlobalConstants.OutsideWorld(pos.X, pos.Y, pos.Z, entity.World.BlockAccessor))
        {
            entity.DespawnReason = new EntityDespawnData()
            {
                Reason = EnumDespawnReason.Death,
                DamageSourceForDeath = new DamageSource() { Source = EnumDamageSource.Fall }
            };
            return;
        }

        // Entity was inside all of these blocks this tick, call events.
        Cuboidd entityBox = collisionTester.entityBox;
        int xMax = (int)entityBox.X2;
        int yMax = (int)entityBox.Y2;
        int zMax = (int)entityBox.Z2;
        int zMin = (int)entityBox.Z1;
        for (int y = (int)entityBox.Y1; y <= yMax; y++)
        {
            for (int x = (int)entityBox.X1; x <= xMax; x++)
            {
                for (int z = zMin; z <= zMax; z++)
                {
                    collisionTester.tmpPos.Set(x, y, z);
                    entity.World.BlockAccessor.GetBlock(x, y, z).OnEntityInside(entity.World, entity, collisionTester.tmpPos);
                }
            }
        }

        // Invoke callbacks. There is no accumulation left because this is fixed tick.
        OnPhysicsTickCallback?.Invoke(0);
        entity.PhysicsUpdateWatcher?.Invoke(0, prevPos);
    }

    public void OnPhysicsTick(float dt)
    {
        if (entity.State != EnumEntityState.Active || !Ticking) return;

        if (mountableSupplier?.IsBeingControlled() == true && entity.World.Side == EnumAppSide.Server) return;

        EntityPos pos = entity.SidedPos;

        // If entity is moving 6 blocks per second test 10 times. Needs dynamic adjustment this is overkill.
        int loops = pos.Motion.Length() > 0.1 ? 10 : 1;
        float newDt = dt / loops;

        for (int i = 0; i < loops; i++)
        {
            SetState(pos);
            MotionAndCollision(pos, newDt);
            ApplyTests(pos);
        }

        entity.Pos.SetFrom(pos);
    }

    public void AfterPhysicsTick(float dt)
    {
    }

    public override void OnEntityDespawn(EntityDespawnData despawn)
    {
        if (sapi != null)
        {
            sapi.Server.RemovePhysicsTickable(this);
        }
    }

    public override string PropertyName()
    {
        return "entitypassivephysics";
    }
}
