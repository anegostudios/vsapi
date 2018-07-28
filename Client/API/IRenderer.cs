namespace Vintagestory.API.Client
{
    /// <summary>
    /// Interface to render something on to the clients screens. Used for block entitites.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// 0 = drawn first, 1 = drawn last
        /// </summary>
        double RenderOrder { get; }

        /// <summary>
        /// Within what range to the player OnRenderFrame() should be called (currently not used!)
        /// </summary>
        int RenderRange { get; }

        /// <summary>
        /// Called every frame for rendering whatever you need to render
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="stage"></param>
        void OnRenderFrame(float deltaTime, EnumRenderStage stage);

        /// <summary>
        /// Called when the renderer was unregistered. Should free up the gpu memory you reserved.
        /// </summary>
        void Dispose();
    }
}
