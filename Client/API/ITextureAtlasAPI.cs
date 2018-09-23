using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
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
        /// <returns></returns>
        TextureAtlasPosition GetPosition(Item item, string textureName = null);
        
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
        /// <returns></returns>
        TextureAtlasPosition GetPosition(Block block, string textureName);
        
    }

	/// <summary>
    /// Entity texture Atlas.
    /// </summary>
    
    /// <summary>
    /// Texture atlas base.
    /// </summary>
    public interface ITextureAtlasAPI
    {
        /// <summary>
        /// The texture atlas position of the "unknown.png" texture
        /// </summary>
        TextureAtlasPosition UnknownTexturePosition { get; }

        /// <summary>
        /// Size of one block texture atlas
        /// </summary>
        int Size { get; }

        /// <summary>
        /// As configured in the clientsettings.json divided by the texture atlas size
        /// </summary>
        float SubPixelPadding { get; }

        /// <summary>
        /// Returns the default texture atlas position for all blocks, referenced  by the texturesubid
        /// </summary>
        TextureAtlasPosition[] Positions { get; }

        /// <summary>
        /// Returns the list of currently loaded texture atlas ids
        /// </summary>
        List<int> AtlasTextureIds { get; }

        /// <summary>
        /// Reserves a spot on the texture atlas. Returns true if allocation was successful.
        /// Can be used to render onto it through the Render API
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <returns></returns>
        bool AllocateTextureSpace(int width, int height, out int textureSubId, out TextureAtlasPosition texPos);

        /// <summary>
        /// Inserts a texture into the texture atlas after the atlas has been generated. Updates the in-ram texture atlas as well as the in-gpu-ram texture atlas. 
        /// The textureSubId can be used to find the TextureAtlasPosition again in case you loose it ;-)
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="textureSubId"></param>
        /// <param name="texPos"></param>
        /// <returns></returns>
        bool InsertTexture(BitmapRef bmp, out int textureSubId, out TextureAtlasPosition texPos, float alphaTest = 0.005f);

        /// <summary>
        /// Deallocates a previously allocated texture space
        /// </summary>
        /// <param name="textureSubId"></param>
        void FreeTextureSpace(int textureSubId);
        
        /// <summary>
        /// Returns an rgba value picked randomly inside the given texture (defined by its sub-id) of given item
        /// </summary>
        /// <param name="textureSubId"></param>
        /// <returns></returns>
        int GetRandomPixel(int textureSubId);

        /// <summary>
        /// Returns you an rgba value picked inside the texture subid of given block at given relative position inside its texture (0..1)
        /// </summary>
        /// <param name="textureSubId"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        int GetPixelAt(int textureSubId, float px, float py);

        /// <summary>
        /// Renders given texture into the texture atlas at given location
        /// </summary>
        /// <param name="fromTexture"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="sourceWidth"></param>
        /// <param name="sourceHeight"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <param name="alphaTest"></param>
        void RenderTextureIntoAtlas(LoadedTexture fromTexture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, float targetX, float targetY, float alphaTest = 0.005f);
    }
}
