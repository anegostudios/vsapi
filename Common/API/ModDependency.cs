using System;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a mod dependency requirement of one mod for another.
    /// </summary>
    public class ModDependency
    {
        /// <summary> The required mod id (domain) of this dependency. </summary>
        public string ModID { get; }
        
        /// <summary>
        /// The minimum version requirement of this dependency.
        /// May be empty if the no specific version is required.
        /// </summary>
        public string Version { get; }
        
        /// <summary>
        /// Creates a new ModDependancy object.
        /// </summary>
        /// <param name="modID">The ID of the required mod.</param>
        /// <param name="version">The version of the required mod (default: empty string.)</param>
        public ModDependency(string modID, string version = "")
        {
            if (modID == null) throw new ArgumentNullException(nameof(modID));
            if (!ModInfo.IsValidModID(modID)) throw new ArgumentException(
                $"'{ modID }' is not a valid mod ID. Please use only lowercase letters and numbers.", nameof(modID));
            ModID   = modID;
            Version = version ?? "";
        }
        
        /// <summary>
        /// Returns the Mod Dependancy as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Version)) return ModID;
            else return $"{ ModID }@{ Version }";
        }
    }
    
    /// <summary>
    /// Applied to a mod assembly multiple times for each required dependency.
    /// Superseded by this mod's "modinfo.json" file, if available.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ModDependencyAttribute : Attribute
    {
        /// <summary> The required mod id (domain) of this dependency. </summary>
        public string ModID { get; }
        
        /// <summary>
        /// The minimum version requirement of this dependency.
        /// May be empty if the no specific version is required.
        /// </summary>
        public string Version { get; }
        
        public ModDependencyAttribute(string modID, string version = "")
        {
            if (modID == null) throw new ArgumentNullException(nameof(modID));
            if (!ModInfo.IsValidModID(modID)) throw new ArgumentException(
                $"'{ modID }' is not a valid mod ID. Please use only lowercase letters and numbers.", nameof(modID));
            ModID   = modID;
            Version = version ?? "";
        }
    }
}
