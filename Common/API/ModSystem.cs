using System;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

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
        public virtual bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        /// <summary>
        /// If you need mods to be executed in a certain order, adjust this methods return value.
        /// The server will call each Mods Start() method the ascending order of each mods execute order value. And thus, as long as every mod registers it's event handlers in the Start() method, all event handlers will be called in the same execution order.
        /// Default execute order of some survival mod parts
        /// Worldgen:
        /// - GenTerra: 0 
        /// - RockStrata: 0.1
        /// - Deposits: 0.2
        /// - Caves: 0.3
        /// - Blocklayers: 0.4
        /// Asset Loading
        /// - Json Overrides loader: 0.05
        /// - Load hardcoded mantle block: 0.1
        /// - Block and Item Loader: 0.2
        /// - Recipes (Smithing, Knapping, Clayforming, Grid recipes, Alloys) Loader: 1
        /// </summary>
        /// <returns></returns>
        public virtual double ExecuteOrder()
        {
            return 0.1;
        }

        /// <summary>
        /// When the server reloads mods at runtime, should this mod also be reloaded. Return false e.g. for any mod that adds blocks.
        /// </summary>
        /// <returns></returns>
        public virtual bool AllowRuntimeReload
        {
            get { return false; }
        }


        /// <summary>
        /// Called during intial mod loading, called before any mod receives the call to Start()
        /// </summary>
        /// <param name="api"></param>
        public virtual void StartPre(ICoreAPI api)
        {

        }

        /// <summary>
        /// Side agnostic Start method, called after all mods received a call to StartPre().
        /// </summary>
        /// <param name="api"></param>
        public virtual void Start(ICoreAPI api)
        {

        }


        /// <summary>
        /// Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="api"></param>
        public virtual void StartClientSide(ICoreClientAPI api)
        {

        }

        /// <summary>
        /// Minor convenience method to save yourself the check for/cast to ICoreServerAPI in Start()
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
