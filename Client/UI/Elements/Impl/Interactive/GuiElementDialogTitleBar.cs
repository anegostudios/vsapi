using System;
using Cairo;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable


namespace Vintagestory.API.Client
{
    /// <summary>
    /// A title bar for your GUI.  
    /// </summary>
    public class GuiElementDialogTitleBar : GuiElementTextBase
    {
        GuiElementListMenu listMenu;

        Action OnClose;
        internal GuiComposer baseComposer;

        /// <summary>
        /// The size of the close icon in the top right corner of the GUI.
        /// </summary>
        public static int unscaledCloseIconSize = 15;

        LoadedTexture closeIconHoverTexture;
        LoadedTexture menuIconHoverTexture;
        Rectangled closeIconRect;
        Rectangled menuIconRect;

        bool didInit = false;
        bool firstFrameRendered = false;
        public bool drawBg = false;
        bool movable = false;
        bool moving = false;

        Vec2i movingStartPos = new Vec2i();
        ElementBounds parentBoundsBefore = null;

        


        public bool Movable
        {
            get { return movable; }
        }

        /// <summary>
        /// Creates a new title bar.  
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="text">The text on the title bar.</param>
        /// <param name="composer">The GuiComposer for the title bar.</param>
        /// <param name="OnClose">The event fired when the title bar is closed.</param>
        /// <param name="font">The font of the title bar.</param>
        /// <param name="bounds">The bounds of the title bar.</param>
        public GuiElementDialogTitleBar(ICoreClientAPI capi, string text, GuiComposer composer, Action OnClose = null, CairoFont font = null, ElementBounds bounds = null) : base(capi, text, font, bounds)
        {
            closeIconHoverTexture = new LoadedTexture(capi);
            menuIconHoverTexture = new LoadedTexture(capi);

            if (bounds == null) this.Bounds = ElementStdBounds.TitleBar();
            if (font == null) this.Font = CairoFont.WhiteSmallText();
            this.OnClose = OnClose;

            ElementBounds dropDownBounds = ElementBounds.Fixed(0, 0, 100, 25);
            this.Bounds.WithChild(dropDownBounds);

            listMenu = new GuiElementListMenu(capi, new string[] { "auto", "manual" }, new string[] { Lang.Get("Fixed"), Lang.Get("Movable") }, 0, onSelectionChanged, dropDownBounds, CairoFont.WhiteSmallText(), false)
            {
                HoveredIndex = 0
            };

            baseComposer = composer;
        }

        private void onSelectionChanged(string val, bool on)
        {
            SetUpMovableState(val);
        }

        private void SetUpMovableState(string val)
        {
            if (val == null)
            {
                Vec2i pos = api.Gui.GetDialogPosition(baseComposer.DialogName);
                if (pos != null)
                {
                    movable = true;
                    parentBoundsBefore = Bounds.ParentBounds.FlatCopy();
                    Bounds.ParentBounds.Alignment = EnumDialogArea.None;
                    Bounds.ParentBounds.fixedX = pos.X;
                    Bounds.ParentBounds.fixedY = Math.Max(-Bounds.ParentBounds.fixedOffsetY, pos.Y);
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
                api.Gui.SetDialogPosition(baseComposer.DialogName, null);
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

            Bounds.CalcWorldBounds();

            double strokeWidth = 5;
            RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, 0);

            ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor[0] * 1.2, GuiStyle.DialogStrongBgColor[1] * 1.2, GuiStyle.DialogStrongBgColor[2] * 1.2, GuiStyle.DialogStrongBgColor[3]);
            ctx.FillPreserve();

            RoundRectangle(ctx, Bounds.bgDrawX + strokeWidth, Bounds.bgDrawY + strokeWidth, Bounds.OuterWidth - 2 * strokeWidth, Bounds.OuterHeight - 2 * strokeWidth, 0);
            ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.6, GuiStyle.DialogStrongBgColor[1] * 1.6, GuiStyle.DialogStrongBgColor[2] * 1.6, 1);
            ctx.LineWidth = strokeWidth * 1.75;
            ctx.StrokePreserve();

            var r = GuiElement.scaled(8);
            surface.BlurPartial(r, (int)(2 * r + 1), (int)Bounds.bgDrawX, (int)(Bounds.bgDrawY + 0), (int)Bounds.OuterWidth, (int)(Bounds.InnerHeight));

