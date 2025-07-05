using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public delegate ITexPositionSource TextureSourceBuilder(Shape shape, string name);
    public delegate string TexturePathUpdater(string path);

    public class ShapeTextureSource : ITexPositionSource
    {
        ICoreClientAPI capi;
        Shape shape;
        string filenameForLogging;
        public Dictionary<string, CompositeTexture> textures = new Dictionary<string, CompositeTexture>();
        public TextureAtlasPosition? firstTexPos;

        HashSet<AssetLocation> missingTextures = new HashSet<AssetLocation>();

        public ShapeTextureSource(ICoreClientAPI capi, Shape shape, string filenameForLogging)
        {
            this.capi = capi;
            this.shape = shape;
            this.filenameForLogging = filenameForLogging;
        }

        public ShapeTextureSource(ICoreClientAPI capi, Shape shape, string filenameForLogging, IDictionary<string, CompositeTexture> texturesSource, TexturePathUpdater pathUpdater) : this(capi, shape, filenameForLogging)
        {
            foreach (var val in texturesSource)
            {
                var ctex = val.Value.Clone();
                ctex.Base.Path = pathUpdater(ctex.Base.Path);
                ctex.Bake(capi.Assets);
                textures[val.Key] = ctex;
            }
        }

        public TextureAtlasPosition this[string textureCode]
        {
            get
            {
                TextureAtlasPosition texPos;

                if (textures.TryGetValue(textureCode, out var ctex))
                {
                    capi.BlockTextureAtlas.GetOrInsertTexture(ctex, out _, out texPos);
                }
                else
                {
                    shape.Textures.TryGetValue(textureCode, out var texturePath);

                    if (texturePath == null)
                    {
                        if (!missingTextures.Contains(texturePath))
                        {
                            capi.Logger.Warning("Shape {0} has an element using texture code {1}, but no such texture exists", filenameForLogging, textureCode);
                            missingTextures.Add(texturePath);
                        }

                        return capi.BlockTextureAtlas.UnknownTexturePosition;
                    }

                    capi.BlockTextureAtlas.GetOrInsertTexture(texturePath, out _, out texPos);
                }


                if (texPos == null)
                {
                    return capi.BlockTextureAtlas.UnknownTexturePosition;
                }

                if (this.firstTexPos == null)
                {
                    this.firstTexPos = texPos;
                }

                return texPos;
            }
        }

        public Size2i AtlasSize => capi.BlockTextureAtlas.Size;
    }
}
