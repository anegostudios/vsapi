
#nullable disable
namespace Vintagestory.API.Common.Entities;

public interface IPhysicsTickable
{
    /// <summary>
    /// Called at a fixed interval, potentially 30 times per second (if server is running smoothly)
    /// </summary>
    public void OnPhysicsTick(float dt);

    /// <summary>
    /// Called once per server tick, after all physics ticking has occurred; on main thread.
    /// </summary>
    public void AfterPhysicsTick(float dt);

    public bool Ticking { get; set; }

    public Entity Entity { get; }
}
