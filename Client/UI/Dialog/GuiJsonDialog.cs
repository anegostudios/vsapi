using Cairo;
using System;
using System.Linq;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// This is a dialogue built from JSON files.
    /// </summary>
    /// <remarks>
    /// JSON made this gui.  Thanks JSON.
    /// </remarks>
    public class GuiJsonDialog : GuiDialogGeneric
    {
        JsonDialogSettings settings;

        /// <summary>
        /// The debug name of the GUI
        /// </summary>
        public override string DebugName
        {
            get { return "jsondialog-" + settings.Code; }
        }

        /// <summary>
        /// Key Combination for the GUI
        /// </summary>
        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }

        /// <summary>
        /// Builds the dialog using the dialog settings from JSON.
        /// </summary>
        /// <param name="settings">The dialog settings.</param>
        /// <param name="capi">The Client API</param>
        public GuiJsonDialog(JsonDialogSettings settings, ICoreClientAPI capi) : base("", capi)
        {
            this.settings = settings;
            ComposeDialog(true);
        }

        /// <summary>
        /// Builds the dialog using the dialog settings from JSON.
        /// </summary>
        /// <param name="settings">The dialog settings.</param>
        /// <param name="capi">The Client API</param>
        /// <param name="focusFirstElement">Should the first element be focused, after building the dialog?</param>
        public GuiJsonDialog(JsonDialogSettings settings, ICoreClientAPI capi, bool focusFirstElement) : base("", capi)
        {
            this.settings = settings;
            ComposeDialog(focusFirstElement);
        }

        /// <summary>
        /// Recomposes the GUI.
        /// </summary>
        public override void Recompose()
        {
            ComposeDialog(false);
        }

        public override bool PrefersUngrabbedMouse => settings.DisableWorldInteract;

        /// <summary>
        /// Composes the dialogue with specifications dictated by JSON.
        /// </summary>
        public void ComposeDialog(bool focusFirstElement = false)
        {
            double factor = settings.SizeMultiplier;

            ElementBounds dlgBounds = ElementStdBounds.AutosizedMainDialog
                .WithAlignment(settings.Alignment)
                .WithFixedPadding(10)
                .WithScale(factor)
                .WithFixedPosition(settings.PosX, settings.PosY)
            ;

            GuiComposer composer =
                capi.Gui
               .CreateCompo("cmdDlg" + settings.Code, dlgBounds)
               .AddDialogBG(ElementStdBounds.DialogBackground().WithScale(factor).WithFixedPadding(settings.Padding), false)
               .BeginChildElements()
            ;

            double y = 0;
            int elemKey = 1;

            for (int i = 0; i < settings.Rows.Length; i++)
            {
                DialogRow row = settings.Rows[i];

                y += row.TopPadding;

                double maxheight = 0;
                double x = 0;
                for (int j = 0; j < row.Elements.Length; j++)
                {
                    DialogElement elem = row.Elements[j];
                    maxheight = Math.Max(elem.Height, maxheight);

                    x += elem.PaddingLeft;

                    ComposeElement(composer, settings, elem, elemKey, x, y);

                    elemKey++;

                    x += elem.Width + 20;
                }

                y += maxheight + row.BottomPadding;
            }


            Composers["cmdDlg" + settings.Code] = composer.EndChildElements().Compose(focusFirstElement);
        }

        private void ComposeElement(GuiComposer composer, JsonDialogSettings settings, DialogElement elem, int elemKey, double x, double y)
        {
            double factor = settings.SizeMultiplier;

            double labelWidth = 0;
            if (elem.Label != null)
            {
                CairoFont font = CairoFont.WhiteSmallText();
                font.UnscaledFontsize *= factor;
                var label = Lang.Get(elem.Label);
                TextExtents extents = font.GetTextExtents(label);
                labelWidth = extents.Width / factor / RuntimeEnv.GUIScale + 1;
                FontExtents fext = font.GetFontExtents();

                ElementBounds labelBounds = ElementBounds.Fixed(x, y + Math.Max(0, (elem.Height * factor - fext.Height / RuntimeEnv.GUIScale) / 2), labelWidth, elem.Height).WithScale(factor);

                composer.AddStaticText(label, font, labelBounds);
                labelWidth += 8;

                if (elem.Tooltip != null)
                {
                    CairoFont tfont = CairoFont.WhiteSmallText();
                    tfont.UnscaledFontsize *= factor;
                    composer.AddHoverText(Lang.Get(elem.Tooltip), tfont, 350, labelBounds.FlatCopy(), "tooltip-" + elem.Code);
                    composer.GetHoverText("tooltip-" + elem.Code).SetAutoWidth(true);
                }
            }


            ElementBounds bounds = ElementBounds.Fixed(x + labelWidth, y, elem.Width - labelWidth, elem.Height).WithScale(factor);

            string currentValue = settings.OnGet?.Invoke(elem.Code);

            switch (elem.Type)
            {
                case EnumDialogElementType.Slider:
                    {
                        string key = "slider-" + elemKey;
                        composer.AddSlider((newval) => { settings.OnSet?.Invoke(elem.Code, newval +""); return true; }, bounds, key);

                        int.TryParse(currentValue, out int curVal);

                        composer.GetSlider(key).SetValues(curVal, elem.MinValue, elem.MaxValue, elem.Step);
                        composer.GetSlider(key).Scale = factor;
                        //composer.GetSlider(key).TriggerOnlyOnMouseUp(true);
                        break;
                    }

                case EnumDialogElementType.Switch:
                    {
                        string key = "switch-" + elemKey;
                        composer.AddSwitch((newval) => { settings.OnSet?.Invoke(elem.Code, newval ? "1" : "0"); }, bounds, key, 30 * factor, 5 * factor);
                        composer.GetSwitch(key).SetValue(currentValue == "1");
                    }
                    break;

                case EnumDialogElementType.Input:
                    {
                        string key = "input-" + elemKey;
                        CairoFont font = CairoFont.WhiteSmallText();
                        font.UnscaledFontsize *= factor;

                        composer.AddTextInput(bounds, (newval) => { settings.OnSet?.Invoke(elem.Code, newval); }, font, key);
                        composer.GetTextInput(key).SetValue(currentValue);
                        break;
                    }

                case EnumDialogElementType.NumberInput:
                    {
                        string key = "numberinput-" + elemKey;
                        CairoFont font = CairoFont.WhiteSmallText();
                        font.UnscaledFontsize *= factor;

                        composer.AddNumberInput(bounds, (newval) => { settings.OnSet?.Invoke(elem.Code, newval); }, font, key);
                        composer.GetNumberInput(key).SetValue(currentValue);
                        break;
                    }

                case EnumDialogElementType.Button:
                    if (elem.Icon != null)
                    {
                        composer.AddIconButton(elem.Icon, (val) => { settings.OnSet?.Invoke(elem.Code, null); }, bounds);
                    } else
                    {
                        CairoFont font = CairoFont.ButtonText();
                        font.WithFontSize(elem.FontSize);

                        composer.AddButton(Lang.Get(elem.Text), () => { settings.OnSet?.Invoke(elem.Code, null); return true; }, bounds.WithFixedPadding(8, 0), font);
                    }

                    if (elem.Tooltip != null && elem.Label == null)
                    {
                        CairoFont tfont = CairoFont.WhiteSmallText();
                        tfont.UnscaledFontsize *= factor;
                        composer.AddHoverText(Lang.Get(elem.Tooltip), tfont, 350, bounds.FlatCopy(), "tooltip-"+elem.Code);
                        composer.GetHoverText("tooltip-" + elem.Code).SetAutoWidth(true);
                    }
                    break;

                case EnumDialogElementType.Text:
                    composer.AddStaticText(Lang.Get(elem.Text), CairoFont.WhiteMediumText().WithFontSize(elem.FontSize), bounds);
                    break;

                case EnumDialogElementType.Select:
                case EnumDialogElementType.DynamicSelect:
                    {
                        string[] values = elem.Values;
                        string[] names = elem.Names;

                        if (elem.Type == EnumDialogElementType.DynamicSelect)
                        {
                            string[] compos = currentValue.Split(new string[] { "\n" }, StringSplitOptions.None);
                            values = compos[0].Split(new string[] { "||" }, StringSplitOptions.None);
                            names = compos[1].Split(new string[] { "||" }, StringSplitOptions.None);
                            currentValue = compos[2];
                        }

                        names = names.Select(s => Lang.Get(s)).ToArray();
                        int selectedIndex = Array.FindIndex(values, w => w.Equals(currentValue));

                        if (elem.Mode == EnumDialogElementMode.DropDown)
                        {
                            string key = "dropdown-" + elemKey;

                            composer.AddDropDown(values, names, selectedIndex, (newval, on) => { settings.OnSet?.Invoke(elem.Code, newval); }, bounds, key);

                            composer.GetDropDown(key).Scale = factor;

                            composer.GetDropDown(key).Font.UnscaledFontsize *= factor;
                        }
                        else
                        {
                            if (elem.Icons != null && elem.Icons.Length > 0)
                            {
                                ElementBounds[] manybounds = new ElementBounds[elem.Icons.Length];
                                double elemHeight = (elem.Height - 4 * elem.Icons.Length) / elem.Icons.Length;

                                for (int i = 0; i < manybounds.Length; i++)
                                {
                                    manybounds[i] = bounds.FlatCopy().WithFixedHeight(elemHeight - 4).WithFixedOffset(0, i * (4 + elemHeight)).WithScale(factor);
                                }

                                string key = "togglebuttons-" + elemKey;
                                CairoFont font = CairoFont.WhiteSmallText();
                                font.UnscaledFontsize *= factor;

                                composer.AddIconToggleButtons(elem.Icons, font, (newval) => { settings.OnSet?.Invoke(elem.Code, elem.Values[newval]); }, manybounds, key);

                                if (currentValue != null && currentValue.Length > 0)
                                {
                                    composer.ToggleButtonsSetValue(key, selectedIndex);
                                }

                                if (elem.Tooltips != null)
                                {
                                    for (int i = 0; i < elem.Tooltips.Length; i++)
                                    {
                                        CairoFont tfont = CairoFont.WhiteSmallText();
                                        tfont.UnscaledFontsize *= factor;

                                        composer.AddHoverText(Lang.Get(elem.Tooltips[i]), tfont, 350, manybounds[i].FlatCopy());
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Fires an event when the mouse is held down.
        /// </summary>
        /// <param name="args">The mouse events.</param>
        public override void OnMouseDown(MouseEvent args)
        {
            base.OnMouseDown(args);

            foreach(GuiComposer composer in Composers.Values)
            {
                if (composer.Bounds.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Reloads the values in the GUI.
        /// </summary>
        public void ReloadValues()
        {
            GuiComposer composer = Composers["cmdDlg" + settings.Code];
            int elemKey = 1;

            for (int i = 0; i < settings.Rows.Length; i++)
            {
                DialogRow row = settings.Rows[i];

                for (int j = 0; j < row.Elements.Length; j++)
                {
                    DialogElement elem = row.Elements[j];

                    string currentValue = settings.OnGet?.Invoke(elem.Code);

                    switch (elem.Type)
                    {
                        case EnumDialogElementType.Slider:
                            {
                                string key = "slider-" + elemKey;
                                int.TryParse(currentValue, out int curVal);
                                composer.GetSlider(key).SetValues(curVal, elem.MinValue, elem.MaxValue, elem.Step);
                                break;
                            }
                        case EnumDialogElementType.Switch:
                            {
                                string key = "switch-" + elemKey;
                                composer.GetSwitch(key).SetValue(currentValue == "1");
                            }
                            break;
                        case EnumDialogElementType.Input:
                            {
                                string key = "input-" + elemKey;
                                composer.GetTextInput(key).SetValue(currentValue);
                                break;
                            }
                        case EnumDialogElementType.NumberInput:
                            {
                                string key = "numberinput-" + elemKey;
                                composer.GetNumberInput(key).SetValue(currentValue);
                                break;
                            }
                        case EnumDialogElementType.Button:
                            break;
                        case EnumDialogElementType.Text:
                            break;
                        case EnumDialogElementType.Select:
                        case EnumDialogElementType.DynamicSelect:
                            {
                                string[] values = elem.Values;

                                if (elem.Type == EnumDialogElementType.DynamicSelect)
                                {
                                    string[] compos = currentValue.Split(new string[] { "\n" }, StringSplitOptions.None);
                                    values = compos[0].Split(new string[] { "||" }, StringSplitOptions.None);
                                    string[] names = compos[1].Split(new string[] { "||" }, StringSplitOptions.None);
                                    currentValue = compos[2];
                                    string key = "dropdown-" + elemKey;
                                    composer.GetDropDown(key).SetList(values, names);
                                }

                                int selectedIndex = Array.FindIndex(values, w => w.Equals(currentValue));

                                if (elem.Mode == EnumDialogElementMode.DropDown)
                                {
                                    string key = "dropdown-" + elemKey;
                                    composer.GetDropDown(key).SetSelectedIndex(selectedIndex);
                                }
                                else
                                {
                                    if (elem.Icons != null && elem.Icons.Length > 0)
                                    {
                                        string key = "togglebuttons-" + elemKey;

                                        if (currentValue != null && currentValue.Length > 0)
                                        {
                                            composer.ToggleButtonsSetValue(key, selectedIndex);
                                        }
                                    }

                                }
                            }
                            break;
                    }

                    elemKey++;
                }
            }
        }
    }
}
