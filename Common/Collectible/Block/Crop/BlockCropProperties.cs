namespace Vintagestory.API.Common
{
    public enum EnumSoilNutrient
    {
        N = 0, P = 1, K = 2
    }

    public class BlockCropProperties
    {
        /// <summary>
        /// Which nutrient category this crop requires to grow
        /// </summary>
        public EnumSoilNutrient RequiredNutrient;

        /// <summary>
        /// Total amount of nutrient consumed to reach full maturity. (100 is the maximum available for farmland)
        /// </summary>
        public float NutrientConsumption;

        /// <summary>
        /// Amount of growth stages this crop has
        /// </summary>
        public int GrowthStages;

        /// <summary>
        /// Total time in ingame days required for the crop to reach full maturity assuming full nutrient levels
        /// </summary>
        public float TotalGrowthDays;

        /// <summary>
        /// If true, the player may harvests from the crop multiple times
        /// </summary>
        public bool MultipleHarvests;

        /// <summary>
        /// When multiple harvets is true, this is the amount of growth stages the crop should go back when harvested
        /// </summary>
        public int HarvestGrowthStageLoss;

        /// <summary>
        /// Allows customization of crop growth behavior. BlockEntityFarmland calls methods on all behaviors to allow greater control.
        /// </summary>
        public CropBehavior[] Behaviors = new CropBehavior[0];
    }
}
