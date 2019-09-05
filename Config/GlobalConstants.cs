using System;
using Vintagestory.API.Common;

namespace Vintagestory.API.Config
{
    public delegate float FoodSpoilageCalcDelegate(float spoilState, ItemStack stack, EntityAgent byEntity);

    /// <summary>
    /// Contains some global constants and static values
    /// </summary>
    public class GlobalConstants
    {
        /// <summary>
        /// Prefix for all default asset locations
        /// </summary>
        public const string DefaultDomain = "game";

        /// <summary>
        /// Max. amount of "bones" for animated model. Limited by max amount of shader uniforms of around 60, but depends on the gfx card
        /// </summary>
        public const int MaxAnimatedElements = 31;

        /// <summary>
        /// Frame time for physics simulation
        /// </summary>
        public static float PhysicsFrameTime = 1 / 75f;

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
        public static float BaseJumpForce = 9f;

        /// <summary>
        /// Multiplier applied to the players sneaking motion
        /// </summary>
        public static float SneakSpeedMultiplier = 0.375f;

        /// <summary>
        /// Multiplier applied to the players sprinting motion
        /// </summary>
        public static double SprintSpeedMultiplier = 2.0;

        /// <summary>
        /// Multiplier applied to the players walk vector while falling
        /// </summary>
        //public static float AirMovingStrength = 0.2f;

        /// <summary>
        /// Multiplier applied to the players motion while on the ground or in air
        /// </summary>
        public static float AirDragAlways = 0.98f;

        /// <summary>
        /// Multiplier applied to the players motion while flying (creative mode)
        /// </summary>
        public static float AirDragFlying = 0.8f;

        /// <summary>
        /// Multiplier applied to the players motion while walking in water
        /// </summary>
        public static float WaterDrag = 0.9f;

        /// <summary>
        /// Amount of gravity per tick applied to all entities affected by gravity
        /// </summary>
        public static float GravityPerSecond = 0.35f;
        
        /// <summary>
        /// Range in blocks at which clients receive regular updates of this entity
        /// </summary>
        public const int DefaultTrackingRange = 128;

        /// <summary>
        /// Range in blocks a player can interact with blocks (break, use, place)
        /// </summary>
        public static float DefaultPickingRange = 4.5f;

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


        public const string WorldSaveExtension = ".vcdbs";
        public const string hotBarInvClassName = "hotbar";
        public const string creativeInvClassName = "creative";
        public const string backpackInvClassName = "backpack";
        public const string groundInvClassName = "ground";
        public const string mousecursorInvClassName = "mouse";
        public const string characterInvClassName = "character";
        public const string craftingInvClassName = "craftinggrid";

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
        /// Channel name for the damage chat log
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
        /// Default Itemstack attributes that should be ignored during a stack.Collectible.Equals() comparison
        /// </summary>
        public static string[] IgnoredStackAttributes = new string[] { "temperature", "toolMode", "renderVariant", "transitionstate" };


        public static float PerishSpeedModifier = 1;

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
