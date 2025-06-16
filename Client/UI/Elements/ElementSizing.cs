
#nullable disable
namespace Vintagestory.API.Client
{
    public enum ElementSizing
    {
        /// <summary>
        /// Only multiplied with scale factor
        /// </summary>
        Fixed,
        
        /// <summary>
        /// Value between 0 and 100% of parent element 
        /// </summary>
        Percentual,

        /// <summary>
        /// Size determined by child elements 
        /// </summary>
        FitToChildren,

        /// <summary>
        /// Value between 0 and 100% of parent element. Will substract fixedWidth and fixedHeight from the final size
        /// </summary>
        PercentualSubstractFixed
    }
}
