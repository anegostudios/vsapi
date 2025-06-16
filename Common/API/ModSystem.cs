using Vintagestory.API.Client;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Base of a system, which is part of a code mod. Takes care of setting up,
    /// registering and handling all sorts of things. You may choose to split up
    /// a mod into multiple distinct systems if you so choose, but there may
    /// also be just one.
    /// </summary>
    public abstract class ModSystem
    {
        /// <summary>
        /// Gets the <see cref="T:Vintagestory.API.Common.Mod"/> this mod system is part of.
        /// </summary>
        public Mod Mod { get; internal set; }

        /// <summary>
        /// Returns if this mod should be loaded for the given app side.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public virtual bool ShouldLoad(ICoreAPI api)
        {
            return ShouldLoad(api.Side);
        }

        /// <summary>
        /// Returns if this mod should be loaded for the given app side, called by ShouldLoad(ICoreApi api)
        /// </summary>
        public virtual bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        /// <summary>
        /// If you need mods to be executed in a certain order, adjust this methods return value.<br/>
        /// The server will call each Mods StartPre() and Start() methods in ascending order of each mods execute order value. And thus, as long as every mod registers it's event handlers in the Start() method, all event handlers will be called in the same execution order.<br/>
        /// Default execute order of some survival mod parts<br/>
        /// Worldgen:<br/>
        /// - GenTerra: 0 <br/>
        /// - RockStrata: 0.1<br/>
        /// - Deposits: 0.2<br/>
        /// - Caves: 0.3<br/>
        /// - Blocklayers: 0.4<br/>
        /// AssetsLoaded event<br/>
        /// - JsonPatch loader: 0.05<br/>
        /// - Load hardcoded mantle block: 0.1<br/>
        /// - Block and Item Loader: 0.2<br/>
        /// - Recipes (Smithing, Knapping, Clayforming, Grid recipes, Alloys) Loader: 1<br/>
        /// </summary>
        /// <returns></returns>
        public virtual double ExecuteOrder()
        {
            return 0.1;
        }

        /// <summary>
        /// Called during intial mod loading, called before any mod receives the call to Start()
        /// </summary>
        /// <param name="api"></param>
        public virtual void StartPre(ICoreAPI api)
        {

        }

        /// <summary>
        /// Start method, called on both server and client after all mods already received a call to StartPre(), but before Blocks/Items/Entities/Recipes etc are loaded and some time before StartServerSide / StartClientSide.
        /// <br/>Typically used to register for events and network packets etc
        /// <br/>Typically also used in a mod's core to register the classes for your blocks, items, entities, blockentities, behaviors etc, prior to loading assets
        /// <br/><br/>Do not make calls to api.Assets at this stage, the assets may not be found, resulting in errors (even if the json file exists on disk). Use AssetsLoaded() stage instead.
        /// </summary>
        /// <param name="api"></param>
        public virtual void Start(ICoreAPI api)
        {

        }


        /// <summary>
        /// Called on the server or the client; implementing code may need to check which side it is.
        /// <br/>On a server, called only after all mods have received Start(), and after asset JSONs have been read from disk and patched, but before runphase ModsAndConfigReady.
        /// <br/>Asset files are now available to load using api.Assets.TryGet() calls or similar. If your execute order is below 0.2, blocks and items are not yet registered at this point, if below 0.6 recipes are not yet registered.
        /// </summary>
        /// <param name="api"></param>
        public virtual void AssetsLoaded(ICoreAPI api)
        {

        }

        /// <summary>
        /// When called on a server, all Block.OnLoaded() methods etc. have already been called, this is for any final asset set-up steps to be done after that.  See VSSurvivalMod system BlockReinforcement.cs for an example.
        /// </summary>
        /// <param name="api"></param>
        public virtual void AssetsFinalize(ICoreAPI api)
        {

        }


        /// <summary>
        /// Full start to the mod on the client side.
        /// <br/>Note, in multiplayer games, the server assets (blocks, items, entities, recipes) have not yet been received and so no blocks etc. are yet registered
        /// <br/>For code that must run only after we have blocks, items, entities and recipes all registered and loaded, add your method to event BlockTexturesLoaded
        /// </summary>
        /// <param name="api"></param>
        public virtual void StartClientSide(ICoreClientAPI api)
        {

        }

        /// <summary>
        /// Full start to the mod on the server side
        /// <br/><br/>Note: preferably, your code which adds or updates behaviors or attributes or other fixed properties of any block, item or entity, should have been run before now.
        /// For example, code which needs to do that could be placed in an overridden AssetsFinalize() method. See VSSurvivalMod system BlockReinforcement.cs for an example.
        /// </summary>
        /// <param name="api"></param>
        public virtual void StartServerSide(ICoreServerAPI api)
        {

        }

        /// <summary>
        /// If this mod allows runtime reloading, you must implement this method to unregister any listeners / handlers
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}
