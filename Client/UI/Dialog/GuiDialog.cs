using System;
using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    public abstract class GuiDialog : IDisposable
    {
        /// <summary>
        /// Dialogue Composer for the GUIDialogue.
        /// </summary>
        public class DlgComposers : IEnumerable<KeyValuePair<string, GuiComposer>>
        {
            protected ConcurrentSmallDictionary<string, GuiComposer> dialogComposers = new ();
            protected GuiDialog dialog;

            /// <summary>
            /// Returns all composers as a flat list
            /// </summary>
            public IEnumerable<GuiComposer> Values { get { return dialogComposers.Values; } }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="dialog">The dialogue this composer belongs to.</param>
            public DlgComposers(GuiDialog dialog)
            {
                this.dialog = dialog;
            }

            /// <summary>
            /// Cleans up and clears the composers.
            /// </summary>
            public void ClearComposers()
            {
                foreach (var val in dialogComposers)
                {
                    val.Value?.Dispose();
                }

                dialogComposers.Clear();
            }

            /// <summary>
            /// Clean disposal method.
            /// </summary>
            public void Dispose()
            {
                foreach (var val in dialogComposers)
                {
                    val.Value?.Dispose();
                }
            }

            /// <summary>
            /// Returns the composer for given composer name
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public GuiComposer this[string key]
            {
                get {
                    dialogComposers.TryGetValue(key, out GuiComposer val);
                    return val;
                }
                set {
                    dialogComposers[key] = value;
                    value.OnFocusChanged = dialog.OnFocusChanged;
                }
            }


            IEnumerator IEnumerable.GetEnumerator()
            {
                return dialogComposers.GetEnumerator();
            }

            IEnumerator<KeyValuePair<string, GuiComposer>> IEnumerable<KeyValuePair<string, GuiComposer>>.GetEnumerator()
            {
                return dialogComposers.GetEnumerator();
            }

            /// <summary>
            /// Checks to see if the key is located within the given dialogue composer.
            /// </summary>
            /// <param name="key">The key you are searching for.</param>
            /// <returns>Do we have your key?</returns>
            public bool ContainsKey(string key)
            {
                return dialogComposers.ContainsKey(key);
            }

            /// <summary>
            /// Removes the given key and the corresponding value from the Dialogue Composer.
            /// </summary>
            /// <param name="key">The Key to remove.</param>
            public void Remove(string key)
            {
                dialogComposers.Remove(key);
            }

            public GuiComposer[] ToArray()
            {
                GuiComposer[] result = new GuiComposer[dialogComposers.Count];
                dialogComposers.Values.CopyTo(result, 0);     // 26.12.24 radfast note for the future: if this method ever throws exceptions, consider making dialogComposers a ConcurrentSmallDictionary
                return result;
            }

        }



        /// <summary>
        /// The Instance of Dialogue Composer for this GUIDialogue.
        /// </summary>
        public DlgComposers Composers;

        /// <summary>
        /// A single composer for this GUIDialogue.
        /// </summary>
        public GuiComposer SingleComposer
        {
            get { return Composers["single"]; }
            set { Composers["single"] = value; }
        }

        /// <summary>
        /// Debug name.  For debugging purposes.
        /// </summary>
        public virtual string DebugName
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// The amount of depth required for this dialog. Default is 150. Required for correct z-ordering of dialogs.
        /// </summary>
        public virtual float ZSize => 150;


        // First comes KeyDown event, opens the gui, then comes KeyPress event - this one we have to ignore
        public bool ignoreNextKeyPress = false;


        protected bool opened;
        protected bool focused;

        /// <summary>
        /// Is the dialogue currently in focus?
        /// </summary>
        public virtual bool Focused { get { return focused; } }

        /// <summary>
        /// Can this dialog be focused?
        /// </summary>
        public virtual bool Focusable => true;


        /// <summary>
        /// Is this dialogue a dialogue or a HUD object?
        /// </summary>
        public virtual EnumDialogType DialogType { get { return EnumDialogType.Dialog; } }

        /// <summary>
        /// The event fired when this dialogue is opened.
        /// </summary>
        public event Action OnOpened;

        /// <summary>
        /// The event fired when this dialogue is closed.
        /// </summary>
        public event Action OnClosed;


        protected ICoreClientAPI capi;

        protected virtual void OnFocusChanged(bool on)
        {
            if (on == focused) return;
            if (DialogType == EnumDialogType.Dialog && !opened) return;

            if (on)
            {
                capi.Gui.RequestFocus(this);
            } else
            {
                focused = false;
            }
        }

        /// <summary>
        /// Constructor for the GUIDialogue.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        public GuiDialog(ICoreClientAPI capi)
        {
            Composers = new DlgComposers(this);
            this.capi = capi;
        }

        /// <summary>
        /// Makes this gui pop up once a pre-set given key combination is set.
        /// </summary>
        public virtual void OnBlockTexturesLoaded()
        {
            string keyCombCode = ToggleKeyCombinationCode;
            if (keyCombCode != null)
            {
                capi.Input.SetHotKeyHandler(keyCombCode, OnKeyCombinationToggle);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void OnLevelFinalize()
        {

        }

        public virtual void OnOwnPlayerDataReceived() { }

        /// <summary>
        /// 0 = draw first, 1 = draw last. Used to enforce tooltips and held itemstack always drawn last to be visible.<br/>
        /// Vanilla dialogs draw order:<br/>
        /// Name tags: -0.1<br/>
        /// Chat dialog: 0<br/>
        /// Block Interaction help: 0.05<br/>
        /// Worldmap HUD: 0.07<br/>
        /// Default value for most other dialogs: 0.1<br/>
        /// Worldmap Dialog: 0.11<br/>
        /// Player and Chest inventories: 0.2<br/>
        /// Various config/edit dialogs: 0.2<br/>
        /// Handbook: 0.2<br/>
        /// Escape menu: 0.89
        /// </summary>
        public virtual double DrawOrder { get { return 0.1; } }

        /// <summary>
        /// Determines the order on which dialog receives keyboard input first when the dialog is opened. 0 = handle inputs first, 9999 = handle inputs last.<br/>
        /// Reference list:<br/>
        /// 0: Escape menu<br/>
        /// 0.5 (default): tick profiler, selection box editor, macro editor, survival&amp;creative inventory, first launch info dialog, dead dialog, character dialog, etc.<br/>
        /// 1: hotbar<br/>
        /// 1.1: chat dialog
        /// </summary>
        public virtual double InputOrder { get { return 0.5; } }

        /// <summary>
        /// Should this dialogue de-register itself once it's closed? (Defaults to no)
        /// </summary>
        public virtual bool UnregisterOnClose {  get { return false; } }

        /// <summary>
        /// Fires when the GUI is opened.
        /// </summary>
        public virtual void OnGuiOpened() {

        }

        /// <summary>
        /// Fires when the GUI is closed.
        /// </summary>
        public virtual void OnGuiClosed() {

        }

        /// <summary>
        /// Attempts to open this dialogue.
        /// </summary>
        /// <returns>Was this dialogue successfully opened?</returns>
        public virtual bool TryOpen()
        {
            return TryOpen(true);
        }

        public virtual bool TryOpen(bool withFocus)
        {
            bool wasOpened = opened;

            if (!capi.Gui.LoadedGuis.Contains(this))
            {
                capi.Gui.RegisterDialog(this);
            }

            opened = true;
            if (DialogType == EnumDialogType.Dialog && withFocus)
            {
                capi.Gui.RequestFocus(this);
            }

            if (!wasOpened)
            {
                OnGuiOpened();
                OnOpened?.Invoke();
                capi.Gui.TriggerDialogOpened(this);
            }

            return true;
        }

        /// <summary>
        /// Attempts to close this dialogue- triggering the OnCloseDialogue event.
        /// </summary>
        /// <returns>Was this dialogue successfully closed?</returns>
        public virtual bool TryClose()
        {
            bool wasOpened = opened;
            opened = false;
            UnFocus();
            if (wasOpened)
            {
                OnGuiClosed();
                OnClosed?.Invoke();
            }
            focused = false;
            if (wasOpened) capi.Gui.TriggerDialogClosed(this);

            return true;
        }

        /// <summary>
        /// Unfocuses the dialogue.
        /// </summary>
        public virtual void UnFocus() {
            focused = false;
        }

        /// <summary>
        /// Focuses the dialog
        /// </summary>
        public virtual void Focus() {
            if (!Focusable) return;

            focused = true;
        }

        /// <summary>
        /// If the dialogue is opened, this attempts to close it.  If the dialogue is closed, this attempts to open it.
        /// </summary>
        public virtual void Toggle()
        {
            if (IsOpened())
            {
                TryClose();
            } else
            {
                TryOpen();
            }
        }

        /// <summary>
        /// Is this dialogue opened?
        /// </summary>
        /// <returns>Whether this dialogue is opened or not.</returns>
        public virtual bool IsOpened()
        {
            return opened;
        }

        /// <summary>
        /// Is this dialogue opened in the given context?
        /// </summary>
        /// <param name="dialogComposerName">The composer context.</param>
        /// <returns>Whether this dialogue was opened or not within the given context.</returns>
        public virtual bool IsOpened(string dialogComposerName)
        {
            return IsOpened();
        }

        /// <summary>
        /// This runs before the render.  Local update method.
        /// </summary>
        /// <param name="deltaTime">The time that has elapsed.</param>
        public virtual void OnBeforeRenderFrame3D(float deltaTime)
        {

        }

        public string MouseOverCursor;

        /// <summary>
        /// This runs when the dialogue is ready to render all of the components.
        /// </summary>
        /// <param name="deltaTime">The time that has elapsed.</param>
        public virtual void OnRenderGUI(float deltaTime)
        {
            foreach (var val in Composers)
            {
                val.Value.Render(deltaTime);

                MouseOverCursor = val.Value.MouseOverCursor;
            }
        }

        /// <summary>
        /// This runs when the dialogue is finalizing and cleaning up all of the components.
        /// </summary>
        /// <param name="dt">The time that has elapsed.</param>
        public virtual void OnFinalizeFrame(float dt)
        {
            foreach (var val in Composers)
            {
                val.Value.PostRender(dt);
            }
        }

        internal virtual bool OnKeyCombinationToggle(KeyCombination viaKeyComb)
        {
            HotKey hotkey = capi.Input.GetHotKeyByCode(ToggleKeyCombinationCode);
            if (hotkey == null) return false;

            if (hotkey.KeyCombinationType == HotkeyType.CreativeTool && capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Creative) return false;

            Toggle();

            /*if (!viaKeyComb.Alt && !viaKeyComb.Ctrl && !viaKeyComb.Shift && viaKeyComb.KeyCode > 66)
            {
                ignoreNextKeyPress = true;
            }*/

            return true;
        }

        /// <summary>
        /// Fires when keys are held down.
        /// </summary>
        /// <param name="args">The key or keys that were held down.</param>
        public virtual void OnKeyDown(KeyEvent args)
        {
            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnKeyDown(args, focused);
                if (args.Handled)
                {
                    return;
                }
            }

            HotKey hotkey = capi.Input.GetHotKeyByCode(ToggleKeyCombinationCode);
            if (hotkey == null) return;


            bool toggleKeyPressed = hotkey.DidPress(args, capi.World, capi.World.Player, true);
            if (toggleKeyPressed && TryClose())
            {
                args.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Fires when the keys are pressed.
        /// </summary>
        /// <param name="args">The key or keys that were pressed.</param>
        public virtual void OnKeyPress(KeyEvent args)
        {
            if (ignoreNextKeyPress)
            {
                ignoreNextKeyPress = false;
                args.Handled = true;
                return;
            }

            if (args.Handled) return;

            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnKeyPress(args);
                if (args.Handled) return;
            }

        }

        /// <summary>
        /// Fires when the keys are released.
        /// </summary>
        /// <param name="args">the key or keys that were released.</param>
        public virtual void OnKeyUp(KeyEvent args)
        {
            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnKeyUp(args);
                if (args.Handled)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Fires explicitly when the Escape key is pressed and attempts to close the dialogue.
        /// </summary>
        /// <returns>Whether the dialogue was closed.</returns>
        public virtual bool OnEscapePressed()
        {
            if (DialogType == EnumDialogType.HUD) return false;
            return TryClose();
        }

        /// <summary>
        /// Fires when the mouse enters the given slot.
        /// </summary>
        /// <param name="slot">The slot the mouse entered.</param>
        /// <returns>Whether this event was handled.</returns>
        public virtual bool OnMouseEnterSlot(ItemSlot slot)
        {
            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                if (composer.OnMouseEnterSlot(slot))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fires when the mouse leaves the slot.
        /// </summary>
        /// <param name="itemSlot">The slot the mouse entered.</param>
        /// <returns>Whether this event was handled.</returns>
        public virtual bool OnMouseLeaveSlot(ItemSlot itemSlot)
        {
            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                if (composer.OnMouseLeaveSlot(itemSlot))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fires when the mouse clicks within the slot.
        /// </summary>
        /// <param name="itemSlot">The slot that the mouse clicked in.</param>
        /// <returns>Whether this event was handled.</returns>
        public virtual bool OnMouseClickSlot(ItemSlot itemSlot) { return false; }

        /// <summary>
        /// Fires when a mouse button is held down.
        /// </summary>
        /// <param name="args">The mouse button or buttons in question.</param>
        public virtual void OnMouseDown(MouseEvent args)
        {
            if (args.Handled) return;

            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnMouseDown(args);
                if (args.Handled)
                {
                    return;
                }
            }

            if (!IsOpened()) return;
            foreach (GuiComposer composer in composers)
            {
                if (composer.Bounds.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Fires when a mouse button is released.
        /// </summary>
        /// <param name="args">The mouse button or buttons in question.</param>
        public virtual void OnMouseUp(MouseEvent args)
        {
            if (args.Handled) return;

            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnMouseUp(args);
                if (args.Handled) return;
            }

            foreach (GuiComposer composer in composers)
            {
                if (composer.Bounds.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Fires when the mouse is moved.
        /// </summary>
        /// <param name="args">The mouse movements in question.</param>
        public virtual void OnMouseMove(MouseEvent args)
        {
            if (args.Handled) return;

            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnMouseMove(args);
                if (args.Handled) return;
            }

            foreach (GuiComposer composer in composers)
            {
                if (composer.Bounds.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Fires when the mouse wheel is scrolled.
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnMouseWheel(MouseWheelEventArgs args)
        {
            var composers = Composers.ToArray();    // Minimise risk of co-modification exception while iterating below [Github #4804]
            foreach (GuiComposer composer in composers)
            {
                composer.OnMouseWheel(args);
                if (args.IsHandled) return;
            }

            if (focused)
            {
                foreach (GuiComposer composer in composers)
                {
                    if (composer.Bounds.PointInside(capi.Input.MouseX, capi.Input.MouseY))
                    {
                        args.SetHandled(true);
                    }
                }
            }
        }

        /// <summary>
        /// A check for whether the dialogue should recieve Render events.
        /// </summary>
        /// <returns>Whether the dialogue is opened or not.</returns>
        public virtual bool ShouldReceiveRenderEvents()
        {
            return opened;
        }

        /// <summary>
        /// A check for whether the dialogue should recieve keyboard events.
        /// </summary>
        /// <returns>Whether the dialogue is focused or not.</returns>
        public virtual bool ShouldReceiveKeyboardEvents()
        {
            return focused;
        }

        /// <summary>
        /// A check if the dialogue should recieve mouse events.
        /// </summary>
        /// <returns>Whether the mouse events should fire.</returns>
        public virtual bool ShouldReceiveMouseEvents()
        {
            return IsOpened();
        }

        /// <summary>
        /// Gets whether it is preferred for the mouse to be not grabbed while this dialog is opened.
        /// If true (default), the Alt button needs to be held to manually grab the mouse.
        /// </summary>
        #pragma warning disable 0618
        public virtual bool PrefersUngrabbedMouse =>
            RequiresUngrabbedMouse();
        #pragma warning restore 0618

        [Obsolete("Use PrefersUngrabbedMouse instead")]
        public virtual bool RequiresUngrabbedMouse()
        {
            return true;
        }

        /// <summary>
        /// Gets whether ability to grab the mouse cursor is disabled while
        /// this dialog is opened. For example, the escape menu. (Default: false)
        /// </summary>
        public virtual bool DisableMouseGrab => false;

        // If true and gui element is opened then all keystrokes (except escape) are only received by this gui element
        /// <summary>
        /// Should this dialog (e.g. textbox) capture all the keyboard events except for escape.
        /// </summary>
        /// <returns></returns>
        public virtual bool CaptureAllInputs()
        {
            return false;
        }

        /// <summary>
        /// Should this dialog capture the raw mouse button clicks - useful for example for the settings menu itself (in case the user has unset them or set them to something crazy)
        /// </summary>
        /// <returns></returns>
        public virtual bool CaptureRawMouse()
        {
            return false;
        }

        /// <summary>
        /// Disposes the Dialog.
        /// </summary>
        public virtual void Dispose() {
            Composers?.Dispose();
        }

        /// <summary>
        /// Clears the composers.
        /// </summary>
        public void ClearComposers()
        {
            Composers?.ClearComposers();
        }

        /// <summary>
        /// The key combination string that toggles this GUI object.
        /// </summary>
        public abstract string ToggleKeyCombinationCode { get; }

        /// <summary>
        /// Checks if the player is in range (pickingrange) of the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>In range or no?</returns>
        public virtual bool IsInRangeOf(Vec3d pos)
        {
            var playerEye = capi.World.Player.Entity.Pos.XYZ.Add(capi.World.Player.Entity.LocalEyePos);
            double dist = pos.DistanceTo(playerEye);

            return dist <= capi.World.Player.WorldData.PickingRange + 0.5;
        }







        public EnumPosFlag GetFreePos(string code)
        {
            var values = Enum.GetValues(typeof(EnumPosFlag));

            posFlagDict().TryGetValue(code, out int flags);

            foreach (EnumPosFlag flag in values)
            {
                if ((flags & (int)flag) > 0) continue;

                return flag;
            }

            return 0;
        }

        public void OccupyPos(string code, EnumPosFlag pos)
        {
            posFlagDict().TryGetValue(code, out int flags);
            posFlagDict()[code] = flags | (int)pos;
        }

        public void FreePos(string code, EnumPosFlag pos)
        {
            posFlagDict().TryGetValue(code, out int flags);
            posFlagDict()[code] = flags & ~(int)pos;
        }

        Dictionary<string, int> posFlagDict()
        {
            capi.ObjectCache.TryGetValue("dialogCount", out object valObj);
            Dictionary<string, int> val = valObj as Dictionary<string, int>;
            if (val == null) capi.ObjectCache["dialogCount"] = val = new Dictionary<string, int>();
            return val;
        }

        protected bool IsRight(EnumPosFlag flag)
        {
            return flag == EnumPosFlag.RightBot || flag == EnumPosFlag.RightMid || flag == EnumPosFlag.RightTop || flag == EnumPosFlag.Right2Top || flag == EnumPosFlag.Right2Mid || flag == EnumPosFlag.Right2Bot || flag == EnumPosFlag.Right3Top || flag == EnumPosFlag.Right3Mid || flag == EnumPosFlag.Right3Bot;
        }

        protected float YOffsetMul(EnumPosFlag flag)
        {
            if (flag == EnumPosFlag.RightTop || flag == EnumPosFlag.LeftTop || flag == EnumPosFlag.Right2Top || flag == EnumPosFlag.Left2Top || flag == EnumPosFlag.Right3Top || flag == EnumPosFlag.Left3Top) return -1;
            if (flag == EnumPosFlag.RightBot || flag == EnumPosFlag.LeftBot || flag == EnumPosFlag.Right2Bot || flag == EnumPosFlag.Left2Bot || flag == EnumPosFlag.Right3Bot || flag == EnumPosFlag.Left3Bot) return 1;
            return 0;
        }

        protected float XOffsetMul(EnumPosFlag flag)
        {
            if (flag == EnumPosFlag.Right2Top || flag == EnumPosFlag.Right2Mid || flag == EnumPosFlag.Right2Bot) return -1;
            if (flag == EnumPosFlag.Left2Top || flag == EnumPosFlag.Left2Mid || flag == EnumPosFlag.Left2Bot) return 1;

            if (flag == EnumPosFlag.Right3Top || flag == EnumPosFlag.Right3Mid || flag == EnumPosFlag.Right3Bot) return -2;
            if (flag == EnumPosFlag.Left3Top || flag == EnumPosFlag.Left3Mid || flag == EnumPosFlag.Left3Bot) return 2;
            return 0;
        }


        [Flags]
        public enum EnumPosFlag
        {
            RightMid = 1,
            RightTop = 2,
            RightBot = 4,
            LeftMid = 8,
            LeftTop = 16,
            LeftBot = 32,

            Right2Mid = 64,
            Right2Top = 128,
            Right2Bot = 256,
            Left2Mid = 512,
            Left2Top = 1024,
            Left2Bot = 2048,

            Right3Mid = 2048 * 2,
            Right3Top = 2048 * 4,
            Right3Bot = 2048 * 8,
            Left3Mid = 2048 * 16,
            Left3Top = 2048 * 32,
            Left3Bot = 2048 * 64,
        }
    }
}
