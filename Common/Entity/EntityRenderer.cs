using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Base class for entity rendering
    /// </summary>
    public abstract class EntityRenderer
    {
        /// <summary>
        /// The current entity
        /// </summary>
        public Entity entity;
        /// <summary>
        /// A reference to the client api
        /// </summary>
        public ICoreClientAPI capi;


        /// <summary>
        /// Creates a new entity renderer instance
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="api"></param>
        public EntityRenderer(Entity entity, ICoreClientAPI api)
        {
            this.entity = entity;
            this.capi = api;
        }

        /// <summary>
        /// Called when the entity is now fully either spawned or fully loaded
        /// </summary>
        public virtual void OnEntityLoaded()
        {

        }


        /// <summary>
        /// Draw call with no shader initialized
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isShadowPass"></param>
        public virtual void DoRender3DOpaque(float dt, bool isShadowPass) { }

        /// <summary>
        /// Draw call with the Entityanimated shader loaded and initialized with the correct color/fog/sunlight/texture values
        /// If shadows are enabled, then this method is called again with shadowmap shader intialized
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isShadowPass"></param>
        public virtual void DoRender3DOpaqueBatched(float dt, bool isShadowPass) { }


        public virtual void DoRender3DAfterOIT(float dt, bool isShadowPass) { }

        /// <summary>
        /// Ortho mode draw call for 2d gui stuff, like name tags. Gui shader initialized already.
        /// </summary>
        /// <param name="dt"></param>
        public virtual void DoRender2D(float dt) { }

        /// <summary>
        /// Called before gui rendering starts. Drawing of the whole model into a gui dialog. Gui shader initialized already.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="yawDelta"></param>
        /// <param name="size"></param>
        public virtual void RenderToGui(float dt, double posX, double posY, double posZ, float yawDelta, float size) {
            
        }

        /// <summary>
        /// Called before in-world rendering starts
        /// </summary>
        /// <param name="dt"></param>
        public virtual void BeforeRender(float dt)
        {
            
        }



        /// <summary>
        /// Should free up all the resources
        /// </summary>
        public abstract void Dispose();


        /// <summary>
        /// Render call for the transparent pass
        /// </summary>
        /// <param name="dt"></param>
        public virtual void DoRender3DOIT(float dt)
        {
            
        }

        /// <summary>
        /// Batched render call for the transparent pass
        /// </summary>
        /// <param name="dt"></param>
        public virtual void DoRender3DOITBatched(float dt)
        {
            
        }


    }
}
