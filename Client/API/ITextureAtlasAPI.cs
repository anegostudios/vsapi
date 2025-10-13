using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate IBitmap CreateTextureDelegate();

    /// <summary>
    /// Item texture Atlas.
    /// </summary>
    public interface IItemTextureAtlasAPI : ITextureAtlasAPI
    {
        /// <summary>
        /// Returns the position in the item texture atlas of given item. For items that don't use custom shapes you don't have to supply the textureName
        /// </summary>
        /// <param name="item"></param>
        /// <param name="textureName"></param>
        /// <param name="returnNullWhenMissing"></param>
        /// <returns></returns>
        TextureAtlasPosition GetPosition(Item item, string textureName = null, bool returnNullWhenMissing = false);
        
    }

    /// <summary>
    /// Block texture Atlas
    /// </summary>
    public interface IBlockTextureAtlasAPI : ITextureAtlasAPI
    {
        /// <summary>
        /// Returns the position in the block texture atlas of given block. 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="textureName"></param>
        /// <param name="returnNullWhenMissing"></param>
        /// <returns></returns>
        TextureAtlasPosition GetPosition(Block block, string textureName, bool returnNullWhenMissing = false);
        
    }

	/// <summary>
    /// Entity texture Atlas.
    /// </summary>
    
    /// <summary>
    /// Texture atlas base.
    /// </summary>
    public interface ITextureAtlasAPI
    {
        TextureAtlasPosition this[AssetLocation textureLocation] { get; }

        /// <summary>
        /// The texture atlas position of the "unknown.png" texture
        /// </summary>
        TextureAtlasPosition UnknownTexturePosition { get; }

        /// <summary>
        /// Size of one block texture atlas
        /// </summary>
        Size2i Size { get; }

        /// <summary>
        /// As configured in the clientsettings.json divided by the texture atlas size
        /// </summary>
        float SubPixelPaddingX { get; }
        float SubPixelPaddingY { get; }

        /// <summary>
        /// Returns the default texture atlas position for all blocks, referenced  by the texturesubid
        /// </summary>
        TextureAtlasPosition[] Positions { get; }

        /// <summary>
        /// Returns the list of currently loaded texture atlas ids
        /// </summary>
        List<LoadedTexture> AtlasTextures { get; }

        /// <summary>
        /// Reserves a spot on the texture atlas. Returns true if allocation was successful.
        /// Can be used to render onto it through the Render API
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="loc">If set, said texture space can be looked up by the tesselated element faces during shape tesselation</param>
        /// <returns></returns>
        bool AllocateTextureSpace(int width, int height, out int textureSubId, out TextureAtlasPosition texPos, AssetLocationAndSource loc = null);

        /// <summary>
        /// Inserts a texture into the texture atlas after the atlas has been generated. Updates the in-ram texture atlas as well as the in-gpu-ram texture atlas. 
        /// The textureSubId can be used to find the TextureAtlasPosition again in case you loose it ;-)
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="alphaTest"></param>
        /// <returns></returns>
        bool InsertTexture(IBitmap bmp, out int textureSubId, out TextureAtlasPosition texPos, float alphaTest = 0f);

        /// <summary>
        /// Inserts a texture into the texture atlas after the atlas has been generated. Updates the in-ram texture atlas as well as the in-gpu-ram texture atlas. 
        /// The textureSubId can be used to find the TextureAtlasPosition again in case you loose it ;-)
        /// </summary>
        /// <param name="pngBytes"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="alphaTest"></param>
        /// <returns></returns>
        bool InsertTexture(byte[] pngBytes, out int textureSubId, out TextureAtlasPosition texPos, float alphaTest = 0f);


        /// <summary>
        /// Loads a bitmap from given asset. Can use ++ syntax for texture overlay and @[int] for texture rotation
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IBitmap LoadCompositeBitmap(AssetLocationAndSource path);

        /// <summary>
        /// Same as <see cref="InsertTexture(IBitmap, out int, out TextureAtlasPosition, float)"/> but this method remembers the inserted texure, which you can access using capi.TextureAtlas[path]
        /// A subsequent call to this method will update the texture, but retain the same texPos. Also a run-time texture reload will reload this texture automatically.
        /// </summary>
        /// <param name="path">Used as reference for caching</param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="onCreate">The method that should load the bitmap, if required. Can be left null to simply attempt to load the bmp from <paramref name="path"/> using method <see cref="LoadCompositeBitmap"/></param>
        /// <param name="alphaTest"></param>
        /// <returns>False if the file was not found or the insert failed</returns>
        bool GetOrInsertTexture(AssetLocationAndSource path, out int textureSubId, out TextureAtlasPosition texPos, CreateTextureDelegate onCreate = null, float alphaTest = 0f);

        /// <summary>
        /// Same as <see cref="InsertTexture(IBitmap, out int, out TextureAtlasPosition, float)"/> but this method remembers the inserted texure, which you can access using capi.TextureAtlas[path]
        /// A subsequent call to this method will update the texture, but retain the same texPos. Also a run-time texture reload will reload this texture automatically.
        /// </summary>
        /// <param name="path">Used as reference for caching</param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="onCreate">The method that should load the bitmap, if required. Can be left null to simply attempt to load the bmp from <paramref name="path"/> using method <see cref="LoadCompositeBitmap"/></param>
        /// <param name="alphaTest"></param>
        /// <returns>False if the file was not found or the insert failed</returns>
        bool GetOrInsertTexture(AssetLocation path, out int textureSubId, out TextureAtlasPosition texPos, CreateTextureDelegate onCreate = null, float alphaTest = 0f);

        /// <summary>
        /// Same as <see cref="InsertTexture(IBitmap, out int, out TextureAtlasPosition, float)"/> but this method remembers the inserted texure, which you can access using capi.TextureAtlas[path]
        /// A subsequent call to this method will update the texture, but retain the same texPos. Also a run-time texture reload will reload this texture automatically.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bmp"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="alphaTest"></param>
        /// <returns></returns>
        [Obsolete("Use GetOrInsertTexture() instead. It's more efficient to load the bmp only if the texture was not found in the cache")]
        bool InsertTextureCached(AssetLocation path, IBitmap bmp, out int textureSubId, out TextureAtlasPosition texPos, float alphaTest = 0f);

        /// <summary>
        /// Same as <see cref="InsertTexture(IBitmap, out int, out TextureAtlasPosition, float)"/> but this method remembers the inserted texure, which you can access using capi.TextureAtlas[path]
        /// A subsequent call to this method will update the texture, but retain the same texPos. Also a run-time texture reload will reload this texture automatically.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pngBytes"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <param name="alphaTest"></param>
        /// <returns></returns>
        bool InsertTextureCached(AssetLocation path, byte[] pngBytes, out int textureSubId, out TextureAtlasPosition texPos, float alphaTest = 0f);


        bool GetOrInsertTexture(CompositeTexture ct, out int textureSubId, out TextureAtlasPosition texPos, float alphaTest = 0f);


        /// <summary>
        /// Deallocates a previously allocated texture space
        /// </summary>
        /// <param name="textureSubId"></param>
        void FreeTextureSpace(int textureSubId);

        /// <summary>
        /// Deallocates a previously allocated texture space
        /// </summary>
        /// <param name="assetloc"></param>
        void FreeTextureSpace(AssetLocationAndSource assetloc);

        /// <summary>
        /// Returns an rgba value picked randomly inside the given texture (defined by its sub-id)
        /// </summary>
        /// <param name="textureSubId"></param>
        /// <returns></returns>
        int GetRandomColor(int textureSubId);

        /// <summary>
        /// Regenerates the mipmaps for one of the atlas textures, given by its array index
        /// </summary>
        /// <param name="atlasIndex"></param>
        void RegenMipMaps(int atlasIndex);

        /// <summary>
        /// Returns one of 30 random rgba values inside the given texture (defined by its sub-id)
        /// </summary>
        /// <param name="textureSubId"></param>
        /// <param name="rndIndex">0..29 for a specific random pixel, or -1 to randomize, which is the same as calling GetRandomColor without the rndIndex argument</param>
        /// <returns></returns>
        int GetRandomColor(int textureSubId, int rndIndex);

        /// <summary>
        /// Returns one of 30 random rgba values inside the given texture (defined by its sub-id)
        /// </summary>
        /// <param name="texPos"></param>
        /// <param name="rndIndex">0..29 for a specific random pixel, or -1 to randomize, which is the same as calling GetRandomColor without the rndIndex argument</param>
        /// <returns></returns>
        int GetRandomColor(TextureAtlasPosition texPos, int rndIndex);

        /// <summary>
        /// Get the random colors array for the specified TextureAtlasPosition, creating it if necessary
        /// </summary>
        /// <param name="texPos"></param>
        /// <returns></returns>
        int[] GetRandomColors(TextureAtlasPosition texPos);

        /// <summary>
        /// Returns you an average rgba value picked inside the texture subid
        /// </summary>
        /// <param name="textureSubId"></param>
        /// <returns></returns>
        int GetAverageColor(int textureSubId);

        /// <summary>
        /// Renders given texture into the texture atlas at given location
        /// </summary>
        /// <param name="intoAtlasTextureId"></param>
        /// <param name="fromTexture"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="sourceWidth"></param>
        /// <param name="sourceHeight"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <param name="alphaTest"></param>
        void RenderTextureIntoAtlas(int intoAtlasTextureId, LoadedTexture fromTexture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, float targetX, float targetY, float alphaTest = 0.005f);
        
    }
}
