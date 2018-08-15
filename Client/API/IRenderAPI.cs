using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// The main api component to assist you in rendering pretty stuff onto the screen
    /// </summary>
    public interface IRenderAPI
    {
        /// <summary>
        /// The currently configured z-far plane
        /// </summary>
        float Zfar { get; }
        /// <summary>
        /// The currently configured z-near plane
        /// </summary>
        float ZNear { get; }

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

        string DecorativeFontName { get; }
        string StandardFontName { get; }

        /// <summary>
        /// Width of the primary render framebuffer
        /// </summary>
        int FrameWidth { get; }

        /// <summary>
        /// Height of the primary render framebuffer
        /// </summary>
        int FrameHeight { get; }

        EnumCameraMode CameraType { get; }

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
        /// <param name="itemstack"></param>
        /// <param name="ground"></param>
        /// <returns></returns>
        ItemRenderInfo GetItemStackRenderInfo(ItemStack itemstack, EnumItemRenderTarget ground);



        #region OpenGL
        /// <summary>
        /// Returns null if no OpenGL Error happened, otherwise one of the official opengl error codes
        /// </summary>
        /// <returns></returns>
        string GlGetError();


        void GlMatrixModeModelView();

        /// <summary>
        /// Pushes a copy of the current matrix onto the games matrix stack
        /// </summary>
        void GlPushMatrix();

        /// <summary>
        /// Pops the top most matrix from the games matrix stack
        /// </summary>
        void GlPopMatrix();

        /// <summary>
        /// Replaces the top most matrix with given one
        /// </summary>
        /// <param name="matrix"></param>
        void GlLoadMatrix(double[] matrix);

        /// <summary>
        /// Translates top most matrix in the games matrix stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void GlTranslate(float x, float y, float z);

        /// <summary>
        /// Translates top most matrix in the games matrix stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void GlTranslate(double x, double y, double z);

        /// <summary>
        /// Scales top most matrix in the games matrix stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void GlScale(float x, float y, float z);

        /// <summary>
        /// Rotates top most matrix in the games matrix stack
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void GlRotate(float angle, float x, float y, float z);


        void GlEnableCullFace();
        void GlDisableCullFace();
        
        void GLDisableDepthTest();
        void GLEnableDepthTest();

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
        /// Convenience method for GlScissor(). Can be turned of again with EndClipArea()
        /// </summary> 
        /// <param name="bounds"></param>
        void BeginScissor(ElementBounds bounds);
        void EndScissor();


        void GlScissor(int x, int y, int width, int height);
        void GlScissorFlag(bool enable);



        #endregion

        #region Texture

        BitmapExternal BitmapCreateFromPng(byte[] pngdata);

        int LoadTextureFromBgra(int[] rgbaPixels, int width, int height, bool linearMag, int clampMode);
        int LoadTextureFromRgba(int[] rgbaPixels, int width, int height, bool linearMag, int clampMode);
        

        /// <summary>
        /// Deletes given texture
        /// </summary>
        /// <param name="textureId"></param>
        void GLDeleteTexture(int textureId);
        

        /// <summary>
        /// Max size a texture can have on the current graphics card
        /// </summary>
        /// <returns></returns>
        int GlGetMaxTextureSize();

        /// <summary>
        /// Binds given texture. For use with shaders - you should assign the texture directly though shader uniforms.
        /// </summary>
        /// <param name="textureid"></param>
        void BindTexture2d(int textureid);

        /// <summary>
        /// Loads given texture through the assets managers and loads it onto the graphics card. Will return a cached version on every subsequent call to this method. Returns a textureid ready to be used in BindTexture2d
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetOrLoadTexture(AssetLocation name);

        /// <summary>
        /// Loads the texture supplied by the bitmap, uploads it to the graphics card and keeps a cached version under given name. Will return that cached version on every subsequent call to this method. Returns a textureid ready to be used in BindTexture2d
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bmp"></param>
        /// <returns></returns>
        int GetOrLoadTexture(AssetLocation name, BitmapRef bmp);

        /// <summary>
        /// Removes given texture from the cache and from graphics card memory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DeleteTexture(AssetLocation name);

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
        /// <returns></returns>
        IStandardShaderProgram PreparedStandardShader(int posX, int posY, int posZ);

        /// <summary>
        /// Gives you a reference to the currently active shader, or null if none is active right now
        /// </summary>
        IShaderProgram CurrentActiveShader { get; }

        #endregion

        #region Mesh Buffer

        /// <summary>
        /// Allocates memory on the graphics card. Can use UpdateMesh() to populate it with data. The custom mesh data parts may be null. Sizes are in bytes.
        /// </summary>
        /// <param name="xyzSize"></param>
        /// <param name="normalSize"></param>
        /// <param name="uvSize"></param>
        /// <param name="rgbaSize"></param>
        /// <param name="rgba2Size"></param>
        /// <param name="flagsSize"></param>
        /// <param name="indicesSize"></param>
        /// <param name="customFloats"></param>
        /// <param name="customBytes"></param>
        /// <param name="drawMode"></param>
        /// <param name="staticDraw"></param>
        /// <returns></returns>
        MeshRef AllocateEmptyMesh(int xyzSize, int normalSize, int uvSize, int rgbaSize, int rgba2Size, int flagsSize, int indicesSize, CustomMeshDataPartFloat customFloats, CustomMeshDataPartByte customBytes, CustomMeshDataPartInt customInts, EnumDrawMode drawMode = EnumDrawMode.Triangles, bool staticDraw = true);

        /// <summary>
        /// Will load your mesh into a VAO. VBO locations:
        /// xyz=0, uv=1, rgba=2, rgba2=3, flags=4, customFloats=5, customInts=6, customBytes=7  (indices do not get their own data location)
        /// If any of them are null, the vbo location is not consumed and all used location numbers shift by -1
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        MeshRef UploadMesh(MeshData data);

        /// <summary>
        /// Updates the existing mesh any non null data from updatedata
        /// </summary>
        /// <param name="meshRef"></param>
        /// <param name="updatedata"></param>
        void UpdateMesh(MeshRef meshRef, MeshData updatedata);

        /// <summary>
        /// Frees up the memory on the graphics card. Should always be called at the end of the lifetime to prevent memory leaks.
        /// </summary>
        /// <param name="vao"></param>
        void DeleteMesh(MeshRef vao);

        #endregion

        #region Render 

        /// <summary>
        /// Renders given mesh onto the screen
        /// </summary>
        /// <param name="meshRef"></param>
        void RenderMesh(MeshRef meshRef);

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
        /// Renders given texture into another texture. If you use the resulting texture for in-world rendering, remember to recreate the mipmaps via api.Render.GlGenerateTex2DMipmaps()
        /// </summary>
        /// <param name="fromTexture"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="sourceWidth"></param>
        /// <param name="sourceHeight"></param>
        /// <param name="intoTexture"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        void RenderTextureIntoTexture(LoadedTexture fromTexture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, LoadedTexture intoTexture, float targetX, float targetY, float alphaTest = 0.005f);

        /// <summary>
        /// Renders given itemstack at given position (gui mode)
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="shading"></param>
        /// <param name="rotate"></param>
        void RenderItemstackToGui(ItemStack itemstack, double posX, double posY, double posZ, float size, int color, bool shading = true, bool rotate = false, bool showStackSize = true);

        /// <summary>
        /// Renders given etnity at given position (gui mode)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="entity"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="yawDelta"></param>
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



        #endregion

        #region Util

        /// <summary>
        /// For use in GenTextTexture
        /// </summary>
        /// <param name="unscaledFontSize"></param>
        /// <param name="fontName"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        ICairoFont GetFont(double unscaledFontSize, string fontName, double[] color, double[] strokeColor = null);

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
        

        #endregion
    }
}
