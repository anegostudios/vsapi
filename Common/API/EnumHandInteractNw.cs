#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Internal state number when sending an interaction from client to server
    /// </summary>
    public enum EnumHandInteractNw
    {
        StartHeldItemUse = 0,
        CancelHeldItemUse = 1,
        StopHeldItemUse = 2,
        StepHeldItemUse = 3,

        StartBlockUse = 4,
        CancelBlockUse = 5,
        StopBlockUse = 6,
        StepBlockUse = 7
    }
}