            double radius = 0;
            ctx.NewPath();
            ctx.MoveTo(Bounds.drawX, Bounds.drawY + Bounds.InnerHeight);
            ctx.LineTo(Bounds.drawX, Bounds.drawY + radius);
            ctx.Arc(Bounds.drawX + radius, Bounds.drawY + radius, radius, 180 * GameMath.DEG2RAD, 270 * GameMath.DEG2RAD);
            ctx.Arc(Bounds.drawX + Bounds.OuterWidth - radius, Bounds.drawY + radius, radius, -90 * GameMath.DEG2RAD, 0 * GameMath.DEG2RAD);
            ctx.LineTo(Bounds.drawX + Bounds.OuterWidth, Bounds.drawY + Bounds.InnerHeight);

            ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, 1 });
            ctx.LineWidth = strokeWidth;
            ctx.Stroke();




            Font.SetupContext(ctx);
            DrawTextLineAt(ctx, text, scaled(GuiStyle.ElementToDialogPadding), (Bounds.InnerHeight - Font.GetFontExtents().Height) / 2 + scaled(1));

            double crossSize = scaled(unscaledCloseIconSize);
            double menuSize = scaled(unscaledCloseIconSize + 2);
            double crossX = Bounds.drawX + Bounds.OuterWidth - crossSize - scaled(12);
            double iconY = Bounds.drawY + scaled(7);
            double crossWidth = scaled(2);

            double menuX = Bounds.drawX + Bounds.OuterWidth - crossSize - menuSize - scaled(20);

            menuIconRect = new Rectangled(Bounds.OuterWidth - crossSize - menuSize - scaled(20), scaled(6), crossSize, crossSize);
            closeIconRect = new Rectangled(Bounds.OuterWidth - crossSize - scaled(12), scaled(5), menuSize, menuSize);

            ctx.Operator = Operator.Over;
            ctx.SetSourceRGBA(0, 0, 0, 0.3);
            api.Gui.Icons.DrawCross(ctx, crossX + 2, iconY + 2, crossWidth, crossSize);
            ctx.Operator = Operator.Source;
            ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);
            api.Gui.Icons.DrawCross(ctx, crossX, iconY, crossWidth, crossSize);

            ctx.Operator = Operator.Over;
            api.Gui.Icons.Drawmenuicon_svg(ctx, (int)menuX+2, (int)iconY+2, (int)menuSize, (int)menuSize, new double[] { 0, 0, 0, 0.3 });
            ctx.Operator = Operator.Source;
            api.Gui.Icons.Drawmenuicon_svg(ctx, (int)menuX, (int)iconY+1, (int)menuSize, (int)menuSize, GuiStyle.DialogDefaultTextColor);

            ctx.Operator = Operator.Over;

            ComposeHoverIcons();


            listMenu.Bounds.fixedX = (Bounds.absX + menuIconRect.X - Bounds.absX) / RuntimeEnv.GUIScale;

            listMenu.ComposeDynamicElements();
        }

        private void ComposeHoverIcons()
        {
            double crossSize = scaled(unscaledCloseIconSize);
            double menuSize = scaled(unscaledCloseIconSize + 2);
            int crossWidth = (int)Math.Round(scaled(2 - 0.1));

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)crossSize + 4, (int)crossSize + 4);
            Context ctx = genContext(surface);

            ctx.Operator = Operator.Source;
            ctx.SetSourceRGBA(0.8, 0, 0, 1);
            api.Gui.Icons.DrawCross(ctx, 0.5, 1.5, crossWidth, crossSize);
            ctx.SetSourceRGBA(0.8, 0.2, 0.2, 1);
            api.Gui.Icons.DrawCross(ctx, 1, 2, crossWidth, crossSize);

            generateTexture(surface, ref closeIconHoverTexture);

            surface.Dispose();
            ctx.Dispose();

            

            surface = new ImageSurface(Format.Argb32, (int)menuSize, (int)menuSize);
            ctx = genContext(surface);

            ctx.Operator = Operator.Source;
            api.Gui.Icons.Drawmenuicon_svg(ctx, 0, scaled(1), (int)menuSize, (int)menuSize, new double[] { 0, 0.8, 0, 0.6 });

            generateTexture(surface, ref menuIconHoverTexture);

            surface.Dispose();
            ctx.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (!firstFrameRendered && movable)
            {
                var scale = RuntimeEnv.GUIScale;
                double maxx = api.Render.FrameWidth - 60 * scale;
                double maxy = api.Render.FrameHeight - 60 * scale;
                
                // In case the dialog somehow got out of bounds, make sure that its still movable by keeping it inside the window bounds
                double x = GameMath.Clamp((int)Bounds.ParentBounds.fixedX + Bounds.ParentBounds.fixedOffsetX, 0, maxx / scale) - Bounds.ParentBounds.fixedOffsetX;
                double y = GameMath.Clamp((int)Bounds.ParentBounds.fixedY + Bounds.ParentBounds.fixedOffsetY, 0, maxy / scale) - Bounds.ParentBounds.fixedOffsetY;
                
                api.Gui.SetDialogPosition(baseComposer.DialogName, new Vec2i((int)x, (int)y));

                Bounds.ParentBounds.fixedX = x;
                Bounds.ParentBounds.fixedY = y;
                Bounds.ParentBounds.CalcWorldBounds();

                firstFrameRendered = true;
            }

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            if (closeIconRect.PointInside(mouseX - Bounds.absX, mouseY - Bounds.absY))
            {
                api.Render.Render2DTexturePremultipliedAlpha(closeIconHoverTexture.TextureId, Bounds.absX + closeIconRect.X - scaled(1), Bounds.absY + closeIconRect.Y, closeIconRect.Width + 4, closeIconRect.Height + 4, 200);
            }
            
            if (menuIconRect.PointInside(mouseX - Bounds.absX, mouseY - Bounds.absY) || listMenu.IsOpened)
            {
                api.Render.Render2DTexturePremultipliedAlpha(menuIconHoverTexture.TextureId, Bounds.absX + menuIconRect.X, Bounds.absY + menuIconRect.Y, menuIconRect.Width + 4, menuIconRect.Height + 4, 200);
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
                return;
            }    
        }
        
        




        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            listMenu.OnKeyDown(api, args);
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseUp(api, args);

            base.OnMouseUp(api, args);

            if (moving)
            {
                api.Gui.SetDialogPosition(baseComposer.DialogName, new Vec2i((int)Bounds.ParentBounds.fixedX, (int)Bounds.ParentBounds.fixedY));
            }

            moving = false;
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseMove(api, args);

            if (moving)
            {
                Bounds.ParentBounds.fixedX += (args.X - movingStartPos.X) / RuntimeEnv.GUIScale;
                Bounds.ParentBounds.fixedY += (args.Y - movingStartPos.Y) / RuntimeEnv.GUIScale;
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

            if (!args.Handled && !listMenu.IsPositionInside(args.X, args.Y))
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
        /// <summary>
        /// Adds a dialog title bar to the GUI.  
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the title bar.</param>
        /// <param name="onClose">The event fired when the title bar is closed.</param>
        /// <param name="font">The font of the title bar.</param>
        /// <param name="bounds">The bounds of the title bar.</param>
        public static GuiComposer AddDialogTitleBar(this GuiComposer composer, string text, Action onClose = null, CairoFont font = null, ElementBounds bounds = null, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementDialogTitleBar(composer.Api, text, composer, onClose, font, bounds), key);
            }

            return composer;
        }

        // Single rectangle shape
        /// <summary>
        /// Adds a dialog title bar to the GUI with a background.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the title bar.</param>
        /// <param name="onClose">The event fired when the title bar is closed.</param>
        /// <param name="font">The font of the title bar.</param>
        /// <param name="bounds">The bounds of the title bar.</param>
        public static GuiComposer AddDialogTitleBarWithBg(this GuiComposer composer, string text, Action onClose = null, CairoFont font = null, ElementBounds bounds = null, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementDialogTitleBar elem = new GuiElementDialogTitleBar(composer.Api, text, composer, onClose, font, bounds);
                elem.drawBg = true;
                composer.AddInteractiveElement(elem, key);
            }

            return composer;
        }


        public static GuiElementDialogTitleBar GetTitleBar(this GuiComposer composer, string key)
        {
            return (GuiElementDialogTitleBar)composer.GetElement(key);
        }

    }
}
