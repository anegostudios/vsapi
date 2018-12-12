namespace Vintagestory.API.Common
{
    public enum EnumMatterState
    {
        /// <summary>
        /// The state of being so thin that molecules don't often touch
        /// </summary>
        Gas,

        /// <summary>
        /// The state of being still together but loose enough to move around each other.
        /// </summary>
        Liquid,

        /// <summary>
        /// The state of being together and held still by the internal structure.
        /// </summary>
        Solid,

        /// <summary>
        /// The state of reacting with itself with some kind of reactant, a high heat state.
        /// </summary>
        Plasma,

        /// <summary>
        /// The state of becomming Quantum Jelly.
        /// </summary>
        BoseEinsteinCondensate
    }
}