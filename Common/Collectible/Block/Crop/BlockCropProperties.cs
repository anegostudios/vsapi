using ProtoBuf;

namespace Vintagestory.API.Common
{
    public enum EnumSoilNutrient
    {
        N = 0, P = 1, K = 2
    }

    [ProtoContract]
    public class BlockCropProperties
    {
        [ProtoMember(1)]
        /// <summary>
        /// Which nutrient category this crop requires to grow
        /// </summary>
        public EnumSoilNutrient RequiredNutrient;

        [ProtoMember(2)]
        /// <summary>
        /// Total amount of nutrient consumed to reach full maturity. (100 is the maximum available for farmland)
        /// </summary>
        public float NutrientConsumption;

        [ProtoMember(3)]
        /// <summary>
        /// Amount of growth stages this crop has
        /// </summary>
        public int GrowthStages;

        [ProtoMember(4)]
        /// <summary>
        /// Total time in ingame days required for the crop to reach full maturity assuming full nutrient levels
        /// </summary>
        public float TotalGrowthDays;

        [ProtoMember(5)]
        /// <summary>
        /// If true, the player may harvests from the crop multiple times
        /// </summary>
        public bool MultipleHarvests;

        [ProtoMember(6)]
        /// <summary>
        /// When multiple harvets is true, this is the amount of growth stages the crop should go back when harvested
        /// </summary>
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
        public CropBehavior[] Behaviors = new CropBehavior[0];
    }
}
