using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Defines a texture to be overlayed on another texture.
    /// </summary>
    [DocumentAsJson]
    public class BlendedOverlayTexture
    {
        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The path to the texture to use as an overlay.
        /// </summary>
        [DocumentAsJson] public AssetLocation Base;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>Normal</jsondefault>-->
        /// The type of blend for each pixel.
        /// </summary>
        [DocumentAsJson] public EnumColorBlendMode BlendMode;

        public BlendedOverlayTexture Clone()
        {
            return new BlendedOverlayTexture() { Base = this.Base.Clone(), BlendMode = BlendMode };
        }

        public override string ToString()
        {
            return Base.ToString() + "-b" + BlendMode;
        }

        public void ToString(StringBuilder sb)
        {
            sb.Append(Base.ToString());
            sb.Append("-b");
            sb.Append(BlendMode);
        }
    }

    /// <summary>
    /// Holds data about a texture. Also allows textures to be overlayed on top of one another.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"textures": {
	///	"charcoal": { "base": "block/coal/charcoal" },
	///	"coke": { "base": "block/coal/coke" },
	///	"ore-anthracite": { "base": "block/coal/anthracite" },
	///	"ore-lignite": { "base": "block/coal/lignite" },
	///	"ore-bituminouscoal": { "base": "block/coal/bituminous" },
	///	"ember": { "base": "block/coal/ember" }
	///},
    /// </code>
    /// <code language="json">
    ///"textures": {
	///	"ore": {
	///		"base": "block/stone/rock/{rock}1",
	///		"overlays": [ "block/stone/ore/{ore}1" ]
	///	}
	///},
    /// </code>
    /// Connected textures example (See https://discord.com/channels/302152934249070593/479736466453561345/1134187385501007962)
    /// <code language="json">
    ///"textures": {
	///	"all": {
	///		"base": "block/stone/cobblestone/tiling/1",
	///		"tiles": [
	///			{ "base": "block/stone/cobblestone/tiling/*" }
	///		],
	///		"tilesWidth": 4
	///	}
	///}
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class CompositeTexture
    {
        public const char AlphaSeparator = 'å';   // This is the ASCII multiplication character (ASCII code 215), very unlikely to be used in a filename!
        public const string AlphaSeparatorRegexSearch = @"å\d+";
        public const string OverlaysSeparator = "++";
        public const char BlendmodeSeparator = '~';

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The basic texture for this composite texture
        /// </summary>
        [DocumentAsJson] public AssetLocation Base;

        /// <summary>
        /// <!--<jsonoptional>Obsolete</jsonoptional>-->
        /// Obsolete. Use <see cref="BlendedOverlays"/> instead.
        /// </summary>
        [DocumentAsJson] public AssetLocation[] Overlays
        {
            set
            {
                BlendedOverlays = value.Select(o => new BlendedOverlayTexture() { Base = o }).ToArray();
            }
        }

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// A set of textures to overlay above this texture. The base texture may be overlayed with any quantity of textures. These are baked together during texture atlas creation.
        /// </summary>
        [DocumentAsJson] public BlendedOverlayTexture[] BlendedOverlays = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// The texture may consists of any amount of alternatives, one of which will be randomly chosen when the block is placed in the world.
        /// </summary>
        [DocumentAsJson] public CompositeTexture[] Alternates = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// A way of basic support for connected textures. Textures should be named numerically from 1 to <see cref="TilesWidth"/> squared.
        /// <br/>E.g., if <see cref="TilesWidth"/> is 3, the order follows the pattern of:<br/>
        /// 1 2 3 <br/>
        /// 4 5 6 <br/>
        /// 7 8 9
        /// </summary>
        [DocumentAsJson] public CompositeTexture[] Tiles = null;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The number of tiles in one direction that make up the full connected textures defined in <see cref="Tiles"/>.
        /// </summary>
        [DocumentAsJson] public int TilesWidth;

        /// <summary>
        /// BakedCompositeTexture is an expanded, atlas friendly version of CompositeTexture. Required during texture atlas generation.
        /// </summary>
        public BakedCompositeTexture Baked;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// Rotation of the texture may only be a multiple of 90
        /// </summary>
        [DocumentAsJson] public int Rotation = 0;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>255</jsondefault>-->
        /// Can be used to modify the opacity of the texture. 255 is fully opaque, 0 is fully transparent.
        /// </summary>
        [DocumentAsJson] public int Alpha = 255;

        [ThreadStatic]    // Lovely ThreadStatic will automatically dispose of any dictionary created on a separate thread (if the thread is disposed of)
        public static Dictionary<AssetLocation, CompositeTexture> basicTexturesCache;
        [ThreadStatic]
        public static Dictionary<AssetLocation, List<IAsset>> wildcardsCache;

        public AssetLocation WildCardNoFiles;

        public AssetLocation AnyWildCardNoFiles
        {
            get
            {
                if (WildCardNoFiles != null) return WildCardNoFiles;
                if (Alternates != null)
                {
                    var f = Alternates.Select(ct => ct.WildCardNoFiles).FirstOrDefault();
                    if (f != null) return f;
                }

                return null;
            }
        }

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

            CompositeTexture[] tilesClone = null;
            if (Tiles != null)
            {
                tilesClone = new CompositeTexture[Tiles.Length];
                for (int i = 0; i < tilesClone.Length; i++)
                {
                    tilesClone[i] = tilesClone[i].CloneWithoutAlternates();
                }
            }

            CompositeTexture ct = new CompositeTexture()
            {
                Base = Base.Clone(),
                Alternates = alternatesClone,
                Tiles = tilesClone,
                Rotation = Rotation,
                Alpha = Alpha,
                TilesWidth = TilesWidth
            };

            if (BlendedOverlays != null)
            {
                ct.BlendedOverlays = new BlendedOverlayTexture[BlendedOverlays.Length];
                for (int i = 0; i < ct.BlendedOverlays.Length; i++)
                {
                    ct.BlendedOverlays[i] = BlendedOverlays[i].Clone();
                }
            }

            return ct;
        }

        internal CompositeTexture CloneWithoutAlternates()
        {
            CompositeTexture ct = new CompositeTexture()
            {
                Base = Base.Clone(),
                Rotation = Rotation,
                Alpha = Alpha,
                TilesWidth = TilesWidth
            };

            if (BlendedOverlays != null)
            {
                ct.BlendedOverlays = new BlendedOverlayTexture[BlendedOverlays.Length];
                for (int i = 0; i < ct.BlendedOverlays.Length; i++)
                {
                    ct.BlendedOverlays[i] = BlendedOverlays[i].Clone();
                }
            }

            if (Tiles != null)
            {
                ct.Tiles = new CompositeTexture[Tiles.Length];
                for (int i = 0; i < ct.Tiles.Length; i++)
                {
                    ct.Tiles[i] = ct.Tiles[i].CloneWithoutAlternates();
                }
            }

            return ct;
        }

        /// <summary>
        /// Tests whether this is a basic CompositeTexture with an asset location only, no rotation, alpha, alternates or overlays
        /// </summary>
        /// <returns></returns>
        public bool IsBasic()
        {
            if (Rotation != 0 || Alpha != 255) return false;
            return Alternates == null && BlendedOverlays == null && Tiles == null;
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

            if (intoAtlas.InsertTexture(bmp, out int textureSubId, out TextureAtlasPosition texpos))
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
        public static BakedCompositeTexture Bake(IAssetManager assetManager, CompositeTexture ct)
        {
            BakedCompositeTexture bct = new BakedCompositeTexture();

            ct.WildCardNoFiles = null;
            
            if (ct.Base.EndsWithWildCard)
            {
                if (wildcardsCache == null) wildcardsCache = new Dictionary<AssetLocation, List<IAsset>>();
                if (!wildcardsCache.TryGetValue(ct.Base, out List<IAsset> assets)) // Saves baking the same basic texture over and over
                {
                    assets = wildcardsCache[ct.Base] = assetManager.GetManyInCategory("textures", ct.Base.Path.Substring(0, ct.Base.Path.Length - 1), ct.Base.Domain);
                }
                if (assets.Count == 0)
                {
                    ct.WildCardNoFiles = ct.Base;
                    ct.Base = new AssetLocation("unknown");
                }
                else
                if (assets.Count == 1)
                {
                    ct.Base = assets[0].Location.CloneWithoutPrefixAndEnding("textures/".Length);
                }
                else
                {
                    int origLength = (ct.Alternates == null ? 0 : ct.Alternates.Length);
                    CompositeTexture[] alternates = new CompositeTexture[origLength + assets.Count - 1];
                    if (ct.Alternates != null)
                    {
                        Array.Copy(ct.Alternates, alternates, ct.Alternates.Length);
                    }

                    if (basicTexturesCache == null) basicTexturesCache = new Dictionary<AssetLocation, CompositeTexture>(); // Initialiser needed because it's ThreadStatic
                    for (int i = 0; i < assets.Count; i++)
                    {
                        IAsset asset = assets[i];
                        AssetLocation newLocation = asset.Location.CloneWithoutPrefixAndEnding("textures/".Length);

                        if (i == 0)
                        {
                            ct.Base = newLocation;
                        }
                        else
                        {
                            CompositeTexture act;
                            if (ct.Rotation == 0 && ct.Alpha == 255)
                            {
                                if (!basicTexturesCache.TryGetValue(newLocation, out act)) // Saves baking the same basic texture over and over
                                {
                                    act = basicTexturesCache[newLocation] = new CompositeTexture(newLocation);
                                }
                            }
                            else {
                                act = new CompositeTexture(newLocation);
                                act.Rotation = ct.Rotation;
                                act.Alpha = ct.Alpha;
                            }
                            alternates[origLength + i - 1] = act;
                        }
                    }

                    ct.Alternates = alternates;
                }
            }

            bct.BakedName = ct.Base.Clone();


            if (ct.BlendedOverlays != null)
            {
                bct.TextureFilenames = new AssetLocation[ct.BlendedOverlays.Length + 1];
                bct.TextureFilenames[0] = ct.Base;

                for (int i = 0; i < ct.BlendedOverlays.Length; i++)
                {
                    BlendedOverlayTexture bov = ct.BlendedOverlays[i];
                    bct.TextureFilenames[i + 1] = bov.Base;
                    bct.BakedName.Path += OverlaysSeparator + ((int)bov.BlendMode).ToString() + BlendmodeSeparator + bov.Base.ToString();
                }
            }
            else
            {
                bct.TextureFilenames = new AssetLocation[] { ct.Base };
            }

            if (ct.Rotation != 0)
            {
                if (ct.Rotation != 90 && ct.Rotation != 180 && ct.Rotation != 270)
                {
                    throw new Exception("Texture definition " + ct.Base + " has a rotation thats not 0, 90, 180 or 270. These are the only allowed values!");
                }
                bct.BakedName.Path += "@" + ct.Rotation;
            }

            if (ct.Alpha != 255)
            {
                if (ct.Alpha < 0 || ct.Alpha > 255)
                {
                    throw new Exception("Texture definition " + ct.Base + " has a alpha value outside the 0..255 range.");
                }

                bct.BakedName.Path += "" + AlphaSeparator + ct.Alpha;
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

            if (ct.Tiles != null)
            {
                List<BakedCompositeTexture> tiles = new List<BakedCompositeTexture>();

                for (int i = 0; i < ct.Tiles.Length; i++)
                {
                    var tile = ct.Tiles[i];
                    if (tile.Base.EndsWithWildCard)
                    {
                        if (wildcardsCache == null) wildcardsCache = new Dictionary<AssetLocation, List<IAsset>>();

                        // Fix borked windows sorting (i.e. 1, 10, 11, 12, ....)
                        var basePath = ct.Base.Path.Substring(0, ct.Base.Path.Length - 1);
                        var assets = wildcardsCache[ct.Base] = assetManager.GetManyInCategory("textures", basePath, ct.Base.Domain);
                        var len = "textures".Length + basePath.Length + "/".Length;
                        var sortedassets = assets.OrderBy(asset => asset.Location.Path.Substring(len).RemoveFileEnding().ToInt()).ToList();

                        for (int j = 0; j < sortedassets.Count; j++)
                        {
                            IAsset asset = sortedassets[j];
                            AssetLocation newLocation = asset.Location.CloneWithoutPrefixAndEnding("textures/".Length);
                            var act = new CompositeTexture(newLocation);
                            act.Rotation = ct.Rotation;
                            act.Alpha = ct.Alpha;
                            act.BlendedOverlays = ct.BlendedOverlays;
                            var bt = Bake(assetManager, act);
                            bt.TilesWidth = ct.TilesWidth;
                            tiles.Add(bt);
                        }
                    } else
                    {
                        var act = ct.Tiles[i];
                        act.BlendedOverlays = ct.BlendedOverlays;
                        var bt = Bake(assetManager, act);
                        bt.TilesWidth = ct.TilesWidth;
                        tiles.Add(bt);
                    }
                }

                bct.BakedTiles = tiles.ToArray();
            }

            return bct;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Base.ToString());
            sb.Append("@");
            sb.Append(Rotation);
            sb.Append("a");

            sb.Append(Alpha);
            if (Alternates != null)
            {
                sb.Append("alts:");
                foreach (var val in Alternates)
                {
                    val.ToString(sb);
                    sb.Append(",");
                }
            }
            if (BlendedOverlays != null)
            {
                sb.Append("ovs:");
                foreach (var val in BlendedOverlays)
                {
                    val.ToString(sb);
                    sb.Append(",");
                }
            }

            return sb.ToString();            
        }

        public void ToString(StringBuilder sb)
        {
            sb.Append(Base.ToString());
            sb.Append("@");
            sb.Append(Rotation);
            sb.Append("a");

            sb.Append(Alpha);
            if (Alternates != null)
            {
                sb.Append("alts:");
                foreach (var val in Alternates)
                {
                    sb.Append(val.ToString());
                    sb.Append(",");
                }
            }
            if (BlendedOverlays != null)
            {
                sb.Append("ovs:");
                foreach (var val in BlendedOverlays)
                {
                    sb.Append(val.ToString());
                    sb.Append(",");
                }
            }
        }


        public void FillPlaceholder(string search, string replace)
        {
            Base.Path = Base.Path.Replace(search, replace);
            if (BlendedOverlays != null) BlendedOverlays.Foreach((ov) => ov.Base.Path = ov.Base.Path.Replace(search, replace));
            if (Alternates != null) Alternates.Foreach((alt) => alt.FillPlaceholder(search, replace));
            if (Tiles != null) Tiles.Foreach((tile) => tile.FillPlaceholder(search, replace));
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

        /// <summary>
        /// If non-null also contains BakedName
        /// </summary>
        public BakedCompositeTexture[] BakedTiles = null;

        public int TilesWidth;

        /// <summary>
        /// For tiled textures, returns the selector index for which one of the tiles to use, for the given x,y,z position and tileSide
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="tileSide"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        public static int GetTiledTexturesSelector(BakedCompositeTexture[] tiles, int tileSide, int posX, int posY, int posZ)
        {
            var baseTile = tiles[0];
            int width = baseTile.TilesWidth;
            int height = tiles.Length / width;
            string name = baseTile.BakedName.Path;
            int index = name.IndexOf('@');
            int rotation = 0;
            if (index > 0)
            {
                int.TryParse(name[(index + 1)..name.Length], out rotation);
                rotation /= 90;
            }

            int x = 0;
            int y = 0;
            int z = 0;
            switch (tileSide)
            {
                case 0:
                    x = posX;
                    y = posZ;
                    z = posY;
                    break;
                case 1:
                    x = posZ;
                    y = -posX;
                    z = posY;
                    break;
                case 2:
                    x = -posX;
                    y = posZ;
                    z = posY;
                    break;
                case 3:
                    x = -posZ;
                    y = -posX;
                    z = posY;
                    break;
                case 4:
                    x = posX;
                    y = posY;
                    z = posZ;
                    break;
                case 5:
                    x = posX;
                    y = posY;
                    z = -posZ;
                    break;
            }

            switch (rotation)
            {
                case 0:
                    return GameMath.Mod(-x + y, width) + width * GameMath.Mod(-z, height);
                case 1:
                    return GameMath.Mod(z + y, width) + width * GameMath.Mod(x, height);
                case 2:
                    return GameMath.Mod(x + y, width) + width * GameMath.Mod(z, height);
                case 3:
                    return GameMath.Mod(-z + y, width) + width * GameMath.Mod(-x, height);
            }

            return 0;
        }
    }
}
