using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Defines a basic entity behavior that can be attached to entities
    /// </summary>
    public abstract class EntityBehavior
    {
        public Entity entity;

        public string ProfilerName { get; private set; }

        public EntityBehavior(Entity entity)
        {
            this.entity = entity;
            ProfilerName = "done-behavior-" + PropertyName();
        }

        /// <summary>
        /// Initializes the entity.
        /// <br/>If your code modifies the supplied attributes (not recommended!), then your changes will apply to all entities of the same type.
        /// </summary>
        /// <param name="properties">The properties of this entity.</param>
        /// <param name="attributes">The attributes of this entity.</param>
        public virtual void Initialize(EntityProperties properties, JsonObject attributes)
        {
        }

        /// <summary>
        /// Called after initializing all the behaviors in case they need to cross-refer to each other or set some initial values only at spawn-time
        /// </summary>
        public virtual void AfterInitialized(bool onFirstSpawn)
        {
        }

        /// <summary>
        /// The event fired when a game ticks over.
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void OnGameTick(float deltaTime) { }

        /// <summary>
        /// An implementing behavior can return true if it is thread-safe: allows the behavior to be multi-threaded for better multiplayer performance
        /// </summary>
        public virtual bool ThreadSafe { get { return false; } }
        /// <summary>
        /// The event fired when the entity is spawned (not called when loaded from the savegame).
        /// </summary>
        public virtual void OnEntitySpawn() { }

        /// <summary>
        /// The event fired when the entity is loaded from disk (not called during spawn)
        /// </summary>
        public virtual void OnEntityLoaded() { }

        /// <summary>
        /// The event fired when the entity is despawned.
        /// </summary>
        /// <param name="despawn">The reason the entity despawned.</param>
        public virtual void OnEntityDespawn(EntityDespawnData despawn) { }

        /// <summary>
        /// The name of the property tied to this entity behavior.
        /// </summary>
        /// <returns></returns>
        public abstract string PropertyName();

        /// <summary>
        /// The event fired when the entity recieves damage.
        /// </summary>
        /// <param name="damageSource">The source of the damage</param>
        /// <param name="damage">The amount of the damage.</param>
        public virtual void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {

        }

        /// <summary>
        /// When the entity got revived (only for players and traders currently)
        /// </summary>
        public virtual void OnEntityRevive()
        {

        }

        /// <summary>
        /// The event fired when the entity falls to the ground.
        /// </summary>
        /// <param name="lastTerrainContact">the point which the entity was previously on the ground.</param>
        /// <param name="withYMotion">The vertical motion the entity had before landing on the ground.</param>
        public virtual void OnFallToGround(Vec3d lastTerrainContact, double withYMotion)
        {
        }

        /// <summary>
        /// The event fired when the entity recieves saturation.
        /// </summary>
        /// <param name="saturation">The amount of saturation recieved.</param>
        /// <param name="foodCat">The category of food recieved.</param>
        /// <param name="saturationLossDelay">The delay before the loss of saturation.</param>
        /// <param name="nutritionGainMultiplier"></param>
        public virtual void OnEntityReceiveSaturation(float saturation, EnumFoodCategory foodCat = EnumFoodCategory.Unknown, float saturationLossDelay = 10, float nutritionGainMultiplier = 1f)
        {

        }

        /// <summary>
        /// The event fired when the server position is changed.
        /// </summary>
        /// <param name="isTeleport">Whether or not this entity was teleported.</param>
        /// <param name="handled">How this event is handled.</param>
        public virtual void OnReceivedServerPos(bool isTeleport, ref EnumHandling handled)
        {

        }

        /// <summary>
        /// gets the drops for this specific entity.
        /// </summary>
        /// <param name="world">The world of this entity</param>
        /// <param name="pos">The block position of the entity.</param>
        /// <param name="byPlayer">The player this entity was killed by.</param>
        /// <param name="handling">How this event was handled.</param>
        /// <returns>the items dropped from this entity</returns>
        public virtual ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;

            return null;
        }

        /// <summary>
        /// The event fired when the state of the entity is changed.
        /// </summary>
        /// <param name="beforeState">The previous state.</param>
        /// <param name="handling">How this event was handled.</param>
        public virtual void OnStateChanged(EnumEntityState beforeState, ref EnumHandling handling)
        {

        }

        /// <summary>
        /// The notify method bubbled up from entity.Notify()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public virtual void Notify(string key, object data)
        {

        }

        /// <summary>
        /// Gets the information text when highlighting this entity.
        /// </summary>
        /// <param name="infotext">The supplied stringbuilder information.</param>
        public virtual void GetInfoText(StringBuilder infotext)
        {

        }

        /// <summary>
        /// The event fired when the entity dies.
        /// </summary>
        /// <param name="damageSourceForDeath">The source of damage for the entity.</param>
        public virtual void OnEntityDeath(DamageSource damageSourceForDeath)
        {

        }

        /// <summary>
        /// The event fired when the entity is interacted with by the player.
        /// </summary>
        /// <param name="byEntity">The entity it was interacted with.</param>
        /// <param name="itemslot">The item slot involved (if any)</param>
        /// <param name="hitPosition">The hit position of the entity.</param>
        /// <param name="mode">The interaction mode for the entity.</param>
        /// <param name="handled">How this event is handled.</param>
        public virtual void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition, EnumInteractMode mode, ref EnumHandling handled)
        {

        }

        /// <summary>
        /// The event fired when the server receives a packet.
        /// </summary>
        /// <param name="player">The server player.</param>
        /// <param name="packetid">the packet id.</param>
        /// <param name="data">The data contents.</param>
        /// <param name="handled">How this event is handled.</param>
        public virtual void OnReceivedClientPacket(IServerPlayer player, int packetid, byte[] data, ref EnumHandling handled)
        {

        }

        /// <summary>
        /// The event fired when the client receives a packet.
        /// </summary>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        /// <param name="handled"></param>
        public virtual void OnReceivedServerPacket(int packetid, byte[] data, ref EnumHandling handled)
        {

        }


        /// <summary>
        /// Called when a player looks at the entity with interaction help enabled
        /// </summary>
        /// <param name="world"></param>
        /// <param name="es"></param>
        /// <param name="player"></param>
        /// <param name="handled"></param>
        public virtual WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection es, IClientPlayer player, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return null;
        }

        public virtual void DidAttack(DamageSource source, EntityAgent targetEntity, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
        }

        public virtual void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {

        }

        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, bool resolveImports)
        {

        }

        /// <summary>
        /// For entities which need to load collectible mappings before Initialisation during worldgen (e.g. Armorstand)
        /// <br/>Return true if the collectible mappings were loaded by this
        /// </summary>
        public virtual bool TryEarlyLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, bool resolveImports, EntityProperties entityProperties, JsonObject behaviorConfig)
        {
            return false;
        }

        public virtual void ToBytes(bool forClient)
        {

        }

        /// <summary>
        /// This method is not called on the server side
        /// </summary>
        /// <param name="isSync"></param>
        public virtual void FromBytes(bool isSync)
        {

        }

        /// <summary>
        /// Can be used by the /entity command or maybe other commands, to test behaviors
        /// <br/>The argument will be an object provided by TextCommandCallingArgs, which can then be cast to the desired type e.g. int
        /// </summary>
        public virtual void TestCommand(object arg)
        {
        }

        public virtual bool TryGiveItemStack(ItemStack itemstack, ref EnumHandling handling)
        {
            return false;
        }

        public virtual void OnTesselation(ref Shape entityShape, string shapePathForLogging, ref bool shapeIsCloned, ref string[] willDeleteElements)
        {

        }

        public virtual ITexPositionSource GetTextureSource(ref EnumHandling handling)
        {
            return null;
        }

        public virtual bool IntersectsRay(Ray ray, AABBIntersectionTest interesectionTester, out double intersectionDistance, ref int selectionBoxIndex, ref EnumHandling handled)
        {
            intersectionDistance = 0;
            return false;
        }

        public virtual void OnTesselated()
        {

        }

        public virtual void UpdateColSelBoxes()
        {

        }

        /// <summary>
        /// The distance at which entities are counted as "touching" each other, for example used by EntityPartitioning and RepulseAgents
        /// <br/>Note: from 1.20.4 this is gathered and cached in a field when each entity is Initialized, if for any reason a mod or behavior needs to change the result later than Initialization then you should also update the Entity field .touchDistance
        /// </summary>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual float GetTouchDistance(ref EnumHandling handling)
        {
            return 0;
        }

        public virtual string GetName(ref EnumHandling handling)
        {
            return null;
        }

        /// <summary>
        /// If true, then this entity will not retaliate if attacked by the specified eOther
        /// If false, then this entity will always retaliate (disregarding subsequent)
        /// </summary>
        /// <param name="eOther"></param>
        public virtual bool ToleratesDamageFrom(Entity eOther, ref EnumHandling handling)
        {
            return false;
        }

        /// <summary>
        /// Returns true if this entity behavior needs to remap inventory items/blocks when placed as part of a schematic during world generation.
        /// If true, EarlyLoadCollectibleMappings() will be called, during schematic placement, instead of OnLoadCollectibleMappings()
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldEarlyLoadCollectibleMappings()
        {
            return false;
        }
    }
}
