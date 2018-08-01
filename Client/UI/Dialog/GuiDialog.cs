using System;
using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public abstract class GuiDialog
    {
        public class DlgComposers : IEnumerable<KeyValuePair<string, GuiComposer>>
        {
            protected Dictionary<string, GuiComposer> dialogComposers = new Dictionary<string, GuiComposer>();
            protected GuiDialog dialog;
            

            public IEnumerable<GuiComposer> Values { get { return dialogComposers.Values; } }

            public DlgComposers(GuiDialog dialog)
            {
                this.dialog = dialog;
            }

            public GuiComposer this[string key]
            {
                get {
                    GuiComposer val = null;
                    dialogComposers.TryGetValue(key, out val);
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

            public bool ContainsKey(string key)
            {
                return dialogComposers.ContainsKey(key);
            }

            public void Remove(string key)
            {
                dialogComposers.Remove(key);
            }
        }




        public DlgComposers DialogComposers;

        public GuiComposer SingleComposer
        {
            get { return DialogComposers["single"]; }
            set { DialogComposers["single"] = value; }
        }

        public virtual string DebugName
        {
            get { return GetType().Name; }
        }
        
        // First comes KeyDown event, opens the gui, then comes KeyPress event - this one we have to ignore
        protected bool ignoreNextKeyPress = false;


        protected bool opened;
        protected bool focused;

        public virtual bool Focused { get { return focused; } }
        public virtual EnumDialogType DialogType { get { return EnumDialogType.Dialog; } }

        public event Common.Action OnOpened;
        public event Common.Action OnClosed;


        protected ICoreClientAPI capi;

        protected virtual void OnFocusChanged(bool on)
        {
            if (on == focused) return;

            if (on)
            {
                capi.Gui.RequestFocus(this);
            } else
            {
                focused = false;
            }
        }

        public GuiDialog(ICoreClientAPI capi)
        {
            DialogComposers = new DlgComposers(this);
            this.capi = capi;
        }

        public virtual void OnBlockTexturesLoaded()
        {
            string keyCombCode = ToggleKeyCombinationCode;
            if (keyCombCode != null)
            {
                capi.Input.SetHotKeyHandler(keyCombCode, OnKeyCombinationToggle);
            }
        }

        public virtual void OnLevelFinalize()
        {

        }

        public virtual void OnOwnPlayerDataReceived() { }

        /// <summary>
        /// 0 = draw first, 1 = draw last. Used to enforce tooltips and held itemstack always drawn last to be visible.
        /// </summary>
        public virtual double DrawOrder { get { return 0.1; } }

        /// <summary>
        /// 0 = handle inputs first, 1 = handle inputs last.
        /// </summary>
        public virtual double InputOrder { get { return 0.5; } }

        public virtual bool UnregisterOnClose {  get { return false; } }

        public virtual void OnGuiOpened() {
            
        }

        public virtual void OnGuiClosed() {
            
        }

        public virtual bool TryOpen()
        {
            bool wasOpened = opened;

            if (!capi.Gui.LoadedGuis.Contains(this))
            {
                capi.Gui.RegisterDialog(this);
            }

            opened = true;
            if (DialogType == EnumDialogType.Dialog)
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

        public virtual bool TryClose()
        {
            opened = false;
            UnFocus();
            OnGuiClosed();
            OnClosed?.Invoke();
            focused = false;
            capi.Gui.TriggerDialogClosed(this);

            return true;
        }

        public virtual void UnFocus() {
            focused = false;
        }

        public virtual void Focus() {
            focused = true;
        }

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

        public virtual bool IsOpened()
        {
            return opened;
        }

        public virtual bool IsOpened(string dialogComposerName)
        {
            return IsOpened();
        }

        public virtual void OnBeforeRenderFrame3D(float deltaTime)
        {
            
        }

        public virtual void OnRender2D(float deltaTime)
        {
            foreach (var val in DialogComposers)
            {
                val.Value.Render(deltaTime);
            }
        }

        public virtual void OnFinalizeFrame(float dt)
        {
            foreach (var val in DialogComposers)
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


        public virtual void OnKeyDown(KeyEvent args)
        {
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.OnKeyDown(capi, args, focused);
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

        public virtual void OnKeyPress(KeyEvent args)
        {
            if (ignoreNextKeyPress)
            {
                ignoreNextKeyPress = false;
                args.Handled = true;
                return;
            }

            if (args.Handled) return;

            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.OnKeyPress(capi, args);
                if (args.Handled) return;
            }
            
        }

        public virtual void OnKeyUp(KeyEvent args) { }


        public virtual bool OnEscapePressed()
        {
            if (DialogType == EnumDialogType.HUD) return false;
            return TryClose();
        }

        public virtual bool OnMouseEnterSlot(ItemSlot slot) { return false; }
        public virtual bool OnMouseLeaveSlot(ItemSlot itemSlot) { return false; }
        public virtual bool OnMouseClickSlot(ItemSlot itemSlot) { return false; }

        public virtual void OnMouseDown(MouseEvent args)
        {
            if (args.Handled) return;

            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.OnMouseDown(capi, args);
                if (args.Handled)
                {
                    return;
                }
            }

            if (!args.Handled)
            {
                foreach (GuiComposer composer in DialogComposers.Values)
                {
                    if (composer.Bounds.PointInside(args.X, args.Y))
                    {
                        args.Handled = true;
                    }
                }
            }
            
        }

        public virtual void OnMouseUp(MouseEvent args)
        {
            if (args.Handled) return;

            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.OnMouseUp(capi, args);
                if (args.Handled) return;
            }

            foreach (GuiComposer composer in DialogComposers.Values)
            {
                if (composer.Bounds.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                }
            }
        }

        public virtual void OnMouseMove(MouseEvent args)
        {
            if (args.Handled) return;

            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.OnMouseMove(capi, args);
                if (args.Handled) return;
            }
            
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                if (composer.Bounds.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                    break;
                }
            }
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs args)
        {
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.OnMouseWheel(capi, args);
                if (args.IsHandled) return;
            }

            if (focused)
            {
                foreach (GuiComposer composer in DialogComposers.Values)
                {
                    if (composer.Bounds.PointInside(capi.Input.MouseX, capi.Input.MouseY))
                    {
                        args.SetHandled(true);
                    }
                }
            }
        }


        public virtual bool ShouldReceiveRenderEvents()
        {
            return opened;
        }

        public virtual bool ShouldReceiveKeyboardEvents()
        {
            return focused;
        }

        public virtual bool ShouldReceiveMouseEvents()
        {
            return IsOpened();
        }

        public virtual bool DisableWorldInteract()
        {
            return true;
        }

        // If true and gui element is opened then all keystrokes (except escape) are only received by this gui element
        public virtual bool CaptureAllInputs()
        {
            return false;
        }


        public virtual void Dispose() { }


        public abstract string ToggleKeyCombinationCode { get; }
    }
}
