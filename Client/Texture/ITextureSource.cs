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
        TextureAtlasPosition? this[string textureCode] { get; }

        /// <summary>
        /// This returns the size of the atlas this texture resides in.
        /// </summary>
        Size2i? AtlasSize { get; }
    }


    /// <summary>
    /// Helper class for implementors of ITexPositionSource
    /// </summary>
    public class ContainedTextureSource : ITexPositionSource
    {
        public Size2i AtlasSize => targetAtlas.Size;

        public TextureAtlasPosition this[string textureCode] => getOrCreateTexPos(Textures[textureCode]);

        public readonly Dictionary<string, AssetLocation> Textures;

        protected readonly ITextureAtlasAPI targetAtlas;
        protected readonly ICoreClientAPI capi;
        protected readonly string sourceForErrorLogging;

        public ContainedTextureSource(ICoreClientAPI capi, ITextureAtlasAPI targetAtlas, Dictionary<string, AssetLocation> textures, string sourceForErrorLogging)
        {
            this.capi = capi;
            this.targetAtlas = targetAtlas;
            this.Textures = textures;
            this.sourceForErrorLogging = sourceForErrorLogging;
        }

        protected virtual TextureAtlasPosition getOrCreateTexPos(AssetLocation texturePath)
        {
            TextureAtlasPosition? position = targetAtlas[texturePath];

            if (position != null)
            {
                return position;
            }
            
            IAsset? texAsset = capi.Assets.TryGet(texturePath.Clone().WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png"));
            if (texAsset != null)
            {
                targetAtlas.GetOrInsertTexture(texturePath, out _, out position, () => texAsset.ToBitmap(capi));
                if (position == null)
                {
                    capi.World.Logger.Error($"{sourceForErrorLogging}, require texture '{texturePath}' which exists, but unable to upload it or allocate space.");
                    position = targetAtlas.UnknownTexturePosition;
                }
            }
            else
            {
                capi.World.Logger.Error($"{sourceForErrorLogging}, require texture '{texturePath}', but no such texture found.");
                position = targetAtlas.UnknownTexturePosition;
            }
            
            return position;
        }
    }
}
