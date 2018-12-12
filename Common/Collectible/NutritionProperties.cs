using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public enum EnumFoodCategory
    {
        Fruit,
        Vegetable,
        Protein,
        Grain,
        Dairy,
        Unknown
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
        public float Saturation = 0f;

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
                Saturation = Saturation,
                Health = Health,
                EatenStack = EatenStack?.Clone()
            };
        }

    }
}
