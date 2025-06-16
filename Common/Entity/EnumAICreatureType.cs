
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// Types of creatures used for pathfinding. Humanoids prefer taking paths.
    /// </summary>
    [DocumentAsJson]
    public enum EnumAICreatureType
    {
        /// <summary>
        /// Dumbest version. Never enters boiling water and lava. Prefers not to be in water. Slightly prefers to walk on blocks that give a walk speed bonus.
        /// </summary>
        Default,
        /// <summary>
        /// Same as Default, Additionally never enters some types of blocks that are on fire (fire pits, coal piles and pit kilns)
        /// </summary>
        LandCreature,
        /// <summary>
        /// Same as LandCreature, additionally strongly prefers to walk on blocks that give a walk speed bonues, such as stone paths
        /// </summary>
        Humanoid,
        /// <summary>
        /// Does not avoid fire or boiling water. Still Avoids Lava
        /// </summary>
        HeatProofCreature,
        /// <summary>
        /// Does not avoid water in any way
        /// </summary>
        SeaCreature,
    }
}
