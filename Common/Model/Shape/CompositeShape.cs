using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Types of shape that can be loaded by the game.
    /// </summary>
    [DocumentAsJson]
    public enum EnumShapeFormat
    {
        /// <summary>
        /// (Recommended) Imports a shape using the default JSON system.
        /// </summary>
        [DocumentAsJson] VintageStory,

        /// <summary>
        /// Imports a shape using an Obj file.
        /// </summary>
        [DocumentAsJson] Obj,

        /// <summary>
        /// Imports a shape using a Gltf file.
        /// </summary>
        [DocumentAsJson] GltfEmbedded
    }

    /// <summary>
    /// Holds shape data to create 3D representations of objects. Also allows shapes to be overlayed on top of one another recursively.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"shape": { "base": "block/basic/cube" },
    /// </code>
    /// <code language="json">
    ///"shapeInventory": {
	/// 	"base": "block/plant/bamboo/{color}/{part}-1",
	/// 	"overlays": [ { "base": "block/plant/bamboo/{color}/{part}lod0-1" } ]
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class CompositeShape
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>None</jsondefault>-->
        /// The path to this shape file.
        /// </summary>
        [DocumentAsJson] public AssetLocation Base;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>VintageStory</jsondefault>-->
        /// The format/filetype of this shape.
        /// </summary>
        [DocumentAsJson] public EnumShapeFormat Format;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// Whether or not to insert baked in textures for mesh formats such as gltf into the texture atlas.
        /// </summary>
        [DocumentAsJson] public bool InsertBakedTextures;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much, in degrees, should this shape be rotated around the X axis?
        /// </summary>
        [DocumentAsJson] public float rotateX;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much, in degrees, should this shape be rotated around the Y axis?
        /// </summary>
        [DocumentAsJson] public float rotateY;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much, in degrees, should this shape be rotated around the Z axis?
        /// </summary>
        [DocumentAsJson] public float rotateZ;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much should this shape be offset on X axis?
        /// </summary>
        [DocumentAsJson] public float offsetX;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much should this shape be offset on Y axis?
        /// </summary>
        [DocumentAsJson] public float offsetY;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much should this shape be offset on Z axis?
        /// </summary>
        [DocumentAsJson] public float offsetZ;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// The scale of this shape on all axes.
        /// </summary>
        [DocumentAsJson] public float Scale = 1f;

        public Vec3f RotateXYZCopy => new Vec3f(rotateX, rotateY, rotateZ);
        public Vec3f OffsetXYZCopy => new Vec3f(offsetX, offsetY, offsetZ);

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The block shape may consists of any amount of alternatives, one of which will be randomly chosen when the shape is chosen.
        /// </summary>
        [DocumentAsJson] public CompositeShape[] Alternates = null;

        /// <summary>
        /// Includes the base shape
        /// </summary>
        public CompositeShape[] BakedAlternates = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The shape will render all overlays on top of this shape. Can be used to group multiple shapes into one composite shape.
        /// </summary>
        [DocumentAsJson] public CompositeShape[] Overlays = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// If true, the shape is created from a voxelized version of the first defined texture
        /// </summary>
        [DocumentAsJson] public bool VoxelizeTexture = false;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// If non zero will only tesselate the first n elements of the shape
        /// </summary>
        [DocumentAsJson] public int? QuantityElements = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// If set will only tesselate elements with given name
        /// </summary>
        [DocumentAsJson] public string[] SelectiveElements;

        /// <summary>
        /// If set will not tesselate elements with given name
        /// </summary>
        public string[] IgnoreElements;


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

        public override string ToString()
        {
            return Base.ToString();
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
                SelectiveElements = (string[])SelectiveElements?.Clone(),
                IgnoreElements = (string[])IgnoreElements?.Clone(),
            };
            return ct;
        }

        /// <summary>
        /// Alias to LoadAlternates
        /// </summary>
        /// <param name="assetManager"></param>
        /// <param name="logger"></param>
        public void Bake(IAssetManager assetManager, ILogger logger)
        {
            LoadAlternates(assetManager, logger);
        }

        /// <summary>
        /// Expands the Composite Shape and populates the Baked field
        /// </summary>
        public void LoadAlternates(IAssetManager assetManager, ILogger logger)
        {
            List<CompositeShape> resolvedAlternates = new List<CompositeShape>();

            if (Base.Path.EndsWith('*'))
            {
                resolvedAlternates.AddRange(resolveShapeWildCards(this, assetManager, logger, true));
            }
            else resolvedAlternates.Add(this);
            if (Alternates != null)
            {
                foreach (var alt in this.Alternates)
                {
                    if (alt.Base == null) alt.Base = Base.Clone();
                    if (alt.Base.Path.EndsWith('*'))
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

                if (altCS.IgnoreElements == null)
                {
                    altCS.IgnoreElements = IgnoreElements;
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
                    return Array.Empty<CompositeShape>();
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
                    SelectiveElements = shape.SelectiveElements,
                    IgnoreElements = shape.IgnoreElements
                };
            }

            return cshapes;

        }
    }
}
