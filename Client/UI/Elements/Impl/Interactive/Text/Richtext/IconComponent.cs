using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draws an icon
    /// </summary>
    public class IconComponent : RichTextComponentBase
    {
        protected ICoreClientAPI capi;

        protected ElementBounds parentBounds;

        public double offY = 0;
        public double sizeMulSvg = 0.7; // no idea why

        protected string iconName;
        protected string iconPath;
        protected CairoFont font;

        public IconComponent(ICoreClientAPI capi, string iconName, string iconPath, CairoFont font) : base(capi)
        {
            this.capi = capi;
            this.iconName = iconName;
            this.iconPath = iconPath;
            this.font = font;

            BoundsPerLine = new LineRectangled[]
            {
                new LineRectangled(0, 0, GuiElement.scaled(font.UnscaledFontsize), GuiElement.scaled(font.UnscaledFontsize))
            };
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            double size = GuiElement.scaled(font.UnscaledFontsize);
            IAsset svgAsset = null;
            if (iconPath != null)
            {
                svgAsset = capi.Assets.TryGet(new AssetLocation(iconPath).WithPathPrefixOnce("textures/"), true);
            }

            if (svgAsset != null) {
                size *= sizeMulSvg;
                var asc = font.GetFontExtents().Ascent;
                capi.Gui.DrawSvg(svgAsset, surface, (int)BoundsPerLine[0].X, (int)(BoundsPerLine[0].Y + asc - (int)size)+2 /* why the 2 offset? Only god knows -_- */, (int)size, (int)size, ColorUtil.ColorFromRgba(font.Color));
            }
            else {
                capi.Gui.Icons.DrawIcon(ctx, iconName, BoundsPerLine[0].X, BoundsPerLine[0].Y, size, size, font.Color);
            }
        }


        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            offsetX += GuiElement.scaled(PaddingLeft);
            bool requireLinebreak = offsetX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : offsetX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight : 0);

            nextOffsetX = (requireLinebreak ? 0 : offsetX) + BoundsPerLine[0].Width;

            return requireLinebreak ? EnumCalcBoundsResult.Nextline : EnumCalcBoundsResult.Continue;
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            /*for (int i = 0; i < BoundsPerLine.Length; i++)
            {
                var bounds = BoundsPerLine[i];
                api.Render.RenderRectangle((float)renderX + (float)bounds.X, (float)renderY + (float)bounds.Y, 50, (float)bounds.Width, (float)bounds.Height, ColorUtil.ColorFromRgba(200, 255, 255, 128));
            }*/
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
