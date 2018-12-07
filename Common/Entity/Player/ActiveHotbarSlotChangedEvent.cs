using System;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// This contains data about an event that fires when a player changes their active hotbar slot.
    /// </summary>
    public class ActiveHotbarSlotChangedEvent
    {
        /// <summary>
        /// The currently selected active hotbar slot index.
        /// </summary>
        public int FromSlot { get; }

        /// <summary>
        /// The active hotbar slot index that will be
        /// switched to if this event is not cancelled.
        /// </summary>
        public int ToSlot { get; }

        /// <summary>
        /// Returns if this event can be cancelled. If the client receives a
        /// hotbar slot change from the server, the event cannot be cancelled.
        /// </summary>
        public bool CanCancel { get; }

        /// <summary>
        /// Gets whether this event has been cancelled.
        /// </summary>
        public bool Cancelled { get; private set; }

        public ActiveHotbarSlotChangedEvent(int from, int to, bool canCancel = true)
        {
            FromSlot  = from;
            ToSlot    = to;
            CanCancel = canCancel;
        }

        /// <summary>
        /// Cancels this event, preventing any following
        /// event handlers from firing and the default action.
        /// </summary>
        public void Cancel()
        {
            if (!CanCancel) throw new InvalidOperationException("Cannot cancel this event");
            Cancelled = true;
        }
    }
}
