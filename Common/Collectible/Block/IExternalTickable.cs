namespace Vintagestory.Common.Collectible.Block;

public interface IExternalTickable
{
    /// <summary>
    /// If true it will be ticked from another objeckt eg. BlockEntity. The own tick listener should be unregistered
    /// </summary>
    public bool IsExternallyTicked { get; protected set; }

    /// <summary>
    /// Sets this object as externally ticked, unregistering its own tick listener.
    /// </summary>
    void SetExternallyTicked()
    {
        if (!IsExternallyTicked)
        {
            IsExternallyTicked = true;
            UnregisterTickListener();
        }
    }

    /// <summary>
    /// Sets this object as internally ticked, registering its own tick listener again.
    /// </summary>
    void SetInternallyTicked()
    {
        if (IsExternallyTicked)
        {
            IsExternallyTicked = false;
            RegisterServerTickListener();
        }
    }

    /// <summary>
    /// Register the own tick listener
    /// Used if the external BlockEntity is destroyed, to reactivate the ticking.
    /// Will be called automatically by SetInternallyTicked
    /// </summary>
    void RegisterServerTickListener();

    /// <summary>
    /// Unregister the normal tick listener so the external entity can control ticking.
    /// Will be called automatically by SetExternallyTicked
    /// </summary>
    void UnregisterTickListener();

    /// <summary>
    /// Called by the external BlockEntity to perform ticking logic
    /// when the internal tick listener is disabled.
    /// </summary>
    /// <param name="dt"></param>
    void OnExternalTick(float dt);
}
