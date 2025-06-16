
#nullable disable
namespace Vintagestory.API.Common
{

    /// <summary>
    /// Habitats for entities. Controls some minor logic for entities.
    /// </summary>
    [DocumentAsJson]
    public enum EnumHabitat
    {
        /// <summary>
        /// No gravity, AiTaskWander will look for water or ice.
        /// </summary>
        Sea,

        /// <summary>
        /// Apply gravity. Standard land creature.
        /// </summary>
        Land,

        /// <summary>
        /// No gravity. 
        /// </summary>
        Air,

        /// <summary>
        /// No gravity, AiTaskWander will look for water or ice.
        /// </summary>
        Underwater
    }
}
