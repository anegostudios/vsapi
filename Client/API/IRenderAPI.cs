using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class WireframeModes
    {
        public bool Chunk;
        public bool BlockEntity;
        public bool Region;
        public bool LandClaim;
        public bool ServerChunk;
        public bool Entity;
        public bool AmbientSounds;
        public bool Vertex;
        public bool Inside;
        public bool Smoothstep;
        public bool Structures;
    }

    /// <summary>
    /// The main api component to assist you in rendering pretty stuff onto the screen
    /// </summary>
    public interface IRenderAPI
    {
        WireframeModes WireframeDebugRender { get; }

        PerceptionEffects PerceptionEffects { get; }

        Stack<ElementBounds> ScissorStack { get; }
        int TextureSize { get; }

        FrustumCulling DefaultFrustumCuller { get; }

        /// <summary>
        /// List of all loaded frame buffers. To get the god rays frame buffer for exampple, do <code>Framebuffers[(int)EnumFrameBuffer.GodRays]</code>
        /// </summary>
        List<FrameBufferRef> FrameBuffers { get; }

        /// <summary>
        /// Set the current framebuffer
        /// </summary>
        FrameBufferRef FrameBuffer { set; }

        /// <summary>
        /// A number of default shader uniforms
        /// </summary>
        DefaultShaderUniforms ShaderUniforms { get; }
        
        /// <summary>
        /// Can be used to offset the position of the player camera
        /// </summary>
        ModelTransform CameraOffset { get; }

        /// <summary>
        /// The render stage the engine is currently at
        /// </summary>
        EnumRenderStage CurrentRenderStage { get; }

        /// <summary>
        /// The default view matrix used during perspective rendering. Is refreshed before EnumRenderStage.Opaque. Useful for doing projections in the Ortho stage via MatrixToolsd.Project()
        /// </summary>
        double[] PerspectiveViewMat { get; }
        
        /// <summary>
        /// The default projection matrix used during perspective rendering. Is refreshed before EnumRenderStage.Opaque. Useful for doing projections in the Ortho stage via MatrixToolsd.Project()
        /// </summary>
        double[] PerspectiveProjectionMat { get; }

        /// <summary>
        /// The name of the font used during this render (if it exists).
        /// </summary>
        [Obsolete("Please use ElementGeometrics.DecorativeFontName instead")]
        string DecorativeFontName { get; }

        /// <summary>
        /// The standard font used during this render (if it exists).
        /// </summary>
        [Obsolete("Please use ElementGeometrics.StandardFontName instead.")]
        string StandardFontName { get; }

        /// <summary>
        /// Width of the primary render framebuffer
        /// </summary>
        int FrameWidth { get; }

        /// <summary>
        /// Height of the primary render framebuffer
        /// </summary>
        int FrameHeight { get; }

        /// <summary>
        /// The desired camera type. Be aware, the actual camera type can change at runtime if the player is against a wall and tries to look from inside the wall - use entityPlayer.CameraMode to get the actual mode
        /// </summary>
        EnumCameraMode CameraType { get; }

        /// <summary>
        /// True if when in IFP mode the camera would end up inside blocks
        /// </summary>
        bool CameraStuck { get; }

        /// <summary>
        /// The current modelview matrix stack
        /// </summary>
        StackMatrix4 MvMatrix { get; }

        /// <summary>
        /// The current projection matrix stack
        /// </summary>
        StackMatrix4 PMatrix { get; }

        /// <summary>
        /// Returns you a render info object of given item stack. Can be used to render held items onto a creature.
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="ground"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        ItemRenderInfo GetItemStackRenderInfo(ItemSlot inSlot, EnumItemRenderTarget ground, float dt);


        #region OpenGL

        void Reset3DProjection();
        void Set3DProjection(float zfar, float fov);

        /// <summary>
        /// Returns null if no OpenGL Error happened, otherwise one of the official opengl error codes
        /// </summary>
        /// <returns></returns>
        string GlGetError();

        /// <summary>
        /// If opengl debug mode is enabled and an opengl error is found this method will throw an exception. 
        /// It is recommended to use this methods in a few spots during render code to track down rendering issues in time.
        /// </summary>
        /// <param name="message"></param>
        void CheckGlError(string message = "");

        /// <summary>
        /// The current model view.
        /// </summary>
        void GlMatrixModeModelView();

        /// <summary>
        /// Pushes a copy of the current matrix onto the games matrix stack
        /// </summary>
//        [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlPushMatrix();

        /// <summary>
        /// Pops the top most matrix from the games matrix stack
        /// </summary>
//        [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlPopMatrix();

        /// <summary>
        /// Replaces the top most matrix with given one
        /// </summary>
        /// <param name="matrix"></param>
//        [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlLoadMatrix(double[] matrix);

        /// <summary>
        /// Translates top most matrix in the games matrix stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
 //       [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlTranslate(float x, float y, float z);

        /// <summary>
        /// Translates top most matrix in the games matrix stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
 //       [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlTranslate(double x, double y, double z);

        /// <summary>
        /// Scales top most matrix in the games matrix stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
//        [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlScale(float x, float y, float z);

        /// <summary>
        /// Rotates top most matrix in the games matrix stack
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
 //       [Obsolete("Use Matrix or Mat4f to perform matrix calculations, see Survival Mod Code for examples. Can still be used for GUI code since there is no alternative as of yet.")]
        void GlRotate(float angle, float x, float y, float z);

        /// <summary>
        /// Enables the Culling faces.
        /// </summary>
        void GlEnableCullFace();

        /// <summary>
        /// Disables the culling faces.
        /// </summary>
        void GlDisableCullFace();

        /// <summary>
        /// Enables the Depth Test.
        /// </summary>
        void GLEnableDepthTest();       
        
        /// <summary>
        /// Disables the Depth Test.
        /// </summary>
        void GLDisableDepthTest();

        void GlViewport(int x, int y, int width, int height);

        float LineWidth { set; }

        /// <summary>
        /// Toggle writing to the depth buffer
        /// </summary>
        /// <param name="on"></param>
        void GLDepthMask(bool on);

        /// <summary>
        /// Regenerates the mip maps for the currently bound texture
        /// </summary>
        void GlGenerateTex2DMipmaps();

        /// <summary>
        /// To enable/disable various blending modes
        /// </summary>
        /// <param name="blend"></param>
        /// <param name="blendMode"></param>
        void GlToggleBlend(bool blend, EnumBlendMode blendMode = EnumBlendMode.Standard);


        /// <summary>
        /// The current top most matrix in the model view matrix stack. 
        /// </summary>
        float[] CurrentModelviewMatrix
        {
            get;
        }

        /// <summary>
        /// Player camera matrix with player positioned at 0,0,0.
        /// You can use this matrix instead of <see cref="CurrentModelviewMatrix"/> for high precision rendering.
        /// </summary>
        double[] CameraMatrixOrigin { get; }

        /// <summary>
        /// Player camera matrix with player positioned at 0,0,0.
        /// You can use this matrix instead of <see cref="CurrentModelviewMatrix"/> for high precision rendering.
        /// </summary>
        float[] CameraMatrixOriginf { get; }


        /// <summary>
        /// The current top most matrix in the projection matrix stack
        /// </summary>
        float[] CurrentProjectionMatrix
        {
            get;
        }

        /// <summary>
        /// The current projection matrix for shadow rendering (renders the scene from the viewpoint of the sun)
        /// </summary>
        float[] CurrentShadowProjectionMatrix
        {
            get;
        }

        /// <summary>
        /// Convenience method for GlScissor(). Tells the graphics card to not render anything outside supplied bounds. Can be turned of again with PopScissor(). Any previously applied scissor will be restored after calling PopScissor().
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="stacking">If true, also applies scissoring from the previous call to PushScissor, otherwise replaces the scissor bounds</param>
        void PushScissor(ElementBounds bounds, bool stacking = false);

        /// <summary>
        /// End scissor mode. Disable any previously set render constraints
        /// </summary>
        void PopScissor();

        /// <summary>
        /// Tells the graphics card to not render anything outside supplied bounds. Only sets the boundaries. Can be turned on/off with GlScissorFlag(true/false)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void GlScissor(int x, int y, int width, int height);

        /// <summary>
        /// Whether scissor mode should be active or not
        /// </summary>
        /// <param name="enable"></param>
        void GlScissorFlag(bool enable);



        #endregion

        #region Texture
        /// <summary>
        /// Creates a bitmap from a given PNG.
        /// </summary>
        /// <param name="pngdata">the PNG data passed in.</param>
        /// <returns>A bitmap object.</returns>
        BitmapExternal BitmapCreateFromPng(byte[] pngdata);

        /// <summary>
        /// Loads texture from Pixels in BGRA format.
        /// </summary>
        /// <param name="bgraPixels">The pixel array</param>
        /// <param name="width">the width of the final texture</param>
        /// <param name="height">the height of the final texture</param>
        /// <param name="linearMag">Enable/Disable Linear rendering or use Nearest rendering.</param>
        /// <param name="clampMode">The current clamp mode</param>
        /// <returns>The GLID for the resulting texture.</returns>
        [Obsolete("Use LoadOrUpdateTextureFromBgra(int[] bgraPixels, bool linearMag, int clampMode, ref LoadedTexture intoTexture); instead. This method cannot warn you of memory leaks when the texture is not properly disposed.")]
        int LoadTextureFromBgra(int[] bgraPixels, int width, int height, bool linearMag, int clampMode);

        /// <summary>
        /// Loads texture from Pixels in RGBA format.
        /// </summary>
        /// <param name="rgbaPixels">The pixel array</param>
        /// <param name="width">the width of the final texture</param>
        /// <param name="height">the height of the final texture</param>
        /// <param name="linearMag">Enable/Disable Linear rendering or use Nearest rendering.</param>
        /// <param name="clampMode">The current clamp mode</param>
        /// <returns>The OpenGL Identifier for the resulting texture.</returns>
        [Obsolete("Use LoadOrUpdateTextureFromRgba(int[] bgraPixels, bool linearMag, int clampMode, ref LoadedTexture intoTexture); instead. This method cannot warn you of memory leaks when the texture is not properly disposed.")]
        int LoadTextureFromRgba(int[] rgbaPixels, int width, int height, bool linearMag, int clampMode);


        /// <summary>
        /// Loads texture from Pixels in BGRA format.
        /// </summary>
        /// <param name="bgraPixels">The pixel array</param>
        /// <param name="linearMag">Enable/Disable Linear rendering or use Nearest rendering.</param>
        /// <param name="clampMode">The current clamp mode</param>
        /// <param name="intoTexture">The target texture space it should load the pixels into. Must have width/height set accordingly. Will set the opengl textureid upon successful load</param>
        /// <returns></returns>
        void LoadOrUpdateTextureFromBgra(int[] bgraPixels, bool linearMag, int clampMode, ref LoadedTexture intoTexture);

        /// <summary>
        /// Loads texture from Pixels in RGBA format.
        /// </summary>
        /// <param name="rgbaPixels">The pixel array</param>
        /// <param name="linearMag">Enable/Disable Linear rendering or use Nearest rendering.</param>
        /// <param name="clampMode">The current clamp mode</param>
        /// <param name="intoTexture">The target texture space it should load the pixels into. Must have width/height set accordingly. Will set the opengl textureid upon successful load.</param>
        /// <returns></returns>
        void LoadOrUpdateTextureFromRgba(int[] rgbaPixels, bool linearMag, int clampMode, ref LoadedTexture intoTexture);

        void LoadTexture(IBitmap bmp, ref LoadedTexture intoTexture, bool linearMag = false, int clampMode = 0, bool generateMipmaps = false);


        /// <summary>
        /// Deletes given texture
        /// </summary>
        /// <param name="textureId">the OpenGL Identifier for the target Texture.</param>
        void GLDeleteTexture(int textureId);
        

        /// <summary>
        /// Max size a texture can have on the current graphics card
        /// </summary>
        /// <returns>The maximum size a texture can have on the current graphics card in Pixels.</returns>
        int GlGetMaxTextureSize();

        /// <summary>
        /// Binds given texture. For use with shaders - you should assign the texture directly though shader uniforms.
        /// </summary>
        /// <param name="textureid">The OpenGL Identifier ID for the target texture to bind.</param>
        void BindTexture2d(int textureid);

        /// <summary>
        /// Loads given texture through the assets managers and loads it onto the graphics card. Will return a cached version on every subsequent call to this method. 
        /// </summary>
        /// <param name="name">the location of the texture as it exists within the game or mod directory.</param>
        /// <returns>The texture id</returns>
        int GetOrLoadTexture(AssetLocation name);

        /// <summary>
        /// Loads given texture through the assets managers and loads it onto the graphics card. Will return a cached version on every subsequent call to this method. 
        /// </summary>
        /// <param name="name">the location of the texture as it exists within the game or mod directory.</param>
        /// <param name="intoTexture">the texture object to be populated. If it already is populated it will be disposed first</param>
        /// <returns></returns>
        void GetOrLoadTexture(AssetLocation name, ref LoadedTexture intoTexture);

        /// <summary>
        /// Loads the texture supplied by the bitmap, uploads it to the graphics card and keeps a cached version under given name. Will return that cached version on every subsequent call to this method. 
        /// </summary>
        /// <param name="name">the location of the texture as it exists within the game or mod directory.</param>
        /// <param name="bmp">The referenced bitmap</param>
        /// <param name="intoTexture">the texture object to be populated. If it already is populated it will be disposed first</param>
        /// <returns></returns>
        void GetOrLoadTexture(AssetLocation name, BitmapRef bmp, ref LoadedTexture intoTexture);

        /// <summary>
        /// Removes given texture from the cache and from graphics card memory
        /// </summary>
        /// <param name="name">the location of the texture as it exists within the game or mod directory.</param>
        /// <returns>whether the operation was successful or not.</returns>
        bool RemoveTexture(AssetLocation name);

        #endregion

        #region Shader


        /// <summary>
        /// Gets you the uniform location of given uniform for given shader
        /// </summary>
        /// <param name="shaderProgramNumber"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetUniformLocation(int shaderProgramNumber, string name);

        /// <summary>
        /// Gives you access to all of the vanilla shaders
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        IShaderProgram GetEngineShader(EnumShaderProgram program);

        /// <summary>
        /// Gives you access to all currently registered shaders identified by their number
        /// </summary>
        /// <param name="shaderProgramNumber"></param>
        /// <returns></returns>
        IShaderProgram GetShader(int shaderProgramNumber);

        /// <summary>
        /// Gives you a reference to the "standard" shader, a general purpose shader for normal shading work
        /// </summary>
        IStandardShaderProgram StandardShader { get; }

        /// <summary>
        /// Populates the uniforms and light values for given positions and calls shader.Use().
        /// </summary>
        /// <param name="posX">The position for light level reading</param>
        /// <param name="posY">The position for light level reading</param>
        /// <param name="posZ">The position for light level reading</param>
        /// <param name="colorMul"></param>
        /// <returns></returns>
        IStandardShaderProgram PreparedStandardShader(int posX, int posY, int posZ, Vec4f colorMul = null);

        /// <summary>
        /// Gives you a reference to the currently active shader, or null if none is active right now
        /// </summary>
        IShaderProgram CurrentActiveShader { get; }

        #endregion

        #region Mesh Buffer

        /// <summary>
        /// Allocates memory on the graphics card. Can use UpdateMesh() to populate it with data. The custom mesh data parts may be null. Sizes are in bytes.
        /// </summary>
        /// <param name="xyzSize">the squared size of the texture.</param>
        /// <param name="normalSize">the size of the normals</param>
        /// <param name="uvSize">the size of the UV map.</param>
        /// <param name="rgbaSize">size of the RGBA colors.</param>
        /// <param name="flagsSize">Size of the render flags.</param>
        /// <param name="indicesSize">Size of the indices</param>
        /// <param name="customFloats">Float values of the mesh</param>
        /// <param name="customInts">Float values of the mesh</param>
        /// <param name="customShorts"></param>
        /// <param name="customBytes">Byte values of the mesh</param>
        /// <param name="drawMode">The current draw mode</param>
        /// <param name="staticDraw">whether the draw should be static or dynamic.</param>
        /// <returns>the reference to the mesh</returns>
        MeshRef AllocateEmptyMesh(int xyzSize, int normalSize, int uvSize, int rgbaSize, int flagsSize, int indicesSize, CustomMeshDataPartFloat customFloats, CustomMeshDataPartShort customShorts, CustomMeshDataPartByte customBytes, CustomMeshDataPartInt customInts, EnumDrawMode drawMode = EnumDrawMode.Triangles, bool staticDraw = true);

        /// <summary>
        /// Uploads your mesh onto the graphics card for rendering (= load into a <see href="https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object">VAO</see>).<br/><br/>
        /// If you use a custom shader, these are the VBO locations:
        /// xyz=0, uv=1, rgba=2, rgba2=3, flags=4, customFloats=5, customInts=6, customBytes=7  (indices do not get their own data location)<br/>
        /// If any of them are null, the vbo location is not consumed and all used location numbers shift by -1
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        MeshRef UploadMesh(MeshData data);

        /// <summary>
        /// Same as <seealso cref="UploadMesh(MeshData)"/> but splits it into multiple MeshRefs, one for each texture
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        MultiTextureMeshRef UploadMultiTextureMesh(MeshData data);

        /// <summary>
        /// Updates the existing mesh. Updates any non null data from <paramref name="updatedata"/>
        /// </summary>
        /// <param name="meshRef"></param>
        /// <param name="updatedata"></param>
        void UpdateMesh(MeshRef meshRef, MeshData updatedata);

        /// <summary>
        /// Updates the existing mesh. Updates any non null data from <paramref name="updatedata"/>. Version for chunks only (also decals, as they use chunk MeshDataPool system)
        /// </summary>
        /// <param name="meshRef"></param>
        /// <param name="updatedata"></param>
        void UpdateChunkMesh(MeshRef meshRef, MeshData updatedata);

        /// <summary>
        /// Frees up the memory on the graphics card. Should always be called at the end of a meshes lifetime to prevent memory leaks. Equivalent to calling Dispose on the meshref itself
        /// </summary>
        /// <param name="vao"></param>
        void DeleteMesh(MeshRef vao);

        #endregion

        #region UBO Buffer
        UBORef CreateUBO(IShaderProgram shaderProgram, int bindingPoint, string blockName, int size);

        #endregion



        #region Render 

        /// <summary>
        /// Renders given mesh onto the screen
        /// </summary>
        /// <param name="meshRef"></param>
        void RenderMesh(MeshRef meshRef);

        /// <summary>
        /// Renders given mesh onto the screen, with the mesh requiring multiple render calls for each texture, asigns the associated texture each call
        /// </summary>
        /// <param name="mmr"></param>
        /// <param name="textureSampleName"></param>
        /// <param name="textureNumber"></param>
        void RenderMultiTextureMesh(MultiTextureMeshRef mmr, string textureSampleName, int textureNumber = 0);


        /// <summary>
        /// Uses the graphics instanced rendering methods to efficiently render the same mesh multiple times. Use the custom mesh data parts with instanced flag on to supply custom data to each mesh.
        /// </summary>
        /// <param name="meshRef"></param>
        /// <param name="quantity"></param>
        void RenderMeshInstanced(MeshRef meshRef, int quantity = 1);

        /// <summary>
        /// Draws only a part of the mesh
        /// </summary>
        /// <param name="meshRef"></param>
        /// <param name="indicesStarts"></param>
        /// <param name="indicesSizes"></param>
        /// <param name="groupCount"></param>
        void RenderMesh(MeshRef meshRef, int[] indicesStarts, int[] indicesSizes, int groupCount);

        /// <summary>
        /// Renders given texture into another texture. If you use the resulting texture for in-world rendering, remember to recreate the mipmaps via <seealso cref="GlGenerateTex2DMipmaps"/>
        /// </summary>
        /// <param name="fromTexture"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="sourceWidth"></param>
        /// <param name="sourceHeight"></param>
        /// <param name="intoTexture"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <param name="alphaTest">If below given threshold, the pixel is not drawn into the target texture. (Default: 0.05)</param>
        void RenderTextureIntoTexture(LoadedTexture fromTexture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, LoadedTexture intoTexture, float targetX, float targetY, float alphaTest = 0.005f);

        /// <summary>
        /// Renders given itemstack at given position (gui/orthographic mode)
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="size"></param>
        /// <param name="color">Set to <seealso cref="ColorUtil.WhiteArgb"/> for normal rendering</param> 
        /// <param name="shading">Unused.</param>
        /// <param name="rotate">If true, will slowly rotate the itemstack around the Y-Axis</param>
        /// <param name="showStackSize">If true, will render a number depicting how many blocks/item are in the stack</param>
        [Obsolete("Use RenderItemstackToGui(inSlot, ....) instead")]
        void RenderItemstackToGui(ItemStack itemstack, double posX, double posY, double posZ, float size, int color, bool shading = true, bool rotate = false, bool showStackSize = true);

        /// <summary>
        /// Renders given itemstack in slot at given position (gui/orthographic mode)
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="shading"></param>
        /// <param name="rotate"></param>
        /// <param name="showStackSize"></param>
        void RenderItemstackToGui(ItemSlot inSlot, double posX, double posY, double posZ, float size, int color, bool shading = true, bool rotate = false, bool showStackSize = true);

        /// <summary>
        /// Renders given itemstack in slot at given position (gui/orthographic mode)
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="dt"></param>
        /// <param name="shading"></param>
        /// <param name="rotate"></param>
        /// <param name="showStackSize"></param>
        void RenderItemstackToGui(ItemSlot inSlot, double posX, double posY, double posZ, float size, int color, float dt, bool shading = true, bool rotate = false, bool showStackSize = true);

        /// <summary>
        /// Renders given itemstack into supplied texture atlas. This is a rather costly operation. Also be sure to cache the results, as each call to this method consumes more space in your texture atlas. If you call this method outside the ortho render stage, it will enqueue a render task for next frame. Rather exceptionally, this method is also thread safe. If called from another thread, the render task always gets enqueued. The call back will always be run on the main thread.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="atlas"></param>
        /// <param name="size"></param>
        /// <param name="onComplete">Once rendered, this returns a texture subid, which you can use to retrieve the textureAtlasPosition from the atlas</param>
        /// <param name="color"></param>
        /// <param name="sepiaLevel"></param>
        /// <param name="scale"></param>
        /// <returns>True if the render could complete immediatly, false if it has to wait until the next ortho render stage</returns>
        bool RenderItemStackToAtlas(ItemStack stack, ITextureAtlasAPI atlas, int size, Action<int> onComplete, int color = ColorUtil.WhiteArgb, float sepiaLevel = 0f, float scale = 1f);

        /// <summary>
        /// Returns the first TextureAtlasPosition it can find for given block or item texture in itemstack. 
        /// </summary>
        TextureAtlasPosition GetTextureAtlasPosition(ItemStack itemstack);

        /// <summary>
        /// Renders given entity at given position (gui/orthographic mode)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="entity"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="yawDelta">For rotating the entity around its y-axis</param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        void RenderEntityToGui(float dt, Entity entity, double posX, double posY, double posZ, float yawDelta, float size, int color);


        

        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode). Assumes the texture to use a premultiplied alpha channel
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        void Render2DTexturePremultipliedAlpha(int textureid, float posX, float posY, float width, float height, float z = 50, Vec4f color = null);

        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode). Assumes the texture to use a premultiplied alpha channel
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        void Render2DTexturePremultipliedAlpha(int textureid, double posX, double posY, double width, double height, float z = 50, Vec4f color = null);

        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode). Assumes the texture to use a premultiplied alpha channel
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="bounds"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        void Render2DTexturePremultipliedAlpha(int textureid, ElementBounds bounds, float z = 50, Vec4f color = null);

        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode)
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        void RenderTexture(int textureid, double posX, double posY, double width, double height, float z = 50, Vec4f color = null);

        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode)
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        void Render2DTexture(int textureid, float posX, float posY, float width, float height, float z = 50, Vec4f color = null);


        /// <summary>
        /// Renders given texture onto the screen, uses supplied quad for rendering (gui mode)
        /// </summary>
        /// <param name="quadModel"></param>
        /// <param name="textureid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="z"></param>
        void Render2DTexture(MeshRef quadModel, int textureid, float posX, float posY, float width, float height, float z = 50);

        /// <summary>
        /// Renders given texture onto the screen, uses supplied quad for rendering (gui mode)
        /// </summary>
        /// <param name="quadModel"></param>
        /// <param name="textureid"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="z"></param>
        void Render2DTexture(MultiTextureMeshRef quadModel, float posX, float posY, float width, float height, float z = 50);



        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode)
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="bounds"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        void Render2DTexture(int textureid, ElementBounds bounds, float z = 50, Vec4f color = null);

        /// <summary>
        /// Renders given texture onto the screen, uses a simple quad for rendering (gui mode)
        /// </summary>
        /// <param name="textTexture"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="z"></param>
        void Render2DLoadedTexture(LoadedTexture textTexture, float posX, float posY, float z = 50);

        /// <summary>
        /// Renders a rectangle outline at given position
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        void RenderRectangle(float posX, float posY, float posZ, float width, float height, int color);


        /// <summary>
        /// Inefficiently renders a line between 2 points 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="posX1"></param>
        /// <param name="posY1"></param>
        /// <param name="posZ1"></param>
        /// <param name="posX2"></param>
        /// <param name="posY2"></param>
        /// <param name="posZ2"></param>
        /// <param name="color"></param>
        void RenderLine(BlockPos origin, float posX1, float posY1, float posZ1, float posX2, float posY2, float posZ2, int color);

        FrameBufferRef CreateFrameBuffer(LoadedTexture intoTexture);

        void RenderTextureIntoFrameBuffer(int atlasTextureId, LoadedTexture fromTexture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, FrameBufferRef fb, float targetX, float targetY, float alphaTest = 0.005f);

        void DestroyFrameBuffer(FrameBufferRef fb);

        #endregion

        #region Util

        /// <summary>
        /// The current ambient color (e.g. will return a blue tint when player is under water)
        /// </summary>
       Vec3f AmbientColor { get; }

        /// <summary>
        /// The current fog color (e.g. will return a blue tint when player is under water)
        /// </summary>
        Vec4f FogColor { get; }

        /// <summary>
        /// Current minimum fog value 
        /// </summary>
        float FogMin { get; }

        /// <summary>
        /// Density of the current fog. Fog is calculated as followed in the shaders: clamp(fogMin + 1 - 1 / exp(gl_FragDepth * fogDensity), 0, 1)
        /// </summary>
        float FogDensity { get; }

        /// <summary>
        /// If true, chunk rendering will use SSBOs - a OpenGL 4.30+ feature - for higher performance in game version 1.21+
        /// </summary>
        bool UseSSBOs { get; }


        #endregion


        #region Light

        /// <summary>
        /// Adds a dynamic light source to the scene. Will not be rendered if the current point light count exceeds max dynamic lights in the graphics settings
        /// </summary>
        /// <param name="pointlight"></param>
        void AddPointLight(IPointLight pointlight);

        /// <summary>
        /// Removes a dynamic light source from the scene
        /// </summary>
        /// <param name="pointlight"></param>
        void RemovePointLight(IPointLight pointlight);

        #endregion

    }



    public interface IPointLight
    {
        Vec3f Color { get; }
        Vec3d Pos { get; }
    }

}

