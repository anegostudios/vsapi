using ProtoBuf;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The three types of soil nutrient for farmland and crops.
    /// </summary>
    [DocumentAsJson]
    public enum EnumSoilNutrient
    {
        N = 0, P = 1, K = 2
    }

    [ProtoContract]
    public class BlockCropProperties
    {
        /// <summary>
        /// Which nutrient category this crop requires to grow
        /// </summary>
        [ProtoMember(1)]
        public EnumSoilNutrient RequiredNutrient;

        /// <summary>
        /// Total amount of nutrient consumed to reach full maturity. (100 is the maximum available for farmland)
        /// </summary>
        [ProtoMember(2)]
        public float NutrientConsumption;

        /// <summary>
        /// Amount of growth stages this crop has
        /// </summary>
        [ProtoMember(3)]
        public int GrowthStages;

        /// <summary>
        /// Total time in ingame days required for the crop to reach full maturity assuming full nutrient levels
        /// </summary>
        [ProtoMember(4)]
        public float TotalGrowthDays;

        /// <summary>
        /// Total time in ingame months required for the crop to reach full maturity assuming full nutrient levels
        /// </summary>
        [ProtoMember(11)]
        public float TotalGrowthMonths;

        /// <summary>
        /// If true, the player may harvests from the crop multiple times
        /// </summary>
        [ProtoMember(5)]
        public bool MultipleHarvests;

        /// <summary>
        /// When multiple harvets is true, this is the amount of growth stages the crop should go back when harvested
        /// </summary>
        [ProtoMember(6)]
        public int HarvestGrowthStageLoss;

        [ProtoMember(7)]
        public float ColdDamageBelow;
        [ProtoMember(8)]
        public float DamageGrowthStuntMul;
        [ProtoMember(9)]
        public float ColdDamageRipeMul;

        [ProtoMember(10)]
        public float HeatDamageAbove;

        /// <summary>
        /// Allows customization of crop growth behavior. BlockEntityFarmland calls methods on all behaviors to allow greater control.
        /// </summary>
        public CropBehavior[] Behaviors = System.Array.Empty<CropBehavior>();
    }
}
