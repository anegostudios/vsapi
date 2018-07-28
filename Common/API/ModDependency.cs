using System;

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
        
        public ModDependency(string modID, string version = "")
        {
            if (modID == null) throw new ArgumentNullException(nameof(modID));
            if (!ModInfo.IsValidModID(modID)) throw new ArgumentException(
                $"'{ modID }' is not a valid mod ID", nameof(modID));
            ModID   = modID;
            Version = version ?? "";
        }
        
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
    public class ModDependencyAttribute : Attribute
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
                $"'{ modID }' is not a valid mod ID", nameof(modID));
            ModID   = modID;
            Version = version ?? "";
        }
    }
}
