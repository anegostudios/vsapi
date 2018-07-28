using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class CompositeShape
    {
        public AssetLocation Base;

        public float rotateX;
        public float rotateY;
        public float rotateZ;

        /// <summary>
        /// The block shape may consists of any amount of alternatives, one of which will be randomly chosen when the block is placed in the world.
        /// </summary>
        public CompositeShape[] Alternates = null;

        /// <summary>
        /// If true, the shape is created from a voxelized version of the first defined texture
        /// </summary>
        public bool VoxelizeTexture = false;

        /// <summary>
        /// If non zero will only tesselate the first n elements of the shape
        /// </summary>
        public int? QuantityElements = null;

        /// <summary>
        /// If set will only tesselate elements with given name
        /// </summary>
        public string[] SelectiveElements;

        /// <summary>
        /// Creates a deep copy of the texture
        /// </summary>
        /// <returns></returns>
        public CompositeShape Clone()
        {
            CompositeShape[] alternatesClone = null;

            if (Alternates != null)
            {
                alternatesClone = new CompositeShape[Alternates.Length];
                for (int i = 0; i < alternatesClone.Length; i++)
                {
                    alternatesClone[i] = Alternates[i].CloneWithoutAlternates();
                }
            }

            CompositeShape ct = new CompositeShape()
            {
                Base = Base.Clone(),
                Alternates = alternatesClone,
                VoxelizeTexture = VoxelizeTexture,
                rotateX = rotateX,
                rotateY = rotateY,
                rotateZ = rotateZ,
                QuantityElements = QuantityElements,
                SelectiveElements = (string[])SelectiveElements?.Clone()
            };

            return ct;
        }

        internal CompositeShape CloneWithoutAlternates()
        {
            CompositeShape ct = new CompositeShape()
            {
                Base = Base.Clone(),
                rotateX = rotateX,
                rotateY = rotateY,
                rotateZ = rotateZ, 
                VoxelizeTexture = VoxelizeTexture,
                QuantityElements = QuantityElements,
                SelectiveElements = (string[])SelectiveElements?.Clone()
            };
            return ct;
        }

        internal void FillPlaceHolders(Dictionary<string, string> searchReplace)
        {
            foreach (var val in searchReplace)
            {
                if (Base?.Path == null) continue;

                Base.Path = Block.FillPlaceHolder(Base.Path, val.Key, val.Value);
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
        public void LoadAlternates(IAssetManager assetManager, ILogger logger)
        {
            if (Base.Path.EndsWith("*"))
            {
                List<IAsset> assets = assetManager.GetMany("shapes/" + Base.Path.Substring(0, Base.Path.Length - 1), Base.Domain);

                if (assets.Count == 0)  
                {
                    Base = new AssetLocation("block/basic/cube");
                    logger.Warning("Could not find any variants for shape {0}, will use standard cube shape.", Base.Path);
                }   

                if (assets.Count == 1)
                {
                    Base = assets[0].Location.CopyWithPath(assets[0].Location.Path.Substring("shapes/".Length));
                    Base.RemoveEnding();
                }

                if (assets.Count > 1)
                {
                    int origLength = (Alternates == null ? 0 : Alternates.Length);
                    CompositeShape[] alternates = new CompositeShape[origLength + assets.Count];
                    if (Alternates != null)
                    {
                        Array.Copy(Alternates, alternates, Alternates.Length);
                    }

                    int i = 0;
                    foreach (IAsset asset in assets)
                    {
                        AssetLocation newLocation = asset.Location.CopyWithPath(asset.Location.Path.Substring("shapes/".Length));
                        newLocation.RemoveEnding();

                        if (i == 0)
                        {
                            Base = newLocation.Clone();
                        }
                        
                        alternates[origLength + i] = new CompositeShape() { Base = newLocation };

                        i++;
                    }

                    Alternates = alternates;
                }
            }

            if (Alternates != null)
            {
                for (int i = 0; i < Alternates.Length; i++)
                {
                    if (Alternates[i].Base == null)
                    {
                        Alternates[i].Base = Base.Clone();
                    }

                    if (Alternates[i].QuantityElements == null)
                    {
                        Alternates[i].QuantityElements = QuantityElements;
                    }

                    if (Alternates[i].SelectiveElements == null)
                    {
                        Alternates[i].SelectiveElements = SelectiveElements;
                    }
                }
            }
        }
    }
}
