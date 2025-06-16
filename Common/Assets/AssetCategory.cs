using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Common
{
    public class AssetCategory
    {
        public static Dictionary<string, AssetCategory> categories = new Dictionary<string, AssetCategory>(14);

        public static AssetCategory blocktypes = new AssetCategory("blocktypes", true, EnumAppSide.Server);
        public static AssetCategory itemtypes = new AssetCategory("itemtypes", true, EnumAppSide.Server);
        public static AssetCategory lang = new AssetCategory("lang", false, EnumAppSide.Universal);
        //public static AssetCategory lore = new AssetCategory("journal", false, EnumAppSide.Universal);
        public static AssetCategory patches = new AssetCategory("patches", false, EnumAppSide.Universal);
        public static AssetCategory config = new AssetCategory("config", false, EnumAppSide.Universal);
        public static AssetCategory worldproperties = new AssetCategory("worldproperties", true, EnumAppSide.Universal);
        public static AssetCategory sounds = new AssetCategory("sounds", false, EnumAppSide.Universal);
        public static AssetCategory shapes = new AssetCategory("shapes", false, EnumAppSide.Universal);

        public static AssetCategory shaders = new AssetCategory("shaders", false, EnumAppSide.Client);
        public static AssetCategory shaderincludes = new AssetCategory("shaderincludes", false, EnumAppSide.Client);
        public static AssetCategory textures = new AssetCategory("textures", false, EnumAppSide.Universal); // Universal because we need textures/environment/sunlight.png for skylight calc -.-
        public static AssetCategory music = new AssetCategory("music", false, EnumAppSide.Client);
        public static AssetCategory dialog = new AssetCategory("dialog", false, EnumAppSide.Client);

        public static AssetCategory recipes = new AssetCategory("recipes", true, EnumAppSide.Server);
        public static AssetCategory worldgen = new AssetCategory("worldgen", true, EnumAppSide.Server);
        public static AssetCategory entities = new AssetCategory("entities", true, EnumAppSide.Server);



        /// <summary>
        /// Path and name
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Determines wether it will be used on server, client or both.
        /// </summary>
        public EnumAppSide SideType { get; private set; }

        /// <summary>
        /// Temporary solution to not change block types. Will be changed
        /// </summary>
        public bool AffectsGameplay { get; private set; }

        public AssetCategory(string code, bool AffectsGameplay, EnumAppSide SideType)
        {
            AssetCategory.categories[code] = this;
            this.Code = code;
            this.AffectsGameplay = AffectsGameplay;
            this.SideType = SideType;
        }

        public override string ToString()
        {
            return Code;
        }

        /// <summary>
        /// Gets the asset category by code name
        /// </summary>
        /// <param name="code">The code name for the asset category.</param>
        /// <returns>An asset category.</returns>
        public static AssetCategory FromCode(string code)
        {
            if (!categories.ContainsKey(code)) return null;

            return categories[code];
        }
    }
}
