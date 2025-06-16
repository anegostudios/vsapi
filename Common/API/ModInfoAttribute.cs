using System;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Applied to a mod assembly to provide additional meta data information
    /// about a code mod. Superseded by "modinfo.json" file, if available.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ModInfoAttribute : Attribute
    {
        // Not needed. CS and DLL mods are implicitly Code mods.
        // public EnumModType Type { get; set; } = EnumModType.Code;
        
        /// <summary> The name of this mod. For example "My Example Mod". </summary>
        public string Name { get; }

        public string IconPath { get; set; }

        /// <summary> The mod ID (domain) of this mod. For example "myexamplemod". </summary>
        public string ModID { get; }
        
        /// <summary> The version of this mod. For example "2.10.4". (optional) </summary>
        public string Version { get; set; }

        /// <summary> Is this mod part of the core game, i.e. vanilla Vintage Story?  (optional)  <br/>Only affects whether the player is given a warning when attempting to disable this mod.<br/>Mods other than VSSurvival, VSEssentials, VSCreative should not use this, obviously... </summary>
        public bool CoreMod { get; set; }

        /// <summary>
        /// The network version of this mod. Change this number when a user that has an older version of your mod should not be allowed to connected to server with a newer version. If not set, the version value is used.
        /// </summary>
        public string NetworkVersion { get; set; }

        /// <summary> A short description of what this mod does. (optional) </summary>
        public string Description { get; set; }

        
        /// <summary> Location of the website or project site of this mod. (optional) </summary>
        public string Website { get; set; }
        
        /// <summary> Names of people working on this mod. (optional) </summary>
        public string[] Authors { get; set; }
        
        /// <summary> Names of people contributing to this mod. (optional) </summary>
        public string[] Contributors { get; set; }

        /// <summary>
        /// Which side(s) this mod runs on. Can be "Server", "Client" or "Universal".
        /// (Optional. Universal (both server and client) by default.)
        /// </summary>
        public string Side { get; set; } = EnumAppSide.Universal.ToString();
        
        /// <summary>
        /// If set to false and the mod is universal, clients don't need it to join.
        /// (Optional. True (required) by default.)
        /// </summary>
        public bool RequiredOnClient { get; set; } = true;
        
        /// <summary>
        /// If set to false and the mod is universal, the mod is not disabled
        /// if it's not present on the server. (Optional. True by default.)
        /// </summary>
        public bool RequiredOnServer { get; set; } = true;

        public string WorldConfig { get; set; } = null;
        
        public ModInfoAttribute(string name, string modID)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (modID == null) throw new ArgumentNullException(nameof(modID));
            if (name.Length == 0) throw new ArgumentException(
                "name can't be empty", nameof(name));
            if (!ModInfo.IsValidModID(modID)) throw new ArgumentException(
                $"'{ modID }' is not a valid mod ID. Please use only lowercase letters and numbers.", nameof(modID));
            Name  = name;
            ModID = modID;
        }
        
        public ModInfoAttribute(string name)
            : this(name, ModInfo.ToModID(name)) {  }
    }
}
