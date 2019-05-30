using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Tells the engine how to handle default or subsequent similar behaviors
    /// </summary>
    public enum EnumHandling
    {
        /// <summary>
        /// Do run default and subsequent behaviors/event listeneres
        /// </summary>
        PassThrough,
        /// <summary>
        /// Do not execute the default behavior, but let subsequent behaviors/event listeneres still execute
        /// </summary>
        PreventDefault,
        /// <summary>
        /// Do not execute default behavior and do not let subsequent behaviors/event listeneres execute
        /// </summary>
        PreventSubsequent,
    }
}
