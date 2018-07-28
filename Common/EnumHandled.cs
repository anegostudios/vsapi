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
        /// Do run default and subsequent behaviors
        /// </summary>
        NotHandled,
        /// <summary>
        /// Do not execute the default behavior, but let subsequent behaviors still execute
        /// </summary>
        PreventDefault,
        /// <summary>
        /// Do not execute default behavior, do not let subsequent behaviors execute
        /// </summary>
        Last
    }
}
