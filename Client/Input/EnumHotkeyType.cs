using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public enum HotkeyType
    {
        /// <summary>
        /// Controls that are always available (survival and creative mode)
        /// </summary>
        GeneralControls,
        /// <summary>
        /// Controls that control the players character. Only triggered when the player not currently inside a dialog.
        /// </summary>
        CharacterControls,
        /// <summary>
        /// Controls that are only available in creative mode
        /// </summary>
        CreativeTool,
        /// <summary>
        /// Controls that are only available in creative or spectator mode
        /// </summary>
        CreativeOrSpectatorTool,
        /// <summary>
        /// Developer tools
        /// </summary>
        DevTool,
    }
}
