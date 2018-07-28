using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
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

    public interface IEntityTextureAtlasAPI : ITextureAtlasAPI
    {
        void RenderTextureIntoAtlas(LoadedTexture fromTexture, float sourceX, float sourceY, float sourceWidth, float sourceHeight, float targetX, float targetY, float alphaTest = 0.005f);
    }

    public interface ITextureAtlasAPI
    {
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
        /// Deallocates a previously allocated texture space
        /// </summary>
        /// <param name="textureSubId"></param>
        void FreeTextureSpace(int textureSubId);
    }
}
