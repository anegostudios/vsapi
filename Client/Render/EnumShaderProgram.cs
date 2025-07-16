
#nullable disable
namespace Vintagestory.API.Client
{
    // No camel casing because these names relate to file names
    public enum EnumShaderProgram
    {
        /// <summary>
        /// A very plain shader for drawing any geometry
        /// </summary>
        Standard = 1,
        /// <summary>
        /// For instanced rendering of cubic particles
        /// </summary>
        Particlescube = 2,
        /// <summary>
        /// For instanced rendering of quad particles
        /// </summary>
        Particlesquad = 3,
        //GUI = 4,
        /// <summary>
        /// For rendering the sky colors
        /// </summary>
        Sky = 5,
        /// <summary>
        /// For rendering the stars skybox
        /// </summary>
        Nightsky = 7,
        /// <summary>
        /// Debug shader for testing Weighted Blended Order Independent
        /// </summary>
        Woittest = 8,
        /// <summary>
        /// Merges opaque geomerty with WOIT geometry
        /// </summary>
        Transparentcompose = 9,
        /// <summary>
        /// 
        /// </summary>
        Debugdepthbuffer = 10,
        /// <summary>
        /// For rendering the currently held item
        /// </summary>
        Helditem = 11,
        /// <summary>
        /// Renders opaque chunk geometry, no blend, alpha discard;
        /// </summary>
        Chunkopaque = 12,
        /// <summary>
        /// Debug shader
        /// </summary>
        MultiTextureTest = 13,
        /// <summary>
        /// Renders liquid chunk geometry
        /// </summary>
        Chunkliquid = 14,
        /// <summary>
        /// Renders decals, obviously O_O
        /// </summary>
        Decals = 15,
        /// <summary>
        /// Color grading and merging of all rendered scenes
        /// </summary>
        Final = 16,
        /// <summary>
        /// For drawing an item stack
        /// </summary>
        Gui = 17,
        /// <summary>
        /// The Blur shader
        /// </summary>
        Blur = 18,
        /// <summary>
        /// Renders half transparent chunk geometry using WOIT
        /// </summary>
        Chunktransparent = 19,
        /// <summary>
        /// For bloom shader
        /// </summary>
        Findbright = 20,
        /// <summary>
        /// Renders top soil chunk geometry 
        /// </summary>
        Chunktopsoil = 21,
        /// <summary>
        /// For god rays
        /// </summary>
        Godrays = 22,
        /// <summary>
        /// Cinematic camera pather rendering
        /// </summary>
        Autocamera = 23,
        /// <summary>
        /// Worldedit block highlights
        /// </summary>
        Blockhighlights = 24,
        /// <summary>
        /// The selected block outline
        /// </summary>
        Wireframe = 25,
        /// <summary>
        /// For animated entities
        /// </summary>
        Entityanimated = 26,
        /// <summary>
        /// Luma prepass for FXAA
        /// </summary>
        Luma=27,
        Blit = 28,
        Shadowmap = 29,
        Particlesquad2d = 30,
        Shadowmapentityanimated = 31,
        Chunkshadowmap = 32,
        Texture2texture = 33,
        Celestialobject = 34,
        Guitopsoil = 35,
        Colorgrade = 37,
        Guigear = 38,
        Ssao = 39,
        Bilateralblur = 40,
        Chunkliquiddepth = 41,
        Chunkshadowmap_NoSSBOs = 42
    }
}
