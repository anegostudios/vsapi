using System;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    public class HotKey
    {
        /// <summary>
        /// For global hotkeys that shall not be blocked by anything (e.g. F11 for fullscreen, F12 for screenshot)
        /// </summary>
        public bool IsGlobalHotkey;

        /// <summary>
        /// For hotkeys that only available during a game session. When the game session ends, the handler to this hotkey is removed (or we have a dead reference to runninggame)
        /// </summary>
        public bool IsIngameHotkey;

        /// <summary>
        /// The current key combination for this hotkey
        /// </summary>
        public KeyCombination CurrentMapping;

        /// <summary>
        /// The default key combination for this hotkey
        /// </summary>
        public KeyCombination DefaultMapping;

        /// <summary>
        /// The code of the Hotkey.
        /// </summary>
        public string Code;

        /// <summary>
        /// The name of the Hotkey
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of the key combination.  This defaults to HotkeyType.CharacterControls.
        /// </summary>
        public HotkeyType KeyCombinationType = HotkeyType.CharacterControls;

        /// <summary>
        /// This is the action that happens when the hotkey is used.
        /// </summary>
        public ActionConsumable<KeyCombination> Handler;

        /// <summary>
        /// If true, the handler will be called twice, once on the key or button down event, and once on the up event
        /// </summary>
        public bool TriggerOnUpAlso;

        /// <summary>
        /// Was this hotkey pressed?
        /// </summary>
        /// <param name="keyEventargs">Event arguments for the given key.</param>
        /// <param name="world">The current world for the key.</param>
        /// <param name="player">The player that pressed the buttons.</param>
        /// <param name="allowCharacterControls">Do we allow character control functions.</param>
        /// <returns>If the hotkey was pressed or not.</returns>
        public virtual bool DidPress(KeyEvent keyEventargs, IWorldAccessor world, IPlayer player, bool allowCharacterControls)
        {
            return
                keyEventargs.KeyCode == CurrentMapping.KeyCode &&
                (MouseControlsIgnoreModifiers() || (keyEventargs.AltPressed == CurrentMapping.Alt && keyEventargs.CtrlPressed == CurrentMapping.Ctrl && keyEventargs.ShiftPressed == CurrentMapping.Shift)) &&
                (KeyCombinationType != HotkeyType.CharacterControls && KeyCombinationType != HotkeyType.MovementControls || allowCharacterControls) &&
                (keyEventargs.KeyCode2 == CurrentMapping.SecondKeyCode || CurrentMapping.SecondKeyCode == null || CurrentMapping.SecondKeyCode == 0)
            ;
        }

        /// <summary>
        /// If this hotkey is a mouse control (primary or secondary mouse click), and this hotkey does not specifically require modifiers, then let's ignore modifiers when matching, because Shift+click or Ctrl+click is common; Alt+click might be used if Alt is being held
        /// </summary>
        /// <returns></returns>
        private bool MouseControlsIgnoreModifiers()
        {
            if (CurrentMapping.IsMouseButton(CurrentMapping.KeyCode))
            {
                bool haveModifier = CurrentMapping.Alt || CurrentMapping.Ctrl || CurrentMapping.Shift;
                return !haveModifier;
            }
            return false;
        }

        /// <summary>
        /// Fallback version of the DidPress event.
        /// </summary>
        /// <param name="keyEventargs">Event arguments for the given key.</param>
        /// <param name="world">The current world for the key.</param>
        /// <param name="player">The player that pressed the buttons.</param>
        /// <param name="allowCharacterControls">Do we allow character control functions.</param>
        /// <returns>If the hotkey was pressed or not.</returns>
        public virtual bool FallbackDidPress(KeyEvent keyEventargs, IWorldAccessor world, IPlayer player, bool allowCharacterControls)
        {
            bool haveModifier = CurrentMapping.Alt || CurrentMapping.Ctrl || CurrentMapping.Shift;

            return
                !haveModifier &&
                keyEventargs.KeyCode == CurrentMapping.KeyCode &&
                (keyEventargs.KeyCode2 == CurrentMapping.SecondKeyCode || CurrentMapping.SecondKeyCode == null || CurrentMapping.SecondKeyCode == 0) &&
                (KeyCombinationType != HotkeyType.CharacterControls && KeyCombinationType != HotkeyType.MovementControls || allowCharacterControls)
            ;
        }


        /// <summary>
        /// Clones the hotkey.
        /// </summary>
        /// <returns>the cloned hotkey.</returns>
        public HotKey Clone()
        {
            HotKey hk = (HotKey)MemberwiseClone();
            hk.CurrentMapping = CurrentMapping.Clone();
            hk.DefaultMapping = DefaultMapping.Clone();
            return hk;
        }

        /// <summary>
        /// Sets the default keymap for this hotkey.
        /// </summary>
        public void SetDefaultMapping()
        {
            DefaultMapping = new KeyCombination()
            {
                KeyCode = CurrentMapping.KeyCode,
                SecondKeyCode = CurrentMapping.SecondKeyCode,
                Alt = CurrentMapping.Alt,
                Ctrl = CurrentMapping.Ctrl,
                Shift = CurrentMapping.Shift
            };
        }
    }
}
