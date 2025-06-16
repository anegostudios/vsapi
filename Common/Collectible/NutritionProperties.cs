using System;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Types of nutrition for foods.
    /// </summary>
    [DocumentAsJson]
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

    /// <summary>
    /// Allows you to make collectibles edible, and adds data about their nutrition.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"nutritionPropsByType": {
	///	"*-flyagaric-*": {
	///		"saturation": 80,
	///		"health": -6.5,
	///		"foodcategory": "Vegetable"
	///	},
	///	"*-earthball-*": {
	///		"saturation": 80,
	///		"health": -8,
	///		"foodcategory": "Vegetable"
	///	},
	/// ...
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class FoodNutritionProperties
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>Fruit</jsondefault>-->
        /// The category of the food.
        /// </summary>
        [DocumentAsJson] public EnumFoodCategory FoodCategory;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// The saturation restored by the food.
        /// </summary>
        [DocumentAsJson] public float Satiety = 0f;

        /// <summary>
        /// <!--<jsonoptional>Obsolete</jsonoptional>-->
        /// Obsolete - Please use <see cref="Satiety"/> instead.
        /// </summary>
        [DocumentAsJson]
        [Obsolete("Use Satiety instead.")]
        public float Saturation
        {
            get { return Satiety; }
            set { Satiety = value; }
        }

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much eating this will affect the player's intoxication.
        /// </summary>
        [DocumentAsJson]
        public float Intoxication
        {
            get; set;
        }

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>10</jsondefault>-->
        /// The delay before that extra saturation starts to go away.
        /// </summary>
        [DocumentAsJson] public float SaturationLossDelay = 10f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// The health restored by the food. Usually actually used to hurt the player with negative values.
        /// </summary>
        [DocumentAsJson] public float Health = 0f;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// When an instance of this collectible is eaten, what item stack should be returned to the player?
        /// Possible example: Eating a 'meat on a stick' item would return a single stick.
        /// (Note: Bowl meals/liquids are specially designed to do this through their attributes and class.)
        /// </summary>
        [DocumentAsJson] public JsonItemStack EatenStack;

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
