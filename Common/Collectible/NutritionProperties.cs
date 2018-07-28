using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public enum EnumFoodNutrient
    {
        Fruit,
        Vegetable,
        Protein,
        Grain,
        Dairy
    }

    public class FoodNutritionProperties
    {
        public EnumFoodNutrient FoodCategory;

        public float Saturation = 0f;

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
