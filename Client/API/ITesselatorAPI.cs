using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
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
        MeshRef GetDefaultBlockMeshRef(Block block);

        /// <summary>
        /// Returns the default block mesh ref that being used by the engine when rendering an item in the inventory. The alternate and inventory versions are seperate.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        MeshRef GetDefaultItemMeshRef(Item block);
    }

    /// <summary>
    /// Interface that allows custom model model meshing for items, blocks and entities
    /// Texturing crash course:
    /// 1. Block, Item and Entity textures are loaded from json files in the form of a CompositeTexture instance
    /// 2. After connecting to a game server, the client inserts all of these textures into their type-respective texture atlasses
    /// 3. After insertion a "texture sub-id" is left behind in the CompositeTexture.Baked Property
    /// 4. You can now find the position of the texture inside the atlas through the Block/Item/Entity-TextureAtlasPositions arrays (teturesubid is the array key)
    /// 
    /// Shape Tesselation crash course:
    /// 1. Block and Item shapes are loaded from json files in the form of a CompositeShape instance
    /// 2. A CompositeShape instance hold some block/item specific information as well as an identifier to a Shape instance
    /// 4. After connecting to a game server, the client loads all shapes from the shape folder then finds each blocks/items shape by its shape identifier 
    /// 5. Result is a MeshData instance that holds all vertices, UV coords, colors and etc. for each block
    /// 6. That meshdata instance is 
    ///    a) Held as-is in memory for using during chunk tesselation (you can get a reference to it through getDefaultBlockMesh())
    ///    b) "Compiled" to a Model for use during rendering in the gui. 
    ///       Model Compilation means all it's mesh data is uploaded onto the graphcis through a VAO and a ModelRef instance is left behind which
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
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Uses the given blocks texture configuration as texture source.
        /// </summary>
        /// <param name="textureSourceBlock"></param>
        /// <param name="shape"></param>
        /// <param name="modeldata"></param>
        /// <param name="meshRotationDeg">In degrees</param>
        /// <param name="quantityElements"></param>
        void TesselateShape(Block textureSourceBlock, Shape shape, out MeshData modeldata, Vec3f meshRotationDeg = null, int? quantityElements = null, string[] selectiveElements = null);

        /// <summary>
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Can be used to supply a custom texture source. 
        /// </summary>
        /// <param name="typeForLogging"></param>
        /// <param name="shapeBase"></param>
        /// <param name="modeldata"></param>
        /// <param name="texSource"></param>
        /// <param name="meshRotationDeg"></param>
        /// <param name="generalGlowLevel"></param>
        /// <param name="generalTintIndex"></param>
        /// <param name="quantityElements"></param>
        void TesselateShape(string typeForLogging, Shape shapeBase, out MeshData modeldata, ITexPositionSource texSource, Vec3f meshRotationDeg = null, int generalGlowLevel = 0, int generalTintIndex = 0, int? quantityElements = null, string[] selectiveElements = null);


        /// <summary>
        /// Turns a shape into a mesh data object that you can feed into the chunk tesselator or upload to the graphics card for rendering. Can be used to supply a custom texture source. Will add a customints array to the meshdata that holds each elements JointId for all its vertices (you will have to manually set the jointid for each element though)
        /// </summary>
        /// <param name="typeForLogging"></param>
        /// <param name="shapeBase"></param>
        /// <param name="modeldata"></param>
        /// <param name="texSource"></param>
        /// <param name="rotation"></param>
        void TesselateShapeWithJointIds(string typeForLogging, Shape shapeBase, out MeshData modeldata, ITexPositionSource texSource, Vec3f rotation, int? quantityElements = null, string[] selectiveElements = null);


        /// <summary>
        /// A helper method that turns a flat texture into its 1-voxel thick voxelized form
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="atlasSize"></param>
        /// <param name="atlasPos"></param>
        /// <returns></returns>
        MeshData VoxelizeTexture(CompositeTexture texture, int atlasSize, TextureAtlasPosition atlasPos);


        /// <summary>
        /// A helper method that turns a flat texture into its 1-voxel thick voxelized form
        /// </summary>
        /// <param name="texturePixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="atlasSize"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        MeshData VoxelizeTexture(int[] texturePixels, int width, int height, int atlasSize, TextureAtlasPosition pos);


        /// <summary>
        /// Returns the texture source from given block. This can be used to obtain the positions of the textures in the block texture atlas.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="altTextureNumber"></param>
        /// <param name="returnNullWhenMissing"></param>
        /// <returns></returns>
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
