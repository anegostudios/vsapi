using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a mod in the mod manager. May contain zero to multiple
    /// <see cref="T:Vintagestory.API.Common.ModSystem"/> instances within it.
    /// </summary>
    public abstract class Mod
    {
        /// <summary> Gets the origin file type of the mod (.cs, .dll, .zip or folder). </summary>
        public EnumModSourceType SourceType { get; internal set; }

        /// <summary> Gets the full path to where this mod originated from, including file name. </summary>
        public string SourcePath { get; internal set; }

        /// <summary> Gets the file name of this mod. </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// Gets the info of this mod. Found either as "modinfo.json" in the
        /// of the mod's folder or archive, or in the case of raw .cs and .dll
        /// files, using the <see cref="T:Vintagestory.API.Common.ModInfoAttribute"/>
        /// on the assembly.
        /// </summary>
        public ModInfo Info { get; internal set; }

        public ModWorldConfiguration WorldConfig { get; internal set; }

        /// <summary>
        /// Holds the icon of this mod. Found as "modicon.png" in the root of
        /// the mod's folder or archive. May be null.
        /// </summary>
        public BitmapExternal Icon { get; internal set; }

        /// <summary> Gets the logger associated with this mod. </summary>
        public ILogger Logger { get; internal set; }

        /// <summary> Gets a collection of systems belonging to this mod. </summary>
        public IReadOnlyCollection<ModSystem> Systems { get; internal set; }
            = new List<ModSystem>(0).AsReadOnly();


        public override string ToString()
        {
            return !string.IsNullOrEmpty(Info?.ModID)
                ? $"'{ FileName }' ({ Info.ModID })"
                : $"'{ FileName }'";
        }
    }

    /// <summary>
    /// Represents the origin file type of the mod.
    /// </summary>
    public enum EnumModSourceType
    {
        /// <summary> A single .cs source file. (Code mod without assets.) </summary>
        CS,
        /// <summary> A single .dll source file. (Code mod without assets.) </summary>
        DLL,
        /// <summary> A .zip archive able to contain assets and code files. </summary>
        ZIP,
        /// <summary> A folder able to contain assets and code files. </summary>
        Folder
    }
}
