using System.Collections.Generic;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public enum EnumShapeFormat
    {
        VintageStory,
        Obj,
        GltfEmbedded
    }

    public class CompositeShape
    {
        public AssetLocation Base;
        public EnumShapeFormat Format;
        /// <summary>
        /// Whether or not to insert baked in textures for mesh formats such as gltf into the texture atlas.
        /// </summary>
        public bool InsertBakedTextures;

        public float rotateX;
        public float rotateY;
        public float rotateZ;

        public float offsetX;
        public float offsetY;
        public float offsetZ;

        public float Scale = 1f;

        public Vec3f RotateXYZCopy => new Vec3f(rotateX, rotateY, rotateZ);
        public Vec3f OffsetXYZCopy => new Vec3f(offsetX, offsetY, offsetZ);

        /// <summary>
        /// The block shape may consists of any amount of alternatives, one of which will be randomly chosen when the block is placed in the world.
        /// </summary>
        public CompositeShape[] Alternates = null;

        /// <summary>
        /// Includes the base shape
        /// </summary>
        public CompositeShape[] BakedAlternates = null;

        public CompositeShape[] Overlays = null;

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


        public override int GetHashCode()
        {
            int hashcode = Base.GetHashCode() + ("@" + rotateX + "/" + rotateY + "/" + rotateZ + "o" + offsetX + "/" + offsetY + "/" + offsetZ).GetHashCode();
            if (Overlays != null)
            {
                for (int i = 0; i < Overlays.Length; i++)
                {
                    hashcode ^= Overlays[i].GetHashCode();
                }
            }

            return hashcode;
        }

        /// <summary>
        /// Creates a deep copy of the composite shape
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
                    alternatesClone[i] = Alternates[i].CloneWithoutAlternatesNorOverlays();
                }
            }

            CompositeShape ct = CloneWithoutAlternates();
            ct.Alternates = alternatesClone;

            return ct;
        }

        /// <summary>
        /// Creates a deep copy of the shape, but omitting its alternates (used to populate the alternates)
        /// </summary>
        /// <returns></returns>
        public CompositeShape CloneWithoutAlternates()
        {
            CompositeShape[] overlaysClone = null;

            if (this.Overlays != null)
            {
                overlaysClone = new CompositeShape[Overlays.Length];
                for (int i = 0; i < overlaysClone.Length; i++)
                {
                    overlaysClone[i] = Overlays[i].CloneWithoutAlternatesNorOverlays();
                }
            }

            CompositeShape ct = CloneWithoutAlternatesNorOverlays();
            ct.Overlays = overlaysClone;

            return ct;
        }

        internal CompositeShape CloneWithoutAlternatesNorOverlays()
        {
            CompositeShape ct = new CompositeShape()
            {
                Base = Base?.Clone(),
                Format = Format,
                InsertBakedTextures = InsertBakedTextures,
                rotateX = rotateX,
                rotateY = rotateY,
                rotateZ = rotateZ,
                offsetX = offsetX,
                offsetY = offsetY,
                offsetZ = offsetZ,
                Scale = Scale,
                VoxelizeTexture = VoxelizeTexture,
                QuantityElements = QuantityElements,
                SelectiveElements = (string[])SelectiveElements?.Clone()
            };
            return ct;
        }


        /// <summary>
        /// Expands the Composite Shape and populates the Baked field
        /// </summary>
        public void LoadAlternates(IAssetManager assetManager, ILogger logger)
        {
            List<CompositeShape> resolvedAlternates = new List<CompositeShape>();

            if (Base.Path.EndsWith("*"))
            {
                resolvedAlternates.AddRange(resolveShapeWildCards(this, assetManager, logger, true));
            }
            else resolvedAlternates.Add(this);
            if (Alternates != null)
            {
                foreach (var alt in this.Alternates)
                {
                    if (alt.Base == null) alt.Base = Base.Clone();
                    if (alt.Base.Path.EndsWith("*"))
                    {
                        resolvedAlternates.AddRange(resolveShapeWildCards(alt, assetManager, logger, false));
                    }
                    else resolvedAlternates.Add(alt);
                }
            }

            Base = resolvedAlternates[0].Base;

            if (resolvedAlternates.Count == 1)
            {
                return;
            }

            Alternates = new CompositeShape[resolvedAlternates.Count - 1];

            for (int i = 0; i < resolvedAlternates.Count - 1; i++)
            {
                Alternates[i] = resolvedAlternates[i + 1];
            }

            BakedAlternates = new CompositeShape[Alternates.Length + 1];
            BakedAlternates[0] = this.CloneWithoutAlternates();

            for (int i = 0; i < Alternates.Length; i++)
            {
                CompositeShape altCS = BakedAlternates[i + 1] = Alternates[i];

                if (altCS.Base == null)
                {
                    altCS.Base = Base.Clone();
                }

                if (altCS.QuantityElements == null)
                {
                    altCS.QuantityElements = QuantityElements;
                }

                if (altCS.SelectiveElements == null)
                {
                    altCS.SelectiveElements = SelectiveElements;
                }
            }
            
        }


        CompositeShape[] resolveShapeWildCards(CompositeShape shape, IAssetManager assetManager, ILogger logger, bool addCubeIfNone)
        {
            List<IAsset> assets = assetManager.GetManyInCategory("shapes", shape.Base.Path.Substring(0, Base.Path.Length - 1), shape.Base.Domain);

            if (assets.Count == 0)
            {
                if (addCubeIfNone)
                {
                    logger.Warning("Could not find any variants for wildcard shape {0}, will use standard cube shape.", shape.Base.Path);
                    return new CompositeShape[] { new CompositeShape() { Base = new AssetLocation("block/basic/cube") } };
                }
                else
                {
                    logger.Warning("Could not find any variants for wildcard shape {0}.", shape.Base.Path);
                    return new CompositeShape[] { };
                }
            }

            CompositeShape[] cshapes = new CompositeShape[assets.Count];
            int i = 0;
            foreach (var asset in assets)
            {
                AssetLocation newLocation = asset.Location.CopyWithPath(asset.Location.Path.Substring("shapes/".Length));
                newLocation.RemoveEnding();

                cshapes[i++] = new CompositeShape()
                {
                    Base = newLocation,
                    rotateX = shape.rotateX,
                    rotateY = shape.rotateY,
                    rotateZ = shape.rotateZ,
                    Scale = shape.Scale,
                    QuantityElements = shape.QuantityElements,
                    SelectiveElements = shape.SelectiveElements
                };
            }

            return cshapes;

        }
    }
}
