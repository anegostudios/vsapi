using System;

#nullable disable

namespace Vintagestory.API.Client
{

    /// <summary>
    /// Interface to render something on to the clients screens. Used for block entitites.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        /// <summary>
        /// 0 = drawn first, 1 = drawn last<br/>
        /// Default render orders by render stage:<br/>
        /// Before:<br/>
        /// 0 = Ambient Manager<br/>
        /// 0 = Camera<br/>
        /// <br/>
        /// Opaque:<br/>
        /// 0.1 = Blue sky (Icosahedron)<br/>
        /// 0.2 = Night skybox<br/>
        /// 0.3 = Sun and moon<br/>
        /// 0.37 = Terrain opaque<br/>
        /// 0.4 = Enitities<br/>
        /// 0.5 = Decals<br/>
        /// 0.5 = Debug wireframe<br/>
        /// 0.6 = particles<br/>
        /// 0.7 = Cinematic camera line preview<br/>
        /// 0.8 = fp held item<br/>
        /// 0.9 = held item opaque custom renderer<br/>
        /// <br/>
        /// OIT:<br/>
        /// 0.2 = Frame buffer debug screen<br/>
        /// 0.35 = Clouds<br/>
        /// 0.37 = Terrain oit<br/>
        /// 0.4 = Enitities<br/>
        /// 0.6 = particles<br/>
        /// 0.9 = held item oit custom renderer<br/>
        /// <br/>
        /// Shadow far:<br/>
        /// 0 = shadow map init<br/>
        /// 0.37 = Terrain shadow far<br/>
        /// 0.4 = Enitities<br/>
        /// <br/>
        /// Shadow far done:<br/>
        /// 1 = shadow map finish<br/>
        /// <br/>
        /// Shadow near:<br/>
        /// 0 = shadow map init<br/>
        /// 0.37 = Terrain shadow near<br/>
        /// 0.4 = Enitities<br/>
        /// <br/>
        /// Shadow near done:<br/>
        /// 1 = shadow map finish<br/>
        /// Ortho:<br/>
        /// 0.2 = Frame buffer debug screen<br/>
        /// 0.4 = Enitities<br/>
        /// 0.9 = held item ortho custom renderer<br/>
        /// 0.95 = sleeping overlay<br/>
        /// 0.98 = bow/spear aiming reticle<br/>
        /// 1 = Gui manager<br/>
        /// 1.02 = crosshair and mouse cursor<br/>
        /// <br/>
        /// AfterFinalComposition:<br/>
        /// 2 = screenshot<br/>
        /// <br/>
        /// Done:<br/>
        /// 0.1 = gui manager<br/>
        /// 2 = screenshot<br/>
        /// <br/>
        /// 0.98 = Cinematic camera camera advancing and frame capture when recording<br/>
        /// 0.99 = Chunk Tesselator Manager (uploads new/modified chunk meshes)<br/>
        /// 0.999 = Compress chunks scan<br/>
        /// 1 = video recorder<br/>
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
    }
}
