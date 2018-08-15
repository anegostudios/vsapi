using System;
using Cairo;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Action = Vintagestory.API.Common.Action;

namespace Vintagestory.API.Client
{
    public class GuiElementDialogTitleBar : GuiElementTextBase
    {
        GuiElementListMenu listMenu;

        Action OnClose;
        internal GuiComposer baseComposer;


        public static int unscaledCloseIconSize = 15;

        LoadedTexture closeIconHoverTexture;
        LoadedTexture menuIconHoverTexture;
        Rectangled closeIconRect;
        Rectangled menuIconRect;

        bool didInit = false;
        public bool drawBg = false;
        bool movable = false;
        bool moving = false;

        Vec2i movingStartPos = new Vec2i();
        ElementBounds parentBoundsBefore = null;


        public GuiElementDialogTitleBar(ICoreClientAPI capi, string text, GuiComposer composer, Action OnClose = null, CairoFont font = null, ElementBounds bounds = null) : base(capi, text, font, bounds)
        {
            closeIconHoverTexture = new LoadedTexture(capi);
            menuIconHoverTexture = new LoadedTexture(capi);

            if (bounds == null) this.Bounds = ElementStdBounds.TitleBar();
            if (font == null) this.Font = CairoFont.SmallDialogText();
            this.OnClose = OnClose;

            ElementBounds dropDownBounds = ElementBounds.Fixed(0, 0, 100, 25);
            this.Bounds.WithChild(dropDownBounds);

            listMenu = new GuiElementListMenu(capi, new string[] { "auto", "manual" }, new string[] { "Fixed", "Movable" }, 0, onSelectionChanged, dropDownBounds)
            {
                hoveredIndex = 0
            };

            baseComposer = composer;
        }

        private void onSelectionChanged(string val)
        {
            SetUpMovableState(val);
        }

        private void SetUpMovableState(string val)
        {
            if (val == null)
            {
                Vec2i pos = api.Gui.GetDialogPosition(baseComposer.dialogName);
                if (pos != null)
                {
                    movable = true;
                    parentBoundsBefore = Bounds.ParentBounds.FlatCopy();
                    Bounds.ParentBounds.Alignment = EnumDialogArea.None;
                    Bounds.ParentBounds.fixedX = pos.X;
                    Bounds.ParentBounds.fixedY = pos.Y;
                    Bounds.ParentBounds.absMarginX = 0;
                    Bounds.ParentBounds.absMarginY = 0;
                    Bounds.ParentBounds.MarkDirtyRecursive();
                    Bounds.ParentBounds.CalcWorldBounds();
                }
                return;
            }

            if (val == "auto")
            {
                if (parentBoundsBefore != null)
                {
                    Bounds.ParentBounds.fixedX = parentBoundsBefore.fixedX;
                    Bounds.ParentBounds.fixedY = parentBoundsBefore.fixedY;
                    Bounds.ParentBounds.fixedOffsetX = parentBoundsBefore.fixedOffsetX;
                    Bounds.ParentBounds.fixedOffsetY = parentBoundsBefore.fixedOffsetY;
                    Bounds.ParentBounds.Alignment = parentBoundsBefore.Alignment;
                    Bounds.ParentBounds.absMarginX = parentBoundsBefore.absMarginX;
                    Bounds.ParentBounds.absMarginY = parentBoundsBefore.absMarginY;

                    Bounds.ParentBounds.MarkDirtyRecursive();
                    Bounds.ParentBounds.CalcWorldBounds();
                }

                movable = false;
                api.Gui.SetDialogPosition(baseComposer.dialogName, null);
            }
            else
            {
                movable = true;
                parentBoundsBefore = Bounds.ParentBounds.FlatCopy();
                Bounds.ParentBounds.Alignment = EnumDialogArea.None;
                Bounds.ParentBounds.fixedOffsetX = 0;
                Bounds.ParentBounds.fixedOffsetY = 0;
                Bounds.ParentBounds.fixedX = Bounds.ParentBounds.absX / RuntimeEnv.GUIScale;
                Bounds.ParentBounds.fixedY = Bounds.ParentBounds.absY / RuntimeEnv.GUIScale;
                Bounds.ParentBounds.absMarginX = 0;
                Bounds.ParentBounds.absMarginY = 0;
                Bounds.ParentBounds.MarkDirtyRecursive();
                Bounds.ParentBounds.CalcWorldBounds();
            }
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            if (!didInit)
            {
                SetUpMovableState(null);
                didInit = true;
            }

            RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, ElementGeometrics.DialogBGRadius);
            ctx.SetSourceRGBA(ElementGeometrics.DialogDefaultBgColor);
            ctx.FillPreserve();

