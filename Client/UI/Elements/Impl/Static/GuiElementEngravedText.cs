﻿using System;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    class GuiElementEngravedText : GuiElementTextBase
    {
        EnumTextOrientation orientation;

        public GuiElementEngravedText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds, EnumTextOrientation orientation = EnumTextOrientation.Left) : base(capi, text, font, bounds) 
        {
            this.orientation = orientation;
        }


        //FreeTypeFontFace fontFace = FreeTypeFontFace.Create(LoadtimeSettings.AssetPath + "/font/" + LoadtimeSettings.GUIFontName, 0);
        //ctx.SetContextFontFace(fontFace);

        public override void ComposeTextElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Font.SetupContext(ctxStatic);

            Bounds.CalcWorldBounds();

            ImageSurface insetShadowSurface = new ImageSurface(Format.Argb32, Bounds.ParentBounds.OuterWidthInt, Bounds.ParentBounds.OuterHeightInt);
            Context ctxInsetShadow = new Context(insetShadowSurface);

            ctxInsetShadow.SetSourceRGB(0, 0, 0);
            ctxInsetShadow.Paint();
            Font.Color = new double[] { 20, 20, 20, 0.35f };
            Font.SetupContext(ctxInsetShadow);

            ShowMultilineText(ctxInsetShadow, text, Bounds.drawX + scaled(2), Bounds.drawY + scaled(2), Bounds.InnerWidth, orientation);


            insetShadowSurface.Blur(7,
                (int)Math.Max(0, Bounds.drawX - 4),
                (int)Math.Max(0, Bounds.drawY - 4),
                (int)Math.Min(Bounds.ParentBounds.OuterWidth, Bounds.drawX + Font.GetTextExtents(text).Width + 6),
                (int)Math.Min(Bounds.ParentBounds.OuterHeight, Bounds.drawY + ctxInsetShadow.FontExtents.Height + 6)
            );

            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.ParentBounds.OuterWidthInt, Bounds.ParentBounds.OuterHeightInt);
            Context ctxText = new Context(surface);

            ctxText.Operator = Operator.Source;

            ctxText.Antialias = Antialias.Best;

            Font.Color = new double[] { 0, 0, 0, 0.4 };
            Font.SetupContext(ctxText);

            ctxText.SetSourceRGBA(0, 0, 0, 0.4);
            ShowMultilineText(ctxText, text, Bounds.drawX - scaled(0.5), Bounds.drawY - scaled(0.5), Bounds.InnerWidth, orientation);

            ctxText.SetSourceRGBA(1, 1, 1, 1);
            ShowMultilineText(ctxText, text, Bounds.drawX + scaled(1), Bounds.drawY + scaled(1), Bounds.InnerWidth, orientation);

            ctxText.Operator = Operator.Atop;
            ctxText.SetSourceSurface(insetShadowSurface, 0, 0);
            ctxText.Paint();

            ctxInsetShadow.Dispose();
            insetShadowSurface.Dispose();

            ctxText.Operator = Operator.Over;
            Font.Color = new double[] { 0, 0, 0, 0.35 };
            Font.SetupContext(ctxText);
            ShowMultilineText(ctxText, text, (int)Bounds.drawX, (int)Bounds.drawY, Bounds.InnerWidth, orientation);

            ctxStatic.Antialias = Antialias.Best;
            ctxStatic.Operator = Operator.HardLight;
            ctxStatic.SetSourceSurface(surface, 0, 0);
            ctxStatic.Paint();

            surface.Dispose();
            ctxText.Dispose();

        }


        internal void TextWithSpacing(Context ctx, string text, double x, double y, float spacing)
        {
            foreach (char c in text)
            {
                TextExtents extents = ctx.TextExtents("" + c);
                ctx.MoveTo(x - extents.XBearing, x - extents.YBearing);
                ctx.ShowText("" + c);
                
                x += extents.Width + spacing * RuntimeEnv.GUIScale;
            }
        }

        
    }

    /*public static partial class GuiComposerHelpers
    {
        public static GuiComposer addEngravedText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, EnumTextOrientation orientation = EnumTextOrientation.Left, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementEngravedText element = new GuiElementEngravedText(text, font, bounds, orientation);

                composer.AddStaticElement(element, key);
            }
            return composer;
        }
    }*/
}
