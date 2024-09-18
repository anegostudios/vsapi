namespace Vintagestory.API.Common.Entities;

public interface IPhysicsTickable
{
    /// <summary>
    /// Flag for load balancer.
    /// </summary>
    ref int FlagTickDone { get; }

    /// <summary>
    /// Called at a fixed interval 10 times per second.
    /// </summary>
    public void OnPhysicsTick(float dt);

    /// <summary>
    /// Called after physics has ticked and is thread-safe.
    /// </summary>
    public void AfterPhysicsTick(float dt);

    public bool Ticking { get; set; }
}