            Bounds.CalcWorldBounds();

            double radius = ElementGeometrics.DialogBGRadius;

            ctx.NewPath();
            ctx.MoveTo(Bounds.drawX, Bounds.drawY + Bounds.InnerHeight - 1);
            ctx.LineTo(Bounds.drawX, Bounds.drawY + radius);
            ctx.Arc(Bounds.drawX + radius, Bounds.drawY + radius, radius, 180 * GameMath.DEG2RAD, 270 * GameMath.DEG2RAD);
            ctx.Arc(Bounds.drawX + Bounds.OuterWidth - radius, Bounds.drawY + radius, radius, -90 * GameMath.DEG2RAD, 0 * GameMath.DEG2RAD);
            ctx.LineTo(Bounds.drawX + Bounds.OuterWidth, Bounds.drawY + Bounds.InnerHeight - 1);
            ctx.ClosePath();

            ctx.SetSourceRGBA(ElementGeometrics.TitleBarColor);
            ctx.FillPreserve();

            //EmbossRoundRectangleDialog(ctx, bounds.drawX, bounds.drawY, bounds.OuterWidth, bounds.InnerHeight - 1);
            EmbossRoundRectangleElement(ctx, Bounds.drawX, Bounds.drawY, Bounds.OuterWidth, Bounds.InnerHeight - 1);

            ctx.NewPath();
            ctx.MoveTo(Bounds.drawX, Bounds.drawY + Bounds.InnerHeight);
            ctx.LineTo(Bounds.drawX + Bounds.OuterWidth, Bounds.drawY + Bounds.InnerHeight);
            ctx.ClosePath();

            
            ctx.MoveTo(0, 0);
            Font.SetupContext(ctx);
            ShowTextCorrectly(ctx, text, scaled(ElementGeometrics.ElementToDialogPadding), scaled(-1));

            double crossSize = scaled(unscaledCloseIconSize);
            double menuSize = scaled(unscaledCloseIconSize + 2);
            double crossX = Bounds.drawX + Bounds.OuterWidth - crossSize - scaled(10);
            double iconY = Bounds.drawY + scaled(5);
            double crossWidth = 2;

            double menuX = Bounds.drawX + Bounds.OuterWidth - crossSize - menuSize - scaled(20);

            menuIconRect = new Rectangled(Bounds.OuterWidth - crossSize - menuSize - scaled(20), scaled(6), crossSize, crossSize);
            closeIconRect = new Rectangled(Bounds.OuterWidth - crossSize - scaled(10), scaled(5), menuSize, menuSize);

            ctx.Operator = Operator.Over;
            ctx.SetSourceRGBA(0, 0, 0, 0.3);
            api.Gui.Icons.DrawCross(ctx, crossX + 1, iconY + 1, crossWidth, crossSize);
            ctx.Operator = Operator.Source;
            ctx.SetSourceRGBA(1, 1, 1, 0.7);
            api.Gui.Icons.DrawCross(ctx, crossX, iconY, crossWidth, crossSize);

            ctx.Operator = Operator.Over;
            api.Gui.Icons.Drawmenuicon_svg(ctx, (int)menuX+1, (int)iconY+2, (int)menuSize, (int)menuSize, new double[] { 0, 0, 0, 0.3 });
            ctx.Operator = Operator.Source;
            api.Gui.Icons.Drawmenuicon_svg(ctx, (int)menuX, (int)iconY+1, (int)menuSize, (int)menuSize, new double[] { 1, 1, 1, 0.7 });

            ctx.Operator = Operator.Over;

            ComposeHoverIcons();


            listMenu.Bounds.fixedX = Bounds.absX + menuIconRect.X - Bounds.absX;

            listMenu.ComposeDynamicElements();
        }

