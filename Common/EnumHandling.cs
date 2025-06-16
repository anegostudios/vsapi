
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// Tells the engine how to handle default or subsequent similar behaviors
    /// </summary>
    public enum EnumHandling
    {
        /// <summary>
        /// Do run default and subsequent behaviors/event listeneres, ignore return values
        /// </summary>
        PassThrough,
        /// <summary>
        /// Do run default and subsequent behaviors/event listeneres, use return values
        /// </summary>
        Handled,
        /// <summary>
        /// Do not execute the default behavior, but let subsequent behaviors/event listeneres still execute, use return values
        /// </summary>
        PreventDefault,
        /// <summary>
        /// Do not execute default behavior and do not let subsequent behaviors/event listeneres execute, use return values
        /// </summary>
        PreventSubsequent,
    }
}
