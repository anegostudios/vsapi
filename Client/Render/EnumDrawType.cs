
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draw types for blocks.
    /// </summary>
    [DocumentAsJson]
    public enum EnumDrawType
    {
        BlockLayer_1 = 1,
        BlockLayer_2 = 2, 
        BlockLayer_3 = 3,
        BlockLayer_4 = 4,
        BlockLayer_5 = 5,
        BlockLayer_6 = 6,
        BlockLayer_7 = 7,

        /// <summary>
        /// You will most likely use JSON for all assets with custom shapes.
        /// </summary>
        JSON = 8,

        Empty = 9,
        Cube = 10,
        Cross = 11,
        Transparent = 12,
        Liquid = 13,
        TopSoil = 14,
        CrossAndSnowlayer = 15,
        JSONAndWater = 16,
        JSONAndSnowLayer = 17,
        CrossAndSnowlayer_2 = 18,
        CrossAndSnowlayer_3 = 19,
        CrossAndSnowlayer_4 = 20,
        SurfaceLayer = 21
    }
}
