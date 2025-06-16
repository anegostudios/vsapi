using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Client
{
    public class MouseButtonState
    {
        public bool Left;
        public bool Middle;
        public bool Right;

        public void Clear()
        {
            Left = false;
            Middle = false;
            Right = false;
        }
    }

    public delegate void OnHotKeyDelegate(string hotkeycode, KeyCombination keyComb);

    /// <summary>
    /// This interface manages the inputs of the player and is used mostly on the client side.
    /// </summary>
    public interface IInputAPI
    {
        /// <summary>
        /// Get set clipboard text
        /// </summary>
        string ClipboardText { get; set; }

        /// <summary>
        /// Triggered when the player attempts to trigger an action, such as walking forward or sprinting
        /// </summary>
        event OnEntityAction InWorldAction;

        /// <summary>
        /// The current keyboard key states, use the <see cref="GlKeys"/> enum to get the index of an array key.
        /// </summary>
        bool[] KeyboardKeyStateRaw { get; }

        /// <summary>
        /// The current keyboard key states that were not handled by a dialog or other client systems (exception: hotkeys), use the <see cref="GlKeys"/> enum to get the array index of a key.
        /// </summary>
        bool[] KeyboardKeyState { get; }

        /// <summary>
        /// The current mouse button state
        /// </summary>
        MouseButtonState MouseButton { get; }

        /// <summary>
        /// The current mouse button state outside of dialogs / clicked inside the game world
        /// </summary>
        MouseButtonState InWorldMouseButton { get; }

        /// <summary>
        /// The current x-position of the mouse, relative to the upper left corner of the game window
        /// </summary>
        int MouseX { get; }

        /// <summary>
        /// The current y-position of the mouse, relative to the upper left corner of the game window
        /// </summary>
        int MouseY { get; }

        /// <summary>
        /// When controlling the camera, this is the camera yaw determined by the game engine
        /// </summary>
        float MouseYaw { get; set; }
        /// <summary>
        /// When controlling the camera, this is the camera pitch determined by the game engine
        /// </summary>
        float MousePitch { get; set; }

        // These should not be here. Should be part of an event bus event (someone has to code that)
        /// <summary>
        /// Handles the event when the mouse enters the bounding box of the given item slot.
        /// </summary>
        /// <param name="slot">The slot of the item.</param>
        /// <remarks>Part of a group of things that will be moved to an event bus at some point.</remarks>
        void TriggerOnMouseEnterSlot(ItemSlot slot);

        /// <summary>
        /// Handles the event when the mouse leaves the bounding box of the given item slot.
        /// </summary>
        /// <param name="itemSlot">The slot of the item</param>
        /// <remarks>Part of a group of things that will be moved to an event bus at some point.</remarks>
        void TriggerOnMouseLeaveSlot(ItemSlot itemSlot);

        /// <summary>
        /// Handles the event when the mouse clicks on a given item slot.
        /// </summary>
        /// <param name="itemSlot">The slot of the item</param>
        /// <remarks>Part of a group of things that will be moved to an event bus at some point.</remarks>
        void TriggerOnMouseClickSlot(ItemSlot itemSlot);



        /// <summary>
        /// Gives the player the ability to still interact with the world even if a gui dialog is opened
        /// </summary>
        bool MouseWorldInteractAnyway { get; set; }

        /// <summary>
        /// True if the mouse cursor is currently grabbed and not visible.
        /// (Such as while controlling the character's view in first person.)
        /// </summary>
        bool MouseGrabbed { get; }

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
        /// Same as RegisterHotKey except it inserts it at the start of the list
        /// </summary>
        /// <param name="hotkeyCode"></param>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="altPressed"></param>
        /// <param name="ctrlPressed"></param>
        /// <param name="shiftPressed"></param>
        void RegisterHotKeyFirst(string hotkeyCode, string name, GlKeys key, HotkeyType type = HotkeyType.CharacterControls, bool altPressed = false, bool ctrlPressed = false, bool shiftPressed = false);


        /// <summary>
        /// Will call the handler if given hotkey has been pressed. Removes the previously assigned handler.
        /// </summary>
        /// <param name="hotkeyCode"></param>
        /// <param name="handler"></param>
        void SetHotKeyHandler(string hotkeyCode, ActionConsumable<KeyCombination> handler);

        void AddHotkeyListener(OnHotKeyDelegate handler);

        /// <summary>
        /// Returns a list of all currently registered hotkeys.
        /// </summary>
        OrderedDictionary<string, HotKey> HotKeys { get; }

        /// <summary>
        /// Gets the hotkey by the given hotkey code.
        /// </summary>
        /// <param name="toggleKeyCombinationCode">the key combination code.</param>
        /// <returns>The registered hotkey.</returns>
        HotKey GetHotKeyByCode(string toggleKeyCombinationCode);

        public bool IsHotKeyPressed(string hotKeyCode);

        public bool IsHotKeyPressed(HotKey hotKey);
    }
}
