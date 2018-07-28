using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

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

        public string Code;
        public string Name;

        public HotkeyType KeyCombinationType = HotkeyType.CharacterControls;

        public ActionConsumable<KeyCombination> Handler;

        public virtual bool DidPress(KeyEvent keyEventargs, IWorldAccessor world, IPlayer player, bool allowCharacterControls)
        {
            return
                keyEventargs.KeyCode == CurrentMapping.KeyCode &&
                keyEventargs.AltPressed == CurrentMapping.Alt &&
                keyEventargs.CtrlPressed == CurrentMapping.Ctrl &&
                keyEventargs.ShiftPressed == CurrentMapping.Shift &&
                (KeyCombinationType != HotkeyType.CharacterControls || allowCharacterControls) &&
                (keyEventargs.KeyCode2 == CurrentMapping.SecondKeyCode || CurrentMapping.SecondKeyCode == null || CurrentMapping.SecondKeyCode == 0)
            ;
        }

        public virtual bool FallbackDidPress(KeyEvent keyEventargs, IWorldAccessor world, IPlayer player, bool allowCharacterControls)
        {
            bool haveModifier = CurrentMapping.Alt || CurrentMapping.Ctrl || CurrentMapping.Shift;

            return
                !haveModifier &&
                keyEventargs.KeyCode == CurrentMapping.KeyCode &&
                (keyEventargs.KeyCode2 == CurrentMapping.SecondKeyCode || CurrentMapping.SecondKeyCode == null || CurrentMapping.SecondKeyCode == 0) &&
                (KeyCombinationType != HotkeyType.CharacterControls || allowCharacterControls)
            ;
        }



        public HotKey Clone()
        {
            HotKey hk = (HotKey)MemberwiseClone();
            hk.CurrentMapping = CurrentMapping.Clone();
            hk.DefaultMapping = DefaultMapping.Clone();
            return hk;
        }

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
