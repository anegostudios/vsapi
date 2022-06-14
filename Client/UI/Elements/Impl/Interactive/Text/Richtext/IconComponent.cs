using Cairo;

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

        protected string iconName;
        protected CairoFont font;

        public IconComponent(ICoreClientAPI capi, string iconName, CairoFont font) : base(capi)
        {
            this.capi = capi;
            this.iconName = iconName;
            this.font = font;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            capi.Gui.Icons.DrawIcon(ctx, iconName, BoundsPerLine[0].X, BoundsPerLine[0].Y, GuiElement.scaled(font.UnscaledFontsize), GuiElement.scaled(font.UnscaledFontsize), font.Color);
        }


        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            offsetX += GuiElement.scaled(PaddingLeft);

            BoundsPerLine = new LineRectangled[]
            {
                new LineRectangled(offsetX, lineY, GuiElement.scaled(font.UnscaledFontsize), GuiElement.scaled(font.UnscaledFontsize))
            };

            bool requireLinebreak = offsetX + BoundsPerLine[0].Width > curfp.X2;

            nextOffsetX = (requireLinebreak ? 0 : offsetX) + BoundsPerLine[0].Width;

            return requireLinebreak;
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
