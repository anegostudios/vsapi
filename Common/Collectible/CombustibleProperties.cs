using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public enum EnumSmeltType
    {
        Smelt,
        Cook,
        Bake,
        Convert,
        Fired
    }

    /// <summary>
    /// Used for an items combustible value
    /// </summary>
    public class CombustibleProperties
    {
        /// <summary>
        /// The temperature at which it burns
        /// </summary>
        public int BurnTemperature;

        /// <summary>
        /// For how long it burns
        /// </summary>
        public float BurnDuration;

        /// <summary>
        /// How many degrees celsius it can resists before it ignites
        /// </summary>
        public int HeatResistance = 500;

        /// <summary>
        /// How many degrees celsius it takes to smelt/transform this into another. Only used when put in a stove and Melted is set 
        /// </summary>
        public int MeltingPoint;

        /// <summary>
        /// If there is a melting point, the max temperature it can reach. Set to 0 for no limit 
        /// </summary>
        public int MaxTemperature;

        /// <summary>
        /// For how many seconds the temperature has to be above the melting point until the item is smelted
        /// </summary>
        public float MeltingDuration;

        /// <summary>
        /// How much smoke this item produces when being used as fuel
        /// </summary>
        public float SmokeLevel = 1f;

        /// <summary>
        /// How many ores are required to produce one output stack
        /// </summary>
        public int SmeltedRatio = 1;

        /// <summary>
        /// Used for correct naming in the tool tip
        /// </summary>
        public EnumSmeltType SmeltingType;

        /// <summary>
        /// If set, the block/item is smeltable in a furnace and this is the resulting itemstack once the MeltingPoint has been reached for the supplied duration.
        /// </summary>
        public JsonItemStack SmeltedStack;

        /// <summary>
        /// If true (default) a container is required to smelt this item. 
        /// </summary>
        public bool RequiresContainer = true;


        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public CombustibleProperties Clone()
        {
            CombustibleProperties cloned = new CombustibleProperties();

            cloned.BurnDuration = BurnDuration;
            cloned.BurnTemperature = BurnTemperature;
            cloned.HeatResistance = HeatResistance;
            cloned.MeltingDuration = MeltingDuration;
            cloned.MeltingPoint = MeltingPoint;
            cloned.SmokeLevel = SmokeLevel;
            cloned.SmeltedRatio = SmeltedRatio;
            cloned.RequiresContainer = RequiresContainer;
            cloned.SmeltingType = SmeltingType;
            cloned.MaxTemperature = MaxTemperature;

            if (SmeltedStack != null)
            {
                cloned.SmeltedStack = SmeltedStack.Clone();
            }
            

            return cloned;
        }

    }
}
