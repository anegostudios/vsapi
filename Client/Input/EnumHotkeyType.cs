
#nullable disable
namespace Vintagestory.API.Client
{
    public enum HotkeyType
    {
        /// <summary>
        /// Help and overlays that are always available (survival and creative mode)
        /// </summary>
        HelpAndOverlays,
        /// <summary>
        /// Mouse modifiers (i.e. for Shift- and Ctrl- click)
        /// </summary>
        MouseModifiers,
        /// <summary>
        /// Controls that are always available (survival and creative mode)
        /// </summary>
        GUIOrOtherControls,
        /// <summary>
        /// Controls that control the players movement. Only available when the player not currently inside a dialog.
        /// </summary>
        MovementControls,
        /// <summary>
        /// Controls that control the players actions. Only triggered when the player not currently inside a dialog.
        /// </summary>
        CharacterControls,
        /// <summary>
        /// Shortcuts for inventory actions.
        /// </summary>
        InventoryHotkeys,
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
        /// <summary>
        /// Primary mouse controls
        /// </summary>
        MouseControls
    }
}
