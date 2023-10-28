using System;

namespace Vintagestory.API.Common
{
    public enum EnumFoodCategory
    {
        NoNutrition = -1,
        Fruit = 0,
        Vegetable = 1,
        Protein = 2,
        Grain = 3,
        Dairy = 4,
        Unknown = 5
    }

    public class FoodNutritionProperties
    {
        /// <summary>
        /// The category of the food.
        /// </summary>
        public EnumFoodCategory FoodCategory;
        
        /// <summary>
        /// The saturation restored by the food.
        /// </summary>
        public float Satiety = 0f;

        [Obsolete("Use Satiety instead.")]
        public float Saturation
        {
            get { return Satiety; }
            set { Satiety = value; }
        }

        public float Intoxication
        {
            get; set;
        }

        /// <summary>
        /// The delay before that extra saturation starts to go away.
        /// </summary>
        public float SaturationLossDelay = 10f;

        /// <summary>
        /// The health restored by the food.
        /// </summary>
        public float Health = 0f;
        
        /// <summary>
        /// The item that was eaten.
        /// </summary>
        public JsonItemStack EatenStack;

        /// <summary>
        /// Duplicates the nutrition properties, which includes cloning the stack that was eaten.
        /// </summary>
        /// <returns></returns>
        public FoodNutritionProperties Clone()
        {
            return new FoodNutritionProperties()
            {
                FoodCategory = FoodCategory,
                Satiety = Satiety,
                Health = Health,
                Intoxication = Intoxication,
                EatenStack = EatenStack?.Clone()
            };
        }

    }
}
