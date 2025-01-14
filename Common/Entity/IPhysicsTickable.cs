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

    /// <summary>
    /// If physics is multithreaded, indicates whether this tickable can proceed to be worked on on this particular thread, or not
    /// </summary>
    /// <returns></returns>
    public bool CanProceedOnThisThread();

    /// <summary>
    /// Should be called at the end of each individual physics tick, necessary for multithreading to share the work properly
    /// </summary>
    public void OnPhysicsTickDone();
}
