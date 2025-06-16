using System;
using System.Collections.Generic;
using System.Globalization;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Config
{
    public delegate float FoodSpoilageCalcDelegate(float spoilState, ItemStack stack, EntityAgent byEntity);

    /// <summary>
    /// Contains some global constants and static values
    /// </summary>
    public class GlobalConstants
    {
        public static CultureInfo DefaultCultureInfo = CultureInfo.InvariantCulture;

        /// <summary>
        /// Prefix for all default asset locations
        /// </summary>
        public const string DefaultDomain = "game";

        /// <summary>
        /// Hard-enforced world size limit, above this the code may break
        /// </summary>
        public const int MaxWorldSizeXZ = 67108864;   // 64m equivalent in base 2  (2 to the power of 26)   From 1.20 any change to this constant may break access to dimensions (for example the Devastation)
        /// <summary>
        /// Hard-enforced world height limit, above this the code may break.
        /// </summary>
        public const int MaxWorldSizeY = 16384;     // 16k equivalent in base 2  (2 to the power of 14)  From 1.19 any change to this constant may break some blockEntities in dimensions (at minimum, saved BlockEntities in dimensions would have wrong x, y, z values)

        /// <summary>
        /// Now a hard-coded constant
        /// </summary>
        public const int ChunkSize = 32;

        /// <summary>
        /// Used in various places if the dimension of a chunk is combined into the chunk's y value.
        /// </summary>
        public const int DimensionSizeInChunks = 2 * MaxWorldSizeY / ChunkSize;

        /// <summary>
        /// Max. amount of "bones" for animated model. Limited by max amount of shader uniforms of around 60, but depends on the gfx card
        /// This value is overriden by ClientSettings.cs
        /// </summary>
        public static int MaxAnimatedElements = 46 * 5;

        /// <summary>
        /// Max. amount of "bones" for color maps. Limited by max amount of shader uniforms, but depends on the gfx card
        /// </summary>
        public const int MaxColorMaps = 40;

        public static int CaveArtColsPerRow = 6;

        /// <summary>
        /// Frame time for physics simulation
        /// </summary>
        public static float PhysicsFrameTime = 1 / 30f;

        /// <summary>
        /// Limits the amount of world time that can be simulated by the physics engine if the server is ticking slowly: if ticks are slower than this, entities will seem to slow down (viewed on client might even jump backwards)
        /// <br/> Recommended range 0.1f to 0.4f
        /// </summary>
        public static float MaxPhysicsIntervalInSlowTicks = 0.135f;

        /// <summary>
        /// A multiplier applied to the y motion of all particles affected by gravity.
        /// </summary>
        public static float GravityStrengthParticle = 0.3f;

        /// <summary>
        /// Attack range when using hands
        /// </summary>
        public static float DefaultAttackRange = 1.5f;

        /// <summary>
        /// Multiplied to all motions and animation speeds
        /// </summary>
        public static float OverallSpeedMultiplier = 1f;

        /// <summary>
        /// Multiplier applied to the players movement motion
        /// </summary>
        public static float BaseMoveSpeed = 1.5f;
        /// <summary>
        /// Multiplier applied to the players jump motion
        /// </summary>
        public static float BaseJumpForce = 8.2f;

        /// <summary>
        /// Multiplier applied to the players sneaking motion
        /// </summary>
        public static float SneakSpeedMultiplier = 0.35f;

        /// <summary>
        /// Multiplier applied to the players sprinting motion
        /// </summary>
        public static double SprintSpeedMultiplier = 2.0;

        
        /// <summary>
        /// Multiplier applied to entity motion while on the ground or in air
        /// </summary>
        public static float AirDragAlways = 0.983f;

        /// <summary>
        /// Multiplier applied to entity motion while flying (creative mode)
        /// </summary>
        public static float AirDragFlying = 0.8f;

        /// <summary>
        /// Multiplier applied to entity motion while walking in water
        /// </summary>
        public static float WaterDrag = 0.92f;

        /// <summary>
        /// Amount of gravity per tick applied to all entities affected by gravity
        /// </summary>
        public static float GravityPerSecond = 0.37f;
        
        /// <summary>
        /// Range in blocks at where this entity is simulated on the server (MagicNum.cs sets this value)
        /// </summary>
        public static int DefaultSimulationRange = 128;

        /// <summary>
        /// Range in blocks a player can interact with blocks (break, use, place)
        /// </summary>
        public static float DefaultPickingRange = 4.5f;

        /// <summary>
        /// Time in seconds for dropped items to remain when dropped after player death; overrides the despawn time set in item.json
        /// </summary>
        public static int TimeToDespawnPlayerInventoryDrops = 600;

        /// <summary>
        /// Set by the WeatherSimulation System in the survival mod at the players position
        /// </summary>
        public static Vec3f CurrentWindSpeedClient = new Vec3f();
        public static Vec3f CurrentSurfaceWindSpeedClient = new Vec3f();

        /// <summary>
        /// Set by the SystemPlayerEnvAwarenessTracker System in the engine at the players position, once every second. 12 horizontal, 4 vertical search distance
        /// </summary>
        public static float CurrentDistanceToRainfallClient;

        /// <summary>
        /// Set by the game client at the players position
        /// </summary>
        public static float CurrentNearbyRelLeavesCountClient;

        /// <summary>
        /// Set by the weather simulation system to determine if snowed variants of blocks should melt. Used a static var to improve performance and reduce memory usage
        /// </summary>
        public static bool MeltingFreezingEnabled;

        public static float GuiGearRotJitter = 0f;

        public const int MaxViewDistanceForLodBiases = 640;


        /// <summary>
        /// Returns true if the player fell out of the world (which is map boundaries + 30 blocks in every direction)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="blockAccessor"></param>
        /// <returns></returns>
        public static bool OutsideWorld(int x, int y, int z, IBlockAccessor blockAccessor)
        {
            return x < -30 || z < -30 || y < -30 || x > blockAccessor.MapSizeX + 30 || z > blockAccessor.MapSizeZ + 30;
        }

        /// <summary>
        /// Returns true if the player fell out of the world (which is map boundaries + 30 blocks in every direction)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="blockAccessor"></param>
        /// <returns></returns>
        public static bool OutsideWorld(double x, double y, double z, IBlockAccessor blockAccessor)
        {
            return x < -30 || z < -30 || y < -30 || x > blockAccessor.MapSizeX + 30 || z > blockAccessor.MapSizeZ + 30;
        }


        /// <summary>
        /// These reserved characters or sequences should not be used in texture filenames or asset locations, they can mess up the BakedTexture system
        /// </summary>
        public static string[] ReservedCharacterSequences = new string[] {
            "" + CompositeTexture.AlphaSeparator,                // "å"
            "" + CompositeTexture.BlendmodeSeparator,            // "~"
            CompositeTexture.OverlaysSeparator,                  // "++"
            "@90",
            "@180",
            "@270"
            // Note: if we add any more reserved characters here, for things outside the textures system, note that the `pathForReservedCharsCheck` parameter in new PathOrigin() and new FolderOrigin() and its sub-types, at present, only ever checks the "assets/textures" sub-folder
        };

        public const string WorldSaveExtension = ".vcdbs";
        public const string hotBarInvClassName = "hotbar";
        public const string creativeInvClassName = "creative";
        public const string backpackInvClassName = "backpack";
        public const string groundInvClassName = "ground";
        public const string mousecursorInvClassName = "mouse";
        public const string characterInvClassName = "character";
        public const string craftingInvClassName = "craftinggrid";


        public static Dictionary<string, double[]> playerColorByEntitlement = new Dictionary<string, double[]>()
        {
            { "vsteam", new double[] { 13 / 255.0, 128 / 255.0, 62 / 255.0, 1 } },
            { "vscontributor", new double[] { 135 / 255.0, 179 / 255.0, 148 / 255.0, 1 } },
            { "vssupporter", new double[] { 254/255.0, 197/255.0, 0, 1 } },

            { "securityresearcher", new double[] { 49/255.0, 159/255.0, 174/255.0, 1 } },
            { "bughunter", new double[] { 174 / 255.0, 96/255.0, 49/255.0, 1 } },
            { "chiselmaster", new double[] { 242 / 255.0, 244 / 255.0, 187 / 255.0, 1 } },
        };


        public static Dictionary<string, TextBackground> playerTagBackgroundByEntitlement = new Dictionary<string, TextBackground>()
        {
            { "vsteam", new TextBackground()
                    {
                        FillColor = GuiStyle.DialogLightBgColor,
                        Padding = 3,
                        Radius = GuiStyle.ElementBGRadius,
                        Shade = true,
                        BorderColor = GuiStyle.DialogBorderColor,
                        BorderWidth = 3,
                    } },
            { "vscontributor", new TextBackground()
                    {
                        FillColor = GuiStyle.DialogLightBgColor,
                        Padding = 3,
                        Radius = GuiStyle.ElementBGRadius,
                        Shade = true,
                        BorderColor = GuiStyle.DialogBorderColor,
                        BorderWidth = 3,
                    } },
            { "vssupporter", new TextBackground()
                    {
                        FillColor = GuiStyle.DialogLightBgColor,
                        Padding = 3,
                        Radius = GuiStyle.ElementBGRadius,
                        Shade = true,
                        BorderColor = GuiStyle.DialogBorderColor,
                        BorderWidth = 3,
                    } },
            { "securityresearcher", new TextBackground()
                    {
                        FillColor = GuiStyle.DialogLightBgColor,
                        Padding = 3,
                        Radius = GuiStyle.ElementBGRadius,
                        Shade = true,
                        BorderColor = GuiStyle.DialogBorderColor,
                        BorderWidth = 3,
                    } },
            { "bughunter", new TextBackground()
                    {
                        FillColor = GuiStyle.DialogLightBgColor,
                        Padding = 3,
                        Radius = GuiStyle.ElementBGRadius,
                        Shade = true,
                        BorderColor = GuiStyle.DialogBorderColor,
                        BorderWidth = 3,
                    } },
            { "chiselmaster", new TextBackground()
                    {
                        FillColor = GuiStyle.DialogLightBgColor,
                        Padding = 3,
                        Radius = GuiStyle.ElementBGRadius,
                        Shade = true,
                        BorderColor = GuiStyle.DialogBorderColor,
                        BorderWidth = 3,
                    } },
        };

        public static int[] DefaultChatGroups = new int[] { GeneralChatGroup, ServerInfoChatGroup, DamageLogChatGroup, InfoLogChatGroup, ConsoleGroup };

        /// <summary>
        /// Channel name for the general chat
        /// </summary>
        public static int GeneralChatGroup = 0;
        /// <summary>
        /// Channel name for the general chat
        /// </summary>
        public static int ServerInfoChatGroup = -1;
        /// <summary>
        /// Channel name for the damage chat log
        /// </summary>
        public static int DamageLogChatGroup = -5;
        /// <summary>
        /// Channel name for the info chat log
        /// </summary>
        public static int InfoLogChatGroup = -6;
        /// <summary>
        /// Special channel key typically to reply a Command inside the same the channel the player sent it
        /// </summary>
        public static int CurrentChatGroup = -2;
        /// <summary>
        /// Special channel key typically to reply a Command inside the same the channel the player sent it
        /// </summary>
        public static int AllChatGroups = -3;
        /// <summary>
        /// Special channel key for message sent via server console
        /// </summary>
        public static int ConsoleGroup = -4;

        /// <summary>
        /// Allowed characters for a player group name
        /// </summary>
        public static string AllowedChatGroupChars = "a-z0-9A-Z_";

        /// <summary>
        /// Bit of a helper thing for single player servers to display the correct entitlements
        /// </summary>
        public static string SinglePlayerEntitlements;

        /// <summary>
        /// The entity class used when spawning items in the world
        /// </summary>
        public static AssetLocation EntityItemTypeCode = new AssetLocation("item");
        /// <summary>
        /// The entity class used when spawning players
        /// </summary>
        public static AssetLocation EntityPlayerTypeCode = new AssetLocation("player");

		/// <summary>
        /// The entity class used when spawning falling blocks
        /// </summary>
        public static AssetLocation EntityBlockFallingTypeCode = new AssetLocation("blockfalling");

        /// <summary>
        /// Default Itemstack attributes that should always be ignored during a stack.Collectible.Equals() comparison
        /// </summary>
        public static string[] IgnoredStackAttributes = new string[] { "temperature", "toolMode", "renderVariant", "transitionstate" };

        /// <summary>
        /// Global modifier to change the spoil rate of foods. Can be changed during run-time. The value is multiplied to the normal spoilage rate (default: 1)
        /// </summary>
        public static float PerishSpeedModifier = 1;

        /// <summary>
        /// Global modifier to change the rate of player hunger. Can be changed during run-time. The value is multiplied to the normal spoilage rate (default: 1)
        /// </summary>
        public static float HungerSpeedModifier = 1f;

        /// <summary>
        /// Global modifier to change the damage melee attacks from creatures inflict. Can be changed during run-time. The value is multiplied to the normal damage value (default: 1)
        /// </summary>
        public static float CreatureDamageModifier = 1;
        /// <summary>
        /// Global modifier to change the block breaking speed of all tools. Can be changed during run-time. The value is multiplied to the breaking speed (default: 1)
        /// </summary>
        public static float ToolMiningSpeedModifier = 1;

        public static FoodSpoilageCalcDelegate FoodSpoilHealthLossMulHandler => (spoilState, stack, byEntity) => (float)Math.Max(0f, 1f - spoilState);
        public static FoodSpoilageCalcDelegate FoodSpoilSatLossMulHandler => (spoilState, stack, byEntity) => (float)Math.Max(0f, 1f - spoilState);


        public static float FoodSpoilageHealthLossMul(float spoilState, ItemStack stack, EntityAgent byEntity)
        {
            return FoodSpoilHealthLossMulHandler(spoilState, stack, byEntity);
        }

        public static float FoodSpoilageSatLossMul(float spoilState, ItemStack stack, EntityAgent byEntity)
        {
            return FoodSpoilSatLossMulHandler(spoilState, stack, byEntity);
        }
    }
}
