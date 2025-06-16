using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IModLoader
    {
        /// <summary>
        /// Gets a collection of all enabled mods.
        /// </summary>
        IEnumerable<Mod> Mods { get; }

        /// <summary>
        /// Gets a collection of all loaded and enabled mod systems.
        /// </summary>
        IEnumerable<ModSystem> Systems { get; }

        /// <summary>
        /// Gets the enabled mod with the specified mod ID (domain).
        /// Returns null if no mod with that mod ID was found.
        /// </summary>
        Mod GetMod(string modID);

        /// <summary>
        /// Returns if the mod with the specified mod ID (domain) is enabled.
        /// </summary>
        bool IsModEnabled(string modID);

        /// <summary>
        /// Gets a loaded mod system with the specified full name, that is the namespace and
        /// class name, for example "Vintagestory.ServerMods.Core" for the survival mod.
        /// Returns null if no mod with that name was found.
        /// </summary>
        ModSystem GetModSystem(string fullName);

        /// <summary>
        /// Gets a loaded mod system with the specified type.
        /// Returns null if no mod of that type was found.
        /// </summary>
        T GetModSystem<T>(bool withInheritance = true) where T : ModSystem;

        /// <summary>
        /// Returns if the mod system with the specified full name is loaded and enabled.
        /// </summary>
        bool IsModSystemEnabled(string fullName);
    }
}
