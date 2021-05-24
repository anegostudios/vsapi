using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A single block texture
    /// </summary>
    public class CompositeTexture
    {
        /// <summary>
        /// The basic texture for this composite texture
        /// </summary>
        public AssetLocation Base;

        /// <summary>
        /// The base texture may be overlayed with any quantity of textures. These are baked together during texture atlas creation
        /// </summary>
        public AssetLocation[] Overlays = null;

        /// <summary>
        /// The texture may consists of any amount of alternatives, one of which will be randomly chosen when the block is placed in the world.
        /// </summary>
        public CompositeTexture[] Alternates = null;

        /// <summary>
        /// BakedCompositeTexture is an expanded, atlas friendly version of CompositeTexture. Required during texture atlas generation.
        /// </summary>
        public BakedCompositeTexture Baked;

        /// <summary>
        /// Rotation of the texture may only be a multiple of 90
        /// </summary>
        public int Rotation = 0;

        /// <summary>
        /// Creates a new empty composite texture
        /// </summary>
        public CompositeTexture()
        {
        }

        /// <summary>
        /// Creates a new empty composite texture with given base texture
        /// </summary>
        /// <param name="Base"></param>
        public CompositeTexture(AssetLocation Base)
        {
            this.Base = Base;
        }



        /// <summary>
        /// Creates a deep copy of the texture
        /// </summary>
        /// <returns></returns>
        public CompositeTexture Clone()
        {
            CompositeTexture[] alternatesClone = null;

            if (Alternates != null)
            {   
                alternatesClone = new CompositeTexture[Alternates.Length];
                for (int i = 0; i < alternatesClone.Length; i++)
                {
                    alternatesClone[i] = Alternates[i].CloneWithoutAlternates();
                }
            }

            CompositeTexture ct = new CompositeTexture()
            {
                Base = Base.Clone(),
                Alternates = alternatesClone,
                Rotation = Rotation
            };

            if (Overlays != null)
            {
                ct.Overlays = new AssetLocation[Overlays.Length];
                for (int i = 0; i < ct.Overlays.Length; i++)
                {
                    ct.Overlays[i] = Overlays[i].Clone();
                }
            }

            return ct;
        }

        internal CompositeTexture CloneWithoutAlternates()
        {
            CompositeTexture ct = new CompositeTexture()
            {
                Base = Base.Clone(),
                Rotation = Rotation
            };

            if (Overlays != null)
            {
                ct.Overlays = new AssetLocation[Overlays.Length];
                for (int i = 0; i < ct.Overlays.Length; i++)
                {
                    ct.Overlays[i] = Overlays[i].Clone();
                }
            }

            return ct;
        }


        /// <summary>
        /// Expands the Composite Texture to a texture atlas friendly version and populates the Baked field. This method is called by the texture atlas managers.
        /// Won't have any effect if called after the texture atlasses have been created.
        /// </summary>
        public void Bake(IAssetManager assetManager)
        {
            // Some CompositeTextures are re-used multiple times in the same Block (e.g. if a blocktype specified a texture for key "all") so no need to re-bake
            if (Baked == null) Baked = Bake(assetManager, this);
        }

        /// <summary>
        /// Expands the Composite Texture to a texture atlas friendly version and populates the Baked field. This method can be called after the game world has loaded.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="intoAtlas">The atlas to insert the baked texture.</param>
        public void RuntimeBake(ICoreClientAPI capi, ITextureAtlasAPI intoAtlas)
        {
            Baked = Bake(capi.Assets, this);

            RuntimeInsert(capi, intoAtlas, Baked);

            if (Baked.BakedVariants != null)
            {
                foreach (var val in Baked.BakedVariants)
                {
                    RuntimeInsert(capi, intoAtlas, val);
                }
            }
        }


        bool RuntimeInsert(ICoreClientAPI capi, ITextureAtlasAPI intoAtlas, BakedCompositeTexture btex)
        {
            BitmapRef bmp = capi.Assets.Get(btex.BakedName).ToBitmap(capi);

            int textureSubId;
            TextureAtlasPosition texpos;
            if (intoAtlas.InsertTexture(bmp, out textureSubId, out texpos))
            {
                btex.TextureSubId = textureSubId;
                capi.Render.RemoveTexture(btex.BakedName);
                return true;
            }

            bmp.Dispose();
            return false;
        }

        /// <summary>
        /// Expands a CompositeTexture to a texture atlas friendly version and populates the Baked field
        /// </summary>
        /// <param name="assetManager"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        static BakedCompositeTexture Bake(IAssetManager assetManager, CompositeTexture ct)
        {
            BakedCompositeTexture bct = new BakedCompositeTexture();

            bct.BakedName = ct.Base.Clone();
             
            if (ct.Base.HasAlternates)
            {
                List<IAsset> assets = assetManager.GetMany("textures/" + bct.BakedName.Path.Substring(0, bct.BakedName.Path.Length - 1), bct.BakedName.Domain);
                if (assets.Count == 0)
                {
                    ct.Base = bct.BakedName = new AssetLocation("unknown");
                }
                else
                if (assets.Count == 1)
                {
                    ct.Base = assets[0].Location.CloneWithoutPrefixAndEnding("textures/".Length);
                    bct.BakedName = ct.Base.Clone();
                }
                else
                if (assets.Count > 1)
                {
                    int origLength = (ct.Alternates == null ? 0 : ct.Alternates.Length);
                    CompositeTexture[] alternates = new CompositeTexture[origLength + assets.Count - 1];
                    if (ct.Alternates != null)
                    {
                        Array.Copy(ct.Alternates, alternates, ct.Alternates.Length);
                    }

                    int i = 0;
                    foreach (IAsset asset in assets)
                    {
                        AssetLocation newLocation = asset.Location.CloneWithoutPrefixAndEnding("textures/".Length);

                        if (i == 0)
                        {
                            ct.Base = newLocation;
                            bct.BakedName = newLocation.Clone();
                        }
                        else
                        {
                            alternates[origLength + i - 1] = new CompositeTexture(newLocation);
                        }

                        i++;
                    }
                    
                    ct.Alternates = alternates;
                }
            }


            if (ct.Overlays != null)
            {
                bct.TextureFilenames = new AssetLocation[ct.Overlays.Length + 1];
                bct.TextureFilenames[0] = ct.Base.Clone();

                for (int i = 0; i < ct.Overlays.Length; i++)
                {
                    bct.TextureFilenames[i + 1] = ct.Overlays[i].Clone();
                    bct.BakedName.Path += "++" + ct.Overlays[i].ToShortString();
                }
            }
            else
            {
                bct.TextureFilenames = new AssetLocation[] { ct.Base.Clone() };
            }

            if (ct.Rotation != 0)
            {
                if (ct.Rotation != 90 && ct.Rotation != 180 && ct.Rotation != 270)
                {
                    throw new Exception("Texture definition " + ct.Base + " has a rotation thats not 0, 90, 180 or 270. These are the only allowed values!");
                }
                bct.BakedName.Path += "@" + ct.Rotation;
            }

            if (ct.Alternates != null)
            {
                bct.BakedVariants = new BakedCompositeTexture[ct.Alternates.Length + 1];
                bct.BakedVariants[0] = bct;
                for (int i = 0; i < ct.Alternates.Length; i++)
                {
                    bct.BakedVariants[i + 1] = Bake(assetManager, ct.Alternates[i]);
                }
            }

            return bct;
        }


        public override string ToString()
        {
            return Base.ToString() + "@" + Rotation;
        }
    }



    /// <summary>
    /// An expanded, atlas-friendly version of a CompositeTexture
    /// </summary>
    public class BakedCompositeTexture
    {
        /// <summary>
        /// Unique identifier for this texture
        /// </summary>
        public int TextureSubId;

        /// <summary>
        /// The Base name and Overlay concatenated (if there was any defined)
        /// </summary>
        public AssetLocation BakedName;

        /// <summary>
        /// The base name and overlays as array
        /// </summary>
        public AssetLocation[] TextureFilenames;

        /// <summary>
        /// If non-null also contains BakedName
        /// </summary>
        public BakedCompositeTexture[] BakedVariants = null;
    }
}
