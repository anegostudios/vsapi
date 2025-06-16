using System;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Materials of which a block may be made of.
    /// Currently only used for mining speed for tools and blast resistance.
    /// </summary>
    [DocumentAsJson]
    public enum EnumBlockMaterial
    {
        Air,
        Soil,
        Gravel,
        Sand,
        Wood,
        Leaves,
        Stone,
        Ore,
        Liquid,
        Snow,
        Ice,
        Metal,
        Mantle,
        Plant,
        Glass,
        Ceramic,
        Cloth,
        Lava,
        Brick,
        Fire,
        Meta,
        Other,
    }


    public static class BlockMaterialUtil
    {
        static double[][] blastResistances;
        static double[][] blastDropChances;

        static BlockMaterialUtil()
        {
            var blasttypes = Enum.GetValues(typeof(EnumBlastType));
            var materials = Enum.GetValues(typeof(EnumBlockMaterial));

            blastResistances = new double[blasttypes.Length][];
            blastDropChances = new double[blasttypes.Length][];

            // Rock blast
            int blastType = (int)EnumBlastType.RockBlast;
            
            blastResistances[blastType] = new double[materials.Length];

            blastDropChances[blastType] = new double[materials.Length];
            blastDropChances[blastType].Fill(0.2);

            blastResistances[blastType][(int)EnumBlockMaterial.Air] = 0;
            blastResistances[blastType][(int)EnumBlockMaterial.Soil] = 3.2;
            blastResistances[blastType][(int)EnumBlockMaterial.Gravel] = 3.2;
            blastResistances[blastType][(int)EnumBlockMaterial.Sand] = 2.4;
            blastResistances[blastType][(int)EnumBlockMaterial.Wood] = 2;
            blastResistances[blastType][(int)EnumBlockMaterial.Leaves] = 0.4;
            blastResistances[blastType][(int)EnumBlockMaterial.Stone] = 2;
            blastResistances[blastType][(int)EnumBlockMaterial.Ore] = 16;
            blastResistances[blastType][(int)EnumBlockMaterial.Liquid] = 4;
            blastResistances[blastType][(int)EnumBlockMaterial.Snow] = 0.4;
            blastResistances[blastType][(int)EnumBlockMaterial.Ice] = 2;
            blastResistances[blastType][(int)EnumBlockMaterial.Metal] = 8;
            blastResistances[blastType][(int)EnumBlockMaterial.Mantle] = 999999;
            blastResistances[blastType][(int)EnumBlockMaterial.Plant] = 0.1;
            blastResistances[blastType][(int)EnumBlockMaterial.Glass] = 0.1;
            blastResistances[blastType][(int)EnumBlockMaterial.Ceramic] = 0.3;
            blastResistances[blastType][(int)EnumBlockMaterial.Cloth] = 0.2;
            blastResistances[blastType][(int)EnumBlockMaterial.Lava] = 6;
            blastResistances[blastType][(int)EnumBlockMaterial.Brick] = 4;
            blastResistances[blastType][(int)EnumBlockMaterial.Other] = 1;


            // Ore blast
            blastType = (int)EnumBlastType.OreBlast;
            blastDropChances[blastType] = new double[materials.Length];
            blastDropChances[blastType].Fill(0.25);
            blastDropChances[blastType][(int)EnumBlockMaterial.Ore] = 0.9;

            blastResistances[blastType] = new double[materials.Length];
            blastResistances[blastType][(int)EnumBlockMaterial.Air] = 0;
            blastResistances[blastType][(int)EnumBlockMaterial.Soil] = 1.6;
            blastResistances[blastType][(int)EnumBlockMaterial.Gravel] = 3.2;
            blastResistances[blastType][(int)EnumBlockMaterial.Sand] = 2.4;
            blastResistances[blastType][(int)EnumBlockMaterial.Wood] = 2;
            blastResistances[blastType][(int)EnumBlockMaterial.Leaves] = 0.4;
            blastResistances[blastType][(int)EnumBlockMaterial.Stone] = 3;
            blastResistances[blastType][(int)EnumBlockMaterial.Liquid] = 4;
            blastResistances[blastType][(int)EnumBlockMaterial.Snow] = 0.4;
            blastResistances[blastType][(int)EnumBlockMaterial.Ice] = 2;
            blastResistances[blastType][(int)EnumBlockMaterial.Metal] = 8;
            blastResistances[blastType][(int)EnumBlockMaterial.Mantle] = 999999;
            blastResistances[blastType][(int)EnumBlockMaterial.Plant] = 0.1;
            blastResistances[blastType][(int)EnumBlockMaterial.Glass] = 0.1;
            blastResistances[blastType][(int)EnumBlockMaterial.Ceramic] = 0.3;
            blastResistances[blastType][(int)EnumBlockMaterial.Cloth] = 0.2;
            blastResistances[blastType][(int)EnumBlockMaterial.Lava] = 6;
            blastResistances[blastType][(int)EnumBlockMaterial.Brick] = 4;
            blastResistances[blastType][(int)EnumBlockMaterial.Other] = 1;



            // Entity blast
            blastType = (int)EnumBlastType.EntityBlast;
            blastDropChances[blastType] = new double[materials.Length];
            blastDropChances[blastType].Fill(0.5);

            blastResistances[blastType] = new double[materials.Length];
            blastResistances[blastType][(int)EnumBlockMaterial.Air] = 0;
            blastResistances[blastType][(int)EnumBlockMaterial.Soil] = 3.2 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Gravel] = 3.2 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Sand] = 2.4 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Wood] = 2 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Leaves] = 0.4 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Stone] = 5.6 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Ore] = 4 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Liquid] = 4 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Snow] = 0.4 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Ice] = 2 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Metal] = 8 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Mantle] = 999999 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Plant] = 0.1 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Glass] = 0.1 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Ceramic] = 0.3 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Cloth] = 0.2 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Lava] = 6 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Brick] = 4 * 12;
            blastResistances[blastType][(int)EnumBlockMaterial.Other] = 1 * 12;


        }

        /// <summary>
        /// Calculates the blast resistance of a given material.
        /// </summary>
        /// <param name="blastType">The blast type the material is being it with.</param>
        /// <param name="material">The material of the block.</param>
        /// <returns>the resulting blast resistance.</returns>
        public static double MaterialBlastResistance(EnumBlastType blastType, EnumBlockMaterial material)
        {
            return blastResistances[(int)blastType][(int)material];
        }

        /// <summary>
        /// Calculates the blast drop chance of a given material.
        /// </summary>
        /// <param name="blastType">The blast type the material is being it with.</param>
        /// <param name="material">The material of the block.</param>
        /// <returns>the resulting drop chance.</returns>
        public static double MaterialBlastDropChances(EnumBlastType blastType, EnumBlockMaterial material)
        {
            return blastDropChances[(int)blastType][(int)material];
        }
    }

}
