using System;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiDialogConfirm : GuiDialog
    {
        string text;
        Action<bool> DidPressButton;

        public override double DrawOrder => 2;

        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }


        public GuiDialogConfirm(ICoreClientAPI capi, string text, Action<bool> DidPressButton) : base(capi)
        {
            this.text = text;
            this.DidPressButton = DidPressButton;

            Compose();
        }

        private void Compose()
        {
            ElementBounds textBounds = ElementStdBounds.Rowed(0.4f, 0, EnumDialogArea.LeftFixed).WithFixedWidth(500);
            ElementBounds bgBounds = ElementStdBounds.DialogBackground().WithFixedPadding(GuiStyle.ElementToDialogPadding, GuiStyle.ElementToDialogPadding);
            TextDrawUtil util = new TextDrawUtil();
            CairoFont font = CairoFont.WhiteSmallText();

            float y = (float)util.GetMultilineTextHeight(font, text, textBounds.fixedWidth);

            SingleComposer =
                capi.Gui
                .CreateCompo("confirmdialog", ElementStdBounds.AutosizedMainDialog)
                .AddShadedDialogBG(bgBounds, true)
                .AddDialogTitleBar(Lang.Get("Please Confirm"), OnTitleBarClose)
                .BeginChildElements(bgBounds)
                    .AddStaticText(text, font, textBounds)

                    .AddSmallButton(Lang.Get("Cancel"), () => { DidPressButton(false); TryClose(); return true; }, ElementStdBounds.MenuButton((y + 80) / 80f).WithAlignment(EnumDialogArea.LeftFixed).WithFixedPadding(6), EnumButtonStyle.Normal)
                    .AddSmallButton(Lang.Get("Confirm"), () => { DidPressButton(true); TryClose(); return true; }, ElementStdBounds.MenuButton((y + 80) / 80f).WithAlignment(EnumDialogArea.RightFixed).WithFixedPadding(6), EnumButtonStyle.Normal)
                .EndChildElements()
                .Compose()
            ;
        }

        private void OnTitleBarClose()
        {
            TryClose();
        }

        public override void OnGuiOpened()
        {
            Compose();
            base.OnGuiOpened();
        }
        
    }
}