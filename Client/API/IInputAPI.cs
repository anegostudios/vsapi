using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Client
{
    public class MouseState
    {
        public bool Left;
        public bool Middle;
        public bool Right;
    }

    public interface IInputAPI
    {
        /// <summary>
        /// The current keyboard key states
        /// </summary>
        bool[] KeyboardKeyStateRaw { get; }

        /// <summary>
        /// The current keyboard key states that were not handled by a dialog or other client systems (exception: hotkeys)
        /// </summary>
        bool[] KeyboardKeyState { get; }

        /// <summary>
        /// The current mouse button state
        /// </summary>
        MouseState Mouse { get; }
        
        /// <summary>
        /// The current mouse button state outside of dialogs / clicked inside the game world
        /// </summary>
        MouseState InWorldMouse { get; }

        int GetMouseCurrentX();
        int GetMouseCurrentY();


        // These should not be here. Should be part of an event bus event
        void TriggerOnMouseEnterSlot(IItemSlot slot);
        void TriggerOnMouseLeaveSlot(IItemSlot itemSlot);
        void TriggerOnMouseClickSlot(IItemSlot itemSlot);



        /// <summary>
        /// Gives the player the ability to still interact with the world even if a gui dialog is opened
        /// </summary>
        bool MouseWorldInteractAnyway { get; set; }

        /// <summary>
        /// True if the player currently has a visible mouse cursor (i.e. is not locked to look around ingame)
        /// </summary>
        bool MouseCursorVisible { get; }

        /// <summary>
        /// Registers a hot key with given default key combination, the player will be able change these in the controls. Supplied hotkeyCode can than be used to register a hotkey handler.
        /// </summary>
        /// <param name="hotkeyCode"></param>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="altPressed"></param>
        /// <param name="ctrlPressed"></param>
        /// <param name="shiftPressed"></param>
        void RegisterHotKey(string hotkeyCode, string name, GlKeys key, HotkeyType type = HotkeyType.CharacterControls, bool altPressed = false, bool ctrlPressed = false, bool shiftPressed = false);


        
        /// <summary>
        /// Will call the handler if given hotkey has been pressed. Removes the previously assigned handler.
        /// </summary>
        /// <param name="hotkeyCode"></param>
        /// <param name="handler"></param>
        void SetHotKeyHandler(string hotkeyCode, ActionConsumable<KeyCombination> handler);

        /// <summary>
        /// Returns a list of all currently registered hotkeys.
        /// </summary>
        OrderedDictionary<string, HotKey> HotKeys { get; }

        HotKey GetHotKeyByCode(string toggleKeyCombinationCode);
    }
}
