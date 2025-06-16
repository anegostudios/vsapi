
#nullable disable
namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Used to define how variant types interact with each other to create unique objects.
    /// </summary>
    [DocumentAsJson]
    public enum EnumCombination
    {
        /// <summary>
        /// This variant type will ignore all other variant types. Each state is appended onto the object's code, without any other variant types.
        /// </summary>
        Add,
        /// <summary>
        /// Default behavior - This variant's states will be enumerated with all other variant states that have this combination.
        /// </summary>
        Multiply,
        /// <summary>
        /// This variant's states will be enumerated only with the selected variant group. You must select another variant code using the 'onVariant' property. 
        /// </summary>
        SelectiveMultiply
    }
}
