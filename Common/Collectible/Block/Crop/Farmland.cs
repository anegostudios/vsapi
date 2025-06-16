using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IFarmlandBlockEntity
    {
        /// <summary>
        /// Total game hours from where on it can enter the next growth stage 
        /// </summary>
        double TotalHoursForNextStage { get; }

        /// <summary>
        /// The last time fertility increase was checked
        /// </summary>
        double TotalHoursFertilityCheck { get;  }

        /// <summary>
        /// Farmland has 3 nutrient levels N, P and K located in this array in that order. 
        /// Each nutrient level has a range of 0-100.
        /// </summary>
        float[] Nutrients { get; }

        /// <summary>
        /// The farmlands moisture level
        /// </summary>
        float MoistureLevel { get; }

        bool IsVisiblyMoist { get; }

        /// <summary>
        /// The fertility the soil will recover to (the soil from which the farmland was made of)
        /// </summary>
        int[] OriginalFertility { get; }

        /// <summary>
        /// The position of the farmland
        /// </summary>
        BlockPos Pos { get; }

        /// <summary>
        /// The position directly above the farmland
        /// </summary>
        BlockPos UpPos { get; }

        ITreeAttribute CropAttributes { get; }
    }
}