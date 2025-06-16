using System;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common.Entities;

public abstract class PhysicsBehaviorBase : EntityBehavior
{
    protected ICoreClientAPI capi;
    protected ICoreServerAPI sapi;

    // How often the client should be sending updates.
    protected const float clientInterval = 1 / 15f;

    protected int previousVersion;

    public IMountable mountableSupplier;

    protected readonly EntityPos lPos = new();
    protected Vec3d nPos;

    public float CollisionYExtra = 1f;

    [ThreadStatic]
    protected internal static CachingCollisionTester collisionTester;

    static PhysicsBehaviorBase()
    {
    }

    public PhysicsBehaviorBase(Entity entity) : base(entity)
    {
    }

    public static void InitServerMT(ICoreServerAPI sapi)
    {
        collisionTester = new CachingCollisionTester();
        sapi.Event.PhysicsThreadStart += () => collisionTester = new CachingCollisionTester();
    }

    public void Init()
    {
        if (entity.Api is ICoreClientAPI capi) this.capi = capi;
        if (entity.Api is ICoreServerAPI sapi) this.sapi = sapi;
    }

    public override void AfterInitialized(bool onFirstSpawn)
    {
        mountableSupplier = entity.GetInterface<IMountable>();
    }
}
