using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Defines a set of properties that allow an object to be ground in a quern.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"crushingPropsByType": {
	///	"ore-poor-ilmenite-*": {
	///		"crushedStack": {
	///			"type": "item",
	///			"code": "crushed-ilmenite"
	///		},
	///		"quantity": { "avg": 1 },
	///		"hardnessTier": 4
	///	},
	///	"ore-poor-cassiterite-*": {
	///		"crushedStack": {
	///			"type": "item",
	///			"code": "crushed-cassiterite"
	///		},
	///		"quantity": { "avg": ".33" },
	///		"hardnessTier": 1
	///	},
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class CrushingProperties
    {
        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// If set, the block/item is crusable in a pulverizer and this is the resulting itemstack once the crushing time is over.
        /// </summary>
        [DocumentAsJson] public JsonItemStack CrushedStack;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>1</jsondefault>-->
        /// The hardness tier for this collectible. Affects what pounder cap must be used for pulverization.
        /// - 0 = stone
        /// - 1 = copper
        /// - 2 = bronze
        /// - 3 = iron
        /// - 4 = steel
        /// </summary>
        [DocumentAsJson] public int HardnessTier = 1;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// The random quantity of item to return. Note that this value is multiplied by <see cref="CrushedStack"/>'s quantity.
        /// </summary>
        [DocumentAsJson] public NatFloat Quantity = NatFloat.One;


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