        private void ComposeHoverIcons()
        {
            double crossSize = scaled(unscaledCloseIconSize);
            double menuSize = scaled(unscaledCloseIconSize + 2);
            int crossWidth = 2;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)crossSize + 4, (int)crossSize + 4);
            Context ctx = genContext(surface);

            ctx.Operator = Operator.Source;
            ctx.SetSourceRGBA(0.8, 0, 0, 0.9);
            api.Gui.Icons.DrawCross(ctx, 1.5, 1.5, crossWidth, crossSize);
            ctx.SetSourceRGBA(0.8, 0, 0, 0.6);
            api.Gui.Icons.DrawCross(ctx, 2, 2, crossWidth, crossSize);

            generateTexture(surface, ref closeIconHoverTexture);

            surface.Dispose();
            ctx.Dispose();

            

            surface = new ImageSurface(Format.Argb32, (int)menuSize + 4, (int)menuSize + 4);
            ctx = genContext(surface);

            ctx.Operator = Operator.Source;
            api.Gui.Icons.Drawmenuicon_svg(ctx, 1.5, 1.5, (int)menuSize, (int)menuSize, new double[] { 0.8, 0, 0, 0.9 });
            api.Gui.Icons.Drawmenuicon_svg(ctx, 2, 2, (int)menuSize, (int)menuSize, new double[] { 0.8, 0, 0, 0.6 });

            generateTexture(surface, ref menuIconHoverTexture);

            surface.Dispose();
            ctx.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            if (closeIconRect.PointInside(mouseX - Bounds.absX, mouseY - Bounds.absY))
            {
                api.Render.Render2DTexturePremultipliedAlpha(closeIconHoverTexture.TextureId, Bounds.absX + closeIconRect.X - 3, Bounds.absY + closeIconRect.Y - 3, closeIconRect.Width + 4, closeIconRect.Height + 4, 200);
            }
            
            if (menuIconRect.PointInside(mouseX - Bounds.absX, mouseY - Bounds.absY) || listMenu.IsOpened)
            {
                api.Render.Render2DTexturePremultipliedAlpha(menuIconHoverTexture.TextureId, Bounds.absX + menuIconRect.X - 2, Bounds.absY + menuIconRect.Y - 2, menuIconRect.Width + 6, menuIconRect.Height + 6, 200);
            }

            listMenu.RenderInteractiveElements(deltaTime);
        }


        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            if (closeIconRect.PointInside(mouseX - Bounds.absX, mouseY - Bounds.absY))
            {
                args.Handled = true;
                OnClose?.Invoke();
                return;
            }

            if (menuIconRect.PointInside(mouseX - Bounds.absX, mouseY - Bounds.absY))
            {
                listMenu.Open();
                //listMenu.hoveredIndex = listMenu.selectedIndex = movable ? 1 : 0;
                return;
            }
            
        }




        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            listMenu.OnKeyDown(api, args);
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseUp(api, args);

            if (moving)
            {
                api.Gui.SetDialogPosition(baseComposer.dialogName, new Vec2i((int)Bounds.ParentBounds.fixedX, (int)Bounds.ParentBounds.fixedY));
            }

            moving = false;
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseMove(api, args);

            if (moving)
            {
                Bounds.ParentBounds.fixedX += args.X - movingStartPos.X;
                Bounds.ParentBounds.fixedY += args.Y - movingStartPos.Y;
                movingStartPos.Set(args.X, args.Y);
                Bounds.ParentBounds.CalcWorldBounds();
            }
        }


        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseDown(api, args);

            if (movable && !args.Handled && IsPositionInside(args.X, args.Y))
            {
                moving = true;
                movingStartPos.Set(args.X, args.Y);
            }

            if (!listMenu.IsPositionInside(args.X, args.Y))
            {
                listMenu.Close();
            }
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();
            listMenu.OnFocusLost();
        }

        internal void SetSelectedIndex(int selectedIndex)
        {
            this.listMenu.SetSelectedIndex(selectedIndex);
        }

        public override void Dispose()
        {
            base.Dispose();

            closeIconHoverTexture.Dispose();
            menuIconHoverTexture.Dispose();
            listMenu?.Dispose();
        }
    }

    public static partial class GuiComposerHelpers
    {

        // Single rectangle shape
        public static GuiComposer AddDialogTitleBar(this GuiComposer composer, string text, Action OnClose = null, CairoFont font = null, ElementBounds bounds = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementDialogTitleBar(composer.Api, text, composer, OnClose, font, bounds));
            }

            return composer;
        }

        // Single rectangle shape
        public static GuiComposer AddDialogTitleBarWithBg(this GuiComposer composer, string text, Action OnClose = null, CairoFont font = null, ElementBounds bounds = null)
        {
            if (!composer.composed)
            {
                GuiElementDialogTitleBar elem = new GuiElementDialogTitleBar(composer.Api, text, composer, OnClose, font, bounds);
                elem.drawBg = true;
                composer.AddInteractiveElement(elem);
            }

            return composer;
        }

    }
}
