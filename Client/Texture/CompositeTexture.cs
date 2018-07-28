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
                Alternates = alternatesClone
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

        internal void FillPlaceHolders(Dictionary<string, string> searchReplace)
        {
            foreach (var val in searchReplace)
            {
                Base.Path = Block.FillPlaceHolder(Base.Path, val.Key, val.Value);

                if (Overlays != null)
                {
                    for (int i = 0; i < Overlays.Length; i++)
                    {
                        Overlays[i].Path = Block.FillPlaceHolder(Overlays[i].Path, val.Key, val.Value);
                    }
                }
            }

            if (Alternates != null)
            {
                for (int i = 0; i < Alternates.Length; i++)
                {
                    Alternates[i].FillPlaceHolders(searchReplace);
                }
            }
        }


        /// <summary>
        /// Expands the Composite Texture to a texture atlas friendly version and populates the Baked field
        /// </summary>
        public void Bake(IAssetManager assetManager)
        {
            Baked = Bake(assetManager, this);
        }


        /// <summary>
        /// Expands a CompositeTexture to a texture atlas friendly version and populates the Baked field
        /// </summary>
        /// <param name="assetManager"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static BakedCompositeTexture Bake(IAssetManager assetManager, CompositeTexture ct)
        {
            BakedCompositeTexture bakedCt = new BakedCompositeTexture();

            bakedCt.BakedName = ct.Base.Clone();
             
            if (ct.Base.Path.EndsWith("*"))
            {
                List<IAsset> assets = assetManager.GetMany("textures/" + bakedCt.BakedName.Path.Substring(0, bakedCt.BakedName.Path.Length - 1), bakedCt.BakedName.Domain);
                if (assets.Count == 0)
                {
                    ct.Base = bakedCt.BakedName = new AssetLocation("unknown");
                }

                if (assets.Count == 1)
                {
                    ct.Base = assets[0].Location.Clone();
                    ct.Base.Path = assets[0].Location.Path.Substring("textures/".Length);
                    ct.Base.RemoveEnding();
                    bakedCt.BakedName = ct.Base.Clone();
                }

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
                        AssetLocation newLocation = assets[0].Location.Clone();
                        newLocation.Path = asset.Location.Path.Substring("textures/".Length);
                        newLocation.RemoveEnding();

                        if (i == 0)
                        {
                            ct.Base = newLocation;
                            bakedCt.BakedName = ct.Base;
                        } else
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
                bakedCt.TextureFilenames = new AssetLocation[ct.Overlays.Length + 1];
                bakedCt.TextureFilenames[0] = ct.Base.Clone();

                for (int i = 0; i < ct.Overlays.Length; i++)
                {
                    bakedCt.TextureFilenames[i + 1] = ct.Overlays[i].Clone();

                    bakedCt.BakedName.Path += "++" + ct.Overlays[i].ToString();
                }
            }
            else
            {
                bakedCt.TextureFilenames = new AssetLocation[] { ct.Base.Clone() };
            }

            if (ct.Alternates != null)
            {
                bakedCt.BakedVariants = new BakedCompositeTexture[ct.Alternates.Length + 1];
                bakedCt.BakedVariants[0] = bakedCt;
                for (int i = 0; i < ct.Alternates.Length; i++)
                {
                    bakedCt.BakedVariants[i + 1] = Bake(assetManager, ct.Alternates[i]);
                }
            }

            return bakedCt;
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
