using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// An interface that can supply texture atlas positions 
    /// </summary>
    public interface ITexPositionSource
    {
        TextureAtlasPosition this[string textureCode] { get; }

        /// <summary>
        /// This returns the size of the atlas this texture resides in.
        /// </summary>
        Size2i AtlasSize { get; }
    }


    /// <summary>
    /// Helper class for implementors of ITexPositionSource
    /// </summary>
    public class ContainedTextureSource : ITexPositionSource
    {
        ITextureAtlasAPI targetAtlas;
        ICoreClientAPI capi;
        public Size2i AtlasSize => targetAtlas.Size;

        public Dictionary<string, AssetLocation> Textures = new Dictionary<string, AssetLocation>();
        private string sourceForErrorLogging;

        public ContainedTextureSource(ICoreClientAPI capi, ITextureAtlasAPI targetAtlas, Dictionary<string, AssetLocation> textures, string sourceForErrorLogging)
        {
            this.capi = capi;
            this.targetAtlas = targetAtlas;
            this.Textures = textures;
            this.sourceForErrorLogging = sourceForErrorLogging;
        }

        public TextureAtlasPosition this[string textureCode]
        {
            get
            {
                return getOrCreateTexPos(Textures[textureCode]);
            }
        }

        protected TextureAtlasPosition getOrCreateTexPos(AssetLocation texturePath)
        {
            TextureAtlasPosition texpos = targetAtlas[texturePath];

            if (texpos == null)
            {
                IAsset texAsset = capi.Assets.TryGet(texturePath.Clone().WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png"));
                if (texAsset != null)
                {
                    targetAtlas.GetOrInsertTexture(texturePath, out _, out texpos, () => texAsset.ToBitmap(capi));
                }
                else
                {
                    capi.World.Logger.Warning("{0}, require texture {1}, but no such texture found.", sourceForErrorLogging, texturePath);
                }
            }

            return texpos;
        }
    }
}
