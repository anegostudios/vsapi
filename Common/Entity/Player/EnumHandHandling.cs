
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// How the engine should handle attacking with an item in hands
    /// </summary>
    public enum EnumHandHandling
    {
        /// <summary>
        /// Uses the engine default behavior which is to play an attack animation and do block breaking/damage entities if in range. Will not call the *Step and *Stop methods.
        /// </summary>
        NotHandled = 0,
        /// <summary>
        /// Uses the engine default behavior which is to play an attack or build animation and do block breaking/damage entities if in range,
        /// but also notify the server that the Use/Attack method has to be called serverside as well. Will call the *Step and *Stop methods.
        /// </summary>
        Handled = 1,
        /// <summary>
        /// Do not play any default first person attack animation, but do block breaking/damage entities if in range. Notifies that the server that the Use/Attack method has to be called serverside as well. Will call the *Step and *Stop methods.
        /// </summary>
        PreventDefaultAnimation = 2,
        /// <summary>
        /// Do play first person attack animation, don't break blocks/damage entities in range. Notifies that the server that the Use/Attack method has to be called serverside as well. Will call the *Step and *Stop methods.
        /// </summary>
        PreventDefaultAction = 3,
        /// <summary>
        /// Do not play any first person attack animation, don't break blocks in range or damage entities in range. Notifies that the server that the Use/Attack method has to be called serverside as well. Will call the *Step and *Stop methods.
        /// </summary>
        PreventDefault = 4,
    }

    public enum EnumHandInteract
    {
        None = 0,
        HeldItemAttack = 1,
        HeldItemInteract = 2,
        BlockInteract = 3
    }
}
