
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// This contains data about an event that fires when a player changes which
    /// slot they're actively using. Such as the currently selected hotbar slot.
    /// </summary>
    public class ActiveSlotChangeEventArgs
    {
        /// <summary>
        /// The currently active slot being switched away from.
        /// </summary>
        public int FromSlot { get; }

        /// <summary>
        /// The target slot that is being switched to.
        /// </summary>
        public int ToSlot { get; }

        public ActiveSlotChangeEventArgs(int from, int to)
        {
            FromSlot = from;
            ToSlot   = to;
        }
    }
}
