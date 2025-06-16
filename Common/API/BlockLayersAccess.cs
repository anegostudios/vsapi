
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// Constants used for GetBlock or GetBlockId calls throughout the engine, to guide whether the block should be read from the solid blocks layer, the fluids layer, or perhaps both.
    /// <br/>The game engine supports different block layers in 1.17+.  Currently there is a solid blocks layer (e.g. terrain, loose stones, plants, reeds) and a fluid layer (e.g. still water, flowing water, lava, lake ice). Both layers can contain a block at the same position.
    /// <br/>Use the .Default value for getting blocks in the general case, use the .Solid/.Fluid value to read from the solid blocks or fluid layer specifically
    /// </summary>
    public class BlockLayersAccess
    {
        /// <summary>
        /// GetBlock:<br/>
        /// Returns the contents of the solid blocks layer. If the solid blocks layer is completely empty, returns the value from the fluid layer instead. Keep in mind, the fluid layer might contain ice!<br/>
        /// If this returns air (block id 0) then it really is an air block: there is nothing in either the solid blocks layer or the fluid layer
        /// SetBlock:<br/>
        /// Sets supplied block id. If that blocks property ForFluidsLayer is true, it will be placed in the fluids layer instead and the solid block layer will be cleared.
        /// </summary>
        public const int Default = 0;

        /// <summary>
        /// Returns a block from the solid blocks layer only.  <br/>A return value of air (block id 0) signifies no solid block is present but there may still be a fluid block, for example still water
        /// </summary>
        public const int SolidBlocks = 1;

        /// <summary>
        /// Returns a block from the solid blocks layer only. Same as <see cref="SolidBlocks"/>
        /// </summary>
        public const int Solid = 1;

        /// <summary>
        /// Returns a block from the fluid layer only, which might contain ice. <br/>A return value of air (block id 0) signifies no fluid is present at this position
        /// </summary>
        public const int Fluid = 2;

        /// <summary>
        /// Returns the contents of the fluid layer, unless it is empty in which case returns the solid blocks layer - useful for generating the RainHeightMap for example
        /// </summary>
        public const int FluidOrSolid = 3;

        /// <summary>
        /// Returns the most solid block, in the following order: <br/>
        /// 1. Ice in the fluid layer<br/>
        /// 2. Whichever block is in the solid blocks layer<br/>
        /// 3. Air (block id 0) if neither is present - note this access may therefore return 0 even if liquid water or lava is present in the fluid layer.<br/><br/>
        /// Useful for block collision checks, side solid checks (can a block attach here?) and similar physics
        /// </summary>
        public const int MostSolid = 4;
    }

}
