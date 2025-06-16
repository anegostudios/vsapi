using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The type of smelting for the collectible. This effects how the object is smelted.
    /// </summary>
    [DocumentAsJson]
    public enum EnumSmeltType
    {
        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Smelt,

        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Cook,

        /// <summary>
        /// This collectible must be baked in a clay oven. Note that you will likely want to use <see cref="BakingProperties"/> in the item's attributes.
        /// </summary>
        Bake,

        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Convert,

        /// <summary>
        /// This collectible must be fired in a kiln.
        /// </summary>
        Fire
    }

    public interface ICombustible
    {
        float GetBurnDuration(IWorldAccessor world, BlockPos pos);
    }

    /// <summary>
    /// Marks an item as combustible, either by cooking, smelting or firing. This can either imply it is used as a fuel, or can be cooked into another object.
    /// </summary>
    /// <example>
    /// Cooking:
    /// <code language="json">
    ///"combustiblePropsByType": {
	///	"bushmeat-raw": {
	///		"meltingPoint": 150,
	///		"meltingDuration": 30,
	///		"smeltedRatio": 1,
	///		"smeltingType": "cook",
	///		"smeltedStack": {
	///			"type": "item",
	///			"code": "bushmeat-cooked"
	///		},
	///		"requiresContainer": false
	///	}
	///},
    /// </code>
    /// Clay Firing:
    /// <code language="json">
    ///"combustiblePropsByType": {
	///	"bowl-raw": {
	///		"meltingPoint": 650,
	///		"meltingDuration": 45,
	///		"smeltedRatio": 1,
	///		"smeltingType": "fire",
	///		"smeltedStack": {
	///			"type": "block",
	///			"code": "bowl-fired"
	///		},
	///		"requiresContainer": false
	///	}
	///},
    /// </code>
    /// Fuel Source:
    /// <code language="json">
    ///"combustibleProps": {
	///	"burnTemperature": 1300,
	///	"burnDuration": 40
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class CombustibleProperties
    {
        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The temperature at which this collectible burns when used as a fuel.
        /// </summary>
        [DocumentAsJson] public int BurnTemperature;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The duration, in real life seconds, that this collectible burns for when used as a fuel. 
        /// </summary>
        [DocumentAsJson] public float BurnDuration;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>500</jsondefault>-->
        /// How many degrees celsius it can resists before it ignites
        /// </summary>
        [DocumentAsJson] public int HeatResistance = 500;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// How many degrees celsius it takes to smelt/transform this collectible into another. Required if <see cref="SmeltedStack"/> is set.
        /// </summary>
        [DocumentAsJson] public int MeltingPoint;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// If there is a melting point, the max temperature it can reach. A value of 0 implies no limit.
        /// </summary>
        [DocumentAsJson] public int MaxTemperature;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// For how many seconds the temperature has to be above the melting point until the item is smelted. Recommended if <see cref="SmeltedStack"/> is set.
        /// </summary>
        [DocumentAsJson] public float MeltingDuration;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much smoke this item produces when being used as fuel
        /// </summary>
        [DocumentAsJson] public float SmokeLevel = 1f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How many of this collectible are needed to smelt into <see cref="SmeltedStack"/>.
        /// </summary>
        [DocumentAsJson] public int SmeltedRatio = 1;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>Smelt</jsondefault>-->
        /// Some smelt types have specific functionality, and are also used for correct naming in the tool tip.
        /// If using <see cref="EnumSmeltType.Bake"/>, you will need to include <see cref="BakingProperties"/> in your item attributes.
        /// </summary>
        [DocumentAsJson] public EnumSmeltType SmeltingType;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// If set, this is the resulting itemstack once the MeltingPoint has been reached for the supplied duration.
        /// </summary>
        [DocumentAsJson] public JsonItemStack SmeltedStack;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>true</jsondefault>-->
        /// If true, a container is required to smelt this item. 
        /// </summary>
        [DocumentAsJson] public bool RequiresContainer = true;


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

    /// <summary>
    /// Baking Properties are collectible attribute used for baking items in a clay oven.
    /// You will need to add these attributes if using <see cref="EnumSmeltType.Bake"/> inside <see cref="CombustibleProperties.SmeltingType"/>.
    /// </summary>
    /// <example>
    /// Example taken from bread. Note that the levelTo value in the baking stage is the same as the levelFrom in the next baking stage.
    /// <code language="json">
    ///"attributesByType": {
	///	"*-partbaked": {
	///		"bakingProperties": {
	///			"temp": 160,
	///			"levelFrom": 0.25,
	///			"levelTo": 0.5,
	///			"startScaleY": 0.95,
	///			"endScaleY": 1.10,
	///			"resultCode": "bread-{type}-perfect",
	///			"initialCode": "dough-{type}"
	///		}
	///	},
	///	"*-perfect": {
	///		"bakingProperties": {
	///			"temp": 160,
	///			"levelFrom": 0.5,
	///			"levelTo": 0.75,
	///			"startScaleY": 1.10,
	///			"endScaleY": 1.13,
	///			"resultCode": "bread-{type}-charred",
	///			"initialCode": "bread-{type}-partbaked"
	///		}
	///	},
	///	"*-charred": {
	///		"bakingProperties": {
	///			"temp": 160,
	///			"levelFrom": 0.75,
	///			"levelTo": 1,
	///			"startScaleY": 1.13,
	///			"endScaleY": 1.10,
	///			"initialCode": "bread-{type}-perfect"
	///		}
	///	}
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class BakingProperties
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>160</jsondefault>-->
        /// The temperature required to bake the item.
        /// </summary>
        [DocumentAsJson] public float? Temp;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// The initial value, from 0 to 1, that determines how cooked the item is.
        /// When cooking an object with numerous cooking stages, these stages can be stacked using these values. Simply set the second stage's <see cref="LevelFrom"/> to the first stages <see cref="LevelTo"/>.
        /// </summary>
        [DocumentAsJson] public float LevelFrom;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>1</jsondefault>-->
        /// The final value, from 0 to 1, that determines how cooked the item is.
        /// When the cooking value reaches this value, the collectible will change into the next item.
        /// When cooking an object with numerous cooking stages, these stages can be stacked using these values. Simply set the second stage's <see cref="LevelFrom"/> to the first stages <see cref="LevelTo"/>.
        /// </summary>
        [DocumentAsJson] public float LevelTo;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// The Y scale of this collectible when it begins cooking. Value will be linearly interpolated between this and <see cref="EndScaleY"/>.
        /// </summary>
        [DocumentAsJson] public float StartScaleY;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// The Y scale of this collectible when it has finished cooking. Value will be linearly interpolated between <see cref="StartScaleY"/> and this.
        /// </summary>
        [DocumentAsJson] public float EndScaleY;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The code of the resulting collectible when this item finishes its cooking stage.
        /// </summary>
        [DocumentAsJson] public string ResultCode;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The code of the initial collectible that is being baked.
        /// </summary>
        [DocumentAsJson] public string InitialCode;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>false</jsondefault>-->
        /// If true, only one instance of this collectible can be baked at a time. If false, 4 of this collectible can be baked at a time.
        /// </summary>
        [DocumentAsJson] public bool LargeItem;

        public static BakingProperties ReadFrom(ItemStack stack)
        {
            if (stack == null) return null;

            BakingProperties result = stack.Collectible?.Attributes?["bakingProperties"]?.AsObject<BakingProperties>();
            if (result == null) return null;

            if (result.Temp == null || result.Temp == 0)
            {
                CombustibleProperties props = stack.Collectible.CombustibleProps;
                if (props != null) result.Temp = props.MeltingPoint - 40;
            }
            return result;
        }
    }


}
