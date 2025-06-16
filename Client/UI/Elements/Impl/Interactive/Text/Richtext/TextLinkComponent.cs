using System;
using Cairo;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Client
{
    public class LinkTextComponent : RichTextComponent
    {
        Action<LinkTextComponent> onLinkClicked;

        public string Href;
        bool clickable =true;
        public bool Clickable {
            get
            {
                return clickable;
            }
            set
            {
                clickable = value;
                MouseOverCursor = clickable ? "linkselect" : null;
            }
        }

        LoadedTexture normalText;
        LoadedTexture hoverText;

        /// <summary>
        /// Create a dummy link text component for use with triggering link protocols through code. Not usable for anything gui related (it'll crash if you try)
        /// </summary>
        /// <param name="href"></param>
        public LinkTextComponent(string href) : base(null, "", null)
        {
            this.Href = href;
        }

        /// <summary>
        /// A text component with an embedded link.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="displayText">The text of the Text.</param>
        /// <param name="font"></param>
        /// <param name="onLinkClicked"></param>
        public LinkTextComponent(ICoreClientAPI api, string displayText, CairoFont font, Action<LinkTextComponent> onLinkClicked) : base(api, displayText, font)
        {
            this.onLinkClicked = onLinkClicked;
            MouseOverCursor = "linkselect";

            this.Font = this.Font.Clone().WithColor(GuiStyle.ActiveButtonTextColor);

            hoverText = new LoadedTexture(api);
            normalText = new LoadedTexture(api);
        }

        double leftMostX;
        double topMostY;

        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            return base.CalcBounds(flowPath, currentLineHeight, offsetX, lineY, out nextOffsetX);
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            leftMostX = 999999;
            topMostY = 999999;
            double rightMostX = 0;
            double bottomMostY = 0;

            for (int i = 0; i < Lines.Length; i++)
            {
                TextLine line = Lines[i];

                leftMostX = Math.Min(leftMostX, line.Bounds.X);
                topMostY = Math.Min(topMostY, line.Bounds.Y);

                rightMostX = Math.Max(rightMostX, line.Bounds.X + line.Bounds.Width);
                bottomMostY = Math.Max(bottomMostY, line.Bounds.Y + line.Bounds.Height);
            }

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(rightMostX - leftMostX), (int)(bottomMostY - topMostY));
            Context ctx = new Context(surface);
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();

            ctx.Save();
            Matrix m = ctx.Matrix;
            m.Translate((int)-leftMostX, (int)-topMostY);
            ctx.Matrix = m;
            
            CairoFont normalFont = this.Font;

            ComposeFor(ctx, surface);
            api.Gui.LoadOrUpdateCairoTexture(surface, false, ref normalText);

            ctx.Operator = Operator.Clear;
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();
            ctx.Operator = Operator.Over;

            this.Font = this.Font.Clone();
            this.Font.Color[0] = Math.Min(1, this.Font.Color[0] * 1.2);
            this.Font.Color[1] = Math.Min(1, this.Font.Color[1] * 1.2);
            this.Font.Color[2] = Math.Min(1, this.Font.Color[2] * 1.2);
            ComposeFor(ctx, surface);
            this.Font = normalFont;

            ctx.Restore();
            
            api.Gui.LoadOrUpdateCairoTexture(surface, false, ref hoverText);
            
            surface.Dispose();
            ctx.Dispose();
        }

        void ComposeFor(Context ctx, ImageSurface surface)
        { 
            textUtil.DrawMultilineText(ctx, Font, Lines, EnumTextOrientation.Left);

            ctx.LineWidth = 1;
            ctx.SetSourceRGBA(Font.Color);

            for (int i = 0; i < Lines.Length; i++)
            {
                TextLine line = Lines[i];
                ctx.MoveTo(line.Bounds.X, line.Bounds.Y + line.Bounds.AscentOrHeight + 2);
                ctx.LineTo(line.Bounds.X + line.Bounds.Width, line.Bounds.Y + line.Bounds.AscentOrHeight + 2);
                ctx.Stroke();
            }
        }

        bool isHover = false;
        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            base.RenderInteractiveElements(deltaTime, renderX, renderY, renderZ);
            isHover = false;

            double offsetX = GetFontOrientOffsetX();

            if (clickable)
            {
                foreach (var val in BoundsPerLine)
                {
                    if (val.PointInside(api.Input.MouseX - renderX - offsetX, api.Input.MouseY - renderY))
                    {
                        isHover = true;
                        break;
                    }
                }
            }

            api.Render.Render2DTexturePremultipliedAlpha(
                isHover ? hoverText.TextureId : normalText.TextureId, 
                (int)(renderX + leftMostX + offsetX), 
                (int)(renderY + topMostY), 
                hoverText.Width, hoverText.Height, (float)renderZ + 50
            );
        }

        public override bool UseMouseOverCursor(ElementBounds richtextBounds)
        {
            return isHover;
        }

        bool wasMouseDown=false;
        public override void OnMouseDown(MouseEvent args)
        {
            if (!clickable) return;

            double offsetX = GetFontOrientOffsetX();

            wasMouseDown = false;
            foreach (var val in BoundsPerLine)
            {
                if (val.PointInside(args.X - offsetX, args.Y))
                {
                    wasMouseDown = true;
                }
            }
        }

        public override void OnMouseUp(MouseEvent args)
        {
            if (!clickable || !wasMouseDown) return;
            double offsetX = GetFontOrientOffsetX();

            foreach (var val in BoundsPerLine)
            {
                if (val.PointInside(args.X - offsetX, args.Y))
                {
                    args.Handled = true;
                    Trigger();                    
                }
            }
        }

        public LinkTextComponent SetHref(string href)
        {
            this.Href = href;
            return this;
        }

        public void Trigger()
        {
            if (onLinkClicked == null)
            {
                if (Href != null) HandleLink();
            }
            else
            {
                onLinkClicked.Invoke(this);
            }
        }


        public void HandleLink()
        {
            if (Href.StartsWithOrdinal("hotkey://"))
            {
                api.Input.GetHotKeyByCode(Href.Substring("hotkey://".Length))?.Handler?.Invoke(null);
            }
            else
            {
                string[] parts = Href.Split(new string[] { "://" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && api.LinkProtocols != null && api.LinkProtocols.ContainsKey(parts[0]))
                {
                    api.LinkProtocols[parts[0]].Invoke(this);
                    return;
                }

                if (parts.Length > 0)
                {
                    if (parts[0].StartsWithOrdinal("http"))   // No need to check for https because "https" starts with "http"
                    {
                        api.Gui.OpenLink(Href);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            hoverText?.Dispose();
            normalText?.Dispose();
        }
    }
}
