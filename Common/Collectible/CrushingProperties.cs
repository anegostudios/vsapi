using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class CrushingProperties
    {
        /// <summary>
        /// If set, the block/item is crusable in a pulverizer and this is the resulting itemstack once the crushing time is over.
        /// </summary>
        public JsonItemStack CrushedStack;

        /// <summary>
        /// 0 = stone, 1 = copper, 2 = bronze, 3 = iron, 4 = steel
        /// </summary>
        public int HardnessTier = 1;

        public NatFloat Quantity = NatFloat.One;


        /// <summary>
        /// Makes a deep copy of the properties.
        /// </summary>
        /// <returns></returns>
        public CrushingProperties Clone()
        {
            return new CrushingProperties()
            {
                CrushedStack = this.CrushedStack.Clone(),
                HardnessTier = HardnessTier,
                Quantity = Quantity.Clone()
            };
        }
    }
}
