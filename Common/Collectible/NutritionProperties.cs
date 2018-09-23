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
        public EnumFoodCategory FoodCategory;
        
        public float Saturation = 0f;

        public float SaturationLossDelay = 10f;

        public float Health = 0f;

        public JsonItemStack EatenStack;


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
