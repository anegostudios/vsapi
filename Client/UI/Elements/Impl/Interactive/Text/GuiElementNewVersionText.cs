using System;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    class GuiElementNewVersionText : GuiElementTextBase
    {
        int textureId;
        public bool visible;
        public double offsetY;

        int shadowHeight = 10;

        double[] backColor = new double[] { 197 / 255.0, 137 / 255.0, 72 / 255.0, 1 };

        public GuiElementNewVersionText(ICoreClientAPI capi, CairoFont font, ElementBounds bounds) : base(capi, "", font, bounds)
        {
            
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            
        }

        public void RecomposeMultiLine(string versionnumber)
        {
            text = "Version " + versionnumber + " now available \\o/\nClick here to go to the downloads page";

            Bounds.fixedHeight = GetMultilineTextHeight(text, Bounds.InnerWidth, 1f) / RuntimeEnv.GUIScale;
            Bounds.CalcWorldBounds();

            offsetY = -2*Bounds.fixedHeight;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight + shadowHeight);
            Context ctx = genContext(surface);

            double iconX = scaled(15);
            double iconSize = scaled(14);
            double iconY = (Bounds.InnerHeight - iconSize) / 2;

            double[] bgc = ElementGeometrics.DarkBrownColor;
            bgc[0] /= 2;
            bgc[1] /= 2;
            bgc[2] /= 2;
            LinearGradient gradient = new LinearGradient(0, Bounds.OuterHeightInt, 0, Bounds.OuterHeightInt + 10);
            gradient.AddColorStop(0, new Color(bgc[0], bgc[1], bgc[2], 1));
            gradient.AddColorStop(1, new Color(bgc[0], bgc[1], bgc[2], 0));
            ctx.SetSource(gradient);
            ctx.Rectangle(0, Bounds.OuterHeightInt, Bounds.OuterWidthInt, Bounds.OuterHeightInt+10);
            ctx.Fill();
            gradient.Dispose();


            gradient = new LinearGradient(0, 0, Bounds.OuterWidth, 0);
            gradient.AddColorStop(0, new Color(backColor[0], backColor[1], backColor[2], 1));
            gradient.AddColorStop(0.99, new Color(backColor[0], backColor[1], backColor[2], 1));
            gradient.AddColorStop(1, new Color(backColor[0], backColor[1], backColor[2], 0));
            ctx.SetSource(gradient);
            ctx.Rectangle(0, 0, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            ctx.Fill();
            gradient.Dispose();

            ctx.Arc(Bounds.drawX + iconX, Bounds.OuterHeight / 2, iconSize/2 + scaled(4), 0, Math.PI * 2);
            ctx.SetSourceRGBA(ElementGeometrics.DarkBrownColor);
            ctx.Fill();

            double fontheight = Font.GetFontExtents().Height;

            byte[] pngdata = api.Assets.Get("textures/gui/newversion.png").Data;
            BitmapExternal bitmap = (BitmapExternal)api.Render.BitmapCreateFromPng(pngdata);
            surface.Image(bitmap.bmp, (int)(Bounds.drawX + iconX - iconSize / 2), (int)(Bounds.drawY + iconY), (int)iconSize, (int)iconSize);
            bitmap.Dispose();

            ShowMultilineText(ctx, text, Bounds.drawX + iconX + 20, Bounds.drawY, Bounds.InnerWidth, EnumTextOrientation.Left, 1f);

            generateTexture(surface, ref textureId);
            ctx.Dispose();
            surface.Dispose();
        }


        internal void Activate(string versionnumber)
        {
            visible = true;
            RecomposeMultiLine(versionnumber);
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (visible)
            {
                api.Render.Render2DTexturePremultipliedAlpha(textureId, (int)Bounds.renderX, (int)Bounds.renderY + offsetY, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight + shadowHeight);

                offsetY = Math.Min(0, offsetY + 100 * deltaTime);
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            if (visible && Bounds.PointInside(args.X, args.Y))
            {
                System.Diagnostics.Process.Start("https://account.vintagestory.at");
            }
        }

        
        



    }
}
