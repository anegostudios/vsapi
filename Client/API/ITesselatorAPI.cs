using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class TesselationMetaData
    {
        public static float[] randomRotations = new float[] { -22.5f, 22.5f, 90 - 22.5f, 90 + 22.5f, 180 - 22.5f, 180 + 22.5f, 270 - 22.5f, 270 + 22.5f };
        public static float[][] randomRotMatrices;

        static TesselationMetaData()
        {
            randomRotMatrices = new float[randomRotations.Length][];

            for (int i = 0; i < randomRotations.Length; i++)
            {
                float[] matrix = Mat4f.Create();
                Mat4f.Translate(matrix, matrix, 0.5f, 0.5f, 0.5f);
                Mat4f.RotateY(matrix, matrix, randomRotations[i] * GameMath.DEG2RAD);
                Mat4f.Translate(matrix, matrix, -0.5f, -0.5f, -0.5f);

                randomRotMatrices[i] = matrix;
            }

        }

        public ITexPositionSource TexSource;
        public int GeneralGlowLevel;
        public byte ClimateColorMapId;
        public byte SeasonColorMapId;
        public int? QuantityElements;
        public string[] SelectiveElements;
        public string[] IgnoreElements;
        public Dictionary<string, int[]> TexturesSizes;
        public string TypeForLogging;
        public bool WithJointIds;
        public bool WithDamageEffect;
        public Vec3f Rotation;

        public int GeneralWindMode;

        public bool UsesColorMap; // bubble up var
        public int[] defaultTextureSize;
        /// <summary>
        /// For performance, used to limit what height gets rendered on transparent textures of plants such as tallgrass and flowers 
        /// </summary>
        public float drawnHeight = 1f;

        public TesselationMetaData Clone()
        {
            return new TesselationMetaData()
            {
                TexSource = TexSource,
                GeneralGlowLevel = GeneralGlowLevel,
                ClimateColorMapId = ClimateColorMapId,
                SeasonColorMapId = SeasonColorMapId,
                QuantityElements = QuantityElements,
                SelectiveElements = SelectiveElements,
                IgnoreElements = IgnoreElements,
                TexturesSizes = TexturesSizes,
                TypeForLogging = TypeForLogging,
                WithJointIds = WithJointIds,
                UsesColorMap = UsesColorMap,
                defaultTextureSize = defaultTextureSize,
                GeneralWindMode = GeneralWindMode,
                WithDamageEffect = WithDamageEffect,
                Rotation = Rotation
            };
        }
    }



    /// <summary>
    /// Manager interface for Tesselators.
    /// </summary>
    public interface ITesselatorManager
    {
        /// <summary>
        /// Returns the default block mesh that being used by the engine when tesselating a chunk. The alternate and inventory versions are seperate.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        MeshData GetDefaultBlockMesh(Block block);

        /// <summary>
        /// Returns the default block mesh ref that being used by the engine when rendering a block in the inventory. The alternate and inventory versions are seperate.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        MultiTextureMeshRef GetDefaultBlockMeshRef(Block block);

        /// <summary>
        /// Returns the default block mesh ref that being used by the engine when rendering an item in the inventory. The alternate and inventory versions are seperate.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        MultiTextureMeshRef GetDefaultItemMeshRef(Item block);

        Shape GetCachedShape(AssetLocation location);
        MeshData CreateMesh(string typeForLogging, CompositeShape cshape, TextureSourceBuilder texgen, ITexPositionSource texSource = null);
        void ThreadDispose();
    }

    /// <summary>
    /// Interface that allows custom model model meshing for items, blocks and entities<br/>
    /// Texturing crash course:<br/>
    /// 1. Block, Item and Entity textures are loaded from json files in the form of a CompositeTexture instance<br/>
    /// 2. After connecting to a game server, the client inserts all of these textures into their type-respective texture atlasses<br/>
    /// 3. After insertion a "texture sub-id" is left behind in the CompositeTexture.Baked Property<br/>
    /// 4. You can now find the position of the texture inside the atlas through the Block/Item/Entity-TextureAtlasPositions arrays (teturesubid is the array key)<br/>
    /// <br/>
    /// Shape Tesselation crash course:<br/>
    /// 1. Block and Item shapes are loaded from json files in the form of a CompositeShape instance<br/>
    /// 2. A CompositeShape instance hold some block/item specific information as well as an identifier to a Shape instance<br/>
    /// 4. After connecting to a game server, the client loads all shapes from the shape folder then finds each blocks/items shape by its shape identifier <br/>
    /// 5. Result is a MeshData instance that holds all vertices, UV coords, colors and etc. for each block<br/>
    /// 6. That meshdata instance is <br/>
    ///    a) Held as-is in memory for using during chunk tesselation (you can get a reference to it through getDefaultBlockMesh())<br/>
    ///    b) "Compiled" to a Model for use during rendering in the gui. <br/>
    ///       Model Compilation means all it's mesh data is uploaded onto the graphcis through a VAO and a ModelRef instance is left behind which<br/>
    ///       can be used by the RenderAPI to render it.
    /// </summary>
    public interface ITesselatorAPI
    {
        /// <summary>
        /// Tesselates a block for you using given blocks shape and texture configuration
        /// </summary>
        /// <param name="block"></param>
        /// <param name="modeldata"></param>
        void TesselateBlock(Block block, out MeshData modeldata);

        /// <summary>
        /// Tesselates an item for you using given items shape and texture configuration
        /// </summary>
        /// <param name="item"></param>
        /// <param name="modeldata"></param>
        void TesselateItem(Item item, out MeshData modeldata);

        /// <summary>
        /// Tesselates an item for you using given items shape and texture configuration
        /// </summary>
        /// <param name="item"></param>
        /// <param name="forShape"></param>
        /// <param name="modeldata"></param>
        void TesselateItem(Item item, CompositeShape forShape, out MeshData modeldata);


        /// <summary>
        /// Tesselates an item for you using the items shape and your own defined texture configuration. You need to implement the ITextureSource yourself.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="modeldata"></param>
        /// <param name="texSource"></param>
        void TesselateItem(Item item, out MeshData modeldata, ITexPositionSource texSource);

        /// <summary>
        /// Tesselate a shape based on its composite shape file
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="compositeShape"></param>
        /// <param name="modeldata"></param>
        /// <param name="texSource"></param>
        /// <param name="generalGlowLevel"></param>
        /// <param name="climateColorMapIndex"></param>
        /// <param name="seasonColorMapIndex"></param>
        /// <param name="quantityElements"></param>
        /// <param name="selectiveElements"></param>
        void TesselateShape(string type, AssetLocation name, CompositeShape compositeShape, out MeshData modeldata, ITexPositionSource texSource, int generalGlowLevel = 0, byte climateColorMapIndex = 0, byte seasonColorMapIndex = 0, int? quantityElements = null, string[] selectiveElements = null);

        /// <summary>
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Uses the given collectible texture configuration as texture source.
        /// </summary>
        /// <param name="textureSourceCollectible"></param>
        /// <param name="shape"></param>
        /// <param name="modeldata"></param>
        /// <param name="meshRotationDeg"></param>
        /// <param name="quantityElements"></param>
        /// <param name="selectiveElements"></param>
        void TesselateShape(CollectibleObject textureSourceCollectible, Shape shape, out MeshData modeldata, Vec3f meshRotationDeg = null, int? quantityElements = null, string[] selectiveElements = null);

        /// <summary>
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Can be used to supply a custom texture source. 
        /// </summary>
        /// <param name="typeForLogging"></param>
        /// <param name="shapeBase"></param>
        /// <param name="modeldata"></param>
        /// <param name="texSource"></param>
        /// <param name="meshRotationDeg"></param>
        /// <param name="generalGlowLevel"></param>
        /// <param name="climateColorMapId"></param>
        /// <param name="seasonColorMapId"></param>
        /// <param name="quantityElements"></param>
        /// <param name="selectiveElements"></param>
        void TesselateShape(
            string typeForLogging, Shape shapeBase, out MeshData modeldata, ITexPositionSource texSource, Vec3f meshRotationDeg = null, 
            int generalGlowLevel = 0, byte climateColorMapId = 0, byte seasonColorMapId = 0, int? quantityElements = null, string[] selectiveElements = null    
        );


        /// <summary>
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Can be used to supply a custom texture source. Will add a customints array to the meshdata that holds each elements JointId for all its vertices (you will have to manually set the jointid for each element though)
        /// </summary>
        /// <param name="typeForLogging"></param>
        /// <param name="shapeBase"></param>
        /// <param name="modeldata"></param>
        /// <param name="texSource"></param>
        /// <param name="rotation"></param>
        /// <param name="quantityElements"></param>
        /// <param name="selectiveElements"></param>
        void TesselateShapeWithJointIds(string typeForLogging, Shape shapeBase, out MeshData modeldata, ITexPositionSource texSource, Vec3f rotation, int? quantityElements = null, string[] selectiveElements = null);

        /// <summary>
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Can be used to supply a custom texture source. Will add a customints array to the meshdata that holds each elements JointId for all its vertices (you will have to manually set the jointid for each element though)
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="shapeBase"></param>
        /// <param name="modeldata"></param>
        void TesselateShape(TesselationMetaData meta, Shape shapeBase, out MeshData modeldata);


        /// <summary>
        /// A helper method that turns a flat texture into its 1-voxel thick voxelized form
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="atlasSize"></param>
        /// <param name="atlasPos"></param>
        /// <returns></returns>
        MeshData VoxelizeTexture(CompositeTexture texture, Size2i atlasSize, TextureAtlasPosition atlasPos);


        /// <summary>
        /// A helper method that turns a flat texture into its 1-voxel thick voxelized form
        /// </summary>
        /// <param name="texturePixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="atlasSize"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        MeshData VoxelizeTexture(int[] texturePixels, int width, int height, Size2i atlasSize, TextureAtlasPosition pos);


        /// <summary>
        /// Returns the texture source from given block. This can be used to obtain the positions of the textures in the block texture atlas.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="altTextureNumber"></param>
        /// <param name="returnNullWhenMissing"></param>
        /// <returns></returns>
        ITexPositionSource GetTextureSource(Block block, int altTextureNumber = 0, bool returnNullWhenMissing = false);

        [Obsolete("Use GetTextureSource instead")]
        ITexPositionSource GetTexSource(Block block, int altTextureNumber = 0, bool returnNullWhenMissing = false);

        /// <summary>
        /// Returns the texture source from given item. This can be used to obtain the positions of the textures in the item texture atlas.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="returnNullWhenMissing"></param>
        /// <returns></returns>
        ITexPositionSource GetTextureSource(Item item, bool returnNullWhenMissing = false);

        /// <summary>
        /// Returns the texture source from given entity. This can be used to obtain the positions of the textures in the entity texture atlas.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="extraTextures"></param>
        /// <param name="altTextureNumber"></param>
        /// <param name="returnNullWhenMissing"></param>
        /// <returns></returns>
        ITexPositionSource GetTextureSource(Entity entity, Dictionary<string, CompositeTexture> extraTextures = null, int altTextureNumber = 0, bool returnNullWhenMissing = false);
    }
}
