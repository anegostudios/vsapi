using System;
using System.Collections.Generic;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate void DrawDelegate(Context ctx, ImageSurface surface);

    public delegate void IconRendererDelegate(Context ctx, int x, int y, float w, float h, double[] rgba);

    public class IconUtil
    {
        ICoreClientAPI capi;

        public Dictionary<string, IconRendererDelegate> CustomIcons = new Dictionary<string, IconRendererDelegate>();

        public IconRendererDelegate SvgIconSource(AssetLocation loc)
        {
            var asset = capi.Assets.TryGet(loc);
            return SvgIconSource(asset);
        }
        public IconRendererDelegate SvgIconSource(IAsset asset)
        {
            return (ctx, x, y, w, h, rgba) =>
            {
                capi.Gui.DrawSvg(asset, ctx.GetTarget() as ImageSurface, x, y, (int)w, (int)h, ColorUtil.FromRGBADoubles(rgba));
            };
        }

        /// <summary>
        /// Creates a new IconUtil instance.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        public IconUtil(ICoreClientAPI capi)
        {
            this.capi = capi;

            CustomIcons["wpCross"] = (ctx, x, y, w, h, rgba) => { ctx.SetSourceRGBA(rgba); capi.Gui.Icons.DrawCross(ctx, x, y, 4, w); };
        }

        public LoadedTexture GenTexture(int width, int height, IAsset asset, double[] rgba = null)
        {
            var handler = SvgIconSource(asset);
            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            handler(ctx,0, 0, width, height, rgba ?? ColorUtil.WhiteArgbDouble);

            int textureId = capi.Gui.LoadCairoTexture(surface, true);

            surface.Dispose();
            ctx.Dispose();

            return new LoadedTexture(capi)
            {
                TextureId = textureId,
                Width = width,
                Height = height
            };
        }


        /// <summary>
        /// Generates the texture.  
        /// </summary>
        /// <param name="width">The width of the drawing</param>
        /// <param name="height">The height of the drawing.</param>
        /// <param name="drawHandler">A delegate which handles the drawing.</param>
        /// <returns>The resulting built texture.</returns>
        public LoadedTexture GenTexture(int width, int height, DrawDelegate drawHandler)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            drawHandler(ctx, surface);

            int textureId = capi.Gui.LoadCairoTexture(surface, true);

            surface.Dispose();
            ctx.Dispose();

            return new LoadedTexture(capi)
            {
                TextureId = textureId,
                Width = width,
                Height = height
            };
        }

        /// <summary>
        /// Draws the icon.
        /// </summary>
        /// <param name="cr">The context.</param>
        /// <param name="type">The type to draw</param>
        /// <param name="x">X position of the Icon.</param>
        /// <param name="y">Y position of the Icon.</param>
        /// <param name="width">Width of the Icon.</param>
        /// <param name="height">Height of the Icon.</param>
        /// <param name="rgba">The color of the icon.</param>
        public void DrawIcon(Context cr, string type, double x, double y, double width, double height, double[] rgba)
        {
            DrawIconInt(cr, type, (int)x, (int)y, (float)width, (float)height, rgba);
        }

        /// <summary>
        /// Draws the icon.
        /// </summary>
        /// <param name="cr">The context.</param>
        /// <param name="type">The type of icon to draw</param>
        /// <param name="x">X position of the Icon.</param>
        /// <param name="y">Y position of the Icon.</param>
        /// <param name="width">Width of the Icon.</param>
        /// <param name="height">Height of the Icon.</param>
        /// <param name="rgba">The color of the icon.</param>
        public void DrawIconInt(Context cr, string type, int x, int y, float width, float height, double[] rgba)
        {
            if (CustomIcons.TryGetValue(type, out var dele))
            {
                dele(cr, x, y, width, height, rgba);
                return;
            }

            switch (type)
            {
                case "none":
                    Drawnone_svg(cr, x, y, width, height, rgba);
                    break;

                case "dice":
                    Drawdice_svg(cr, x, y, width, height, rgba);
                    break;

                case "paintbrush":
                    Drawbrush_svg(cr, x, y, width, height, rgba);
                    break;

                case "raiselower":
                    Drawraiselower_svg(cr, x, y, width, height, rgba);
                    break;

                case "airbrush":
                    Drawairbrush_svg(cr, x, y, width, height, rgba);
                    break;

                case "erode":
                    Drawerode_svg(cr, x, y, width, height, rgba);
                    break;

                case "move":
                    Drawcursor_svg(cr, x, y, width, height, rgba);
                    break;

                case "import":
                    Drawimport_svg(cr, x, y, width, height, rgba);
                    break;

                case "eraser":
                    Draweraser_svg(cr, x, y, width, height, rgba);
                    break;

                case "growshrink":
                    Drawgrowshrink_svg(cr, x, y, width, height, rgba);
                    break;

                case "line":
                    Drawline_svg(cr, x, y, width, height, rgba);
                    break;

                case "lake":
                    Drawlake_svg(cr, x, y, width, height, rgba);
                    break;

                case "floodfill":
                    Drawfloodfill_svg(cr, x, y, width, height, rgba);
                    break;

                case "tree":
                    Drawtree_svg(cr, x, y, width, height, rgba);
                    break;

                case "undo":
                    Drawundo_svg(cr, x, y, width, height, rgba);
                    break;

                case "redo":
                    Drawredo_svg(cr, x, y, width, height, rgba);
                    break;

                case "select":
                    Drawselect_svg(cr, x, y, width, height, rgba);
                    break;

                case "repeat":
                    Drawrepeat_svg(cr, x, y, width, height, rgba);
                    break;

                case "trousers":
                    Drawtrousers_svg(cr, x, y, width, height, rgba);
                    break;

                case "gloves":
                    Drawgloves_svg(cr, x, y, width, height, rgba);
                    break;

                case "hat":
                    Drawhat_svg(cr, x, y, width, height, rgba);
                    break;

                case "shirt":
                    Drawshirt_svg(cr, x, y, width, height, rgba);
                    break;

                case "boots":
                    Drawboots_svg(cr, x, y, width, height, rgba);
                    break;

                case "basket":
                    Drawbasket_svg(cr, x, y, width, height, rgba);
                    break;

                case "cape":
                    Drawcape_svg(cr, x, y, width, height, rgba);
                    break;

                case "ring":
                    Drawring_svg(cr, x, y, width, height, rgba);
                    break;

                case "medal":
                    Drawmedal_svg(cr, x, y, width, height, rgba);
                    break;

                case "belt":
                    Drawbelt_svg(cr, x, y, width, height, rgba);
                    break;

                case "necklace":
                    Drawnecklace_svg(cr, x, y, width, height, rgba);
                    break;

                case "pullover":
                    Drawpullover_svg(cr, x, y, width, height, rgba);
                    break;

                case "mask":
                    Drawmask_svg(cr, x, y, width, height, rgba);
                    break;

                case "bracers":
                    Drawbracers_svg(cr, x, y, width, height, rgba);
                    break;

                case "handheld":
                    Drawhandheld_svg(cr, x, y, width, height, rgba);
                    break;

                case "left":
                    Drawleft_svg(cr, x, y, width, height, rgba);
                    break;

                case "right":
                    Drawright_svg(cr, x, y, width, height, rgba);
                    break;

                case "offhand":
                    Drawoffhand_svg(cr, x, y, width, height, rgba);
                    break;

                case "leftmousebutton":
                    DrawLeftMouseButton(cr, x, y, width, height, rgba);
                    break;

                case "rightmousebutton":
                    DrawRightMouseButton(cr, x, y, width, height, rgba);
                    break;

                case "plus":
                    float lineWidth = width / 8f;
                    DrawPlus(cr, x, y, width, height, rgba, lineWidth);
                    break;

                case "wpCross":
                    cr.SetSourceRGBA(0.8, 0.2, 0.2, 0.7);
                    DrawCross(cr, x, y, width, height);
                    cr.SetSourceRGBA(0.8, 0.2, 0.2, 0.5);
                    cr.Fill();
                    break;

                case "wpBee":
                    DrawWayointBee(cr, x, y, width, height, rgba);
                    break;

                case "wpCave":
                    DrawWaypointCave(cr, x, y, width, height, rgba);
                    break;

                case "wpHome":
                    DrawWaypointHome(cr, x, y, width, height, rgba);
                    break;

                case "wpLadder":
                    DrawWaypointLadder(cr, x, y, width, height, rgba);
                    break;

                case "wpCircle":
                    DrawWaypointCircle(cr, x, y, width, height, rgba);
                    break;

                case "wpPick":
                    DrawWaypointPick(cr, x, y, width, height, rgba);
                    break;

                case "wpPlayer":
                    DrawWaypointPlayer(cr, x, y, width, height, rgba);
                    break;

                case "wpRocks":
                    DrawWaypointRocks(cr, x, y, width, height, rgba);
                    break;

                case "wpRuins":
                    DrawWaypointRuins(cr, x, y, width, height, rgba);
                    break;

                case "wpSpiral":
                    DrawWaypointSpiral(cr, x, y, width, height, rgba);
                    break;

                case "wpStar1":
                    DrawWaypointStar1(cr, x, y, width, height, rgba);
                    break;

                case "wpStar2":
                    DrawWaypointStar2(cr, x, y, width, height, rgba);
                    break;

                case "wpTrader":
                    DrawWaypointTrader(cr, x, y, width, height, rgba);
                    break;

                case "wpVessel":
                    DrawWaypointVessel(cr, x, y, width, height, rgba);
                    break;

            }
        }


        public void DrawRightMouseButton(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 328;
            float h = 388;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 25;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(234.429688, 32.785156);
            cr.CurveTo(348.980469, 86.199219, 316.414063, 148.554688, 291.503906, 201.976563);
            cr.LineTo(249.421875, 292.214844);
            cr.CurveTo(229.363281, 335.230469, 179.390625, 407.363281, 81.46875, 360.8125);
            cr.CurveTo(-1.492188, 322.125, 6.308594, 221.769531, 23.949219, 183.9375);
            cr.LineTo(67.332031, 90.898438);
            cr.CurveTo(90.460938, 41.300781, 119.824219, -20.65625, 234.429688, 32.785156);
            cr.ClosePath();
            cr.MoveTo(234.429688, 32.785156);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.605242, -122.63237);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 25;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(234.429688, 32.78125);
            cr.LineTo(169.65625, 171.6875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.605242, -122.63237);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 25;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(61.726563, 102.519531);
            cr.CurveTo(61.726563, 102.519531, 93.355469, 137.152344, 169.257813, 172.546875);
            cr.CurveTo(213.550781, 193.199219, 285.589844, 215.28125, 285.589844, 215.28125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.605242, -122.63237);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(243.921875, 67.125);
            cr.LineTo(201.984375, 160.421875);
            cr.CurveTo(201.984375, 160.421875, 243.628906, 179.277344, 272.992188, 185.171875);
            cr.CurveTo(299.121094, 133.617188, 296.421875, 96.832031, 243.921875, 67.125);
            cr.ClosePath();
            cr.MoveTo(243.921875, 67.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.605242, -122.63237);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(219.097656, 165.527344);
            cr.CurveTo(237.675781, 173.027344, 256.894531, 179.460938, 268.761719, 182.160156);
            cr.LineTo(272.261719, 182.957031);
            cr.LineTo(276.113281, 173.964844);
            cr.CurveTo(282.488281, 159.089844, 285.617188, 148.3125, 286.726563, 137.398438);
            cr.CurveTo(288.980469, 115.261719, 280.804688, 97.152344, 261.460938, 81.414063);
            cr.CurveTo(256.835938, 77.65625, 245.53125, 69.957031, 244.945313, 70.167969);
            cr.CurveTo(244.621094, 70.289063, 217.152344, 131.136719, 209.398438, 148.917969);
            cr.LineTo(204.808594, 159.445313);
            cr.LineTo(208.546875, 161.109375);
            cr.CurveTo(210.601563, 162.027344, 215.347656, 164.015625, 219.097656, 165.527344);
            cr.ClosePath();
            cr.MoveTo(219.097656, 165.527344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.505076;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(219.097656, 165.527344);
            cr.CurveTo(237.675781, 173.027344, 256.894531, 179.460938, 268.761719, 182.160156);
            cr.LineTo(272.261719, 182.957031);
            cr.LineTo(276.113281, 173.964844);
            cr.CurveTo(282.488281, 159.089844, 285.617188, 148.3125, 286.726563, 137.398438);
            cr.CurveTo(288.980469, 115.261719, 280.804688, 97.152344, 261.460938, 81.414063);
            cr.CurveTo(256.835938, 77.65625, 245.53125, 69.957031, 244.945313, 70.167969);
            cr.CurveTo(244.621094, 70.289063, 217.152344, 131.136719, 209.398438, 148.917969);
            cr.LineTo(204.808594, 159.445313);
            cr.LineTo(208.546875, 161.109375);
            cr.CurveTo(210.601563, 162.027344, 215.347656, 164.015625, 219.097656, 165.527344);
            cr.ClosePath();
            cr.MoveTo(219.097656, 165.527344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.605242, -122.63237);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void DrawLeftMouseButton(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 328;
            float h = 388;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 25;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(234.425781, 32.78125);
            cr.CurveTo(348.976563, 86.199219, 316.410156, 148.554688, 291.5, 201.972656);
            cr.LineTo(249.421875, 292.214844);
            cr.CurveTo(229.363281, 335.226563, 179.390625, 407.359375, 81.464844, 360.808594);
            cr.CurveTo(-1.496094, 322.125, 6.308594, 221.765625, 23.949219, 183.933594);
            cr.LineTo(67.332031, 90.898438);
            cr.CurveTo(90.460938, 41.300781, 119.824219, -20.65625, 234.425781, 32.78125);
            cr.ClosePath();
            cr.MoveTo(234.425781, 32.78125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.606997, -122.634106);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 25;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(234.429688, 32.78125);
            cr.LineTo(169.652344, 171.6875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.606997, -122.634106);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 25;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(61.722656, 102.519531);
            cr.CurveTo(61.722656, 102.519531, 93.355469, 137.152344, 169.253906, 172.542969);
            cr.CurveTo(213.550781, 193.199219, 285.589844, 215.28125, 285.589844, 215.28125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.606997, -122.634106);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(200.53125, 44.171875);
            cr.LineTo(156.019531, 136.265625);
            cr.CurveTo(156.019531, 136.265625, 114.804688, 116.484375, 91.414063, 97.78125);
            cr.CurveTo(114.113281, 44.625, 144.027344, 23.046875, 200.53125, 44.171875);
            cr.ClosePath();
            cr.MoveTo(200.53125, 44.171875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.606997, -122.634106);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.109375, 126.4375);
            cr.CurveTo(123.417969, 117.027344, 106.136719, 106.441406, 96.441406, 99.085938);
            cr.LineTo(93.585938, 96.914063);
            cr.LineTo(97.996094, 88.183594);
            cr.CurveTo(105.292969, 73.738281, 111.539063, 64.414063, 119.183594, 56.546875);
            cr.CurveTo(134.695313, 40.59375, 153.820313, 35.214844, 178.308594, 39.921875);
            cr.CurveTo(184.164063, 41.042969, 197.328125, 44.757813, 197.539063, 45.34375);
            cr.CurveTo(197.660156, 45.667969, 168.703125, 105.820313, 160.066406, 123.191406);
            cr.LineTo(154.949219, 133.476563);
            cr.LineTo(151.273438, 131.683594);
            cr.CurveTo(149.25, 130.699219, 144.675781, 128.335938, 141.109375, 126.4375);
            cr.ClosePath();
            cr.MoveTo(141.109375, 126.4375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.505076;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.109375, 126.4375);
            cr.CurveTo(123.417969, 117.027344, 106.136719, 106.441406, 96.441406, 99.085938);
            cr.LineTo(93.585938, 96.914063);
            cr.LineTo(97.996094, 88.183594);
            cr.CurveTo(105.292969, 73.738281, 111.539063, 64.414063, 119.183594, 56.546875);
            cr.CurveTo(134.695313, 40.59375, 153.820313, 35.214844, 178.308594, 39.921875);
            cr.CurveTo(184.164063, 41.042969, 197.328125, 44.757813, 197.539063, 45.34375);
            cr.CurveTo(197.660156, 45.667969, 168.703125, 105.820313, 160.066406, 123.191406);
            cr.LineTo(154.949219, 133.476563);
            cr.LineTo(151.273438, 131.683594);
            cr.CurveTo(149.25, 130.699219, 144.675781, 128.335938, 141.109375, 126.4375);
            cr.ClosePath();
            cr.MoveTo(141.109375, 126.4375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -168.606997, -122.634106);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }




        public void Drawapple_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 70;
            float h = 84;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(58.730469, 27.890625);
            cr.CurveTo(55.242188, 25.496094, 50.855469, 24.300781, 46.667969, 25);
            cr.CurveTo(42.878906, 25.597656, 40.085938, 27.988281, 37.292969, 30.285156);
            cr.CurveTo(36.996094, 30.484375, 36.59375, 30.285156, 36.695313, 29.882813);
            cr.CurveTo(37.59375, 25.796875, 37.492188, 21.410156, 36.496094, 17.222656);
            cr.CurveTo(45.171875, 20.3125, 62.023438, 16.222656, 61.722656, 4.957031);
            cr.CurveTo(61.722656, 4.359375, 61.226563, 4.058594, 60.726563, 4.15625);
            cr.CurveTo(52.351563, 1.265625, 38.488281, 6.253906, 36.097656, 15.527344);
            cr.CurveTo(34.898438, 12.136719, 33.105469, 8.84375, 30.613281, 6.351563);
            cr.CurveTo(30.015625, 5.753906, 28.917969, 6.550781, 29.414063, 7.25);
            cr.CurveTo(34.601563, 15.027344, 36.894531, 22.707031, 34.601563, 31.878906);
            cr.CurveTo(32.707031, 29.785156, 30.8125, 27.589844, 28.019531, 26.394531);
            cr.CurveTo(24.429688, 24.800781, 20.542969, 24.898438, 16.953125, 26.292969);
            cr.CurveTo(7.679688, 29.785156, 3.089844, 40.851563, 2.992188, 50.226563);
            cr.CurveTo(2.890625, 60.496094, 9.773438, 69.171875, 17.25, 75.554688);
            cr.CurveTo(21.140625, 78.84375, 28.417969, 81.9375, 32.40625, 77.25);
            cr.CurveTo(33.105469, 76.453125, 34.402344, 76.351563, 35.101563, 77.25);
            cr.CurveTo(38.890625, 81.636719, 46.367188, 81.636719, 50.457031, 78.148438);
            cr.CurveTo(58.035156, 71.566406, 64.914063, 60.496094, 66.609375, 50.527344);
            cr.CurveTo(68.003906, 42.25, 65.910156, 32.777344, 58.730469, 27.890625);
            cr.ClosePath();
            cr.MoveTo(59.730469, 5.851563);
            cr.CurveTo(56.738281, 14.628906, 45.667969, 17.820313, 37.09375, 16.222656);
            cr.CurveTo(40.585938, 7.648438, 51.253906, 4.757813, 59.730469, 5.851563);
            cr.ClosePath();
            cr.MoveTo(65.015625, 48.53125);
            cr.CurveTo(64.214844, 55.808594, 60.527344, 62.390625, 56.339844, 68.273438);
            cr.CurveTo(52.449219, 73.757813, 40.984375, 82.832031, 35.101563, 71.566406);
            cr.CurveTo(34.898438, 71.265625, 34.5, 71.167969, 34.300781, 71.367188);
            cr.CurveTo(33.902344, 71.066406, 33.304688, 71.066406, 33.105469, 71.566406);
            cr.CurveTo(27.621094, 80.738281, 15.953125, 72.164063, 11.964844, 67.378906);
            cr.CurveTo(6.28125, 60.398438, 3.789063, 52.320313, 5.984375, 43.445313);
            cr.CurveTo(8.078125, 35.367188, 13.960938, 26.695313, 23.234375, 27.09375);
            cr.CurveTo(27.921875, 27.292969, 31.308594, 30.085938, 34.300781, 33.375);
            cr.CurveTo(36.097656, 35.367188, 41.78125, 41.75, 44.375, 42.546875);
            cr.CurveTo(44.972656, 42.746094, 40.882813, 36.964844, 40.285156, 36.664063);
            cr.CurveTo(39.785156, 36.464844, 39.289063, 36.167969, 38.890625, 35.867188);
            cr.CurveTo(37.492188, 34.96875, 37.59375, 32.875, 38.890625, 31.878906);
            cr.CurveTo(43.875, 28.191406, 48.5625, 25.097656, 55.441406, 28.289063);
            cr.CurveTo(63.019531, 31.878906, 65.8125, 40.554688, 65.015625, 48.53125);
            cr.ClosePath();
            cr.MoveTo(65.015625, 48.53125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 6;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(58.730469, 27.890625);
            cr.CurveTo(55.242188, 25.496094, 50.855469, 24.300781, 46.667969, 25);
            cr.CurveTo(42.878906, 25.597656, 40.085938, 27.988281, 37.292969, 30.285156);
            cr.CurveTo(36.996094, 30.484375, 36.59375, 30.285156, 36.695313, 29.882813);
            cr.CurveTo(37.59375, 25.796875, 37.492188, 21.410156, 36.496094, 17.222656);
            cr.CurveTo(45.171875, 20.3125, 62.023438, 16.222656, 61.722656, 4.957031);
            cr.CurveTo(61.722656, 4.359375, 61.226563, 4.058594, 60.726563, 4.15625);
            cr.CurveTo(52.351563, 1.265625, 38.488281, 6.253906, 36.097656, 15.527344);
            cr.CurveTo(34.898438, 12.136719, 33.105469, 8.84375, 30.613281, 6.351563);
            cr.CurveTo(30.015625, 5.753906, 28.917969, 6.550781, 29.414063, 7.25);
            cr.CurveTo(34.601563, 15.027344, 36.894531, 22.707031, 34.601563, 31.878906);
            cr.CurveTo(32.707031, 29.785156, 30.8125, 27.589844, 28.019531, 26.394531);
            cr.CurveTo(24.429688, 24.800781, 20.542969, 24.898438, 16.953125, 26.292969);
            cr.CurveTo(7.679688, 29.785156, 3.089844, 40.851563, 2.992188, 50.226563);
            cr.CurveTo(2.890625, 60.496094, 9.773438, 69.171875, 17.25, 75.554688);
            cr.CurveTo(21.140625, 78.84375, 28.417969, 81.9375, 32.40625, 77.25);
            cr.CurveTo(33.105469, 76.453125, 34.402344, 76.351563, 35.101563, 77.25);
            cr.CurveTo(38.890625, 81.636719, 46.367188, 81.636719, 50.457031, 78.148438);
            cr.CurveTo(58.035156, 71.566406, 64.914063, 60.496094, 66.609375, 50.527344);
            cr.CurveTo(68.003906, 42.25, 65.910156, 32.777344, 58.730469, 27.890625);
            cr.ClosePath();
            cr.MoveTo(59.730469, 5.851563);
            cr.CurveTo(56.738281, 14.628906, 45.667969, 17.820313, 37.09375, 16.222656);
            cr.CurveTo(40.585938, 7.648438, 51.253906, 4.757813, 59.730469, 5.851563);
            cr.ClosePath();
            cr.MoveTo(65.015625, 48.53125);
            cr.CurveTo(64.214844, 55.808594, 60.527344, 62.390625, 56.339844, 68.273438);
            cr.CurveTo(52.449219, 73.757813, 40.984375, 82.832031, 35.101563, 71.566406);
            cr.CurveTo(34.898438, 71.265625, 34.5, 71.167969, 34.300781, 71.367188);
            cr.CurveTo(33.902344, 71.066406, 33.304688, 71.066406, 33.105469, 71.566406);
            cr.CurveTo(27.621094, 80.738281, 15.953125, 72.164063, 11.964844, 67.378906);
            cr.CurveTo(6.28125, 60.398438, 3.789063, 52.320313, 5.984375, 43.445313);
            cr.CurveTo(8.078125, 35.367188, 13.960938, 26.695313, 23.234375, 27.09375);
            cr.CurveTo(27.921875, 27.292969, 31.308594, 30.085938, 34.300781, 33.375);
            cr.CurveTo(36.097656, 35.367188, 41.78125, 41.75, 44.375, 42.546875);
            cr.CurveTo(44.972656, 42.746094, 40.882813, 36.964844, 40.285156, 36.664063);
            cr.CurveTo(39.785156, 36.464844, 39.289063, 36.167969, 38.890625, 35.867188);
            cr.CurveTo(37.492188, 34.96875, 37.59375, 32.875, 38.890625, 31.878906);
            cr.CurveTo(43.875, 28.191406, 48.5625, 25.097656, 55.441406, 28.289063);
            cr.CurveTo(63.019531, 31.878906, 65.8125, 40.554688, 65.015625, 48.53125);
            cr.ClosePath();
            cr.MoveTo(65.015625, 48.53125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.997151, 0, 0, 0.997151, 0, 0.368946);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawhealth_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 98;
            float h = 98;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 8;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(94, 58);
            cr.LineTo(58, 58);
            cr.LineTo(58, 94);
            cr.LineTo(40, 94);
            cr.LineTo(40, 58);
            cr.LineTo(4, 58);
            cr.LineTo(4, 40);
            cr.LineTo(40, 40);
            cr.LineTo(40, 4);
            cr.LineTo(58, 4);
            cr.LineTo(58, 40);
            cr.LineTo(94, 40);
            cr.ClosePath();
            cr.MoveTo(94, 58);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }




        public void Drawoffhand_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            
            
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            matrix.Rotate(GameMath.PIHALF);
            matrix.Translate(-5, -5);


            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(226.101563, 50.898438);
            cr.LineTo(163.101563, 100.199219);
            cr.LineTo(169.300781, 108.101563);
            cr.LineTo(145.699219, 126.601563);
            cr.LineTo(151.898438, 134.5);
            cr.LineTo(136, 146.699219);
            cr.LineTo(142.199219, 154.601563);
            cr.LineTo(134.300781, 160.800781);
            cr.LineTo(140.5, 168.699219);
            cr.LineTo(124.699219, 181);
            cr.LineTo(130.898438, 188.898438);
            cr.LineTo(123, 195.101563);
            cr.LineTo(129.199219, 203);
            cr.LineTo(121.300781, 209.199219);
            cr.LineTo(127.5, 217.101563);
            cr.LineTo(119.601563, 223.300781);
            cr.LineTo(125.800781, 231.199219);
            cr.LineTo(117.898438, 237.398438);
            cr.LineTo(124.101563, 245.300781);
            cr.LineTo(116.199219, 251.5);
            cr.LineTo(122.398438, 259.398438);
            cr.LineTo(114.5, 265.601563);
            cr.LineTo(120.699219, 273.5);
            cr.LineTo(112.800781, 279.699219);
            cr.LineTo(119, 287.601563);
            cr.LineTo(111.101563, 293.800781);
            cr.LineTo(117.300781, 301.699219);
            cr.LineTo(109.398438, 307.898438);
            cr.LineTo(115.601563, 315.800781);
            cr.LineTo(107.699219, 322);
            cr.LineTo(113.898438, 329.898438);
            cr.LineTo(106, 336.101563);
            cr.LineTo(112.199219, 344);
            cr.LineTo(104.300781, 350.199219);
            cr.LineTo(110.5, 358.101563);
            cr.LineTo(102.601563, 364.300781);
            cr.LineTo(127.199219, 395.800781);
            cr.LineTo(135.101563, 389.601563);
            cr.LineTo(141.300781, 397.5);
            cr.LineTo(149.199219, 391.300781);
            cr.LineTo(155.398438, 399.199219);
            cr.LineTo(163.300781, 393);
            cr.LineTo(169.5, 400.898438);
            cr.LineTo(177.398438, 394.699219);
            cr.LineTo(183.601563, 402.601563);
            cr.LineTo(230.898438, 365.699219);
            cr.LineTo(224.699219, 357.800781);
            cr.LineTo(232.601563, 351.601563);
            cr.LineTo(226.398438, 343.699219);
            cr.LineTo(234.300781, 337.5);
            cr.LineTo(228.101563, 329.601563);
            cr.LineTo(236, 323.398438);
            cr.LineTo(229.800781, 315.5);
            cr.LineTo(237.699219, 309.300781);
            cr.LineTo(231.5, 301.398438);
            cr.LineTo(239.398438, 295.199219);
            cr.LineTo(245.601563, 303.101563);
            cr.LineTo(253.5, 296.898438);
            cr.LineTo(259.699219, 304.800781);
            cr.LineTo(267.601563, 298.601563);
            cr.LineTo(273.800781, 306.5);
            cr.LineTo(289.601563, 294.199219);
            cr.LineTo(295.800781, 302.101563);
            cr.LineTo(327.300781, 277.5);
            cr.LineTo(321.101563, 269.601563);
            cr.LineTo(329, 263.398438);
            cr.LineTo(322.800781, 255.5);
            cr.LineTo(330.699219, 249.300781);
            cr.LineTo(312.199219, 225.699219);
            cr.LineTo(304.300781, 231.898438);
            cr.LineTo(298.101563, 224);
            cr.LineTo(313.898438, 211.699219);
            cr.LineTo(332.398438, 235.300781);
            cr.LineTo(371.800781, 204.5);
            cr.LineTo(365.601563, 196.601563);
            cr.LineTo(381.398438, 184.300781);
            cr.LineTo(375.199219, 176.398438);
            cr.LineTo(383.101563, 170.199219);
            cr.LineTo(376.898438, 162.300781);
            cr.LineTo(384.800781, 156.101563);
            cr.LineTo(378.601563, 148.199219);
            cr.LineTo(386.5, 142);
            cr.LineTo(368, 118.398438);
            cr.LineTo(375.898438, 112.199219);
            cr.LineTo(332.800781, 57);
            cr.LineTo(324.898438, 63.199219);
            cr.LineTo(318.699219, 55.300781);
            cr.LineTo(310.800781, 61.5);
            cr.LineTo(304.601563, 53.601563);
            cr.LineTo(296.699219, 59.800781);
            cr.LineTo(290.5, 51.898438);
            cr.LineTo(282.601563, 58.101563);
            cr.LineTo(276.398438, 50.199219);
            cr.LineTo(268.5, 56.398438);
            cr.LineTo(262.300781, 48.5);
            cr.LineTo(254.398438, 54.699219);
            cr.LineTo(248.199219, 46.800781);
            cr.LineTo(232.398438, 59.101563);
            cr.ClosePath();
            cr.MoveTo(175.398438, 115.898438);
            cr.LineTo(246.300781, 60.5);
            cr.LineTo(252.5, 68.398438);
            cr.LineTo(260.398438, 62.199219);
            cr.LineTo(266.601563, 70.101563);
            cr.LineTo(274.5, 63.898438);
            cr.LineTo(280.699219, 71.800781);
            cr.LineTo(272.800781, 78);
            cr.LineTo(279, 85.898438);
            cr.LineTo(294.800781, 73.601563);
            cr.LineTo(301, 81.5);
            cr.LineTo(308.898438, 75.300781);
            cr.LineTo(315.101563, 83.199219);
            cr.LineTo(323, 77);
            cr.LineTo(335.300781, 92.800781);
            cr.LineTo(343.199219, 86.601563);
            cr.LineTo(355.5, 102.398438);
            cr.LineTo(347.601563, 108.601563);
            cr.LineTo(341.398438, 100.699219);
            cr.LineTo(333.5, 106.898438);
            cr.LineTo(327.300781, 99);
            cr.LineTo(319.398438, 105.199219);
            cr.LineTo(313.199219, 97.300781);
            cr.LineTo(297.398438, 109.601563);
            cr.LineTo(303.601563, 117.5);
            cr.LineTo(311.5, 111.300781);
            cr.LineTo(317.699219, 119.199219);
            cr.LineTo(325.601563, 113);
            cr.LineTo(331.800781, 120.898438);
            cr.LineTo(339.699219, 114.699219);
            cr.LineTo(345.898438, 122.601563);
            cr.LineTo(353.800781, 116.398438);
            cr.LineTo(366.101563, 132.199219);
            cr.LineTo(350, 144.398438);
            cr.LineTo(368.5, 168);
            cr.LineTo(337, 192.601563);
            cr.LineTo(330.800781, 184.699219);
            cr.LineTo(322.898438, 190.898438);
            cr.LineTo(316.699219, 183);
            cr.LineTo(277.300781, 213.800781);
            cr.LineTo(283.5, 221.699219);
            cr.LineTo(259.898438, 240.199219);
            cr.LineTo(266.101563, 248.101563);
            cr.LineTo(250.300781, 260.398438);
            cr.LineTo(256.5, 268.300781);
            cr.LineTo(264.398438, 262.101563);
            cr.LineTo(270.601563, 270);
            cr.LineTo(302.101563, 245.398438);
            cr.LineTo(314.398438, 261.199219);
            cr.LineTo(282.898438, 285.800781);
            cr.LineTo(276.699219, 277.898438);
            cr.LineTo(268.800781, 284.101563);
            cr.LineTo(262.601563, 276.199219);
            cr.LineTo(254.699219, 282.398438);
            cr.LineTo(248.5, 274.5);
            cr.LineTo(240.601563, 280.699219);
            cr.LineTo(234.398438, 272.800781);
            cr.LineTo(218.699219, 285);
            cr.LineTo(224.898438, 292.898438);
            cr.LineTo(185.5, 323.699219);
            cr.LineTo(179.300781, 315.800781);
            cr.LineTo(171.398438, 322);
            cr.LineTo(165.199219, 314.101563);
            cr.LineTo(157.300781, 320.300781);
            cr.LineTo(145, 304.5);
            cr.LineTo(137.101563, 310.699219);
            cr.LineTo(130.898438, 302.800781);
            cr.LineTo(138.800781, 296.601563);
            cr.LineTo(132.601563, 288.699219);
            cr.LineTo(140.5, 282.5);
            cr.LineTo(134.300781, 274.601563);
            cr.LineTo(142.199219, 268.398438);
            cr.LineTo(136, 260.5);
            cr.LineTo(143.898438, 254.300781);
            cr.LineTo(162.398438, 277.898438);
            cr.LineTo(170.300781, 271.699219);
            cr.LineTo(139.5, 232.300781);
            cr.LineTo(147.398438, 226.101563);
            cr.LineTo(166, 250.101563);
            cr.LineTo(173.898438, 243.898438);
            cr.LineTo(143.101563, 204.5);
            cr.LineTo(151, 198.300781);
            cr.LineTo(169.5, 222);
            cr.LineTo(177.398438, 215.800781);
            cr.LineTo(146.601563, 176.398438);
            cr.LineTo(162.398438, 164.101563);
            cr.LineTo(156.199219, 156.199219);
            cr.LineTo(164.101563, 150);
            cr.LineTo(157.898438, 142.101563);
            cr.LineTo(181.5, 123.601563);
            cr.ClosePath();
            cr.MoveTo(133.800781, 338.699219);
            cr.LineTo(141.699219, 332.5);
            cr.LineTo(154, 348.300781);
            cr.LineTo(161.898438, 342.101563);
            cr.LineTo(168.101563, 350);
            cr.LineTo(176, 343.800781);
            cr.LineTo(182.199219, 351.699219);
            cr.LineTo(213.699219, 327.101563);
            cr.LineTo(219.898438, 335);
            cr.LineTo(204, 347.398438);
            cr.LineTo(210.199219, 355.300781);
            cr.LineTo(178.699219, 379.898438);
            cr.LineTo(172.5, 372);
            cr.LineTo(164.601563, 378.199219);
            cr.LineTo(158.398438, 370.300781);
            cr.LineTo(150.5, 376.5);
            cr.LineTo(132, 352.898438);
            cr.LineTo(139.898438, 346.699219);
            cr.ClosePath();
            cr.MoveTo(133.800781, 338.699219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(266.5, 70.101563);
            cr.LineTo(258.601563, 76.199219);
            cr.LineTo(264.699219, 84.101563);
            cr.LineTo(272.601563, 77.898438);
            cr.ClosePath();
            cr.MoveTo(266.5, 70.101563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(291.101563, 101.601563);
            cr.LineTo(275.300781, 113.898438);
            cr.LineTo(281.5, 121.800781);
            cr.LineTo(297.300781, 109.5);
            cr.ClosePath();
            cr.MoveTo(291.101563, 101.601563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(307.898438, 139.199219);
            cr.LineTo(284.300781, 157.699219);
            cr.LineTo(290.5, 165.601563);
            cr.LineTo(306.300781, 153.300781);
            cr.LineTo(312.5, 161.199219);
            cr.LineTo(328.300781, 148.898438);
            cr.LineTo(334.5, 156.800781);
            cr.LineTo(350.300781, 144.5);
            cr.LineTo(344.101563, 136.601563);
            cr.LineTo(336.199219, 142.800781);
            cr.LineTo(330, 134.898438);
            cr.LineTo(314, 147.101563);
            cr.ClosePath();
            cr.MoveTo(307.898438, 139.199219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawmask_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.515625, 88.488281);
            cr.LineTo(69.308594, 88.488281);
            cr.LineTo(69.308594, 97.589844);
            cr.LineTo(51.105469, 97.589844);
            cr.LineTo(51.105469, 115.792969);
            cr.LineTo(42.003906, 115.792969);
            cr.LineTo(42.003906, 170.394531);
            cr.LineTo(51.105469, 170.394531);
            cr.LineTo(51.105469, 124.894531);
            cr.LineTo(60.207031, 124.894531);
            cr.LineTo(60.207031, 106.691406);
            cr.LineTo(87.507813, 106.691406);
            cr.LineTo(87.507813, 161.296875);
            cr.LineTo(96.609375, 161.296875);
            cr.LineTo(96.609375, 188.597656);
            cr.LineTo(69.308594, 188.597656);
            cr.LineTo(69.308594, 197.699219);
            cr.LineTo(105.710938, 197.699219);
            cr.LineTo(105.710938, 206.800781);
            cr.LineTo(114.808594, 206.800781);
            cr.LineTo(114.808594, 215.898438);
            cr.LineTo(133.011719, 215.898438);
            cr.LineTo(133.011719, 225);
            cr.LineTo(160.3125, 225);
            cr.LineTo(160.3125, 234.101563);
            cr.LineTo(205.816406, 234.101563);
            cr.LineTo(205.816406, 243.203125);
            cr.LineTo(214.917969, 243.203125);
            cr.LineTo(214.917969, 252.304688);
            cr.LineTo(224.019531, 252.304688);
            cr.LineTo(224.019531, 270.503906);
            cr.LineTo(233.117188, 270.503906);
            cr.LineTo(233.117188, 279.605469);
            cr.LineTo(242.222656, 279.605469);
            cr.LineTo(242.222656, 288.703125);
            cr.LineTo(251.320313, 288.703125);
            cr.LineTo(251.320313, 297.808594);
            cr.LineTo(269.523438, 297.808594);
            cr.LineTo(269.523438, 306.90625);
            cr.LineTo(287.722656, 306.90625);
            cr.LineTo(287.722656, 325.109375);
            cr.LineTo(296.824219, 325.109375);
            cr.LineTo(296.824219, 306.90625);
            cr.LineTo(333.226563, 306.90625);
            cr.LineTo(333.226563, 297.808594);
            cr.LineTo(378.730469, 297.808594);
            cr.LineTo(378.730469, 288.703125);
            cr.LineTo(396.929688, 288.703125);
            cr.LineTo(396.929688, 316.007813);
            cr.LineTo(406.03125, 316.007813);
            cr.LineTo(406.03125, 297.808594);
            cr.LineTo(415.132813, 297.808594);
            cr.LineTo(415.132813, 261.40625);
            cr.LineTo(406.03125, 261.40625);
            cr.LineTo(406.03125, 243.203125);
            cr.LineTo(396.929688, 243.203125);
            cr.LineTo(396.929688, 225);
            cr.LineTo(387.828125, 225);
            cr.LineTo(387.828125, 215.898438);
            cr.LineTo(378.726563, 215.898438);
            cr.LineTo(378.726563, 206.796875);
            cr.LineTo(369.628906, 206.796875);
            cr.LineTo(369.628906, 197.695313);
            cr.LineTo(360.527344, 197.695313);
            cr.LineTo(360.527344, 188.597656);
            cr.LineTo(351.425781, 188.597656);
            cr.LineTo(351.425781, 179.496094);
            cr.LineTo(333.226563, 179.496094);
            cr.LineTo(333.226563, 188.597656);
            cr.LineTo(342.324219, 188.597656);
            cr.LineTo(342.324219, 206.796875);
            cr.LineTo(360.527344, 206.796875);
            cr.LineTo(360.527344, 215.898438);
            cr.LineTo(342.324219, 215.898438);
            cr.LineTo(342.324219, 206.796875);
            cr.LineTo(333.226563, 206.796875);
            cr.LineTo(333.226563, 197.695313);
            cr.LineTo(305.921875, 197.695313);
            cr.LineTo(305.921875, 188.597656);
            cr.LineTo(287.722656, 188.597656);
            cr.LineTo(287.722656, 179.496094);
            cr.LineTo(278.621094, 179.496094);
            cr.LineTo(278.621094, 188.597656);
            cr.LineTo(269.519531, 188.597656);
            cr.LineTo(269.519531, 179.496094);
            cr.LineTo(260.417969, 179.496094);
            cr.LineTo(260.417969, 188.597656);
            cr.LineTo(251.316406, 188.597656);
            cr.LineTo(251.316406, 197.695313);
            cr.LineTo(242.21875, 197.695313);
            cr.LineTo(242.21875, 206.796875);
            cr.LineTo(233.113281, 206.796875);
            cr.LineTo(233.113281, 215.898438);
            cr.LineTo(242.21875, 215.898438);
            cr.LineTo(242.21875, 225);
            cr.LineTo(251.316406, 225);
            cr.LineTo(251.316406, 234.101563);
            cr.LineTo(242.21875, 234.101563);
            cr.LineTo(242.21875, 225);
            cr.LineTo(233.113281, 225);
            cr.LineTo(233.113281, 234.101563);
            cr.LineTo(224.015625, 234.101563);
            cr.LineTo(224.015625, 225);
            cr.LineTo(233.113281, 225);
            cr.LineTo(233.113281, 215.898438);
            cr.LineTo(224.015625, 215.898438);
            cr.LineTo(224.015625, 206.796875);
            cr.LineTo(214.914063, 206.796875);
            cr.LineTo(214.914063, 215.898438);
            cr.LineTo(205.8125, 215.898438);
            cr.LineTo(205.8125, 206.796875);
            cr.LineTo(214.914063, 206.796875);
            cr.LineTo(214.914063, 197.695313);
            cr.LineTo(224.015625, 197.695313);
            cr.LineTo(224.015625, 188.597656);
            cr.LineTo(233.113281, 188.597656);
            cr.LineTo(233.113281, 179.496094);
            cr.LineTo(242.21875, 179.496094);
            cr.LineTo(242.21875, 170.394531);
            cr.LineTo(251.316406, 170.394531);
            cr.LineTo(251.316406, 161.292969);
            cr.LineTo(260.417969, 161.292969);
            cr.LineTo(260.417969, 170.394531);
            cr.LineTo(269.519531, 170.394531);
            cr.LineTo(269.519531, 161.292969);
            cr.LineTo(278.621094, 161.292969);
            cr.LineTo(278.621094, 170.394531);
            cr.LineTo(305.921875, 170.394531);
            cr.LineTo(305.921875, 179.496094);
            cr.LineTo(333.226563, 179.496094);
            cr.LineTo(333.226563, 170.394531);
            cr.LineTo(324.121094, 170.394531);
            cr.LineTo(324.121094, 161.292969);
            cr.LineTo(287.722656, 161.292969);
            cr.LineTo(287.722656, 152.191406);
            cr.LineTo(278.621094, 152.191406);
            cr.LineTo(278.621094, 143.09375);
            cr.LineTo(260.417969, 143.09375);
            cr.LineTo(260.417969, 133.992188);
            cr.LineTo(251.320313, 133.992188);
            cr.LineTo(251.320313, 124.890625);
            cr.LineTo(242.21875, 124.890625);
            cr.LineTo(242.21875, 115.789063);
            cr.LineTo(224.015625, 115.789063);
            cr.LineTo(224.015625, 106.6875);
            cr.LineTo(214.914063, 106.6875);
            cr.LineTo(214.914063, 97.589844);
            cr.LineTo(178.511719, 97.589844);
            cr.LineTo(178.511719, 88.488281);
            cr.ClosePath();
            cr.MoveTo(96.609375, 106.691406);
            cr.LineTo(96.609375, 97.589844);
            cr.LineTo(114.808594, 97.589844);
            cr.LineTo(114.808594, 106.691406);
            cr.LineTo(123.910156, 106.691406);
            cr.LineTo(123.910156, 97.589844);
            cr.LineTo(151.210938, 97.589844);
            cr.LineTo(151.210938, 106.691406);
            cr.LineTo(196.714844, 106.691406);
            cr.LineTo(196.714844, 115.792969);
            cr.LineTo(205.816406, 115.792969);
            cr.LineTo(205.816406, 124.894531);
            cr.LineTo(224.015625, 124.894531);
            cr.LineTo(224.015625, 133.992188);
            cr.LineTo(242.21875, 133.992188);
            cr.LineTo(242.21875, 143.09375);
            cr.LineTo(251.320313, 143.09375);
            cr.LineTo(251.320313, 152.195313);
            cr.LineTo(242.21875, 152.195313);
            cr.LineTo(242.21875, 161.296875);
            cr.LineTo(233.117188, 161.296875);
            cr.LineTo(233.117188, 152.195313);
            cr.LineTo(224.015625, 152.195313);
            cr.LineTo(224.015625, 143.09375);
            cr.LineTo(205.8125, 143.09375);
            cr.LineTo(205.8125, 133.992188);
            cr.LineTo(187.613281, 133.992188);
            cr.LineTo(187.613281, 143.09375);
            cr.LineTo(196.714844, 143.09375);
            cr.LineTo(196.714844, 152.195313);
            cr.LineTo(205.8125, 152.195313);
            cr.LineTo(205.8125, 179.496094);
            cr.LineTo(169.410156, 179.496094);
            cr.LineTo(169.410156, 170.398438);
            cr.LineTo(151.210938, 170.398438);
            cr.LineTo(151.210938, 161.296875);
            cr.LineTo(142.109375, 161.296875);
            cr.LineTo(142.109375, 133.992188);
            cr.LineTo(187.613281, 133.992188);
            cr.LineTo(187.613281, 124.894531);
            cr.LineTo(178.511719, 124.894531);
            cr.LineTo(178.511719, 115.792969);
            cr.LineTo(133.007813, 115.792969);
            cr.LineTo(133.007813, 133.992188);
            cr.LineTo(123.90625, 133.992188);
            cr.LineTo(123.90625, 170.394531);
            cr.LineTo(142.109375, 170.394531);
            cr.LineTo(142.109375, 179.496094);
            cr.LineTo(160.3125, 179.496094);
            cr.LineTo(160.3125, 188.597656);
            cr.LineTo(169.410156, 188.597656);
            cr.LineTo(169.410156, 197.699219);
            cr.LineTo(187.613281, 197.699219);
            cr.LineTo(187.613281, 206.800781);
            cr.LineTo(169.410156, 206.800781);
            cr.LineTo(169.410156, 197.699219);
            cr.LineTo(123.910156, 197.699219);
            cr.LineTo(123.910156, 188.597656);
            cr.LineTo(114.808594, 188.597656);
            cr.LineTo(114.808594, 170.394531);
            cr.LineTo(105.707031, 170.394531);
            cr.LineTo(105.707031, 115.792969);
            cr.LineTo(114.808594, 115.792969);
            cr.LineTo(114.808594, 106.691406);
            cr.ClosePath();
            cr.MoveTo(196.714844, 206.796875);
            cr.LineTo(196.714844, 197.695313);
            cr.LineTo(205.816406, 197.695313);
            cr.LineTo(205.816406, 206.796875);
            cr.ClosePath();
            cr.MoveTo(242.21875, 261.402344);
            cr.LineTo(242.21875, 243.199219);
            cr.LineTo(260.417969, 243.199219);
            cr.LineTo(260.417969, 261.402344);
            cr.LineTo(269.519531, 261.402344);
            cr.LineTo(269.519531, 270.503906);
            cr.LineTo(278.621094, 270.503906);
            cr.LineTo(278.621094, 279.605469);
            cr.LineTo(269.519531, 279.605469);
            cr.LineTo(269.519531, 270.503906);
            cr.LineTo(260.417969, 270.503906);
            cr.LineTo(260.417969, 261.402344);
            cr.ClosePath();
            cr.MoveTo(269.519531, 243.199219);
            cr.LineTo(269.519531, 215.898438);
            cr.LineTo(260.417969, 215.898438);
            cr.LineTo(260.417969, 206.796875);
            cr.LineTo(305.921875, 206.796875);
            cr.LineTo(305.921875, 215.898438);
            cr.LineTo(324.121094, 215.898438);
            cr.LineTo(324.121094, 225);
            cr.LineTo(333.226563, 225);
            cr.LineTo(333.226563, 234.101563);
            cr.LineTo(342.324219, 234.101563);
            cr.LineTo(342.324219, 243.203125);
            cr.LineTo(351.425781, 243.203125);
            cr.LineTo(351.425781, 252.304688);
            cr.LineTo(342.324219, 252.304688);
            cr.LineTo(342.324219, 261.402344);
            cr.LineTo(287.722656, 261.402344);
            cr.LineTo(287.722656, 252.304688);
            cr.LineTo(278.621094, 252.304688);
            cr.LineTo(278.621094, 243.203125);
            cr.LineTo(269.519531, 243.203125);
            cr.ClosePath();
            cr.MoveTo(296.820313, 288.703125);
            cr.LineTo(296.820313, 279.601563);
            cr.LineTo(305.921875, 279.601563);
            cr.LineTo(305.921875, 288.703125);
            cr.ClosePath();
            cr.MoveTo(315.023438, 288.703125);
            cr.LineTo(315.023438, 279.601563);
            cr.LineTo(333.222656, 279.601563);
            cr.LineTo(333.222656, 288.703125);
            cr.ClosePath();
            cr.MoveTo(342.324219, 279.601563);
            cr.LineTo(342.324219, 270.503906);
            cr.LineTo(351.425781, 270.503906);
            cr.LineTo(351.425781, 279.601563);
            cr.LineTo(360.527344, 279.601563);
            cr.LineTo(360.527344, 288.703125);
            cr.LineTo(351.425781, 288.703125);
            cr.LineTo(351.425781, 279.601563);
            cr.ClosePath();
            cr.MoveTo(387.828125, 270.503906);
            cr.LineTo(387.828125, 261.402344);
            cr.LineTo(396.929688, 261.402344);
            cr.LineTo(396.929688, 270.503906);
            cr.ClosePath();
            cr.MoveTo(360.527344, 234.097656);
            cr.LineTo(360.527344, 225);
            cr.LineTo(369.628906, 225);
            cr.LineTo(369.628906, 234.101563);
            cr.LineTo(360.527344, 234.101563);
            cr.ClosePath();
            cr.MoveTo(378.726563, 252.300781);
            cr.LineTo(378.726563, 243.199219);
            cr.LineTo(387.828125, 243.199219);
            cr.LineTo(387.828125, 252.300781);
            cr.ClosePath();
            cr.MoveTo(378.726563, 252.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(242.21875, 143.09375);
            cr.LineTo(233.117188, 143.09375);
            cr.LineTo(233.117188, 152.195313);
            cr.LineTo(242.21875, 152.195313);
            cr.ClosePath();
            cr.MoveTo(242.21875, 143.09375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(242.21875, 143.09375);
            cr.LineTo(233.117188, 143.09375);
            cr.LineTo(233.117188, 152.195313);
            cr.LineTo(242.21875, 152.195313);
            cr.ClosePath();
            cr.MoveTo(242.21875, 143.09375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(60.207031, 170.394531);
            cr.LineTo(51.105469, 170.394531);
            cr.LineTo(51.105469, 188.597656);
            cr.LineTo(69.308594, 188.597656);
            cr.LineTo(69.308594, 179.496094);
            cr.LineTo(60.207031, 179.496094);
            cr.ClosePath();
            cr.MoveTo(60.207031, 170.394531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(260.417969, 170.394531);
            cr.LineTo(251.320313, 170.394531);
            cr.LineTo(251.320313, 179.496094);
            cr.LineTo(260.417969, 179.496094);
            cr.ClosePath();
            cr.MoveTo(260.417969, 170.394531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(260.417969, 170.394531);
            cr.LineTo(251.320313, 170.394531);
            cr.LineTo(251.320313, 179.496094);
            cr.LineTo(260.417969, 179.496094);
            cr.ClosePath();
            cr.MoveTo(260.417969, 170.394531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(278.621094, 170.394531);
            cr.LineTo(269.519531, 170.394531);
            cr.LineTo(269.519531, 179.496094);
            cr.LineTo(278.621094, 179.496094);
            cr.ClosePath();
            cr.MoveTo(278.621094, 170.394531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(278.621094, 170.394531);
            cr.LineTo(269.519531, 170.394531);
            cr.LineTo(269.519531, 179.496094);
            cr.LineTo(278.621094, 179.496094);
            cr.ClosePath();
            cr.MoveTo(278.621094, 170.394531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(251.320313, 179.496094);
            cr.LineTo(242.21875, 179.496094);
            cr.LineTo(242.21875, 188.597656);
            cr.LineTo(251.320313, 188.597656);
            cr.ClosePath();
            cr.MoveTo(251.320313, 179.496094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(251.320313, 179.496094);
            cr.LineTo(242.21875, 179.496094);
            cr.LineTo(242.21875, 188.597656);
            cr.LineTo(251.320313, 188.597656);
            cr.ClosePath();
            cr.MoveTo(251.320313, 179.496094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(242.21875, 188.597656);
            cr.LineTo(233.117188, 188.597656);
            cr.LineTo(233.117188, 197.699219);
            cr.LineTo(242.21875, 197.699219);
            cr.ClosePath();
            cr.MoveTo(242.21875, 188.597656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(242.21875, 188.597656);
            cr.LineTo(233.117188, 188.597656);
            cr.LineTo(233.117188, 197.699219);
            cr.LineTo(242.21875, 197.699219);
            cr.ClosePath();
            cr.MoveTo(242.21875, 188.597656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(233.117188, 197.699219);
            cr.LineTo(224.019531, 197.699219);
            cr.LineTo(224.019531, 206.796875);
            cr.LineTo(233.117188, 206.796875);
            cr.ClosePath();
            cr.MoveTo(233.117188, 197.699219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(233.117188, 197.699219);
            cr.LineTo(224.019531, 197.699219);
            cr.LineTo(224.019531, 206.796875);
            cr.LineTo(233.117188, 206.796875);
            cr.ClosePath();
            cr.MoveTo(233.117188, 197.699219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(396.929688, 316.007813);
            cr.LineTo(387.828125, 316.007813);
            cr.LineTo(387.828125, 334.207031);
            cr.LineTo(396.929688, 334.207031);
            cr.ClosePath();
            cr.MoveTo(396.929688, 316.007813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(287.722656, 325.105469);
            cr.LineTo(278.621094, 325.105469);
            cr.LineTo(278.621094, 361.507813);
            cr.LineTo(351.425781, 361.507813);
            cr.LineTo(351.425781, 352.40625);
            cr.LineTo(287.722656, 352.40625);
            cr.ClosePath();
            cr.MoveTo(287.722656, 325.105469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(387.828125, 334.207031);
            cr.LineTo(360.527344, 334.207031);
            cr.LineTo(360.527344, 343.308594);
            cr.LineTo(387.828125, 343.308594);
            cr.ClosePath();
            cr.MoveTo(387.828125, 334.207031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(360.527344, 343.308594);
            cr.LineTo(351.425781, 343.308594);
            cr.LineTo(351.425781, 352.40625);
            cr.LineTo(360.527344, 352.40625);
            cr.ClosePath();
            cr.MoveTo(360.527344, 343.308594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(360.527344, 343.308594);
            cr.LineTo(351.425781, 343.308594);
            cr.LineTo(351.425781, 352.40625);
            cr.LineTo(360.527344, 352.40625);
            cr.ClosePath();
            cr.MoveTo(360.527344, 343.308594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawbracers_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344.8125, 60);
            cr.LineTo(282.5625, 60);
            cr.LineTo(282.5625, 70.375);
            cr.LineTo(251.4375, 70.375);
            cr.LineTo(251.4375, 80.75);
            cr.LineTo(241.0625, 80.75);
            cr.LineTo(241.0625, 91.125);
            cr.LineTo(230.6875, 91.125);
            cr.LineTo(230.6875, 101.5);
            cr.LineTo(220.3125, 101.5);
            cr.LineTo(220.3125, 111.875);
            cr.LineTo(209.9375, 111.875);
            cr.LineTo(209.9375, 122.25);
            cr.LineTo(189.1875, 122.25);
            cr.LineTo(189.1875, 132.625);
            cr.LineTo(178.8125, 132.625);
            cr.LineTo(178.8125, 143);
            cr.LineTo(168.4375, 143);
            cr.LineTo(168.4375, 153.375);
            cr.LineTo(158.0625, 153.375);
            cr.LineTo(158.0625, 163.75);
            cr.LineTo(147.6875, 163.75);
            cr.LineTo(147.6875, 174.125);
            cr.LineTo(137.3125, 174.125);
            cr.LineTo(137.3125, 184.5);
            cr.LineTo(116.5625, 184.5);
            cr.LineTo(116.5625, 194.875);
            cr.LineTo(106.1875, 194.875);
            cr.LineTo(106.1875, 205.25);
            cr.LineTo(95.8125, 205.25);
            cr.LineTo(95.8125, 215.625);
            cr.LineTo(85.4375, 215.625);
            cr.LineTo(85.4375, 226);
            cr.LineTo(64.6875, 226);
            cr.LineTo(64.6875, 236.375);
            cr.LineTo(54.3125, 236.375);
            cr.LineTo(54.3125, 257.125);
            cr.LineTo(43.9375, 257.125);
            cr.LineTo(43.9375, 319.375);
            cr.LineTo(54.3125, 319.375);
            cr.LineTo(54.3125, 350.5);
            cr.LineTo(64.6875, 350.5);
            cr.LineTo(64.6875, 360.875);
            cr.LineTo(75.0625, 360.875);
            cr.LineTo(75.0625, 371.25);
            cr.LineTo(85.4375, 371.25);
            cr.LineTo(85.4375, 381.625);
            cr.LineTo(106.1875, 381.625);
            cr.LineTo(106.1875, 392);
            cr.LineTo(126.9375, 392);
            cr.LineTo(126.9375, 381.625);
            cr.LineTo(147.6875, 381.625);
            cr.LineTo(147.6875, 371.25);
            cr.LineTo(168.4375, 371.25);
            cr.LineTo(168.4375, 360.875);
            cr.LineTo(178.8125, 360.875);
            cr.LineTo(178.8125, 350.5);
            cr.LineTo(189.1875, 350.5);
            cr.LineTo(189.1875, 340.125);
            cr.LineTo(220.3125, 340.125);
            cr.LineTo(220.3125, 329.75);
            cr.LineTo(241.0625, 329.75);
            cr.LineTo(241.0625, 319.375);
            cr.LineTo(261.8125, 319.375);
            cr.LineTo(261.8125, 309);
            cr.LineTo(272.1875, 309);
            cr.LineTo(272.1875, 298.625);
            cr.LineTo(292.9375, 298.625);
            cr.LineTo(292.9375, 288.25);
            cr.LineTo(313.6875, 288.25);
            cr.LineTo(313.6875, 277.875);
            cr.LineTo(334.4375, 277.875);
            cr.LineTo(334.4375, 267.5);
            cr.LineTo(344.8125, 267.5);
            cr.LineTo(344.8125, 257.125);
            cr.LineTo(365.5625, 257.125);
            cr.LineTo(365.5625, 246.75);
            cr.LineTo(375.9375, 246.75);
            cr.LineTo(375.9375, 226);
            cr.LineTo(386.3125, 226);
            cr.LineTo(386.3125, 205.25);
            cr.LineTo(396.6875, 205.25);
            cr.LineTo(396.6875, 184.5);
            cr.LineTo(407.0625, 184.5);
            cr.LineTo(407.0625, 80.75);
            cr.LineTo(396.6875, 80.75);
            cr.LineTo(396.6875, 70.375);
            cr.LineTo(344.8125, 70.375);
            cr.ClosePath();
            cr.MoveTo(261.8125, 80.75);
            cr.LineTo(292.9375, 80.75);
            cr.LineTo(292.9375, 70.375);
            cr.LineTo(334.4375, 70.375);
            cr.LineTo(334.4375, 80.75);
            cr.LineTo(386.3125, 80.75);
            cr.LineTo(386.3125, 143);
            cr.LineTo(375.9375, 143);
            cr.LineTo(375.9375, 194.875);
            cr.LineTo(365.5625, 194.875);
            cr.LineTo(365.5625, 205.25);
            cr.LineTo(355.1875, 205.25);
            cr.LineTo(355.1875, 174.125);
            cr.LineTo(344.8125, 174.125);
            cr.LineTo(344.8125, 226);
            cr.LineTo(334.4375, 226);
            cr.LineTo(334.4375, 236.375);
            cr.LineTo(324.0625, 236.375);
            cr.LineTo(324.0625, 205.25);
            cr.LineTo(313.6875, 205.25);
            cr.LineTo(313.6875, 246.75);
            cr.LineTo(303.3125, 246.75);
            cr.LineTo(303.3125, 257.125);
            cr.LineTo(292.9375, 257.125);
            cr.LineTo(292.9375, 236.375);
            cr.LineTo(282.5625, 236.375);
            cr.LineTo(282.5625, 267.5);
            cr.LineTo(272.1875, 267.5);
            cr.LineTo(272.1875, 277.875);
            cr.LineTo(261.8125, 277.875);
            cr.LineTo(261.8125, 246.75);
            cr.LineTo(251.4375, 246.75);
            cr.LineTo(251.4375, 277.875);
            cr.LineTo(241.0625, 277.875);
            cr.LineTo(241.0625, 288.25);
            cr.LineTo(230.6875, 288.25);
            cr.LineTo(230.6875, 298.625);
            cr.LineTo(209.9375, 298.625);
            cr.LineTo(209.9375, 277.875);
            cr.LineTo(199.5625, 277.875);
            cr.LineTo(199.5625, 309);
            cr.LineTo(189.1875, 309);
            cr.LineTo(189.1875, 319.375);
            cr.LineTo(178.8125, 319.375);
            cr.LineTo(178.8125, 288.25);
            cr.LineTo(168.4375, 288.25);
            cr.LineTo(168.4375, 329.75);
            cr.LineTo(147.6875, 329.75);
            cr.LineTo(147.6875, 309);
            cr.LineTo(137.3125, 309);
            cr.LineTo(137.3125, 350.5);
            cr.LineTo(126.9375, 350.5);
            cr.LineTo(126.9375, 329.75);
            cr.LineTo(116.5625, 329.75);
            cr.LineTo(116.5625, 298.625);
            cr.LineTo(106.1875, 298.625);
            cr.LineTo(106.1875, 277.875);
            cr.LineTo(95.8125, 277.875);
            cr.LineTo(95.8125, 267.5);
            cr.LineTo(85.4375, 267.5);
            cr.LineTo(85.4375, 246.75);
            cr.LineTo(75.0625, 246.75);
            cr.LineTo(75.0625, 236.375);
            cr.LineTo(95.8125, 236.375);
            cr.LineTo(95.8125, 226);
            cr.LineTo(106.1875, 226);
            cr.LineTo(106.1875, 215.625);
            cr.LineTo(116.5625, 215.625);
            cr.LineTo(116.5625, 205.25);
            cr.LineTo(126.9375, 205.25);
            cr.LineTo(126.9375, 194.875);
            cr.LineTo(147.6875, 194.875);
            cr.LineTo(147.6875, 184.5);
            cr.LineTo(158.0625, 184.5);
            cr.LineTo(158.0625, 174.125);
            cr.LineTo(168.4375, 174.125);
            cr.LineTo(168.4375, 163.75);
            cr.LineTo(178.8125, 163.75);
            cr.LineTo(178.8125, 153.375);
            cr.LineTo(189.1875, 153.375);
            cr.LineTo(189.1875, 143);
            cr.LineTo(199.5625, 143);
            cr.LineTo(199.5625, 132.625);
            cr.LineTo(220.3125, 132.625);
            cr.LineTo(220.3125, 122.25);
            cr.LineTo(230.6875, 122.25);
            cr.LineTo(230.6875, 111.875);
            cr.LineTo(241.0625, 111.875);
            cr.LineTo(241.0625, 101.5);
            cr.LineTo(251.4375, 101.5);
            cr.LineTo(251.4375, 91.125);
            cr.LineTo(261.8125, 91.125);
            cr.ClosePath();
            cr.MoveTo(64.6875, 298.625);
            cr.LineTo(75.0625, 298.625);
            cr.LineTo(75.0625, 309);
            cr.LineTo(64.6875, 309);
            cr.ClosePath();
            cr.MoveTo(64.6875, 329.75);
            cr.LineTo(64.6875, 319.375);
            cr.LineTo(75.0625, 319.375);
            cr.LineTo(75.0625, 329.75);
            cr.LineTo(85.4375, 329.75);
            cr.LineTo(85.4375, 350.5);
            cr.LineTo(75.0625, 350.5);
            cr.LineTo(75.0625, 329.75);
            cr.ClosePath();
            cr.MoveTo(95.8125, 360.875);
            cr.LineTo(95.8125, 350.5);
            cr.LineTo(106.1875, 350.5);
            cr.LineTo(106.1875, 360.875);
            cr.ClosePath();
            cr.MoveTo(95.8125, 360.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(324.0625, 80.75);
            cr.LineTo(313.6875, 80.75);
            cr.LineTo(313.6875, 91.125);
            cr.LineTo(324.0625, 91.125);
            cr.ClosePath();
            cr.MoveTo(324.0625, 80.75);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(324.0625, 80.75);
            cr.LineTo(313.6875, 80.75);
            cr.LineTo(313.6875, 91.125);
            cr.LineTo(324.0625, 91.125);
            cr.ClosePath();
            cr.MoveTo(324.0625, 80.75);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(272.1875, 91.125);
            cr.LineTo(261.8125, 91.125);
            cr.LineTo(261.8125, 101.5);
            cr.LineTo(272.1875, 101.5);
            cr.ClosePath();
            cr.MoveTo(272.1875, 91.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(272.1875, 91.125);
            cr.LineTo(261.8125, 91.125);
            cr.LineTo(261.8125, 101.5);
            cr.LineTo(272.1875, 101.5);
            cr.ClosePath();
            cr.MoveTo(272.1875, 91.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344.8125, 91.125);
            cr.LineTo(334.4375, 91.125);
            cr.LineTo(334.4375, 101.5);
            cr.LineTo(282.5625, 101.5);
            cr.LineTo(282.5625, 111.875);
            cr.LineTo(272.1875, 111.875);
            cr.LineTo(272.1875, 132.625);
            cr.LineTo(261.8125, 132.625);
            cr.LineTo(261.8125, 163.75);
            cr.LineTo(272.1875, 163.75);
            cr.LineTo(272.1875, 174.125);
            cr.LineTo(292.9375, 174.125);
            cr.LineTo(292.9375, 163.75);
            cr.LineTo(303.3125, 163.75);
            cr.LineTo(303.3125, 174.125);
            cr.LineTo(313.6875, 174.125);
            cr.LineTo(313.6875, 153.375);
            cr.LineTo(324.0625, 153.375);
            cr.LineTo(324.0625, 143);
            cr.LineTo(334.4375, 143);
            cr.LineTo(334.4375, 132.625);
            cr.LineTo(324.0625, 132.625);
            cr.LineTo(324.0625, 122.25);
            cr.LineTo(334.4375, 122.25);
            cr.LineTo(334.4375, 111.875);
            cr.LineTo(344.8125, 111.875);
            cr.ClosePath();
            cr.MoveTo(292.9375, 111.875);
            cr.LineTo(313.6875, 111.875);
            cr.LineTo(313.6875, 122.25);
            cr.LineTo(303.3125, 122.25);
            cr.LineTo(303.3125, 143);
            cr.LineTo(282.5625, 143);
            cr.LineTo(282.5625, 153.375);
            cr.LineTo(272.1875, 153.375);
            cr.LineTo(272.1875, 143);
            cr.LineTo(282.5625, 143);
            cr.LineTo(282.5625, 122.25);
            cr.LineTo(292.9375, 122.25);
            cr.ClosePath();
            cr.MoveTo(292.9375, 111.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(375.9375, 91.125);
            cr.LineTo(355.1875, 91.125);
            cr.LineTo(355.1875, 101.5);
            cr.LineTo(365.5625, 101.5);
            cr.LineTo(365.5625, 122.25);
            cr.LineTo(375.9375, 122.25);
            cr.ClosePath();
            cr.MoveTo(375.9375, 91.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(251.4375, 111.875);
            cr.LineTo(241.0625, 111.875);
            cr.LineTo(241.0625, 122.25);
            cr.LineTo(251.4375, 122.25);
            cr.ClosePath();
            cr.MoveTo(251.4375, 111.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(251.4375, 111.875);
            cr.LineTo(241.0625, 111.875);
            cr.LineTo(241.0625, 122.25);
            cr.LineTo(251.4375, 122.25);
            cr.ClosePath();
            cr.MoveTo(251.4375, 111.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344.8125, 122.25);
            cr.LineTo(334.4375, 122.25);
            cr.LineTo(334.4375, 132.625);
            cr.LineTo(344.8125, 132.625);
            cr.ClosePath();
            cr.MoveTo(344.8125, 122.25);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344.8125, 122.25);
            cr.LineTo(334.4375, 122.25);
            cr.LineTo(334.4375, 132.625);
            cr.LineTo(344.8125, 132.625);
            cr.ClosePath();
            cr.MoveTo(344.8125, 122.25);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(230.6875, 132.625);
            cr.LineTo(220.3125, 132.625);
            cr.LineTo(220.3125, 143);
            cr.LineTo(230.6875, 143);
            cr.ClosePath();
            cr.MoveTo(230.6875, 132.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(230.6875, 132.625);
            cr.LineTo(220.3125, 132.625);
            cr.LineTo(220.3125, 143);
            cr.LineTo(230.6875, 143);
            cr.ClosePath();
            cr.MoveTo(230.6875, 132.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(375.9375, 132.625);
            cr.LineTo(365.5625, 132.625);
            cr.LineTo(365.5625, 143);
            cr.LineTo(375.9375, 143);
            cr.ClosePath();
            cr.MoveTo(375.9375, 132.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(375.9375, 132.625);
            cr.LineTo(365.5625, 132.625);
            cr.LineTo(365.5625, 143);
            cr.LineTo(375.9375, 143);
            cr.ClosePath();
            cr.MoveTo(375.9375, 132.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(199.5625, 153.375);
            cr.LineTo(189.1875, 153.375);
            cr.LineTo(189.1875, 163.75);
            cr.LineTo(199.5625, 163.75);
            cr.ClosePath();
            cr.MoveTo(199.5625, 153.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(199.5625, 153.375);
            cr.LineTo(189.1875, 153.375);
            cr.LineTo(189.1875, 163.75);
            cr.LineTo(199.5625, 163.75);
            cr.ClosePath();
            cr.MoveTo(199.5625, 153.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(334.4375, 153.375);
            cr.LineTo(324.0625, 153.375);
            cr.LineTo(324.0625, 163.75);
            cr.LineTo(334.4375, 163.75);
            cr.ClosePath();
            cr.MoveTo(334.4375, 153.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(334.4375, 153.375);
            cr.LineTo(324.0625, 153.375);
            cr.LineTo(324.0625, 163.75);
            cr.LineTo(334.4375, 163.75);
            cr.ClosePath();
            cr.MoveTo(334.4375, 153.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344.8125, 163.75);
            cr.LineTo(334.4375, 163.75);
            cr.LineTo(334.4375, 174.125);
            cr.LineTo(344.8125, 174.125);
            cr.ClosePath();
            cr.MoveTo(344.8125, 163.75);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344.8125, 163.75);
            cr.LineTo(334.4375, 163.75);
            cr.LineTo(334.4375, 174.125);
            cr.LineTo(344.8125, 174.125);
            cr.ClosePath();
            cr.MoveTo(344.8125, 163.75);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.8125, 174.125);
            cr.LineTo(168.4375, 174.125);
            cr.LineTo(168.4375, 184.5);
            cr.LineTo(178.8125, 184.5);
            cr.ClosePath();
            cr.MoveTo(178.8125, 174.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.8125, 174.125);
            cr.LineTo(168.4375, 174.125);
            cr.LineTo(168.4375, 184.5);
            cr.LineTo(178.8125, 184.5);
            cr.ClosePath();
            cr.MoveTo(178.8125, 174.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(158.0625, 194.875);
            cr.LineTo(147.6875, 194.875);
            cr.LineTo(147.6875, 205.25);
            cr.LineTo(158.0625, 205.25);
            cr.ClosePath();
            cr.MoveTo(158.0625, 194.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(158.0625, 194.875);
            cr.LineTo(147.6875, 194.875);
            cr.LineTo(147.6875, 205.25);
            cr.LineTo(158.0625, 205.25);
            cr.ClosePath();
            cr.MoveTo(158.0625, 194.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(313.6875, 194.875);
            cr.LineTo(303.3125, 194.875);
            cr.LineTo(303.3125, 205.25);
            cr.LineTo(313.6875, 205.25);
            cr.ClosePath();
            cr.MoveTo(313.6875, 194.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(313.6875, 194.875);
            cr.LineTo(303.3125, 194.875);
            cr.LineTo(303.3125, 205.25);
            cr.LineTo(313.6875, 205.25);
            cr.ClosePath();
            cr.MoveTo(313.6875, 194.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(137.3125, 205.25);
            cr.LineTo(126.9375, 205.25);
            cr.LineTo(126.9375, 215.625);
            cr.LineTo(137.3125, 215.625);
            cr.ClosePath();
            cr.MoveTo(137.3125, 205.25);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(137.3125, 205.25);
            cr.LineTo(126.9375, 205.25);
            cr.LineTo(126.9375, 215.625);
            cr.LineTo(137.3125, 215.625);
            cr.ClosePath();
            cr.MoveTo(137.3125, 205.25);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(116.5625, 226);
            cr.LineTo(106.1875, 226);
            cr.LineTo(106.1875, 236.375);
            cr.LineTo(116.5625, 236.375);
            cr.ClosePath();
            cr.MoveTo(116.5625, 226);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(116.5625, 226);
            cr.LineTo(106.1875, 226);
            cr.LineTo(106.1875, 236.375);
            cr.LineTo(116.5625, 236.375);
            cr.ClosePath();
            cr.MoveTo(116.5625, 226);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(282.5625, 226);
            cr.LineTo(272.1875, 226);
            cr.LineTo(272.1875, 236.375);
            cr.LineTo(282.5625, 236.375);
            cr.ClosePath();
            cr.MoveTo(282.5625, 226);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(282.5625, 226);
            cr.LineTo(272.1875, 226);
            cr.LineTo(272.1875, 236.375);
            cr.LineTo(282.5625, 236.375);
            cr.ClosePath();
            cr.MoveTo(282.5625, 226);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(106.1875, 236.375);
            cr.LineTo(95.8125, 236.375);
            cr.LineTo(95.8125, 246.75);
            cr.LineTo(106.1875, 246.75);
            cr.ClosePath();
            cr.MoveTo(106.1875, 236.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(106.1875, 236.375);
            cr.LineTo(95.8125, 236.375);
            cr.LineTo(95.8125, 246.75);
            cr.LineTo(106.1875, 246.75);
            cr.ClosePath();
            cr.MoveTo(106.1875, 236.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(251.4375, 236.375);
            cr.LineTo(241.0625, 236.375);
            cr.LineTo(241.0625, 246.75);
            cr.LineTo(251.4375, 246.75);
            cr.ClosePath();
            cr.MoveTo(251.4375, 236.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(251.4375, 236.375);
            cr.LineTo(241.0625, 236.375);
            cr.LineTo(241.0625, 246.75);
            cr.LineTo(251.4375, 246.75);
            cr.ClosePath();
            cr.MoveTo(251.4375, 236.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(116.5625, 257.125);
            cr.LineTo(106.1875, 257.125);
            cr.LineTo(106.1875, 267.5);
            cr.LineTo(116.5625, 267.5);
            cr.ClosePath();
            cr.MoveTo(116.5625, 257.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(116.5625, 257.125);
            cr.LineTo(106.1875, 257.125);
            cr.LineTo(106.1875, 267.5);
            cr.LineTo(116.5625, 267.5);
            cr.ClosePath();
            cr.MoveTo(116.5625, 257.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(126.9375, 267.5);
            cr.LineTo(116.5625, 267.5);
            cr.LineTo(116.5625, 288.25);
            cr.LineTo(126.9375, 288.25);
            cr.ClosePath();
            cr.MoveTo(126.9375, 267.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(199.5625, 267.5);
            cr.LineTo(189.1875, 267.5);
            cr.LineTo(189.1875, 277.875);
            cr.LineTo(199.5625, 277.875);
            cr.ClosePath();
            cr.MoveTo(199.5625, 267.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(199.5625, 267.5);
            cr.LineTo(189.1875, 267.5);
            cr.LineTo(189.1875, 277.875);
            cr.LineTo(199.5625, 277.875);
            cr.ClosePath();
            cr.MoveTo(199.5625, 267.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(168.4375, 277.875);
            cr.LineTo(158.0625, 277.875);
            cr.LineTo(158.0625, 288.25);
            cr.LineTo(168.4375, 288.25);
            cr.ClosePath();
            cr.MoveTo(168.4375, 277.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(168.4375, 277.875);
            cr.LineTo(158.0625, 277.875);
            cr.LineTo(158.0625, 288.25);
            cr.LineTo(168.4375, 288.25);
            cr.ClosePath();
            cr.MoveTo(168.4375, 277.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(137.3125, 298.625);
            cr.LineTo(126.9375, 298.625);
            cr.LineTo(126.9375, 309);
            cr.LineTo(137.3125, 309);
            cr.ClosePath();
            cr.MoveTo(137.3125, 298.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(137.3125, 298.625);
            cr.LineTo(126.9375, 298.625);
            cr.LineTo(126.9375, 309);
            cr.LineTo(137.3125, 309);
            cr.ClosePath();
            cr.MoveTo(137.3125, 298.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }
        

        public void Drawpullover_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 49.976563);
            cr.LineTo(177.105469, 49.976563);
            cr.LineTo(177.105469, 59.753906);
            cr.LineTo(89.09375, 59.753906);
            cr.LineTo(89.09375, 69.53125);
            cr.LineTo(69.53125, 69.53125);
            cr.LineTo(69.53125, 118.429688);
            cr.LineTo(79.3125, 118.429688);
            cr.LineTo(79.3125, 128.207031);
            cr.LineTo(98.871094, 128.207031);
            cr.LineTo(98.871094, 137.988281);
            cr.LineTo(167.324219, 137.988281);
            cr.LineTo(167.324219, 147.765625);
            cr.LineTo(157.546875, 147.765625);
            cr.LineTo(157.546875, 157.546875);
            cr.LineTo(137.988281, 157.546875);
            cr.LineTo(137.988281, 167.324219);
            cr.LineTo(118.429688, 167.324219);
            cr.LineTo(118.429688, 177.101563);
            cr.LineTo(98.871094, 177.101563);
            cr.LineTo(98.871094, 186.882813);
            cr.LineTo(89.09375, 186.882813);
            cr.LineTo(89.09375, 196.660156);
            cr.LineTo(69.53125, 196.660156);
            cr.LineTo(69.53125, 206.441406);
            cr.LineTo(59.753906, 206.441406);
            cr.LineTo(59.753906, 216.21875);
            cr.LineTo(49.976563, 216.21875);
            cr.LineTo(49.976563, 226);
            cr.LineTo(40.195313, 226);
            cr.LineTo(40.195313, 235.777344);
            cr.LineTo(49.976563, 235.777344);
            cr.LineTo(49.976563, 255.339844);
            cr.LineTo(59.753906, 255.339844);
            cr.LineTo(59.753906, 274.894531);
            cr.LineTo(69.53125, 274.894531);
            cr.LineTo(69.53125, 294.453125);
            cr.LineTo(79.3125, 294.453125);
            cr.LineTo(79.3125, 284.675781);
            cr.LineTo(89.089844, 284.675781);
            cr.LineTo(89.089844, 294.453125);
            cr.LineTo(79.3125, 294.453125);
            cr.LineTo(79.3125, 314.015625);
            cr.LineTo(89.089844, 314.015625);
            cr.LineTo(89.089844, 304.234375);
            cr.LineTo(98.871094, 304.234375);
            cr.LineTo(98.871094, 314.015625);
            cr.LineTo(89.089844, 314.015625);
            cr.LineTo(89.089844, 323.792969);
            cr.LineTo(98.871094, 323.792969);
            cr.LineTo(98.871094, 333.570313);
            cr.LineTo(108.648438, 333.570313);
            cr.LineTo(108.648438, 343.351563);
            cr.LineTo(118.429688, 343.351563);
            cr.LineTo(118.429688, 353.128906);
            cr.LineTo(137.984375, 353.128906);
            cr.LineTo(137.984375, 362.90625);
            cr.LineTo(167.324219, 362.90625);
            cr.LineTo(167.324219, 372.6875);
            cr.LineTo(196.660156, 372.6875);
            cr.LineTo(196.660156, 362.90625);
            cr.LineTo(206.441406, 362.90625);
            cr.LineTo(206.441406, 353.128906);
            cr.LineTo(216.21875, 353.128906);
            cr.LineTo(216.21875, 343.351563);
            cr.LineTo(226, 343.351563);
            cr.LineTo(226, 333.570313);
            cr.LineTo(235.777344, 333.570313);
            cr.LineTo(235.777344, 323.792969);
            cr.LineTo(245.558594, 323.792969);
            cr.LineTo(245.558594, 314.015625);
            cr.LineTo(255.335938, 314.015625);
            cr.LineTo(255.335938, 304.234375);
            cr.LineTo(265.117188, 304.234375);
            cr.LineTo(265.117188, 294.457031);
            cr.LineTo(274.894531, 294.457031);
            cr.LineTo(274.894531, 284.675781);
            cr.LineTo(284.671875, 284.675781);
            cr.LineTo(284.671875, 274.898438);
            cr.LineTo(294.453125, 274.898438);
            cr.LineTo(294.453125, 284.675781);
            cr.LineTo(284.671875, 284.675781);
            cr.LineTo(284.671875, 304.234375);
            cr.LineTo(294.453125, 304.234375);
            cr.LineTo(294.453125, 294.457031);
            cr.LineTo(304.230469, 294.457031);
            cr.LineTo(304.230469, 274.898438);
            cr.LineTo(314.011719, 274.898438);
            cr.LineTo(314.011719, 245.5625);
            cr.LineTo(323.789063, 245.5625);
            cr.LineTo(323.789063, 235.78125);
            cr.LineTo(333.566406, 235.78125);
            cr.LineTo(333.566406, 226);
            cr.LineTo(343.347656, 226);
            cr.LineTo(343.347656, 216.222656);
            cr.LineTo(333.566406, 216.222656);
            cr.LineTo(333.566406, 206.441406);
            cr.LineTo(323.789063, 206.441406);
            cr.LineTo(323.789063, 216.222656);
            cr.LineTo(314.011719, 216.222656);
            cr.LineTo(314.011719, 226);
            cr.LineTo(304.230469, 226);
            cr.LineTo(304.230469, 235.777344);
            cr.LineTo(294.453125, 235.777344);
            cr.LineTo(294.453125, 255.339844);
            cr.LineTo(284.671875, 255.339844);
            cr.LineTo(284.671875, 265.117188);
            cr.LineTo(245.558594, 265.117188);
            cr.LineTo(245.558594, 255.339844);
            cr.LineTo(226, 255.339844);
            cr.LineTo(226, 245.558594);
            cr.LineTo(206.441406, 245.558594);
            cr.LineTo(206.441406, 255.339844);
            cr.LineTo(216.21875, 255.339844);
            cr.LineTo(216.21875, 265.117188);
            cr.LineTo(235.777344, 265.117188);
            cr.LineTo(235.777344, 274.894531);
            cr.LineTo(265.117188, 274.894531);
            cr.LineTo(265.117188, 284.675781);
            cr.LineTo(255.339844, 284.675781);
            cr.LineTo(255.339844, 294.453125);
            cr.LineTo(245.558594, 294.453125);
            cr.LineTo(245.558594, 304.230469);
            cr.LineTo(235.78125, 304.230469);
            cr.LineTo(235.78125, 314.011719);
            cr.LineTo(226, 314.011719);
            cr.LineTo(226, 323.789063);
            cr.LineTo(216.222656, 323.789063);
            cr.LineTo(216.222656, 333.566406);
            cr.LineTo(206.441406, 333.566406);
            cr.LineTo(206.441406, 343.347656);
            cr.LineTo(186.882813, 343.347656);
            cr.LineTo(186.882813, 333.566406);
            cr.LineTo(177.105469, 333.566406);
            cr.LineTo(177.105469, 323.789063);
            cr.LineTo(167.324219, 323.789063);
            cr.LineTo(167.324219, 314.011719);
            cr.LineTo(157.546875, 314.011719);
            cr.LineTo(157.546875, 304.230469);
            cr.LineTo(147.769531, 304.230469);
            cr.LineTo(147.769531, 294.453125);
            cr.LineTo(137.988281, 294.453125);
            cr.LineTo(137.988281, 284.675781);
            cr.LineTo(128.210938, 284.675781);
            cr.LineTo(128.210938, 265.117188);
            cr.LineTo(118.429688, 265.117188);
            cr.LineTo(118.429688, 255.335938);
            cr.LineTo(108.652344, 255.335938);
            cr.LineTo(108.652344, 245.558594);
            cr.LineTo(89.09375, 245.558594);
            cr.LineTo(89.09375, 235.777344);
            cr.LineTo(69.53125, 235.777344);
            cr.LineTo(69.53125, 226);
            cr.LineTo(79.3125, 226);
            cr.LineTo(79.3125, 216.222656);
            cr.LineTo(89.089844, 216.222656);
            cr.LineTo(89.089844, 206.441406);
            cr.LineTo(108.648438, 206.441406);
            cr.LineTo(108.648438, 196.664063);
            cr.LineTo(128.207031, 196.664063);
            cr.LineTo(128.207031, 186.882813);
            cr.LineTo(137.988281, 186.882813);
            cr.LineTo(137.988281, 177.105469);
            cr.LineTo(157.546875, 177.105469);
            cr.LineTo(157.546875, 206.441406);
            cr.LineTo(167.324219, 206.441406);
            cr.LineTo(167.324219, 216.222656);
            cr.LineTo(177.105469, 216.222656);
            cr.LineTo(177.105469, 196.664063);
            cr.LineTo(167.324219, 196.664063);
            cr.LineTo(167.324219, 167.324219);
            cr.LineTo(177.105469, 167.324219);
            cr.LineTo(177.105469, 157.546875);
            cr.LineTo(186.882813, 157.546875);
            cr.LineTo(186.882813, 147.769531);
            cr.LineTo(206.441406, 147.769531);
            cr.LineTo(206.441406, 137.988281);
            cr.LineTo(216.21875, 137.988281);
            cr.LineTo(216.21875, 128.210938);
            cr.LineTo(235.777344, 128.210938);
            cr.LineTo(235.777344, 108.648438);
            cr.LineTo(216.21875, 108.648438);
            cr.LineTo(216.21875, 118.429688);
            cr.LineTo(98.871094, 118.429688);
            cr.LineTo(98.871094, 108.648438);
            cr.LineTo(89.09375, 108.648438);
            cr.LineTo(89.09375, 98.871094);
            cr.LineTo(79.3125, 98.871094);
            cr.LineTo(79.3125, 79.3125);
            cr.LineTo(98.871094, 79.3125);
            cr.LineTo(98.871094, 69.53125);
            cr.LineTo(186.882813, 69.53125);
            cr.LineTo(186.882813, 59.753906);
            cr.LineTo(304.234375, 59.753906);
            cr.LineTo(304.234375, 69.53125);
            cr.LineTo(314.011719, 69.53125);
            cr.LineTo(314.011719, 79.3125);
            cr.LineTo(323.792969, 79.3125);
            cr.LineTo(323.792969, 118.429688);
            cr.LineTo(343.351563, 118.429688);
            cr.LineTo(343.351563, 128.207031);
            cr.LineTo(372.6875, 128.207031);
            cr.LineTo(372.6875, 147.765625);
            cr.LineTo(382.46875, 147.765625);
            cr.LineTo(382.46875, 157.546875);
            cr.LineTo(392.246094, 157.546875);
            cr.LineTo(392.246094, 206.441406);
            cr.LineTo(382.46875, 206.441406);
            cr.LineTo(382.46875, 226);
            cr.LineTo(372.6875, 226);
            cr.LineTo(372.6875, 255.339844);
            cr.LineTo(362.910156, 255.339844);
            cr.LineTo(362.910156, 274.894531);
            cr.LineTo(353.128906, 274.894531);
            cr.LineTo(353.128906, 294.453125);
            cr.LineTo(343.351563, 294.453125);
            cr.LineTo(343.351563, 314.015625);
            cr.LineTo(333.570313, 314.015625);
            cr.LineTo(333.570313, 333.570313);
            cr.LineTo(323.792969, 333.570313);
            cr.LineTo(323.792969, 353.132813);
            cr.LineTo(314.015625, 353.132813);
            cr.LineTo(314.015625, 362.910156);
            cr.LineTo(304.234375, 362.910156);
            cr.LineTo(304.234375, 372.6875);
            cr.LineTo(294.457031, 372.6875);
            cr.LineTo(294.457031, 382.46875);
            cr.LineTo(265.121094, 382.46875);
            cr.LineTo(265.121094, 372.6875);
            cr.LineTo(255.339844, 372.6875);
            cr.LineTo(255.339844, 362.910156);
            cr.LineTo(265.121094, 362.910156);
            cr.LineTo(265.121094, 353.132813);
            cr.LineTo(274.898438, 353.132813);
            cr.LineTo(274.898438, 333.570313);
            cr.LineTo(284.675781, 333.570313);
            cr.LineTo(284.675781, 304.234375);
            cr.LineTo(274.898438, 304.234375);
            cr.LineTo(274.898438, 323.792969);
            cr.LineTo(265.121094, 323.792969);
            cr.LineTo(265.121094, 343.351563);
            cr.LineTo(255.339844, 343.351563);
            cr.LineTo(255.339844, 353.128906);
            cr.LineTo(245.5625, 353.128906);
            cr.LineTo(245.5625, 392.246094);
            cr.LineTo(265.121094, 392.246094);
            cr.LineTo(265.121094, 402.023438);
            cr.LineTo(304.238281, 402.023438);
            cr.LineTo(304.238281, 392.246094);
            cr.LineTo(314.015625, 392.246094);
            cr.LineTo(314.015625, 382.46875);
            cr.LineTo(323.796875, 382.46875);
            cr.LineTo(323.796875, 372.6875);
            cr.LineTo(333.574219, 372.6875);
            cr.LineTo(333.574219, 343.351563);
            cr.LineTo(343.351563, 343.351563);
            cr.LineTo(343.351563, 333.570313);
            cr.LineTo(353.132813, 333.570313);
            cr.LineTo(353.132813, 314.011719);
            cr.LineTo(362.910156, 314.011719);
            cr.LineTo(362.910156, 294.453125);
            cr.LineTo(372.6875, 294.453125);
            cr.LineTo(372.6875, 284.675781);
            cr.LineTo(382.46875, 284.675781);
            cr.LineTo(382.46875, 265.117188);
            cr.LineTo(392.246094, 265.117188);
            cr.LineTo(392.246094, 235.777344);
            cr.LineTo(402.027344, 235.777344);
            cr.LineTo(402.027344, 216.21875);
            cr.LineTo(411.804688, 216.21875);
            cr.LineTo(411.804688, 147.765625);
            cr.LineTo(402.027344, 147.765625);
            cr.LineTo(402.027344, 98.871094);
            cr.LineTo(392.246094, 98.871094);
            cr.LineTo(392.246094, 118.429688);
            cr.LineTo(382.46875, 118.429688);
            cr.LineTo(382.46875, 108.648438);
            cr.LineTo(372.6875, 108.648438);
            cr.LineTo(372.6875, 98.871094);
            cr.LineTo(362.910156, 98.871094);
            cr.LineTo(362.910156, 89.09375);
            cr.LineTo(353.132813, 89.09375);
            cr.LineTo(353.132813, 79.3125);
            cr.LineTo(343.351563, 79.3125);
            cr.LineTo(343.351563, 69.535156);
            cr.LineTo(362.910156, 69.535156);
            cr.LineTo(362.910156, 79.3125);
            cr.LineTo(372.691406, 79.3125);
            cr.LineTo(372.691406, 89.09375);
            cr.LineTo(382.46875, 89.09375);
            cr.LineTo(382.46875, 98.871094);
            cr.LineTo(392.246094, 98.871094);
            cr.LineTo(392.246094, 79.3125);
            cr.LineTo(382.46875, 79.3125);
            cr.LineTo(382.46875, 69.53125);
            cr.LineTo(372.691406, 69.53125);
            cr.LineTo(372.691406, 59.753906);
            cr.LineTo(314.015625, 59.753906);
            cr.LineTo(314.015625, 49.976563);
            cr.ClosePath();
            cr.MoveTo(108.648438, 323.792969);
            cr.LineTo(108.648438, 314.011719);
            cr.LineTo(118.429688, 314.011719);
            cr.LineTo(118.429688, 323.792969);
            cr.LineTo(128.207031, 323.792969);
            cr.LineTo(128.207031, 333.570313);
            cr.LineTo(137.988281, 333.570313);
            cr.LineTo(137.988281, 343.347656);
            cr.LineTo(128.207031, 343.347656);
            cr.LineTo(128.207031, 333.570313);
            cr.LineTo(118.429688, 333.570313);
            cr.LineTo(118.429688, 323.792969);
            cr.ClosePath();
            cr.MoveTo(108.648438, 323.792969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 79.3125);
            cr.LineTo(304.234375, 79.3125);
            cr.LineTo(304.234375, 89.09375);
            cr.LineTo(314.011719, 89.09375);
            cr.ClosePath();
            cr.MoveTo(314.011719, 79.3125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 79.3125);
            cr.LineTo(304.234375, 79.3125);
            cr.LineTo(304.234375, 89.09375);
            cr.LineTo(314.011719, 89.09375);
            cr.ClosePath();
            cr.MoveTo(314.011719, 79.3125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245.558594, 98.871094);
            cr.LineTo(235.777344, 98.871094);
            cr.LineTo(235.777344, 108.648438);
            cr.LineTo(245.558594, 108.648438);
            cr.ClosePath();
            cr.MoveTo(245.558594, 98.871094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245.558594, 98.871094);
            cr.LineTo(235.777344, 98.871094);
            cr.LineTo(235.777344, 108.648438);
            cr.LineTo(245.558594, 108.648438);
            cr.ClosePath();
            cr.MoveTo(245.558594, 98.871094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 98.871094);
            cr.LineTo(304.234375, 98.871094);
            cr.LineTo(304.234375, 108.648438);
            cr.LineTo(314.011719, 108.648438);
            cr.ClosePath();
            cr.MoveTo(314.011719, 98.871094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 98.871094);
            cr.LineTo(304.234375, 98.871094);
            cr.LineTo(304.234375, 108.648438);
            cr.LineTo(314.011719, 108.648438);
            cr.ClosePath();
            cr.MoveTo(314.011719, 98.871094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 128.207031);
            cr.LineTo(304.234375, 128.207031);
            cr.LineTo(304.234375, 137.988281);
            cr.LineTo(314.011719, 137.988281);
            cr.ClosePath();
            cr.MoveTo(314.011719, 128.207031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314.011719, 128.207031);
            cr.LineTo(304.234375, 128.207031);
            cr.LineTo(304.234375, 137.988281);
            cr.LineTo(314.011719, 137.988281);
            cr.ClosePath();
            cr.MoveTo(314.011719, 128.207031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(333.570313, 137.988281);
            cr.LineTo(323.792969, 137.988281);
            cr.LineTo(323.792969, 147.765625);
            cr.LineTo(333.570313, 147.765625);
            cr.ClosePath();
            cr.MoveTo(333.570313, 137.988281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(333.570313, 137.988281);
            cr.LineTo(323.792969, 137.988281);
            cr.LineTo(323.792969, 147.765625);
            cr.LineTo(333.570313, 147.765625);
            cr.ClosePath();
            cr.MoveTo(333.570313, 137.988281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(362.90625, 137.988281);
            cr.LineTo(353.128906, 137.988281);
            cr.LineTo(353.128906, 147.765625);
            cr.LineTo(362.90625, 147.765625);
            cr.ClosePath();
            cr.MoveTo(362.90625, 137.988281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(362.90625, 137.988281);
            cr.LineTo(353.128906, 137.988281);
            cr.LineTo(353.128906, 147.765625);
            cr.LineTo(362.90625, 147.765625);
            cr.ClosePath();
            cr.MoveTo(362.90625, 137.988281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(196.664063, 216.21875);
            cr.LineTo(177.105469, 216.21875);
            cr.LineTo(177.105469, 226);
            cr.LineTo(167.324219, 226);
            cr.LineTo(167.324219, 235.777344);
            cr.LineTo(177.105469, 235.777344);
            cr.LineTo(177.105469, 255.339844);
            cr.LineTo(186.882813, 255.339844);
            cr.LineTo(186.882813, 245.558594);
            cr.LineTo(206.441406, 245.558594);
            cr.LineTo(206.441406, 235.78125);
            cr.LineTo(216.222656, 235.78125);
            cr.LineTo(216.222656, 226);
            cr.LineTo(196.664063, 226);
            cr.ClosePath();
            cr.MoveTo(196.664063, 216.21875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawhandheld_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(298.800781, 105.847656);
            cr.LineTo(261.902344, 105.847656);
            cr.LineTo(261.902344, 124.300781);
            cr.LineTo(271.125, 124.300781);
            cr.LineTo(271.125, 151.972656);
            cr.LineTo(280.351563, 151.972656);
            cr.LineTo(280.351563, 170.425781);
            cr.LineTo(271.125, 170.425781);
            cr.LineTo(271.125, 179.648438);
            cr.LineTo(252.675781, 179.648438);
            cr.LineTo(252.675781, 170.425781);
            cr.LineTo(234.226563, 170.425781);
            cr.LineTo(234.226563, 161.199219);
            cr.LineTo(188.101563, 161.199219);
            cr.LineTo(188.101563, 151.972656);
            cr.LineTo(160.425781, 151.972656);
            cr.LineTo(160.425781, 170.425781);
            cr.LineTo(188.101563, 170.425781);
            cr.LineTo(188.101563, 179.648438);
            cr.LineTo(234.226563, 179.648438);
            cr.LineTo(234.226563, 188.875);
            cr.LineTo(243.449219, 188.875);
            cr.LineTo(243.449219, 198.097656);
            cr.LineTo(261.902344, 198.097656);
            cr.LineTo(261.902344, 207.324219);
            cr.LineTo(280.351563, 207.324219);
            cr.LineTo(280.351563, 198.097656);
            cr.LineTo(271.125, 198.097656);
            cr.LineTo(271.125, 188.875);
            cr.LineTo(280.351563, 188.875);
            cr.LineTo(280.351563, 179.648438);
            cr.LineTo(308.027344, 179.648438);
            cr.LineTo(308.027344, 188.875);
            cr.LineTo(317.25, 188.875);
            cr.LineTo(317.25, 198.097656);
            cr.LineTo(326.476563, 198.097656);
            cr.LineTo(326.476563, 207.324219);
            cr.LineTo(317.25, 207.324219);
            cr.LineTo(317.25, 216.550781);
            cr.LineTo(298.800781, 216.550781);
            cr.LineTo(298.800781, 207.324219);
            cr.LineTo(280.351563, 207.324219);
            cr.LineTo(280.351563, 225.773438);
            cr.LineTo(289.574219, 225.773438);
            cr.LineTo(289.574219, 262.675781);
            cr.LineTo(280.351563, 262.675781);
            cr.LineTo(280.351563, 271.898438);
            cr.LineTo(271.125, 271.898438);
            cr.LineTo(271.125, 262.675781);
            cr.LineTo(280.351563, 262.675781);
            cr.LineTo(280.351563, 253.449219);
            cr.LineTo(271.125, 253.449219);
            cr.LineTo(271.125, 244.222656);
            cr.LineTo(261.898438, 244.222656);
            cr.LineTo(261.898438, 299.574219);
            cr.LineTo(252.675781, 299.574219);
            cr.LineTo(252.675781, 308.800781);
            cr.LineTo(234.222656, 308.800781);
            cr.LineTo(234.222656, 299.574219);
            cr.LineTo(243.449219, 299.574219);
            cr.LineTo(243.449219, 290.347656);
            cr.LineTo(234.222656, 290.347656);
            cr.LineTo(234.222656, 281.121094);
            cr.LineTo(243.449219, 281.121094);
            cr.LineTo(243.449219, 271.898438);
            cr.LineTo(234.222656, 271.898438);
            cr.LineTo(234.222656, 262.675781);
            cr.LineTo(225, 262.675781);
            cr.LineTo(225, 318.023438);
            cr.LineTo(215.773438, 318.023438);
            cr.LineTo(215.773438, 308.800781);
            cr.LineTo(206.550781, 308.800781);
            cr.LineTo(206.550781, 299.574219);
            cr.LineTo(215.773438, 299.574219);
            cr.LineTo(215.773438, 290.347656);
            cr.LineTo(206.550781, 290.347656);
            cr.LineTo(206.550781, 281.125);
            cr.LineTo(197.324219, 281.125);
            cr.LineTo(197.324219, 327.25);
            cr.LineTo(178.875, 327.25);
            cr.LineTo(178.875, 318.023438);
            cr.LineTo(188.101563, 318.023438);
            cr.LineTo(188.101563, 308.800781);
            cr.LineTo(178.875, 308.800781);
            cr.LineTo(178.875, 299.574219);
            cr.LineTo(169.648438, 299.574219);
            cr.LineTo(169.648438, 290.347656);
            cr.LineTo(160.425781, 290.347656);
            cr.LineTo(160.425781, 281.125);
            cr.LineTo(151.199219, 281.125);
            cr.LineTo(151.199219, 290.347656);
            cr.LineTo(141.976563, 290.347656);
            cr.LineTo(141.976563, 299.574219);
            cr.LineTo(151.199219, 299.574219);
            cr.LineTo(151.199219, 308.800781);
            cr.LineTo(132.75, 308.800781);
            cr.LineTo(132.75, 318.027344);
            cr.LineTo(114.300781, 318.027344);
            cr.LineTo(114.300781, 327.25);
            cr.LineTo(95.851563, 327.25);
            cr.LineTo(95.851563, 336.476563);
            cr.LineTo(86.625, 336.476563);
            cr.LineTo(86.625, 327.25);
            cr.LineTo(77.398438, 327.25);
            cr.LineTo(77.398438, 336.476563);
            cr.LineTo(58.949219, 336.476563);
            cr.LineTo(58.949219, 318.027344);
            cr.LineTo(49.726563, 318.027344);
            cr.LineTo(49.726563, 308.800781);
            cr.LineTo(40.5, 308.800781);
            cr.LineTo(40.5, 336.476563);
            cr.LineTo(49.726563, 336.476563);
            cr.LineTo(49.726563, 354.925781);
            cr.LineTo(77.398438, 354.925781);
            cr.LineTo(77.398438, 364.152344);
            cr.LineTo(95.851563, 364.152344);
            cr.LineTo(95.851563, 354.925781);
            cr.LineTo(105.074219, 354.925781);
            cr.LineTo(105.074219, 345.699219);
            cr.LineTo(132.75, 345.699219);
            cr.LineTo(132.75, 336.472656);
            cr.LineTo(151.199219, 336.472656);
            cr.LineTo(151.199219, 345.699219);
            cr.LineTo(206.550781, 345.699219);
            cr.LineTo(206.550781, 336.472656);
            cr.LineTo(225, 336.472656);
            cr.LineTo(225, 327.25);
            cr.LineTo(252.675781, 327.25);
            cr.LineTo(252.675781, 318.023438);
            cr.LineTo(261.902344, 318.023438);
            cr.LineTo(261.902344, 308.800781);
            cr.LineTo(271.125, 308.800781);
            cr.LineTo(271.125, 299.574219);
            cr.LineTo(280.351563, 299.574219);
            cr.LineTo(280.351563, 290.347656);
            cr.LineTo(289.578125, 290.347656);
            cr.LineTo(289.578125, 271.898438);
            cr.LineTo(298.804688, 271.898438);
            cr.LineTo(298.804688, 262.675781);
            cr.LineTo(308.027344, 262.675781);
            cr.LineTo(308.027344, 244.222656);
            cr.LineTo(317.253906, 244.222656);
            cr.LineTo(317.253906, 235);
            cr.LineTo(326.476563, 235);
            cr.LineTo(326.476563, 225.773438);
            cr.LineTo(335.703125, 225.773438);
            cr.LineTo(335.703125, 244.222656);
            cr.LineTo(344.929688, 244.222656);
            cr.LineTo(344.929688, 262.675781);
            cr.LineTo(372.601563, 262.675781);
            cr.LineTo(372.601563, 253.449219);
            cr.LineTo(391.050781, 253.449219);
            cr.LineTo(391.050781, 244.222656);
            cr.LineTo(409.5, 244.222656);
            cr.LineTo(409.5, 225.773438);
            cr.LineTo(400.273438, 225.773438);
            cr.LineTo(400.273438, 207.324219);
            cr.LineTo(391.050781, 207.324219);
            cr.LineTo(391.050781, 235);
            cr.LineTo(372.601563, 235);
            cr.LineTo(372.601563, 225.773438);
            cr.LineTo(363.375, 225.773438);
            cr.LineTo(363.375, 207.324219);
            cr.LineTo(354.148438, 207.324219);
            cr.LineTo(354.148438, 198.097656);
            cr.LineTo(344.925781, 198.097656);
            cr.LineTo(344.925781, 188.875);
            cr.LineTo(335.699219, 188.875);
            cr.LineTo(335.699219, 170.425781);
            cr.LineTo(326.472656, 170.425781);
            cr.LineTo(326.472656, 151.972656);
            cr.LineTo(317.25, 151.972656);
            cr.LineTo(317.25, 133.523438);
            cr.LineTo(344.925781, 133.523438);
            cr.LineTo(344.925781, 151.972656);
            cr.LineTo(354.148438, 151.972656);
            cr.LineTo(354.148438, 124.300781);
            cr.LineTo(326.472656, 124.300781);
            cr.LineTo(326.472656, 115.074219);
            cr.LineTo(298.800781, 115.074219);
            cr.ClosePath();
            cr.MoveTo(308.023438, 133.523438);
            cr.LineTo(308.023438, 124.300781);
            cr.LineTo(317.25, 124.300781);
            cr.LineTo(317.25, 133.523438);
            cr.ClosePath();
            cr.MoveTo(169.648438, 318.023438);
            cr.LineTo(169.648438, 327.25);
            cr.LineTo(160.425781, 327.25);
            cr.LineTo(160.425781, 318.023438);
            cr.ClosePath();
            cr.MoveTo(169.648438, 318.023438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(372.601563, 151.972656);
            cr.LineTo(354.148438, 151.972656);
            cr.LineTo(354.148438, 161.199219);
            cr.LineTo(363.375, 161.199219);
            cr.LineTo(363.375, 170.425781);
            cr.LineTo(372.601563, 170.425781);
            cr.ClosePath();
            cr.MoveTo(372.601563, 151.972656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(381.824219, 170.425781);
            cr.LineTo(372.601563, 170.425781);
            cr.LineTo(372.601563, 188.875);
            cr.LineTo(381.824219, 188.875);
            cr.ClosePath();
            cr.MoveTo(381.824219, 170.425781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(391.050781, 188.875);
            cr.LineTo(381.824219, 188.875);
            cr.LineTo(381.824219, 207.324219);
            cr.LineTo(391.050781, 207.324219);
            cr.ClosePath();
            cr.MoveTo(391.050781, 188.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(317.25, 198.097656);
            cr.LineTo(308.023438, 198.097656);
            cr.LineTo(308.023438, 207.324219);
            cr.LineTo(317.25, 207.324219);
            cr.ClosePath();
            cr.MoveTo(317.25, 198.097656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(317.25, 198.097656);
            cr.LineTo(308.023438, 198.097656);
            cr.LineTo(308.023438, 207.324219);
            cr.LineTo(317.25, 207.324219);
            cr.ClosePath();
            cr.MoveTo(317.25, 198.097656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105.074219, 207.324219);
            cr.LineTo(95.851563, 207.324219);
            cr.LineTo(95.851563, 216.550781);
            cr.LineTo(105.074219, 216.550781);
            cr.ClosePath();
            cr.MoveTo(105.074219, 207.324219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105.074219, 207.324219);
            cr.LineTo(95.851563, 207.324219);
            cr.LineTo(95.851563, 216.550781);
            cr.LineTo(105.074219, 216.550781);
            cr.ClosePath();
            cr.MoveTo(105.074219, 207.324219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(234.226563, 207.324219);
            cr.LineTo(225, 207.324219);
            cr.LineTo(225, 216.550781);
            cr.LineTo(234.226563, 216.550781);
            cr.ClosePath();
            cr.MoveTo(234.226563, 207.324219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(234.226563, 207.324219);
            cr.LineTo(225, 207.324219);
            cr.LineTo(225, 216.550781);
            cr.LineTo(234.226563, 216.550781);
            cr.ClosePath();
            cr.MoveTo(234.226563, 207.324219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95.851563, 216.550781);
            cr.LineTo(77.398438, 216.550781);
            cr.LineTo(77.398438, 225.777344);
            cr.LineTo(86.625, 225.777344);
            cr.LineTo(86.625, 235);
            cr.LineTo(95.851563, 235);
            cr.LineTo(95.851563, 244.226563);
            cr.LineTo(105.074219, 244.226563);
            cr.LineTo(105.074219, 253.449219);
            cr.LineTo(114.300781, 253.449219);
            cr.LineTo(114.300781, 262.675781);
            cr.LineTo(95.851563, 262.675781);
            cr.LineTo(95.851563, 271.902344);
            cr.LineTo(86.625, 271.902344);
            cr.LineTo(86.625, 281.128906);
            cr.LineTo(77.398438, 281.128906);
            cr.LineTo(77.398438, 290.351563);
            cr.LineTo(58.949219, 290.351563);
            cr.LineTo(58.949219, 299.578125);
            cr.LineTo(49.726563, 299.578125);
            cr.LineTo(49.726563, 308.804688);
            cr.LineTo(68.175781, 308.804688);
            cr.LineTo(68.175781, 299.578125);
            cr.LineTo(86.625, 299.578125);
            cr.LineTo(86.625, 290.351563);
            cr.LineTo(95.851563, 290.351563);
            cr.LineTo(95.851563, 281.128906);
            cr.LineTo(105.074219, 281.128906);
            cr.LineTo(105.074219, 271.898438);
            cr.LineTo(141.976563, 271.898438);
            cr.LineTo(141.976563, 262.675781);
            cr.LineTo(151.199219, 262.675781);
            cr.LineTo(151.199219, 244.222656);
            cr.LineTo(141.976563, 244.222656);
            cr.LineTo(141.976563, 253.449219);
            cr.LineTo(132.75, 253.449219);
            cr.LineTo(132.75, 244.222656);
            cr.LineTo(123.523438, 244.222656);
            cr.LineTo(123.523438, 235);
            cr.LineTo(114.300781, 235);
            cr.LineTo(114.300781, 225.773438);
            cr.LineTo(95.851563, 225.773438);
            cr.ClosePath();
            cr.MoveTo(95.851563, 216.550781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(123.523438, 216.550781);
            cr.LineTo(114.300781, 216.550781);
            cr.LineTo(114.300781, 225.773438);
            cr.LineTo(123.523438, 225.773438);
            cr.ClosePath();
            cr.MoveTo(123.523438, 216.550781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(123.523438, 216.550781);
            cr.LineTo(114.300781, 216.550781);
            cr.LineTo(114.300781, 225.773438);
            cr.LineTo(123.523438, 225.773438);
            cr.ClosePath();
            cr.MoveTo(123.523438, 216.550781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(243.449219, 216.550781);
            cr.LineTo(234.226563, 216.550781);
            cr.LineTo(234.226563, 225.773438);
            cr.LineTo(243.449219, 225.773438);
            cr.ClosePath();
            cr.MoveTo(243.449219, 216.550781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(243.449219, 216.550781);
            cr.LineTo(234.226563, 216.550781);
            cr.LineTo(234.226563, 225.773438);
            cr.LineTo(243.449219, 225.773438);
            cr.ClosePath();
            cr.MoveTo(243.449219, 216.550781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132.75, 225.773438);
            cr.LineTo(123.523438, 225.773438);
            cr.LineTo(123.523438, 235);
            cr.LineTo(132.75, 235);
            cr.ClosePath();
            cr.MoveTo(132.75, 225.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132.75, 225.773438);
            cr.LineTo(123.523438, 225.773438);
            cr.LineTo(123.523438, 235);
            cr.LineTo(132.75, 235);
            cr.ClosePath();
            cr.MoveTo(132.75, 225.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(252.675781, 225.773438);
            cr.LineTo(243.449219, 225.773438);
            cr.LineTo(243.449219, 235);
            cr.LineTo(252.675781, 235);
            cr.ClosePath();
            cr.MoveTo(252.675781, 225.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(252.675781, 225.773438);
            cr.LineTo(243.449219, 225.773438);
            cr.LineTo(243.449219, 235);
            cr.LineTo(252.675781, 235);
            cr.ClosePath();
            cr.MoveTo(252.675781, 225.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(271.125, 225.773438);
            cr.LineTo(261.898438, 225.773438);
            cr.LineTo(261.898438, 235);
            cr.LineTo(271.125, 235);
            cr.ClosePath();
            cr.MoveTo(271.125, 225.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(271.125, 225.773438);
            cr.LineTo(261.898438, 225.773438);
            cr.LineTo(261.898438, 235);
            cr.LineTo(271.125, 235);
            cr.ClosePath();
            cr.MoveTo(271.125, 225.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.976563, 235);
            cr.LineTo(132.75, 235);
            cr.LineTo(132.75, 244.222656);
            cr.LineTo(141.976563, 244.222656);
            cr.ClosePath();
            cr.MoveTo(141.976563, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.976563, 235);
            cr.LineTo(132.75, 235);
            cr.LineTo(132.75, 244.222656);
            cr.LineTo(141.976563, 244.222656);
            cr.ClosePath();
            cr.MoveTo(141.976563, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(206.550781, 235);
            cr.LineTo(197.324219, 235);
            cr.LineTo(197.324219, 244.222656);
            cr.LineTo(206.550781, 244.222656);
            cr.ClosePath();
            cr.MoveTo(206.550781, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(206.550781, 235);
            cr.LineTo(197.324219, 235);
            cr.LineTo(197.324219, 244.222656);
            cr.LineTo(206.550781, 244.222656);
            cr.ClosePath();
            cr.MoveTo(206.550781, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(261.898438, 235);
            cr.LineTo(252.675781, 235);
            cr.LineTo(252.675781, 244.222656);
            cr.LineTo(261.898438, 244.222656);
            cr.ClosePath();
            cr.MoveTo(261.898438, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(261.898438, 235);
            cr.LineTo(252.675781, 235);
            cr.LineTo(252.675781, 244.222656);
            cr.LineTo(261.898438, 244.222656);
            cr.ClosePath();
            cr.MoveTo(261.898438, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(280.351563, 235);
            cr.LineTo(271.125, 235);
            cr.LineTo(271.125, 244.222656);
            cr.LineTo(280.351563, 244.222656);
            cr.ClosePath();
            cr.MoveTo(280.351563, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(280.351563, 235);
            cr.LineTo(271.125, 235);
            cr.LineTo(271.125, 244.222656);
            cr.LineTo(280.351563, 244.222656);
            cr.ClosePath();
            cr.MoveTo(280.351563, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 244.222656);
            cr.LineTo(206.550781, 244.222656);
            cr.LineTo(206.550781, 253.449219);
            cr.LineTo(215.773438, 253.449219);
            cr.LineTo(215.773438, 262.675781);
            cr.LineTo(225, 262.675781);
            cr.ClosePath();
            cr.MoveTo(225, 244.222656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.875, 253.449219);
            cr.LineTo(169.648438, 253.449219);
            cr.LineTo(169.648438, 262.675781);
            cr.LineTo(178.875, 262.675781);
            cr.ClosePath();
            cr.MoveTo(178.875, 253.449219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.875, 253.449219);
            cr.LineTo(169.648438, 253.449219);
            cr.LineTo(169.648438, 262.675781);
            cr.LineTo(178.875, 262.675781);
            cr.ClosePath();
            cr.MoveTo(178.875, 253.449219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(243.449219, 253.449219);
            cr.LineTo(234.226563, 253.449219);
            cr.LineTo(234.226563, 262.675781);
            cr.LineTo(243.449219, 262.675781);
            cr.ClosePath();
            cr.MoveTo(243.449219, 253.449219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(243.449219, 253.449219);
            cr.LineTo(234.226563, 253.449219);
            cr.LineTo(234.226563, 262.675781);
            cr.LineTo(243.449219, 262.675781);
            cr.ClosePath();
            cr.MoveTo(243.449219, 253.449219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(160.425781, 262.675781);
            cr.LineTo(151.199219, 262.675781);
            cr.LineTo(151.199219, 271.898438);
            cr.LineTo(160.425781, 271.898438);
            cr.ClosePath();
            cr.MoveTo(160.425781, 262.675781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(160.425781, 262.675781);
            cr.LineTo(151.199219, 262.675781);
            cr.LineTo(151.199219, 271.898438);
            cr.LineTo(160.425781, 271.898438);
            cr.ClosePath();
            cr.MoveTo(160.425781, 262.675781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(188.101563, 262.675781);
            cr.LineTo(178.875, 262.675781);
            cr.LineTo(178.875, 271.898438);
            cr.LineTo(188.101563, 271.898438);
            cr.ClosePath();
            cr.MoveTo(188.101563, 262.675781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(188.101563, 262.675781);
            cr.LineTo(178.875, 262.675781);
            cr.LineTo(178.875, 271.898438);
            cr.LineTo(188.101563, 271.898438);
            cr.ClosePath();
            cr.MoveTo(188.101563, 262.675781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(151.199219, 271.898438);
            cr.LineTo(141.976563, 271.898438);
            cr.LineTo(141.976563, 281.125);
            cr.LineTo(151.199219, 281.125);
            cr.ClosePath();
            cr.MoveTo(151.199219, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(151.199219, 271.898438);
            cr.LineTo(141.976563, 271.898438);
            cr.LineTo(141.976563, 281.125);
            cr.LineTo(151.199219, 281.125);
            cr.ClosePath();
            cr.MoveTo(151.199219, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(169.648438, 271.898438);
            cr.LineTo(160.425781, 271.898438);
            cr.LineTo(160.425781, 281.125);
            cr.LineTo(169.648438, 281.125);
            cr.ClosePath();
            cr.MoveTo(169.648438, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(169.648438, 271.898438);
            cr.LineTo(160.425781, 271.898438);
            cr.LineTo(160.425781, 281.125);
            cr.LineTo(169.648438, 281.125);
            cr.ClosePath();
            cr.MoveTo(169.648438, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(197.324219, 271.898438);
            cr.LineTo(188.101563, 271.898438);
            cr.LineTo(188.101563, 281.125);
            cr.LineTo(197.324219, 281.125);
            cr.ClosePath();
            cr.MoveTo(197.324219, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(197.324219, 271.898438);
            cr.LineTo(188.101563, 271.898438);
            cr.LineTo(188.101563, 281.125);
            cr.LineTo(197.324219, 281.125);
            cr.ClosePath();
            cr.MoveTo(197.324219, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215.773438, 271.898438);
            cr.LineTo(206.550781, 271.898438);
            cr.LineTo(206.550781, 281.125);
            cr.LineTo(215.773438, 281.125);
            cr.ClosePath();
            cr.MoveTo(215.773438, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215.773438, 271.898438);
            cr.LineTo(206.550781, 271.898438);
            cr.LineTo(206.550781, 281.125);
            cr.LineTo(215.773438, 281.125);
            cr.ClosePath();
            cr.MoveTo(215.773438, 271.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.875, 281.125);
            cr.LineTo(169.648438, 281.125);
            cr.LineTo(169.648438, 290.347656);
            cr.LineTo(178.875, 290.347656);
            cr.ClosePath();
            cr.MoveTo(178.875, 281.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(178.875, 281.125);
            cr.LineTo(169.648438, 281.125);
            cr.LineTo(169.648438, 290.347656);
            cr.LineTo(178.875, 290.347656);
            cr.ClosePath();
            cr.MoveTo(178.875, 281.125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(188.101563, 290.347656);
            cr.LineTo(178.875, 290.347656);
            cr.LineTo(178.875, 299.574219);
            cr.LineTo(188.101563, 299.574219);
            cr.ClosePath();
            cr.MoveTo(188.101563, 290.347656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(188.101563, 290.347656);
            cr.LineTo(178.875, 290.347656);
            cr.LineTo(178.875, 299.574219);
            cr.LineTo(188.101563, 299.574219);
            cr.ClosePath();
            cr.MoveTo(188.101563, 290.347656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132.75, 299.574219);
            cr.LineTo(123.523438, 299.574219);
            cr.LineTo(123.523438, 308.800781);
            cr.LineTo(132.75, 308.800781);
            cr.ClosePath();
            cr.MoveTo(132.75, 299.574219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132.75, 299.574219);
            cr.LineTo(123.523438, 299.574219);
            cr.LineTo(123.523438, 308.800781);
            cr.LineTo(132.75, 308.800781);
            cr.ClosePath();
            cr.MoveTo(132.75, 299.574219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(114.300781, 308.800781);
            cr.LineTo(105.074219, 308.800781);
            cr.LineTo(105.074219, 318.023438);
            cr.LineTo(114.300781, 318.023438);
            cr.ClosePath();
            cr.MoveTo(114.300781, 308.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(114.300781, 308.800781);
            cr.LineTo(105.074219, 308.800781);
            cr.LineTo(105.074219, 318.023438);
            cr.LineTo(114.300781, 318.023438);
            cr.ClosePath();
            cr.MoveTo(114.300781, 308.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95.851563, 318.023438);
            cr.LineTo(86.625, 318.023438);
            cr.LineTo(86.625, 327.25);
            cr.LineTo(95.851563, 327.25);
            cr.ClosePath();
            cr.MoveTo(95.851563, 318.023438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95.851563, 318.023438);
            cr.LineTo(86.625, 318.023438);
            cr.LineTo(86.625, 327.25);
            cr.LineTo(95.851563, 327.25);
            cr.ClosePath();
            cr.MoveTo(95.851563, 318.023438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawdice_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 366;
            float h = 368;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(211.785156, 0);
            cr.CurveTo(210.726563, -0.0234375, 209.582031, 0.238281, 208.128906, 0.730469);
            cr.LineTo(47.214844, 47.980469);
            cr.CurveTo(45.875, 48.292969, 45.953125, 48.65625, 47.066406, 49.515625);
            cr.LineTo(189.910156, 163.546875);
            cr.CurveTo(192.539063, 165.726563, 193.777344, 166, 197.441406, 164.863281);
            cr.LineTo(357.617188, 109.351563);
            cr.CurveTo(361.289063, 108, 361.546875, 107.671875, 357.761719, 105.179688);
            cr.LineTo(217.183594, 2.347656);
            cr.CurveTo(215.011719, 0.820313, 213.535156, 0.0429688, 211.769531, 0.0078125);
            cr.ClosePath();
            cr.MoveTo(193.425781, 13.824219);
            cr.CurveTo(196.632813, 13.800781, 200.015625, 14.152344, 203.519531, 14.921875);
            cr.CurveTo(217.546875, 17.996094, 227.492188, 26.566406, 225.683594, 34.011719);
            cr.CurveTo(223.871094, 41.453125, 211.035156, 44.984375, 197.011719, 41.910156);
            cr.CurveTo(182.984375, 38.835938, 173.039063, 30.335938, 174.847656, 22.894531);
            cr.CurveTo(176.207031, 17.308594, 183.8125, 13.871094, 193.425781, 13.824219);
            cr.ClosePath();
            cr.MoveTo(102.734375, 41.398438);
            cr.CurveTo(105.9375, 41.375, 109.394531, 41.726563, 112.898438, 42.496094);
            cr.CurveTo(126.925781, 45.570313, 136.800781, 54.070313, 134.988281, 61.511719);
            cr.CurveTo(133.175781, 68.957031, 120.34375, 72.558594, 106.316406, 69.484375);
            cr.CurveTo(92.289063, 66.410156, 82.417969, 57.839844, 84.226563, 50.394531);
            cr.CurveTo(85.585938, 44.8125, 93.121094, 41.445313, 102.730469, 41.398438);
            cr.ClosePath();
            cr.MoveTo(240.160156, 48.417969);
            cr.CurveTo(243.363281, 48.394531, 246.820313, 48.820313, 250.328125, 49.589844);
            cr.CurveTo(264.351563, 52.664063, 274.226563, 61.164063, 272.414063, 68.605469);
            cr.CurveTo(270.605469, 76.050781, 257.769531, 79.582031, 243.742188, 76.503906);
            cr.CurveTo(229.71875, 73.429688, 219.84375, 64.933594, 221.65625, 57.488281);
            cr.CurveTo(223.015625, 51.90625, 230.546875, 48.46875, 240.160156, 48.417969);
            cr.ClosePath();
            cr.MoveTo(37.125, 57.269531);
            cr.CurveTo(36.832031, 57.34375, 36.738281, 57.804688, 36.542969, 58.734375);
            cr.LineTo(0.265625, 240.191406);
            cr.CurveTo(-0.160156, 242.078125, -0.0859375, 242.546875, 1.652344, 244.066406);
            cr.LineTo(139.453125, 362.851563);
            cr.CurveTo(142.65625, 365.914063, 143.328125, 365.875, 143.765625, 362.121094);
            cr.LineTo(183.921875, 178.101563);
            cr.CurveTo(184.761719, 175.59375, 183.894531, 174.214844, 182.457031, 173.054688);
            cr.LineTo(38.585938, 58.160156);
            cr.CurveTo(37.847656, 57.539063, 37.417969, 57.210938, 37.125, 57.285156);
            cr.ClosePath();
            cr.MoveTo(149.035156, 77.96875);
            cr.CurveTo(152.242188, 77.945313, 155.695313, 78.296875, 159.203125, 79.066406);
            cr.CurveTo(173.226563, 82.140625, 183.101563, 90.640625, 181.289063, 98.082031);
            cr.CurveTo(179.480469, 105.523438, 166.644531, 109.058594, 152.621094, 105.980469);
            cr.CurveTo(138.59375, 102.90625, 128.71875, 94.40625, 130.53125, 86.964844);
            cr.CurveTo(131.890625, 81.382813, 139.421875, 78.015625, 149.035156, 77.96875);
            cr.ClosePath();
            cr.MoveTo(287.933594, 84.039063);
            cr.CurveTo(291.136719, 84.015625, 294.59375, 84.367188, 298.101563, 85.136719);
            cr.CurveTo(312.125, 88.210938, 322, 96.710938, 320.1875, 104.152344);
            cr.CurveTo(318.378906, 111.597656, 305.542969, 115.199219, 291.519531, 112.125);
            cr.CurveTo(277.492188, 109.050781, 267.617188, 100.480469, 269.429688, 93.035156);
            cr.CurveTo(270.789063, 87.453125, 278.320313, 84.085938, 287.933594, 84.039063);
            cr.ClosePath();
            cr.MoveTo(46.054688, 90.695313);
            cr.CurveTo(46.59375, 90.640625, 47.183594, 90.664063, 47.734375, 90.695313);
            cr.CurveTo(54.335938, 91.042969, 61.484375, 97.230469, 65.65625, 107.152344);
            cr.CurveTo(71.21875, 120.378906, 69.363281, 135.226563, 61.558594, 140.285156);
            cr.CurveTo(53.753906, 145.339844, 42.910156, 138.738281, 37.351563, 125.507813);
            cr.CurveTo(31.789063, 112.28125, 33.640625, 97.433594, 41.445313, 92.375);
            cr.CurveTo(42.910156, 91.429688, 44.433594, 90.855469, 46.054688, 90.695313);
            cr.ClosePath();
            cr.MoveTo(196.800781, 114.539063);
            cr.CurveTo(200.003906, 114.515625, 203.460938, 114.867188, 206.964844, 115.636719);
            cr.CurveTo(220.992188, 118.710938, 230.867188, 127.207031, 229.054688, 134.652344);
            cr.CurveTo(227.242188, 142.09375, 214.410156, 145.628906, 200.382813, 142.550781);
            cr.CurveTo(186.355469, 139.476563, 176.484375, 130.976563, 178.292969, 123.535156);
            cr.CurveTo(179.652344, 117.953125, 187.1875, 114.585938, 196.796875, 114.539063);
            cr.ClosePath();
            cr.MoveTo(365.097656, 119.585938);
            cr.CurveTo(364.792969, 119.648438, 364.359375, 119.757813, 363.855469, 119.949219);
            cr.LineTo(199.359375, 177.011719);
            cr.CurveTo(196.007813, 178.390625, 196.109375, 178.953125, 195.554688, 181.472656);
            cr.LineTo(155.472656, 364.914063);
            cr.CurveTo(155.339844, 367.613281, 154.34375, 368.84375, 157.523438, 367.398438);
            cr.LineTo(319.605469, 306.839844);
            cr.CurveTo(322.75, 305.332031, 323.355469, 305.070313, 323.703125, 302.011719);
            cr.LineTo(365.757813, 121.132813);
            cr.CurveTo(366.105469, 119.859375, 366.023438, 119.40625, 365.101563, 119.597656);
            cr.ClosePath();
            cr.MoveTo(338.402344, 149.351563);
            cr.CurveTo(343.035156, 149.429688, 346.667969, 151.679688, 348.277344, 156.007813);
            cr.CurveTo(351.492188, 164.664063, 345.300781, 178.519531, 334.378906, 186.945313);
            cr.CurveTo(323.460938, 195.375, 311.996094, 195.164063, 308.78125, 186.507813);
            cr.CurveTo(305.566406, 177.851563, 311.757813, 164, 322.679688, 155.570313);
            cr.CurveTo(328.136719, 151.355469, 333.773438, 149.277344, 338.402344, 149.351563);
            cr.ClosePath();
            cr.MoveTo(139.023438, 165.222656);
            cr.CurveTo(145.707031, 165.394531, 152.996094, 171.675781, 157.234375, 181.753906);
            cr.CurveTo(162.796875, 194.984375, 161.015625, 209.828125, 153.210938, 214.886719);
            cr.CurveTo(145.40625, 219.945313, 134.5625, 213.269531, 129, 200.039063);
            cr.CurveTo(123.441406, 186.8125, 125.292969, 172.039063, 133.097656, 166.980469);
            cr.CurveTo(134.5625, 166.03125, 136.085938, 165.457031, 137.707031, 165.296875);
            cr.CurveTo(138.144531, 165.253906, 138.578125, 165.214844, 139.023438, 165.226563);
            cr.ClosePath();
            cr.MoveTo(80.289063, 186.433594);
            cr.CurveTo(86.972656, 186.605469, 94.265625, 192.890625, 98.503906, 202.964844);
            cr.CurveTo(104.066406, 216.195313, 102.285156, 231.039063, 94.480469, 236.097656);
            cr.CurveTo(86.675781, 241.15625, 75.832031, 234.480469, 70.269531, 221.25);
            cr.CurveTo(64.707031, 208.023438, 66.5625, 193.25, 74.367188, 188.191406);
            cr.CurveTo(75.828125, 187.242188, 77.351563, 186.667969, 78.972656, 186.507813);
            cr.CurveTo(79.414063, 186.464844, 79.84375, 186.425781, 80.289063, 186.4375);
            cr.ClosePath();
            cr.MoveTo(22.4375, 207.425781);
            cr.CurveTo(22.976563, 207.371094, 23.570313, 207.398438, 24.117188, 207.425781);
            cr.CurveTo(30.71875, 207.777344, 37.792969, 214.035156, 41.964844, 223.957031);
            cr.CurveTo(47.527344, 237.183594, 45.746094, 251.957031, 37.941406, 257.015625);
            cr.CurveTo(30.140625, 262.074219, 19.296875, 255.46875, 13.734375, 242.242188);
            cr.CurveTo(8.171875, 229.011719, 10.027344, 214.164063, 17.828125, 209.109375);
            cr.CurveTo(19.292969, 208.160156, 20.816406, 207.585938, 22.4375, 207.425781);
            cr.ClosePath();
            cr.MoveTo(276.675781, 224.980469);
            cr.CurveTo(281.207031, 225.128906, 284.820313, 227.375, 286.402344, 231.636719);
            cr.CurveTo(289.621094, 240.292969, 283.355469, 254.144531, 272.433594, 262.574219);
            cr.CurveTo(261.515625, 271, 250.050781, 270.789063, 246.835938, 262.132813);
            cr.CurveTo(243.621094, 253.480469, 249.882813, 239.625, 260.804688, 231.195313);
            cr.CurveTo(265.582031, 227.507813, 270.453125, 225.457031, 274.703125, 225.054688);
            cr.CurveTo(275.382813, 224.988281, 276.027344, 224.960938, 276.675781, 224.980469);
            cr.ClosePath();
            cr.MoveTo(112.328125, 283.710938);
            cr.CurveTo(112.871094, 283.660156, 113.460938, 283.683594, 114.011719, 283.710938);
            cr.CurveTo(120.613281, 284.0625, 127.761719, 290.246094, 131.933594, 300.167969);
            cr.CurveTo(137.496094, 313.398438, 135.640625, 328.242188, 127.835938, 333.300781);
            cr.CurveTo(120.03125, 338.359375, 109.261719, 331.753906, 103.699219, 318.527344);
            cr.CurveTo(98.136719, 305.296875, 99.917969, 290.453125, 107.722656, 285.394531);
            cr.CurveTo(109.1875, 284.445313, 110.710938, 283.871094, 112.332031, 283.710938);
            cr.ClosePath();
            cr.MoveTo(209.679688, 302.730469);
            cr.CurveTo(214.3125, 302.804688, 217.945313, 304.984375, 219.554688, 309.3125);
            cr.CurveTo(222.769531, 317.96875, 216.578125, 331.820313, 205.65625, 340.25);
            cr.CurveTo(194.738281, 348.675781, 183.273438, 348.539063, 180.058594, 339.882813);
            cr.CurveTo(176.84375, 331.226563, 183.035156, 317.375, 193.953125, 308.945313);
            cr.CurveTo(199.414063, 304.730469, 205.046875, 302.652344, 209.679688, 302.726563);
            cr.ClosePath();
            cr.MoveTo(209.679688, 302.730469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawright_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 18;
            float h = 36;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 3;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.492188, 32.417969);
            cr.LineTo(1.492188, 3.582031);
            cr.LineTo(15.910156, 18);
            cr.ClosePath();
            cr.MoveTo(1.492188, 32.417969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.994475, 0, 0, 0.994475, 0, 0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawleft_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 18;
            float h = 36;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 3;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(16.507813, 32.417969);
            cr.LineTo(16.507813, 3.582031);
            cr.LineTo(2.089844, 18);
            cr.ClosePath();
            cr.MoveTo(16.507813, 32.417969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.994475, 0, 0, 0.994475, 0, 0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void Drawnecklace_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(360, 100);
            cr.LineTo(270, 100);
            cr.LineTo(270, 110);
            cr.LineTo(230, 110);
            cr.LineTo(230, 120);
            cr.LineTo(210, 120);
            cr.LineTo(210, 130);
            cr.LineTo(190, 130);
            cr.LineTo(190, 140);
            cr.LineTo(170, 140);
            cr.LineTo(170, 150);
            cr.LineTo(160, 150);
            cr.LineTo(160, 160);
            cr.LineTo(150, 160);
            cr.LineTo(150, 170);
            cr.LineTo(140, 170);
            cr.LineTo(140, 190);
            cr.LineTo(130, 190);
            cr.LineTo(130, 210);
            cr.LineTo(120, 210);
            cr.LineTo(120, 250);
            cr.LineTo(110, 250);
            cr.LineTo(110, 270);
            cr.LineTo(90, 270);
            cr.LineTo(90, 280);
            cr.LineTo(70, 280);
            cr.LineTo(70, 300);
            cr.LineTo(60, 300);
            cr.LineTo(60, 330);
            cr.LineTo(70, 330);
            cr.LineTo(70, 340);
            cr.LineTo(80, 340);
            cr.LineTo(80, 350);
            cr.LineTo(100, 350);
            cr.LineTo(100, 340);
            cr.LineTo(120, 340);
            cr.LineTo(120, 330);
            cr.LineTo(130, 330);
            cr.LineTo(130, 290);
            cr.LineTo(140, 290);
            cr.LineTo(140, 270);
            cr.LineTo(180, 270);
            cr.LineTo(180, 260);
            cr.LineTo(250, 260);
            cr.LineTo(250, 250);
            cr.LineTo(290, 250);
            cr.LineTo(290, 240);
            cr.LineTo(310, 240);
            cr.LineTo(310, 230);
            cr.LineTo(340, 230);
            cr.LineTo(340, 220);
            cr.LineTo(360, 220);
            cr.LineTo(360, 210);
            cr.LineTo(370, 210);
            cr.LineTo(370, 200);
            cr.LineTo(380, 200);
            cr.LineTo(380, 190);
            cr.LineTo(390, 190);
            cr.LineTo(390, 140);
            cr.LineTo(380, 140);
            cr.LineTo(380, 120);
            cr.LineTo(370, 120);
            cr.LineTo(370, 110);
            cr.LineTo(360, 110);
            cr.ClosePath();
            cr.MoveTo(240, 130);
            cr.LineTo(280, 130);
            cr.LineTo(280, 120);
            cr.LineTo(350, 120);
            cr.LineTo(350, 130);
            cr.LineTo(360, 130);
            cr.LineTo(360, 140);
            cr.LineTo(370, 140);
            cr.LineTo(370, 180);
            cr.LineTo(360, 180);
            cr.LineTo(360, 190);
            cr.LineTo(340, 190);
            cr.LineTo(340, 200);
            cr.LineTo(330, 200);
            cr.LineTo(330, 210);
            cr.LineTo(300, 210);
            cr.LineTo(300, 220);
            cr.LineTo(280, 220);
            cr.LineTo(280, 230);
            cr.LineTo(240, 230);
            cr.LineTo(240, 240);
            cr.LineTo(180, 240);
            cr.LineTo(180, 250);
            cr.LineTo(150, 250);
            cr.LineTo(150, 240);
            cr.LineTo(140, 240);
            cr.LineTo(140, 210);
            cr.LineTo(150, 210);
            cr.LineTo(150, 190);
            cr.LineTo(160, 190);
            cr.LineTo(160, 180);
            cr.LineTo(170, 180);
            cr.LineTo(170, 170);
            cr.LineTo(180, 170);
            cr.LineTo(180, 160);
            cr.LineTo(200, 160);
            cr.LineTo(200, 150);
            cr.LineTo(220, 150);
            cr.LineTo(220, 140);
            cr.LineTo(240, 140);
            cr.ClosePath();
            cr.MoveTo(120, 260);
            cr.LineTo(130, 260);
            cr.LineTo(130, 270);
            cr.LineTo(120, 270);
            cr.ClosePath();
            cr.MoveTo(90, 290);
            cr.LineTo(110, 290);
            cr.LineTo(110, 320);
            cr.LineTo(100, 320);
            cr.LineTo(100, 330);
            cr.LineTo(90, 330);
            cr.LineTo(90, 320);
            cr.LineTo(80, 320);
            cr.LineTo(80, 310);
            cr.LineTo(90, 310);
            cr.ClosePath();
            cr.MoveTo(90, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100, 310);
            cr.LineTo(90, 310);
            cr.LineTo(90, 320);
            cr.LineTo(100, 320);
            cr.ClosePath();
            cr.MoveTo(100, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100, 310);
            cr.LineTo(90, 310);
            cr.LineTo(90, 320);
            cr.LineTo(100, 320);
            cr.ClosePath();
            cr.MoveTo(100, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawbasket_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 50);
            cr.LineTo(205, 50);
            cr.LineTo(205, 60);
            cr.LineTo(185, 60);
            cr.LineTo(185, 70);
            cr.LineTo(165, 70);
            cr.LineTo(165, 80);
            cr.LineTo(145, 80);
            cr.LineTo(145, 90);
            cr.LineTo(125, 90);
            cr.LineTo(125, 100);
            cr.LineTo(115, 100);
            cr.LineTo(115, 130);
            cr.LineTo(125, 130);
            cr.LineTo(125, 150);
            cr.LineTo(135, 150);
            cr.LineTo(135, 160);
            cr.LineTo(145, 160);
            cr.LineTo(145, 190);
            cr.LineTo(155, 190);
            cr.LineTo(155, 200);
            cr.LineTo(135, 200);
            cr.LineTo(135, 210);
            cr.LineTo(115, 210);
            cr.LineTo(115, 220);
            cr.LineTo(95, 220);
            cr.LineTo(95, 230);
            cr.LineTo(65, 230);
            cr.LineTo(65, 240);
            cr.LineTo(45, 240);
            cr.LineTo(45, 250);
            cr.LineTo(35, 250);
            cr.LineTo(35, 260);
            cr.LineTo(25, 260);
            cr.LineTo(25, 280);
            cr.LineTo(35, 280);
            cr.LineTo(35, 290);
            cr.LineTo(45, 290);
            cr.LineTo(45, 300);
            cr.LineTo(55, 300);
            cr.LineTo(55, 310);
            cr.LineTo(65, 310);
            cr.LineTo(65, 320);
            cr.LineTo(75, 320);
            cr.LineTo(75, 330);
            cr.LineTo(85, 330);
            cr.LineTo(85, 340);
            cr.LineTo(95, 340);
            cr.LineTo(95, 350);
            cr.LineTo(105, 350);
            cr.LineTo(105, 360);
            cr.LineTo(115, 360);
            cr.LineTo(115, 370);
            cr.LineTo(125, 370);
            cr.LineTo(125, 380);
            cr.LineTo(135, 380);
            cr.LineTo(135, 390);
            cr.LineTo(155, 390);
            cr.LineTo(155, 400);
            cr.LineTo(215, 400);
            cr.LineTo(215, 390);
            cr.LineTo(245, 390);
            cr.LineTo(245, 380);
            cr.LineTo(275, 380);
            cr.LineTo(275, 370);
            cr.LineTo(305, 370);
            cr.LineTo(305, 360);
            cr.LineTo(325, 360);
            cr.LineTo(325, 350);
            cr.LineTo(355, 350);
            cr.LineTo(355, 340);
            cr.LineTo(375, 340);
            cr.LineTo(375, 330);
            cr.LineTo(395, 330);
            cr.LineTo(395, 310);
            cr.LineTo(405, 310);
            cr.LineTo(405, 290);
            cr.LineTo(415, 290);
            cr.LineTo(415, 270);
            cr.LineTo(425, 270);
            cr.LineTo(425, 140);
            cr.LineTo(415, 140);
            cr.LineTo(415, 120);
            cr.LineTo(405, 120);
            cr.LineTo(405, 110);
            cr.LineTo(365, 110);
            cr.LineTo(365, 120);
            cr.LineTo(345, 120);
            cr.LineTo(345, 130);
            cr.LineTo(315, 130);
            cr.LineTo(315, 140);
            cr.LineTo(295, 140);
            cr.LineTo(295, 130);
            cr.LineTo(285, 130);
            cr.LineTo(285, 110);
            cr.LineTo(275, 110);
            cr.LineTo(275, 90);
            cr.LineTo(265, 90);
            cr.LineTo(265, 70);
            cr.LineTo(255, 70);
            cr.LineTo(255, 60);
            cr.LineTo(245, 60);
            cr.ClosePath();
            cr.MoveTo(215, 70);
            cr.LineTo(215, 60);
            cr.LineTo(225, 60);
            cr.LineTo(225, 70);
            cr.ClosePath();
            cr.MoveTo(195, 70);
            cr.LineTo(205, 70);
            cr.LineTo(205, 80);
            cr.LineTo(195, 80);
            cr.ClosePath();
            cr.MoveTo(195, 90);
            cr.LineTo(215, 90);
            cr.LineTo(215, 80);
            cr.LineTo(235, 80);
            cr.LineTo(235, 100);
            cr.LineTo(245, 100);
            cr.LineTo(245, 120);
            cr.LineTo(255, 120);
            cr.LineTo(255, 140);
            cr.LineTo(265, 140);
            cr.LineTo(265, 150);
            cr.LineTo(255, 150);
            cr.LineTo(255, 160);
            cr.LineTo(245, 160);
            cr.LineTo(245, 170);
            cr.LineTo(225, 170);
            cr.LineTo(225, 180);
            cr.LineTo(205, 180);
            cr.LineTo(205, 190);
            cr.LineTo(185, 190);
            cr.LineTo(185, 160);
            cr.LineTo(175, 160);
            cr.LineTo(175, 140);
            cr.LineTo(165, 140);
            cr.LineTo(165, 120);
            cr.LineTo(155, 120);
            cr.LineTo(155, 110);
            cr.LineTo(175, 110);
            cr.LineTo(175, 100);
            cr.LineTo(195, 100);
            cr.ClosePath();
            cr.MoveTo(175, 80);
            cr.LineTo(185, 80);
            cr.LineTo(185, 90);
            cr.LineTo(175, 90);
            cr.ClosePath();
            cr.MoveTo(155, 90);
            cr.LineTo(165, 90);
            cr.LineTo(165, 100);
            cr.LineTo(155, 100);
            cr.ClosePath();
            cr.MoveTo(135, 100);
            cr.LineTo(145, 100);
            cr.LineTo(145, 110);
            cr.LineTo(135, 110);
            cr.ClosePath();
            cr.MoveTo(145, 130);
            cr.LineTo(155, 130);
            cr.LineTo(155, 140);
            cr.LineTo(145, 140);
            cr.ClosePath();
            cr.MoveTo(155, 150);
            cr.LineTo(165, 150);
            cr.LineTo(165, 160);
            cr.LineTo(155, 160);
            cr.ClosePath();
            cr.MoveTo(265, 180);
            cr.LineTo(295, 180);
            cr.LineTo(295, 210);
            cr.LineTo(275, 210);
            cr.LineTo(275, 220);
            cr.LineTo(295, 220);
            cr.LineTo(295, 230);
            cr.LineTo(305, 230);
            cr.LineTo(305, 210);
            cr.LineTo(315, 210);
            cr.LineTo(315, 200);
            cr.LineTo(335, 200);
            cr.LineTo(335, 210);
            cr.LineTo(345, 210);
            cr.LineTo(345, 230);
            cr.LineTo(325, 230);
            cr.LineTo(325, 240);
            cr.LineTo(345, 240);
            cr.LineTo(345, 250);
            cr.LineTo(335, 250);
            cr.LineTo(335, 260);
            cr.LineTo(345, 260);
            cr.LineTo(345, 270);
            cr.LineTo(335, 270);
            cr.LineTo(335, 280);
            cr.LineTo(345, 280);
            cr.LineTo(345, 310);
            cr.LineTo(355, 310);
            cr.LineTo(355, 320);
            cr.LineTo(325, 320);
            cr.LineTo(325, 310);
            cr.LineTo(315, 310);
            cr.LineTo(315, 330);
            cr.LineTo(305, 330);
            cr.LineTo(305, 340);
            cr.LineTo(275, 340);
            cr.LineTo(275, 350);
            cr.LineTo(255, 350);
            cr.LineTo(255, 360);
            cr.LineTo(225, 360);
            cr.LineTo(225, 370);
            cr.LineTo(205, 370);
            cr.LineTo(205, 380);
            cr.LineTo(185, 380);
            cr.LineTo(185, 370);
            cr.LineTo(175, 370);
            cr.LineTo(175, 380);
            cr.LineTo(165, 380);
            cr.LineTo(165, 370);
            cr.LineTo(175, 370);
            cr.LineTo(175, 360);
            cr.LineTo(185, 360);
            cr.LineTo(185, 350);
            cr.LineTo(175, 350);
            cr.LineTo(175, 310);
            cr.LineTo(165, 310);
            cr.LineTo(165, 270);
            cr.LineTo(175, 270);
            cr.LineTo(175, 260);
            cr.LineTo(155, 260);
            cr.LineTo(155, 270);
            cr.LineTo(135, 270);
            cr.LineTo(135, 280);
            cr.LineTo(155, 280);
            cr.LineTo(155, 320);
            cr.LineTo(165, 320);
            cr.LineTo(165, 360);
            cr.LineTo(135, 360);
            cr.LineTo(135, 350);
            cr.LineTo(155, 350);
            cr.LineTo(155, 340);
            cr.LineTo(125, 340);
            cr.LineTo(125, 330);
            cr.LineTo(155, 330);
            cr.LineTo(155, 320);
            cr.LineTo(125, 320);
            cr.LineTo(125, 310);
            cr.LineTo(115, 310);
            cr.LineTo(115, 330);
            cr.LineTo(105, 330);
            cr.LineTo(105, 320);
            cr.LineTo(95, 320);
            cr.LineTo(95, 310);
            cr.LineTo(115, 310);
            cr.LineTo(115, 300);
            cr.LineTo(125, 300);
            cr.LineTo(125, 290);
            cr.LineTo(115, 290);
            cr.LineTo(115, 270);
            cr.LineTo(105, 270);
            cr.LineTo(105, 300);
            cr.LineTo(75, 300);
            cr.LineTo(75, 290);
            cr.LineTo(85, 290);
            cr.LineTo(85, 280);
            cr.LineTo(55, 280);
            cr.LineTo(55, 270);
            cr.LineTo(65, 270);
            cr.LineTo(65, 260);
            cr.LineTo(95, 260);
            cr.LineTo(95, 270);
            cr.LineTo(105, 270);
            cr.LineTo(105, 250);
            cr.LineTo(125, 250);
            cr.LineTo(125, 240);
            cr.LineTo(145, 240);
            cr.LineTo(145, 260);
            cr.LineTo(155, 260);
            cr.LineTo(155, 230);
            cr.LineTo(175, 230);
            cr.LineTo(175, 220);
            cr.LineTo(195, 220);
            cr.LineTo(195, 250);
            cr.LineTo(205, 250);
            cr.LineTo(205, 240);
            cr.LineTo(215, 240);
            cr.LineTo(215, 250);
            cr.LineTo(225, 250);
            cr.LineTo(225, 240);
            cr.LineTo(245, 240);
            cr.LineTo(245, 230);
            cr.LineTo(255, 230);
            cr.LineTo(255, 270);
            cr.LineTo(245, 270);
            cr.LineTo(245, 280);
            cr.LineTo(265, 280);
            cr.LineTo(265, 270);
            cr.LineTo(285, 270);
            cr.LineTo(285, 260);
            cr.LineTo(265, 260);
            cr.LineTo(265, 230);
            cr.LineTo(275, 230);
            cr.LineTo(275, 220);
            cr.LineTo(265, 220);
            cr.ClosePath();
            cr.MoveTo(165, 170);
            cr.LineTo(175, 170);
            cr.LineTo(175, 180);
            cr.LineTo(165, 180);
            cr.ClosePath();
            cr.MoveTo(245, 190);
            cr.LineTo(255, 190);
            cr.LineTo(255, 220);
            cr.LineTo(245, 220);
            cr.LineTo(245, 230);
            cr.LineTo(205, 230);
            cr.LineTo(205, 210);
            cr.LineTo(225, 210);
            cr.LineTo(225, 200);
            cr.LineTo(245, 200);
            cr.ClosePath();
            cr.MoveTo(305, 200);
            cr.LineTo(305, 170);
            cr.LineTo(325, 170);
            cr.LineTo(325, 160);
            cr.LineTo(335, 160);
            cr.LineTo(335, 170);
            cr.LineTo(325, 170);
            cr.LineTo(325, 180);
            cr.LineTo(335, 180);
            cr.LineTo(335, 190);
            cr.LineTo(315, 190);
            cr.LineTo(315, 200);
            cr.ClosePath();
            cr.MoveTo(345, 160);
            cr.LineTo(345, 150);
            cr.LineTo(355, 150);
            cr.LineTo(355, 160);
            cr.LineTo(365, 160);
            cr.LineTo(365, 180);
            cr.LineTo(345, 180);
            cr.LineTo(345, 170);
            cr.LineTo(355, 170);
            cr.LineTo(355, 160);
            cr.ClosePath();
            cr.MoveTo(375, 170);
            cr.LineTo(375, 150);
            cr.LineTo(395, 150);
            cr.LineTo(395, 160);
            cr.LineTo(385, 160);
            cr.LineTo(385, 170);
            cr.LineTo(395, 170);
            cr.LineTo(395, 180);
            cr.LineTo(385, 180);
            cr.LineTo(385, 190);
            cr.LineTo(395, 190);
            cr.LineTo(395, 200);
            cr.LineTo(385, 200);
            cr.LineTo(385, 210);
            cr.LineTo(395, 210);
            cr.LineTo(395, 220);
            cr.LineTo(385, 220);
            cr.LineTo(385, 230);
            cr.LineTo(395, 230);
            cr.LineTo(395, 250);
            cr.LineTo(385, 250);
            cr.LineTo(385, 260);
            cr.LineTo(395, 260);
            cr.LineTo(395, 270);
            cr.LineTo(385, 270);
            cr.LineTo(385, 260);
            cr.LineTo(375, 260);
            cr.LineTo(375, 240);
            cr.LineTo(385, 240);
            cr.LineTo(385, 230);
            cr.LineTo(375, 230);
            cr.LineTo(375, 220);
            cr.LineTo(385, 220);
            cr.LineTo(385, 210);
            cr.LineTo(375, 210);
            cr.LineTo(375, 200);
            cr.LineTo(385, 200);
            cr.LineTo(385, 190);
            cr.LineTo(375, 190);
            cr.LineTo(375, 180);
            cr.LineTo(385, 180);
            cr.LineTo(385, 170);
            cr.ClosePath();
            cr.MoveTo(345, 200);
            cr.LineTo(345, 190);
            cr.LineTo(365, 190);
            cr.LineTo(365, 200);
            cr.ClosePath();
            cr.MoveTo(355, 220);
            cr.LineTo(355, 210);
            cr.LineTo(365, 210);
            cr.LineTo(365, 220);
            cr.ClosePath();
            cr.MoveTo(355, 240);
            cr.LineTo(355, 230);
            cr.LineTo(365, 230);
            cr.LineTo(365, 240);
            cr.ClosePath();
            cr.MoveTo(355, 270);
            cr.LineTo(355, 250);
            cr.LineTo(365, 250);
            cr.LineTo(365, 270);
            cr.LineTo(375, 270);
            cr.LineTo(375, 280);
            cr.LineTo(365, 280);
            cr.LineTo(365, 290);
            cr.LineTo(375, 290);
            cr.LineTo(375, 310);
            cr.LineTo(365, 310);
            cr.LineTo(365, 290);
            cr.LineTo(355, 290);
            cr.LineTo(355, 280);
            cr.LineTo(365, 280);
            cr.LineTo(365, 270);
            cr.ClosePath();
            cr.MoveTo(355, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 210);
            cr.LineTo(325, 210);
            cr.LineTo(325, 220);
            cr.LineTo(335, 220);
            cr.ClosePath();
            cr.MoveTo(335, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 210);
            cr.LineTo(325, 210);
            cr.LineTo(325, 220);
            cr.LineTo(335, 220);
            cr.ClosePath();
            cr.MoveTo(335, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 230);
            cr.LineTo(305, 230);
            cr.LineTo(305, 250);
            cr.LineTo(285, 250);
            cr.LineTo(285, 260);
            cr.LineTo(305, 260);
            cr.LineTo(305, 290);
            cr.LineTo(295, 290);
            cr.LineTo(295, 300);
            cr.LineTo(305, 300);
            cr.LineTo(305, 310);
            cr.LineTo(315, 310);
            cr.LineTo(315, 290);
            cr.LineTo(335, 290);
            cr.LineTo(335, 280);
            cr.LineTo(315, 280);
            cr.LineTo(315, 250);
            cr.LineTo(325, 250);
            cr.LineTo(325, 240);
            cr.LineTo(315, 240);
            cr.ClosePath();
            cr.MoveTo(315, 230);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 250);
            cr.LineTo(175, 250);
            cr.LineTo(175, 260);
            cr.LineTo(195, 260);
            cr.ClosePath();
            cr.MoveTo(195, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 250);
            cr.LineTo(205, 250);
            cr.LineTo(205, 290);
            cr.LineTo(195, 290);
            cr.LineTo(195, 300);
            cr.LineTo(205, 300);
            cr.LineTo(205, 310);
            cr.LineTo(215, 310);
            cr.LineTo(215, 290);
            cr.LineTo(245, 290);
            cr.LineTo(245, 280);
            cr.LineTo(215, 280);
            cr.ClosePath();
            cr.MoveTo(215, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 270);
            cr.LineTo(85, 270);
            cr.LineTo(85, 280);
            cr.LineTo(95, 280);
            cr.ClosePath();
            cr.MoveTo(95, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 270);
            cr.LineTo(85, 270);
            cr.LineTo(85, 280);
            cr.LineTo(95, 280);
            cr.ClosePath();
            cr.MoveTo(95, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 280);
            cr.LineTo(125, 280);
            cr.LineTo(125, 290);
            cr.LineTo(135, 290);
            cr.ClosePath();
            cr.MoveTo(135, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 280);
            cr.LineTo(125, 280);
            cr.LineTo(125, 290);
            cr.LineTo(135, 290);
            cr.ClosePath();
            cr.MoveTo(135, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 280);
            cr.LineTo(265, 280);
            cr.LineTo(265, 310);
            cr.LineTo(255, 310);
            cr.LineTo(255, 320);
            cr.LineTo(265, 320);
            cr.LineTo(265, 340);
            cr.LineTo(275, 340);
            cr.LineTo(275, 310);
            cr.LineTo(295, 310);
            cr.LineTo(295, 300);
            cr.LineTo(275, 300);
            cr.ClosePath();
            cr.MoveTo(275, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 300);
            cr.LineTo(175, 300);
            cr.LineTo(175, 310);
            cr.LineTo(195, 310);
            cr.ClosePath();
            cr.MoveTo(195, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 310);
            cr.LineTo(215, 310);
            cr.LineTo(215, 330);
            cr.LineTo(205, 330);
            cr.LineTo(205, 340);
            cr.LineTo(215, 340);
            cr.LineTo(215, 360);
            cr.LineTo(225, 360);
            cr.LineTo(225, 340);
            cr.LineTo(235, 340);
            cr.LineTo(235, 330);
            cr.LineTo(225, 330);
            cr.ClosePath();
            cr.MoveTo(225, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 320);
            cr.LineTo(235, 320);
            cr.LineTo(235, 330);
            cr.LineTo(255, 330);
            cr.ClosePath();
            cr.MoveTo(255, 320);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 340);
            cr.LineTo(185, 340);
            cr.LineTo(185, 350);
            cr.LineTo(205, 350);
            cr.ClosePath();
            cr.MoveTo(205, 340);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawbelt_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 39);
            cr.LineTo(305, 39);
            cr.LineTo(305, 49);
            cr.LineTo(295, 49);
            cr.LineTo(295, 69);
            cr.LineTo(285, 69);
            cr.LineTo(285, 89);
            cr.LineTo(275, 89);
            cr.LineTo(275, 109);
            cr.LineTo(265, 109);
            cr.LineTo(265, 129);
            cr.LineTo(255, 129);
            cr.LineTo(255, 139);
            cr.LineTo(245, 139);
            cr.LineTo(245, 149);
            cr.LineTo(215, 149);
            cr.LineTo(215, 159);
            cr.LineTo(185, 159);
            cr.LineTo(185, 169);
            cr.LineTo(155, 169);
            cr.LineTo(155, 179);
            cr.LineTo(135, 179);
            cr.LineTo(135, 189);
            cr.LineTo(115, 189);
            cr.LineTo(115, 199);
            cr.LineTo(95, 199);
            cr.LineTo(95, 209);
            cr.LineTo(75, 209);
            cr.LineTo(75, 219);
            cr.LineTo(65, 219);
            cr.LineTo(65, 229);
            cr.LineTo(55, 229);
            cr.LineTo(55, 239);
            cr.LineTo(45, 239);
            cr.LineTo(45, 249);
            cr.LineTo(35, 249);
            cr.LineTo(35, 259);
            cr.LineTo(25, 259);
            cr.LineTo(25, 279);
            cr.LineTo(15, 279);
            cr.LineTo(15, 359);
            cr.LineTo(25, 359);
            cr.LineTo(25, 379);
            cr.LineTo(35, 379);
            cr.LineTo(35, 389);
            cr.LineTo(45, 389);
            cr.LineTo(45, 399);
            cr.LineTo(65, 399);
            cr.LineTo(65, 409);
            cr.LineTo(115, 409);
            cr.LineTo(115, 399);
            cr.LineTo(145, 399);
            cr.LineTo(145, 389);
            cr.LineTo(155, 389);
            cr.LineTo(155, 379);
            cr.LineTo(175, 379);
            cr.LineTo(175, 369);
            cr.LineTo(185, 369);
            cr.LineTo(185, 359);
            cr.LineTo(195, 359);
            cr.LineTo(195, 349);
            cr.LineTo(205, 349);
            cr.LineTo(205, 339);
            cr.LineTo(215, 339);
            cr.LineTo(215, 329);
            cr.LineTo(225, 329);
            cr.LineTo(225, 319);
            cr.LineTo(235, 319);
            cr.LineTo(235, 309);
            cr.LineTo(245, 309);
            cr.LineTo(245, 289);
            cr.LineTo(255, 289);
            cr.LineTo(255, 279);
            cr.LineTo(265, 279);
            cr.LineTo(265, 269);
            cr.LineTo(275, 269);
            cr.LineTo(275, 249);
            cr.LineTo(285, 249);
            cr.LineTo(285, 239);
            cr.LineTo(295, 239);
            cr.LineTo(295, 229);
            cr.LineTo(325, 229);
            cr.LineTo(325, 239);
            cr.LineTo(315, 239);
            cr.LineTo(315, 249);
            cr.LineTo(305, 249);
            cr.LineTo(305, 259);
            cr.LineTo(295, 259);
            cr.LineTo(295, 269);
            cr.LineTo(285, 269);
            cr.LineTo(285, 279);
            cr.LineTo(275, 279);
            cr.LineTo(275, 309);
            cr.LineTo(285, 309);
            cr.LineTo(285, 319);
            cr.LineTo(305, 319);
            cr.LineTo(305, 329);
            cr.LineTo(335, 329);
            cr.LineTo(335, 319);
            cr.LineTo(355, 319);
            cr.LineTo(355, 309);
            cr.LineTo(365, 309);
            cr.LineTo(365, 299);
            cr.LineTo(375, 299);
            cr.LineTo(375, 289);
            cr.LineTo(385, 289);
            cr.LineTo(385, 279);
            cr.LineTo(395, 279);
            cr.LineTo(395, 269);
            cr.LineTo(405, 269);
            cr.LineTo(405, 259);
            cr.LineTo(415, 259);
            cr.LineTo(415, 249);
            cr.LineTo(425, 249);
            cr.LineTo(425, 239);
            cr.LineTo(435, 239);
            cr.LineTo(435, 179);
            cr.LineTo(425, 179);
            cr.LineTo(425, 169);
            cr.LineTo(415, 169);
            cr.LineTo(415, 159);
            cr.LineTo(335, 159);
            cr.LineTo(335, 149);
            cr.LineTo(345, 149);
            cr.LineTo(345, 129);
            cr.LineTo(355, 129);
            cr.LineTo(355, 119);
            cr.LineTo(365, 119);
            cr.LineTo(365, 109);
            cr.LineTo(375, 109);
            cr.LineTo(375, 79);
            cr.LineTo(365, 79);
            cr.LineTo(365, 69);
            cr.LineTo(355, 69);
            cr.LineTo(355, 59);
            cr.LineTo(345, 59);
            cr.LineTo(345, 49);
            cr.LineTo(335, 49);
            cr.ClosePath();
            cr.MoveTo(305, 69);
            cr.LineTo(315, 69);
            cr.LineTo(315, 59);
            cr.LineTo(325, 59);
            cr.LineTo(325, 69);
            cr.LineTo(335, 69);
            cr.LineTo(335, 79);
            cr.LineTo(345, 79);
            cr.LineTo(345, 89);
            cr.LineTo(355, 89);
            cr.LineTo(355, 99);
            cr.LineTo(345, 99);
            cr.LineTo(345, 109);
            cr.LineTo(325, 109);
            cr.LineTo(325, 99);
            cr.LineTo(335, 99);
            cr.LineTo(335, 79);
            cr.LineTo(325, 79);
            cr.LineTo(325, 89);
            cr.LineTo(305, 89);
            cr.ClosePath();
            cr.MoveTo(295, 109);
            cr.LineTo(305, 109);
            cr.LineTo(305, 119);
            cr.LineTo(315, 119);
            cr.LineTo(315, 129);
            cr.LineTo(325, 129);
            cr.LineTo(325, 139);
            cr.LineTo(315, 139);
            cr.LineTo(315, 129);
            cr.LineTo(305, 129);
            cr.LineTo(305, 119);
            cr.LineTo(295, 119);
            cr.ClosePath();
            cr.MoveTo(285, 129);
            cr.LineTo(295, 129);
            cr.LineTo(295, 139);
            cr.LineTo(305, 139);
            cr.LineTo(305, 149);
            cr.LineTo(315, 149);
            cr.LineTo(315, 159);
            cr.LineTo(305, 159);
            cr.LineTo(305, 169);
            cr.LineTo(315, 169);
            cr.LineTo(315, 179);
            cr.LineTo(305, 179);
            cr.LineTo(305, 189);
            cr.LineTo(295, 189);
            cr.LineTo(295, 209);
            cr.LineTo(285, 209);
            cr.LineTo(285, 219);
            cr.LineTo(275, 219);
            cr.LineTo(275, 229);
            cr.LineTo(265, 229);
            cr.LineTo(265, 249);
            cr.LineTo(255, 249);
            cr.LineTo(255, 259);
            cr.LineTo(245, 259);
            cr.LineTo(245, 269);
            cr.LineTo(235, 269);
            cr.LineTo(235, 289);
            cr.LineTo(225, 289);
            cr.LineTo(225, 299);
            cr.LineTo(215, 299);
            cr.LineTo(215, 309);
            cr.LineTo(205, 309);
            cr.LineTo(205, 319);
            cr.LineTo(195, 319);
            cr.LineTo(195, 329);
            cr.LineTo(185, 329);
            cr.LineTo(185, 339);
            cr.LineTo(175, 339);
            cr.LineTo(175, 349);
            cr.LineTo(165, 349);
            cr.LineTo(165, 359);
            cr.LineTo(155, 359);
            cr.LineTo(155, 369);
            cr.LineTo(135, 369);
            cr.LineTo(135, 379);
            cr.LineTo(115, 379);
            cr.LineTo(115, 389);
            cr.LineTo(85, 389);
            cr.LineTo(85, 379);
            cr.LineTo(65, 379);
            cr.LineTo(65, 369);
            cr.LineTo(55, 369);
            cr.LineTo(55, 349);
            cr.LineTo(85, 349);
            cr.LineTo(85, 339);
            cr.LineTo(115, 339);
            cr.LineTo(115, 329);
            cr.LineTo(135, 329);
            cr.LineTo(135, 319);
            cr.LineTo(145, 319);
            cr.LineTo(145, 309);
            cr.LineTo(155, 309);
            cr.LineTo(155, 299);
            cr.LineTo(165, 299);
            cr.LineTo(165, 289);
            cr.LineTo(175, 289);
            cr.LineTo(175, 279);
            cr.LineTo(185, 279);
            cr.LineTo(185, 269);
            cr.LineTo(195, 269);
            cr.LineTo(195, 259);
            cr.LineTo(205, 259);
            cr.LineTo(205, 239);
            cr.LineTo(215, 239);
            cr.LineTo(215, 229);
            cr.LineTo(225, 229);
            cr.LineTo(225, 209);
            cr.LineTo(235, 209);
            cr.LineTo(235, 199);
            cr.LineTo(245, 199);
            cr.LineTo(245, 179);
            cr.LineTo(255, 179);
            cr.LineTo(255, 169);
            cr.LineTo(265, 169);
            cr.LineTo(265, 149);
            cr.LineTo(275, 149);
            cr.LineTo(275, 139);
            cr.LineTo(285, 139);
            cr.ClosePath();
            cr.MoveTo(335, 189);
            cr.LineTo(335, 179);
            cr.LineTo(345, 179);
            cr.LineTo(345, 189);
            cr.ClosePath();
            cr.MoveTo(315, 199);
            cr.LineTo(325, 199);
            cr.LineTo(325, 209);
            cr.LineTo(315, 209);
            cr.ClosePath();
            cr.MoveTo(335, 209);
            cr.LineTo(335, 199);
            cr.LineTo(345, 199);
            cr.LineTo(345, 209);
            cr.ClosePath();
            cr.MoveTo(355, 189);
            cr.LineTo(355, 179);
            cr.LineTo(365, 179);
            cr.LineTo(365, 189);
            cr.ClosePath();
            cr.MoveTo(185, 189);
            cr.LineTo(215, 189);
            cr.LineTo(215, 199);
            cr.LineTo(195, 199);
            cr.LineTo(195, 209);
            cr.LineTo(205, 209);
            cr.LineTo(205, 219);
            cr.LineTo(195, 219);
            cr.LineTo(195, 229);
            cr.LineTo(175, 229);
            cr.LineTo(175, 249);
            cr.LineTo(155, 249);
            cr.LineTo(155, 259);
            cr.LineTo(135, 259);
            cr.LineTo(135, 269);
            cr.LineTo(125, 269);
            cr.LineTo(125, 279);
            cr.LineTo(115, 279);
            cr.LineTo(115, 289);
            cr.LineTo(105, 289);
            cr.LineTo(105, 299);
            cr.LineTo(95, 299);
            cr.LineTo(95, 309);
            cr.LineTo(75, 309);
            cr.LineTo(75, 319);
            cr.LineTo(65, 319);
            cr.LineTo(65, 309);
            cr.LineTo(55, 309);
            cr.LineTo(55, 299);
            cr.LineTo(45, 299);
            cr.LineTo(45, 289);
            cr.LineTo(55, 289);
            cr.LineTo(55, 269);
            cr.LineTo(65, 269);
            cr.LineTo(65, 259);
            cr.LineTo(75, 259);
            cr.LineTo(75, 249);
            cr.LineTo(85, 249);
            cr.LineTo(85, 239);
            cr.LineTo(105, 239);
            cr.LineTo(105, 229);
            cr.LineTo(125, 229);
            cr.LineTo(125, 219);
            cr.LineTo(145, 219);
            cr.LineTo(145, 209);
            cr.LineTo(155, 209);
            cr.LineTo(155, 199);
            cr.LineTo(185, 199);
            cr.ClosePath();
            cr.MoveTo(325, 259);
            cr.LineTo(335, 259);
            cr.LineTo(335, 249);
            cr.LineTo(345, 249);
            cr.LineTo(345, 239);
            cr.LineTo(355, 239);
            cr.LineTo(355, 229);
            cr.LineTo(365, 229);
            cr.LineTo(365, 219);
            cr.LineTo(375, 219);
            cr.LineTo(375, 209);
            cr.LineTo(385, 209);
            cr.LineTo(385, 199);
            cr.LineTo(395, 199);
            cr.LineTo(395, 189);
            cr.LineTo(405, 189);
            cr.LineTo(405, 199);
            cr.LineTo(415, 199);
            cr.LineTo(415, 209);
            cr.LineTo(405, 209);
            cr.LineTo(405, 219);
            cr.LineTo(415, 219);
            cr.LineTo(415, 229);
            cr.LineTo(405, 229);
            cr.LineTo(405, 249);
            cr.LineTo(395, 249);
            cr.LineTo(395, 259);
            cr.LineTo(385, 259);
            cr.LineTo(385, 269);
            cr.LineTo(375, 269);
            cr.LineTo(375, 279);
            cr.LineTo(365, 279);
            cr.LineTo(365, 289);
            cr.LineTo(355, 289);
            cr.LineTo(355, 299);
            cr.LineTo(335, 299);
            cr.LineTo(335, 309);
            cr.LineTo(305, 309);
            cr.LineTo(305, 299);
            cr.LineTo(295, 299);
            cr.LineTo(295, 289);
            cr.LineTo(305, 289);
            cr.LineTo(305, 269);
            cr.LineTo(325, 269);
            cr.ClosePath();
            cr.MoveTo(35, 329);
            cr.LineTo(45, 329);
            cr.LineTo(45, 339);
            cr.LineTo(55, 339);
            cr.LineTo(55, 349);
            cr.LineTo(45, 349);
            cr.LineTo(45, 339);
            cr.LineTo(35, 339);
            cr.ClosePath();
            cr.MoveTo(35, 329);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 139);
            cr.LineTo(285, 139);
            cr.LineTo(285, 149);
            cr.LineTo(295, 149);
            cr.ClosePath();
            cr.MoveTo(295, 139);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 139);
            cr.LineTo(285, 139);
            cr.LineTo(285, 149);
            cr.LineTo(295, 149);
            cr.ClosePath();
            cr.MoveTo(295, 139);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 149);
            cr.LineTo(295, 149);
            cr.LineTo(295, 159);
            cr.LineTo(305, 159);
            cr.ClosePath();
            cr.MoveTo(305, 149);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 149);
            cr.LineTo(295, 149);
            cr.LineTo(295, 159);
            cr.LineTo(305, 159);
            cr.ClosePath();
            cr.MoveTo(305, 149);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(405, 199);
            cr.LineTo(395, 199);
            cr.LineTo(395, 209);
            cr.LineTo(405, 209);
            cr.ClosePath();
            cr.MoveTo(405, 199);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(405, 199);
            cr.LineTo(395, 199);
            cr.LineTo(395, 209);
            cr.LineTo(405, 209);
            cr.ClosePath();
            cr.MoveTo(405, 199);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 209);
            cr.LineTo(175, 209);
            cr.LineTo(175, 219);
            cr.LineTo(195, 219);
            cr.ClosePath();
            cr.MoveTo(195, 209);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(395, 209);
            cr.LineTo(385, 209);
            cr.LineTo(385, 219);
            cr.LineTo(395, 219);
            cr.ClosePath();
            cr.MoveTo(395, 209);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(395, 209);
            cr.LineTo(385, 209);
            cr.LineTo(385, 219);
            cr.LineTo(395, 219);
            cr.ClosePath();
            cr.MoveTo(395, 209);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(405, 219);
            cr.LineTo(395, 219);
            cr.LineTo(395, 229);
            cr.LineTo(405, 229);
            cr.ClosePath();
            cr.MoveTo(405, 219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(405, 219);
            cr.LineTo(395, 219);
            cr.LineTo(395, 229);
            cr.LineTo(405, 229);
            cr.ClosePath();
            cr.MoveTo(405, 219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(385, 229);
            cr.LineTo(375, 229);
            cr.LineTo(375, 239);
            cr.LineTo(385, 239);
            cr.ClosePath();
            cr.MoveTo(385, 229);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(385, 229);
            cr.LineTo(375, 229);
            cr.LineTo(375, 239);
            cr.LineTo(385, 239);
            cr.ClosePath();
            cr.MoveTo(385, 229);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 249);
            cr.LineTo(355, 249);
            cr.LineTo(355, 259);
            cr.LineTo(365, 259);
            cr.ClosePath();
            cr.MoveTo(365, 249);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 249);
            cr.LineTo(355, 249);
            cr.LineTo(355, 259);
            cr.LineTo(365, 259);
            cr.ClosePath();
            cr.MoveTo(365, 249);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 269);
            cr.LineTo(335, 269);
            cr.LineTo(335, 279);
            cr.LineTo(345, 279);
            cr.ClosePath();
            cr.MoveTo(345, 269);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 269);
            cr.LineTo(335, 269);
            cr.LineTo(335, 279);
            cr.LineTo(345, 279);
            cr.ClosePath();
            cr.MoveTo(345, 269);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(75, 279);
            cr.LineTo(65, 279);
            cr.LineTo(65, 289);
            cr.LineTo(75, 289);
            cr.ClosePath();
            cr.MoveTo(75, 279);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(75, 279);
            cr.LineTo(65, 279);
            cr.LineTo(65, 289);
            cr.LineTo(75, 289);
            cr.ClosePath();
            cr.MoveTo(75, 279);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(65, 289);
            cr.LineTo(55, 289);
            cr.LineTo(55, 299);
            cr.LineTo(65, 299);
            cr.ClosePath();
            cr.MoveTo(65, 289);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(65, 289);
            cr.LineTo(55, 289);
            cr.LineTo(55, 299);
            cr.LineTo(65, 299);
            cr.ClosePath();
            cr.MoveTo(65, 289);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 289);
            cr.LineTo(75, 289);
            cr.LineTo(75, 299);
            cr.LineTo(85, 299);
            cr.ClosePath();
            cr.MoveTo(85, 289);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 289);
            cr.LineTo(75, 289);
            cr.LineTo(75, 299);
            cr.LineTo(85, 299);
            cr.ClosePath();
            cr.MoveTo(85, 289);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(325, 289);
            cr.LineTo(315, 289);
            cr.LineTo(315, 299);
            cr.LineTo(325, 299);
            cr.ClosePath();
            cr.MoveTo(325, 289);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(325, 289);
            cr.LineTo(315, 289);
            cr.LineTo(315, 299);
            cr.LineTo(325, 299);
            cr.ClosePath();
            cr.MoveTo(325, 289);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(75, 299);
            cr.LineTo(65, 299);
            cr.LineTo(65, 309);
            cr.LineTo(75, 309);
            cr.ClosePath();
            cr.MoveTo(75, 299);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(75, 299);
            cr.LineTo(65, 299);
            cr.LineTo(65, 309);
            cr.LineTo(75, 309);
            cr.ClosePath();
            cr.MoveTo(75, 299);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 349);
            cr.LineTo(85, 349);
            cr.LineTo(85, 359);
            cr.LineTo(95, 359);
            cr.ClosePath();
            cr.MoveTo(95, 349);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 349);
            cr.LineTo(85, 349);
            cr.LineTo(85, 359);
            cr.LineTo(95, 359);
            cr.ClosePath();
            cr.MoveTo(95, 349);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 359);
            cr.LineTo(65, 359);
            cr.LineTo(65, 369);
            cr.LineTo(85, 369);
            cr.ClosePath();
            cr.MoveTo(85, 359);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 359);
            cr.LineTo(105, 359);
            cr.LineTo(105, 369);
            cr.LineTo(115, 369);
            cr.ClosePath();
            cr.MoveTo(115, 359);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 359);
            cr.LineTo(105, 359);
            cr.LineTo(105, 369);
            cr.LineTo(115, 369);
            cr.ClosePath();
            cr.MoveTo(115, 359);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 369);
            cr.LineTo(85, 369);
            cr.LineTo(85, 379);
            cr.LineTo(105, 379);
            cr.ClosePath();
            cr.MoveTo(105, 369);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void Drawmedal_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 64);
            cr.LineTo(335, 64);
            cr.LineTo(335, 74);
            cr.LineTo(365, 74);
            cr.ClosePath();
            cr.MoveTo(365, 64);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 74);
            cr.LineTo(305, 74);
            cr.LineTo(305, 84);
            cr.LineTo(335, 84);
            cr.ClosePath();
            cr.MoveTo(335, 74);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(385, 74);
            cr.LineTo(365, 74);
            cr.LineTo(365, 84);
            cr.LineTo(375, 84);
            cr.LineTo(375, 94);
            cr.LineTo(385, 94);
            cr.LineTo(385, 124);
            cr.LineTo(395, 124);
            cr.LineTo(395, 134);
            cr.LineTo(385, 134);
            cr.LineTo(385, 154);
            cr.LineTo(375, 154);
            cr.LineTo(375, 164);
            cr.LineTo(365, 164);
            cr.LineTo(365, 174);
            cr.LineTo(315, 174);
            cr.LineTo(315, 164);
            cr.LineTo(305, 164);
            cr.LineTo(305, 154);
            cr.LineTo(295, 154);
            cr.LineTo(295, 94);
            cr.LineTo(285, 94);
            cr.LineTo(285, 114);
            cr.LineTo(275, 114);
            cr.LineTo(275, 124);
            cr.LineTo(245, 124);
            cr.LineTo(245, 134);
            cr.LineTo(265, 134);
            cr.LineTo(265, 144);
            cr.LineTo(245, 144);
            cr.LineTo(245, 154);
            cr.LineTo(215, 154);
            cr.LineTo(215, 164);
            cr.LineTo(195, 164);
            cr.LineTo(195, 174);
            cr.LineTo(185, 174);
            cr.LineTo(185, 164);
            cr.LineTo(175, 164);
            cr.LineTo(175, 174);
            cr.LineTo(165, 174);
            cr.LineTo(165, 184);
            cr.LineTo(145, 184);
            cr.LineTo(145, 194);
            cr.LineTo(135, 194);
            cr.LineTo(135, 204);
            cr.LineTo(115, 204);
            cr.LineTo(115, 214);
            cr.LineTo(105, 214);
            cr.LineTo(105, 194);
            cr.LineTo(85, 194);
            cr.LineTo(85, 184);
            cr.LineTo(75, 184);
            cr.LineTo(75, 174);
            cr.LineTo(45, 174);
            cr.LineTo(45, 184);
            cr.LineTo(35, 184);
            cr.LineTo(35, 194);
            cr.LineTo(55, 194);
            cr.LineTo(55, 204);
            cr.LineTo(85, 204);
            cr.LineTo(85, 214);
            cr.LineTo(95, 214);
            cr.LineTo(95, 224);
            cr.LineTo(85, 224);
            cr.LineTo(85, 244);
            cr.LineTo(105, 244);
            cr.LineTo(105, 234);
            cr.LineTo(115, 234);
            cr.LineTo(115, 224);
            cr.LineTo(135, 224);
            cr.LineTo(135, 214);
            cr.LineTo(145, 214);
            cr.LineTo(145, 204);
            cr.LineTo(165, 204);
            cr.LineTo(165, 194);
            cr.LineTo(195, 194);
            cr.LineTo(195, 184);
            cr.LineTo(215, 184);
            cr.LineTo(215, 174);
            cr.LineTo(245, 174);
            cr.LineTo(245, 164);
            cr.LineTo(255, 164);
            cr.LineTo(255, 174);
            cr.LineTo(265, 174);
            cr.LineTo(265, 164);
            cr.LineTo(275, 164);
            cr.LineTo(275, 174);
            cr.LineTo(285, 174);
            cr.LineTo(285, 184);
            cr.LineTo(275, 184);
            cr.LineTo(275, 194);
            cr.LineTo(265, 194);
            cr.LineTo(265, 184);
            cr.LineTo(255, 184);
            cr.LineTo(255, 204);
            cr.LineTo(245, 204);
            cr.LineTo(245, 214);
            cr.LineTo(225, 214);
            cr.LineTo(225, 224);
            cr.LineTo(215, 224);
            cr.LineTo(215, 234);
            cr.LineTo(205, 234);
            cr.LineTo(205, 224);
            cr.LineTo(195, 224);
            cr.LineTo(195, 234);
            cr.LineTo(175, 234);
            cr.LineTo(175, 244);
            cr.LineTo(185, 244);
            cr.LineTo(185, 254);
            cr.LineTo(175, 254);
            cr.LineTo(175, 264);
            cr.LineTo(165, 264);
            cr.LineTo(165, 274);
            cr.LineTo(155, 274);
            cr.LineTo(155, 284);
            cr.LineTo(145, 284);
            cr.LineTo(145, 294);
            cr.LineTo(135, 294);
            cr.LineTo(135, 304);
            cr.LineTo(125, 304);
            cr.LineTo(125, 314);
            cr.LineTo(115, 314);
            cr.LineTo(115, 324);
            cr.LineTo(105, 324);
            cr.LineTo(105, 334);
            cr.LineTo(95, 334);
            cr.LineTo(95, 344);
            cr.LineTo(85, 344);
            cr.LineTo(85, 304);
            cr.LineTo(95, 304);
            cr.LineTo(95, 284);
            cr.LineTo(75, 284);
            cr.LineTo(75, 274);
            cr.LineTo(55, 274);
            cr.LineTo(55, 264);
            cr.LineTo(45, 264);
            cr.LineTo(45, 274);
            cr.LineTo(35, 274);
            cr.LineTo(35, 284);
            cr.LineTo(45, 284);
            cr.LineTo(45, 294);
            cr.LineTo(75, 294);
            cr.LineTo(75, 304);
            cr.LineTo(65, 304);
            cr.LineTo(65, 344);
            cr.LineTo(55, 344);
            cr.LineTo(55, 374);
            cr.LineTo(65, 374);
            cr.LineTo(65, 384);
            cr.LineTo(75, 384);
            cr.LineTo(75, 374);
            cr.LineTo(85, 374);
            cr.LineTo(85, 364);
            cr.LineTo(95, 364);
            cr.LineTo(95, 354);
            cr.LineTo(105, 354);
            cr.LineTo(105, 344);
            cr.LineTo(115, 344);
            cr.LineTo(115, 334);
            cr.LineTo(125, 334);
            cr.LineTo(125, 324);
            cr.LineTo(135, 324);
            cr.LineTo(135, 314);
            cr.LineTo(145, 314);
            cr.LineTo(145, 304);
            cr.LineTo(155, 304);
            cr.LineTo(155, 294);
            cr.LineTo(165, 294);
            cr.LineTo(165, 284);
            cr.LineTo(175, 284);
            cr.LineTo(175, 274);
            cr.LineTo(185, 274);
            cr.LineTo(185, 264);
            cr.LineTo(195, 264);
            cr.LineTo(195, 254);
            cr.LineTo(215, 254);
            cr.LineTo(215, 244);
            cr.LineTo(225, 244);
            cr.LineTo(225, 234);
            cr.LineTo(245, 234);
            cr.LineTo(245, 224);
            cr.LineTo(255, 224);
            cr.LineTo(255, 214);
            cr.LineTo(275, 214);
            cr.LineTo(275, 204);
            cr.LineTo(285, 204);
            cr.LineTo(285, 194);
            cr.LineTo(325, 194);
            cr.LineTo(325, 204);
            cr.LineTo(355, 204);
            cr.LineTo(355, 194);
            cr.LineTo(375, 194);
            cr.LineTo(375, 184);
            cr.LineTo(385, 184);
            cr.LineTo(385, 174);
            cr.LineTo(395, 174);
            cr.LineTo(395, 164);
            cr.LineTo(405, 164);
            cr.LineTo(405, 144);
            cr.LineTo(415, 144);
            cr.LineTo(415, 114);
            cr.LineTo(405, 114);
            cr.LineTo(405, 94);
            cr.LineTo(395, 94);
            cr.LineTo(395, 84);
            cr.LineTo(385, 84);
            cr.ClosePath();
            cr.MoveTo(385, 74);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 84);
            cr.LineTo(295, 84);
            cr.LineTo(295, 94);
            cr.LineTo(305, 94);
            cr.ClosePath();
            cr.MoveTo(305, 84);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 84);
            cr.LineTo(295, 84);
            cr.LineTo(295, 94);
            cr.LineTo(305, 94);
            cr.ClosePath();
            cr.MoveTo(305, 84);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(355, 84);
            cr.LineTo(345, 84);
            cr.LineTo(345, 94);
            cr.LineTo(355, 94);
            cr.ClosePath();
            cr.MoveTo(355, 84);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(355, 84);
            cr.LineTo(345, 84);
            cr.LineTo(345, 94);
            cr.LineTo(355, 94);
            cr.ClosePath();
            cr.MoveTo(355, 84);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 94);
            cr.LineTo(315, 94);
            cr.LineTo(315, 104);
            cr.LineTo(335, 104);
            cr.ClosePath();
            cr.MoveTo(335, 94);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 94);
            cr.LineTo(355, 94);
            cr.LineTo(355, 104);
            cr.LineTo(365, 104);
            cr.ClosePath();
            cr.MoveTo(365, 94);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 94);
            cr.LineTo(355, 94);
            cr.LineTo(355, 104);
            cr.LineTo(365, 104);
            cr.ClosePath();
            cr.MoveTo(365, 94);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 104);
            cr.LineTo(305, 104);
            cr.LineTo(305, 144);
            cr.LineTo(315, 144);
            cr.ClosePath();
            cr.MoveTo(315, 104);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(375, 104);
            cr.LineTo(365, 104);
            cr.LineTo(365, 144);
            cr.LineTo(375, 144);
            cr.ClosePath();
            cr.MoveTo(375, 104);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 134);
            cr.LineTo(195, 134);
            cr.LineTo(195, 144);
            cr.LineTo(245, 144);
            cr.ClosePath();
            cr.MoveTo(245, 134);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 144);
            cr.LineTo(145, 144);
            cr.LineTo(145, 154);
            cr.LineTo(185, 154);
            cr.LineTo(185, 164);
            cr.LineTo(195, 164);
            cr.ClosePath();
            cr.MoveTo(195, 144);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(325, 144);
            cr.LineTo(315, 144);
            cr.LineTo(315, 164);
            cr.LineTo(355, 164);
            cr.LineTo(355, 154);
            cr.LineTo(325, 154);
            cr.ClosePath();
            cr.MoveTo(325, 144);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 144);
            cr.LineTo(355, 144);
            cr.LineTo(355, 154);
            cr.LineTo(365, 154);
            cr.ClosePath();
            cr.MoveTo(365, 144);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(365, 144);
            cr.LineTo(355, 144);
            cr.LineTo(355, 154);
            cr.LineTo(365, 154);
            cr.ClosePath();
            cr.MoveTo(365, 144);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 154);
            cr.LineTo(105, 154);
            cr.LineTo(105, 164);
            cr.LineTo(145, 164);
            cr.ClosePath();
            cr.MoveTo(145, 154);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 164);
            cr.LineTo(75, 164);
            cr.LineTo(75, 174);
            cr.LineTo(105, 174);
            cr.ClosePath();
            cr.MoveTo(105, 164);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 164);
            cr.LineTo(155, 164);
            cr.LineTo(155, 174);
            cr.LineTo(165, 174);
            cr.ClosePath();
            cr.MoveTo(165, 164);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 164);
            cr.LineTo(155, 164);
            cr.LineTo(155, 174);
            cr.LineTo(165, 174);
            cr.ClosePath();
            cr.MoveTo(165, 164);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 174);
            cr.LineTo(135, 174);
            cr.LineTo(135, 184);
            cr.LineTo(145, 184);
            cr.ClosePath();
            cr.MoveTo(145, 174);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 174);
            cr.LineTo(135, 174);
            cr.LineTo(135, 184);
            cr.LineTo(145, 184);
            cr.ClosePath();
            cr.MoveTo(145, 174);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 174);
            cr.LineTo(245, 174);
            cr.LineTo(245, 184);
            cr.LineTo(255, 184);
            cr.ClosePath();
            cr.MoveTo(255, 174);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 174);
            cr.LineTo(245, 174);
            cr.LineTo(245, 184);
            cr.LineTo(255, 184);
            cr.ClosePath();
            cr.MoveTo(255, 174);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 174);
            cr.LineTo(265, 174);
            cr.LineTo(265, 184);
            cr.LineTo(275, 184);
            cr.ClosePath();
            cr.MoveTo(275, 174);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 174);
            cr.LineTo(265, 174);
            cr.LineTo(265, 184);
            cr.LineTo(275, 184);
            cr.ClosePath();
            cr.MoveTo(275, 174);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 184);
            cr.LineTo(125, 184);
            cr.LineTo(125, 194);
            cr.LineTo(135, 194);
            cr.ClosePath();
            cr.MoveTo(135, 184);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 184);
            cr.LineTo(125, 184);
            cr.LineTo(125, 194);
            cr.LineTo(135, 194);
            cr.ClosePath();
            cr.MoveTo(135, 184);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 194);
            cr.LineTo(235, 194);
            cr.LineTo(235, 204);
            cr.LineTo(245, 204);
            cr.ClosePath();
            cr.MoveTo(245, 194);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 194);
            cr.LineTo(235, 194);
            cr.LineTo(235, 204);
            cr.LineTo(245, 204);
            cr.ClosePath();
            cr.MoveTo(245, 194);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 204);
            cr.LineTo(185, 204);
            cr.LineTo(185, 214);
            cr.LineTo(195, 214);
            cr.ClosePath();
            cr.MoveTo(195, 204);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 204);
            cr.LineTo(185, 204);
            cr.LineTo(185, 214);
            cr.LineTo(195, 214);
            cr.ClosePath();
            cr.MoveTo(195, 204);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 204);
            cr.LineTo(215, 204);
            cr.LineTo(215, 214);
            cr.LineTo(225, 214);
            cr.ClosePath();
            cr.MoveTo(225, 204);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 204);
            cr.LineTo(215, 204);
            cr.LineTo(215, 214);
            cr.LineTo(225, 214);
            cr.ClosePath();
            cr.MoveTo(225, 204);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 214);
            cr.LineTo(165, 214);
            cr.LineTo(165, 224);
            cr.LineTo(185, 224);
            cr.ClosePath();
            cr.MoveTo(185, 214);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 214);
            cr.LineTo(205, 214);
            cr.LineTo(205, 224);
            cr.LineTo(215, 224);
            cr.ClosePath();
            cr.MoveTo(215, 214);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 214);
            cr.LineTo(205, 214);
            cr.LineTo(205, 224);
            cr.LineTo(215, 224);
            cr.ClosePath();
            cr.MoveTo(215, 214);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 224);
            cr.LineTo(155, 224);
            cr.LineTo(155, 234);
            cr.LineTo(165, 234);
            cr.ClosePath();
            cr.MoveTo(165, 224);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 224);
            cr.LineTo(155, 224);
            cr.LineTo(155, 234);
            cr.LineTo(165, 234);
            cr.ClosePath();
            cr.MoveTo(165, 224);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 244);
            cr.LineTo(75, 244);
            cr.LineTo(75, 254);
            cr.LineTo(85, 254);
            cr.ClosePath();
            cr.MoveTo(85, 244);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 244);
            cr.LineTo(75, 244);
            cr.LineTo(75, 254);
            cr.LineTo(85, 254);
            cr.ClosePath();
            cr.MoveTo(85, 244);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 244);
            cr.LineTo(165, 244);
            cr.LineTo(165, 254);
            cr.LineTo(175, 254);
            cr.ClosePath();
            cr.MoveTo(175, 244);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 244);
            cr.LineTo(165, 244);
            cr.LineTo(165, 254);
            cr.LineTo(175, 254);
            cr.ClosePath();
            cr.MoveTo(175, 244);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(75, 254);
            cr.LineTo(55, 254);
            cr.LineTo(55, 264);
            cr.LineTo(75, 264);
            cr.ClosePath();
            cr.MoveTo(75, 254);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 254);
            cr.LineTo(155, 254);
            cr.LineTo(155, 264);
            cr.LineTo(165, 264);
            cr.ClosePath();
            cr.MoveTo(165, 254);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 254);
            cr.LineTo(155, 254);
            cr.LineTo(155, 264);
            cr.LineTo(165, 264);
            cr.ClosePath();
            cr.MoveTo(165, 254);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(155, 264);
            cr.LineTo(145, 264);
            cr.LineTo(145, 274);
            cr.LineTo(155, 274);
            cr.ClosePath();
            cr.MoveTo(155, 264);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(155, 264);
            cr.LineTo(145, 264);
            cr.LineTo(145, 274);
            cr.LineTo(155, 274);
            cr.ClosePath();
            cr.MoveTo(155, 264);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 274);
            cr.LineTo(135, 274);
            cr.LineTo(135, 284);
            cr.LineTo(145, 284);
            cr.ClosePath();
            cr.MoveTo(145, 274);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 274);
            cr.LineTo(135, 274);
            cr.LineTo(135, 284);
            cr.LineTo(145, 284);
            cr.ClosePath();
            cr.MoveTo(145, 274);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 284);
            cr.LineTo(125, 284);
            cr.LineTo(125, 294);
            cr.LineTo(135, 294);
            cr.ClosePath();
            cr.MoveTo(135, 284);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 284);
            cr.LineTo(125, 284);
            cr.LineTo(125, 294);
            cr.LineTo(135, 294);
            cr.ClosePath();
            cr.MoveTo(135, 284);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125, 294);
            cr.LineTo(115, 294);
            cr.LineTo(115, 304);
            cr.LineTo(125, 304);
            cr.ClosePath();
            cr.MoveTo(125, 294);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125, 294);
            cr.LineTo(115, 294);
            cr.LineTo(115, 304);
            cr.LineTo(125, 304);
            cr.ClosePath();
            cr.MoveTo(125, 294);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 304);
            cr.LineTo(105, 304);
            cr.LineTo(105, 314);
            cr.LineTo(115, 314);
            cr.ClosePath();
            cr.MoveTo(115, 304);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 304);
            cr.LineTo(105, 304);
            cr.LineTo(105, 314);
            cr.LineTo(115, 314);
            cr.ClosePath();
            cr.MoveTo(115, 304);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 314);
            cr.LineTo(95, 314);
            cr.LineTo(95, 324);
            cr.LineTo(105, 324);
            cr.ClosePath();
            cr.MoveTo(105, 314);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 314);
            cr.LineTo(95, 314);
            cr.LineTo(95, 324);
            cr.LineTo(105, 324);
            cr.ClosePath();
            cr.MoveTo(105, 314);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void Drawring_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(265, 110);
            cr.LineTo(205, 110);
            cr.LineTo(205, 120);
            cr.LineTo(175, 120);
            cr.LineTo(175, 130);
            cr.LineTo(155, 130);
            cr.LineTo(155, 140);
            cr.LineTo(135, 140);
            cr.LineTo(135, 150);
            cr.LineTo(125, 150);
            cr.LineTo(125, 160);
            cr.LineTo(115, 160);
            cr.LineTo(115, 170);
            cr.LineTo(105, 170);
            cr.LineTo(105, 180);
            cr.LineTo(95, 180);
            cr.LineTo(95, 190);
            cr.LineTo(85, 190);
            cr.LineTo(85, 210);
            cr.LineTo(75, 210);
            cr.LineTo(75, 230);
            cr.LineTo(65, 230);
            cr.LineTo(65, 290);
            cr.LineTo(75, 290);
            cr.LineTo(75, 310);
            cr.LineTo(85, 310);
            cr.LineTo(85, 320);
            cr.LineTo(95, 320);
            cr.LineTo(95, 330);
            cr.LineTo(115, 330);
            cr.LineTo(115, 340);
            cr.LineTo(195, 340);
            cr.LineTo(195, 330);
            cr.LineTo(225, 330);
            cr.LineTo(225, 320);
            cr.LineTo(245, 320);
            cr.LineTo(245, 310);
            cr.LineTo(265, 310);
            cr.LineTo(265, 300);
            cr.LineTo(275, 300);
            cr.LineTo(275, 290);
            cr.LineTo(285, 290);
            cr.LineTo(285, 280);
            cr.LineTo(295, 280);
            cr.LineTo(295, 270);
            cr.LineTo(305, 270);
            cr.LineTo(305, 260);
            cr.LineTo(315, 260);
            cr.LineTo(315, 240);
            cr.LineTo(325, 240);
            cr.LineTo(325, 220);
            cr.LineTo(335, 220);
            cr.LineTo(335, 200);
            cr.LineTo(345, 200);
            cr.LineTo(345, 190);
            cr.LineTo(365, 190);
            cr.LineTo(365, 180);
            cr.LineTo(375, 180);
            cr.LineTo(375, 170);
            cr.LineTo(385, 170);
            cr.LineTo(385, 130);
            cr.LineTo(375, 130);
            cr.LineTo(375, 120);
            cr.LineTo(365, 120);
            cr.LineTo(365, 110);
            cr.LineTo(315, 110);
            cr.LineTo(315, 120);
            cr.LineTo(305, 120);
            cr.LineTo(305, 130);
            cr.LineTo(295, 130);
            cr.LineTo(295, 120);
            cr.LineTo(265, 120);
            cr.ClosePath();
            cr.MoveTo(205, 130);
            cr.LineTo(225, 130);
            cr.LineTo(225, 140);
            cr.LineTo(255, 140);
            cr.LineTo(255, 160);
            cr.LineTo(265, 160);
            cr.LineTo(265, 170);
            cr.LineTo(255, 170);
            cr.LineTo(255, 180);
            cr.LineTo(245, 180);
            cr.LineTo(245, 170);
            cr.LineTo(235, 170);
            cr.LineTo(235, 180);
            cr.LineTo(195, 180);
            cr.LineTo(195, 190);
            cr.LineTo(175, 190);
            cr.LineTo(175, 200);
            cr.LineTo(155, 200);
            cr.LineTo(155, 210);
            cr.LineTo(145, 210);
            cr.LineTo(145, 220);
            cr.LineTo(135, 220);
            cr.LineTo(135, 230);
            cr.LineTo(125, 230);
            cr.LineTo(125, 240);
            cr.LineTo(115, 240);
            cr.LineTo(115, 230);
            cr.LineTo(125, 230);
            cr.LineTo(125, 220);
            cr.LineTo(115, 220);
            cr.LineTo(115, 210);
            cr.LineTo(105, 210);
            cr.LineTo(105, 200);
            cr.LineTo(115, 200);
            cr.LineTo(115, 180);
            cr.LineTo(125, 180);
            cr.LineTo(125, 170);
            cr.LineTo(135, 170);
            cr.LineTo(135, 160);
            cr.LineTo(155, 160);
            cr.LineTo(155, 150);
            cr.LineTo(175, 150);
            cr.LineTo(175, 140);
            cr.LineTo(205, 140);
            cr.ClosePath();
            cr.MoveTo(275, 150);
            cr.LineTo(275, 140);
            cr.LineTo(285, 140);
            cr.LineTo(285, 150);
            cr.LineTo(295, 150);
            cr.LineTo(295, 160);
            cr.LineTo(305, 160);
            cr.LineTo(305, 170);
            cr.LineTo(315, 170);
            cr.LineTo(315, 180);
            cr.LineTo(305, 180);
            cr.LineTo(305, 190);
            cr.LineTo(315, 190);
            cr.LineTo(315, 200);
            cr.LineTo(305, 200);
            cr.LineTo(305, 210);
            cr.LineTo(315, 210);
            cr.LineTo(315, 220);
            cr.LineTo(305, 220);
            cr.LineTo(305, 240);
            cr.LineTo(295, 240);
            cr.LineTo(295, 250);
            cr.LineTo(285, 250);
            cr.LineTo(285, 270);
            cr.LineTo(275, 270);
            cr.LineTo(275, 280);
            cr.LineTo(255, 280);
            cr.LineTo(255, 290);
            cr.LineTo(245, 290);
            cr.LineTo(245, 300);
            cr.LineTo(225, 300);
            cr.LineTo(225, 310);
            cr.LineTo(195, 310);
            cr.LineTo(195, 320);
            cr.LineTo(145, 320);
            cr.LineTo(145, 310);
            cr.LineTo(105, 310);
            cr.LineTo(105, 300);
            cr.LineTo(95, 300);
            cr.LineTo(95, 290);
            cr.LineTo(85, 290);
            cr.LineTo(85, 280);
            cr.LineTo(95, 280);
            cr.LineTo(95, 270);
            cr.LineTo(105, 270);
            cr.LineTo(105, 280);
            cr.LineTo(175, 280);
            cr.LineTo(175, 270);
            cr.LineTo(205, 270);
            cr.LineTo(205, 260);
            cr.LineTo(225, 260);
            cr.LineTo(225, 250);
            cr.LineTo(245, 250);
            cr.LineTo(245, 240);
            cr.LineTo(255, 240);
            cr.LineTo(255, 230);
            cr.LineTo(265, 230);
            cr.LineTo(265, 220);
            cr.LineTo(285, 220);
            cr.LineTo(285, 210);
            cr.LineTo(295, 210);
            cr.LineTo(295, 200);
            cr.LineTo(305, 200);
            cr.LineTo(305, 190);
            cr.LineTo(295, 190);
            cr.LineTo(295, 180);
            cr.LineTo(305, 180);
            cr.LineTo(305, 170);
            cr.LineTo(295, 170);
            cr.LineTo(295, 160);
            cr.LineTo(285, 160);
            cr.LineTo(285, 150);
            cr.ClosePath();
            cr.MoveTo(315, 140);
            cr.LineTo(315, 130);
            cr.LineTo(325, 130);
            cr.LineTo(325, 120);
            cr.LineTo(355, 120);
            cr.LineTo(355, 130);
            cr.LineTo(365, 130);
            cr.LineTo(365, 140);
            cr.LineTo(355, 140);
            cr.LineTo(355, 150);
            cr.LineTo(365, 150);
            cr.LineTo(365, 160);
            cr.LineTo(355, 160);
            cr.LineTo(355, 170);
            cr.LineTo(345, 170);
            cr.LineTo(345, 160);
            cr.LineTo(335, 160);
            cr.LineTo(335, 170);
            cr.LineTo(325, 170);
            cr.LineTo(325, 160);
            cr.LineTo(315, 160);
            cr.LineTo(315, 150);
            cr.LineTo(325, 150);
            cr.LineTo(325, 140);
            cr.ClosePath();
            cr.MoveTo(195, 200);
            cr.LineTo(265, 200);
            cr.LineTo(265, 210);
            cr.LineTo(255, 210);
            cr.LineTo(255, 220);
            cr.LineTo(235, 220);
            cr.LineTo(235, 230);
            cr.LineTo(225, 230);
            cr.LineTo(225, 240);
            cr.LineTo(205, 240);
            cr.LineTo(205, 250);
            cr.LineTo(175, 250);
            cr.LineTo(175, 260);
            cr.LineTo(135, 260);
            cr.LineTo(135, 240);
            cr.LineTo(145, 240);
            cr.LineTo(145, 230);
            cr.LineTo(155, 230);
            cr.LineTo(155, 220);
            cr.LineTo(175, 220);
            cr.LineTo(175, 210);
            cr.LineTo(195, 210);
            cr.ClosePath();
            cr.MoveTo(85, 240);
            cr.LineTo(95, 240);
            cr.LineTo(95, 250);
            cr.LineTo(85, 250);
            cr.ClosePath();
            cr.MoveTo(85, 260);
            cr.LineTo(95, 260);
            cr.LineTo(95, 270);
            cr.LineTo(85, 270);
            cr.ClosePath();
            cr.MoveTo(85, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 130);
            cr.LineTo(325, 130);
            cr.LineTo(325, 140);
            cr.LineTo(335, 140);
            cr.ClosePath();
            cr.MoveTo(335, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 130);
            cr.LineTo(325, 130);
            cr.LineTo(325, 140);
            cr.LineTo(335, 140);
            cr.ClosePath();
            cr.MoveTo(335, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 140);
            cr.LineTo(335, 140);
            cr.LineTo(335, 150);
            cr.LineTo(345, 150);
            cr.ClosePath();
            cr.MoveTo(345, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 140);
            cr.LineTo(335, 140);
            cr.LineTo(335, 150);
            cr.LineTo(345, 150);
            cr.ClosePath();
            cr.MoveTo(345, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 150);
            cr.LineTo(235, 150);
            cr.LineTo(235, 160);
            cr.LineTo(245, 160);
            cr.ClosePath();
            cr.MoveTo(245, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 150);
            cr.LineTo(235, 150);
            cr.LineTo(235, 160);
            cr.LineTo(245, 160);
            cr.ClosePath();
            cr.MoveTo(245, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 150);
            cr.LineTo(325, 150);
            cr.LineTo(325, 160);
            cr.LineTo(335, 160);
            cr.ClosePath();
            cr.MoveTo(335, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(335, 150);
            cr.LineTo(325, 150);
            cr.LineTo(325, 160);
            cr.LineTo(335, 160);
            cr.ClosePath();
            cr.MoveTo(335, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(355, 150);
            cr.LineTo(345, 150);
            cr.LineTo(345, 160);
            cr.LineTo(355, 160);
            cr.ClosePath();
            cr.MoveTo(355, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(355, 150);
            cr.LineTo(345, 150);
            cr.LineTo(345, 160);
            cr.LineTo(355, 160);
            cr.ClosePath();
            cr.MoveTo(355, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125, 200);
            cr.LineTo(115, 200);
            cr.LineTo(115, 210);
            cr.LineTo(125, 210);
            cr.ClosePath();
            cr.MoveTo(125, 200);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125, 200);
            cr.LineTo(115, 200);
            cr.LineTo(115, 210);
            cr.LineTo(125, 210);
            cr.ClosePath();
            cr.MoveTo(125, 200);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 210);
            cr.LineTo(295, 210);
            cr.LineTo(295, 220);
            cr.LineTo(305, 220);
            cr.ClosePath();
            cr.MoveTo(305, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 210);
            cr.LineTo(295, 210);
            cr.LineTo(295, 220);
            cr.LineTo(305, 220);
            cr.ClosePath();
            cr.MoveTo(305, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 220);
            cr.LineTo(285, 220);
            cr.LineTo(285, 230);
            cr.LineTo(295, 230);
            cr.ClosePath();
            cr.MoveTo(295, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 220);
            cr.LineTo(285, 220);
            cr.LineTo(285, 230);
            cr.LineTo(295, 230);
            cr.ClosePath();
            cr.MoveTo(295, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 280);
            cr.LineTo(95, 280);
            cr.LineTo(95, 290);
            cr.LineTo(105, 290);
            cr.ClosePath();
            cr.MoveTo(105, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 280);
            cr.LineTo(95, 280);
            cr.LineTo(95, 290);
            cr.LineTo(105, 290);
            cr.ClosePath();
            cr.MoveTo(105, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -58, -188);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 290);
            cr.LineTo(105, 290);
            cr.LineTo(105, 300);
            cr.LineTo(135, 300);
            cr.ClosePath();
            cr.MoveTo(135, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void Drawcape_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(415.910156, 67.5);
            cr.LineTo(320.453125, 67.5);
            cr.LineTo(320.453125, 77.046875);
            cr.LineTo(291.816406, 77.046875);
            cr.LineTo(291.816406, 86.589844);
            cr.LineTo(272.726563, 86.589844);
            cr.LineTo(272.726563, 96.136719);
            cr.LineTo(244.089844, 96.136719);
            cr.LineTo(244.089844, 105.679688);
            cr.LineTo(225, 105.679688);
            cr.LineTo(225, 115.226563);
            cr.LineTo(196.363281, 115.226563);
            cr.LineTo(196.363281, 124.769531);
            cr.LineTo(167.726563, 124.769531);
            cr.LineTo(167.726563, 134.316406);
            cr.LineTo(148.636719, 134.316406);
            cr.LineTo(148.636719, 143.859375);
            cr.LineTo(129.546875, 143.859375);
            cr.LineTo(129.546875, 153.40625);
            cr.LineTo(110.453125, 153.40625);
            cr.LineTo(110.453125, 162.949219);
            cr.LineTo(100.910156, 162.949219);
            cr.LineTo(100.910156, 172.496094);
            cr.LineTo(81.820313, 172.496094);
            cr.LineTo(81.820313, 182.039063);
            cr.LineTo(62.726563, 182.039063);
            cr.LineTo(62.726563, 191.585938);
            cr.LineTo(43.636719, 191.585938);
            cr.LineTo(43.636719, 201.132813);
            cr.LineTo(24.546875, 201.132813);
            cr.LineTo(24.546875, 220.222656);
            cr.LineTo(15, 220.222656);
            cr.LineTo(15, 287.039063);
            cr.LineTo(24.546875, 287.039063);
            cr.LineTo(24.546875, 296.585938);
            cr.LineTo(34.089844, 296.585938);
            cr.LineTo(34.089844, 315.675781);
            cr.LineTo(43.636719, 315.675781);
            cr.LineTo(43.636719, 325.222656);
            cr.LineTo(62.726563, 325.222656);
            cr.LineTo(62.726563, 334.769531);
            cr.LineTo(110.453125, 334.769531);
            cr.LineTo(110.453125, 344.3125);
            cr.LineTo(148.636719, 344.3125);
            cr.LineTo(148.636719, 353.859375);
            cr.LineTo(158.179688, 353.859375);
            cr.LineTo(158.179688, 372.949219);
            cr.LineTo(177.269531, 372.949219);
            cr.LineTo(177.269531, 382.496094);
            cr.LineTo(205.90625, 382.496094);
            cr.LineTo(205.90625, 372.949219);
            cr.LineTo(225, 372.949219);
            cr.LineTo(225, 363.40625);
            cr.LineTo(234.546875, 363.40625);
            cr.LineTo(234.546875, 353.859375);
            cr.LineTo(244.089844, 353.859375);
            cr.LineTo(244.089844, 344.3125);
            cr.LineTo(253.636719, 344.3125);
            cr.LineTo(253.636719, 334.769531);
            cr.LineTo(263.183594, 334.769531);
            cr.LineTo(263.183594, 325.222656);
            cr.LineTo(272.726563, 325.222656);
            cr.LineTo(272.726563, 315.675781);
            cr.LineTo(282.273438, 315.675781);
            cr.LineTo(282.273438, 306.132813);
            cr.LineTo(291.820313, 306.132813);
            cr.LineTo(291.820313, 296.585938);
            cr.LineTo(301.363281, 296.585938);
            cr.LineTo(301.363281, 287.039063);
            cr.LineTo(310.910156, 287.039063);
            cr.LineTo(310.910156, 277.5);
            cr.LineTo(320.453125, 277.5);
            cr.LineTo(320.453125, 267.953125);
            cr.LineTo(330, 267.953125);
            cr.LineTo(330, 258.410156);
            cr.LineTo(339.546875, 258.410156);
            cr.LineTo(339.546875, 248.863281);
            cr.LineTo(349.089844, 248.863281);
            cr.LineTo(349.089844, 239.316406);
            cr.LineTo(358.636719, 239.316406);
            cr.LineTo(358.636719, 229.773438);
            cr.LineTo(368.183594, 229.773438);
            cr.LineTo(368.183594, 220.226563);
            cr.LineTo(377.726563, 220.226563);
            cr.LineTo(377.726563, 210.683594);
            cr.LineTo(387.273438, 210.683594);
            cr.LineTo(387.273438, 201.136719);
            cr.LineTo(396.820313, 201.136719);
            cr.LineTo(396.820313, 182.046875);
            cr.LineTo(406.363281, 182.046875);
            cr.LineTo(406.363281, 172.5);
            cr.LineTo(415.910156, 172.5);
            cr.LineTo(415.910156, 153.410156);
            cr.LineTo(425.453125, 153.410156);
            cr.LineTo(425.453125, 134.316406);
            cr.LineTo(415.910156, 134.316406);
            cr.LineTo(415.910156, 124.773438);
            cr.LineTo(387.273438, 124.773438);
            cr.LineTo(387.273438, 115.226563);
            cr.LineTo(425.453125, 115.226563);
            cr.LineTo(425.453125, 105.683594);
            cr.LineTo(435, 105.683594);
            cr.LineTo(435, 77.046875);
            cr.LineTo(415.910156, 77.046875);
            cr.ClosePath();
            cr.MoveTo(291.816406, 96.136719);
            cr.LineTo(330, 96.136719);
            cr.LineTo(330, 86.589844);
            cr.LineTo(415.910156, 86.589844);
            cr.LineTo(415.910156, 96.136719);
            cr.LineTo(387.273438, 96.136719);
            cr.LineTo(387.273438, 105.679688);
            cr.LineTo(377.726563, 105.679688);
            cr.LineTo(377.726563, 134.316406);
            cr.LineTo(396.816406, 134.316406);
            cr.LineTo(396.816406, 143.863281);
            cr.LineTo(406.363281, 143.863281);
            cr.LineTo(406.363281, 153.40625);
            cr.LineTo(387.273438, 153.40625);
            cr.LineTo(387.273438, 172.5);
            cr.LineTo(377.726563, 172.5);
            cr.LineTo(377.726563, 182.046875);
            cr.LineTo(368.179688, 182.046875);
            cr.LineTo(368.179688, 201.136719);
            cr.LineTo(358.636719, 201.136719);
            cr.LineTo(358.636719, 210.679688);
            cr.LineTo(349.089844, 210.679688);
            cr.LineTo(349.089844, 220.226563);
            cr.LineTo(339.546875, 220.226563);
            cr.LineTo(339.546875, 229.769531);
            cr.LineTo(330, 229.769531);
            cr.LineTo(330, 239.316406);
            cr.LineTo(320.453125, 239.316406);
            cr.LineTo(320.453125, 248.863281);
            cr.LineTo(310.910156, 248.863281);
            cr.LineTo(310.910156, 258.40625);
            cr.LineTo(301.363281, 258.40625);
            cr.LineTo(301.363281, 267.953125);
            cr.LineTo(291.816406, 267.953125);
            cr.LineTo(291.816406, 277.5);
            cr.LineTo(282.273438, 277.5);
            cr.LineTo(282.273438, 287.046875);
            cr.LineTo(272.726563, 287.046875);
            cr.LineTo(272.726563, 296.589844);
            cr.LineTo(263.179688, 296.589844);
            cr.LineTo(263.179688, 306.136719);
            cr.LineTo(253.636719, 306.136719);
            cr.LineTo(253.636719, 315.679688);
            cr.LineTo(244.089844, 315.679688);
            cr.LineTo(244.089844, 325.226563);
            cr.LineTo(234.546875, 325.226563);
            cr.LineTo(234.546875, 334.773438);
            cr.LineTo(225, 334.773438);
            cr.LineTo(225, 344.316406);
            cr.LineTo(215.453125, 344.316406);
            cr.LineTo(215.453125, 353.863281);
            cr.LineTo(205.910156, 353.863281);
            cr.LineTo(205.910156, 363.410156);
            cr.LineTo(186.820313, 363.410156);
            cr.LineTo(186.820313, 353.863281);
            cr.LineTo(177.273438, 353.863281);
            cr.LineTo(177.273438, 344.316406);
            cr.LineTo(167.730469, 344.316406);
            cr.LineTo(167.730469, 325.226563);
            cr.LineTo(177.273438, 325.226563);
            cr.LineTo(177.273438, 315.679688);
            cr.LineTo(167.730469, 315.679688);
            cr.LineTo(167.730469, 296.589844);
            cr.LineTo(129.546875, 296.589844);
            cr.LineTo(129.546875, 287.046875);
            cr.LineTo(120, 287.046875);
            cr.LineTo(120, 296.589844);
            cr.LineTo(100.910156, 296.589844);
            cr.LineTo(100.910156, 306.136719);
            cr.LineTo(158.179688, 306.136719);
            cr.LineTo(158.179688, 325.226563);
            cr.LineTo(129.546875, 325.226563);
            cr.LineTo(129.546875, 315.679688);
            cr.LineTo(100.910156, 315.679688);
            cr.LineTo(100.910156, 306.136719);
            cr.LineTo(81.816406, 306.136719);
            cr.LineTo(81.816406, 315.679688);
            cr.LineTo(62.726563, 315.679688);
            cr.LineTo(62.726563, 306.136719);
            cr.LineTo(53.179688, 306.136719);
            cr.LineTo(53.179688, 287.046875);
            cr.LineTo(43.636719, 287.046875);
            cr.LineTo(43.636719, 277.5);
            cr.LineTo(34.089844, 277.5);
            cr.LineTo(34.089844, 229.773438);
            cr.LineTo(43.636719, 229.773438);
            cr.LineTo(43.636719, 210.679688);
            cr.LineTo(53.179688, 210.679688);
            cr.LineTo(53.179688, 239.316406);
            cr.LineTo(62.726563, 239.316406);
            cr.LineTo(62.726563, 201.136719);
            cr.LineTo(81.816406, 201.136719);
            cr.LineTo(81.816406, 220.226563);
            cr.LineTo(91.363281, 220.226563);
            cr.LineTo(91.363281, 191.589844);
            cr.LineTo(110.453125, 191.589844);
            cr.LineTo(110.453125, 172.5);
            cr.LineTo(120, 172.5);
            cr.LineTo(120, 191.589844);
            cr.LineTo(129.546875, 191.589844);
            cr.LineTo(129.546875, 162.953125);
            cr.LineTo(148.636719, 162.953125);
            cr.LineTo(148.636719, 153.410156);
            cr.LineTo(177.273438, 153.410156);
            cr.LineTo(177.273438, 143.863281);
            cr.LineTo(196.363281, 143.863281);
            cr.LineTo(196.363281, 134.320313);
            cr.LineTo(225, 134.320313);
            cr.LineTo(225, 124.773438);
            cr.LineTo(244.089844, 124.773438);
            cr.LineTo(244.089844, 115.230469);
            cr.LineTo(272.726563, 115.230469);
            cr.LineTo(272.726563, 105.683594);
            cr.LineTo(291.820313, 105.683594);
            cr.LineTo(291.820313, 96.136719);
            cr.ClosePath();
            cr.MoveTo(291.816406, 96.136719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(310.910156, 162.953125);
            cr.LineTo(301.363281, 162.953125);
            cr.LineTo(301.363281, 172.5);
            cr.LineTo(310.910156, 172.5);
            cr.ClosePath();
            cr.MoveTo(310.910156, 162.953125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(310.910156, 162.953125);
            cr.LineTo(301.363281, 162.953125);
            cr.LineTo(301.363281, 172.5);
            cr.LineTo(310.910156, 172.5);
            cr.ClosePath();
            cr.MoveTo(310.910156, 162.953125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(301.363281, 172.5);
            cr.LineTo(291.816406, 172.5);
            cr.LineTo(291.816406, 182.046875);
            cr.LineTo(301.363281, 182.046875);
            cr.ClosePath();
            cr.MoveTo(301.363281, 172.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(301.363281, 172.5);
            cr.LineTo(291.816406, 172.5);
            cr.LineTo(291.816406, 182.046875);
            cr.LineTo(301.363281, 182.046875);
            cr.ClosePath();
            cr.MoveTo(301.363281, 172.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(291.816406, 182.046875);
            cr.LineTo(272.726563, 182.046875);
            cr.LineTo(272.726563, 191.589844);
            cr.LineTo(291.816406, 191.589844);
            cr.ClosePath();
            cr.MoveTo(291.816406, 182.046875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(139.089844, 191.589844);
            cr.LineTo(129.546875, 191.589844);
            cr.LineTo(129.546875, 201.136719);
            cr.LineTo(139.089844, 201.136719);
            cr.ClosePath();
            cr.MoveTo(139.089844, 191.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(139.089844, 191.589844);
            cr.LineTo(129.546875, 191.589844);
            cr.LineTo(129.546875, 201.136719);
            cr.LineTo(139.089844, 201.136719);
            cr.ClosePath();
            cr.MoveTo(139.089844, 191.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(272.726563, 191.589844);
            cr.LineTo(263.179688, 191.589844);
            cr.LineTo(263.179688, 201.136719);
            cr.LineTo(272.726563, 201.136719);
            cr.ClosePath();
            cr.MoveTo(272.726563, 191.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(272.726563, 191.589844);
            cr.LineTo(263.179688, 191.589844);
            cr.LineTo(263.179688, 201.136719);
            cr.LineTo(272.726563, 201.136719);
            cr.ClosePath();
            cr.MoveTo(272.726563, 191.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(263.179688, 201.136719);
            cr.LineTo(244.089844, 201.136719);
            cr.LineTo(244.089844, 210.679688);
            cr.LineTo(234.546875, 210.679688);
            cr.LineTo(234.546875, 220.226563);
            cr.LineTo(215.453125, 220.226563);
            cr.LineTo(215.453125, 229.769531);
            cr.LineTo(244.089844, 229.769531);
            cr.LineTo(244.089844, 220.226563);
            cr.LineTo(253.636719, 220.226563);
            cr.LineTo(253.636719, 210.679688);
            cr.LineTo(263.179688, 210.679688);
            cr.ClosePath();
            cr.MoveTo(263.179688, 201.136719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100.910156, 220.226563);
            cr.LineTo(91.363281, 220.226563);
            cr.LineTo(91.363281, 229.773438);
            cr.LineTo(100.910156, 229.773438);
            cr.ClosePath();
            cr.MoveTo(100.910156, 220.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100.910156, 220.226563);
            cr.LineTo(91.363281, 220.226563);
            cr.LineTo(91.363281, 229.773438);
            cr.LineTo(100.910156, 229.773438);
            cr.ClosePath();
            cr.MoveTo(100.910156, 220.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(282.273438, 220.226563);
            cr.LineTo(272.726563, 220.226563);
            cr.LineTo(272.726563, 229.773438);
            cr.LineTo(282.273438, 229.773438);
            cr.ClosePath();
            cr.MoveTo(282.273438, 220.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(282.273438, 220.226563);
            cr.LineTo(272.726563, 220.226563);
            cr.LineTo(272.726563, 229.773438);
            cr.LineTo(282.273438, 229.773438);
            cr.ClosePath();
            cr.MoveTo(282.273438, 220.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215.453125, 229.773438);
            cr.LineTo(205.910156, 229.773438);
            cr.LineTo(205.910156, 239.320313);
            cr.LineTo(186.816406, 239.320313);
            cr.LineTo(186.816406, 248.863281);
            cr.LineTo(225, 248.863281);
            cr.LineTo(225, 239.320313);
            cr.LineTo(215.453125, 239.320313);
            cr.ClosePath();
            cr.MoveTo(215.453125, 229.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(272.726563, 229.773438);
            cr.LineTo(263.179688, 229.773438);
            cr.LineTo(263.179688, 239.320313);
            cr.LineTo(272.726563, 239.320313);
            cr.ClosePath();
            cr.MoveTo(272.726563, 229.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(272.726563, 229.773438);
            cr.LineTo(263.179688, 229.773438);
            cr.LineTo(263.179688, 239.320313);
            cr.LineTo(272.726563, 239.320313);
            cr.ClosePath();
            cr.MoveTo(272.726563, 229.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(72.273438, 239.320313);
            cr.LineTo(62.726563, 239.320313);
            cr.LineTo(62.726563, 248.863281);
            cr.LineTo(72.273438, 248.863281);
            cr.ClosePath();
            cr.MoveTo(72.273438, 239.320313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(72.273438, 239.320313);
            cr.LineTo(62.726563, 239.320313);
            cr.LineTo(62.726563, 248.863281);
            cr.LineTo(72.273438, 248.863281);
            cr.ClosePath();
            cr.MoveTo(72.273438, 239.320313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(263.179688, 239.320313);
            cr.LineTo(253.636719, 239.320313);
            cr.LineTo(253.636719, 248.863281);
            cr.LineTo(263.179688, 248.863281);
            cr.ClosePath();
            cr.MoveTo(263.179688, 239.320313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(263.179688, 239.320313);
            cr.LineTo(253.636719, 239.320313);
            cr.LineTo(253.636719, 248.863281);
            cr.LineTo(263.179688, 248.863281);
            cr.ClosePath();
            cr.MoveTo(263.179688, 239.320313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(186.816406, 248.863281);
            cr.LineTo(177.273438, 248.863281);
            cr.LineTo(177.273438, 258.410156);
            cr.LineTo(158.183594, 258.410156);
            cr.LineTo(158.183594, 267.953125);
            cr.LineTo(196.363281, 267.953125);
            cr.LineTo(196.363281, 277.5);
            cr.LineTo(205.910156, 277.5);
            cr.LineTo(205.910156, 287.046875);
            cr.LineTo(215.453125, 287.046875);
            cr.LineTo(215.453125, 277.5);
            cr.LineTo(225, 277.5);
            cr.LineTo(225, 267.953125);
            cr.LineTo(205.910156, 267.953125);
            cr.LineTo(205.910156, 258.410156);
            cr.LineTo(186.816406, 258.410156);
            cr.ClosePath();
            cr.MoveTo(186.816406, 248.863281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(253.636719, 248.863281);
            cr.LineTo(244.089844, 248.863281);
            cr.LineTo(244.089844, 258.410156);
            cr.LineTo(253.636719, 258.410156);
            cr.ClosePath();
            cr.MoveTo(253.636719, 248.863281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(253.636719, 248.863281);
            cr.LineTo(244.089844, 248.863281);
            cr.LineTo(244.089844, 258.410156);
            cr.LineTo(253.636719, 258.410156);
            cr.ClosePath();
            cr.MoveTo(253.636719, 248.863281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(244.089844, 258.410156);
            cr.LineTo(225, 258.410156);
            cr.LineTo(225, 267.953125);
            cr.LineTo(244.089844, 267.953125);
            cr.ClosePath();
            cr.MoveTo(244.089844, 258.410156);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(158.179688, 267.953125);
            cr.LineTo(148.636719, 267.953125);
            cr.LineTo(148.636719, 277.5);
            cr.LineTo(129.546875, 277.5);
            cr.LineTo(129.546875, 287.046875);
            cr.LineTo(177.273438, 287.046875);
            cr.LineTo(177.273438, 277.5);
            cr.LineTo(158.183594, 277.5);
            cr.LineTo(158.183594, 267.953125);
            cr.ClosePath();
            cr.MoveTo(158.179688, 267.953125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(186.816406, 287.046875);
            cr.LineTo(177.273438, 287.046875);
            cr.LineTo(177.273438, 296.589844);
            cr.LineTo(186.816406, 296.589844);
            cr.ClosePath();
            cr.MoveTo(186.816406, 287.046875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(186.816406, 287.046875);
            cr.LineTo(177.273438, 287.046875);
            cr.LineTo(177.273438, 296.589844);
            cr.LineTo(186.816406, 296.589844);
            cr.ClosePath();
            cr.MoveTo(186.816406, 287.046875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205.910156, 287.046875);
            cr.LineTo(196.363281, 287.046875);
            cr.LineTo(196.363281, 296.589844);
            cr.LineTo(205.910156, 296.589844);
            cr.ClosePath();
            cr.MoveTo(205.910156, 287.046875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205.910156, 287.046875);
            cr.LineTo(196.363281, 287.046875);
            cr.LineTo(196.363281, 296.589844);
            cr.LineTo(205.910156, 296.589844);
            cr.ClosePath();
            cr.MoveTo(205.910156, 287.046875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(196.363281, 296.589844);
            cr.LineTo(186.816406, 296.589844);
            cr.LineTo(186.816406, 306.136719);
            cr.LineTo(196.363281, 306.136719);
            cr.ClosePath();
            cr.MoveTo(196.363281, 296.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(196.363281, 296.589844);
            cr.LineTo(186.816406, 296.589844);
            cr.LineTo(186.816406, 306.136719);
            cr.LineTo(196.363281, 306.136719);
            cr.ClosePath();
            cr.MoveTo(196.363281, 296.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(186.816406, 306.136719);
            cr.LineTo(177.273438, 306.136719);
            cr.LineTo(177.273438, 315.683594);
            cr.LineTo(186.816406, 315.683594);
            cr.ClosePath();
            cr.MoveTo(186.816406, 306.136719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(186.816406, 306.136719);
            cr.LineTo(177.273438, 306.136719);
            cr.LineTo(177.273438, 315.683594);
            cr.LineTo(186.816406, 315.683594);
            cr.ClosePath();
            cr.MoveTo(186.816406, 306.136719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void Drawshirt_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 60);
            cr.LineTo(175, 60);
            cr.LineTo(175, 70);
            cr.LineTo(165, 70);
            cr.LineTo(165, 110);
            cr.LineTo(175, 110);
            cr.LineTo(175, 130);
            cr.LineTo(185, 130);
            cr.LineTo(185, 150);
            cr.LineTo(165, 150);
            cr.LineTo(165, 160);
            cr.LineTo(155, 160);
            cr.LineTo(155, 170);
            cr.LineTo(135, 170);
            cr.LineTo(135, 180);
            cr.LineTo(125, 180);
            cr.LineTo(125, 190);
            cr.LineTo(105, 190);
            cr.LineTo(105, 200);
            cr.LineTo(85, 200);
            cr.LineTo(85, 210);
            cr.LineTo(75, 210);
            cr.LineTo(75, 220);
            cr.LineTo(55, 220);
            cr.LineTo(55, 230);
            cr.LineTo(45, 230);
            cr.LineTo(45, 240);
            cr.LineTo(35, 240);
            cr.LineTo(35, 250);
            cr.LineTo(45, 250);
            cr.LineTo(45, 260);
            cr.LineTo(55, 260);
            cr.LineTo(55, 270);
            cr.LineTo(75, 270);
            cr.LineTo(75, 280);
            cr.LineTo(85, 280);
            cr.LineTo(85, 290);
            cr.LineTo(95, 290);
            cr.LineTo(95, 300);
            cr.LineTo(105, 300);
            cr.LineTo(105, 310);
            cr.LineTo(115, 310);
            cr.LineTo(115, 320);
            cr.LineTo(125, 320);
            cr.LineTo(125, 330);
            cr.LineTo(135, 330);
            cr.LineTo(135, 340);
            cr.LineTo(145, 340);
            cr.LineTo(145, 350);
            cr.LineTo(155, 350);
            cr.LineTo(155, 370);
            cr.LineTo(165, 370);
            cr.LineTo(165, 380);
            cr.LineTo(175, 380);
            cr.LineTo(175, 390);
            cr.LineTo(195, 390);
            cr.LineTo(195, 380);
            cr.LineTo(205, 380);
            cr.LineTo(205, 370);
            cr.LineTo(215, 370);
            cr.LineTo(215, 360);
            cr.LineTo(225, 360);
            cr.LineTo(225, 350);
            cr.LineTo(235, 350);
            cr.LineTo(235, 340);
            cr.LineTo(245, 340);
            cr.LineTo(245, 330);
            cr.LineTo(255, 330);
            cr.LineTo(255, 320);
            cr.LineTo(265, 320);
            cr.LineTo(265, 310);
            cr.LineTo(275, 310);
            cr.LineTo(275, 300);
            cr.LineTo(285, 300);
            cr.LineTo(285, 290);
            cr.LineTo(295, 290);
            cr.LineTo(295, 280);
            cr.LineTo(315, 280);
            cr.LineTo(315, 290);
            cr.LineTo(325, 290);
            cr.LineTo(325, 300);
            cr.LineTo(335, 300);
            cr.LineTo(335, 310);
            cr.LineTo(345, 310);
            cr.LineTo(345, 320);
            cr.LineTo(365, 320);
            cr.LineTo(365, 310);
            cr.LineTo(375, 310);
            cr.LineTo(375, 300);
            cr.LineTo(385, 300);
            cr.LineTo(385, 280);
            cr.LineTo(395, 280);
            cr.LineTo(395, 250);
            cr.LineTo(405, 250);
            cr.LineTo(405, 230);
            cr.LineTo(415, 230);
            cr.LineTo(415, 160);
            cr.LineTo(405, 160);
            cr.LineTo(405, 150);
            cr.LineTo(395, 150);
            cr.LineTo(395, 130);
            cr.LineTo(385, 130);
            cr.LineTo(385, 120);
            cr.LineTo(375, 120);
            cr.LineTo(375, 110);
            cr.LineTo(365, 110);
            cr.LineTo(365, 100);
            cr.LineTo(355, 100);
            cr.LineTo(355, 90);
            cr.LineTo(345, 90);
            cr.LineTo(345, 80);
            cr.LineTo(335, 80);
            cr.LineTo(335, 70);
            cr.LineTo(315, 70);
            cr.ClosePath();
            cr.MoveTo(185, 90);
            cr.LineTo(185, 80);
            cr.LineTo(195, 80);
            cr.LineTo(195, 70);
            cr.LineTo(205, 70);
            cr.LineTo(205, 80);
            cr.LineTo(215, 80);
            cr.LineTo(215, 70);
            cr.LineTo(235, 70);
            cr.LineTo(235, 80);
            cr.LineTo(245, 80);
            cr.LineTo(245, 70);
            cr.LineTo(265, 70);
            cr.LineTo(265, 80);
            cr.LineTo(275, 80);
            cr.LineTo(275, 70);
            cr.LineTo(305, 70);
            cr.LineTo(305, 80);
            cr.LineTo(315, 80);
            cr.LineTo(315, 90);
            cr.LineTo(325, 90);
            cr.LineTo(325, 130);
            cr.LineTo(345, 130);
            cr.LineTo(345, 140);
            cr.LineTo(375, 140);
            cr.LineTo(375, 150);
            cr.LineTo(365, 150);
            cr.LineTo(365, 160);
            cr.LineTo(395, 160);
            cr.LineTo(395, 190);
            cr.LineTo(385, 190);
            cr.LineTo(385, 200);
            cr.LineTo(395, 200);
            cr.LineTo(395, 220);
            cr.LineTo(385, 220);
            cr.LineTo(385, 250);
            cr.LineTo(375, 250);
            cr.LineTo(375, 280);
            cr.LineTo(365, 280);
            cr.LineTo(365, 290);
            cr.LineTo(355, 290);
            cr.LineTo(355, 280);
            cr.LineTo(345, 280);
            cr.LineTo(345, 290);
            cr.LineTo(335, 290);
            cr.LineTo(335, 280);
            cr.LineTo(325, 280);
            cr.LineTo(325, 270);
            cr.LineTo(335, 270);
            cr.LineTo(335, 260);
            cr.LineTo(325, 260);
            cr.LineTo(325, 250);
            cr.LineTo(335, 250);
            cr.LineTo(335, 240);
            cr.LineTo(345, 240);
            cr.LineTo(345, 230);
            cr.LineTo(335, 230);
            cr.LineTo(335, 220);
            cr.LineTo(325, 220);
            cr.LineTo(325, 230);
            cr.LineTo(315, 230);
            cr.LineTo(315, 240);
            cr.LineTo(305, 240);
            cr.LineTo(305, 250);
            cr.LineTo(295, 250);
            cr.LineTo(295, 270);
            cr.LineTo(285, 270);
            cr.LineTo(285, 280);
            cr.LineTo(275, 280);
            cr.LineTo(275, 290);
            cr.LineTo(265, 290);
            cr.LineTo(265, 300);
            cr.LineTo(255, 300);
            cr.LineTo(255, 310);
            cr.LineTo(245, 310);
            cr.LineTo(245, 320);
            cr.LineTo(235, 320);
            cr.LineTo(235, 330);
            cr.LineTo(225, 330);
            cr.LineTo(225, 340);
            cr.LineTo(215, 340);
            cr.LineTo(215, 350);
            cr.LineTo(205, 350);
            cr.LineTo(205, 360);
            cr.LineTo(195, 360);
            cr.LineTo(195, 350);
            cr.LineTo(185, 350);
            cr.LineTo(185, 360);
            cr.LineTo(175, 360);
            cr.LineTo(175, 350);
            cr.LineTo(165, 350);
            cr.LineTo(165, 340);
            cr.LineTo(155, 340);
            cr.LineTo(155, 330);
            cr.LineTo(145, 330);
            cr.LineTo(145, 320);
            cr.LineTo(135, 320);
            cr.LineTo(135, 310);
            cr.LineTo(125, 310);
            cr.LineTo(125, 300);
            cr.LineTo(115, 300);
            cr.LineTo(115, 290);
            cr.LineTo(105, 290);
            cr.LineTo(105, 280);
            cr.LineTo(95, 280);
            cr.LineTo(95, 270);
            cr.LineTo(85, 270);
            cr.LineTo(85, 260);
            cr.LineTo(75, 260);
            cr.LineTo(75, 250);
            cr.LineTo(65, 250);
            cr.LineTo(65, 240);
            cr.LineTo(75, 240);
            cr.LineTo(75, 230);
            cr.LineTo(85, 230);
            cr.LineTo(85, 220);
            cr.LineTo(105, 220);
            cr.LineTo(105, 210);
            cr.LineTo(125, 210);
            cr.LineTo(125, 200);
            cr.LineTo(135, 200);
            cr.LineTo(135, 190);
            cr.LineTo(145, 190);
            cr.LineTo(145, 200);
            cr.LineTo(155, 200);
            cr.LineTo(155, 180);
            cr.LineTo(165, 180);
            cr.LineTo(165, 190);
            cr.LineTo(175, 190);
            cr.LineTo(175, 170);
            cr.LineTo(185, 170);
            cr.LineTo(185, 160);
            cr.LineTo(205, 160);
            cr.LineTo(205, 150);
            cr.LineTo(215, 150);
            cr.LineTo(215, 140);
            cr.LineTo(235, 140);
            cr.LineTo(235, 120);
            cr.LineTo(215, 120);
            cr.LineTo(215, 130);
            cr.LineTo(205, 130);
            cr.LineTo(205, 120);
            cr.LineTo(195, 120);
            cr.LineTo(195, 110);
            cr.LineTo(185, 110);
            cr.LineTo(185, 100);
            cr.LineTo(195, 100);
            cr.LineTo(195, 90);
            cr.ClosePath();
            cr.MoveTo(185, 90);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 90);
            cr.LineTo(305, 90);
            cr.LineTo(305, 100);
            cr.LineTo(315, 100);
            cr.ClosePath();
            cr.MoveTo(315, 90);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 90);
            cr.LineTo(305, 90);
            cr.LineTo(305, 100);
            cr.LineTo(315, 100);
            cr.ClosePath();
            cr.MoveTo(315, 90);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 100);
            cr.LineTo(195, 100);
            cr.LineTo(195, 110);
            cr.LineTo(205, 110);
            cr.ClosePath();
            cr.MoveTo(205, 100);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 100);
            cr.LineTo(195, 100);
            cr.LineTo(195, 110);
            cr.LineTo(205, 110);
            cr.ClosePath();
            cr.MoveTo(205, 100);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 110);
            cr.LineTo(205, 110);
            cr.LineTo(205, 120);
            cr.LineTo(215, 120);
            cr.ClosePath();
            cr.MoveTo(215, 110);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 110);
            cr.LineTo(205, 110);
            cr.LineTo(205, 120);
            cr.LineTo(215, 120);
            cr.ClosePath();
            cr.MoveTo(215, 110);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 110);
            cr.LineTo(235, 110);
            cr.LineTo(235, 120);
            cr.LineTo(245, 120);
            cr.ClosePath();
            cr.MoveTo(245, 110);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 110);
            cr.LineTo(235, 110);
            cr.LineTo(235, 120);
            cr.LineTo(245, 120);
            cr.ClosePath();
            cr.MoveTo(245, 110);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 110);
            cr.LineTo(305, 110);
            cr.LineTo(305, 120);
            cr.LineTo(315, 120);
            cr.ClosePath();
            cr.MoveTo(315, 110);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 110);
            cr.LineTo(305, 110);
            cr.LineTo(305, 120);
            cr.LineTo(315, 120);
            cr.ClosePath();
            cr.MoveTo(315, 110);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 120);
            cr.LineTo(245, 120);
            cr.LineTo(245, 130);
            cr.LineTo(255, 130);
            cr.ClosePath();
            cr.MoveTo(255, 120);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 120);
            cr.LineTo(245, 120);
            cr.LineTo(245, 130);
            cr.LineTo(255, 130);
            cr.ClosePath();
            cr.MoveTo(255, 120);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(285, 130);
            cr.LineTo(275, 130);
            cr.LineTo(275, 140);
            cr.LineTo(285, 140);
            cr.ClosePath();
            cr.MoveTo(285, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(285, 130);
            cr.LineTo(275, 130);
            cr.LineTo(275, 140);
            cr.LineTo(285, 140);
            cr.ClosePath();
            cr.MoveTo(285, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(325, 130);
            cr.LineTo(315, 130);
            cr.LineTo(315, 140);
            cr.LineTo(325, 140);
            cr.ClosePath();
            cr.MoveTo(325, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(325, 130);
            cr.LineTo(315, 130);
            cr.LineTo(315, 140);
            cr.LineTo(325, 140);
            cr.ClosePath();
            cr.MoveTo(325, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(315, 140);
            cr.LineTo(295, 140);
            cr.LineTo(295, 150);
            cr.LineTo(315, 150);
            cr.ClosePath();
            cr.MoveTo(315, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 150);
            cr.LineTo(285, 150);
            cr.LineTo(285, 160);
            cr.LineTo(295, 160);
            cr.ClosePath();
            cr.MoveTo(295, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 150);
            cr.LineTo(285, 150);
            cr.LineTo(285, 160);
            cr.LineTo(295, 160);
            cr.ClosePath();
            cr.MoveTo(295, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 150);
            cr.LineTo(335, 150);
            cr.LineTo(335, 160);
            cr.LineTo(345, 160);
            cr.ClosePath();
            cr.MoveTo(345, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 150);
            cr.LineTo(335, 150);
            cr.LineTo(335, 160);
            cr.LineTo(345, 160);
            cr.ClosePath();
            cr.MoveTo(345, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 160);
            cr.LineTo(205, 160);
            cr.LineTo(205, 170);
            cr.LineTo(215, 170);
            cr.ClosePath();
            cr.MoveTo(215, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 160);
            cr.LineTo(205, 160);
            cr.LineTo(205, 170);
            cr.LineTo(215, 170);
            cr.ClosePath();
            cr.MoveTo(215, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 160);
            cr.LineTo(245, 160);
            cr.LineTo(245, 170);
            cr.LineTo(255, 170);
            cr.ClosePath();
            cr.MoveTo(255, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 160);
            cr.LineTo(245, 160);
            cr.LineTo(245, 170);
            cr.LineTo(255, 170);
            cr.ClosePath();
            cr.MoveTo(255, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(285, 160);
            cr.LineTo(275, 160);
            cr.LineTo(275, 170);
            cr.LineTo(285, 170);
            cr.ClosePath();
            cr.MoveTo(285, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(285, 160);
            cr.LineTo(275, 160);
            cr.LineTo(275, 170);
            cr.LineTo(285, 170);
            cr.ClosePath();
            cr.MoveTo(285, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 170);
            cr.LineTo(185, 170);
            cr.LineTo(185, 180);
            cr.LineTo(195, 180);
            cr.ClosePath();
            cr.MoveTo(195, 170);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 170);
            cr.LineTo(185, 170);
            cr.LineTo(185, 180);
            cr.LineTo(195, 180);
            cr.ClosePath();
            cr.MoveTo(195, 170);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 170);
            cr.LineTo(265, 170);
            cr.LineTo(265, 180);
            cr.LineTo(275, 180);
            cr.ClosePath();
            cr.MoveTo(275, 170);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 170);
            cr.LineTo(265, 170);
            cr.LineTo(265, 180);
            cr.LineTo(275, 180);
            cr.ClosePath();
            cr.MoveTo(275, 170);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(265, 180);
            cr.LineTo(255, 180);
            cr.LineTo(255, 190);
            cr.LineTo(265, 190);
            cr.ClosePath();
            cr.MoveTo(265, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(265, 180);
            cr.LineTo(255, 180);
            cr.LineTo(255, 190);
            cr.LineTo(265, 190);
            cr.ClosePath();
            cr.MoveTo(265, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 190);
            cr.LineTo(215, 190);
            cr.LineTo(215, 200);
            cr.LineTo(225, 200);
            cr.ClosePath();
            cr.MoveTo(225, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 190);
            cr.LineTo(215, 190);
            cr.LineTo(215, 200);
            cr.LineTo(225, 200);
            cr.ClosePath();
            cr.MoveTo(225, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 190);
            cr.LineTo(245, 190);
            cr.LineTo(245, 200);
            cr.LineTo(255, 200);
            cr.ClosePath();
            cr.MoveTo(255, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 190);
            cr.LineTo(245, 190);
            cr.LineTo(245, 200);
            cr.LineTo(255, 200);
            cr.ClosePath();
            cr.MoveTo(255, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 200);
            cr.LineTo(225, 200);
            cr.LineTo(225, 210);
            cr.LineTo(245, 210);
            cr.ClosePath();
            cr.MoveTo(245, 200);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 200);
            cr.LineTo(335, 200);
            cr.LineTo(335, 210);
            cr.LineTo(345, 210);
            cr.ClosePath();
            cr.MoveTo(345, 200);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 200);
            cr.LineTo(335, 200);
            cr.LineTo(335, 210);
            cr.LineTo(345, 210);
            cr.ClosePath();
            cr.MoveTo(345, 200);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 210);
            cr.LineTo(125, 210);
            cr.LineTo(125, 220);
            cr.LineTo(135, 220);
            cr.ClosePath();
            cr.MoveTo(135, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 210);
            cr.LineTo(125, 210);
            cr.LineTo(125, 220);
            cr.LineTo(135, 220);
            cr.ClosePath();
            cr.MoveTo(135, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 210);
            cr.LineTo(185, 210);
            cr.LineTo(185, 220);
            cr.LineTo(195, 220);
            cr.ClosePath();
            cr.MoveTo(195, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 210);
            cr.LineTo(185, 210);
            cr.LineTo(185, 220);
            cr.LineTo(195, 220);
            cr.ClosePath();
            cr.MoveTo(195, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 210);
            cr.LineTo(215, 210);
            cr.LineTo(215, 220);
            cr.LineTo(225, 220);
            cr.ClosePath();
            cr.MoveTo(225, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 210);
            cr.LineTo(215, 210);
            cr.LineTo(215, 220);
            cr.LineTo(225, 220);
            cr.ClosePath();
            cr.MoveTo(225, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 210);
            cr.LineTo(285, 210);
            cr.LineTo(285, 220);
            cr.LineTo(295, 220);
            cr.ClosePath();
            cr.MoveTo(295, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(295, 210);
            cr.LineTo(285, 210);
            cr.LineTo(285, 220);
            cr.LineTo(295, 220);
            cr.ClosePath();
            cr.MoveTo(295, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(385, 210);
            cr.LineTo(375, 210);
            cr.LineTo(375, 220);
            cr.LineTo(385, 220);
            cr.ClosePath();
            cr.MoveTo(385, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(385, 210);
            cr.LineTo(375, 210);
            cr.LineTo(375, 220);
            cr.LineTo(385, 220);
            cr.ClosePath();
            cr.MoveTo(385, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 220);
            cr.LineTo(105, 220);
            cr.LineTo(105, 230);
            cr.LineTo(115, 230);
            cr.ClosePath();
            cr.MoveTo(115, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 220);
            cr.LineTo(105, 220);
            cr.LineTo(105, 230);
            cr.LineTo(115, 230);
            cr.ClosePath();
            cr.MoveTo(115, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 220);
            cr.LineTo(205, 220);
            cr.LineTo(205, 230);
            cr.LineTo(215, 230);
            cr.ClosePath();
            cr.MoveTo(215, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 220);
            cr.LineTo(205, 220);
            cr.LineTo(205, 230);
            cr.LineTo(215, 230);
            cr.ClosePath();
            cr.MoveTo(215, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(305, 220);
            cr.LineTo(295, 220);
            cr.LineTo(295, 240);
            cr.LineTo(305, 240);
            cr.ClosePath();
            cr.MoveTo(305, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 230);
            cr.LineTo(85, 230);
            cr.LineTo(85, 240);
            cr.LineTo(95, 240);
            cr.ClosePath();
            cr.MoveTo(95, 230);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 230);
            cr.LineTo(85, 230);
            cr.LineTo(85, 240);
            cr.LineTo(95, 240);
            cr.ClosePath();
            cr.MoveTo(95, 230);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 230);
            cr.LineTo(195, 230);
            cr.LineTo(195, 240);
            cr.LineTo(205, 240);
            cr.ClosePath();
            cr.MoveTo(205, 230);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 230);
            cr.LineTo(195, 230);
            cr.LineTo(195, 240);
            cr.LineTo(205, 240);
            cr.ClosePath();
            cr.MoveTo(205, 230);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 240);
            cr.LineTo(75, 240);
            cr.LineTo(75, 250);
            cr.LineTo(85, 250);
            cr.ClosePath();
            cr.MoveTo(85, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(85, 240);
            cr.LineTo(75, 240);
            cr.LineTo(75, 250);
            cr.LineTo(85, 250);
            cr.ClosePath();
            cr.MoveTo(85, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 240);
            cr.LineTo(155, 240);
            cr.LineTo(155, 250);
            cr.LineTo(165, 250);
            cr.ClosePath();
            cr.MoveTo(165, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 240);
            cr.LineTo(155, 240);
            cr.LineTo(155, 250);
            cr.LineTo(165, 250);
            cr.ClosePath();
            cr.MoveTo(165, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 240);
            cr.LineTo(185, 240);
            cr.LineTo(185, 250);
            cr.LineTo(195, 250);
            cr.ClosePath();
            cr.MoveTo(195, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 240);
            cr.LineTo(185, 240);
            cr.LineTo(185, 250);
            cr.LineTo(195, 250);
            cr.ClosePath();
            cr.MoveTo(195, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 240);
            cr.LineTo(265, 240);
            cr.LineTo(265, 250);
            cr.LineTo(275, 250);
            cr.ClosePath();
            cr.MoveTo(275, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(275, 240);
            cr.LineTo(265, 240);
            cr.LineTo(265, 250);
            cr.LineTo(275, 250);
            cr.ClosePath();
            cr.MoveTo(275, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(375, 240);
            cr.LineTo(365, 240);
            cr.LineTo(365, 250);
            cr.LineTo(375, 250);
            cr.ClosePath();
            cr.MoveTo(375, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(375, 240);
            cr.LineTo(365, 240);
            cr.LineTo(365, 250);
            cr.LineTo(375, 250);
            cr.ClosePath();
            cr.MoveTo(375, 240);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 250);
            cr.LineTo(85, 250);
            cr.LineTo(85, 260);
            cr.LineTo(95, 260);
            cr.ClosePath();
            cr.MoveTo(95, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(95, 250);
            cr.LineTo(85, 250);
            cr.LineTo(85, 260);
            cr.LineTo(95, 260);
            cr.ClosePath();
            cr.MoveTo(95, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 250);
            cr.LineTo(175, 250);
            cr.LineTo(175, 260);
            cr.LineTo(185, 260);
            cr.ClosePath();
            cr.MoveTo(185, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 250);
            cr.LineTo(175, 250);
            cr.LineTo(175, 260);
            cr.LineTo(185, 260);
            cr.ClosePath();
            cr.MoveTo(185, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(285, 250);
            cr.LineTo(275, 250);
            cr.LineTo(275, 270);
            cr.LineTo(285, 270);
            cr.ClosePath();
            cr.MoveTo(285, 250);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 260);
            cr.LineTo(95, 260);
            cr.LineTo(95, 270);
            cr.LineTo(105, 270);
            cr.ClosePath();
            cr.MoveTo(105, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(105, 260);
            cr.LineTo(95, 260);
            cr.LineTo(95, 270);
            cr.LineTo(105, 270);
            cr.ClosePath();
            cr.MoveTo(105, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 260);
            cr.LineTo(125, 260);
            cr.LineTo(125, 270);
            cr.LineTo(135, 270);
            cr.ClosePath();
            cr.MoveTo(135, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 260);
            cr.LineTo(125, 260);
            cr.LineTo(125, 270);
            cr.LineTo(135, 270);
            cr.ClosePath();
            cr.MoveTo(135, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 260);
            cr.LineTo(155, 260);
            cr.LineTo(155, 270);
            cr.LineTo(175, 270);
            cr.ClosePath();
            cr.MoveTo(175, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 260);
            cr.LineTo(245, 260);
            cr.LineTo(245, 270);
            cr.LineTo(255, 270);
            cr.ClosePath();
            cr.MoveTo(255, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(255, 260);
            cr.LineTo(245, 260);
            cr.LineTo(245, 270);
            cr.LineTo(255, 270);
            cr.ClosePath();
            cr.MoveTo(255, 260);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 270);
            cr.LineTo(105, 270);
            cr.LineTo(105, 280);
            cr.LineTo(115, 280);
            cr.ClosePath();
            cr.MoveTo(115, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(115, 270);
            cr.LineTo(105, 270);
            cr.LineTo(105, 280);
            cr.LineTo(115, 280);
            cr.ClosePath();
            cr.MoveTo(115, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(155, 270);
            cr.LineTo(145, 270);
            cr.LineTo(145, 280);
            cr.LineTo(155, 280);
            cr.ClosePath();
            cr.MoveTo(155, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(155, 270);
            cr.LineTo(145, 270);
            cr.LineTo(145, 280);
            cr.LineTo(155, 280);
            cr.ClosePath();
            cr.MoveTo(155, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(265, 270);
            cr.LineTo(255, 270);
            cr.LineTo(255, 290);
            cr.LineTo(265, 290);
            cr.ClosePath();
            cr.MoveTo(265, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 270);
            cr.LineTo(335, 270);
            cr.LineTo(335, 280);
            cr.LineTo(345, 280);
            cr.ClosePath();
            cr.MoveTo(345, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(345, 270);
            cr.LineTo(335, 270);
            cr.LineTo(335, 280);
            cr.LineTo(345, 280);
            cr.ClosePath();
            cr.MoveTo(345, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125, 280);
            cr.LineTo(115, 280);
            cr.LineTo(115, 290);
            cr.LineTo(125, 290);
            cr.ClosePath();
            cr.MoveTo(125, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125, 280);
            cr.LineTo(115, 280);
            cr.LineTo(115, 290);
            cr.LineTo(125, 290);
            cr.ClosePath();
            cr.MoveTo(125, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 280);
            cr.LineTo(135, 280);
            cr.LineTo(135, 290);
            cr.LineTo(145, 290);
            cr.ClosePath();
            cr.MoveTo(145, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 280);
            cr.LineTo(135, 280);
            cr.LineTo(135, 290);
            cr.LineTo(145, 290);
            cr.ClosePath();
            cr.MoveTo(145, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(235, 280);
            cr.LineTo(225, 280);
            cr.LineTo(225, 290);
            cr.LineTo(235, 290);
            cr.ClosePath();
            cr.MoveTo(235, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(235, 280);
            cr.LineTo(225, 280);
            cr.LineTo(225, 290);
            cr.LineTo(235, 290);
            cr.ClosePath();
            cr.MoveTo(235, 280);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 290);
            cr.LineTo(125, 290);
            cr.LineTo(125, 300);
            cr.LineTo(135, 300);
            cr.ClosePath();
            cr.MoveTo(135, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(135, 290);
            cr.LineTo(125, 290);
            cr.LineTo(125, 300);
            cr.LineTo(135, 300);
            cr.ClosePath();
            cr.MoveTo(135, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 290);
            cr.LineTo(195, 290);
            cr.LineTo(195, 300);
            cr.LineTo(205, 300);
            cr.ClosePath();
            cr.MoveTo(205, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 290);
            cr.LineTo(195, 290);
            cr.LineTo(195, 300);
            cr.LineTo(205, 300);
            cr.ClosePath();
            cr.MoveTo(205, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(245, 290);
            cr.LineTo(235, 290);
            cr.LineTo(235, 310);
            cr.LineTo(245, 310);
            cr.ClosePath();
            cr.MoveTo(245, 290);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 300);
            cr.LineTo(135, 300);
            cr.LineTo(135, 310);
            cr.LineTo(145, 310);
            cr.ClosePath();
            cr.MoveTo(145, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(145, 300);
            cr.LineTo(135, 300);
            cr.LineTo(135, 310);
            cr.LineTo(145, 310);
            cr.ClosePath();
            cr.MoveTo(145, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 300);
            cr.LineTo(165, 300);
            cr.LineTo(165, 310);
            cr.LineTo(175, 310);
            cr.ClosePath();
            cr.MoveTo(175, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 300);
            cr.LineTo(165, 300);
            cr.LineTo(165, 310);
            cr.LineTo(175, 310);
            cr.ClosePath();
            cr.MoveTo(175, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 300);
            cr.LineTo(205, 300);
            cr.LineTo(205, 310);
            cr.LineTo(215, 310);
            cr.ClosePath();
            cr.MoveTo(215, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(215, 300);
            cr.LineTo(205, 300);
            cr.LineTo(205, 310);
            cr.LineTo(215, 310);
            cr.ClosePath();
            cr.MoveTo(215, 300);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(155, 310);
            cr.LineTo(145, 310);
            cr.LineTo(145, 320);
            cr.LineTo(155, 320);
            cr.ClosePath();
            cr.MoveTo(155, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(155, 310);
            cr.LineTo(145, 310);
            cr.LineTo(145, 320);
            cr.LineTo(155, 320);
            cr.ClosePath();
            cr.MoveTo(155, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 310);
            cr.LineTo(175, 310);
            cr.LineTo(175, 320);
            cr.LineTo(185, 320);
            cr.ClosePath();
            cr.MoveTo(185, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 310);
            cr.LineTo(175, 310);
            cr.LineTo(175, 320);
            cr.LineTo(185, 320);
            cr.ClosePath();
            cr.MoveTo(185, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(225, 310);
            cr.LineTo(215, 310);
            cr.LineTo(215, 330);
            cr.LineTo(225, 330);
            cr.ClosePath();
            cr.MoveTo(225, 310);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 320);
            cr.LineTo(155, 320);
            cr.LineTo(155, 330);
            cr.LineTo(165, 330);
            cr.ClosePath();
            cr.MoveTo(165, 320);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(165, 320);
            cr.LineTo(155, 320);
            cr.LineTo(155, 330);
            cr.LineTo(165, 330);
            cr.ClosePath();
            cr.MoveTo(165, 320);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 320);
            cr.LineTo(185, 320);
            cr.LineTo(185, 330);
            cr.LineTo(195, 330);
            cr.ClosePath();
            cr.MoveTo(195, 320);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(195, 320);
            cr.LineTo(185, 320);
            cr.LineTo(185, 330);
            cr.LineTo(195, 330);
            cr.ClosePath();
            cr.MoveTo(195, 320);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 330);
            cr.LineTo(165, 330);
            cr.LineTo(165, 340);
            cr.LineTo(175, 340);
            cr.ClosePath();
            cr.MoveTo(175, 330);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(175, 330);
            cr.LineTo(165, 330);
            cr.LineTo(165, 340);
            cr.LineTo(175, 340);
            cr.ClosePath();
            cr.MoveTo(175, 330);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(205, 330);
            cr.LineTo(195, 330);
            cr.LineTo(195, 350);
            cr.LineTo(205, 350);
            cr.ClosePath();
            cr.MoveTo(205, 330);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 340);
            cr.LineTo(175, 340);
            cr.LineTo(175, 350);
            cr.LineTo(185, 350);
            cr.ClosePath();
            cr.MoveTo(185, 340);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(185, 340);
            cr.LineTo(175, 340);
            cr.LineTo(175, 350);
            cr.LineTo(185, 350);
            cr.ClosePath();
            cr.MoveTo(185, 340);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -86, -146);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawboots_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(170, 55);
            cr.LineTo(130, 55);
            cr.LineTo(130, 75);
            cr.LineTo(70, 75);
            cr.LineTo(70, 95);
            cr.LineTo(50, 95);
            cr.LineTo(50, 105);
            cr.LineTo(40, 105);
            cr.LineTo(40, 125);
            cr.LineTo(30, 125);
            cr.LineTo(30, 165);
            cr.LineTo(40, 165);
            cr.LineTo(40, 215);
            cr.LineTo(50, 215);
            cr.LineTo(50, 265);
            cr.LineTo(60, 265);
            cr.LineTo(60, 315);
            cr.LineTo(70, 315);
            cr.LineTo(70, 365);
            cr.LineTo(80, 365);
            cr.LineTo(80, 395);
            cr.LineTo(120, 395);
            cr.LineTo(120, 385);
            cr.LineTo(150, 385);
            cr.LineTo(150, 375);
            cr.LineTo(160, 375);
            cr.LineTo(160, 365);
            cr.LineTo(190, 365);
            cr.LineTo(190, 355);
            cr.LineTo(220, 355);
            cr.LineTo(220, 345);
            cr.LineTo(240, 345);
            cr.LineTo(240, 335);
            cr.LineTo(260, 335);
            cr.LineTo(260, 325);
            cr.LineTo(280, 325);
            cr.LineTo(280, 315);
            cr.LineTo(310, 315);
            cr.LineTo(310, 305);
            cr.LineTo(330, 305);
            cr.LineTo(330, 295);
            cr.LineTo(350, 295);
            cr.LineTo(350, 285);
            cr.LineTo(380, 285);
            cr.LineTo(380, 275);
            cr.LineTo(400, 275);
            cr.LineTo(400, 265);
            cr.LineTo(410, 265);
            cr.LineTo(410, 255);
            cr.LineTo(420, 255);
            cr.LineTo(420, 205);
            cr.LineTo(410, 205);
            cr.LineTo(410, 195);
            cr.LineTo(400, 195);
            cr.LineTo(400, 185);
            cr.LineTo(390, 185);
            cr.LineTo(390, 175);
            cr.LineTo(370, 175);
            cr.LineTo(370, 165);
            cr.LineTo(350, 165);
            cr.LineTo(350, 155);
            cr.LineTo(260, 155);
            cr.LineTo(260, 165);
            cr.LineTo(230, 165);
            cr.LineTo(230, 175);
            cr.LineTo(220, 175);
            cr.LineTo(220, 185);
            cr.LineTo(210, 185);
            cr.LineTo(210, 115);
            cr.LineTo(200, 115);
            cr.LineTo(200, 95);
            cr.LineTo(190, 95);
            cr.LineTo(190, 75);
            cr.LineTo(180, 75);
            cr.LineTo(180, 65);
            cr.LineTo(170, 65);
            cr.ClosePath();
            cr.MoveTo(90, 95);
            cr.LineTo(130, 95);
            cr.LineTo(130, 115);
            cr.LineTo(120, 115);
            cr.LineTo(120, 125);
            cr.LineTo(110, 125);
            cr.LineTo(110, 145);
            cr.LineTo(120, 145);
            cr.LineTo(120, 135);
            cr.LineTo(140, 135);
            cr.LineTo(140, 155);
            cr.LineTo(130, 155);
            cr.LineTo(130, 165);
            cr.LineTo(110, 165);
            cr.LineTo(110, 175);
            cr.LineTo(140, 175);
            cr.LineTo(140, 165);
            cr.LineTo(150, 165);
            cr.LineTo(150, 185);
            cr.LineTo(160, 185);
            cr.LineTo(160, 195);
            cr.LineTo(140, 195);
            cr.LineTo(140, 215);
            cr.LineTo(130, 215);
            cr.LineTo(130, 235);
            cr.LineTo(140, 235);
            cr.LineTo(140, 225);
            cr.LineTo(150, 225);
            cr.LineTo(150, 215);
            cr.LineTo(160, 215);
            cr.LineTo(160, 205);
            cr.LineTo(170, 205);
            cr.LineTo(170, 225);
            cr.LineTo(160, 225);
            cr.LineTo(160, 265);
            cr.LineTo(170, 265);
            cr.LineTo(170, 235);
            cr.LineTo(180, 235);
            cr.LineTo(180, 225);
            cr.LineTo(190, 225);
            cr.LineTo(190, 235);
            cr.LineTo(200, 235);
            cr.LineTo(200, 225);
            cr.LineTo(210, 225);
            cr.LineTo(210, 215);
            cr.LineTo(220, 215);
            cr.LineTo(220, 205);
            cr.LineTo(230, 205);
            cr.LineTo(230, 195);
            cr.LineTo(240, 195);
            cr.LineTo(240, 185);
            cr.LineTo(250, 185);
            cr.LineTo(250, 205);
            cr.LineTo(240, 205);
            cr.LineTo(240, 255);
            cr.LineTo(230, 255);
            cr.LineTo(230, 265);
            cr.LineTo(250, 265);
            cr.LineTo(250, 275);
            cr.LineTo(240, 275);
            cr.LineTo(240, 285);
            cr.LineTo(230, 285);
            cr.LineTo(230, 295);
            cr.LineTo(210, 295);
            cr.LineTo(210, 305);
            cr.LineTo(190, 305);
            cr.LineTo(190, 315);
            cr.LineTo(170, 315);
            cr.LineTo(170, 325);
            cr.LineTo(160, 325);
            cr.LineTo(160, 335);
            cr.LineTo(140, 335);
            cr.LineTo(140, 345);
            cr.LineTo(120, 345);
            cr.LineTo(120, 355);
            cr.LineTo(90, 355);
            cr.LineTo(90, 345);
            cr.LineTo(110, 345);
            cr.LineTo(110, 335);
            cr.LineTo(90, 335);
            cr.LineTo(90, 325);
            cr.LineTo(100, 325);
            cr.LineTo(100, 315);
            cr.LineTo(90, 315);
            cr.LineTo(90, 305);
            cr.LineTo(80, 305);
            cr.LineTo(80, 295);
            cr.LineTo(90, 295);
            cr.LineTo(90, 285);
            cr.LineTo(80, 285);
            cr.LineTo(80, 265);
            cr.LineTo(70, 265);
            cr.LineTo(70, 245);
            cr.LineTo(80, 245);
            cr.LineTo(80, 235);
            cr.LineTo(70, 235);
            cr.LineTo(70, 215);
            cr.LineTo(60, 215);
            cr.LineTo(60, 195);
            cr.LineTo(70, 195);
            cr.LineTo(70, 185);
            cr.LineTo(60, 185);
            cr.LineTo(60, 165);
            cr.LineTo(50, 165);
            cr.LineTo(50, 145);
            cr.LineTo(60, 145);
            cr.LineTo(60, 135);
            cr.LineTo(70, 135);
            cr.LineTo(70, 125);
            cr.LineTo(60, 125);
            cr.LineTo(60, 115);
            cr.LineTo(80, 115);
            cr.LineTo(80, 105);
            cr.LineTo(90, 105);
            cr.ClosePath();
            cr.MoveTo(150, 115);
            cr.LineTo(150, 85);
            cr.LineTo(160, 85);
            cr.LineTo(160, 95);
            cr.LineTo(170, 95);
            cr.LineTo(170, 105);
            cr.LineTo(160, 105);
            cr.LineTo(160, 115);
            cr.ClosePath();
            cr.MoveTo(160, 145);
            cr.LineTo(160, 125);
            cr.LineTo(180, 125);
            cr.LineTo(180, 145);
            cr.ClosePath();
            cr.MoveTo(170, 175);
            cr.LineTo(170, 165);
            cr.LineTo(190, 165);
            cr.LineTo(190, 175);
            cr.ClosePath();
            cr.MoveTo(180, 205);
            cr.LineTo(180, 195);
            cr.LineTo(190, 195);
            cr.LineTo(190, 205);
            cr.ClosePath();
            cr.MoveTo(270, 205);
            cr.LineTo(270, 185);
            cr.LineTo(280, 185);
            cr.LineTo(280, 175);
            cr.LineTo(330, 175);
            cr.LineTo(330, 185);
            cr.LineTo(360, 185);
            cr.LineTo(360, 195);
            cr.LineTo(370, 195);
            cr.LineTo(370, 205);
            cr.LineTo(390, 205);
            cr.LineTo(390, 225);
            cr.LineTo(400, 225);
            cr.LineTo(400, 235);
            cr.LineTo(340, 235);
            cr.LineTo(340, 245);
            cr.LineTo(310, 245);
            cr.LineTo(310, 255);
            cr.LineTo(260, 255);
            cr.LineTo(260, 205);
            cr.ClosePath();
            cr.MoveTo(270, 205);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70, 155);
            cr.LineTo(60, 155);
            cr.LineTo(60, 165);
            cr.LineTo(70, 165);
            cr.ClosePath();
            cr.MoveTo(70, 155);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70, 155);
            cr.LineTo(60, 155);
            cr.LineTo(60, 165);
            cr.LineTo(70, 165);
            cr.ClosePath();
            cr.MoveTo(70, 155);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(80, 175);
            cr.LineTo(70, 175);
            cr.LineTo(70, 185);
            cr.LineTo(80, 185);
            cr.ClosePath();
            cr.MoveTo(80, 175);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(80, 175);
            cr.LineTo(70, 175);
            cr.LineTo(70, 185);
            cr.LineTo(80, 185);
            cr.ClosePath();
            cr.MoveTo(80, 175);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(80, 205);
            cr.LineTo(70, 205);
            cr.LineTo(70, 215);
            cr.LineTo(80, 215);
            cr.ClosePath();
            cr.MoveTo(80, 205);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(80, 205);
            cr.LineTo(70, 205);
            cr.LineTo(70, 215);
            cr.LineTo(80, 215);
            cr.ClosePath();
            cr.MoveTo(80, 205);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(370, 215);
            cr.LineTo(320, 215);
            cr.LineTo(320, 225);
            cr.LineTo(370, 225);
            cr.ClosePath();
            cr.MoveTo(370, 215);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(90, 225);
            cr.LineTo(80, 225);
            cr.LineTo(80, 235);
            cr.LineTo(90, 235);
            cr.ClosePath();
            cr.MoveTo(90, 225);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(90, 225);
            cr.LineTo(80, 225);
            cr.LineTo(80, 235);
            cr.LineTo(90, 235);
            cr.ClosePath();
            cr.MoveTo(90, 225);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(320, 225);
            cr.LineTo(310, 225);
            cr.LineTo(310, 235);
            cr.LineTo(320, 235);
            cr.ClosePath();
            cr.MoveTo(320, 225);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(320, 225);
            cr.LineTo(310, 225);
            cr.LineTo(310, 235);
            cr.LineTo(320, 235);
            cr.ClosePath();
            cr.MoveTo(320, 225);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(190, 235);
            cr.LineTo(180, 235);
            cr.LineTo(180, 255);
            cr.LineTo(190, 255);
            cr.ClosePath();
            cr.MoveTo(190, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(290, 235);
            cr.LineTo(280, 235);
            cr.LineTo(280, 245);
            cr.LineTo(290, 245);
            cr.ClosePath();
            cr.MoveTo(290, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(290, 235);
            cr.LineTo(280, 235);
            cr.LineTo(280, 245);
            cr.LineTo(290, 245);
            cr.ClosePath();
            cr.MoveTo(290, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(90, 255);
            cr.LineTo(80, 255);
            cr.LineTo(80, 265);
            cr.LineTo(90, 265);
            cr.ClosePath();
            cr.MoveTo(90, 255);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(90, 255);
            cr.LineTo(80, 255);
            cr.LineTo(80, 265);
            cr.LineTo(90, 265);
            cr.ClosePath();
            cr.MoveTo(90, 255);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(230, 265);
            cr.LineTo(210, 265);
            cr.LineTo(210, 275);
            cr.LineTo(230, 275);
            cr.ClosePath();
            cr.MoveTo(230, 265);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100, 275);
            cr.LineTo(90, 275);
            cr.LineTo(90, 285);
            cr.LineTo(100, 285);
            cr.ClosePath();
            cr.MoveTo(100, 275);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100, 275);
            cr.LineTo(90, 275);
            cr.LineTo(90, 285);
            cr.LineTo(100, 285);
            cr.ClosePath();
            cr.MoveTo(100, 275);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(210, 275);
            cr.LineTo(200, 275);
            cr.LineTo(200, 285);
            cr.LineTo(210, 285);
            cr.ClosePath();
            cr.MoveTo(210, 275);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(210, 275);
            cr.LineTo(200, 275);
            cr.LineTo(200, 285);
            cr.LineTo(210, 285);
            cr.ClosePath();
            cr.MoveTo(210, 275);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(200, 285);
            cr.LineTo(170, 285);
            cr.LineTo(170, 295);
            cr.LineTo(200, 295);
            cr.ClosePath();
            cr.MoveTo(200, 285);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(170, 295);
            cr.LineTo(160, 295);
            cr.LineTo(160, 305);
            cr.LineTo(170, 305);
            cr.ClosePath();
            cr.MoveTo(170, 295);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(170, 295);
            cr.LineTo(160, 295);
            cr.LineTo(160, 305);
            cr.LineTo(170, 305);
            cr.ClosePath();
            cr.MoveTo(170, 295);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(110, 305);
            cr.LineTo(100, 305);
            cr.LineTo(100, 315);
            cr.LineTo(110, 315);
            cr.ClosePath();
            cr.MoveTo(110, 305);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(110, 305);
            cr.LineTo(100, 305);
            cr.LineTo(100, 315);
            cr.LineTo(110, 315);
            cr.ClosePath();
            cr.MoveTo(110, 305);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(160, 305);
            cr.LineTo(140, 305);
            cr.LineTo(140, 315);
            cr.LineTo(160, 315);
            cr.ClosePath();
            cr.MoveTo(160, 305);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(140, 315);
            cr.LineTo(120, 315);
            cr.LineTo(120, 325);
            cr.LineTo(110, 325);
            cr.LineTo(110, 335);
            cr.LineTo(130, 335);
            cr.LineTo(130, 325);
            cr.LineTo(140, 325);
            cr.ClosePath();
            cr.MoveTo(140, 315);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void Drawhat_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(260, 90);
            cr.LineTo(230, 90);
            cr.LineTo(230, 100);
            cr.LineTo(210, 100);
            cr.LineTo(210, 110);
            cr.LineTo(190, 110);
            cr.LineTo(190, 120);
            cr.LineTo(170, 120);
            cr.LineTo(170, 130);
            cr.LineTo(150, 130);
            cr.LineTo(150, 140);
            cr.LineTo(120, 140);
            cr.LineTo(120, 150);
            cr.LineTo(100, 150);
            cr.LineTo(100, 160);
            cr.LineTo(90, 160);
            cr.LineTo(90, 240);
            cr.LineTo(100, 240);
            cr.LineTo(100, 300);
            cr.LineTo(70, 300);
            cr.LineTo(70, 310);
            cr.LineTo(60, 310);
            cr.LineTo(60, 300);
            cr.LineTo(30, 300);
            cr.LineTo(30, 330);
            cr.LineTo(40, 330);
            cr.LineTo(40, 340);
            cr.LineTo(60, 340);
            cr.LineTo(60, 360);
            cr.LineTo(150, 360);
            cr.LineTo(150, 350);
            cr.LineTo(170, 350);
            cr.LineTo(170, 340);
            cr.LineTo(190, 340);
            cr.LineTo(190, 330);
            cr.LineTo(210, 330);
            cr.LineTo(210, 320);
            cr.LineTo(230, 320);
            cr.LineTo(230, 310);
            cr.LineTo(250, 310);
            cr.LineTo(250, 300);
            cr.LineTo(270, 300);
            cr.LineTo(270, 290);
            cr.LineTo(290, 290);
            cr.LineTo(290, 280);
            cr.LineTo(320, 280);
            cr.LineTo(320, 270);
            cr.LineTo(340, 270);
            cr.LineTo(340, 260);
            cr.LineTo(360, 260);
            cr.LineTo(360, 250);
            cr.LineTo(380, 250);
            cr.LineTo(380, 240);
            cr.LineTo(400, 240);
            cr.LineTo(400, 230);
            cr.LineTo(410, 230);
            cr.LineTo(410, 220);
            cr.LineTo(420, 220);
            cr.LineTo(420, 150);
            cr.LineTo(410, 150);
            cr.LineTo(410, 140);
            cr.LineTo(390, 140);
            cr.LineTo(390, 160);
            cr.LineTo(380, 160);
            cr.LineTo(380, 170);
            cr.LineTo(370, 170);
            cr.LineTo(370, 180);
            cr.LineTo(340, 180);
            cr.LineTo(340, 170);
            cr.LineTo(330, 170);
            cr.LineTo(330, 160);
            cr.LineTo(320, 160);
            cr.LineTo(320, 150);
            cr.LineTo(310, 150);
            cr.LineTo(310, 140);
            cr.LineTo(300, 140);
            cr.LineTo(300, 130);
            cr.LineTo(290, 130);
            cr.LineTo(290, 120);
            cr.LineTo(280, 120);
            cr.LineTo(280, 110);
            cr.LineTo(270, 110);
            cr.LineTo(270, 100);
            cr.LineTo(260, 100);
            cr.ClosePath();
            cr.MoveTo(220, 120);
            cr.LineTo(250, 120);
            cr.LineTo(250, 130);
            cr.LineTo(240, 130);
            cr.LineTo(240, 140);
            cr.LineTo(260, 140);
            cr.LineTo(260, 150);
            cr.LineTo(280, 150);
            cr.LineTo(280, 170);
            cr.LineTo(300, 170);
            cr.LineTo(300, 180);
            cr.LineTo(310, 180);
            cr.LineTo(310, 190);
            cr.LineTo(320, 190);
            cr.LineTo(320, 200);
            cr.LineTo(310, 200);
            cr.LineTo(310, 210);
            cr.LineTo(280, 210);
            cr.LineTo(280, 220);
            cr.LineTo(260, 220);
            cr.LineTo(260, 230);
            cr.LineTo(250, 230);
            cr.LineTo(250, 240);
            cr.LineTo(210, 240);
            cr.LineTo(210, 250);
            cr.LineTo(200, 250);
            cr.LineTo(200, 260);
            cr.LineTo(160, 260);
            cr.LineTo(160, 270);
            cr.LineTo(140, 270);
            cr.LineTo(140, 280);
            cr.LineTo(130, 280);
            cr.LineTo(130, 290);
            cr.LineTo(120, 290);
            cr.LineTo(120, 230);
            cr.LineTo(110, 230);
            cr.LineTo(110, 180);
            cr.LineTo(120, 180);
            cr.LineTo(120, 170);
            cr.LineTo(140, 170);
            cr.LineTo(140, 160);
            cr.LineTo(160, 160);
            cr.LineTo(160, 150);
            cr.LineTo(180, 150);
            cr.LineTo(180, 140);
            cr.LineTo(200, 140);
            cr.LineTo(200, 130);
            cr.LineTo(220, 130);
            cr.ClosePath();
            cr.MoveTo(320, 210);
            cr.LineTo(340, 210);
            cr.LineTo(340, 200);
            cr.LineTo(370, 200);
            cr.LineTo(370, 190);
            cr.LineTo(390, 190);
            cr.LineTo(390, 170);
            cr.LineTo(400, 170);
            cr.LineTo(400, 210);
            cr.LineTo(390, 210);
            cr.LineTo(390, 220);
            cr.LineTo(370, 220);
            cr.LineTo(370, 230);
            cr.LineTo(360, 230);
            cr.LineTo(360, 220);
            cr.LineTo(370, 220);
            cr.LineTo(370, 210);
            cr.LineTo(340, 210);
            cr.LineTo(340, 220);
            cr.LineTo(320, 220);
            cr.LineTo(320, 230);
            cr.LineTo(300, 230);
            cr.LineTo(300, 240);
            cr.LineTo(280, 240);
            cr.LineTo(280, 250);
            cr.LineTo(260, 250);
            cr.LineTo(260, 260);
            cr.LineTo(240, 260);
            cr.LineTo(240, 270);
            cr.LineTo(220, 270);
            cr.LineTo(220, 280);
            cr.LineTo(190, 280);
            cr.LineTo(190, 290);
            cr.LineTo(170, 290);
            cr.LineTo(170, 300);
            cr.LineTo(150, 300);
            cr.LineTo(150, 310);
            cr.LineTo(130, 310);
            cr.LineTo(130, 320);
            cr.LineTo(120, 320);
            cr.LineTo(120, 340);
            cr.LineTo(70, 340);
            cr.LineTo(70, 330);
            cr.LineTo(90, 330);
            cr.LineTo(90, 320);
            cr.LineTo(110, 320);
            cr.LineTo(110, 310);
            cr.LineTo(130, 310);
            cr.LineTo(130, 300);
            cr.LineTo(150, 300);
            cr.LineTo(150, 290);
            cr.LineTo(170, 290);
            cr.LineTo(170, 280);
            cr.LineTo(190, 280);
            cr.LineTo(190, 270);
            cr.LineTo(220, 270);
            cr.LineTo(220, 260);
            cr.LineTo(240, 260);
            cr.LineTo(240, 250);
            cr.LineTo(260, 250);
            cr.LineTo(260, 240);
            cr.LineTo(280, 240);
            cr.LineTo(280, 230);
            cr.LineTo(300, 230);
            cr.LineTo(300, 220);
            cr.LineTo(320, 220);
            cr.ClosePath();
            cr.MoveTo(320, 210);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(240, 140);
            cr.LineTo(230, 140);
            cr.LineTo(230, 150);
            cr.LineTo(240, 150);
            cr.ClosePath();
            cr.MoveTo(240, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(240, 140);
            cr.LineTo(230, 140);
            cr.LineTo(230, 150);
            cr.LineTo(240, 150);
            cr.ClosePath();
            cr.MoveTo(240, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(260, 150);
            cr.LineTo(250, 150);
            cr.LineTo(250, 160);
            cr.LineTo(260, 160);
            cr.ClosePath();
            cr.MoveTo(260, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(260, 150);
            cr.LineTo(250, 150);
            cr.LineTo(250, 160);
            cr.LineTo(260, 160);
            cr.ClosePath();
            cr.MoveTo(260, 150);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(250, 160);
            cr.LineTo(240, 160);
            cr.LineTo(240, 170);
            cr.LineTo(250, 170);
            cr.ClosePath();
            cr.MoveTo(250, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(250, 160);
            cr.LineTo(240, 160);
            cr.LineTo(240, 170);
            cr.LineTo(250, 170);
            cr.ClosePath();
            cr.MoveTo(250, 160);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(280, 170);
            cr.LineTo(270, 170);
            cr.LineTo(270, 180);
            cr.LineTo(280, 180);
            cr.ClosePath();
            cr.MoveTo(280, 170);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(280, 170);
            cr.LineTo(270, 170);
            cr.LineTo(270, 180);
            cr.LineTo(280, 180);
            cr.ClosePath();
            cr.MoveTo(280, 170);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(270, 180);
            cr.LineTo(260, 180);
            cr.LineTo(260, 190);
            cr.LineTo(270, 190);
            cr.ClosePath();
            cr.MoveTo(270, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(270, 180);
            cr.LineTo(260, 180);
            cr.LineTo(260, 190);
            cr.LineTo(270, 190);
            cr.ClosePath();
            cr.MoveTo(270, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(300, 180);
            cr.LineTo(290, 180);
            cr.LineTo(290, 190);
            cr.LineTo(300, 190);
            cr.ClosePath();
            cr.MoveTo(300, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(300, 180);
            cr.LineTo(290, 180);
            cr.LineTo(290, 190);
            cr.LineTo(300, 190);
            cr.ClosePath();
            cr.MoveTo(300, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(130, 190);
            cr.LineTo(120, 190);
            cr.LineTo(120, 200);
            cr.LineTo(130, 200);
            cr.ClosePath();
            cr.MoveTo(130, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(130, 190);
            cr.LineTo(120, 190);
            cr.LineTo(120, 200);
            cr.LineTo(130, 200);
            cr.ClosePath();
            cr.MoveTo(130, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(290, 190);
            cr.LineTo(280, 190);
            cr.LineTo(280, 200);
            cr.LineTo(290, 200);
            cr.ClosePath();
            cr.MoveTo(290, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(290, 190);
            cr.LineTo(280, 190);
            cr.LineTo(280, 200);
            cr.LineTo(290, 200);
            cr.ClosePath();
            cr.MoveTo(290, 190);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(140, 200);
            cr.LineTo(130, 200);
            cr.LineTo(130, 230);
            cr.LineTo(140, 230);
            cr.LineTo(140, 250);
            cr.LineTo(170, 250);
            cr.LineTo(170, 240);
            cr.LineTo(160, 240);
            cr.LineTo(160, 230);
            cr.LineTo(150, 230);
            cr.LineTo(150, 220);
            cr.LineTo(140, 220);
            cr.ClosePath();
            cr.MoveTo(140, 200);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawgloves_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(330, 100);
            cr.LineTo(250, 100);
            cr.LineTo(250, 110);
            cr.LineTo(220, 110);
            cr.LineTo(220, 120);
            cr.LineTo(200, 120);
            cr.LineTo(200, 130);
            cr.LineTo(190, 130);
            cr.LineTo(190, 140);
            cr.LineTo(170, 140);
            cr.LineTo(170, 150);
            cr.LineTo(160, 150);
            cr.LineTo(160, 160);
            cr.LineTo(150, 160);
            cr.LineTo(150, 170);
            cr.LineTo(140, 170);
            cr.LineTo(140, 180);
            cr.LineTo(130, 180);
            cr.LineTo(130, 190);
            cr.LineTo(120, 190);
            cr.LineTo(120, 200);
            cr.LineTo(110, 200);
            cr.LineTo(110, 210);
            cr.LineTo(100, 210);
            cr.LineTo(100, 220);
            cr.LineTo(90, 220);
            cr.LineTo(90, 230);
            cr.LineTo(80, 230);
            cr.LineTo(80, 240);
            cr.LineTo(70, 240);
            cr.LineTo(70, 250);
            cr.LineTo(60, 250);
            cr.LineTo(60, 260);
            cr.LineTo(50, 260);
            cr.LineTo(50, 270);
            cr.LineTo(40, 270);
            cr.LineTo(40, 310);
            cr.LineTo(50, 310);
            cr.LineTo(50, 320);
            cr.LineTo(60, 320);
            cr.LineTo(60, 330);
            cr.LineTo(70, 330);
            cr.LineTo(70, 340);
            cr.LineTo(80, 340);
            cr.LineTo(80, 350);
            cr.LineTo(140, 350);
            cr.LineTo(140, 340);
            cr.LineTo(150, 340);
            cr.LineTo(150, 330);
            cr.LineTo(160, 330);
            cr.LineTo(160, 320);
            cr.LineTo(170, 320);
            cr.LineTo(170, 310);
            cr.LineTo(180, 310);
            cr.LineTo(180, 300);
            cr.LineTo(190, 300);
            cr.LineTo(190, 310);
            cr.LineTo(200, 310);
            cr.LineTo(200, 320);
            cr.LineTo(210, 320);
            cr.LineTo(210, 330);
            cr.LineTo(230, 330);
            cr.LineTo(230, 340);
            cr.LineTo(270, 340);
            cr.LineTo(270, 330);
            cr.LineTo(280, 330);
            cr.LineTo(280, 320);
            cr.LineTo(290, 320);
            cr.LineTo(290, 290);
            cr.LineTo(280, 290);
            cr.LineTo(280, 280);
            cr.LineTo(300, 280);
            cr.LineTo(300, 310);
            cr.LineTo(350, 310);
            cr.LineTo(350, 300);
            cr.LineTo(370, 300);
            cr.LineTo(370, 290);
            cr.LineTo(380, 290);
            cr.LineTo(380, 280);
            cr.LineTo(390, 280);
            cr.LineTo(390, 270);
            cr.LineTo(400, 270);
            cr.LineTo(400, 240);
            cr.LineTo(410, 240);
            cr.LineTo(410, 170);
            cr.LineTo(400, 170);
            cr.LineTo(400, 160);
            cr.LineTo(390, 160);
            cr.LineTo(390, 150);
            cr.LineTo(380, 150);
            cr.LineTo(380, 140);
            cr.LineTo(370, 140);
            cr.LineTo(370, 130);
            cr.LineTo(360, 130);
            cr.LineTo(360, 120);
            cr.LineTo(350, 120);
            cr.LineTo(350, 110);
            cr.LineTo(330, 110);
            cr.ClosePath();
            cr.MoveTo(250, 120);
            cr.LineTo(340, 120);
            cr.LineTo(340, 130);
            cr.LineTo(350, 130);
            cr.LineTo(350, 140);
            cr.LineTo(360, 140);
            cr.LineTo(360, 150);
            cr.LineTo(350, 150);
            cr.LineTo(350, 160);
            cr.LineTo(370, 160);
            cr.LineTo(370, 170);
            cr.LineTo(380, 170);
            cr.LineTo(380, 180);
            cr.LineTo(390, 180);
            cr.LineTo(390, 200);
            cr.LineTo(400, 200);
            cr.LineTo(400, 220);
            cr.LineTo(390, 220);
            cr.LineTo(390, 210);
            cr.LineTo(380, 210);
            cr.LineTo(380, 200);
            cr.LineTo(370, 200);
            cr.LineTo(370, 190);
            cr.LineTo(350, 190);
            cr.LineTo(350, 200);
            cr.LineTo(360, 200);
            cr.LineTo(360, 210);
            cr.LineTo(370, 210);
            cr.LineTo(370, 220);
            cr.LineTo(380, 220);
            cr.LineTo(380, 230);
            cr.LineTo(390, 230);
            cr.LineTo(390, 250);
            cr.LineTo(370, 250);
            cr.LineTo(370, 280);
            cr.LineTo(330, 280);
            cr.LineTo(330, 270);
            cr.LineTo(320, 270);
            cr.LineTo(320, 260);
            cr.LineTo(270, 260);
            cr.LineTo(270, 270);
            cr.LineTo(240, 270);
            cr.LineTo(240, 280);
            cr.LineTo(220, 280);
            cr.LineTo(220, 290);
            cr.LineTo(230, 290);
            cr.LineTo(230, 300);
            cr.LineTo(270, 300);
            cr.LineTo(270, 320);
            cr.LineTo(230, 320);
            cr.LineTo(230, 310);
            cr.LineTo(220, 310);
            cr.LineTo(220, 300);
            cr.LineTo(210, 300);
            cr.LineTo(210, 290);
            cr.LineTo(200, 290);
            cr.LineTo(200, 280);
            cr.LineTo(180, 280);
            cr.LineTo(180, 290);
            cr.LineTo(130, 290);
            cr.LineTo(130, 280);
            cr.LineTo(120, 280);
            cr.LineTo(120, 270);
            cr.LineTo(110, 270);
            cr.LineTo(110, 250);
            cr.LineTo(100, 250);
            cr.LineTo(100, 240);
            cr.LineTo(110, 240);
            cr.LineTo(110, 230);
            cr.LineTo(120, 230);
            cr.LineTo(120, 220);
            cr.LineTo(130, 220);
            cr.LineTo(130, 210);
            cr.LineTo(140, 210);
            cr.LineTo(140, 240);
            cr.LineTo(150, 240);
            cr.LineTo(150, 190);
            cr.LineTo(160, 190);
            cr.LineTo(160, 220);
            cr.LineTo(170, 220);
            cr.LineTo(170, 170);
            cr.LineTo(180, 170);
            cr.LineTo(180, 200);
            cr.LineTo(190, 200);
            cr.LineTo(190, 150);
            cr.LineTo(210, 150);
            cr.LineTo(210, 140);
            cr.LineTo(220, 140);
            cr.LineTo(220, 130);
            cr.LineTo(250, 130);
            cr.ClosePath();
            cr.MoveTo(80, 270);
            cr.LineTo(90, 270);
            cr.LineTo(90, 290);
            cr.LineTo(100, 290);
            cr.LineTo(100, 300);
            cr.LineTo(110, 300);
            cr.LineTo(110, 310);
            cr.LineTo(150, 310);
            cr.LineTo(150, 320);
            cr.LineTo(130, 320);
            cr.LineTo(130, 330);
            cr.LineTo(90, 330);
            cr.LineTo(90, 320);
            cr.LineTo(80, 320);
            cr.LineTo(80, 310);
            cr.LineTo(70, 310);
            cr.LineTo(70, 280);
            cr.LineTo(80, 280);
            cr.ClosePath();
            cr.MoveTo(80, 270);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(350, 140);
            cr.LineTo(340, 140);
            cr.LineTo(340, 150);
            cr.LineTo(350, 150);
            cr.ClosePath();
            cr.MoveTo(350, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(350, 140);
            cr.LineTo(340, 140);
            cr.LineTo(340, 150);
            cr.LineTo(350, 150);
            cr.ClosePath();
            cr.MoveTo(350, 140);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(350, 180);
            cr.LineTo(330, 180);
            cr.LineTo(330, 190);
            cr.LineTo(350, 190);
            cr.ClosePath();
            cr.MoveTo(350, 180);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(340, 220);
            cr.LineTo(310, 220);
            cr.LineTo(310, 230);
            cr.LineTo(330, 230);
            cr.LineTo(330, 240);
            cr.LineTo(350, 240);
            cr.LineTo(350, 250);
            cr.LineTo(370, 250);
            cr.LineTo(370, 240);
            cr.LineTo(360, 240);
            cr.LineTo(360, 230);
            cr.LineTo(340, 230);
            cr.ClosePath();
            cr.MoveTo(340, 220);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawtrousers_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 450;
            float h = 450;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 55);
            cr.LineTo(284, 55);
            cr.LineTo(284, 65);
            cr.LineTo(264, 65);
            cr.LineTo(264, 75);
            cr.LineTo(244, 75);
            cr.LineTo(244, 85);
            cr.LineTo(224, 85);
            cr.LineTo(224, 95);
            cr.LineTo(214, 95);
            cr.LineTo(214, 105);
            cr.LineTo(194, 105);
            cr.LineTo(194, 115);
            cr.LineTo(174, 115);
            cr.LineTo(174, 125);
            cr.LineTo(144, 125);
            cr.LineTo(144, 135);
            cr.LineTo(124, 135);
            cr.LineTo(124, 145);
            cr.LineTo(94, 145);
            cr.LineTo(94, 155);
            cr.LineTo(74, 155);
            cr.LineTo(74, 165);
            cr.LineTo(54, 165);
            cr.LineTo(54, 175);
            cr.LineTo(34, 175);
            cr.LineTo(34, 185);
            cr.LineTo(24, 185);
            cr.LineTo(24, 225);
            cr.LineTo(34, 225);
            cr.LineTo(34, 235);
            cr.LineTo(44, 235);
            cr.LineTo(44, 245);
            cr.LineTo(54, 245);
            cr.LineTo(54, 255);
            cr.LineTo(94, 255);
            cr.LineTo(94, 245);
            cr.LineTo(134, 245);
            cr.LineTo(134, 235);
            cr.LineTo(164, 235);
            cr.LineTo(164, 225);
            cr.LineTo(204, 225);
            cr.LineTo(204, 215);
            cr.LineTo(244, 215);
            cr.LineTo(244, 205);
            cr.LineTo(254, 205);
            cr.LineTo(254, 215);
            cr.LineTo(244, 215);
            cr.LineTo(244, 225);
            cr.LineTo(224, 225);
            cr.LineTo(224, 235);
            cr.LineTo(204, 235);
            cr.LineTo(204, 245);
            cr.LineTo(194, 245);
            cr.LineTo(194, 255);
            cr.LineTo(184, 255);
            cr.LineTo(184, 265);
            cr.LineTo(164, 265);
            cr.LineTo(164, 275);
            cr.LineTo(154, 275);
            cr.LineTo(154, 285);
            cr.LineTo(134, 285);
            cr.LineTo(134, 295);
            cr.LineTo(124, 295);
            cr.LineTo(124, 305);
            cr.LineTo(114, 305);
            cr.LineTo(114, 315);
            cr.LineTo(94, 315);
            cr.LineTo(94, 375);
            cr.LineTo(104, 375);
            cr.LineTo(104, 385);
            cr.LineTo(114, 385);
            cr.LineTo(114, 395);
            cr.LineTo(144, 395);
            cr.LineTo(144, 385);
            cr.LineTo(164, 385);
            cr.LineTo(164, 375);
            cr.LineTo(184, 375);
            cr.LineTo(184, 365);
            cr.LineTo(204, 365);
            cr.LineTo(204, 355);
            cr.LineTo(224, 355);
            cr.LineTo(224, 345);
            cr.LineTo(244, 345);
            cr.LineTo(244, 335);
            cr.LineTo(264, 335);
            cr.LineTo(264, 325);
            cr.LineTo(284, 325);
            cr.LineTo(284, 315);
            cr.LineTo(294, 315);
            cr.LineTo(294, 305);
            cr.LineTo(314, 305);
            cr.LineTo(314, 295);
            cr.LineTo(334, 295);
            cr.LineTo(334, 285);
            cr.LineTo(354, 285);
            cr.LineTo(354, 275);
            cr.LineTo(374, 275);
            cr.LineTo(374, 265);
            cr.LineTo(404, 265);
            cr.LineTo(404, 255);
            cr.LineTo(414, 255);
            cr.LineTo(414, 245);
            cr.LineTo(424, 245);
            cr.LineTo(424, 225);
            cr.LineTo(414, 225);
            cr.LineTo(414, 215);
            cr.LineTo(404, 215);
            cr.LineTo(404, 195);
            cr.LineTo(394, 195);
            cr.LineTo(394, 185);
            cr.LineTo(384, 185);
            cr.LineTo(384, 175);
            cr.LineTo(374, 175);
            cr.LineTo(374, 165);
            cr.LineTo(364, 165);
            cr.LineTo(364, 155);
            cr.LineTo(354, 155);
            cr.LineTo(354, 135);
            cr.LineTo(344, 135);
            cr.LineTo(344, 115);
            cr.LineTo(334, 115);
            cr.LineTo(334, 85);
            cr.LineTo(324, 85);
            cr.LineTo(324, 65);
            cr.LineTo(314, 65);
            cr.ClosePath();
            cr.MoveTo(274, 85);
            cr.LineTo(294, 85);
            cr.LineTo(294, 75);
            cr.LineTo(304, 75);
            cr.LineTo(304, 85);
            cr.LineTo(294, 85);
            cr.LineTo(294, 95);
            cr.LineTo(304, 95);
            cr.LineTo(304, 115);
            cr.LineTo(314, 115);
            cr.LineTo(314, 135);
            cr.LineTo(324, 135);
            cr.LineTo(324, 155);
            cr.LineTo(334, 155);
            cr.LineTo(334, 165);
            cr.LineTo(344, 165);
            cr.LineTo(344, 185);
            cr.LineTo(354, 185);
            cr.LineTo(354, 195);
            cr.LineTo(364, 195);
            cr.LineTo(364, 205);
            cr.LineTo(374, 205);
            cr.LineTo(374, 215);
            cr.LineTo(384, 215);
            cr.LineTo(384, 225);
            cr.LineTo(394, 225);
            cr.LineTo(394, 235);
            cr.LineTo(384, 235);
            cr.LineTo(384, 245);
            cr.LineTo(364, 245);
            cr.LineTo(364, 255);
            cr.LineTo(344, 255);
            cr.LineTo(344, 265);
            cr.LineTo(334, 265);
            cr.LineTo(334, 275);
            cr.LineTo(314, 275);
            cr.LineTo(314, 285);
            cr.LineTo(294, 285);
            cr.LineTo(294, 295);
            cr.LineTo(284, 295);
            cr.LineTo(284, 305);
            cr.LineTo(264, 305);
            cr.LineTo(264, 315);
            cr.LineTo(244, 315);
            cr.LineTo(244, 325);
            cr.LineTo(224, 325);
            cr.LineTo(224, 335);
            cr.LineTo(204, 335);
            cr.LineTo(204, 345);
            cr.LineTo(184, 345);
            cr.LineTo(184, 355);
            cr.LineTo(164, 355);
            cr.LineTo(164, 365);
            cr.LineTo(144, 365);
            cr.LineTo(144, 375);
            cr.LineTo(124, 375);
            cr.LineTo(124, 365);
            cr.LineTo(114, 365);
            cr.LineTo(114, 325);
            cr.LineTo(134, 325);
            cr.LineTo(134, 315);
            cr.LineTo(144, 315);
            cr.LineTo(144, 305);
            cr.LineTo(154, 305);
            cr.LineTo(154, 295);
            cr.LineTo(174, 295);
            cr.LineTo(174, 285);
            cr.LineTo(184, 285);
            cr.LineTo(184, 275);
            cr.LineTo(204, 275);
            cr.LineTo(204, 265);
            cr.LineTo(214, 265);
            cr.LineTo(214, 255);
            cr.LineTo(224, 255);
            cr.LineTo(224, 245);
            cr.LineTo(244, 245);
            cr.LineTo(244, 235);
            cr.LineTo(254, 235);
            cr.LineTo(254, 225);
            cr.LineTo(264, 225);
            cr.LineTo(264, 215);
            cr.LineTo(284, 215);
            cr.LineTo(284, 205);
            cr.LineTo(304, 205);
            cr.LineTo(304, 195);
            cr.LineTo(284, 195);
            cr.LineTo(284, 185);
            cr.LineTo(294, 185);
            cr.LineTo(294, 175);
            cr.LineTo(274, 175);
            cr.LineTo(274, 185);
            cr.LineTo(244, 185);
            cr.LineTo(244, 195);
            cr.LineTo(204, 195);
            cr.LineTo(204, 205);
            cr.LineTo(164, 205);
            cr.LineTo(164, 215);
            cr.LineTo(134, 215);
            cr.LineTo(134, 225);
            cr.LineTo(94, 225);
            cr.LineTo(94, 235);
            cr.LineTo(64, 235);
            cr.LineTo(64, 225);
            cr.LineTo(54, 225);
            cr.LineTo(54, 215);
            cr.LineTo(44, 215);
            cr.LineTo(44, 195);
            cr.LineTo(54, 195);
            cr.LineTo(54, 205);
            cr.LineTo(64, 205);
            cr.LineTo(64, 185);
            cr.LineTo(74, 185);
            cr.LineTo(74, 175);
            cr.LineTo(84, 175);
            cr.LineTo(84, 195);
            cr.LineTo(94, 195);
            cr.LineTo(94, 165);
            cr.LineTo(104, 165);
            cr.LineTo(104, 185);
            cr.LineTo(114, 185);
            cr.LineTo(114, 165);
            cr.LineTo(124, 165);
            cr.LineTo(124, 155);
            cr.LineTo(134, 155);
            cr.LineTo(134, 175);
            cr.LineTo(144, 175);
            cr.LineTo(144, 145);
            cr.LineTo(154, 145);
            cr.LineTo(154, 165);
            cr.LineTo(164, 165);
            cr.LineTo(164, 145);
            cr.LineTo(174, 145);
            cr.LineTo(174, 135);
            cr.LineTo(184, 135);
            cr.LineTo(184, 155);
            cr.LineTo(194, 155);
            cr.LineTo(194, 125);
            cr.LineTo(204, 125);
            cr.LineTo(204, 145);
            cr.LineTo(214, 145);
            cr.LineTo(214, 115);
            cr.LineTo(234, 115);
            cr.LineTo(234, 125);
            cr.LineTo(244, 125);
            cr.LineTo(244, 105);
            cr.LineTo(254, 105);
            cr.LineTo(254, 95);
            cr.LineTo(264, 95);
            cr.LineTo(264, 115);
            cr.LineTo(274, 115);
            cr.ClosePath();
            cr.MoveTo(274, 85);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(304, 115);
            cr.LineTo(294, 115);
            cr.LineTo(294, 125);
            cr.LineTo(304, 125);
            cr.ClosePath();
            cr.MoveTo(304, 115);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(304, 115);
            cr.LineTo(294, 115);
            cr.LineTo(294, 125);
            cr.LineTo(304, 125);
            cr.ClosePath();
            cr.MoveTo(304, 115);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 135);
            cr.LineTo(304, 135);
            cr.LineTo(304, 145);
            cr.LineTo(314, 145);
            cr.ClosePath();
            cr.MoveTo(314, 135);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 135);
            cr.LineTo(304, 135);
            cr.LineTo(304, 145);
            cr.LineTo(314, 145);
            cr.ClosePath();
            cr.MoveTo(314, 135);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(324, 155);
            cr.LineTo(304, 155);
            cr.LineTo(304, 165);
            cr.LineTo(294, 165);
            cr.LineTo(294, 175);
            cr.LineTo(314, 175);
            cr.LineTo(314, 165);
            cr.LineTo(324, 165);
            cr.ClosePath();
            cr.MoveTo(324, 155);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(334, 165);
            cr.LineTo(324, 165);
            cr.LineTo(324, 175);
            cr.LineTo(334, 175);
            cr.ClosePath();
            cr.MoveTo(334, 165);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(334, 165);
            cr.LineTo(324, 165);
            cr.LineTo(324, 175);
            cr.LineTo(334, 175);
            cr.ClosePath();
            cr.MoveTo(334, 165);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(324, 175);
            cr.LineTo(314, 175);
            cr.LineTo(314, 185);
            cr.LineTo(324, 185);
            cr.ClosePath();
            cr.MoveTo(324, 175);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(324, 175);
            cr.LineTo(314, 175);
            cr.LineTo(314, 185);
            cr.LineTo(324, 185);
            cr.ClosePath();
            cr.MoveTo(324, 175);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 185);
            cr.LineTo(304, 185);
            cr.LineTo(304, 195);
            cr.LineTo(314, 195);
            cr.ClosePath();
            cr.MoveTo(314, 185);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 185);
            cr.LineTo(304, 185);
            cr.LineTo(304, 195);
            cr.LineTo(314, 195);
            cr.ClosePath();
            cr.MoveTo(314, 185);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344, 185);
            cr.LineTo(334, 185);
            cr.LineTo(334, 195);
            cr.LineTo(344, 195);
            cr.ClosePath();
            cr.MoveTo(344, 185);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344, 185);
            cr.LineTo(334, 185);
            cr.LineTo(334, 195);
            cr.LineTo(344, 195);
            cr.ClosePath();
            cr.MoveTo(344, 185);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(354, 195);
            cr.LineTo(344, 195);
            cr.LineTo(344, 205);
            cr.LineTo(354, 205);
            cr.ClosePath();
            cr.MoveTo(354, 195);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(354, 195);
            cr.LineTo(344, 195);
            cr.LineTo(344, 205);
            cr.LineTo(354, 205);
            cr.ClosePath();
            cr.MoveTo(354, 195);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(364, 205);
            cr.LineTo(354, 205);
            cr.LineTo(354, 215);
            cr.LineTo(364, 215);
            cr.ClosePath();
            cr.MoveTo(364, 205);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(364, 205);
            cr.LineTo(354, 205);
            cr.LineTo(354, 215);
            cr.LineTo(364, 215);
            cr.ClosePath();
            cr.MoveTo(364, 205);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(374, 215);
            cr.LineTo(364, 215);
            cr.LineTo(364, 225);
            cr.LineTo(374, 225);
            cr.ClosePath();
            cr.MoveTo(374, 215);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(374, 215);
            cr.LineTo(364, 215);
            cr.LineTo(364, 225);
            cr.LineTo(374, 225);
            cr.ClosePath();
            cr.MoveTo(374, 215);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(384, 225);
            cr.LineTo(374, 225);
            cr.LineTo(374, 235);
            cr.LineTo(384, 235);
            cr.ClosePath();
            cr.MoveTo(384, 225);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(384, 225);
            cr.LineTo(374, 225);
            cr.LineTo(374, 235);
            cr.LineTo(384, 235);
            cr.ClosePath();
            cr.MoveTo(384, 225);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(364, 235);
            cr.LineTo(354, 235);
            cr.LineTo(354, 245);
            cr.LineTo(364, 245);
            cr.ClosePath();
            cr.MoveTo(364, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(364, 235);
            cr.LineTo(354, 235);
            cr.LineTo(354, 245);
            cr.LineTo(364, 245);
            cr.ClosePath();
            cr.MoveTo(364, 235);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344, 245);
            cr.LineTo(334, 245);
            cr.LineTo(334, 255);
            cr.LineTo(344, 255);
            cr.ClosePath();
            cr.MoveTo(344, 245);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(344, 245);
            cr.LineTo(334, 245);
            cr.LineTo(334, 255);
            cr.LineTo(344, 255);
            cr.ClosePath();
            cr.MoveTo(344, 245);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(334, 255);
            cr.LineTo(324, 255);
            cr.LineTo(324, 265);
            cr.LineTo(334, 265);
            cr.ClosePath();
            cr.MoveTo(334, 255);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(334, 255);
            cr.LineTo(324, 255);
            cr.LineTo(324, 265);
            cr.LineTo(334, 265);
            cr.ClosePath();
            cr.MoveTo(334, 255);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 265);
            cr.LineTo(304, 265);
            cr.LineTo(304, 275);
            cr.LineTo(314, 275);
            cr.ClosePath();
            cr.MoveTo(314, 265);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(314, 265);
            cr.LineTo(304, 265);
            cr.LineTo(304, 275);
            cr.LineTo(314, 275);
            cr.ClosePath();
            cr.MoveTo(314, 265);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(294, 275);
            cr.LineTo(284, 275);
            cr.LineTo(284, 285);
            cr.LineTo(294, 285);
            cr.ClosePath();
            cr.MoveTo(294, 275);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(294, 275);
            cr.LineTo(284, 275);
            cr.LineTo(284, 285);
            cr.LineTo(294, 285);
            cr.ClosePath();
            cr.MoveTo(294, 275);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(284, 285);
            cr.LineTo(274, 285);
            cr.LineTo(274, 295);
            cr.LineTo(284, 295);
            cr.ClosePath();
            cr.MoveTo(284, 285);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(284, 285);
            cr.LineTo(274, 285);
            cr.LineTo(274, 295);
            cr.LineTo(284, 295);
            cr.ClosePath();
            cr.MoveTo(284, 285);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(264, 295);
            cr.LineTo(254, 295);
            cr.LineTo(254, 305);
            cr.LineTo(264, 305);
            cr.ClosePath();
            cr.MoveTo(264, 295);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(264, 295);
            cr.LineTo(254, 295);
            cr.LineTo(254, 305);
            cr.LineTo(264, 305);
            cr.ClosePath();
            cr.MoveTo(264, 295);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(244, 305);
            cr.LineTo(234, 305);
            cr.LineTo(234, 315);
            cr.LineTo(244, 315);
            cr.ClosePath();
            cr.MoveTo(244, 305);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(244, 305);
            cr.LineTo(234, 305);
            cr.LineTo(234, 315);
            cr.LineTo(244, 315);
            cr.ClosePath();
            cr.MoveTo(244, 305);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(224, 315);
            cr.LineTo(214, 315);
            cr.LineTo(214, 325);
            cr.LineTo(224, 325);
            cr.ClosePath();
            cr.MoveTo(224, 315);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(224, 315);
            cr.LineTo(214, 315);
            cr.LineTo(214, 325);
            cr.LineTo(224, 325);
            cr.ClosePath();
            cr.MoveTo(224, 315);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(204, 325);
            cr.LineTo(194, 325);
            cr.LineTo(194, 335);
            cr.LineTo(204, 335);
            cr.ClosePath();
            cr.MoveTo(204, 325);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(204, 325);
            cr.LineTo(194, 325);
            cr.LineTo(194, 335);
            cr.LineTo(204, 335);
            cr.ClosePath();
            cr.MoveTo(204, 325);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(184, 335);
            cr.LineTo(174, 335);
            cr.LineTo(174, 345);
            cr.LineTo(184, 345);
            cr.ClosePath();
            cr.MoveTo(184, 335);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(184, 335);
            cr.LineTo(174, 335);
            cr.LineTo(174, 345);
            cr.LineTo(184, 345);
            cr.ClosePath();
            cr.MoveTo(184, 335);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(164, 345);
            cr.LineTo(154, 345);
            cr.LineTo(154, 355);
            cr.LineTo(164, 355);
            cr.ClosePath();
            cr.MoveTo(164, 345);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(164, 345);
            cr.LineTo(154, 345);
            cr.LineTo(154, 355);
            cr.LineTo(164, 355);
            cr.ClosePath();
            cr.MoveTo(164, 345);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(144, 355);
            cr.LineTo(134, 355);
            cr.LineTo(134, 365);
            cr.LineTo(144, 365);
            cr.ClosePath();
            cr.MoveTo(144, 355);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.03;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(144, 355);
            cr.LineTo(134, 355);
            cr.LineTo(134, 365);
            cr.LineTo(144, 365);
            cr.ClosePath();
            cr.MoveTo(144, 355);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -81, -171);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void DrawArrowRight(Context cr, double lineWidth = 5, bool strokeOrFill = true, bool defaultPattern = true)
        {
            Pattern pattern;

            cr.Operator = Operator.Over;
            cr.LineWidth = lineWidth;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            if (defaultPattern)
            {
                pattern = new SolidPattern(0, 0, 0, 1);
                cr.SetSource(pattern);
                pattern.Dispose();
            }
            cr.NewPath();
            cr.MoveTo(93.054688, 8.605469);
            cr.CurveTo(104.347656, 18.074219, 115.644531, 27.542969, 126.757813, 37.011719);
            cr.LineTo(93.054688, 63.167969);
            cr.LineTo(93.054688, 49.527344);
            cr.LineTo(7.988281, 49.527344);
            cr.LineTo(7.988281, 22.246094);
            cr.LineTo(93.054688, 22.246094);
            cr.ClosePath();
            cr.MoveTo(93.054688, 8.605469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            if (strokeOrFill) cr.Stroke(); else cr.Fill();
            
        }


        public void DrawFlame(Context cr, double lineWidth = 3, bool strokeOrFill = true, bool defaultPattern = true)
        {
            Pattern pattern;

            cr.Operator = Operator.Over;
            cr.LineWidth = lineWidth;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            if (defaultPattern)
            {
                pattern = new SolidPattern(0, 0, 0, 1);
                cr.SetSource(pattern);
                pattern.Dispose();
            }
            cr.NewPath();
            cr.MoveTo(18.6875, 205.261719);
            cr.LineTo(43.011719, 205.261719);
            cr.CurveTo(39.6875, 178.445313, 16.839844, 155.210938, 27.570313, 126.023438);
            cr.CurveTo(38.300781, 96.832031, 48.164063, 74.972656, 43.652344, 59.785156);
            cr.CurveTo(39.144531, 44.597656, 26.6875, 27.261719, 26.6875, 27.261719);
            cr.CurveTo(26.6875, 27.261719, 21.585938, 20.191406, 20.398438, 24.226563);
            cr.CurveTo(19.210938, 28.261719, 32.789063, 47.546875, 35.164063, 64.160156);
            cr.CurveTo(37.535156, 80.769531, 11.859375, 112.835938, 6.398438, 132.769531);
            cr.CurveTo(0.941406, 152.703125, 18.6875, 205.261719, 18.6875, 205.261719);
            cr.ClosePath();
            cr.MoveTo(18.6875, 205.261719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            if (strokeOrFill) cr.Stroke(); else cr.Fill();
            /********************/
            cr.Operator = Operator.Over;
            cr.LineWidth = 3;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            if (defaultPattern)
            {
                pattern = new SolidPattern(0, 0, 0, 1);
                cr.SetSource(pattern);
                pattern.Dispose();
            }
            cr.NewPath();
            cr.MoveTo(66, 205.445313);
            cr.CurveTo(74, 205.402344, 82, 205.570313, 90, 205.527344);
            cr.CurveTo(83.332031, 183.859375, 73.125, 170.457031, 74.984375, 148.804688);
            cr.CurveTo(76.839844, 127.152344, 95.308594, 98.640625, 91.238281, 77.261719);
            cr.CurveTo(87.167969, 55.886719, 77.367188, 43.011719, 77.289063, 28.75);
            cr.CurveTo(77.214844, 14.488281, 78.308594, 6.550781, 78.054688, 5.042969);
            cr.CurveTo(77.800781, 3.535156, 76.371094, 2.691406, 75, 3.753906);
            cr.CurveTo(73.628906, 4.816406, 69.21875, 15.316406, 68.535156, 26.070313);
            cr.CurveTo(67.847656, 36.824219, 75.628906, 58.503906, 79.109375, 78.628906);
            cr.CurveTo(82.589844, 98.753906, 62.640625, 116.386719, 58, 138.523438);
            cr.CurveTo(53.359375, 160.664063, 55.453125, 186.828125, 66, 205.445313);
            cr.ClosePath();
            cr.MoveTo(66, 205.445313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            if (strokeOrFill) cr.Stroke(); else cr.Fill();
            /********************/
            cr.Operator = Operator.Over;
            cr.LineWidth = 3;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            if (defaultPattern)
            {
                pattern = new SolidPattern(0, 0, 0, 1);
                cr.SetSource(pattern);
                pattern.Dispose();
            }
            cr.NewPath(); ;
            cr.MoveTo(114.75, 205.621094);
            cr.LineTo(135.761719, 205.320313);
            cr.CurveTo(133.589844, 193.070313, 122.96875, 182.257813, 127.90625, 166.570313);
            cr.CurveTo(132.84375, 150.878906, 147.074219, 135.644531, 140.1875, 117.496094);
            cr.CurveTo(133.296875, 99.351563, 126.015625, 96.335938, 124.855469, 82.945313);
            cr.CurveTo(123.695313, 69.558594, 131.972656, 52.335938, 134.324219, 47.585938);
            cr.CurveTo(136.671875, 42.835938, 138.050781, 30.433594, 129.757813, 42.273438);
            cr.CurveTo(121.460938, 54.117188, 113.96875, 65.066406, 113.148438, 81);
            cr.CurveTo(112.328125, 96.9375, 119.726563, 100.0625, 122.570313, 119.335938);
            cr.CurveTo(125.414063, 138.613281, 110.496094, 140.839844, 105.148438, 165.1875);
            cr.CurveTo(104.425781, 180.949219, 110.3125, 192.886719, 114.75, 205.621094);
            cr.ClosePath();
            cr.MoveTo(114.75, 205.621094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            if (strokeOrFill) cr.Stroke(); else cr.Fill();
            /********************/
        }



        /// <summary>
        /// Draws 5 vertical bars of increasing size and Quality amount of them green
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="quality"></param>
        /// <param name="size"></param>
        public void DrawConnectionQuality(Context ctx, double x, double y, int quality, double size)
        {
            double padding = 2;
            double barWidth = (size - 4 * padding) / 5.0;

            for (int i = 0; i < 5; i++)
            {
                double xpos = x + i * (padding + barWidth);
                double barHeight = (i + 1) / 5.0 * size;

                ctx.NewPath();
                ctx.MoveTo(xpos, y + size);
                ctx.LineTo(xpos + barWidth, y + size);
                ctx.LineTo(xpos + barWidth, y + size - barHeight);
                ctx.LineTo(xpos, y + size - barHeight);
                ctx.ClosePath();

                ctx.SetSourceRGBA(0, 0, 0, 0.7);
                ctx.StrokePreserve();

                if (i < quality)
                {
                    ctx.SetSourceRGBA(0.2, 0.8, 0.2, 0.5);
                } else
                {
                    ctx.SetSourceRGBA(0, 0, 0, 0.5);
                }

                ctx.Fill();
            }

            if (quality == 0)
            {
                ctx.SetSourceRGBA(0.8, 0.2, 0.2, 0.7);
                DrawCross(ctx, x, y, 2, size, true);
                ctx.SetSourceRGBA(0.8, 0.2, 0.2, 0.5);
                ctx.Fill();
            }
        }


        public void Drawmenuicon_svg(Context cr, double x, double y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 158;
            float h = 144;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.LineWidth = 20;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 14);
            cr.LineTo(158, 14);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.LineWidth = 20;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 72);
            cr.LineTo(158, 72);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.LineWidth = 20;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 130);
            cr.LineTo(158, 130);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void DrawMapMarker(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 91;
            float h = 150;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 23;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(12.945313, 47.4375);
            cr.CurveTo(-0.539063, 1.199219, 92.136719, -2.097656, 77.855469, 47.4375);
            cr.CurveTo(65.773438, 89.179688, 45.398438, 125.632813, 45.398438, 125.632813);
            cr.CurveTo(45.398438, 125.632813, 22.53125, 80.394531, 12.945313, 47.4375);
            cr.ClosePath();
            cr.MoveTo(12.945313, 47.4375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.998668, 0, 0, 0.998668, 0.160453, 0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        
        public void DrawMapPlayer(Context cr, int x, int y, float width, float height, double[] strokeRgba, double[] fillRgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 26;
            float h = 39;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(fillRgba[0], fillRgba[1], fillRgba[2], fillRgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.75, 14.835938);
            cr.CurveTo(19.75, 7.820313, 16.726563, 2.132813, 13, 2.132813);
            cr.CurveTo(9.273438, 2.132813, 6.25, 7.820313, 6.25, 14.835938);
            cr.CurveTo(6.25, 21.851563, 9.273438, 27.539063, 13, 27.539063);
            cr.CurveTo(16.726563, 27.539063, 19.75, 21.851563, 19.75, 14.835938);
            cr.ClosePath();
            cr.MoveTo(19.75, 14.835938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 4;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(strokeRgba[0], strokeRgba[1], strokeRgba[2], strokeRgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.75, 14.835938);
            cr.CurveTo(19.75, 7.820313, 16.726563, 2.132813, 13, 2.132813);
            cr.CurveTo(9.273438, 2.132813, 6.25, 7.820313, 6.25, 14.835938);
            cr.CurveTo(6.25, 21.851563, 9.273438, 27.539063, 13, 27.539063);
            cr.CurveTo(16.726563, 27.539063, 19.75, 21.851563, 19.75, 14.835938);
            cr.ClosePath();
            cr.MoveTo(19.75, 14.835938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.992366, 0, 0, 0.992366, 0, 0.148855);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(fillRgba[0], fillRgba[1], fillRgba[2], fillRgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(24.015625, 25.851563);
            cr.CurveTo(24.015625, 31.933594, 19.082031, 36.867188, 13, 36.867188);
            cr.CurveTo(6.917969, 36.867188, 1.984375, 31.933594, 1.984375, 25.851563);
            cr.CurveTo(1.984375, 19.769531, 6.917969, 14.835938, 13, 14.835938);
            cr.CurveTo(19.082031, 14.835938, 24.015625, 19.769531, 24.015625, 25.851563);
            cr.ClosePath();
            cr.MoveTo(24.015625, 25.851563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 4;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(strokeRgba[0], strokeRgba[1], strokeRgba[2], strokeRgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(24.015625, 25.851563);
            cr.CurveTo(24.015625, 31.933594, 19.082031, 36.867188, 13, 36.867188);
            cr.CurveTo(6.917969, 36.867188, 1.984375, 31.933594, 1.984375, 25.851563);
            cr.CurveTo(1.984375, 19.769531, 6.917969, 14.835938, 13, 14.835938);
            cr.CurveTo(19.082031, 14.835938, 24.015625, 19.769531, 24.015625, 25.851563);
            cr.ClosePath();
            cr.MoveTo(24.015625, 25.851563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.992366, 0, 0, 0.992366, 0, 0.148855);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(fillRgba[0], fillRgba[1], fillRgba[2], fillRgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(11.558594, 34.703125);
            cr.CurveTo(9.667969, 34.386719, 7.675781, 33.269531, 6.339844, 31.769531);
            cr.CurveTo(2.125, 27.050781, 4, 19.707031, 10, 17.4375);
            cr.CurveTo(11.214844, 16.980469, 13.53125, 16.816406, 14.878906, 17.097656);
            cr.CurveTo(17.867188, 17.71875, 20.46875, 20.09375, 21.511719, 23.164063);
            cr.CurveTo(21.960938, 24.472656, 22.015625, 26.71875, 21.636719, 28.167969);
            cr.CurveTo(20.519531, 32.460938, 15.929688, 35.433594, 11.558594, 34.703125);
            cr.ClosePath();
            cr.MoveTo(11.558594, 34.703125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(fillRgba[0], fillRgba[1], fillRgba[2], fillRgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(17.296875, 13.511719);
            cr.CurveTo(17.160156, 13.433594, 16.519531, 13.253906, 15.871094, 13.105469);
            cr.CurveTo(13.867188, 12.65625, 11.019531, 12.789063, 8.785156, 13.433594);
            cr.LineTo(8.386719, 13.550781);
            cr.LineTo(8.492188, 12.335938);
            cr.CurveTo(8.722656, 9.628906, 9.820313, 6.734375, 11.15625, 5.308594);
            cr.CurveTo(12.140625, 4.25, 12.890625, 3.988281, 13.753906, 4.398438);
            cr.CurveTo(15.496094, 5.226563, 17.199219, 8.871094, 17.5625, 12.554688);
            cr.CurveTo(17.621094, 13.164063, 17.640625, 13.660156, 17.605469, 13.65625);
            cr.CurveTo(17.570313, 13.652344, 17.433594, 13.589844, 17.296875, 13.511719);
            cr.ClosePath();
            cr.MoveTo(17.296875, 13.511719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void DrawPlus(Context ctx, int x, int y, float width, float height, double[] strokeRgba, double lineWidth)
        {
            ctx.LineWidth = lineWidth;

            ctx.SetSourceRGBA(strokeRgba);

            ctx.NewPath();
            ctx.MoveTo(x, y + height / 2);
            ctx.LineTo(x + width, y + height / 2);

            ctx.MoveTo(x + width / 2, y);
            ctx.LineTo(x + width / 2, y + height);

            ctx.ClosePath();

            ctx.Stroke();
        }


        public void DrawCross(Context ctx, double x, double y, double lineWidth, double size, bool preserverePath = false)
        {
            ctx.LineWidth = lineWidth;

            ctx.NewPath();
            ctx.MoveTo(x + lineWidth, y + lineWidth);
            ctx.LineTo(x + lineWidth + size, y + lineWidth + size);

            ctx.MoveTo(x + lineWidth + size, y + lineWidth);
            ctx.LineTo(x + lineWidth, y + lineWidth + size);

            ctx.ClosePath();

            if (preserverePath)
            {
                ctx.StrokePreserve();
            } else
            {
                ctx.Stroke();
            }        
        }


        public void DrawPen(Context ctx, double x, double y, double lineWidth, double size)
        {
            ctx.LineWidth = lineWidth;

            ctx.NewPath();
            ctx.MoveTo(x + lineWidth + size, y + lineWidth);
            ctx.LineTo(x + lineWidth + 0.3 * size, y + lineWidth + size * 0.7);
            ctx.ClosePath();
            ctx.Stroke();


            double length = lineWidth / (2 * Math.Sqrt(2));
            ctx.NewPath();
            ctx.MoveTo(x + lineWidth + 0.2 * size - length, y + lineWidth + size * 0.8 - length);
            ctx.LineTo(x + lineWidth, y + lineWidth + size);
            ctx.LineTo(x + lineWidth + 0.2 * size + length, y + lineWidth + size * 0.8 + length);
            ctx.ClosePath();
            ctx.Fill();
        }


        public void DrawRandomSymbol(Context ctx, double x, double y, double size, double[] color, double lineWidth, int seed, int addLines = 0)
        {
            int blurRadius = 7;
            double innerSize = size - 2 * blurRadius;
            Random rnd = new Random(seed);
            int quantity = rnd.Next(2, 5) + addLines;
            double x1, y1, x2, y2;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)size+1, (int)size + 1);
            Context blurCtx = new Context(surface);
            blurCtx.SetSourceRGBA(0, 0, 0, 0);
            blurCtx.Paint();
            blurCtx.LineWidth = lineWidth;
            blurCtx.SetSourceRGBA(color);
            
            while (quantity-- > 0)
            {
                x1 = rnd.NextDouble() * innerSize / 2;
                y1 = blurRadius + rnd.NextDouble() * innerSize;
                x2 = rnd.NextDouble() * innerSize / 2;
                y2 = blurRadius + rnd.NextDouble() * innerSize;

                blurCtx.NewPath();
                blurCtx.MoveTo(blurRadius + x1, y1);
                blurCtx.LineTo(blurRadius + x2, y2);
                blurCtx.ClosePath();
                blurCtx.Stroke();

                blurCtx.NewPath();
                blurCtx.MoveTo(size - blurRadius - x1, y1);
                blurCtx.LineTo(size - blurRadius - x2, y2);
                blurCtx.ClosePath();
                blurCtx.Stroke();
            }

            ctx.Operator = Operator.Over;
            ctx.SetSourceSurface(surface, (int)x, (int)y);
            ctx.Rectangle(x, y, (int)size + 1, (int)size + 1);
            ctx.Fill();

            surface.BlurFull(blurRadius);

            ctx.Operator = Operator.DestOver;
            ctx.Rectangle(x, y, (int)size + 1, (int)size + 1);
            ctx.Fill();

            blurCtx.Dispose();
            surface.Dispose();

            ctx.Operator = Operator.Over;   
        }


        public void DrawVSGear(Context cr, ImageSurface surface, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 84;
            float h = 84;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(46.996094, 0.703125);
            cr.LineTo(44.824219, 6.175781);
            cr.LineTo(39.605469, 6.175781);
            cr.LineTo(37.4375, 0.703125);
            cr.LineTo(32.53125, 1.570313);
            cr.LineTo(32.523438, 1.570313);
            cr.LineTo(32.355469, 7.453125);
            cr.LineTo(27.457031, 9.238281);
            cr.LineTo(23.625, 4.792969);
            cr.LineTo(23.621094, 4.796875);
            cr.LineTo(23.617188, 4.792969);
            cr.LineTo(19.144531, 7.378906);
            cr.LineTo(21.078125, 12.921875);
            cr.LineTo(17.082031, 16.273438);
            cr.LineTo(12.027344, 13.332031);
            cr.LineTo(12.019531, 13.332031);
            cr.LineTo(8.574219, 17.441406);
            cr.LineTo(8.574219, 17.445313);
            cr.LineTo(12.347656, 21.914063);
            cr.LineTo(9.738281, 26.433594);
            cr.LineTo(3.925781, 25.546875);
            cr.LineTo(2.199219, 30.289063);
            cr.LineTo(2.199219, 30.292969);
            cr.LineTo(7.222656, 33.351563);
            cr.LineTo(6.316406, 38.488281);
            cr.LineTo(0.550781, 39.6875);
            cr.LineTo(0.550781, 44.65625);
            cr.LineTo(6.316406, 45.855469);
            cr.LineTo(7.222656, 50.992188);
            cr.LineTo(2.308594, 53.988281);
            cr.LineTo(2.207031, 54.042969);
            cr.LineTo(2.214844, 54.0625);
            cr.LineTo(2.203125, 54.070313);
            cr.LineTo(3.921875, 58.792969);
            cr.LineTo(3.921875, 58.796875);
            cr.LineTo(9.742188, 57.910156);
            cr.LineTo(12.347656, 62.429688);
            cr.LineTo(8.574219, 66.898438);
            cr.LineTo(8.574219, 66.902344);
            cr.LineTo(12.023438, 71.011719);
            cr.LineTo(12.027344, 71.011719);
            cr.LineTo(17.082031, 68.070313);
            cr.LineTo(21.078125, 71.421875);
            cr.LineTo(19.144531, 76.964844);
            cr.LineTo(23.621094, 79.546875);
            cr.LineTo(23.625, 79.546875);
            cr.LineTo(27.457031, 75.105469);
            cr.LineTo(32.359375, 76.886719);
            cr.LineTo(32.515625, 82.667969);
            cr.LineTo(32.511719, 82.773438);
            cr.LineTo(32.53125, 82.777344);
            cr.LineTo(32.53125, 82.789063);
            cr.LineTo(37.433594, 83.652344);
            cr.LineTo(37.4375, 83.652344);
            cr.LineTo(39.609375, 78.167969);
            cr.LineTo(44.824219, 78.167969);
            cr.LineTo(46.996094, 83.636719);
            cr.LineTo(51.902344, 82.773438);
            cr.LineTo(51.910156, 82.773438);
            cr.LineTo(51.910156, 82.769531);
            cr.LineTo(52.078125, 76.890625);
            cr.LineTo(56.976563, 75.105469);
            cr.LineTo(60.808594, 79.550781);
            cr.LineTo(60.8125, 79.546875);
            cr.LineTo(60.816406, 79.550781);
            cr.LineTo(65.289063, 76.964844);
            cr.LineTo(63.355469, 71.425781);
            cr.LineTo(67.351563, 68.070313);
            cr.LineTo(72.300781, 70.980469);
            cr.LineTo(75.886719, 66.902344);
            cr.LineTo(72.085938, 62.429688);
            cr.LineTo(74.695313, 57.914063);
            cr.LineTo(80.507813, 58.796875);
            cr.LineTo(82.234375, 54.054688);
            cr.LineTo(82.230469, 54.050781);
            cr.LineTo(77.210938, 50.992188);
            cr.LineTo(78.117188, 45.855469);
            cr.LineTo(83.882813, 44.65625);
            cr.LineTo(83.882813, 39.691406);
            cr.LineTo(78.117188, 38.492188);
            cr.LineTo(77.210938, 33.355469);
            cr.LineTo(82.132813, 30.351563);
            cr.LineTo(82.226563, 30.300781);
            cr.LineTo(82.21875, 30.285156);
            cr.LineTo(82.230469, 30.277344);
            cr.LineTo(82.226563, 30.273438);
            cr.LineTo(82.230469, 30.273438);
            cr.LineTo(80.511719, 25.550781);
            cr.LineTo(74.691406, 26.433594);
            cr.LineTo(72.085938, 21.917969);
            cr.LineTo(75.859375, 17.449219);
            cr.LineTo(75.859375, 17.445313);
            cr.LineTo(72.414063, 13.335938);
            cr.LineTo(72.410156, 13.339844);
            cr.LineTo(72.410156, 13.335938);
            cr.LineTo(72.40625, 13.335938);
            cr.LineTo(67.351563, 16.277344);
            cr.LineTo(63.355469, 12.925781);
            cr.LineTo(65.289063, 7.382813);
            cr.LineTo(65.285156, 7.382813);
            cr.LineTo(65.285156, 7.378906);
            cr.LineTo(60.8125, 4.796875);
            cr.LineTo(60.808594, 4.796875);
            cr.LineTo(56.976563, 9.242188);
            cr.LineTo(52.078125, 7.457031);
            cr.LineTo(51.90625, 1.574219);
            cr.ClosePath();
            cr.MoveTo(45.148438, 17.554688);
            cr.CurveTo(45.304688, 17.570313, 45.457031, 17.59375, 45.609375, 17.613281);
            cr.CurveTo(45.636719, 17.617188, 45.667969, 17.621094, 45.695313, 17.625);
            cr.LineTo(45.703125, 17.625);
            cr.CurveTo(45.820313, 17.640625, 45.9375, 17.660156, 46.054688, 17.679688);
            cr.CurveTo(46.136719, 17.691406, 46.222656, 17.707031, 46.304688, 17.71875);
            cr.CurveTo(46.414063, 17.738281, 46.523438, 17.757813, 46.632813, 17.777344);
            cr.CurveTo(46.714844, 17.792969, 46.792969, 17.808594, 46.875, 17.820313);
            cr.CurveTo(46.988281, 17.84375, 47.101563, 17.867188, 47.210938, 17.890625);
            cr.CurveTo(47.292969, 17.90625, 47.371094, 17.921875, 47.449219, 17.941406);
            cr.CurveTo(47.566406, 17.964844, 47.679688, 17.988281, 47.792969, 18.015625);
            cr.CurveTo(47.867188, 18.03125, 47.9375, 18.050781, 48.007813, 18.066406);
            cr.CurveTo(48.128906, 18.097656, 48.25, 18.125, 48.367188, 18.15625);
            cr.CurveTo(48.425781, 18.171875, 48.484375, 18.1875, 48.542969, 18.203125);
            cr.CurveTo(48.851563, 18.285156, 49.160156, 18.371094, 49.464844, 18.464844);
            cr.CurveTo(49.664063, 18.523438, 49.863281, 18.589844, 50.0625, 18.65625);
            cr.CurveTo(50.257813, 18.722656, 50.457031, 18.789063, 50.652344, 18.859375);
            cr.CurveTo(50.652344, 18.859375, 50.652344, 18.859375, 50.652344, 18.859375);
            cr.CurveTo(50.847656, 18.933594, 51.042969, 19.003906, 51.238281, 19.082031);
            cr.CurveTo(51.238281, 19.082031, 51.238281, 19.082031, 51.238281, 19.082031);
            cr.CurveTo(51.628906, 19.234375, 52.011719, 19.394531, 52.390625, 19.566406);
            cr.CurveTo(52.769531, 19.734375, 53.144531, 19.917969, 53.515625, 20.105469);
            cr.CurveTo(53.699219, 20.199219, 53.882813, 20.296875, 54.066406, 20.398438);
            cr.CurveTo(54.433594, 20.597656, 54.792969, 20.804688, 55.148438, 21.019531);
            cr.CurveTo(55.503906, 21.238281, 55.851563, 21.464844, 56.195313, 21.699219);
            cr.CurveTo(56.441406, 21.867188, 56.679688, 22.039063, 56.917969, 22.214844);
            cr.CurveTo(56.949219, 22.234375, 56.980469, 22.257813, 57.011719, 22.28125);
            cr.CurveTo(57.242188, 22.453125, 57.472656, 22.628906, 57.703125, 22.8125);
            cr.CurveTo(57.863281, 22.941406, 58.027344, 23.074219, 58.183594, 23.210938);
            cr.LineTo(58.1875, 23.210938);
            cr.CurveTo(58.34375, 23.34375, 58.503906, 23.480469, 58.660156, 23.617188);
            cr.CurveTo(58.816406, 23.757813, 58.96875, 23.898438, 59.125, 24.039063);
            cr.CurveTo(59.308594, 24.214844, 59.492188, 24.390625, 59.675781, 24.570313);
            cr.CurveTo(59.691406, 24.585938, 59.707031, 24.601563, 59.722656, 24.617188);
            cr.LineTo(59.726563, 24.621094);
            cr.CurveTo(59.878906, 24.773438, 60.03125, 24.929688, 60.183594, 25.089844);
            cr.CurveTo(60.21875, 25.128906, 60.257813, 25.167969, 60.292969, 25.207031);
            cr.CurveTo(60.292969, 25.207031, 60.292969, 25.207031, 60.296875, 25.207031);
            cr.CurveTo(60.4375, 25.359375, 60.578125, 25.511719, 60.714844, 25.667969);
            cr.CurveTo(60.742188, 25.699219, 60.769531, 25.726563, 60.792969, 25.757813);
            cr.CurveTo(60.960938, 25.945313, 61.125, 26.136719, 61.285156, 26.328125);
            cr.CurveTo(61.421875, 26.492188, 61.554688, 26.65625, 61.683594, 26.820313);
            cr.CurveTo(61.683594, 26.820313, 61.6875, 26.820313, 61.6875, 26.820313);
            cr.CurveTo(61.816406, 26.988281, 61.945313, 27.152344, 62.070313, 27.324219);
            cr.LineTo(52.035156, 33.117188);
            cr.LineTo(45.148438, 29.140625);
            cr.ClosePath();
            cr.MoveTo(39.285156, 17.554688);
            cr.LineTo(39.285156, 29.140625);
            cr.LineTo(32.394531, 33.117188);
            cr.LineTo(22.363281, 27.324219);
            cr.CurveTo(22.394531, 27.28125, 22.429688, 27.238281, 22.460938, 27.195313);
            cr.CurveTo(22.546875, 27.082031, 22.632813, 26.96875, 22.71875, 26.859375);
            cr.CurveTo(22.777344, 26.785156, 22.835938, 26.710938, 22.898438, 26.636719);
            cr.CurveTo(22.960938, 26.558594, 23.023438, 26.480469, 23.085938, 26.402344);
            cr.CurveTo(23.148438, 26.328125, 23.214844, 26.25, 23.28125, 26.171875);
            cr.CurveTo(23.347656, 26.09375, 23.410156, 26.019531, 23.476563, 25.941406);
            cr.CurveTo(23.539063, 25.871094, 23.597656, 25.800781, 23.660156, 25.734375);
            cr.CurveTo(23.734375, 25.652344, 23.804688, 25.570313, 23.878906, 25.488281);
            cr.CurveTo(23.941406, 25.421875, 24.003906, 25.355469, 24.0625, 25.289063);
            cr.CurveTo(24.132813, 25.214844, 24.203125, 25.140625, 24.277344, 25.066406);
            cr.CurveTo(24.34375, 24.992188, 24.410156, 24.921875, 24.480469, 24.851563);
            cr.CurveTo(24.542969, 24.789063, 24.605469, 24.722656, 24.671875, 24.660156);
            cr.CurveTo(24.75, 24.582031, 24.824219, 24.503906, 24.90625, 24.425781);
            cr.CurveTo(24.933594, 24.398438, 24.964844, 24.367188, 24.996094, 24.339844);
            cr.CurveTo(25.097656, 24.238281, 25.203125, 24.140625, 25.308594, 24.042969);
            cr.CurveTo(25.460938, 23.898438, 25.617188, 23.757813, 25.773438, 23.621094);
            cr.CurveTo(25.929688, 23.480469, 26.085938, 23.34375, 26.246094, 23.210938);
            cr.CurveTo(26.246094, 23.210938, 26.246094, 23.210938, 26.246094, 23.210938);
            cr.CurveTo(26.40625, 23.074219, 26.566406, 22.945313, 26.730469, 22.8125);
            cr.CurveTo(26.730469, 22.8125, 26.730469, 22.8125, 26.730469, 22.8125);
            cr.CurveTo(27.058594, 22.554688, 27.390625, 22.300781, 27.726563, 22.058594);
            cr.CurveTo(28.066406, 21.8125, 28.40625, 21.578125, 28.757813, 21.355469);
            cr.CurveTo(28.933594, 21.242188, 29.109375, 21.128906, 29.285156, 21.023438);
            cr.CurveTo(29.640625, 20.804688, 30, 20.597656, 30.367188, 20.398438);
            cr.CurveTo(30.730469, 20.199219, 31.101563, 20.011719, 31.476563, 19.828125);
            cr.CurveTo(31.746094, 19.699219, 32.023438, 19.574219, 32.296875, 19.453125);
            cr.CurveTo(32.316406, 19.445313, 32.335938, 19.4375, 32.355469, 19.425781);
            cr.CurveTo(32.632813, 19.308594, 32.914063, 19.191406, 33.195313, 19.082031);
            cr.CurveTo(33.390625, 19.007813, 33.582031, 18.933594, 33.78125, 18.859375);
            cr.CurveTo(33.976563, 18.789063, 34.171875, 18.722656, 34.371094, 18.65625);
            cr.CurveTo(34.570313, 18.589844, 34.769531, 18.527344, 34.96875, 18.464844);
            cr.CurveTo(35.46875, 18.3125, 35.976563, 18.175781, 36.484375, 18.050781);
            cr.CurveTo(36.996094, 17.929688, 37.511719, 17.824219, 38.03125, 17.734375);
            cr.CurveTo(38.242188, 17.699219, 38.449219, 17.667969, 38.65625, 17.636719);
            cr.CurveTo(38.65625, 17.636719, 38.660156, 17.636719, 38.660156, 17.636719);
            cr.CurveTo(38.867188, 17.605469, 39.074219, 17.578125, 39.285156, 17.554688);
            cr.ClosePath();
            cr.MoveTo(65, 32.402344);
            cr.CurveTo(65.21875, 32.910156, 65.421875, 33.425781, 65.601563, 33.949219);
            cr.CurveTo(65.605469, 33.949219, 65.605469, 33.949219, 65.605469, 33.953125);
            cr.CurveTo(65.910156, 34.820313, 66.164063, 35.703125, 66.371094, 36.59375);
            cr.CurveTo(66.417969, 36.796875, 66.460938, 37, 66.503906, 37.207031);
            cr.CurveTo(66.503906, 37.207031, 66.503906, 37.207031, 66.503906, 37.207031);
            cr.CurveTo(66.542969, 37.410156, 66.585938, 37.617188, 66.621094, 37.820313);
            cr.CurveTo(66.65625, 38.027344, 66.691406, 38.234375, 66.722656, 38.4375);
            cr.CurveTo(66.785156, 38.851563, 66.835938, 39.265625, 66.878906, 39.679688);
            cr.CurveTo(66.921875, 40.09375, 66.953125, 40.507813, 66.972656, 40.921875);
            cr.CurveTo(66.984375, 41.128906, 66.992188, 41.339844, 66.996094, 41.546875);
            cr.CurveTo(67.007813, 41.960938, 67.007813, 42.378906, 66.996094, 42.792969);
            cr.CurveTo(66.988281, 43.210938, 66.964844, 43.625, 66.933594, 44.039063);
            cr.CurveTo(66.886719, 44.664063, 66.816406, 45.28125, 66.722656, 45.902344);
            cr.CurveTo(66.691406, 46.109375, 66.65625, 46.3125, 66.621094, 46.519531);
            cr.CurveTo(66.585938, 46.722656, 66.546875, 46.929688, 66.503906, 47.132813);
            cr.CurveTo(66.460938, 47.339844, 66.417969, 47.542969, 66.371094, 47.746094);
            cr.CurveTo(66.136719, 48.765625, 65.835938, 49.773438, 65.46875, 50.761719);
            cr.LineTo(65.46875, 50.765625);
            cr.CurveTo(65.398438, 50.960938, 65.320313, 51.160156, 65.242188, 51.355469);
            cr.CurveTo(65.164063, 51.550781, 65.082031, 51.746094, 65, 51.941406);
            cr.LineTo(56.769531, 47.1875);
            cr.LineTo(54.964844, 46.148438);
            cr.LineTo(54.964844, 38.195313);
            cr.LineTo(62.800781, 33.671875);
            cr.ClosePath();
            cr.MoveTo(19.433594, 32.402344);
            cr.LineTo(29.464844, 38.195313);
            cr.LineTo(29.464844, 46.148438);
            cr.LineTo(21.632813, 50.667969);
            cr.LineTo(19.433594, 51.941406);
            cr.CurveTo(18.847656, 50.578125, 18.390625, 49.171875, 18.0625, 47.746094);
            cr.CurveTo(18.015625, 47.542969, 17.972656, 47.339844, 17.929688, 47.136719);
            cr.CurveTo(17.886719, 46.929688, 17.847656, 46.726563, 17.8125, 46.519531);
            cr.CurveTo(17.777344, 46.316406, 17.742188, 46.109375, 17.710938, 45.902344);
            cr.CurveTo(17.648438, 45.492188, 17.597656, 45.078125, 17.554688, 44.664063);
            cr.CurveTo(17.511719, 44.25, 17.480469, 43.832031, 17.460938, 43.417969);
            cr.CurveTo(17.449219, 43.210938, 17.441406, 43.003906, 17.4375, 42.796875);
            cr.LineTo(17.4375, 42.792969);
            cr.CurveTo(17.425781, 42.378906, 17.425781, 41.964844, 17.4375, 41.546875);
            cr.CurveTo(17.445313, 41.132813, 17.46875, 40.714844, 17.5, 40.300781);
            cr.CurveTo(17.546875, 39.679688, 17.617188, 39.058594, 17.710938, 38.441406);
            cr.CurveTo(17.742188, 38.234375, 17.777344, 38.027344, 17.8125, 37.824219);
            cr.CurveTo(17.847656, 37.617188, 17.886719, 37.414063, 17.929688, 37.207031);
            cr.CurveTo(17.972656, 37.003906, 18.015625, 36.796875, 18.0625, 36.59375);
            cr.CurveTo(18.296875, 35.574219, 18.597656, 34.566406, 18.964844, 33.578125);
            cr.CurveTo(19.035156, 33.378906, 19.113281, 33.183594, 19.191406, 32.984375);
            cr.CurveTo(19.269531, 32.789063, 19.351563, 32.59375, 19.433594, 32.402344);
            cr.ClosePath();
            cr.MoveTo(42.1875, 35.085938);
            cr.CurveTo(42.207031, 35.085938, 42.222656, 35.085938, 42.242188, 35.085938);
            cr.CurveTo(42.246094, 35.085938, 42.246094, 35.085938, 42.25, 35.085938);
            cr.CurveTo(42.339844, 35.089844, 42.429688, 35.089844, 42.519531, 35.09375);
            cr.CurveTo(42.519531, 35.09375, 42.523438, 35.09375, 42.523438, 35.09375);
            cr.CurveTo(42.558594, 35.097656, 42.589844, 35.097656, 42.621094, 35.097656);
            cr.CurveTo(42.625, 35.097656, 42.625, 35.097656, 42.625, 35.097656);
            cr.CurveTo(42.691406, 35.101563, 42.757813, 35.109375, 42.824219, 35.113281);
            cr.CurveTo(42.902344, 35.121094, 42.980469, 35.128906, 43.058594, 35.136719);
            cr.CurveTo(43.082031, 35.140625, 43.105469, 35.144531, 43.128906, 35.148438);
            cr.CurveTo(43.171875, 35.152344, 43.210938, 35.160156, 43.25, 35.164063);
            cr.CurveTo(43.320313, 35.175781, 43.390625, 35.1875, 43.460938, 35.199219);
            cr.CurveTo(43.519531, 35.207031, 43.574219, 35.21875, 43.632813, 35.230469);
            cr.CurveTo(43.714844, 35.246094, 43.796875, 35.265625, 43.875, 35.285156);
            cr.CurveTo(43.921875, 35.296875, 43.96875, 35.308594, 44.015625, 35.320313);
            cr.CurveTo(44.015625, 35.320313, 44.015625, 35.320313, 44.019531, 35.320313);
            cr.CurveTo(44.058594, 35.332031, 44.101563, 35.34375, 44.140625, 35.355469);
            cr.CurveTo(44.253906, 35.386719, 44.363281, 35.421875, 44.476563, 35.457031);
            cr.CurveTo(44.515625, 35.472656, 44.558594, 35.484375, 44.597656, 35.5);
            cr.CurveTo(44.71875, 35.542969, 44.835938, 35.589844, 44.953125, 35.640625);
            cr.CurveTo(44.984375, 35.652344, 45.015625, 35.664063, 45.046875, 35.675781);
            cr.CurveTo(45.050781, 35.679688, 45.058594, 35.679688, 45.0625, 35.683594);
            cr.CurveTo(45.109375, 35.703125, 45.152344, 35.726563, 45.195313, 35.746094);
            cr.CurveTo(45.199219, 35.746094, 45.199219, 35.746094, 45.199219, 35.746094);
            cr.CurveTo(45.265625, 35.777344, 45.335938, 35.808594, 45.402344, 35.84375);
            cr.CurveTo(45.402344, 35.84375, 45.402344, 35.84375, 45.402344, 35.84375);
            cr.CurveTo(45.425781, 35.855469, 45.449219, 35.867188, 45.472656, 35.878906);
            cr.CurveTo(45.476563, 35.882813, 45.484375, 35.886719, 45.492188, 35.890625);
            cr.CurveTo(45.492188, 35.890625, 45.492188, 35.890625, 45.496094, 35.890625);
            cr.CurveTo(45.570313, 35.933594, 45.648438, 35.972656, 45.726563, 36.015625);
            cr.CurveTo(45.726563, 36.019531, 45.730469, 36.019531, 45.730469, 36.019531);
            cr.CurveTo(45.730469, 36.019531, 45.730469, 36.019531, 45.734375, 36.019531);
            cr.CurveTo(45.734375, 36.023438, 45.738281, 36.023438, 45.738281, 36.023438);
            cr.CurveTo(45.753906, 36.035156, 45.769531, 36.042969, 45.78125, 36.050781);
            cr.CurveTo(45.78125, 36.050781, 45.78125, 36.050781, 45.78125, 36.050781);
            cr.CurveTo(45.785156, 36.050781, 45.785156, 36.050781, 45.785156, 36.050781);
            cr.CurveTo(45.785156, 36.050781, 45.785156, 36.054688, 45.789063, 36.054688);
            cr.CurveTo(45.839844, 36.082031, 45.886719, 36.113281, 45.9375, 36.144531);
            cr.CurveTo(45.964844, 36.160156, 45.992188, 36.175781, 46.019531, 36.195313);
            cr.CurveTo(46.046875, 36.210938, 46.074219, 36.230469, 46.101563, 36.246094);
            cr.CurveTo(46.101563, 36.25, 46.105469, 36.25, 46.105469, 36.25);
            cr.CurveTo(46.167969, 36.292969, 46.226563, 36.332031, 46.289063, 36.375);
            cr.CurveTo(46.289063, 36.375, 46.289063, 36.375, 46.289063, 36.375);
            cr.CurveTo(46.289063, 36.375, 46.289063, 36.375, 46.289063, 36.375);
            cr.CurveTo(46.292969, 36.378906, 46.296875, 36.378906, 46.296875, 36.378906);
            cr.CurveTo(46.335938, 36.40625, 46.371094, 36.433594, 46.410156, 36.460938);
            cr.CurveTo(46.410156, 36.460938, 46.410156, 36.460938, 46.410156, 36.460938);
            cr.CurveTo(46.410156, 36.460938, 46.410156, 36.464844, 46.410156, 36.464844);
            cr.CurveTo(46.414063, 36.464844, 46.417969, 36.46875, 46.421875, 36.472656);
            cr.CurveTo(46.457031, 36.496094, 46.492188, 36.523438, 46.523438, 36.550781);
            cr.CurveTo(46.613281, 36.617188, 46.703125, 36.691406, 46.789063, 36.761719);
            cr.CurveTo(46.832031, 36.796875, 46.871094, 36.832031, 46.914063, 36.867188);
            cr.CurveTo(46.980469, 36.929688, 47.046875, 36.992188, 47.113281, 37.054688);
            cr.CurveTo(47.164063, 37.105469, 47.21875, 37.15625, 47.269531, 37.207031);
            cr.CurveTo(47.285156, 37.222656, 47.300781, 37.238281, 47.316406, 37.253906);
            cr.CurveTo(47.316406, 37.253906, 47.316406, 37.257813, 47.316406, 37.257813);
            cr.CurveTo(47.394531, 37.335938, 47.46875, 37.417969, 47.542969, 37.5);
            cr.CurveTo(47.566406, 37.527344, 47.585938, 37.550781, 47.609375, 37.578125);
            cr.CurveTo(47.679688, 37.664063, 47.75, 37.75, 47.820313, 37.835938);
            cr.CurveTo(47.824219, 37.84375, 47.832031, 37.855469, 47.839844, 37.867188);
            cr.CurveTo(47.851563, 37.878906, 47.863281, 37.894531, 47.871094, 37.90625);
            cr.CurveTo(47.929688, 37.984375, 47.988281, 38.0625, 48.042969, 38.140625);
            cr.CurveTo(48.070313, 38.183594, 48.101563, 38.226563, 48.128906, 38.269531);
            cr.CurveTo(48.132813, 38.273438, 48.132813, 38.277344, 48.136719, 38.28125);
            cr.CurveTo(48.136719, 38.28125, 48.136719, 38.28125, 48.136719, 38.28125);
            cr.CurveTo(48.160156, 38.3125, 48.179688, 38.347656, 48.199219, 38.378906);
            cr.CurveTo(48.246094, 38.453125, 48.289063, 38.523438, 48.335938, 38.601563);
            cr.CurveTo(48.335938, 38.601563, 48.335938, 38.605469, 48.335938, 38.605469);
            cr.CurveTo(48.34375, 38.617188, 48.347656, 38.628906, 48.355469, 38.636719);
            cr.CurveTo(48.359375, 38.644531, 48.363281, 38.648438, 48.363281, 38.652344);
            cr.CurveTo(48.363281, 38.652344, 48.367188, 38.65625, 48.367188, 38.65625);
            cr.CurveTo(48.378906, 38.679688, 48.390625, 38.699219, 48.402344, 38.722656);
            cr.CurveTo(48.433594, 38.78125, 48.464844, 38.835938, 48.496094, 38.894531);
            cr.CurveTo(48.496094, 38.894531, 48.5, 38.898438, 48.5, 38.898438);
            cr.CurveTo(48.511719, 38.925781, 48.523438, 38.949219, 48.535156, 38.972656);
            cr.CurveTo(48.539063, 38.976563, 48.542969, 38.980469, 48.542969, 38.988281);
            cr.CurveTo(48.542969, 38.988281, 48.542969, 38.988281, 48.546875, 38.988281);
            cr.CurveTo(48.5625, 39.019531, 48.574219, 39.050781, 48.589844, 39.082031);
            cr.CurveTo(48.597656, 39.101563, 48.609375, 39.121094, 48.617188, 39.140625);
            cr.CurveTo(48.660156, 39.226563, 48.699219, 39.316406, 48.738281, 39.402344);
            cr.CurveTo(48.746094, 39.421875, 48.753906, 39.445313, 48.761719, 39.464844);
            cr.CurveTo(48.769531, 39.476563, 48.773438, 39.492188, 48.78125, 39.507813);
            cr.CurveTo(48.820313, 39.605469, 48.855469, 39.703125, 48.890625, 39.800781);
            cr.CurveTo(48.902344, 39.832031, 48.914063, 39.863281, 48.925781, 39.894531);
            cr.CurveTo(48.957031, 40, 48.992188, 40.101563, 49.019531, 40.207031);
            cr.CurveTo(49.027344, 40.230469, 49.035156, 40.253906, 49.042969, 40.28125);
            cr.CurveTo(49.0625, 40.359375, 49.082031, 40.433594, 49.101563, 40.511719);
            cr.CurveTo(49.117188, 40.589844, 49.136719, 40.664063, 49.152344, 40.742188);
            cr.CurveTo(49.164063, 40.804688, 49.179688, 40.863281, 49.1875, 40.925781);
            cr.CurveTo(49.207031, 41.027344, 49.222656, 41.132813, 49.238281, 41.234375);
            cr.CurveTo(49.242188, 41.28125, 49.25, 41.328125, 49.253906, 41.375);
            cr.CurveTo(49.253906, 41.382813, 49.257813, 41.386719, 49.257813, 41.390625);
            cr.CurveTo(49.257813, 41.390625, 49.257813, 41.390625, 49.257813, 41.390625);
            cr.CurveTo(49.261719, 41.433594, 49.265625, 41.476563, 49.269531, 41.519531);
            cr.CurveTo(49.277344, 41.597656, 49.285156, 41.679688, 49.289063, 41.761719);
            cr.CurveTo(49.289063, 41.761719, 49.289063, 41.765625, 49.289063, 41.765625);
            cr.CurveTo(49.289063, 41.800781, 49.292969, 41.832031, 49.292969, 41.867188);
            cr.CurveTo(49.296875, 41.953125, 49.300781, 42.042969, 49.300781, 42.132813);
            cr.CurveTo(49.300781, 42.136719, 49.300781, 42.136719, 49.300781, 42.140625);
            cr.CurveTo(49.300781, 42.160156, 49.300781, 42.179688, 49.300781, 42.199219);
            cr.CurveTo(49.300781, 42.203125, 49.300781, 42.203125, 49.300781, 42.207031);
            cr.CurveTo(49.300781, 42.296875, 49.296875, 42.386719, 49.292969, 42.472656);
            cr.CurveTo(49.289063, 42.507813, 49.289063, 42.539063, 49.289063, 42.574219);
            cr.CurveTo(49.289063, 42.574219, 49.289063, 42.578125, 49.289063, 42.578125);
            cr.CurveTo(49.285156, 42.652344, 49.277344, 42.726563, 49.269531, 42.796875);
            cr.CurveTo(49.269531, 42.800781, 49.269531, 42.800781, 49.269531, 42.800781);
            cr.CurveTo(49.265625, 42.851563, 49.261719, 42.898438, 49.257813, 42.949219);
            cr.CurveTo(49.257813, 42.949219, 49.257813, 42.949219, 49.257813, 42.949219);
            cr.CurveTo(49.257813, 42.953125, 49.253906, 42.960938, 49.253906, 42.964844);
            cr.CurveTo(49.25, 43.011719, 49.242188, 43.058594, 49.238281, 43.105469);
            cr.CurveTo(49.226563, 43.175781, 49.21875, 43.25, 49.207031, 43.320313);
            cr.CurveTo(49.203125, 43.34375, 49.199219, 43.367188, 49.195313, 43.386719);
            cr.CurveTo(49.175781, 43.484375, 49.160156, 43.578125, 49.140625, 43.671875);
            cr.CurveTo(49.109375, 43.808594, 49.074219, 43.941406, 49.039063, 44.074219);
            cr.CurveTo(49.035156, 44.082031, 49.035156, 44.089844, 49.03125, 44.097656);
            cr.CurveTo(48.996094, 44.226563, 48.957031, 44.351563, 48.914063, 44.476563);
            cr.CurveTo(48.914063, 44.476563, 48.914063, 44.480469, 48.914063, 44.480469);
            cr.CurveTo(48.90625, 44.492188, 48.902344, 44.507813, 48.898438, 44.519531);
            cr.CurveTo(48.859375, 44.632813, 48.816406, 44.742188, 48.769531, 44.855469);
            cr.CurveTo(48.761719, 44.882813, 48.75, 44.910156, 48.738281, 44.9375);
            cr.CurveTo(48.699219, 45.027344, 48.660156, 45.113281, 48.617188, 45.199219);
            cr.CurveTo(48.59375, 45.25, 48.574219, 45.296875, 48.550781, 45.34375);
            cr.CurveTo(48.53125, 45.382813, 48.511719, 45.417969, 48.492188, 45.457031);
            cr.CurveTo(48.449219, 45.535156, 48.40625, 45.613281, 48.363281, 45.6875);
            cr.CurveTo(48.355469, 45.703125, 48.347656, 45.71875, 48.335938, 45.734375);
            cr.CurveTo(48.335938, 45.738281, 48.335938, 45.738281, 48.335938, 45.742188);
            cr.CurveTo(48.289063, 45.820313, 48.242188, 45.894531, 48.195313, 45.972656);
            cr.CurveTo(48.191406, 45.972656, 48.191406, 45.972656, 48.191406, 45.976563);
            cr.CurveTo(48.171875, 46.003906, 48.15625, 46.03125, 48.136719, 46.058594);
            cr.CurveTo(48.136719, 46.058594, 48.136719, 46.058594, 48.136719, 46.0625);
            cr.CurveTo(48.097656, 46.117188, 48.0625, 46.171875, 48.023438, 46.222656);
            cr.CurveTo(47.988281, 46.273438, 47.957031, 46.324219, 47.921875, 46.371094);
            cr.CurveTo(47.917969, 46.375, 47.917969, 46.375, 47.914063, 46.378906);
            cr.CurveTo(47.890625, 46.410156, 47.867188, 46.441406, 47.839844, 46.476563);
            cr.CurveTo(47.792969, 46.539063, 47.742188, 46.601563, 47.691406, 46.664063);
            cr.CurveTo(47.679688, 46.679688, 47.667969, 46.695313, 47.65625, 46.707031);
            cr.CurveTo(47.597656, 46.777344, 47.539063, 46.84375, 47.480469, 46.910156);
            cr.CurveTo(47.472656, 46.917969, 47.464844, 46.925781, 47.457031, 46.9375);
            cr.CurveTo(47.453125, 46.941406, 47.449219, 46.945313, 47.445313, 46.953125);
            cr.CurveTo(47.441406, 46.953125, 47.4375, 46.957031, 47.4375, 46.960938);
            cr.CurveTo(47.375, 47.027344, 47.3125, 47.089844, 47.25, 47.15625);
            cr.CurveTo(47.242188, 47.164063, 47.234375, 47.171875, 47.226563, 47.179688);
            cr.LineTo(47.222656, 47.183594);
            cr.CurveTo(47.222656, 47.183594, 47.21875, 47.1875, 47.214844, 47.1875);
            cr.CurveTo(47.113281, 47.289063, 47.007813, 47.386719, 46.898438, 47.484375);
            cr.CurveTo(46.867188, 47.511719, 46.835938, 47.539063, 46.800781, 47.566406);
            cr.CurveTo(46.703125, 47.652344, 46.605469, 47.730469, 46.503906, 47.808594);
            cr.CurveTo(46.472656, 47.832031, 46.441406, 47.855469, 46.410156, 47.878906);
            cr.CurveTo(46.371094, 47.910156, 46.332031, 47.9375, 46.289063, 47.964844);
            cr.CurveTo(46.230469, 48.007813, 46.167969, 48.050781, 46.105469, 48.09375);
            cr.CurveTo(46.074219, 48.113281, 46.042969, 48.132813, 46.015625, 48.148438);
            cr.CurveTo(45.9375, 48.199219, 45.863281, 48.246094, 45.785156, 48.289063);
            cr.CurveTo(45.785156, 48.289063, 45.785156, 48.289063, 45.785156, 48.292969);
            cr.CurveTo(45.78125, 48.292969, 45.777344, 48.292969, 45.777344, 48.296875);
            cr.CurveTo(45.761719, 48.304688, 45.75, 48.3125, 45.734375, 48.320313);
            cr.CurveTo(45.730469, 48.320313, 45.730469, 48.320313, 45.726563, 48.324219);
            cr.CurveTo(45.644531, 48.371094, 45.5625, 48.414063, 45.480469, 48.457031);
            cr.CurveTo(45.457031, 48.46875, 45.433594, 48.484375, 45.410156, 48.496094);
            cr.CurveTo(45.410156, 48.496094, 45.40625, 48.496094, 45.40625, 48.496094);
            cr.CurveTo(45.328125, 48.535156, 45.253906, 48.570313, 45.175781, 48.605469);
            cr.CurveTo(45.140625, 48.625, 45.101563, 48.640625, 45.066406, 48.65625);
            cr.CurveTo(45.007813, 48.683594, 44.953125, 48.707031, 44.894531, 48.730469);
            cr.CurveTo(44.839844, 48.75, 44.785156, 48.773438, 44.730469, 48.796875);
            cr.CurveTo(44.726563, 48.796875, 44.726563, 48.796875, 44.722656, 48.796875);
            cr.CurveTo(44.6875, 48.8125, 44.652344, 48.824219, 44.617188, 48.835938);
            cr.CurveTo(44.546875, 48.859375, 44.476563, 48.886719, 44.410156, 48.90625);
            cr.CurveTo(44.382813, 48.917969, 44.355469, 48.925781, 44.328125, 48.933594);
            cr.CurveTo(44.238281, 48.960938, 44.148438, 48.988281, 44.054688, 49.011719);
            cr.CurveTo(43.960938, 49.039063, 43.863281, 49.0625, 43.769531, 49.082031);
            cr.CurveTo(43.738281, 49.089844, 43.707031, 49.097656, 43.675781, 49.105469);
            cr.CurveTo(43.601563, 49.121094, 43.527344, 49.132813, 43.453125, 49.148438);
            cr.CurveTo(43.449219, 49.148438, 43.445313, 49.148438, 43.4375, 49.148438);
            cr.CurveTo(43.429688, 49.152344, 43.421875, 49.152344, 43.410156, 49.15625);
            cr.CurveTo(43.375, 49.160156, 43.339844, 49.167969, 43.300781, 49.171875);
            cr.CurveTo(43.261719, 49.179688, 43.21875, 49.183594, 43.179688, 49.191406);
            cr.CurveTo(43.117188, 49.199219, 43.058594, 49.207031, 42.996094, 49.210938);
            cr.CurveTo(42.976563, 49.214844, 42.960938, 49.214844, 42.941406, 49.21875);
            cr.CurveTo(42.910156, 49.222656, 42.878906, 49.222656, 42.851563, 49.226563);
            cr.CurveTo(42.777344, 49.234375, 42.699219, 49.238281, 42.625, 49.246094);
            cr.CurveTo(42.625, 49.246094, 42.621094, 49.246094, 42.617188, 49.246094);
            cr.CurveTo(42.585938, 49.246094, 42.558594, 49.246094, 42.527344, 49.25);
            cr.CurveTo(42.433594, 49.253906, 42.34375, 49.253906, 42.25, 49.253906);
            cr.CurveTo(42.230469, 49.257813, 42.207031, 49.257813, 42.183594, 49.253906);
            cr.CurveTo(42.09375, 49.253906, 42, 49.253906, 41.910156, 49.25);
            cr.CurveTo(41.875, 49.246094, 41.839844, 49.246094, 41.804688, 49.246094);
            cr.CurveTo(41.730469, 49.238281, 41.65625, 49.234375, 41.582031, 49.226563);
            cr.CurveTo(41.535156, 49.222656, 41.484375, 49.21875, 41.433594, 49.214844);
            cr.CurveTo(41.382813, 49.207031, 41.332031, 49.199219, 41.277344, 49.191406);
            cr.CurveTo(41.207031, 49.183594, 41.136719, 49.175781, 41.066406, 49.164063);
            cr.CurveTo(41.042969, 49.160156, 41.019531, 49.152344, 40.996094, 49.148438);
            cr.CurveTo(40.964844, 49.144531, 40.9375, 49.136719, 40.90625, 49.132813);
            cr.CurveTo(40.820313, 49.117188, 40.734375, 49.097656, 40.648438, 49.082031);
            cr.CurveTo(40.566406, 49.0625, 40.484375, 49.039063, 40.398438, 49.019531);
            cr.CurveTo(40.335938, 49, 40.273438, 48.984375, 40.207031, 48.964844);
            cr.CurveTo(40.207031, 48.964844, 40.203125, 48.960938, 40.199219, 48.960938);
            cr.CurveTo(40.195313, 48.960938, 40.191406, 48.960938, 40.1875, 48.957031);
            cr.CurveTo(40.121094, 48.9375, 40.054688, 48.917969, 39.988281, 48.894531);
            cr.CurveTo(39.921875, 48.875, 39.855469, 48.851563, 39.789063, 48.828125);
            cr.LineTo(39.785156, 48.824219);
            cr.CurveTo(39.734375, 48.804688, 39.679688, 48.785156, 39.628906, 48.765625);
            cr.CurveTo(39.597656, 48.753906, 39.5625, 48.738281, 39.527344, 48.726563);
            cr.CurveTo(39.480469, 48.707031, 39.433594, 48.6875, 39.386719, 48.664063);
            cr.CurveTo(39.339844, 48.644531, 39.292969, 48.625, 39.242188, 48.601563);
            cr.CurveTo(39.171875, 48.566406, 39.101563, 48.535156, 39.027344, 48.5);
            cr.CurveTo(39.027344, 48.496094, 39.023438, 48.496094, 39.023438, 48.496094);
            cr.CurveTo(39, 48.484375, 38.976563, 48.472656, 38.953125, 48.460938);
            cr.CurveTo(38.871094, 48.417969, 38.789063, 48.371094, 38.707031, 48.324219);
            cr.CurveTo(38.703125, 48.324219, 38.703125, 48.320313, 38.699219, 48.320313);
            cr.CurveTo(38.683594, 48.3125, 38.671875, 48.304688, 38.65625, 48.296875);
            cr.CurveTo(38.65625, 48.296875, 38.652344, 48.292969, 38.652344, 48.292969);
            cr.CurveTo(38.648438, 48.292969, 38.648438, 48.292969, 38.648438, 48.292969);
            cr.CurveTo(38.570313, 48.246094, 38.496094, 48.199219, 38.417969, 48.152344);
            cr.CurveTo(38.390625, 48.132813, 38.359375, 48.113281, 38.332031, 48.09375);
            cr.CurveTo(38.265625, 48.050781, 38.203125, 48.007813, 38.144531, 47.964844);
            cr.CurveTo(38.101563, 47.9375, 38.0625, 47.910156, 38.023438, 47.878906);
            cr.CurveTo(37.992188, 47.859375, 37.960938, 47.832031, 37.929688, 47.808594);
            cr.CurveTo(37.828125, 47.730469, 37.730469, 47.652344, 37.632813, 47.570313);
            cr.CurveTo(37.597656, 47.542969, 37.566406, 47.511719, 37.535156, 47.484375);
            cr.CurveTo(37.421875, 47.386719, 37.316406, 47.289063, 37.210938, 47.183594);
            cr.LineTo(37.207031, 47.183594);
            cr.CurveTo(37.199219, 47.175781, 37.191406, 47.164063, 37.183594, 47.15625);
            cr.CurveTo(37.121094, 47.09375, 37.058594, 47.027344, 36.996094, 46.960938);
            cr.CurveTo(36.996094, 46.957031, 36.992188, 46.957031, 36.992188, 46.953125);
            cr.CurveTo(36.984375, 46.945313, 36.976563, 46.9375, 36.96875, 46.929688);
            cr.CurveTo(36.964844, 46.925781, 36.964844, 46.925781, 36.960938, 46.921875);
            cr.CurveTo(36.898438, 46.855469, 36.839844, 46.78125, 36.777344, 46.710938);
            cr.CurveTo(36.765625, 46.695313, 36.753906, 46.679688, 36.742188, 46.667969);
            cr.CurveTo(36.691406, 46.605469, 36.640625, 46.539063, 36.59375, 46.476563);
            cr.CurveTo(36.566406, 46.445313, 36.542969, 46.414063, 36.519531, 46.378906);
            cr.CurveTo(36.515625, 46.378906, 36.515625, 46.375, 36.511719, 46.371094);
            cr.CurveTo(36.476563, 46.324219, 36.445313, 46.273438, 36.410156, 46.226563);
            cr.CurveTo(36.371094, 46.171875, 36.332031, 46.117188, 36.296875, 46.0625);
            cr.CurveTo(36.296875, 46.0625, 36.296875, 46.0625, 36.296875, 46.058594);
            cr.CurveTo(36.277344, 46.03125, 36.261719, 46.003906, 36.242188, 45.976563);
            cr.CurveTo(36.242188, 45.976563, 36.242188, 45.972656, 36.238281, 45.972656);
            cr.CurveTo(36.191406, 45.894531, 36.144531, 45.820313, 36.097656, 45.742188);
            cr.CurveTo(36.097656, 45.738281, 36.097656, 45.738281, 36.097656, 45.738281);
            cr.CurveTo(36.089844, 45.726563, 36.082031, 45.714844, 36.078125, 45.703125);
            cr.CurveTo(36.074219, 45.699219, 36.070313, 45.695313, 36.070313, 45.691406);
            cr.CurveTo(36.027344, 45.613281, 35.984375, 45.535156, 35.941406, 45.457031);
            cr.CurveTo(35.921875, 45.421875, 35.902344, 45.382813, 35.882813, 45.34375);
            cr.CurveTo(35.859375, 45.296875, 35.839844, 45.25, 35.816406, 45.203125);
            cr.CurveTo(35.773438, 45.113281, 35.734375, 45.027344, 35.695313, 44.9375);
            cr.CurveTo(35.683594, 44.910156, 35.671875, 44.882813, 35.664063, 44.855469);
            cr.CurveTo(35.617188, 44.746094, 35.574219, 44.632813, 35.535156, 44.519531);
            cr.CurveTo(35.53125, 44.507813, 35.527344, 44.496094, 35.519531, 44.480469);
            cr.CurveTo(35.519531, 44.480469, 35.519531, 44.480469, 35.519531, 44.480469);
            cr.CurveTo(35.476563, 44.351563, 35.4375, 44.226563, 35.402344, 44.101563);
            cr.CurveTo(35.398438, 44.09375, 35.398438, 44.085938, 35.394531, 44.078125);
            cr.CurveTo(35.359375, 43.945313, 35.324219, 43.808594, 35.292969, 43.675781);
            cr.CurveTo(35.273438, 43.582031, 35.257813, 43.484375, 35.238281, 43.390625);
            cr.CurveTo(35.234375, 43.367188, 35.230469, 43.34375, 35.226563, 43.320313);
            cr.CurveTo(35.214844, 43.25, 35.207031, 43.179688, 35.195313, 43.105469);
            cr.CurveTo(35.191406, 43.058594, 35.183594, 43.011719, 35.179688, 42.964844);
            cr.CurveTo(35.179688, 42.960938, 35.175781, 42.957031, 35.175781, 42.949219);
            cr.CurveTo(35.175781, 42.949219, 35.175781, 42.949219, 35.175781, 42.949219);
            cr.CurveTo(35.171875, 42.90625, 35.167969, 42.863281, 35.164063, 42.824219);
            cr.CurveTo(35.15625, 42.742188, 35.148438, 42.660156, 35.144531, 42.582031);
            cr.CurveTo(35.144531, 42.578125, 35.144531, 42.578125, 35.144531, 42.574219);
            cr.CurveTo(35.144531, 42.542969, 35.140625, 42.507813, 35.140625, 42.476563);
            cr.CurveTo(35.136719, 42.386719, 35.132813, 42.296875, 35.132813, 42.207031);
            cr.CurveTo(35.132813, 42.207031, 35.132813, 42.203125, 35.132813, 42.199219);
            cr.CurveTo(35.132813, 42.179688, 35.132813, 42.160156, 35.132813, 42.140625);
            cr.CurveTo(35.132813, 42.140625, 35.132813, 42.136719, 35.132813, 42.136719);
            cr.CurveTo(35.132813, 42.046875, 35.136719, 41.957031, 35.140625, 41.867188);
            cr.CurveTo(35.144531, 41.835938, 35.144531, 41.800781, 35.144531, 41.769531);
            cr.CurveTo(35.144531, 41.765625, 35.144531, 41.765625, 35.144531, 41.761719);
            cr.CurveTo(35.148438, 41.6875, 35.15625, 41.617188, 35.164063, 41.542969);
            cr.CurveTo(35.164063, 41.542969, 35.164063, 41.542969, 35.164063, 41.542969);
            cr.CurveTo(35.167969, 41.492188, 35.171875, 41.441406, 35.175781, 41.394531);
            cr.CurveTo(35.175781, 41.394531, 35.175781, 41.394531, 35.175781, 41.390625);
            cr.CurveTo(35.175781, 41.386719, 35.179688, 41.382813, 35.179688, 41.378906);
            cr.CurveTo(35.183594, 41.328125, 35.191406, 41.28125, 35.195313, 41.234375);
            cr.CurveTo(35.199219, 41.210938, 35.203125, 41.1875, 35.207031, 41.164063);
            cr.CurveTo(35.21875, 41.09375, 35.226563, 41.023438, 35.238281, 40.953125);
            cr.CurveTo(35.238281, 40.953125, 35.238281, 40.953125, 35.238281, 40.953125);
            cr.CurveTo(35.253906, 40.875, 35.269531, 40.796875, 35.285156, 40.71875);
            cr.CurveTo(35.296875, 40.667969, 35.308594, 40.617188, 35.320313, 40.566406);
            cr.CurveTo(35.34375, 40.464844, 35.367188, 40.363281, 35.394531, 40.265625);
            cr.CurveTo(35.398438, 40.257813, 35.398438, 40.25, 35.402344, 40.242188);
            cr.CurveTo(35.4375, 40.117188, 35.476563, 39.988281, 35.519531, 39.863281);
            cr.CurveTo(35.519531, 39.863281, 35.519531, 39.863281, 35.519531, 39.863281);
            cr.CurveTo(35.527344, 39.847656, 35.53125, 39.835938, 35.535156, 39.824219);
            cr.CurveTo(35.574219, 39.710938, 35.617188, 39.597656, 35.664063, 39.488281);
            cr.CurveTo(35.671875, 39.460938, 35.683594, 39.433594, 35.695313, 39.40625);
            cr.CurveTo(35.734375, 39.316406, 35.773438, 39.226563, 35.816406, 39.140625);
            cr.CurveTo(35.839844, 39.09375, 35.859375, 39.046875, 35.882813, 39);
            cr.CurveTo(35.902344, 38.960938, 35.921875, 38.921875, 35.941406, 38.886719);
            cr.CurveTo(35.984375, 38.808594, 36.027344, 38.730469, 36.070313, 38.652344);
            cr.CurveTo(36.078125, 38.636719, 36.085938, 38.621094, 36.097656, 38.605469);
            cr.CurveTo(36.097656, 38.605469, 36.097656, 38.601563, 36.097656, 38.601563);
            cr.CurveTo(36.144531, 38.523438, 36.191406, 38.445313, 36.238281, 38.371094);
            cr.CurveTo(36.242188, 38.367188, 36.242188, 38.367188, 36.242188, 38.363281);
            cr.CurveTo(36.261719, 38.335938, 36.277344, 38.308594, 36.296875, 38.28125);
            cr.CurveTo(36.296875, 38.28125, 36.296875, 38.28125, 36.296875, 38.28125);
            cr.CurveTo(36.335938, 38.226563, 36.371094, 38.171875, 36.410156, 38.117188);
            cr.CurveTo(36.414063, 38.109375, 36.421875, 38.101563, 36.429688, 38.089844);
            cr.CurveTo(36.464844, 38.039063, 36.503906, 37.984375, 36.542969, 37.929688);
            cr.CurveTo(36.558594, 37.910156, 36.574219, 37.886719, 36.59375, 37.867188);
            cr.CurveTo(36.621094, 37.828125, 36.652344, 37.792969, 36.679688, 37.753906);
            cr.CurveTo(36.71875, 37.707031, 36.753906, 37.664063, 36.789063, 37.621094);
            cr.CurveTo(36.832031, 37.566406, 36.878906, 37.515625, 36.921875, 37.464844);
            cr.CurveTo(36.972656, 37.410156, 37.023438, 37.355469, 37.078125, 37.296875);
            cr.CurveTo(37.113281, 37.261719, 37.148438, 37.226563, 37.183594, 37.1875);
            cr.CurveTo(37.183594, 37.1875, 37.183594, 37.1875, 37.183594, 37.1875);
            cr.CurveTo(37.214844, 37.15625, 37.246094, 37.125, 37.277344, 37.097656);
            cr.CurveTo(37.363281, 37.015625, 37.445313, 36.933594, 37.535156, 36.859375);
            cr.CurveTo(37.566406, 36.828125, 37.597656, 36.800781, 37.632813, 36.773438);
            cr.CurveTo(37.730469, 36.691406, 37.828125, 36.609375, 37.929688, 36.535156);
            cr.CurveTo(37.960938, 36.507813, 37.992188, 36.484375, 38.023438, 36.460938);
            cr.CurveTo(38.0625, 36.433594, 38.101563, 36.40625, 38.144531, 36.378906);
            cr.CurveTo(38.203125, 36.335938, 38.265625, 36.292969, 38.328125, 36.25);
            cr.CurveTo(38.328125, 36.25, 38.328125, 36.25, 38.332031, 36.25);
            cr.CurveTo(38.351563, 36.234375, 38.375, 36.21875, 38.398438, 36.207031);
            cr.CurveTo(38.402344, 36.203125, 38.410156, 36.199219, 38.414063, 36.195313);
            cr.CurveTo(38.417969, 36.195313, 38.417969, 36.191406, 38.417969, 36.191406);
            cr.CurveTo(38.476563, 36.15625, 38.535156, 36.121094, 38.589844, 36.085938);
            cr.CurveTo(38.609375, 36.078125, 38.625, 36.066406, 38.640625, 36.054688);
            cr.CurveTo(38.644531, 36.054688, 38.644531, 36.054688, 38.644531, 36.054688);
            cr.CurveTo(38.644531, 36.054688, 38.648438, 36.050781, 38.648438, 36.050781);
            cr.CurveTo(38.648438, 36.050781, 38.648438, 36.050781, 38.652344, 36.050781);
            cr.CurveTo(38.652344, 36.050781, 38.65625, 36.046875, 38.65625, 36.046875);
            cr.CurveTo(38.671875, 36.039063, 38.683594, 36.03125, 38.699219, 36.023438);
            cr.CurveTo(38.703125, 36.019531, 38.703125, 36.019531, 38.707031, 36.019531);
            cr.CurveTo(38.726563, 36.007813, 38.746094, 35.996094, 38.765625, 35.984375);
            cr.CurveTo(38.824219, 35.953125, 38.882813, 35.921875, 38.941406, 35.890625);
            cr.CurveTo(38.972656, 35.875, 39, 35.859375, 39.027344, 35.84375);
            cr.CurveTo(39.03125, 35.84375, 39.03125, 35.84375, 39.035156, 35.84375);
            cr.CurveTo(39.101563, 35.808594, 39.167969, 35.777344, 39.234375, 35.746094);
            cr.CurveTo(39.234375, 35.746094, 39.234375, 35.746094, 39.234375, 35.746094);
            cr.CurveTo(39.277344, 35.726563, 39.324219, 35.703125, 39.371094, 35.683594);
            cr.CurveTo(39.375, 35.683594, 39.378906, 35.679688, 39.382813, 35.679688);
            cr.CurveTo(39.429688, 35.660156, 39.472656, 35.640625, 39.515625, 35.625);
            cr.CurveTo(39.519531, 35.621094, 39.523438, 35.621094, 39.53125, 35.617188);
            cr.CurveTo(39.621094, 35.582031, 39.707031, 35.546875, 39.800781, 35.511719);
            cr.CurveTo(39.863281, 35.492188, 39.925781, 35.46875, 39.992188, 35.445313);
            cr.CurveTo(40.058594, 35.425781, 40.125, 35.40625, 40.1875, 35.386719);
            cr.CurveTo(40.277344, 35.359375, 40.367188, 35.332031, 40.453125, 35.308594);
            cr.CurveTo(40.460938, 35.308594, 40.46875, 35.304688, 40.476563, 35.304688);
            cr.CurveTo(40.601563, 35.273438, 40.726563, 35.246094, 40.851563, 35.21875);
            cr.CurveTo(40.871094, 35.214844, 40.890625, 35.210938, 40.910156, 35.210938);
            cr.CurveTo(41.027344, 35.1875, 41.144531, 35.167969, 41.261719, 35.152344);
            cr.CurveTo(41.292969, 35.148438, 41.320313, 35.144531, 41.351563, 35.140625);
            cr.CurveTo(41.449219, 35.128906, 41.542969, 35.121094, 41.640625, 35.113281);
            cr.CurveTo(41.691406, 35.109375, 41.746094, 35.101563, 41.796875, 35.101563);
            cr.CurveTo(41.839844, 35.097656, 41.882813, 35.09375, 41.925781, 35.09375);
            cr.CurveTo(42.011719, 35.089844, 42.101563, 35.089844, 42.1875, 35.085938);
            cr.ClosePath();
            cr.MoveTo(32.398438, 51.226563);
            cr.LineTo(39.285156, 55.203125);
            cr.LineTo(39.285156, 66.789063);
            cr.CurveTo(37.601563, 66.589844, 35.957031, 66.21875, 34.375, 65.6875);
            cr.CurveTo(34.371094, 65.6875, 34.371094, 65.6875, 34.367188, 65.6875);
            cr.CurveTo(33.972656, 65.554688, 33.582031, 65.414063, 33.195313, 65.261719);
            cr.CurveTo(33.195313, 65.261719, 33.195313, 65.261719, 33.191406, 65.261719);
            cr.CurveTo(33, 65.183594, 32.808594, 65.105469, 32.617188, 65.027344);
            cr.CurveTo(32.617188, 65.027344, 32.613281, 65.023438, 32.613281, 65.023438);
            cr.CurveTo(32.613281, 65.023438, 32.613281, 65.023438, 32.609375, 65.023438);
            cr.CurveTo(32.230469, 64.863281, 31.851563, 64.695313, 31.476563, 64.515625);
            cr.CurveTo(31.476563, 64.515625, 31.476563, 64.511719, 31.472656, 64.511719);
            cr.CurveTo(31.289063, 64.421875, 31.101563, 64.332031, 30.917969, 64.238281);
            cr.CurveTo(30.734375, 64.144531, 30.554688, 64.046875, 30.371094, 63.949219);
            cr.CurveTo(30.367188, 63.945313, 30.363281, 63.945313, 30.359375, 63.941406);
            cr.CurveTo(27.257813, 62.253906, 24.523438, 59.902344, 22.363281, 57.019531);
            cr.ClosePath();
            cr.MoveTo(52.035156, 51.226563);
            cr.LineTo(54.789063, 52.816406);
            cr.LineTo(62.070313, 57.019531);
            cr.CurveTo(58.003906, 62.453125, 51.886719, 65.980469, 45.148438, 66.789063);
            cr.LineTo(45.148438, 55.203125);
            cr.ClosePath();
            cr.MoveTo(52.035156, 51.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 2.5;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Round;

            cr.NewPath();
            cr.MoveTo(46.996094, 0.703125);
            cr.LineTo(44.824219, 6.175781);
            cr.LineTo(39.605469, 6.175781);
            cr.LineTo(37.4375, 0.703125);
            cr.LineTo(32.53125, 1.570313);
            cr.LineTo(32.523438, 1.570313);
            cr.LineTo(32.355469, 7.453125);
            cr.LineTo(27.457031, 9.238281);
            cr.LineTo(23.625, 4.792969);
            cr.LineTo(23.621094, 4.796875);
            cr.LineTo(23.617188, 4.792969);
            cr.LineTo(19.144531, 7.378906);
            cr.LineTo(21.078125, 12.921875);
            cr.LineTo(17.082031, 16.273438);
            cr.LineTo(12.027344, 13.332031);
            cr.LineTo(12.023438, 13.335938);
            cr.LineTo(12.019531, 13.332031);
            cr.LineTo(8.574219, 17.441406);
            cr.LineTo(8.574219, 17.445313);
            cr.LineTo(12.347656, 21.914063);
            cr.LineTo(9.738281, 26.433594);
            cr.LineTo(3.925781, 25.546875);
            cr.LineTo(2.199219, 30.292969);
            cr.LineTo(2.199219, 30.296875);
            cr.LineTo(7.222656, 33.351563);
            cr.LineTo(6.316406, 38.488281);
            cr.LineTo(0.550781, 39.6875);
            cr.LineTo(0.550781, 44.65625);
            cr.LineTo(6.316406, 45.855469);
            cr.LineTo(7.222656, 50.992188);
            cr.LineTo(2.308594, 53.988281);
            cr.LineTo(2.207031, 54.042969);
            cr.LineTo(2.214844, 54.0625);
            cr.LineTo(2.203125, 54.070313);
            cr.LineTo(3.921875, 58.792969);
            cr.LineTo(3.921875, 58.796875);
            cr.LineTo(9.742188, 57.910156);
            cr.LineTo(12.347656, 62.425781);
            cr.LineTo(8.574219, 66.898438);
            cr.LineTo(12.023438, 71.011719);
            cr.LineTo(12.027344, 71.011719);
            cr.LineTo(17.082031, 68.070313);
            cr.LineTo(21.078125, 71.421875);
            cr.LineTo(19.144531, 76.964844);
            cr.LineTo(23.621094, 79.546875);
            cr.LineTo(23.625, 79.546875);
            cr.LineTo(27.457031, 75.105469);
            cr.LineTo(32.359375, 76.886719);
            cr.LineTo(32.515625, 82.667969);
            cr.LineTo(32.511719, 82.773438);
            cr.LineTo(32.53125, 82.773438);
            cr.LineTo(32.53125, 82.789063);
            cr.LineTo(37.433594, 83.652344);
            cr.LineTo(37.4375, 83.652344);
            cr.LineTo(39.609375, 78.167969);
            cr.LineTo(44.824219, 78.167969);
            cr.LineTo(46.996094, 83.636719);
            cr.LineTo(51.902344, 82.773438);
            cr.LineTo(51.910156, 82.773438);
            cr.LineTo(51.910156, 82.769531);
            cr.LineTo(52.078125, 76.886719);
            cr.LineTo(56.976563, 75.105469);
            cr.LineTo(60.808594, 79.546875);
            cr.LineTo(60.816406, 79.546875);
            cr.LineTo(65.289063, 76.964844);
            cr.LineTo(63.355469, 71.421875);
            cr.LineTo(67.351563, 68.070313);
            cr.LineTo(72.300781, 70.980469);
            cr.LineTo(75.886719, 66.898438);
            cr.LineTo(72.085938, 62.429688);
            cr.LineTo(74.695313, 57.910156);
            cr.LineTo(80.507813, 58.796875);
            cr.LineTo(82.234375, 54.054688);
            cr.LineTo(82.230469, 54.050781);
            cr.LineTo(77.210938, 50.992188);
            cr.LineTo(78.117188, 45.855469);
            cr.LineTo(83.882813, 44.65625);
            cr.LineTo(83.882813, 39.6875);
            cr.LineTo(78.117188, 38.492188);
            cr.LineTo(77.210938, 33.355469);
            cr.LineTo(82.132813, 30.351563);
            cr.LineTo(82.226563, 30.300781);
            cr.LineTo(82.21875, 30.285156);
            cr.LineTo(82.230469, 30.277344);
            cr.LineTo(82.226563, 30.273438);
            cr.LineTo(82.230469, 30.273438);
            cr.LineTo(80.511719, 25.550781);
            cr.LineTo(74.691406, 26.433594);
            cr.LineTo(72.085938, 21.917969);
            cr.LineTo(75.859375, 17.445313);
            cr.LineTo(72.414063, 13.335938);
            cr.LineTo(72.410156, 13.335938);
            cr.LineTo(72.40625, 13.332031);
            cr.LineTo(72.40625, 13.335938);
            cr.LineTo(67.351563, 16.277344);
            cr.LineTo(63.355469, 12.925781);
            cr.LineTo(65.289063, 7.382813);
            cr.LineTo(65.285156, 7.382813);
            cr.LineTo(65.285156, 7.378906);
            cr.LineTo(60.8125, 4.796875);
            cr.LineTo(60.808594, 4.796875);
            cr.LineTo(56.976563, 9.242188);
            cr.LineTo(52.078125, 7.457031);
            cr.LineTo(51.90625, 1.574219);
            cr.LineTo(51.90625, 1.570313);
            cr.ClosePath();
            cr.MoveTo(45.148438, 17.554688);
            cr.CurveTo(45.304688, 17.570313, 45.457031, 17.59375, 45.609375, 17.613281);
            cr.CurveTo(45.636719, 17.617188, 45.667969, 17.621094, 45.695313, 17.625);
            cr.LineTo(45.703125, 17.625);
            cr.CurveTo(45.820313, 17.640625, 45.9375, 17.660156, 46.054688, 17.679688);
            cr.CurveTo(46.136719, 17.691406, 46.222656, 17.707031, 46.304688, 17.71875);
            cr.CurveTo(46.414063, 17.738281, 46.523438, 17.757813, 46.632813, 17.777344);
            cr.CurveTo(46.714844, 17.792969, 46.792969, 17.808594, 46.875, 17.824219);
            cr.CurveTo(46.988281, 17.84375, 47.101563, 17.867188, 47.210938, 17.890625);
            cr.CurveTo(47.292969, 17.90625, 47.371094, 17.921875, 47.449219, 17.941406);
            cr.CurveTo(47.566406, 17.964844, 47.679688, 17.992188, 47.792969, 18.015625);
            cr.CurveTo(47.867188, 18.035156, 47.9375, 18.050781, 48.007813, 18.066406);
            cr.CurveTo(48.128906, 18.097656, 48.25, 18.125, 48.367188, 18.15625);
            cr.CurveTo(48.425781, 18.171875, 48.484375, 18.1875, 48.542969, 18.203125);
            cr.CurveTo(48.851563, 18.285156, 49.160156, 18.371094, 49.464844, 18.464844);
            cr.CurveTo(49.664063, 18.523438, 49.863281, 18.589844, 50.0625, 18.65625);
            cr.CurveTo(50.257813, 18.722656, 50.457031, 18.789063, 50.652344, 18.859375);
            cr.CurveTo(50.652344, 18.859375, 50.652344, 18.859375, 50.652344, 18.859375);
            cr.CurveTo(50.847656, 18.933594, 51.042969, 19.003906, 51.238281, 19.082031);
            cr.CurveTo(51.238281, 19.082031, 51.238281, 19.082031, 51.238281, 19.082031);
            cr.CurveTo(51.628906, 19.234375, 52.011719, 19.394531, 52.390625, 19.566406);
            cr.CurveTo(52.769531, 19.738281, 53.144531, 19.917969, 53.515625, 20.105469);
            cr.CurveTo(53.699219, 20.199219, 53.882813, 20.296875, 54.066406, 20.398438);
            cr.CurveTo(54.433594, 20.597656, 54.792969, 20.804688, 55.148438, 21.019531);
            cr.CurveTo(55.503906, 21.238281, 55.851563, 21.464844, 56.195313, 21.699219);
            cr.CurveTo(56.441406, 21.867188, 56.679688, 22.039063, 56.917969, 22.214844);
            cr.CurveTo(56.949219, 22.234375, 56.980469, 22.257813, 57.011719, 22.28125);
            cr.CurveTo(57.242188, 22.453125, 57.472656, 22.632813, 57.703125, 22.8125);
            cr.CurveTo(57.863281, 22.941406, 58.027344, 23.074219, 58.183594, 23.210938);
            cr.LineTo(58.1875, 23.210938);
            cr.CurveTo(58.34375, 23.34375, 58.503906, 23.480469, 58.660156, 23.617188);
            cr.CurveTo(58.816406, 23.757813, 58.96875, 23.898438, 59.125, 24.039063);
            cr.CurveTo(59.308594, 24.214844, 59.492188, 24.390625, 59.675781, 24.570313);
            cr.CurveTo(59.691406, 24.585938, 59.707031, 24.601563, 59.722656, 24.617188);
            cr.LineTo(59.726563, 24.621094);
            cr.CurveTo(59.878906, 24.777344, 60.03125, 24.929688, 60.183594, 25.089844);
            cr.CurveTo(60.21875, 25.128906, 60.257813, 25.167969, 60.292969, 25.207031);
            cr.CurveTo(60.292969, 25.207031, 60.292969, 25.207031, 60.296875, 25.210938);
            cr.CurveTo(60.4375, 25.359375, 60.578125, 25.515625, 60.714844, 25.667969);
            cr.CurveTo(60.742188, 25.699219, 60.769531, 25.726563, 60.792969, 25.757813);
            cr.CurveTo(60.960938, 25.945313, 61.125, 26.136719, 61.285156, 26.328125);
            cr.CurveTo(61.421875, 26.492188, 61.554688, 26.65625, 61.683594, 26.820313);
            cr.CurveTo(61.683594, 26.820313, 61.6875, 26.820313, 61.6875, 26.820313);
            cr.CurveTo(61.816406, 26.988281, 61.945313, 27.15625, 62.070313, 27.324219);
            cr.LineTo(52.035156, 33.117188);
            cr.LineTo(45.148438, 29.140625);
            cr.ClosePath();
            cr.MoveTo(39.285156, 17.554688);
            cr.LineTo(39.285156, 29.140625);
            cr.LineTo(32.394531, 33.117188);
            cr.LineTo(22.363281, 27.324219);
            cr.CurveTo(22.394531, 27.28125, 22.429688, 27.238281, 22.460938, 27.195313);
            cr.CurveTo(22.546875, 27.082031, 22.632813, 26.96875, 22.71875, 26.859375);
            cr.CurveTo(22.777344, 26.785156, 22.835938, 26.710938, 22.898438, 26.636719);
            cr.CurveTo(22.960938, 26.558594, 23.023438, 26.480469, 23.085938, 26.402344);
            cr.CurveTo(23.148438, 26.328125, 23.214844, 26.25, 23.28125, 26.171875);
            cr.CurveTo(23.347656, 26.09375, 23.410156, 26.019531, 23.476563, 25.941406);
            cr.CurveTo(23.539063, 25.871094, 23.597656, 25.800781, 23.660156, 25.734375);
            cr.CurveTo(23.734375, 25.652344, 23.804688, 25.570313, 23.878906, 25.488281);
            cr.CurveTo(23.941406, 25.421875, 24.003906, 25.355469, 24.0625, 25.289063);
            cr.CurveTo(24.132813, 25.214844, 24.203125, 25.140625, 24.277344, 25.066406);
            cr.CurveTo(24.34375, 24.992188, 24.410156, 24.921875, 24.480469, 24.851563);
            cr.CurveTo(24.542969, 24.789063, 24.605469, 24.722656, 24.671875, 24.660156);
            cr.CurveTo(24.75, 24.582031, 24.824219, 24.503906, 24.90625, 24.425781);
            cr.CurveTo(24.933594, 24.398438, 24.964844, 24.371094, 24.996094, 24.339844);
            cr.CurveTo(25.097656, 24.238281, 25.203125, 24.140625, 25.308594, 24.042969);
            cr.CurveTo(25.460938, 23.898438, 25.617188, 23.757813, 25.773438, 23.621094);
            cr.CurveTo(25.929688, 23.480469, 26.085938, 23.34375, 26.246094, 23.210938);
            cr.CurveTo(26.246094, 23.210938, 26.246094, 23.210938, 26.246094, 23.210938);
            cr.CurveTo(26.40625, 23.074219, 26.566406, 22.945313, 26.730469, 22.8125);
            cr.CurveTo(26.730469, 22.8125, 26.730469, 22.8125, 26.730469, 22.8125);
            cr.CurveTo(27.058594, 22.554688, 27.390625, 22.300781, 27.726563, 22.058594);
            cr.CurveTo(28.066406, 21.816406, 28.40625, 21.578125, 28.757813, 21.355469);
            cr.CurveTo(28.933594, 21.242188, 29.109375, 21.128906, 29.285156, 21.023438);
            cr.CurveTo(29.640625, 20.804688, 30, 20.597656, 30.367188, 20.398438);
            cr.CurveTo(30.730469, 20.199219, 31.101563, 20.011719, 31.476563, 19.828125);
            cr.CurveTo(31.746094, 19.699219, 32.023438, 19.574219, 32.296875, 19.453125);
            cr.CurveTo(32.316406, 19.445313, 32.335938, 19.4375, 32.355469, 19.425781);
            cr.CurveTo(32.632813, 19.308594, 32.914063, 19.191406, 33.195313, 19.082031);
            cr.CurveTo(33.390625, 19.007813, 33.582031, 18.933594, 33.78125, 18.863281);
            cr.CurveTo(33.976563, 18.789063, 34.171875, 18.722656, 34.371094, 18.65625);
            cr.CurveTo(34.570313, 18.589844, 34.769531, 18.527344, 34.96875, 18.464844);
            cr.CurveTo(35.46875, 18.3125, 35.976563, 18.175781, 36.484375, 18.054688);
            cr.CurveTo(36.996094, 17.929688, 37.511719, 17.824219, 38.03125, 17.738281);
            cr.LineTo(38.03125, 17.734375);
            cr.CurveTo(38.242188, 17.699219, 38.449219, 17.667969, 38.65625, 17.636719);
            cr.CurveTo(38.65625, 17.636719, 38.660156, 17.636719, 38.660156, 17.636719);
            cr.CurveTo(38.867188, 17.605469, 39.074219, 17.578125, 39.285156, 17.554688);
            cr.ClosePath();
            cr.MoveTo(65, 32.402344);
            cr.CurveTo(65.21875, 32.910156, 65.421875, 33.429688, 65.601563, 33.949219);
            cr.CurveTo(65.605469, 33.949219, 65.605469, 33.949219, 65.605469, 33.953125);
            cr.CurveTo(65.910156, 34.820313, 66.164063, 35.703125, 66.371094, 36.59375);
            cr.CurveTo(66.417969, 36.796875, 66.460938, 37, 66.503906, 37.207031);
            cr.CurveTo(66.503906, 37.207031, 66.503906, 37.207031, 66.503906, 37.207031);
            cr.CurveTo(66.542969, 37.410156, 66.585938, 37.617188, 66.621094, 37.820313);
            cr.CurveTo(66.65625, 38.027344, 66.691406, 38.234375, 66.722656, 38.4375);
            cr.CurveTo(66.785156, 38.851563, 66.835938, 39.265625, 66.878906, 39.679688);
            cr.CurveTo(66.921875, 40.09375, 66.953125, 40.507813, 66.972656, 40.921875);
            cr.CurveTo(66.984375, 41.128906, 66.992188, 41.339844, 66.996094, 41.546875);
            cr.CurveTo(67.007813, 41.960938, 67.007813, 42.378906, 66.996094, 42.792969);
            cr.CurveTo(66.988281, 43.210938, 66.964844, 43.625, 66.933594, 44.039063);
            cr.CurveTo(66.886719, 44.664063, 66.816406, 45.285156, 66.722656, 45.902344);
            cr.CurveTo(66.691406, 46.109375, 66.65625, 46.3125, 66.621094, 46.519531);
            cr.CurveTo(66.585938, 46.722656, 66.546875, 46.929688, 66.503906, 47.132813);
            cr.CurveTo(66.460938, 47.339844, 66.417969, 47.542969, 66.371094, 47.746094);
            cr.CurveTo(66.136719, 48.765625, 65.835938, 49.773438, 65.46875, 50.765625);
            cr.CurveTo(65.398438, 50.960938, 65.320313, 51.160156, 65.242188, 51.355469);
            cr.CurveTo(65.164063, 51.550781, 65.082031, 51.746094, 65, 51.941406);
            cr.LineTo(56.769531, 47.1875);
            cr.LineTo(54.964844, 46.148438);
            cr.LineTo(54.964844, 38.195313);
            cr.LineTo(62.800781, 33.671875);
            cr.ClosePath();
            cr.MoveTo(19.433594, 32.402344);
            cr.LineTo(29.464844, 38.195313);
            cr.LineTo(29.464844, 46.148438);
            cr.LineTo(21.632813, 50.667969);
            cr.LineTo(19.433594, 51.941406);
            cr.CurveTo(18.847656, 50.578125, 18.390625, 49.171875, 18.0625, 47.75);
            cr.CurveTo(18.015625, 47.542969, 17.972656, 47.339844, 17.929688, 47.136719);
            cr.CurveTo(17.886719, 46.929688, 17.847656, 46.726563, 17.8125, 46.519531);
            cr.CurveTo(17.777344, 46.316406, 17.742188, 46.109375, 17.710938, 45.902344);
            cr.CurveTo(17.648438, 45.492188, 17.597656, 45.078125, 17.554688, 44.664063);
            cr.CurveTo(17.511719, 44.25, 17.480469, 43.832031, 17.460938, 43.417969);
            cr.CurveTo(17.449219, 43.210938, 17.441406, 43.003906, 17.4375, 42.796875);
            cr.LineTo(17.4375, 42.792969);
            cr.CurveTo(17.425781, 42.378906, 17.425781, 41.964844, 17.4375, 41.546875);
            cr.CurveTo(17.445313, 41.132813, 17.46875, 40.714844, 17.5, 40.300781);
            cr.CurveTo(17.546875, 39.679688, 17.617188, 39.058594, 17.710938, 38.441406);
            cr.CurveTo(17.742188, 38.234375, 17.777344, 38.027344, 17.8125, 37.824219);
            cr.CurveTo(17.847656, 37.617188, 17.886719, 37.414063, 17.929688, 37.207031);
            cr.CurveTo(17.972656, 37.003906, 18.015625, 36.800781, 18.0625, 36.59375);
            cr.CurveTo(18.296875, 35.574219, 18.597656, 34.566406, 18.964844, 33.578125);
            cr.CurveTo(19.035156, 33.378906, 19.113281, 33.183594, 19.191406, 32.984375);
            cr.CurveTo(19.269531, 32.789063, 19.351563, 32.59375, 19.433594, 32.402344);
            cr.ClosePath();
            cr.MoveTo(42.1875, 35.085938);
            cr.CurveTo(42.207031, 35.085938, 42.222656, 35.085938, 42.242188, 35.085938);
            cr.CurveTo(42.246094, 35.085938, 42.246094, 35.085938, 42.25, 35.085938);
            cr.CurveTo(42.339844, 35.089844, 42.429688, 35.089844, 42.519531, 35.09375);
            cr.CurveTo(42.519531, 35.09375, 42.523438, 35.09375, 42.523438, 35.09375);
            cr.CurveTo(42.558594, 35.097656, 42.589844, 35.097656, 42.621094, 35.097656);
            cr.CurveTo(42.625, 35.097656, 42.625, 35.097656, 42.625, 35.097656);
            cr.CurveTo(42.691406, 35.101563, 42.757813, 35.109375, 42.824219, 35.113281);
            cr.CurveTo(42.902344, 35.121094, 42.980469, 35.128906, 43.058594, 35.136719);
            cr.CurveTo(43.082031, 35.140625, 43.105469, 35.144531, 43.128906, 35.148438);
            cr.CurveTo(43.171875, 35.152344, 43.210938, 35.160156, 43.25, 35.164063);
            cr.CurveTo(43.320313, 35.175781, 43.390625, 35.1875, 43.460938, 35.199219);
            cr.CurveTo(43.519531, 35.207031, 43.574219, 35.21875, 43.632813, 35.230469);
            cr.CurveTo(43.714844, 35.25, 43.796875, 35.265625, 43.875, 35.285156);
            cr.CurveTo(43.921875, 35.296875, 43.96875, 35.308594, 44.015625, 35.320313);
            cr.CurveTo(44.015625, 35.320313, 44.015625, 35.320313, 44.019531, 35.320313);
            cr.CurveTo(44.058594, 35.332031, 44.101563, 35.34375, 44.140625, 35.355469);
            cr.CurveTo(44.253906, 35.386719, 44.363281, 35.421875, 44.476563, 35.457031);
            cr.CurveTo(44.515625, 35.472656, 44.558594, 35.484375, 44.597656, 35.5);
            cr.CurveTo(44.71875, 35.542969, 44.835938, 35.589844, 44.953125, 35.640625);
            cr.CurveTo(44.984375, 35.652344, 45.015625, 35.664063, 45.046875, 35.675781);
            cr.CurveTo(45.050781, 35.679688, 45.058594, 35.679688, 45.0625, 35.683594);
            cr.CurveTo(45.109375, 35.703125, 45.152344, 35.726563, 45.195313, 35.746094);
            cr.CurveTo(45.199219, 35.746094, 45.199219, 35.746094, 45.199219, 35.746094);
            cr.CurveTo(45.265625, 35.777344, 45.335938, 35.808594, 45.402344, 35.84375);
            cr.CurveTo(45.402344, 35.84375, 45.402344, 35.84375, 45.402344, 35.84375);
            cr.CurveTo(45.425781, 35.855469, 45.449219, 35.867188, 45.472656, 35.878906);
            cr.CurveTo(45.476563, 35.882813, 45.484375, 35.886719, 45.492188, 35.890625);
            cr.CurveTo(45.492188, 35.890625, 45.492188, 35.890625, 45.496094, 35.890625);
            cr.CurveTo(45.570313, 35.933594, 45.648438, 35.972656, 45.726563, 36.019531);
            cr.CurveTo(45.726563, 36.019531, 45.730469, 36.019531, 45.730469, 36.019531);
            cr.CurveTo(45.730469, 36.019531, 45.730469, 36.019531, 45.734375, 36.023438);
            cr.CurveTo(45.734375, 36.023438, 45.738281, 36.023438, 45.738281, 36.027344);
            cr.CurveTo(45.753906, 36.035156, 45.769531, 36.042969, 45.78125, 36.050781);
            cr.CurveTo(45.78125, 36.050781, 45.78125, 36.050781, 45.78125, 36.050781);
            cr.CurveTo(45.785156, 36.050781, 45.785156, 36.050781, 45.785156, 36.050781);
            cr.CurveTo(45.785156, 36.050781, 45.785156, 36.054688, 45.789063, 36.054688);
            cr.CurveTo(45.839844, 36.082031, 45.886719, 36.113281, 45.9375, 36.144531);
            cr.CurveTo(45.964844, 36.160156, 45.992188, 36.175781, 46.019531, 36.195313);
            cr.CurveTo(46.046875, 36.210938, 46.074219, 36.230469, 46.101563, 36.246094);
            cr.CurveTo(46.101563, 36.25, 46.105469, 36.25, 46.105469, 36.25);
            cr.CurveTo(46.167969, 36.292969, 46.226563, 36.332031, 46.289063, 36.375);
            cr.CurveTo(46.289063, 36.375, 46.289063, 36.375, 46.289063, 36.375);
            cr.CurveTo(46.289063, 36.375, 46.289063, 36.375, 46.289063, 36.375);
            cr.CurveTo(46.292969, 36.378906, 46.296875, 36.378906, 46.296875, 36.382813);
            cr.CurveTo(46.335938, 36.40625, 46.371094, 36.433594, 46.410156, 36.460938);
            cr.CurveTo(46.410156, 36.460938, 46.410156, 36.460938, 46.410156, 36.460938);
            cr.CurveTo(46.410156, 36.460938, 46.410156, 36.464844, 46.410156, 36.464844);
            cr.CurveTo(46.414063, 36.464844, 46.417969, 36.46875, 46.421875, 36.472656);
            cr.CurveTo(46.457031, 36.496094, 46.492188, 36.523438, 46.523438, 36.550781);
            cr.CurveTo(46.613281, 36.617188, 46.703125, 36.691406, 46.789063, 36.761719);
            cr.CurveTo(46.832031, 36.796875, 46.871094, 36.832031, 46.914063, 36.867188);
            cr.CurveTo(46.980469, 36.929688, 47.046875, 36.992188, 47.113281, 37.054688);
            cr.CurveTo(47.164063, 37.105469, 47.21875, 37.15625, 47.269531, 37.207031);
            cr.CurveTo(47.285156, 37.222656, 47.300781, 37.238281, 47.316406, 37.253906);
            cr.CurveTo(47.316406, 37.257813, 47.316406, 37.257813, 47.316406, 37.257813);
            cr.CurveTo(47.394531, 37.335938, 47.46875, 37.417969, 47.542969, 37.5);
            cr.CurveTo(47.566406, 37.527344, 47.585938, 37.550781, 47.609375, 37.578125);
            cr.CurveTo(47.679688, 37.664063, 47.75, 37.75, 47.816406, 37.835938);
            cr.CurveTo(47.824219, 37.847656, 47.832031, 37.855469, 47.839844, 37.867188);
            cr.CurveTo(47.851563, 37.878906, 47.863281, 37.894531, 47.871094, 37.90625);
            cr.CurveTo(47.929688, 37.984375, 47.988281, 38.0625, 48.042969, 38.140625);
            cr.CurveTo(48.070313, 38.183594, 48.101563, 38.226563, 48.128906, 38.273438);
            cr.CurveTo(48.132813, 38.273438, 48.132813, 38.277344, 48.136719, 38.28125);
            cr.CurveTo(48.136719, 38.28125, 48.136719, 38.28125, 48.136719, 38.28125);
            cr.CurveTo(48.160156, 38.3125, 48.179688, 38.347656, 48.199219, 38.378906);
            cr.CurveTo(48.246094, 38.453125, 48.289063, 38.527344, 48.335938, 38.601563);
            cr.CurveTo(48.335938, 38.601563, 48.335938, 38.605469, 48.335938, 38.605469);
            cr.CurveTo(48.34375, 38.617188, 48.347656, 38.628906, 48.355469, 38.636719);
            cr.CurveTo(48.359375, 38.644531, 48.363281, 38.648438, 48.363281, 38.652344);
            cr.CurveTo(48.363281, 38.652344, 48.367188, 38.65625, 48.367188, 38.65625);
            cr.CurveTo(48.378906, 38.679688, 48.390625, 38.699219, 48.402344, 38.722656);
            cr.CurveTo(48.433594, 38.78125, 48.464844, 38.835938, 48.496094, 38.894531);
            cr.CurveTo(48.496094, 38.898438, 48.5, 38.898438, 48.5, 38.902344);
            cr.CurveTo(48.511719, 38.925781, 48.523438, 38.949219, 48.535156, 38.972656);
            cr.CurveTo(48.539063, 38.976563, 48.542969, 38.980469, 48.542969, 38.988281);
            cr.CurveTo(48.542969, 38.988281, 48.542969, 38.988281, 48.546875, 38.988281);
            cr.CurveTo(48.5625, 39.019531, 48.574219, 39.050781, 48.589844, 39.082031);
            cr.CurveTo(48.597656, 39.101563, 48.609375, 39.121094, 48.617188, 39.140625);
            cr.CurveTo(48.660156, 39.226563, 48.699219, 39.316406, 48.738281, 39.402344);
            cr.CurveTo(48.746094, 39.425781, 48.753906, 39.445313, 48.761719, 39.464844);
            cr.CurveTo(48.769531, 39.480469, 48.773438, 39.492188, 48.78125, 39.507813);
            cr.CurveTo(48.820313, 39.605469, 48.855469, 39.703125, 48.890625, 39.800781);
            cr.CurveTo(48.902344, 39.832031, 48.914063, 39.863281, 48.925781, 39.894531);
            cr.CurveTo(48.957031, 40, 48.992188, 40.101563, 49.019531, 40.207031);
            cr.CurveTo(49.027344, 40.230469, 49.035156, 40.257813, 49.042969, 40.28125);
            cr.CurveTo(49.0625, 40.359375, 49.082031, 40.433594, 49.101563, 40.511719);
            cr.CurveTo(49.117188, 40.589844, 49.136719, 40.667969, 49.152344, 40.742188);
            cr.CurveTo(49.164063, 40.804688, 49.179688, 40.863281, 49.1875, 40.925781);
            cr.CurveTo(49.207031, 41.027344, 49.222656, 41.132813, 49.238281, 41.234375);
            cr.CurveTo(49.242188, 41.28125, 49.25, 41.328125, 49.253906, 41.375);
            cr.CurveTo(49.253906, 41.382813, 49.257813, 41.386719, 49.257813, 41.390625);
            cr.CurveTo(49.257813, 41.390625, 49.257813, 41.390625, 49.257813, 41.390625);
            cr.CurveTo(49.261719, 41.433594, 49.265625, 41.476563, 49.269531, 41.519531);
            cr.CurveTo(49.277344, 41.597656, 49.28125, 41.679688, 49.289063, 41.761719);
            cr.CurveTo(49.289063, 41.761719, 49.289063, 41.765625, 49.289063, 41.765625);
            cr.CurveTo(49.289063, 41.800781, 49.292969, 41.832031, 49.292969, 41.867188);
            cr.CurveTo(49.296875, 41.953125, 49.300781, 42.042969, 49.300781, 42.132813);
            cr.CurveTo(49.300781, 42.136719, 49.300781, 42.136719, 49.300781, 42.140625);
            cr.CurveTo(49.300781, 42.160156, 49.300781, 42.179688, 49.300781, 42.199219);
            cr.CurveTo(49.300781, 42.203125, 49.300781, 42.203125, 49.300781, 42.207031);
            cr.CurveTo(49.300781, 42.296875, 49.296875, 42.386719, 49.292969, 42.472656);
            cr.CurveTo(49.289063, 42.507813, 49.289063, 42.539063, 49.289063, 42.574219);
            cr.CurveTo(49.289063, 42.574219, 49.289063, 42.578125, 49.289063, 42.578125);
            cr.CurveTo(49.285156, 42.652344, 49.277344, 42.726563, 49.269531, 42.796875);
            cr.CurveTo(49.269531, 42.800781, 49.269531, 42.800781, 49.269531, 42.800781);
            cr.CurveTo(49.265625, 42.851563, 49.261719, 42.898438, 49.257813, 42.949219);
            cr.CurveTo(49.257813, 42.949219, 49.257813, 42.949219, 49.257813, 42.949219);
            cr.CurveTo(49.257813, 42.953125, 49.253906, 42.960938, 49.253906, 42.964844);
            cr.CurveTo(49.25, 43.011719, 49.242188, 43.058594, 49.238281, 43.105469);
            cr.CurveTo(49.226563, 43.175781, 49.21875, 43.25, 49.207031, 43.320313);
            cr.CurveTo(49.203125, 43.34375, 49.199219, 43.367188, 49.195313, 43.386719);
            cr.CurveTo(49.175781, 43.484375, 49.160156, 43.578125, 49.140625, 43.675781);
            cr.CurveTo(49.109375, 43.808594, 49.074219, 43.941406, 49.039063, 44.078125);
            cr.CurveTo(49.035156, 44.082031, 49.035156, 44.089844, 49.03125, 44.097656);
            cr.CurveTo(48.996094, 44.226563, 48.957031, 44.351563, 48.914063, 44.476563);
            cr.CurveTo(48.914063, 44.480469, 48.914063, 44.480469, 48.914063, 44.480469);
            cr.CurveTo(48.90625, 44.492188, 48.902344, 44.507813, 48.898438, 44.519531);
            cr.CurveTo(48.859375, 44.632813, 48.816406, 44.742188, 48.769531, 44.855469);
            cr.CurveTo(48.761719, 44.882813, 48.75, 44.910156, 48.738281, 44.9375);
            cr.CurveTo(48.699219, 45.027344, 48.660156, 45.113281, 48.617188, 45.203125);
            cr.CurveTo(48.59375, 45.25, 48.574219, 45.296875, 48.550781, 45.34375);
            cr.CurveTo(48.53125, 45.382813, 48.511719, 45.417969, 48.492188, 45.457031);
            cr.CurveTo(48.449219, 45.535156, 48.40625, 45.613281, 48.363281, 45.6875);
            cr.CurveTo(48.355469, 45.703125, 48.347656, 45.71875, 48.335938, 45.734375);
            cr.CurveTo(48.335938, 45.738281, 48.335938, 45.738281, 48.335938, 45.742188);
            cr.CurveTo(48.289063, 45.820313, 48.242188, 45.894531, 48.195313, 45.972656);
            cr.CurveTo(48.191406, 45.972656, 48.191406, 45.976563, 48.191406, 45.976563);
            cr.CurveTo(48.171875, 46.003906, 48.15625, 46.03125, 48.136719, 46.058594);
            cr.CurveTo(48.136719, 46.058594, 48.136719, 46.058594, 48.136719, 46.0625);
            cr.CurveTo(48.097656, 46.117188, 48.0625, 46.171875, 48.023438, 46.226563);
            cr.CurveTo(47.988281, 46.273438, 47.957031, 46.324219, 47.921875, 46.371094);
            cr.CurveTo(47.917969, 46.375, 47.917969, 46.375, 47.914063, 46.378906);
            cr.CurveTo(47.890625, 46.410156, 47.867188, 46.441406, 47.839844, 46.476563);
            cr.CurveTo(47.792969, 46.539063, 47.742188, 46.601563, 47.691406, 46.664063);
            cr.CurveTo(47.679688, 46.679688, 47.667969, 46.695313, 47.65625, 46.707031);
            cr.CurveTo(47.597656, 46.777344, 47.539063, 46.84375, 47.480469, 46.910156);
            cr.CurveTo(47.472656, 46.917969, 47.464844, 46.929688, 47.457031, 46.9375);
            cr.CurveTo(47.453125, 46.941406, 47.449219, 46.945313, 47.445313, 46.953125);
            cr.CurveTo(47.441406, 46.953125, 47.4375, 46.957031, 47.4375, 46.960938);
            cr.CurveTo(47.375, 47.027344, 47.3125, 47.089844, 47.25, 47.15625);
            cr.CurveTo(47.242188, 47.164063, 47.234375, 47.171875, 47.226563, 47.179688);
            cr.LineTo(47.222656, 47.183594);
            cr.CurveTo(47.222656, 47.183594, 47.21875, 47.1875, 47.214844, 47.1875);
            cr.CurveTo(47.113281, 47.292969, 47.007813, 47.386719, 46.898438, 47.484375);
            cr.CurveTo(46.867188, 47.511719, 46.835938, 47.539063, 46.800781, 47.570313);
            cr.CurveTo(46.703125, 47.652344, 46.605469, 47.730469, 46.503906, 47.808594);
            cr.CurveTo(46.472656, 47.832031, 46.441406, 47.855469, 46.410156, 47.878906);
            cr.CurveTo(46.371094, 47.910156, 46.332031, 47.9375, 46.289063, 47.964844);
            cr.CurveTo(46.230469, 48.007813, 46.167969, 48.050781, 46.105469, 48.09375);
            cr.CurveTo(46.074219, 48.113281, 46.042969, 48.132813, 46.015625, 48.148438);
            cr.CurveTo(45.9375, 48.199219, 45.863281, 48.246094, 45.785156, 48.289063);
            cr.CurveTo(45.785156, 48.289063, 45.785156, 48.289063, 45.785156, 48.292969);
            cr.CurveTo(45.78125, 48.292969, 45.777344, 48.292969, 45.777344, 48.296875);
            cr.CurveTo(45.761719, 48.304688, 45.75, 48.3125, 45.734375, 48.320313);
            cr.CurveTo(45.730469, 48.320313, 45.730469, 48.320313, 45.726563, 48.324219);
            cr.CurveTo(45.644531, 48.371094, 45.5625, 48.414063, 45.480469, 48.457031);
            cr.CurveTo(45.457031, 48.472656, 45.433594, 48.484375, 45.410156, 48.496094);
            cr.CurveTo(45.410156, 48.496094, 45.40625, 48.496094, 45.40625, 48.496094);
            cr.CurveTo(45.328125, 48.535156, 45.253906, 48.570313, 45.175781, 48.605469);
            cr.CurveTo(45.140625, 48.625, 45.101563, 48.640625, 45.066406, 48.65625);
            cr.CurveTo(45.007813, 48.683594, 44.953125, 48.707031, 44.894531, 48.730469);
            cr.CurveTo(44.839844, 48.75, 44.785156, 48.773438, 44.730469, 48.796875);
            cr.CurveTo(44.726563, 48.796875, 44.726563, 48.796875, 44.722656, 48.796875);
            cr.CurveTo(44.6875, 48.8125, 44.652344, 48.824219, 44.617188, 48.835938);
            cr.CurveTo(44.546875, 48.859375, 44.476563, 48.886719, 44.410156, 48.90625);
            cr.CurveTo(44.382813, 48.917969, 44.355469, 48.925781, 44.328125, 48.933594);
            cr.CurveTo(44.238281, 48.960938, 44.148438, 48.988281, 44.054688, 49.011719);
            cr.CurveTo(43.960938, 49.039063, 43.863281, 49.0625, 43.769531, 49.082031);
            cr.CurveTo(43.738281, 49.089844, 43.707031, 49.097656, 43.675781, 49.105469);
            cr.CurveTo(43.601563, 49.121094, 43.527344, 49.132813, 43.453125, 49.148438);
            cr.CurveTo(43.449219, 49.148438, 43.445313, 49.148438, 43.4375, 49.148438);
            cr.CurveTo(43.429688, 49.152344, 43.421875, 49.152344, 43.410156, 49.15625);
            cr.CurveTo(43.375, 49.160156, 43.339844, 49.167969, 43.300781, 49.171875);
            cr.CurveTo(43.261719, 49.179688, 43.21875, 49.183594, 43.179688, 49.191406);
            cr.CurveTo(43.117188, 49.199219, 43.058594, 49.207031, 42.996094, 49.214844);
            cr.CurveTo(42.976563, 49.214844, 42.960938, 49.214844, 42.941406, 49.21875);
            cr.CurveTo(42.910156, 49.222656, 42.878906, 49.222656, 42.851563, 49.226563);
            cr.CurveTo(42.777344, 49.234375, 42.699219, 49.238281, 42.625, 49.246094);
            cr.CurveTo(42.625, 49.246094, 42.621094, 49.246094, 42.617188, 49.246094);
            cr.CurveTo(42.585938, 49.246094, 42.558594, 49.246094, 42.527344, 49.25);
            cr.CurveTo(42.433594, 49.253906, 42.34375, 49.253906, 42.25, 49.257813);
            cr.CurveTo(42.230469, 49.257813, 42.207031, 49.257813, 42.183594, 49.257813);
            cr.CurveTo(42.09375, 49.253906, 42, 49.253906, 41.910156, 49.25);
            cr.CurveTo(41.875, 49.246094, 41.839844, 49.246094, 41.804688, 49.246094);
            cr.CurveTo(41.730469, 49.238281, 41.65625, 49.234375, 41.582031, 49.226563);
            cr.CurveTo(41.535156, 49.222656, 41.484375, 49.21875, 41.433594, 49.214844);
            cr.CurveTo(41.382813, 49.207031, 41.332031, 49.199219, 41.277344, 49.191406);
            cr.CurveTo(41.207031, 49.183594, 41.136719, 49.175781, 41.066406, 49.164063);
            cr.CurveTo(41.042969, 49.160156, 41.019531, 49.15625, 40.996094, 49.148438);
            cr.CurveTo(40.964844, 49.144531, 40.9375, 49.140625, 40.90625, 49.132813);
            cr.CurveTo(40.820313, 49.117188, 40.734375, 49.101563, 40.652344, 49.082031);
            cr.CurveTo(40.566406, 49.0625, 40.484375, 49.039063, 40.398438, 49.019531);
            cr.CurveTo(40.335938, 49, 40.273438, 48.984375, 40.207031, 48.964844);
            cr.CurveTo(40.207031, 48.964844, 40.203125, 48.964844, 40.199219, 48.960938);
            cr.CurveTo(40.195313, 48.960938, 40.191406, 48.960938, 40.1875, 48.957031);
            cr.CurveTo(40.121094, 48.9375, 40.054688, 48.917969, 39.988281, 48.898438);
            cr.CurveTo(39.921875, 48.875, 39.855469, 48.851563, 39.789063, 48.828125);
            cr.LineTo(39.785156, 48.824219);
            cr.CurveTo(39.734375, 48.804688, 39.679688, 48.785156, 39.628906, 48.765625);
            cr.CurveTo(39.597656, 48.753906, 39.5625, 48.738281, 39.527344, 48.726563);
            cr.CurveTo(39.480469, 48.707031, 39.433594, 48.6875, 39.386719, 48.667969);
            cr.CurveTo(39.339844, 48.644531, 39.292969, 48.625, 39.242188, 48.601563);
            cr.CurveTo(39.171875, 48.566406, 39.101563, 48.535156, 39.027344, 48.5);
            cr.CurveTo(39.027344, 48.496094, 39.023438, 48.496094, 39.023438, 48.496094);
            cr.CurveTo(39, 48.484375, 38.976563, 48.472656, 38.953125, 48.460938);
            cr.CurveTo(38.871094, 48.417969, 38.789063, 48.371094, 38.707031, 48.324219);
            cr.CurveTo(38.703125, 48.324219, 38.703125, 48.324219, 38.699219, 48.320313);
            cr.CurveTo(38.683594, 48.3125, 38.671875, 48.304688, 38.65625, 48.296875);
            cr.CurveTo(38.65625, 48.296875, 38.652344, 48.292969, 38.652344, 48.292969);
            cr.CurveTo(38.648438, 48.292969, 38.648438, 48.292969, 38.648438, 48.292969);
            cr.CurveTo(38.570313, 48.246094, 38.496094, 48.199219, 38.417969, 48.152344);
            cr.CurveTo(38.390625, 48.132813, 38.359375, 48.113281, 38.332031, 48.09375);
            cr.CurveTo(38.265625, 48.050781, 38.203125, 48.007813, 38.144531, 47.964844);
            cr.CurveTo(38.101563, 47.9375, 38.0625, 47.910156, 38.023438, 47.882813);
            cr.CurveTo(37.992188, 47.859375, 37.960938, 47.832031, 37.929688, 47.808594);
            cr.CurveTo(37.828125, 47.734375, 37.730469, 47.652344, 37.632813, 47.570313);
            cr.CurveTo(37.597656, 47.542969, 37.566406, 47.511719, 37.535156, 47.484375);
            cr.CurveTo(37.421875, 47.386719, 37.316406, 47.289063, 37.210938, 47.183594);
            cr.LineTo(37.207031, 47.183594);
            cr.CurveTo(37.199219, 47.175781, 37.191406, 47.164063, 37.183594, 47.15625);
            cr.CurveTo(37.121094, 47.09375, 37.058594, 47.027344, 36.996094, 46.960938);
            cr.CurveTo(36.996094, 46.957031, 36.992188, 46.957031, 36.992188, 46.953125);
            cr.CurveTo(36.984375, 46.945313, 36.976563, 46.9375, 36.96875, 46.929688);
            cr.CurveTo(36.964844, 46.925781, 36.964844, 46.925781, 36.964844, 46.925781);
            cr.CurveTo(36.898438, 46.855469, 36.839844, 46.78125, 36.777344, 46.710938);
            cr.CurveTo(36.765625, 46.695313, 36.753906, 46.679688, 36.742188, 46.667969);
            cr.CurveTo(36.691406, 46.605469, 36.640625, 46.539063, 36.59375, 46.476563);
            cr.CurveTo(36.566406, 46.445313, 36.542969, 46.414063, 36.519531, 46.378906);
            cr.CurveTo(36.515625, 46.378906, 36.515625, 46.375, 36.511719, 46.375);
            cr.CurveTo(36.476563, 46.324219, 36.445313, 46.273438, 36.410156, 46.226563);
            cr.CurveTo(36.371094, 46.171875, 36.332031, 46.117188, 36.296875, 46.0625);
            cr.CurveTo(36.296875, 46.0625, 36.296875, 46.0625, 36.296875, 46.058594);
            cr.CurveTo(36.277344, 46.03125, 36.261719, 46.003906, 36.242188, 45.976563);
            cr.CurveTo(36.242188, 45.976563, 36.242188, 45.972656, 36.238281, 45.972656);
            cr.CurveTo(36.191406, 45.898438, 36.144531, 45.820313, 36.097656, 45.742188);
            cr.CurveTo(36.097656, 45.738281, 36.097656, 45.738281, 36.097656, 45.738281);
            cr.CurveTo(36.089844, 45.726563, 36.082031, 45.714844, 36.078125, 45.703125);
            cr.CurveTo(36.074219, 45.699219, 36.070313, 45.695313, 36.070313, 45.691406);
            cr.CurveTo(36.027344, 45.613281, 35.984375, 45.535156, 35.941406, 45.457031);
            cr.CurveTo(35.921875, 45.421875, 35.902344, 45.382813, 35.882813, 45.34375);
            cr.CurveTo(35.859375, 45.296875, 35.839844, 45.25, 35.816406, 45.203125);
            cr.CurveTo(35.773438, 45.113281, 35.734375, 45.027344, 35.695313, 44.9375);
            cr.CurveTo(35.683594, 44.910156, 35.671875, 44.882813, 35.664063, 44.855469);
            cr.CurveTo(35.617188, 44.746094, 35.574219, 44.632813, 35.535156, 44.519531);
            cr.CurveTo(35.53125, 44.507813, 35.527344, 44.496094, 35.519531, 44.480469);
            cr.CurveTo(35.519531, 44.480469, 35.519531, 44.480469, 35.519531, 44.480469);
            cr.CurveTo(35.476563, 44.355469, 35.4375, 44.226563, 35.402344, 44.101563);
            cr.CurveTo(35.398438, 44.09375, 35.398438, 44.085938, 35.394531, 44.078125);
            cr.CurveTo(35.359375, 43.945313, 35.324219, 43.808594, 35.292969, 43.675781);
            cr.CurveTo(35.273438, 43.582031, 35.257813, 43.484375, 35.238281, 43.390625);
            cr.CurveTo(35.234375, 43.367188, 35.230469, 43.34375, 35.226563, 43.320313);
            cr.CurveTo(35.214844, 43.25, 35.207031, 43.179688, 35.195313, 43.105469);
            cr.CurveTo(35.191406, 43.058594, 35.183594, 43.011719, 35.179688, 42.964844);
            cr.CurveTo(35.179688, 42.960938, 35.175781, 42.957031, 35.175781, 42.949219);
            cr.CurveTo(35.175781, 42.949219, 35.175781, 42.949219, 35.175781, 42.949219);
            cr.CurveTo(35.171875, 42.90625, 35.167969, 42.867188, 35.164063, 42.824219);
            cr.CurveTo(35.15625, 42.742188, 35.148438, 42.660156, 35.144531, 42.582031);
            cr.CurveTo(35.144531, 42.578125, 35.144531, 42.578125, 35.144531, 42.574219);
            cr.CurveTo(35.144531, 42.542969, 35.140625, 42.507813, 35.140625, 42.476563);
            cr.CurveTo(35.136719, 42.386719, 35.132813, 42.296875, 35.132813, 42.207031);
            cr.CurveTo(35.132813, 42.207031, 35.132813, 42.203125, 35.132813, 42.199219);
            cr.CurveTo(35.132813, 42.179688, 35.132813, 42.160156, 35.132813, 42.140625);
            cr.CurveTo(35.132813, 42.140625, 35.132813, 42.136719, 35.132813, 42.136719);
            cr.CurveTo(35.132813, 42.046875, 35.136719, 41.957031, 35.140625, 41.867188);
            cr.CurveTo(35.144531, 41.835938, 35.144531, 41.800781, 35.144531, 41.769531);
            cr.CurveTo(35.144531, 41.765625, 35.144531, 41.765625, 35.144531, 41.761719);
            cr.CurveTo(35.148438, 41.691406, 35.15625, 41.617188, 35.164063, 41.542969);
            cr.CurveTo(35.164063, 41.542969, 35.164063, 41.542969, 35.164063, 41.542969);
            cr.CurveTo(35.167969, 41.492188, 35.171875, 41.441406, 35.175781, 41.394531);
            cr.CurveTo(35.175781, 41.394531, 35.175781, 41.394531, 35.175781, 41.394531);
            cr.CurveTo(35.175781, 41.386719, 35.179688, 41.382813, 35.179688, 41.378906);
            cr.CurveTo(35.183594, 41.332031, 35.191406, 41.28125, 35.195313, 41.234375);
            cr.CurveTo(35.199219, 41.210938, 35.203125, 41.1875, 35.207031, 41.164063);
            cr.CurveTo(35.21875, 41.09375, 35.226563, 41.023438, 35.238281, 40.953125);
            cr.CurveTo(35.238281, 40.953125, 35.242188, 40.953125, 35.242188, 40.953125);
            cr.CurveTo(35.253906, 40.875, 35.269531, 40.796875, 35.285156, 40.71875);
            cr.CurveTo(35.296875, 40.667969, 35.308594, 40.617188, 35.320313, 40.566406);
            cr.CurveTo(35.34375, 40.464844, 35.367188, 40.363281, 35.394531, 40.265625);
            cr.CurveTo(35.398438, 40.257813, 35.398438, 40.25, 35.402344, 40.242188);
            cr.CurveTo(35.4375, 40.117188, 35.476563, 39.988281, 35.519531, 39.863281);
            cr.CurveTo(35.519531, 39.863281, 35.519531, 39.863281, 35.519531, 39.863281);
            cr.CurveTo(35.527344, 39.847656, 35.53125, 39.835938, 35.535156, 39.824219);
            cr.CurveTo(35.574219, 39.710938, 35.617188, 39.597656, 35.664063, 39.488281);
            cr.CurveTo(35.671875, 39.460938, 35.683594, 39.433594, 35.695313, 39.40625);
            cr.CurveTo(35.734375, 39.316406, 35.773438, 39.226563, 35.816406, 39.140625);
            cr.CurveTo(35.839844, 39.09375, 35.859375, 39.046875, 35.882813, 39);
            cr.CurveTo(35.902344, 38.960938, 35.921875, 38.921875, 35.941406, 38.886719);
            cr.CurveTo(35.984375, 38.808594, 36.027344, 38.730469, 36.070313, 38.652344);
            cr.CurveTo(36.078125, 38.636719, 36.085938, 38.621094, 36.097656, 38.605469);
            cr.CurveTo(36.097656, 38.605469, 36.097656, 38.601563, 36.097656, 38.601563);
            cr.CurveTo(36.144531, 38.523438, 36.191406, 38.445313, 36.238281, 38.371094);
            cr.CurveTo(36.242188, 38.371094, 36.242188, 38.367188, 36.242188, 38.363281);
            cr.CurveTo(36.261719, 38.335938, 36.277344, 38.308594, 36.296875, 38.28125);
            cr.CurveTo(36.296875, 38.28125, 36.296875, 38.28125, 36.296875, 38.28125);
            cr.CurveTo(36.335938, 38.226563, 36.371094, 38.171875, 36.410156, 38.117188);
            cr.CurveTo(36.414063, 38.109375, 36.421875, 38.101563, 36.429688, 38.089844);
            cr.CurveTo(36.464844, 38.039063, 36.503906, 37.984375, 36.542969, 37.929688);
            cr.CurveTo(36.558594, 37.910156, 36.574219, 37.886719, 36.59375, 37.867188);
            cr.CurveTo(36.621094, 37.828125, 36.652344, 37.792969, 36.679688, 37.753906);
            cr.CurveTo(36.71875, 37.710938, 36.753906, 37.664063, 36.789063, 37.621094);
            cr.CurveTo(36.832031, 37.566406, 36.878906, 37.515625, 36.921875, 37.464844);
            cr.CurveTo(36.972656, 37.410156, 37.023438, 37.355469, 37.078125, 37.296875);
            cr.CurveTo(37.113281, 37.261719, 37.148438, 37.226563, 37.183594, 37.1875);
            cr.CurveTo(37.183594, 37.1875, 37.183594, 37.1875, 37.183594, 37.1875);
            cr.CurveTo(37.214844, 37.15625, 37.246094, 37.125, 37.277344, 37.097656);
            cr.CurveTo(37.363281, 37.015625, 37.445313, 36.933594, 37.535156, 36.859375);
            cr.CurveTo(37.566406, 36.828125, 37.597656, 36.800781, 37.632813, 36.773438);
            cr.CurveTo(37.730469, 36.691406, 37.828125, 36.609375, 37.929688, 36.535156);
            cr.CurveTo(37.960938, 36.507813, 37.992188, 36.484375, 38.023438, 36.460938);
            cr.CurveTo(38.0625, 36.433594, 38.101563, 36.40625, 38.144531, 36.378906);
            cr.CurveTo(38.203125, 36.335938, 38.265625, 36.292969, 38.328125, 36.25);
            cr.CurveTo(38.328125, 36.25, 38.328125, 36.25, 38.332031, 36.25);
            cr.CurveTo(38.351563, 36.234375, 38.375, 36.21875, 38.398438, 36.207031);
            cr.CurveTo(38.402344, 36.203125, 38.410156, 36.199219, 38.414063, 36.195313);
            cr.CurveTo(38.417969, 36.195313, 38.417969, 36.191406, 38.417969, 36.191406);
            cr.CurveTo(38.476563, 36.15625, 38.535156, 36.121094, 38.589844, 36.085938);
            cr.CurveTo(38.609375, 36.078125, 38.625, 36.066406, 38.640625, 36.054688);
            cr.CurveTo(38.644531, 36.054688, 38.644531, 36.054688, 38.644531, 36.054688);
            cr.CurveTo(38.644531, 36.054688, 38.648438, 36.054688, 38.648438, 36.050781);
            cr.CurveTo(38.648438, 36.050781, 38.648438, 36.050781, 38.652344, 36.050781);
            cr.CurveTo(38.652344, 36.050781, 38.65625, 36.046875, 38.65625, 36.046875);
            cr.CurveTo(38.671875, 36.039063, 38.683594, 36.03125, 38.699219, 36.023438);
            cr.CurveTo(38.703125, 36.019531, 38.703125, 36.019531, 38.707031, 36.019531);
            cr.CurveTo(38.726563, 36.007813, 38.746094, 35.996094, 38.765625, 35.988281);
            cr.CurveTo(38.824219, 35.953125, 38.882813, 35.921875, 38.941406, 35.890625);
            cr.CurveTo(38.972656, 35.875, 39, 35.859375, 39.027344, 35.84375);
            cr.CurveTo(39.03125, 35.84375, 39.03125, 35.84375, 39.035156, 35.84375);
            cr.CurveTo(39.101563, 35.808594, 39.167969, 35.777344, 39.234375, 35.746094);
            cr.CurveTo(39.234375, 35.746094, 39.234375, 35.746094, 39.234375, 35.746094);
            cr.CurveTo(39.277344, 35.726563, 39.324219, 35.703125, 39.371094, 35.683594);
            cr.CurveTo(39.375, 35.683594, 39.378906, 35.679688, 39.382813, 35.679688);
            cr.CurveTo(39.429688, 35.660156, 39.472656, 35.640625, 39.515625, 35.625);
            cr.CurveTo(39.519531, 35.621094, 39.527344, 35.621094, 39.53125, 35.617188);
            cr.CurveTo(39.621094, 35.582031, 39.707031, 35.546875, 39.800781, 35.515625);
            cr.CurveTo(39.863281, 35.492188, 39.925781, 35.46875, 39.992188, 35.445313);
            cr.CurveTo(40.058594, 35.425781, 40.125, 35.40625, 40.1875, 35.386719);
            cr.CurveTo(40.277344, 35.359375, 40.367188, 35.332031, 40.453125, 35.308594);
            cr.CurveTo(40.460938, 35.308594, 40.46875, 35.304688, 40.476563, 35.304688);
            cr.CurveTo(40.601563, 35.273438, 40.726563, 35.246094, 40.851563, 35.21875);
            cr.CurveTo(40.871094, 35.21875, 40.890625, 35.214844, 40.910156, 35.210938);
            cr.CurveTo(41.027344, 35.1875, 41.144531, 35.167969, 41.261719, 35.152344);
            cr.CurveTo(41.292969, 35.148438, 41.320313, 35.144531, 41.351563, 35.140625);
            cr.CurveTo(41.449219, 35.128906, 41.542969, 35.121094, 41.640625, 35.113281);
            cr.CurveTo(41.691406, 35.109375, 41.746094, 35.101563, 41.796875, 35.101563);
            cr.CurveTo(41.839844, 35.097656, 41.882813, 35.097656, 41.925781, 35.09375);
            cr.CurveTo(42.011719, 35.089844, 42.101563, 35.089844, 42.1875, 35.089844);
            cr.ClosePath();
            cr.MoveTo(32.398438, 51.226563);
            cr.LineTo(39.285156, 55.203125);
            cr.LineTo(39.285156, 66.789063);
            cr.CurveTo(37.601563, 66.589844, 35.957031, 66.21875, 34.375, 65.6875);
            cr.CurveTo(34.371094, 65.6875, 34.371094, 65.6875, 34.367188, 65.6875);
            cr.CurveTo(33.972656, 65.554688, 33.582031, 65.414063, 33.195313, 65.261719);
            cr.CurveTo(33.195313, 65.261719, 33.195313, 65.261719, 33.191406, 65.261719);
            cr.CurveTo(33, 65.183594, 32.808594, 65.109375, 32.617188, 65.027344);
            cr.CurveTo(32.617188, 65.027344, 32.613281, 65.023438, 32.613281, 65.023438);
            cr.CurveTo(32.613281, 65.023438, 32.613281, 65.023438, 32.609375, 65.023438);
            cr.CurveTo(32.230469, 64.863281, 31.851563, 64.695313, 31.476563, 64.515625);
            cr.CurveTo(31.476563, 64.515625, 31.476563, 64.511719, 31.472656, 64.511719);
            cr.CurveTo(31.289063, 64.421875, 31.101563, 64.332031, 30.917969, 64.238281);
            cr.CurveTo(30.734375, 64.144531, 30.554688, 64.046875, 30.371094, 63.949219);
            cr.CurveTo(30.367188, 63.945313, 30.363281, 63.945313, 30.359375, 63.941406);
            cr.CurveTo(27.257813, 62.253906, 24.523438, 59.902344, 22.363281, 57.019531);
            cr.ClosePath();
            cr.MoveTo(52.035156, 51.226563);
            cr.LineTo(54.789063, 52.816406);
            cr.LineTo(62.070313, 57.019531);
            cr.CurveTo(58.003906, 62.453125, 51.886719, 65.984375, 45.148438, 66.789063);
            cr.LineTo(45.148438, 55.203125);
            cr.ClosePath();
            cr.MoveTo(52.035156, 51.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;

            cr.LineWidth = 2;
            pattern = new SolidPattern(0.9, 0.9, 0.9, rgba[3]);
            cr.SetSource(pattern);
            matrix = new Matrix(0.219784, 0, 0, 0.219784, 111.929039, -26.59924);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            surface.BlurFull(3);
            //cr.StrokePreserve();
            //surface.Blur(3);
            if (pattern != null) pattern.Dispose();

            cr.LineWidth = 2.5;
            pattern = new SolidPattern(rgba[0] / 2, rgba[1] / 2, rgba[2] / 2, rgba[3]);
            cr.SetSource(pattern);
            matrix = new Matrix(0.219784, 0, 0, 0.219784, 111.929039, -26.59924);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();


            cr.Restore();
        }


        #region Waypoints


        public void DrawWaypointPlayer(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 26;
            float h = 39;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.75, 14.835938);
            cr.CurveTo(19.75, 7.820313, 16.726563, 2.132813, 13, 2.132813);
            cr.CurveTo(9.273438, 2.132813, 6.25, 7.820313, 6.25, 14.835938);
            cr.CurveTo(6.25, 21.851563, 9.273438, 27.539063, 13, 27.539063);
            cr.CurveTo(16.726563, 27.539063, 19.75, 21.851563, 19.75, 14.835938);
            cr.ClosePath();
            cr.MoveTo(19.75, 14.835938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 4;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.75, 14.835938);
            cr.CurveTo(19.75, 7.820313, 16.726563, 2.132813, 13, 2.132813);
            cr.CurveTo(9.273438, 2.132813, 6.25, 7.820313, 6.25, 14.835938);
            cr.CurveTo(6.25, 21.851563, 9.273438, 27.539063, 13, 27.539063);
            cr.CurveTo(16.726563, 27.539063, 19.75, 21.851563, 19.75, 14.835938);
            cr.ClosePath();
            cr.MoveTo(19.75, 14.835938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.992366, 0, 0, 0.992366, 0, 0.148855);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(24.015625, 25.851563);
            cr.CurveTo(24.015625, 31.933594, 19.082031, 36.867188, 13, 36.867188);
            cr.CurveTo(6.917969, 36.867188, 1.984375, 31.933594, 1.984375, 25.851563);
            cr.CurveTo(1.984375, 19.769531, 6.917969, 14.835938, 13, 14.835938);
            cr.CurveTo(19.082031, 14.835938, 24.015625, 19.769531, 24.015625, 25.851563);
            cr.ClosePath();
            cr.MoveTo(24.015625, 25.851563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 4;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(24.015625, 25.851563);
            cr.CurveTo(24.015625, 31.933594, 19.082031, 36.867188, 13, 36.867188);
            cr.CurveTo(6.917969, 36.867188, 1.984375, 31.933594, 1.984375, 25.851563);
            cr.CurveTo(1.984375, 19.769531, 6.917969, 14.835938, 13, 14.835938);
            cr.CurveTo(19.082031, 14.835938, 24.015625, 19.769531, 24.015625, 25.851563);
            cr.ClosePath();
            cr.MoveTo(24.015625, 25.851563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.992366, 0, 0, 0.992366, 0, 0.148855);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(11.558594, 34.703125);
            cr.CurveTo(9.667969, 34.386719, 7.675781, 33.269531, 6.339844, 31.769531);
            cr.CurveTo(2.125, 27.050781, 4, 19.707031, 10, 17.4375);
            cr.CurveTo(11.214844, 16.980469, 13.53125, 16.816406, 14.878906, 17.097656);
            cr.CurveTo(17.867188, 17.71875, 20.46875, 20.09375, 21.511719, 23.164063);
            cr.CurveTo(21.960938, 24.472656, 22.015625, 26.71875, 21.636719, 28.167969);
            cr.CurveTo(20.519531, 32.460938, 15.929688, 35.433594, 11.558594, 34.703125);
            cr.ClosePath();
            cr.MoveTo(11.558594, 34.703125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(17.296875, 13.511719);
            cr.CurveTo(17.160156, 13.433594, 16.519531, 13.253906, 15.871094, 13.105469);
            cr.CurveTo(13.867188, 12.65625, 11.019531, 12.789063, 8.785156, 13.433594);
            cr.LineTo(8.386719, 13.550781);
            cr.LineTo(8.492188, 12.335938);
            cr.CurveTo(8.722656, 9.628906, 9.820313, 6.734375, 11.15625, 5.308594);
            cr.CurveTo(12.140625, 4.25, 12.890625, 3.988281, 13.753906, 4.398438);
            cr.CurveTo(15.496094, 5.226563, 17.199219, 8.871094, 17.5625, 12.554688);
            cr.CurveTo(17.621094, 13.164063, 17.640625, 13.660156, 17.605469, 13.65625);
            cr.CurveTo(17.570313, 13.652344, 17.433594, 13.589844, 17.296875, 13.511719);
            cr.ClosePath();
            cr.MoveTo(17.296875, 13.511719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void DrawWaypointCircle(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            cr.LineWidth = 1.5;
            cr.Arc(x + width/2, y + width/2, width/2 * 0.5f, 0, 2 * Math.PI);
            cr.SetSourceRGBA(rgba);
            cr.Fill();

            cr.SetSourceRGBA(0.5, 0.5, 0.5, 1);
            cr.Stroke();
        }

        public void DrawWaypointPick(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 78;
            float h = 78;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(43.984375, 27.375);
            cr.LineTo(50.679688, 34.070313);
            cr.LineTo(7.257813, 77.496094);
            cr.LineTo(0.558594, 70.796875);
            cr.ClosePath();
            cr.MoveTo(43.984375, 27.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.223351;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(43.984375, 27.375);
            cr.LineTo(50.679688, 34.070313);
            cr.LineTo(7.257813, 77.496094);
            cr.LineTo(0.558594, 70.796875);
            cr.ClosePath();
            cr.MoveTo(43.984375, 27.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(2.505496, 2.505496, -2.505496, 2.505496, 54.439511, -366.010225);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(54.597656, 12.59375);
            cr.LineTo(65.378906, 23.378906);
            cr.LineTo(53.839844, 34.917969);
            cr.LineTo(43.058594, 24.132813);
            cr.ClosePath();
            cr.MoveTo(54.597656, 12.59375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.14611;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(54.597656, 12.59375);
            cr.LineTo(65.378906, 23.378906);
            cr.LineTo(53.839844, 34.917969);
            cr.LineTo(43.058594, 24.132813);
            cr.ClosePath();
            cr.MoveTo(54.597656, 12.59375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(2.505496, 2.505496, -2.505496, 2.505496, 54.439511, -366.010225);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(64.117188, 24.96875);
            cr.LineTo(58.507813, 30.578125);
            cr.CurveTo(58.507813, 30.578125, 64.816406, 36.835938, 70.019531, 46.054688);
            cr.CurveTo(73.320313, 51.902344, 74.496094, 63.597656, 74.496094, 63.597656);
            cr.CurveTo(74.496094, 63.597656, 80.476563, 56.988281, 75.046875, 40.792969);
            cr.CurveTo(72.902344, 34.390625, 64.117188, 24.96875, 64.117188, 24.96875);
            cr.ClosePath();
            cr.MoveTo(64.117188, 24.96875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(64.117188, 24.96875);
            cr.LineTo(58.507813, 30.578125);
            cr.CurveTo(58.507813, 30.578125, 64.816406, 36.835938, 70.019531, 46.054688);
            cr.CurveTo(73.320313, 51.902344, 74.496094, 63.597656, 74.496094, 63.597656);
            cr.CurveTo(74.496094, 63.597656, 80.476563, 56.988281, 75.046875, 40.792969);
            cr.CurveTo(72.902344, 34.390625, 64.117188, 24.96875, 64.117188, 24.96875);
            cr.ClosePath();
            cr.MoveTo(64.117188, 24.96875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(2.505496, 2.505496, -2.505496, 2.505496, 54.439511, -366.010225);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(52.753906, 13.679688);
            cr.LineTo(47.148438, 19.285156);
            cr.CurveTo(47.148438, 19.285156, 40.890625, 12.976563, 31.671875, 7.773438);
            cr.CurveTo(25.824219, 4.472656, 14.125, 3.296875, 14.125, 3.296875);
            cr.CurveTo(14.125, 3.296875, 20.734375, -2.683594, 36.933594, 2.746094);
            cr.CurveTo(43.332031, 4.894531, 52.753906, 13.679688, 52.753906, 13.679688);
            cr.ClosePath();
            cr.MoveTo(52.753906, 13.679688);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(52.753906, 13.679688);
            cr.LineTo(47.148438, 19.285156);
            cr.CurveTo(47.148438, 19.285156, 40.890625, 12.976563, 31.671875, 7.773438);
            cr.CurveTo(25.824219, 4.472656, 14.125, 3.296875, 14.125, 3.296875);
            cr.CurveTo(14.125, 3.296875, 20.734375, -2.683594, 36.933594, 2.746094);
            cr.CurveTo(43.332031, 4.894531, 52.753906, 13.679688, 52.753906, 13.679688);
            cr.ClosePath();
            cr.MoveTo(52.753906, 13.679688);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(2.505496, 2.505496, -2.505496, 2.505496, 54.439511, -366.010225);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointLadder(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 101;
            float h = 72;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(50.191406, 33.230469);
            cr.CurveTo(43.304688, 33.234375, 36.492188, 33.78125, 30.1875, 34.835938);
            cr.LineTo(30.1875, 41.871094);
            cr.LineTo(67.472656, 41.871094);
            cr.LineTo(67.472656, 34.429688);
            cr.CurveTo(61.945313, 33.644531, 56.09375, 33.238281, 50.191406, 33.230469);
            cr.ClosePath();
            cr.MoveTo(74.8125, 35.71875);
            cr.LineTo(74.8125, 68.613281);
            cr.CurveTo(90.378906, 65.25, 100.003906, 58.972656, 100.027344, 52.171875);
            cr.CurveTo(100.011719, 45.363281, 90.386719, 39.082031, 74.8125, 35.71875);
            cr.ClosePath();
            cr.MoveTo(22.84375, 36.355469);
            cr.CurveTo(8.824219, 39.851563, 0.371094, 45.796875, 0.355469, 52.171875);
            cr.CurveTo(0.363281, 58.546875, 8.816406, 64.496094, 22.84375, 67.996094);
            cr.ClosePath();
            cr.MoveTo(30.1875, 48.027344);
            cr.LineTo(30.1875, 57.972656);
            cr.LineTo(67.472656, 57.972656);
            cr.LineTo(67.472656, 48.027344);
            cr.ClosePath();
            cr.MoveTo(30.1875, 64.128906);
            cr.LineTo(30.1875, 69.492188);
            cr.CurveTo(36.488281, 70.550781, 43.300781, 71.101563, 50.191406, 71.109375);
            cr.CurveTo(56.09375, 71.105469, 61.945313, 70.703125, 67.472656, 69.921875);
            cr.LineTo(67.472656, 64.128906);
            cr.ClosePath();
            cr.MoveTo(30.1875, 64.128906);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.755906;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(50.191406, 33.230469);
            cr.CurveTo(43.304688, 33.234375, 36.492188, 33.78125, 30.1875, 34.835938);
            cr.LineTo(30.1875, 41.871094);
            cr.LineTo(67.472656, 41.871094);
            cr.LineTo(67.472656, 34.429688);
            cr.CurveTo(61.945313, 33.644531, 56.09375, 33.238281, 50.191406, 33.230469);
            cr.ClosePath();
            cr.MoveTo(74.8125, 35.71875);
            cr.LineTo(74.8125, 68.613281);
            cr.CurveTo(90.378906, 65.25, 100.003906, 58.972656, 100.027344, 52.171875);
            cr.CurveTo(100.011719, 45.363281, 90.386719, 39.082031, 74.8125, 35.71875);
            cr.ClosePath();
            cr.MoveTo(22.84375, 36.355469);
            cr.CurveTo(8.824219, 39.851563, 0.371094, 45.796875, 0.355469, 52.171875);
            cr.CurveTo(0.363281, 58.546875, 8.816406, 64.496094, 22.84375, 67.996094);
            cr.ClosePath();
            cr.MoveTo(30.1875, 48.027344);
            cr.LineTo(30.1875, 57.972656);
            cr.LineTo(67.472656, 57.972656);
            cr.LineTo(67.472656, 48.027344);
            cr.ClosePath();
            cr.MoveTo(30.1875, 64.128906);
            cr.LineTo(30.1875, 69.492188);
            cr.CurveTo(36.488281, 70.550781, 43.300781, 71.101563, 50.191406, 71.109375);
            cr.CurveTo(56.09375, 71.105469, 61.945313, 70.703125, 67.472656, 69.921875);
            cr.LineTo(67.472656, 64.128906);
            cr.ClosePath();
            cr.MoveTo(30.1875, 64.128906);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -623.257122, -384.393083);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(23.109375, 0.375);
            cr.LineTo(23.109375, 36.273438);
            cr.CurveTo(25.28125, 35.730469, 27.558594, 35.257813, 29.921875, 34.851563);
            cr.LineTo(29.921875, 0.375);
            cr.ClosePath();
            cr.MoveTo(23.109375, 0.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.801221;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(23.109375, 0.375);
            cr.LineTo(23.109375, 36.273438);
            cr.CurveTo(25.28125, 35.730469, 27.558594, 35.257813, 29.921875, 34.851563);
            cr.LineTo(29.921875, 0.375);
            cr.ClosePath();
            cr.MoveTo(23.109375, 0.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -623.257122, -384.393083);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(67.714844, 0.703125);
            cr.LineTo(67.714844, 34.304688);
            cr.CurveTo(70.074219, 34.628906, 72.367188, 35.015625, 74.574219, 35.46875);
            cr.LineTo(74.574219, 0.703125);
            cr.ClosePath();
            cr.MoveTo(67.714844, 0.703125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.806412;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(67.714844, 0.703125);
            cr.LineTo(67.714844, 34.304688);
            cr.CurveTo(70.074219, 34.628906, 72.367188, 35.015625, 74.574219, 35.46875);
            cr.LineTo(74.574219, 0.703125);
            cr.ClosePath();
            cr.MoveTo(67.714844, 0.703125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -623.257122, -384.393083);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.421875, 6.242188);
            cr.LineTo(67.828125, 6.242188);
            cr.LineTo(67.828125, 12.394531);
            cr.LineTo(30.421875, 12.394531);
            cr.ClosePath();
            cr.MoveTo(30.421875, 6.242188);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.421875, 6.242188);
            cr.LineTo(67.828125, 6.242188);
            cr.LineTo(67.828125, 12.394531);
            cr.LineTo(30.421875, 12.394531);
            cr.ClosePath();
            cr.MoveTo(30.421875, 6.242188);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -623.257122, -384.393083);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.421875, 24.707031);
            cr.LineTo(67.828125, 24.707031);
            cr.LineTo(67.828125, 30.863281);
            cr.LineTo(30.421875, 30.863281);
            cr.ClosePath();
            cr.MoveTo(30.421875, 24.707031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.421875, 24.707031);
            cr.LineTo(67.828125, 24.707031);
            cr.LineTo(67.828125, 30.863281);
            cr.LineTo(30.421875, 30.863281);
            cr.ClosePath();
            cr.MoveTo(30.421875, 24.707031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -623.257122, -384.393083);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.4;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100.574219, 51.898438);
            cr.CurveTo(100.574219, 41.4375, 78.261719, 32.957031, 50.738281, 32.957031);
            cr.CurveTo(23.214844, 32.957031, 0.902344, 41.4375, 0.902344, 51.898438);
            cr.CurveTo(0.902344, 62.359375, 23.214844, 70.839844, 50.738281, 70.839844);
            cr.CurveTo(78.261719, 70.839844, 100.574219, 62.359375, 100.574219, 51.898438);
            cr.ClosePath();
            cr.MoveTo(100.574219, 51.898438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -623.257122, -384.393083);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointHome(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 110;
            float h = 99;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(44.785156, 59.65625);
            cr.LineTo(67.039063, 59.65625);
            cr.LineTo(67.039063, 97.964844);
            cr.LineTo(44.785156, 97.964844);
            cr.ClosePath();
            cr.MoveTo(44.785156, 59.65625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.266077;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(44.785156, 59.65625);
            cr.LineTo(67.039063, 59.65625);
            cr.LineTo(67.039063, 97.964844);
            cr.LineTo(44.785156, 97.964844);
            cr.ClosePath();
            cr.MoveTo(44.785156, 59.65625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -352.016688, -246.50022);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.570313, 56.621094);
            cr.LineTo(41.824219, 56.621094);
            cr.LineTo(41.824219, 97.847656);
            cr.LineTo(19.570313, 97.847656);
            cr.ClosePath();
            cr.MoveTo(19.570313, 56.621094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.276022;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.570313, 56.621094);
            cr.LineTo(41.824219, 56.621094);
            cr.LineTo(41.824219, 97.847656);
            cr.LineTo(19.570313, 97.847656);
            cr.ClosePath();
            cr.MoveTo(19.570313, 56.621094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -352.016688, -246.50022);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70, 56.507813);
            cr.LineTo(92.253906, 56.507813);
            cr.LineTo(92.253906, 98.007813);
            cr.LineTo(70, 98.007813);
            cr.ClosePath();
            cr.MoveTo(70, 56.507813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.276936;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70, 56.507813);
            cr.LineTo(92.253906, 56.507813);
            cr.LineTo(92.253906, 98.007813);
            cr.LineTo(70, 98.007813);
            cr.ClosePath();
            cr.MoveTo(70, 56.507813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -352.016688, -246.50022);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.609375, 43.207031);
            cr.LineTo(92.25, 43.207031);
            cr.LineTo(92.25, 56.507813);
            cr.LineTo(19.609375, 56.507813);
            cr.ClosePath();
            cr.MoveTo(19.609375, 43.207031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.283257;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.609375, 43.207031);
            cr.LineTo(92.25, 43.207031);
            cr.LineTo(92.25, 56.507813);
            cr.LineTo(19.609375, 56.507813);
            cr.ClosePath();
            cr.MoveTo(19.609375, 43.207031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -352.016688, -246.50022);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.421875, 40.121094);
            cr.LineTo(108.792969, 40.121094);
            cr.LineTo(54.929688, 0.582031);
            cr.ClosePath();
            cr.MoveTo(1.421875, 40.121094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.421875, 40.121094);
            cr.LineTo(108.792969, 40.121094);
            cr.LineTo(54.929688, 0.582031);
            cr.ClosePath();
            cr.MoveTo(1.421875, 40.121094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -352.016688, -246.50022);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(92.625, 24.640625);
            cr.LineTo(82.332031, 17.105469);
            cr.LineTo(82.332031, 2.457031);
            cr.LineTo(92.710938, 2.457031);
            cr.ClosePath();
            cr.MoveTo(92.625, 24.640625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(92.625, 24.640625);
            cr.LineTo(82.332031, 17.105469);
            cr.LineTo(82.332031, 2.457031);
            cr.LineTo(92.710938, 2.457031);
            cr.ClosePath();
            cr.MoveTo(92.625, 24.640625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -352.016688, -246.50022);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointCave(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 86;
            float h = 56;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(6.480469, 55.003906);
            cr.LineTo(23.746094, 55.003906);
            cr.LineTo(28.679688, 36.191406);
            cr.LineTo(31.964844, 55.003906);
            cr.LineTo(47.589844, 55.003906);
            cr.LineTo(52.933594, 25.214844);
            cr.LineTo(57.863281, 53.960938);
            cr.LineTo(85.40625, 53.960938);
            cr.LineTo(84.996094, 42.984375);
            cr.LineTo(80.886719, 29.917969);
            cr.LineTo(73.898438, 16.851563);
            cr.LineTo(71.84375, 32.53125);
            cr.LineTo(68.964844, 13.191406);
            cr.LineTo(61.566406, 4.308594);
            cr.LineTo(50.875, 0.648438);
            cr.LineTo(43.476563, 0.648438);
            cr.LineTo(41.421875, 30.964844);
            cr.LineTo(37.3125, 7.964844);
            cr.LineTo(36.078125, 19.988281);
            cr.LineTo(31.554688, 5.875);
            cr.LineTo(22.511719, 10.578125);
            cr.LineTo(11.824219, 19.464844);
            cr.LineTo(3.191406, 32.53125);
            cr.LineTo(0.722656, 55.003906);
            cr.ClosePath();
            cr.MoveTo(6.480469, 55.003906);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.36629;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(6.480469, 55.003906);
            cr.LineTo(23.746094, 55.003906);
            cr.LineTo(28.679688, 36.191406);
            cr.LineTo(31.964844, 55.003906);
            cr.LineTo(47.589844, 55.003906);
            cr.LineTo(52.933594, 25.214844);
            cr.LineTo(57.863281, 53.960938);
            cr.LineTo(85.40625, 53.960938);
            cr.LineTo(84.996094, 42.984375);
            cr.LineTo(80.886719, 29.917969);
            cr.LineTo(73.898438, 16.851563);
            cr.LineTo(71.84375, 32.53125);
            cr.LineTo(68.964844, 13.191406);
            cr.LineTo(61.566406, 4.308594);
            cr.LineTo(50.875, 0.648438);
            cr.LineTo(43.476563, 0.648438);
            cr.LineTo(41.421875, 30.964844);
            cr.LineTo(37.3125, 7.964844);
            cr.LineTo(36.078125, 19.988281);
            cr.LineTo(31.554688, 5.875);
            cr.LineTo(22.511719, 10.578125);
            cr.LineTo(11.824219, 19.464844);
            cr.LineTo(3.191406, 32.53125);
            cr.LineTo(0.722656, 55.003906);
            cr.ClosePath();
            cr.MoveTo(6.480469, 55.003906);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -505.725591, -396.845823);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void DrawWaypointVessel(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 81;
            float h = 85;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(7, 77.371094);
            cr.LineTo(72.890625, 77.371094);
            cr.LineTo(72.890625, 84.074219);
            cr.LineTo(7, 84.074219);
            cr.ClosePath();
            cr.MoveTo(7, 77.371094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.240944;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(7, 77.371094);
            cr.LineTo(72.890625, 77.371094);
            cr.LineTo(72.890625, 84.074219);
            cr.LineTo(7, 84.074219);
            cr.ClosePath();
            cr.MoveTo(7, 77.371094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -623.457957, -133.061857);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.296875, 21.320313);
            cr.LineTo(1.296875, 26.53125);
            cr.CurveTo(8.367188, 27.929688, 24.253906, 31.1875, 24.253906, 31.1875);
            cr.LineTo(28.855469, 40.226563);
            cr.LineTo(42.667969, 37.214844);
            cr.LineTo(48.445313, 40.8125);
            cr.LineTo(55.644531, 34.703125);
            cr.LineTo(51.289063, 24.238281);
            cr.LineTo(56.566406, 25.996094);
            cr.LineTo(60.917969, 35.875);
            cr.LineTo(51.792969, 43.324219);
            cr.LineTo(57.566406, 47.089844);
            cr.LineTo(56.980469, 57.972656);
            cr.LineTo(64.265625, 66.007813);
            cr.LineTo(60.246094, 67.847656);
            cr.LineTo(52.714844, 59.3125);
            cr.LineTo(53.550781, 49.433594);
            cr.LineTo(42, 41.566406);
            cr.LineTo(30.28125, 44.078125);
            cr.LineTo(34.132813, 50.691406);
            cr.LineTo(30.78125, 56.046875);
            cr.LineTo(35.804688, 61.570313);
            cr.LineTo(32.957031, 64.835938);
            cr.LineTo(25.761719, 56.632813);
            cr.LineTo(29.277344, 50.773438);
            cr.LineTo(21.15625, 34.871094);
            cr.LineTo(1.296875, 31.519531);
            cr.LineTo(1.296875, 73.503906);
            cr.LineTo(79.542969, 73.503906);
            cr.LineTo(79.542969, 21.320313);
            cr.ClosePath();
            cr.MoveTo(1.296875, 21.320313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 2.768866;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.296875, 21.320313);
            cr.LineTo(1.296875, 26.53125);
            cr.CurveTo(8.367188, 27.929688, 24.253906, 31.1875, 24.253906, 31.1875);
            cr.LineTo(28.855469, 40.226563);
            cr.LineTo(42.667969, 37.214844);
            cr.LineTo(48.445313, 40.8125);
            cr.LineTo(55.644531, 34.703125);
            cr.LineTo(51.289063, 24.238281);
            cr.LineTo(56.566406, 25.996094);
            cr.LineTo(60.917969, 35.875);
            cr.LineTo(51.792969, 43.324219);
            cr.LineTo(57.566406, 47.089844);
            cr.LineTo(56.980469, 57.972656);
            cr.LineTo(64.265625, 66.007813);
            cr.LineTo(60.246094, 67.847656);
            cr.LineTo(52.714844, 59.3125);
            cr.LineTo(53.550781, 49.433594);
            cr.LineTo(42, 41.566406);
            cr.LineTo(30.28125, 44.078125);
            cr.LineTo(34.132813, 50.691406);
            cr.LineTo(30.78125, 56.046875);
            cr.LineTo(35.804688, 61.570313);
            cr.LineTo(32.957031, 64.835938);
            cr.LineTo(25.761719, 56.632813);
            cr.LineTo(29.277344, 50.773438);
            cr.LineTo(21.15625, 34.871094);
            cr.LineTo(1.296875, 31.519531);
            cr.LineTo(1.296875, 73.503906);
            cr.LineTo(79.542969, 73.503906);
            cr.LineTo(79.542969, 21.320313);
            cr.ClosePath();
            cr.MoveTo(1.296875, 21.320313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -623.457957, -133.061857);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(7, 10.445313);
            cr.LineTo(72.890625, 10.445313);
            cr.LineTo(72.890625, 17.148438);
            cr.LineTo(7, 17.148438);
            cr.ClosePath();
            cr.MoveTo(7, 10.445313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.240944;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(7, 10.445313);
            cr.LineTo(72.890625, 10.445313);
            cr.LineTo(72.890625, 17.148438);
            cr.LineTo(7, 17.148438);
            cr.ClosePath();
            cr.MoveTo(7, 10.445313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -623.457957, -133.061857);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(2.375, 0.457031);
            cr.LineTo(77.515625, 0.457031);
            cr.LineTo(77.515625, 7.160156);
            cr.LineTo(2.375, 7.160156);
            cr.ClosePath();
            cr.MoveTo(2.375, 0.457031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.257309;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(2.375, 0.457031);
            cr.LineTo(77.515625, 0.457031);
            cr.LineTo(77.515625, 7.160156);
            cr.LineTo(2.375, 7.160156);
            cr.ClosePath();
            cr.MoveTo(2.375, 0.457031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -623.457957, -133.061857);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointTrader(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 79;
            float h = 77;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70.195313, 65.226563);
            cr.CurveTo(70.195313, 71.4375, 65.160156, 76.472656, 58.949219, 76.472656);
            cr.CurveTo(52.738281, 76.472656, 47.707031, 71.4375, 47.707031, 65.226563);
            cr.CurveTo(47.707031, 59.015625, 52.738281, 53.980469, 58.949219, 53.980469);
            cr.CurveTo(65.160156, 53.980469, 70.195313, 59.015625, 70.195313, 65.226563);
            cr.ClosePath();
            cr.MoveTo(70.195313, 65.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70.195313, 65.226563);
            cr.CurveTo(70.195313, 71.4375, 65.160156, 76.472656, 58.949219, 76.472656);
            cr.CurveTo(52.738281, 76.472656, 47.707031, 71.4375, 47.707031, 65.226563);
            cr.CurveTo(47.707031, 59.015625, 52.738281, 53.980469, 58.949219, 53.980469);
            cr.CurveTo(65.160156, 53.980469, 70.195313, 59.015625, 70.195313, 65.226563);
            cr.ClosePath();
            cr.MoveTo(70.195313, 65.226563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(35.277344, 65.105469);
            cr.CurveTo(35.277344, 71.316406, 30.242188, 76.351563, 24.03125, 76.351563);
            cr.CurveTo(17.820313, 76.351563, 12.785156, 71.316406, 12.785156, 65.105469);
            cr.CurveTo(12.785156, 58.894531, 17.820313, 53.859375, 24.03125, 53.859375);
            cr.CurveTo(30.242188, 53.859375, 35.277344, 58.894531, 35.277344, 65.105469);
            cr.ClosePath();
            cr.MoveTo(35.277344, 65.105469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(35.277344, 65.105469);
            cr.CurveTo(35.277344, 71.316406, 30.242188, 76.351563, 24.03125, 76.351563);
            cr.CurveTo(17.820313, 76.351563, 12.785156, 71.316406, 12.785156, 65.105469);
            cr.CurveTo(12.785156, 58.894531, 17.820313, 53.859375, 24.03125, 53.859375);
            cr.CurveTo(30.242188, 53.859375, 35.277344, 58.894531, 35.277344, 65.105469);
            cr.ClosePath();
            cr.MoveTo(35.277344, 65.105469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4.378906, 46.640625);
            cr.LineTo(4.378906, 64.160156);
            cr.LineTo(11.222656, 64.160156);
            cr.CurveTo(11.726563, 57.582031, 17.308594, 52.492188, 24.03125, 52.488281);
            cr.CurveTo(30.753906, 52.488281, 36.34375, 57.578125, 36.847656, 64.160156);
            cr.LineTo(46.253906, 64.160156);
            cr.CurveTo(46.699219, 57.53125, 52.304688, 52.375, 59.070313, 52.371094);
            cr.CurveTo(65.839844, 52.371094, 71.449219, 57.527344, 71.894531, 64.160156);
            cr.LineTo(78.484375, 64.160156);
            cr.LineTo(78.484375, 46.640625);
            cr.ClosePath();
            cr.MoveTo(4.378906, 46.640625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.755906;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4.378906, 46.640625);
            cr.LineTo(4.378906, 64.160156);
            cr.LineTo(11.222656, 64.160156);
            cr.CurveTo(11.726563, 57.582031, 17.308594, 52.492188, 24.03125, 52.488281);
            cr.CurveTo(30.753906, 52.488281, 36.34375, 57.578125, 36.847656, 64.160156);
            cr.LineTo(46.253906, 64.160156);
            cr.CurveTo(46.699219, 57.53125, 52.304688, 52.375, 59.070313, 52.371094);
            cr.CurveTo(65.839844, 52.371094, 71.449219, 57.527344, 71.894531, 64.160156);
            cr.LineTo(78.484375, 64.160156);
            cr.LineTo(78.484375, 46.640625);
            cr.ClosePath();
            cr.MoveTo(4.378906, 46.640625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4.378906, 22.371094);
            cr.LineTo(4.378906, 46.640625);
            cr.LineTo(55.992188, 46.640625);
            cr.LineTo(55.992188, 22.371094);
            cr.ClosePath();
            cr.MoveTo(27.214844, 26.117188);
            cr.LineTo(40.441406, 26.117188);
            cr.LineTo(40.441406, 39.007813);
            cr.LineTo(27.214844, 39.007813);
            cr.ClosePath();
            cr.MoveTo(27.214844, 26.117188);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.755906;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4.378906, 22.371094);
            cr.LineTo(4.378906, 46.640625);
            cr.LineTo(55.992188, 46.640625);
            cr.LineTo(55.992188, 22.371094);
            cr.ClosePath();
            cr.MoveTo(27.214844, 26.117188);
            cr.LineTo(40.441406, 26.117188);
            cr.LineTo(40.441406, 39.007813);
            cr.LineTo(27.214844, 39.007813);
            cr.ClosePath();
            cr.MoveTo(27.214844, 26.117188);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.355469, 0.355469);
            cr.LineTo(78.839844, 0.355469);
            cr.LineTo(78.839844, 20.359375);
            cr.LineTo(0.355469, 20.359375);
            cr.ClosePath();
            cr.MoveTo(0.355469, 0.355469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.355469, 0.355469);
            cr.LineTo(78.839844, 0.355469);
            cr.LineTo(78.839844, 20.359375);
            cr.LineTo(0.355469, 20.359375);
            cr.ClosePath();
            cr.MoveTo(0.355469, 0.355469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(57.265625, 33.316406);
            cr.LineTo(78.695313, 33.316406);
            cr.LineTo(78.695313, 35.40625);
            cr.LineTo(57.265625, 35.40625);
            cr.ClosePath();
            cr.MoveTo(57.265625, 33.316406);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(57.265625, 33.316406);
            cr.LineTo(78.695313, 33.316406);
            cr.LineTo(78.695313, 35.40625);
            cr.LineTo(57.265625, 35.40625);
            cr.ClosePath();
            cr.MoveTo(57.265625, 33.316406);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(61.074219, 35.28125);
            cr.LineTo(63.167969, 35.28125);
            cr.LineTo(63.167969, 45.074219);
            cr.LineTo(61.074219, 45.074219);
            cr.ClosePath();
            cr.MoveTo(61.074219, 35.28125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(61.074219, 35.28125);
            cr.LineTo(63.167969, 35.28125);
            cr.LineTo(63.167969, 45.074219);
            cr.LineTo(61.074219, 45.074219);
            cr.ClosePath();
            cr.MoveTo(61.074219, 35.28125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(67.433594, 35.28125);
            cr.LineTo(69.527344, 35.28125);
            cr.LineTo(69.527344, 45.074219);
            cr.LineTo(67.433594, 45.074219);
            cr.ClosePath();
            cr.MoveTo(67.433594, 35.28125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(67.433594, 35.28125);
            cr.LineTo(69.527344, 35.28125);
            cr.LineTo(69.527344, 45.074219);
            cr.LineTo(67.433594, 45.074219);
            cr.ClosePath();
            cr.MoveTo(67.433594, 35.28125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(73.714844, 35.113281);
            cr.LineTo(75.804688, 35.113281);
            cr.LineTo(75.804688, 44.910156);
            cr.LineTo(73.714844, 44.910156);
            cr.ClosePath();
            cr.MoveTo(73.714844, 35.113281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(73.714844, 35.113281);
            cr.LineTo(75.804688, 35.113281);
            cr.LineTo(75.804688, 44.910156);
            cr.LineTo(73.714844, 44.910156);
            cr.ClosePath();
            cr.MoveTo(73.714844, 35.113281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(28.824219, 27.640625);
            cr.LineTo(38.589844, 27.640625);
            cr.LineTo(38.589844, 37.109375);
            cr.LineTo(28.824219, 37.109375);
            cr.ClosePath();
            cr.MoveTo(28.824219, 27.640625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(28.824219, 27.640625);
            cr.LineTo(38.589844, 27.640625);
            cr.LineTo(38.589844, 37.109375);
            cr.LineTo(28.824219, 37.109375);
            cr.ClosePath();
            cr.MoveTo(28.824219, 27.640625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -252.025955, -136.123087);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointStar2(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 100;
            float h = 95;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(49.949219, 0.800781);
            cr.LineTo(65.148438, 31.597656);
            cr.LineTo(99.136719, 36.539063);
            cr.LineTo(74.542969, 60.511719);
            cr.LineTo(80.347656, 94.359375);
            cr.LineTo(49.949219, 78.378906);
            cr.LineTo(19.550781, 94.359375);
            cr.LineTo(25.355469, 60.511719);
            cr.LineTo(0.761719, 36.539063);
            cr.LineTo(34.75, 31.597656);
            cr.ClosePath();
            cr.MoveTo(49.949219, 0.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(49.949219, 0.800781);
            cr.LineTo(65.148438, 31.597656);
            cr.LineTo(99.136719, 36.539063);
            cr.LineTo(74.542969, 60.511719);
            cr.LineTo(80.347656, 94.359375);
            cr.LineTo(49.949219, 78.378906);
            cr.LineTo(19.550781, 94.359375);
            cr.LineTo(25.355469, 60.511719);
            cr.LineTo(0.761719, 36.539063);
            cr.LineTo(34.75, 31.597656);
            cr.ClosePath();
            cr.MoveTo(49.949219, 0.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -615.676004, -245.155238);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointStar1(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 97;
            float h = 97;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(96.402344, 48.6875);
            cr.LineTo(65.558594, 65.558594);
            cr.LineTo(48.6875, 96.402344);
            cr.LineTo(31.820313, 65.558594);
            cr.LineTo(0.976563, 48.6875);
            cr.LineTo(31.820313, 31.820313);
            cr.LineTo(48.6875, 0.976563);
            cr.LineTo(65.558594, 31.820313);
            cr.ClosePath();
            cr.MoveTo(96.402344, 48.6875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(96.402344, 48.6875);
            cr.LineTo(65.558594, 65.558594);
            cr.LineTo(48.6875, 96.402344);
            cr.LineTo(31.820313, 65.558594);
            cr.LineTo(0.976563, 48.6875);
            cr.LineTo(31.820313, 31.820313);
            cr.LineTo(48.6875, 0.976563);
            cr.LineTo(65.558594, 31.820313);
            cr.ClosePath();
            cr.MoveTo(96.402344, 48.6875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -492.549626, -244.465395);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointSpiral(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 102;
            float h = 94;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 3.865 * 2.5;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(42.414063, 53.421875);
            cr.CurveTo(38.78125, 50.910156, 39.828125, 45.539063, 42.640625, 42.867188);
            cr.CurveTo(47.402344, 38.34375, 55.039063, 40.097656, 58.875, 44.898438);
            cr.CurveTo(64.371094, 51.773438, 61.851563, 61.863281, 55.042969, 66.8125);
            cr.CurveTo(46.089844, 73.316406, 33.484375, 70, 27.449219, 61.171875);
            cr.CurveTo(19.921875, 50.160156, 24.035156, 35.015625, 34.890625, 27.902344);
            cr.CurveTo(47.957031, 19.335938, 65.65625, 24.261719, 73.839844, 37.148438);
            cr.CurveTo(83.445313, 52.261719, 77.707031, 72.523438, 62.792969, 81.777344);
            cr.CurveTo(45.632813, 92.421875, 22.804688, 85.871094, 12.484375, 68.921875);
            cr.CurveTo(0.789063, 49.71875, 8.160156, 24.320313, 27.140625, 12.9375);
            cr.CurveTo(48.386719, 0.195313, 76.363281, 8.378906, 88.804688, 29.394531);
            cr.CurveTo(99.523438, 47.496094, 96.761719, 70.976563, 82.890625, 86.609375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.158052, -481.352003);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 2.065 * 2.5;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Round;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(46.902344, 53.464844);
            cr.CurveTo(46.902344, 51.371094, 45.3125, 49.675781, 43.351563, 49.675781);
            cr.CurveTo(41.390625, 49.675781, 39.800781, 51.371094, 39.800781, 53.464844);
            cr.CurveTo(39.800781, 55.558594, 41.390625, 57.253906, 43.351563, 57.253906);
            cr.CurveTo(45.3125, 57.253906, 46.902344, 55.558594, 46.902344, 53.464844);
            cr.ClosePath();
            cr.MoveTo(46.902344, 53.464844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.158052, -481.352003);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }





        public void DrawWaypointRuins(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 116;
            float h = 61;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.300781, 55.210938);
            cr.LineTo(0.300781, 60.570313);
            cr.LineTo(105.339844, 60.570313);
            cr.LineTo(105.339844, 57.410156);
            cr.CurveTo(104.492188, 56.628906, 103.71875, 55.933594, 102.90625, 55.210938);
            cr.ClosePath();
            cr.MoveTo(0.300781, 55.210938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.64416;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.300781, 55.210938);
            cr.LineTo(0.300781, 60.570313);
            cr.LineTo(105.339844, 60.570313);
            cr.LineTo(105.339844, 57.410156);
            cr.CurveTo(104.492188, 56.628906, 103.71875, 55.933594, 102.90625, 55.210938);
            cr.ClosePath();
            cr.MoveTo(0.300781, 55.210938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(6.867188, 49.855469);
            cr.LineTo(99.261719, 49.855469);
            cr.LineTo(99.261719, 54.878906);
            cr.LineTo(6.867188, 54.878906);
            cr.ClosePath();
            cr.MoveTo(6.867188, 49.855469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.170434;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(6.867188, 49.855469);
            cr.LineTo(99.261719, 49.855469);
            cr.LineTo(99.261719, 54.878906);
            cr.LineTo(6.867188, 54.878906);
            cr.ClosePath();
            cr.MoveTo(6.867188, 49.855469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(12.914063, 6.222656);
            cr.LineTo(23.613281, 6.222656);
            cr.LineTo(23.613281, 49.351563);
            cr.LineTo(12.914063, 49.351563);
            cr.ClosePath();
            cr.MoveTo(12.914063, 6.222656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.206208;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(12.914063, 6.222656);
            cr.LineTo(23.613281, 6.222656);
            cr.LineTo(23.613281, 49.351563);
            cr.LineTo(12.914063, 49.351563);
            cr.ClosePath();
            cr.MoveTo(12.914063, 6.222656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(9.875, 0.300781);
            cr.LineTo(26.410156, 0.300781);
            cr.LineTo(26.410156, 6.160156);
            cr.LineTo(9.875, 6.160156);
            cr.ClosePath();
            cr.MoveTo(9.875, 0.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.170434;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(9.875, 0.300781);
            cr.LineTo(26.410156, 0.300781);
            cr.LineTo(26.410156, 6.160156);
            cr.LineTo(9.875, 6.160156);
            cr.ClosePath();
            cr.MoveTo(9.875, 0.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(36.585938, 6.382813);
            cr.LineTo(47.28125, 6.382813);
            cr.LineTo(47.28125, 49.515625);
            cr.LineTo(36.585938, 49.515625);
            cr.ClosePath();
            cr.MoveTo(36.585938, 6.382813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.206208;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(36.585938, 6.382813);
            cr.LineTo(47.28125, 6.382813);
            cr.LineTo(47.28125, 49.515625);
            cr.LineTo(36.585938, 49.515625);
            cr.ClosePath();
            cr.MoveTo(36.585938, 6.382813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(33.546875, 0.464844);
            cr.LineTo(50.078125, 0.464844);
            cr.LineTo(50.078125, 6.324219);
            cr.LineTo(33.546875, 6.324219);
            cr.ClosePath();
            cr.MoveTo(33.546875, 0.464844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.170434;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(33.546875, 0.464844);
            cr.LineTo(50.078125, 0.464844);
            cr.LineTo(50.078125, 6.324219);
            cr.LineTo(33.546875, 6.324219);
            cr.ClosePath();
            cr.MoveTo(33.546875, 0.464844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(60.023438, 16.011719);
            cr.LineTo(60.023438, 49.683594);
            cr.LineTo(70.71875, 49.683594);
            cr.LineTo(70.71875, 22.992188);
            cr.LineTo(69.851563, 22.929688);
            cr.LineTo(66.671875, 20.835938);
            cr.LineTo(65.417969, 16.902344);
            cr.CurveTo(65.417969, 16.902344, 64.109375, 16.316406, 63.433594, 16.011719);
            cr.ClosePath();
            cr.MoveTo(60.023438, 16.011719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.779368;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(60.023438, 16.011719);
            cr.LineTo(60.023438, 49.683594);
            cr.LineTo(70.71875, 49.683594);
            cr.LineTo(70.71875, 22.992188);
            cr.LineTo(69.851563, 22.929688);
            cr.LineTo(66.671875, 20.835938);
            cr.LineTo(65.417969, 16.902344);
            cr.CurveTo(65.417969, 16.902344, 64.109375, 16.316406, 63.433594, 16.011719);
            cr.ClosePath();
            cr.MoveTo(60.023438, 16.011719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(91.28125, 31.71875);
            cr.LineTo(88.183594, 33.726563);
            cr.LineTo(84.835938, 33.644531);
            cr.LineTo(83.34375, 34.976563);
            cr.LineTo(83.34375, 49.421875);
            cr.LineTo(94.039063, 49.421875);
            cr.LineTo(94.039063, 33.558594);
            cr.ClosePath();
            cr.MoveTo(91.28125, 31.71875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.647486;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(91.28125, 31.71875);
            cr.LineTo(88.183594, 33.726563);
            cr.LineTo(84.835938, 33.644531);
            cr.LineTo(83.34375, 34.976563);
            cr.LineTo(83.34375, 49.421875);
            cr.LineTo(94.039063, 49.421875);
            cr.LineTo(94.039063, 33.558594);
            cr.ClosePath();
            cr.MoveTo(91.28125, 31.71875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(103.449219, 45.875);
            cr.LineTo(101.441406, 47.941406);
            cr.CurveTo(101.667969, 48.46875, 101.890625, 48.984375, 101.890625, 48.984375);
            cr.LineTo(101.304688, 49.984375);
            cr.LineTo(101.890625, 51.972656);
            cr.LineTo(101.675781, 52.320313);
            cr.LineTo(110.195313, 60.597656);
            cr.LineTo(112.199219, 60.597656);
            cr.LineTo(115.308594, 57.394531);
            cr.ClosePath();
            cr.MoveTo(103.449219, 45.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.64416;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(103.449219, 45.875);
            cr.LineTo(101.441406, 47.941406);
            cr.CurveTo(101.667969, 48.46875, 101.890625, 48.984375, 101.890625, 48.984375);
            cr.LineTo(101.304688, 49.984375);
            cr.LineTo(101.890625, 51.972656);
            cr.LineTo(101.675781, 52.320313);
            cr.LineTo(110.195313, 60.597656);
            cr.LineTo(112.199219, 60.597656);
            cr.LineTo(115.308594, 57.394531);
            cr.ClosePath();
            cr.MoveTo(103.449219, 45.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(53.320313, 48.3125);
            cr.CurveTo(53.734375, 48.339844, 58.527344, 48.3125, 58.527344, 48.3125);
            cr.LineTo(58.527344, 45.261719);
            cr.LineTo(56.398438, 44.878906);
            cr.LineTo(54.949219, 46.417969);
            cr.LineTo(52.582031, 46.949219);
            cr.ClosePath();
            cr.MoveTo(53.320313, 48.3125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(53.320313, 48.3125);
            cr.CurveTo(53.734375, 48.339844, 58.527344, 48.3125, 58.527344, 48.3125);
            cr.LineTo(58.527344, 45.261719);
            cr.LineTo(56.398438, 44.878906);
            cr.LineTo(54.949219, 46.417969);
            cr.LineTo(52.582031, 46.949219);
            cr.ClosePath();
            cr.MoveTo(53.320313, 48.3125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(81.699219, 48.542969);
            cr.CurveTo(81.488281, 48.625, 76.214844, 48.542969, 76.214844, 48.542969);
            cr.CurveTo(76.214844, 48.542969, 76.382813, 45.527344, 76.550781, 45.570313);
            cr.CurveTo(76.71875, 45.613281, 79.269531, 43.9375, 79.269531, 43.9375);
            cr.LineTo(81.699219, 46.242188);
            cr.ClosePath();
            cr.MoveTo(81.699219, 48.542969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(81.699219, 48.542969);
            cr.CurveTo(81.488281, 48.625, 76.214844, 48.542969, 76.214844, 48.542969);
            cr.CurveTo(76.214844, 48.542969, 76.382813, 45.527344, 76.550781, 45.570313);
            cr.CurveTo(76.71875, 45.613281, 79.269531, 43.9375, 79.269531, 43.9375);
            cr.LineTo(81.699219, 46.242188);
            cr.ClosePath();
            cr.MoveTo(81.699219, 48.542969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -354.28179, -151.113224);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWaypointRocks(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 128;
            float h = 69;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(47.019531, 0.488281);
            cr.LineTo(25.589844, 3.167969);
            cr.CurveTo(25.589844, 3.167969, 20.234375, 15.035156, 17.292969, 19.75);
            cr.CurveTo(19.128906, 22.652344, 22.945313, 28.414063, 22.945313, 28.414063);
            cr.LineTo(33.125, 31.847656);
            cr.LineTo(34.308594, 36.34375);
            cr.LineTo(20.574219, 30.070313);
            cr.LineTo(15.289063, 22.042969);
            cr.CurveTo(12.5625, 24.1875, 4.621094, 31.863281, 1.816406, 34.976563);
            cr.CurveTo(-1.195313, 38.324219, 1.816406, 46.695313, 1.816406, 46.695313);
            cr.LineTo(12.867188, 59.75);
            cr.LineTo(24.417969, 60.671875);
            cr.LineTo(33.960938, 52.21875);
            cr.LineTo(44.570313, 53.613281);
            cr.LineTo(47.6875, 62.011719);
            cr.LineTo(73.804688, 59.75);
            cr.LineTo(73.804688, 49.039063);
            cr.LineTo(81.503906, 37.652344);
            cr.LineTo(94.5625, 31.292969);
            cr.LineTo(80.5, 15.554688);
            cr.LineTo(70.445313, 16.179688);
            cr.LineTo(73.964844, 20.957031);
            cr.LineTo(73.90625, 27.882813);
            cr.LineTo(69.527344, 33.859375);
            cr.LineTo(68.578125, 30.605469);
            cr.LineTo(71.332031, 26.914063);
            cr.LineTo(71.449219, 21.648438);
            cr.LineTo(67.105469, 16.226563);
            cr.ClosePath();
            cr.MoveTo(47.019531, 0.488281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(47.019531, 0.488281);
            cr.LineTo(25.589844, 3.167969);
            cr.CurveTo(25.589844, 3.167969, 20.234375, 15.035156, 17.292969, 19.75);
            cr.CurveTo(19.128906, 22.652344, 22.945313, 28.414063, 22.945313, 28.414063);
            cr.LineTo(33.125, 31.847656);
            cr.LineTo(34.308594, 36.34375);
            cr.LineTo(20.574219, 30.070313);
            cr.LineTo(15.289063, 22.042969);
            cr.CurveTo(12.5625, 24.1875, 4.621094, 31.863281, 1.816406, 34.976563);
            cr.CurveTo(-1.195313, 38.324219, 1.816406, 46.695313, 1.816406, 46.695313);
            cr.LineTo(12.867188, 59.75);
            cr.LineTo(24.417969, 60.671875);
            cr.LineTo(33.960938, 52.21875);
            cr.LineTo(44.570313, 53.613281);
            cr.LineTo(47.6875, 62.011719);
            cr.LineTo(73.804688, 59.75);
            cr.LineTo(73.804688, 49.039063);
            cr.LineTo(81.503906, 37.652344);
            cr.LineTo(94.5625, 31.292969);
            cr.LineTo(80.5, 15.554688);
            cr.LineTo(70.445313, 16.179688);
            cr.LineTo(73.964844, 20.957031);
            cr.LineTo(73.90625, 27.882813);
            cr.LineTo(69.527344, 33.859375);
            cr.LineTo(68.578125, 30.605469);
            cr.LineTo(71.332031, 26.914063);
            cr.LineTo(71.449219, 21.648438);
            cr.LineTo(67.105469, 16.226563);
            cr.ClosePath();
            cr.MoveTo(47.019531, 0.488281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(0.9375, 0, 0, 0.9375, -496.731573, -169.619323);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(103.027344, 32.023438);
            cr.LineTo(84.855469, 40.84375);
            cr.LineTo(77.515625, 49.722656);
            cr.LineTo(76.628906, 64.402344);
            cr.LineTo(86.15625, 67.714844);
            cr.LineTo(103.324219, 68.664063);
            cr.LineTo(112.199219, 64.28125);
            cr.LineTo(121.480469, 64.402344);
            cr.LineTo(127.675781, 55.816406);
            cr.LineTo(122.738281, 39.328125);
            cr.LineTo(109.007813, 31.292969);
            cr.ClosePath();
            cr.MoveTo(103.027344, 32.023438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(103.027344, 32.023438);
            cr.LineTo(84.855469, 40.84375);
            cr.LineTo(77.515625, 49.722656);
            cr.LineTo(76.628906, 64.402344);
            cr.LineTo(86.15625, 67.714844);
            cr.LineTo(103.324219, 68.664063);
            cr.LineTo(112.199219, 64.28125);
            cr.LineTo(121.480469, 64.402344);
            cr.LineTo(127.675781, 55.816406);
            cr.LineTo(122.738281, 39.328125);
            cr.LineTo(109.007813, 31.292969);
            cr.ClosePath();
            cr.MoveTo(103.027344, 32.023438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -496.731573, -169.619323);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(44.339844, 63.183594);
            cr.LineTo(41.953125, 56.363281);
            cr.LineTo(35.257813, 55.648438);
            cr.LineTo(27.558594, 62.179688);
            cr.LineTo(27.347656, 65.234375);
            cr.LineTo(32.914063, 67.789063);
            cr.LineTo(40.949219, 67.621094);
            cr.ClosePath();
            cr.MoveTo(44.339844, 63.183594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.264583;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(44.339844, 63.183594);
            cr.LineTo(41.953125, 56.363281);
            cr.LineTo(35.257813, 55.648438);
            cr.LineTo(27.558594, 62.179688);
            cr.LineTo(27.347656, 65.234375);
            cr.LineTo(32.914063, 67.789063);
            cr.LineTo(40.949219, 67.621094);
            cr.ClosePath();
            cr.MoveTo(44.339844, 63.183594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -496.731573, -169.619323);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void DrawWayointBee(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 118;
            float h = 84;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(58.746094, 31.207031);
            cr.CurveTo(54.160156, 31.226563, 48.335938, 36.019531, 45.0625, 40.820313);
            cr.LineTo(72.511719, 40.988281);
            cr.CurveTo(69.238281, 36.183594, 63.332031, 31.226563, 58.746094, 31.207031);
            cr.ClosePath();
            cr.MoveTo(43.410156, 44.761719);
            cr.CurveTo(42.902344, 46.117188, 42.054688, 49.710938, 41.714844, 51.179688);
            cr.LineTo(75.945313, 51.261719);
            cr.CurveTo(75.605469, 49.796875, 74.59375, 46.117188, 74.085938, 44.761719);
            cr.ClosePath();
            cr.MoveTo(41.261719, 55.035156);
            cr.CurveTo(41.203125, 55.855469, 41.171875, 56.675781, 41.167969, 57.496094);
            cr.CurveTo(41.171875, 58.222656, 41.363281, 61.457031, 41.410156, 62.175781);
            cr.LineTo(76.078125, 62.261719);
            cr.CurveTo(76.125, 61.539063, 76.316406, 58.222656, 76.324219, 57.496094);
            cr.CurveTo(76.320313, 56.675781, 76.292969, 55.855469, 76.242188, 55.035156);
            cr.ClosePath();
            cr.MoveTo(42.070313, 65.699219);
            cr.CurveTo(42.417969, 67.296875, 42.824219, 69.75, 44.679688, 72.746094);
            cr.LineTo(72.742188, 72.910156);
            cr.CurveTo(74.210938, 71.023438, 75.082031, 67.300781, 75.433594, 65.699219);
            cr.ClosePath();
            cr.MoveTo(46.519531, 76.351563);
            cr.CurveTo(49.796875, 81.109375, 54.179688, 83.777344, 58.746094, 83.785156);
            cr.CurveTo(63.320313, 83.785156, 67.714844, 81.117188, 70.996094, 76.351563);
            cr.ClosePath();
            cr.MoveTo(46.519531, 76.351563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.185269;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(58.746094, 31.207031);
            cr.CurveTo(54.160156, 31.226563, 48.335938, 36.019531, 45.0625, 40.820313);
            cr.LineTo(72.511719, 40.988281);
            cr.CurveTo(69.238281, 36.183594, 63.332031, 31.226563, 58.746094, 31.207031);
            cr.ClosePath();
            cr.MoveTo(43.410156, 44.761719);
            cr.CurveTo(42.902344, 46.117188, 42.054688, 49.710938, 41.714844, 51.179688);
            cr.LineTo(75.945313, 51.261719);
            cr.CurveTo(75.605469, 49.796875, 74.59375, 46.117188, 74.085938, 44.761719);
            cr.ClosePath();
            cr.MoveTo(41.261719, 55.035156);
            cr.CurveTo(41.203125, 55.855469, 41.171875, 56.675781, 41.167969, 57.496094);
            cr.CurveTo(41.171875, 58.222656, 41.363281, 61.457031, 41.410156, 62.175781);
            cr.LineTo(76.078125, 62.261719);
            cr.CurveTo(76.125, 61.539063, 76.316406, 58.222656, 76.324219, 57.496094);
            cr.CurveTo(76.320313, 56.675781, 76.292969, 55.855469, 76.242188, 55.035156);
            cr.ClosePath();
            cr.MoveTo(42.070313, 65.699219);
            cr.CurveTo(42.417969, 67.296875, 42.824219, 69.75, 44.679688, 72.746094);
            cr.LineTo(72.742188, 72.910156);
            cr.CurveTo(74.210938, 71.023438, 75.082031, 67.300781, 75.433594, 65.699219);
            cr.ClosePath();
            cr.MoveTo(46.519531, 76.351563);
            cr.CurveTo(49.796875, 81.109375, 54.179688, 83.777344, 58.746094, 83.785156);
            cr.CurveTo(63.320313, 83.785156, 67.714844, 81.117188, 70.996094, 76.351563);
            cr.ClosePath();
            cr.MoveTo(46.519531, 76.351563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(72.183594, 24.976563);
            cr.CurveTo(72.183594, 19.734375, 66.292969, 15.484375, 59.023438, 15.484375);
            cr.CurveTo(51.753906, 15.484375, 45.863281, 19.734375, 45.863281, 24.976563);
            cr.CurveTo(45.863281, 30.21875, 51.753906, 34.46875, 59.023438, 34.46875);
            cr.CurveTo(66.292969, 34.46875, 72.183594, 30.21875, 72.183594, 24.976563);
            cr.ClosePath();
            cr.MoveTo(72.183594, 24.976563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.188078;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(72.183594, 24.976563);
            cr.CurveTo(72.183594, 19.734375, 66.292969, 15.484375, 59.023438, 15.484375);
            cr.CurveTo(51.753906, 15.484375, 45.863281, 19.734375, 45.863281, 24.976563);
            cr.CurveTo(45.863281, 30.21875, 51.753906, 34.46875, 59.023438, 34.46875);
            cr.CurveTo(66.292969, 34.46875, 72.183594, 30.21875, 72.183594, 24.976563);
            cr.ClosePath();
            cr.MoveTo(72.183594, 24.976563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(69.320313, 8.402344);
            cr.CurveTo(69.320313, 3.957031, 64.976563, 0.355469, 59.613281, 0.355469);
            cr.CurveTo(54.253906, 0.355469, 49.90625, 3.957031, 49.90625, 8.402344);
            cr.CurveTo(49.90625, 12.851563, 54.253906, 16.453125, 59.613281, 16.453125);
            cr.CurveTo(64.976563, 16.453125, 69.320313, 12.851563, 69.320313, 8.402344);
            cr.ClosePath();
            cr.MoveTo(69.320313, 8.402344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(69.320313, 8.402344);
            cr.CurveTo(69.320313, 3.957031, 64.976563, 0.355469, 59.613281, 0.355469);
            cr.CurveTo(54.253906, 0.355469, 49.90625, 3.957031, 49.90625, 8.402344);
            cr.CurveTo(49.90625, 12.851563, 54.253906, 16.453125, 59.613281, 16.453125);
            cr.CurveTo(64.976563, 16.453125, 69.320313, 12.851563, 69.320313, 8.402344);
            cr.ClosePath();
            cr.MoveTo(69.320313, 8.402344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.543307, 0, 0, 3.543307, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(117.222656, 9);
            cr.CurveTo(115.738281, 3.464844, 104.542969, 1.652344, 92.21875, 4.953125);
            cr.CurveTo(79.894531, 8.257813, 71.105469, 15.421875, 72.589844, 20.960938);
            cr.CurveTo(74.074219, 26.496094, 85.265625, 28.308594, 97.59375, 25.007813);
            cr.CurveTo(109.917969, 21.703125, 118.703125, 14.539063, 117.222656, 9);
            cr.ClosePath();
            cr.MoveTo(117.222656, 9);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(117.222656, 9);
            cr.CurveTo(115.738281, 3.464844, 104.542969, 1.652344, 92.21875, 4.953125);
            cr.CurveTo(79.894531, 8.257813, 71.105469, 15.421875, 72.589844, 20.960938);
            cr.CurveTo(74.074219, 26.496094, 85.265625, 28.308594, 97.59375, 25.007813);
            cr.CurveTo(109.917969, 21.703125, 118.703125, 14.539063, 117.222656, 9);
            cr.ClosePath();
            cr.MoveTo(117.222656, 9);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(3.422572, -0.917075, 0.917075, 3.422572, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(101.746094, 54.027344);
            cr.CurveTo(103.851563, 49.046875, 98.976563, 39.816406, 90.851563, 33.414063);
            cr.CurveTo(82.730469, 27.015625, 74.433594, 25.863281, 72.328125, 30.84375);
            cr.CurveTo(70.222656, 35.828125, 75.097656, 45.054688, 83.222656, 51.457031);
            cr.CurveTo(91.34375, 57.855469, 99.640625, 59.007813, 101.746094, 54.027344);
            cr.ClosePath();
            cr.MoveTo(101.746094, 54.027344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.174912;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(101.746094, 54.027344);
            cr.CurveTo(103.851563, 49.046875, 98.976563, 39.816406, 90.851563, 33.414063);
            cr.CurveTo(82.730469, 27.015625, 74.433594, 25.863281, 72.328125, 30.84375);
            cr.CurveTo(70.222656, 35.828125, 75.097656, 45.054688, 83.222656, 51.457031);
            cr.CurveTo(91.34375, 57.855469, 99.640625, 59.007813, 101.746094, 54.027344);
            cr.ClosePath();
            cr.MoveTo(101.746094, 54.027344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(2.783042, 2.193103, -1.380399, 3.263361, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.515625, 8.839844);
            cr.CurveTo(2, 3.300781, 13.195313, 1.492188, 25.519531, 4.792969);
            cr.CurveTo(37.84375, 8.097656, 46.632813, 15.261719, 45.148438, 20.796875);
            cr.CurveTo(43.664063, 26.335938, 32.46875, 28.148438, 20.144531, 24.84375);
            cr.CurveTo(7.820313, 21.542969, -0.964844, 14.375, 0.515625, 8.839844);
            cr.ClosePath();
            cr.MoveTo(0.515625, 8.839844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.2;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.515625, 8.839844);
            cr.CurveTo(2, 3.300781, 13.195313, 1.492188, 25.519531, 4.792969);
            cr.CurveTo(37.84375, 8.097656, 46.632813, 15.261719, 45.148438, 20.796875);
            cr.CurveTo(43.664063, 26.335938, 32.46875, 28.148438, 20.144531, 24.84375);
            cr.CurveTo(7.820313, 21.542969, -0.964844, 14.375, 0.515625, 8.839844);
            cr.ClosePath();
            cr.MoveTo(0.515625, 8.839844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(-3.422572, -0.917075, -0.917075, 3.422572, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(15.992188, 53.867188);
            cr.CurveTo(13.882813, 48.882813, 18.761719, 39.65625, 26.886719, 33.253906);
            cr.CurveTo(35.007813, 26.851563, 43.300781, 25.703125, 45.410156, 30.683594);
            cr.CurveTo(47.515625, 35.664063, 42.640625, 44.894531, 34.515625, 51.292969);
            cr.CurveTo(26.394531, 57.695313, 18.097656, 58.847656, 15.992188, 53.867188);
            cr.ClosePath();
            cr.MoveTo(15.992188, 53.867188);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 0.174912;
            cr.MiterLimit = 4;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(15.992188, 53.867188);
            cr.CurveTo(13.882813, 48.882813, 18.761719, 39.65625, 26.886719, 33.253906);
            cr.CurveTo(35.007813, 26.851563, 43.300781, 25.703125, 45.410156, 30.683594);
            cr.CurveTo(47.515625, 35.664063, 42.640625, 44.894531, 34.515625, 51.292969);
            cr.CurveTo(26.394531, 57.695313, 18.097656, 58.847656, 15.992188, 53.867188);
            cr.ClosePath();
            cr.MoveTo(15.992188, 53.867188);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(-2.783042, 2.193103, 1.380399, 3.263361, -186.846715, -379.625084);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }




        #endregion


        #region Creative Toolbar
        public void Drawundo_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 14;
            float h = 9;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(13.40625, 8.871094);
            cr.CurveTo(13.40625, 8.871094, 15.296875, 2.445313, 10.375, 0.683594);
            cr.CurveTo(7.996094, -0.171875, 4.972656, 2.125, 2.800781, 4.265625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 7.433594);
            cr.CurveTo(0.785156, 5.460938, 1.503906, 2.789063, 1.539063, 0.761719);
            cr.LineTo(3.203125, 3.886719);
            cr.LineTo(6.480469, 5.222656);
            cr.CurveTo(4.464844, 5.464844, 1.878906, 6.449219, 0, 7.433594);
            cr.ClosePath();
            cr.MoveTo(0, 7.433594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void Drawcursor_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 24;
            float h = 24;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4, 0);
            cr.LineTo(20, 12.277344);
            cr.LineTo(13.050781, 13.449219);
            cr.LineTo(17.375, 22.265625);
            cr.LineTo(13.777344, 24);
            cr.LineTo(9.429688, 15.121094);
            cr.LineTo(4, 19.824219);
            cr.ClosePath();
            cr.MoveTo(4, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawrepeat_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 11;
            float h = 8;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.566406, 1.558594);
            cr.LineTo(3.578125, 1.558594);
            cr.LineTo(3.578125, 6.441406);
            cr.LineTo(0.566406, 6.441406);
            cr.ClosePath();
            cr.MoveTo(0.566406, 1.558594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.566406, 1.558594);
            cr.LineTo(3.578125, 1.558594);
            cr.LineTo(3.578125, 6.441406);
            cr.LineTo(0.566406, 6.441406);
            cr.ClosePath();
            cr.MoveTo(0.566406, 1.558594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1.038961, 0, 0, 1.038961, 0.0454545, 0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(5.550781, 0);
            cr.LineTo(5.550781, 8);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1.038961, 0, 0, 1.038961, 0.0454545, 0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(7.421875, 1.558594);
            cr.LineTo(10.433594, 1.558594);
            cr.LineTo(10.433594, 6.441406);
            cr.LineTo(7.421875, 6.441406);
            cr.ClosePath();
            cr.MoveTo(7.421875, 1.558594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(7.421875, 1.558594);
            cr.LineTo(10.433594, 1.558594);
            cr.LineTo(10.433594, 6.441406);
            cr.LineTo(7.421875, 6.441406);
            cr.ClosePath();
            cr.MoveTo(7.421875, 1.558594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1.038961, 0, 0, 1.038961, 0.0454545, 0);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawselect_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 151;
            float h = 151;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(31.699219, 0);
            cr.LineTo(51, 0);
            cr.LineTo(51, 8.800781);
            cr.LineTo(31.699219, 8.800781);
            cr.ClosePath();
            cr.MoveTo(31.699219, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(31.699219, 0);
            cr.LineTo(51, 0);
            cr.LineTo(51, 8.800781);
            cr.LineTo(31.699219, 8.800781);
            cr.ClosePath();
            cr.MoveTo(31.699219, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(65, 0);
            cr.LineTo(88.300781, 0);
            cr.LineTo(88.300781, 8.800781);
            cr.LineTo(65, 8.800781);
            cr.ClosePath();
            cr.MoveTo(65, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(65, 0);
            cr.LineTo(88.300781, 0);
            cr.LineTo(88.300781, 8.800781);
            cr.LineTo(65, 8.800781);
            cr.ClosePath();
            cr.MoveTo(65, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(101.5, 0);
            cr.LineTo(120.699219, 0);
            cr.LineTo(120.699219, 8.800781);
            cr.LineTo(101.5, 8.800781);
            cr.ClosePath();
            cr.MoveTo(101.5, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(101.5, 0);
            cr.LineTo(120.699219, 0);
            cr.LineTo(120.699219, 8.800781);
            cr.LineTo(101.5, 8.800781);
            cr.ClosePath();
            cr.MoveTo(101.5, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132, 0);
            cr.LineTo(151, 0);
            cr.LineTo(151, 8.800781);
            cr.LineTo(132, 8.800781);
            cr.ClosePath();
            cr.MoveTo(132, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132, 0);
            cr.LineTo(151, 0);
            cr.LineTo(151, 8.800781);
            cr.LineTo(132, 8.800781);
            cr.ClosePath();
            cr.MoveTo(132, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 6.300781);
            cr.LineTo(151, 6.300781);
            cr.LineTo(151, 19.101563);
            cr.LineTo(141.5, 19.101563);
            cr.ClosePath();
            cr.MoveTo(141.5, 6.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 6.300781);
            cr.LineTo(151, 6.300781);
            cr.LineTo(151, 19.101563);
            cr.LineTo(141.5, 19.101563);
            cr.ClosePath();
            cr.MoveTo(141.5, 6.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 32);
            cr.LineTo(151, 32);
            cr.LineTo(151, 51.5);
            cr.LineTo(141.5, 51.5);
            cr.ClosePath();
            cr.MoveTo(141.5, 32);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 32);
            cr.LineTo(151, 32);
            cr.LineTo(151, 51.5);
            cr.LineTo(141.5, 51.5);
            cr.ClosePath();
            cr.MoveTo(141.5, 32);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 66.300781);
            cr.LineTo(151, 66.300781);
            cr.LineTo(151, 89.601563);
            cr.LineTo(141.5, 89.601563);
            cr.ClosePath();
            cr.MoveTo(141.5, 66.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 66.300781);
            cr.LineTo(151, 66.300781);
            cr.LineTo(151, 89.601563);
            cr.LineTo(141.5, 89.601563);
            cr.ClosePath();
            cr.MoveTo(141.5, 66.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 101.800781);
            cr.LineTo(151, 101.800781);
            cr.LineTo(151, 120.699219);
            cr.LineTo(141.5, 120.699219);
            cr.ClosePath();
            cr.MoveTo(141.5, 101.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 101.800781);
            cr.LineTo(151, 101.800781);
            cr.LineTo(151, 120.699219);
            cr.LineTo(141.5, 120.699219);
            cr.ClosePath();
            cr.MoveTo(141.5, 101.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 132);
            cr.LineTo(151, 132);
            cr.LineTo(151, 151);
            cr.LineTo(141.5, 151);
            cr.ClosePath();
            cr.MoveTo(141.5, 132);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(141.5, 132);
            cr.LineTo(151, 132);
            cr.LineTo(151, 151);
            cr.LineTo(141.5, 151);
            cr.ClosePath();
            cr.MoveTo(141.5, 132);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132, 141.5);
            cr.LineTo(146.300781, 141.5);
            cr.LineTo(146.300781, 151);
            cr.LineTo(132, 151);
            cr.ClosePath();
            cr.MoveTo(132, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(132, 141.5);
            cr.LineTo(146.300781, 141.5);
            cr.LineTo(146.300781, 151);
            cr.LineTo(132, 151);
            cr.ClosePath();
            cr.MoveTo(132, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(99.5, 141.5);
            cr.LineTo(119, 141.5);
            cr.LineTo(119, 151);
            cr.LineTo(99.5, 151);
            cr.ClosePath();
            cr.MoveTo(99.5, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(99.5, 141.5);
            cr.LineTo(119, 141.5);
            cr.LineTo(119, 151);
            cr.LineTo(99.5, 151);
            cr.ClosePath();
            cr.MoveTo(99.5, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(63, 141.5);
            cr.LineTo(86.5, 141.5);
            cr.LineTo(86.5, 151);
            cr.LineTo(63, 151);
            cr.ClosePath();
            cr.MoveTo(63, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(63, 141.5);
            cr.LineTo(86.5, 141.5);
            cr.LineTo(86.5, 151);
            cr.LineTo(63, 151);
            cr.ClosePath();
            cr.MoveTo(63, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.300781, 141.5);
            cr.LineTo(49.300781, 141.5);
            cr.LineTo(49.300781, 151);
            cr.LineTo(30.300781, 151);
            cr.ClosePath();
            cr.MoveTo(30.300781, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.300781, 141.5);
            cr.LineTo(49.300781, 141.5);
            cr.LineTo(49.300781, 151);
            cr.LineTo(30.300781, 151);
            cr.ClosePath();
            cr.MoveTo(30.300781, 141.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 132);
            cr.LineTo(8.800781, 132);
            cr.LineTo(8.800781, 151);
            cr.LineTo(0, 151);
            cr.ClosePath();
            cr.MoveTo(0, 132);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 132);
            cr.LineTo(8.800781, 132);
            cr.LineTo(8.800781, 151);
            cr.LineTo(0, 151);
            cr.ClosePath();
            cr.MoveTo(0, 132);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(8.699219, 142.5);
            cr.LineTo(19, 142.5);
            cr.LineTo(19, 151);
            cr.LineTo(8.699219, 151);
            cr.ClosePath();
            cr.MoveTo(8.699219, 142.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(8.699219, 142.5);
            cr.LineTo(19, 142.5);
            cr.LineTo(19, 151);
            cr.LineTo(8.699219, 151);
            cr.ClosePath();
            cr.MoveTo(8.699219, 142.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 99.5);
            cr.LineTo(8.800781, 99.5);
            cr.LineTo(8.800781, 119.300781);
            cr.LineTo(0, 119.300781);
            cr.ClosePath();
            cr.MoveTo(0, 99.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 99.5);
            cr.LineTo(8.800781, 99.5);
            cr.LineTo(8.800781, 119.300781);
            cr.LineTo(0, 119.300781);
            cr.ClosePath();
            cr.MoveTo(0, 99.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 62.800781);
            cr.LineTo(8.800781, 62.800781);
            cr.LineTo(8.800781, 85.300781);
            cr.LineTo(0, 85.300781);
            cr.ClosePath();
            cr.MoveTo(0, 62.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 62.800781);
            cr.LineTo(8.800781, 62.800781);
            cr.LineTo(8.800781, 85.300781);
            cr.LineTo(0, 85.300781);
            cr.ClosePath();
            cr.MoveTo(0, 62.800781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 30.300781);
            cr.LineTo(8.800781, 30.300781);
            cr.LineTo(8.800781, 49.800781);
            cr.LineTo(0, 49.800781);
            cr.ClosePath();
            cr.MoveTo(0, 30.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 30.300781);
            cr.LineTo(8.800781, 30.300781);
            cr.LineTo(8.800781, 49.800781);
            cr.LineTo(0, 49.800781);
            cr.ClosePath();
            cr.MoveTo(0, 30.300781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 0);
            cr.LineTo(8.800781, 0);
            cr.LineTo(8.800781, 19);
            cr.LineTo(0, 19);
            cr.ClosePath();
            cr.MoveTo(0, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 0);
            cr.LineTo(8.800781, 0);
            cr.LineTo(8.800781, 19);
            cr.LineTo(0, 19);
            cr.ClosePath();
            cr.MoveTo(0, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.699219, 0);
            cr.LineTo(19, 0);
            cr.LineTo(19, 8.800781);
            cr.LineTo(1.699219, 8.800781);
            cr.ClosePath();
            cr.MoveTo(1.699219, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(1.699219, 0);
            cr.LineTo(19, 0);
            cr.LineTo(19, 8.800781);
            cr.LineTo(1.699219, 8.800781);
            cr.ClosePath();
            cr.MoveTo(1.699219, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, 230, -321);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawredo_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 14;
            float h = 9;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 1;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0.855469, 8.871094);
            cr.CurveTo(0.855469, 8.871094, -1.03125, 2.445313, 3.890625, 0.683594);
            cr.CurveTo(6.269531, -0.171875, 9.292969, 2.125, 11.460938, 4.265625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(14.265625, 7.433594);
            cr.CurveTo(12.386719, 6.449219, 9.796875, 5.464844, 7.785156, 5.222656);
            cr.LineTo(11.0625, 3.886719);
            cr.LineTo(12.726563, 0.761719);
            cr.CurveTo(12.761719, 2.789063, 13.480469, 5.464844, 14.265625, 7.433594);
            cr.ClosePath();
            cr.MoveTo(14.265625, 7.433594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }




        public void Drawairbrush_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 48;
            float h = 105;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 5;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(5.5, 30.589844);
            cr.CurveTo(3.851563, 30.574219, 2.5, 31.910156, 2.5, 33.5625);
            cr.LineTo(2.5, 99.894531);
            cr.CurveTo(2.5, 101.546875, 3.851563, 102.894531, 5.5, 102.894531);
            cr.LineTo(35.832031, 102.894531);
            cr.CurveTo(37.484375, 102.894531, 38.832031, 101.546875, 38.832031, 99.894531);
            cr.LineTo(38.832031, 33.5625);
            cr.CurveTo(38.832031, 31.910156, 37.484375, 30.5625, 35.832031, 30.5625);
            cr.LineTo(32.5, 30.5625);
            cr.CurveTo(30.851563, 30.5625, 29.507813, 29.210938, 29.515625, 27.5625);
            cr.LineTo(29.527344, 24.894531);
            cr.CurveTo(29.535156, 23.246094, 28.191406, 21.894531, 26.539063, 21.894531);
            cr.LineTo(14.5, 21.894531);
            cr.CurveTo(12.851563, 21.894531, 11.5, 23.246094, 11.5, 24.894531);
            cr.LineTo(11.5, 27.644531);
            cr.CurveTo(11.5, 29.296875, 10.148438, 30.632813, 8.5, 30.617188);
            cr.ClosePath();
            cr.MoveTo(5.5, 30.589844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -29.541, -5.355);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 3;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(20.457031, 21.894531);
            cr.LineTo(20.457031, 14.351563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -29.541, -5.355);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(17.46875, 11.164063);
            cr.LineTo(23.445313, 11.164063);
            cr.LineTo(23.445313, 14.351563);
            cr.LineTo(17.46875, 14.351563);
            cr.ClosePath();
            cr.MoveTo(17.46875, 11.164063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(25.585938, 9.789063);
            cr.LineTo(28.335938, 9.789063);
            cr.LineTo(28.335938, 12.539063);
            cr.LineTo(25.585938, 12.539063);
            cr.ClosePath();
            cr.MoveTo(25.585938, 9.789063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(25.585938, 13.789063);
            cr.LineTo(28.335938, 13.789063);
            cr.LineTo(28.335938, 16.539063);
            cr.LineTo(25.585938, 16.539063);
            cr.ClosePath();
            cr.MoveTo(25.585938, 13.789063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.667969, 6.414063);
            cr.LineTo(33.417969, 6.414063);
            cr.LineTo(33.417969, 9.164063);
            cr.LineTo(30.667969, 9.164063);
            cr.ClosePath();
            cr.MoveTo(30.667969, 6.414063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.667969, 10.414063);
            cr.LineTo(33.417969, 10.414063);
            cr.LineTo(33.417969, 13.164063);
            cr.LineTo(30.667969, 13.164063);
            cr.ClosePath();
            cr.MoveTo(30.667969, 10.414063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(35.75, 4.414063);
            cr.LineTo(38.5, 4.414063);
            cr.LineTo(38.5, 7.164063);
            cr.LineTo(35.75, 7.164063);
            cr.ClosePath();
            cr.MoveTo(35.75, 4.414063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(35.75, 8.414063);
            cr.LineTo(38.5, 8.414063);
            cr.LineTo(38.5, 11.164063);
            cr.LineTo(35.75, 11.164063);
            cr.ClosePath();
            cr.MoveTo(35.75, 8.414063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(35.75, 12.5);
            cr.LineTo(38.5, 12.5);
            cr.LineTo(38.5, 15.25);
            cr.LineTo(35.75, 15.25);
            cr.ClosePath();
            cr.MoveTo(35.75, 12.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(35.75, 16.5);
            cr.LineTo(38.5, 16.5);
            cr.LineTo(38.5, 19.25);
            cr.LineTo(35.75, 19.25);
            cr.ClosePath();
            cr.MoveTo(35.75, 16.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(40.832031, 2.019531);
            cr.LineTo(43.582031, 2.019531);
            cr.LineTo(43.582031, 4.769531);
            cr.LineTo(40.832031, 4.769531);
            cr.ClosePath();
            cr.MoveTo(40.832031, 2.019531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(40.832031, 6.375);
            cr.LineTo(43.582031, 6.375);
            cr.LineTo(43.582031, 9.125);
            cr.LineTo(40.832031, 9.125);
            cr.ClosePath();
            cr.MoveTo(40.832031, 6.375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(40.832031, 10.457031);
            cr.LineTo(43.582031, 10.457031);
            cr.LineTo(43.582031, 13.207031);
            cr.LineTo(40.832031, 13.207031);
            cr.ClosePath();
            cr.MoveTo(40.832031, 10.457031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(40.832031, 14.457031);
            cr.LineTo(43.582031, 14.457031);
            cr.LineTo(43.582031, 17.207031);
            cr.LineTo(40.832031, 17.207031);
            cr.ClosePath();
            cr.MoveTo(40.832031, 14.457031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(45.667969, 0);
            cr.LineTo(48.417969, 0);
            cr.LineTo(48.417969, 2.75);
            cr.LineTo(45.667969, 2.75);
            cr.ClosePath();
            cr.MoveTo(45.667969, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(45.667969, 4);
            cr.LineTo(48.417969, 4);
            cr.LineTo(48.417969, 6.75);
            cr.LineTo(45.667969, 6.75);
            cr.ClosePath();
            cr.MoveTo(45.667969, 4);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(45.667969, 8.082031);
            cr.LineTo(48.417969, 8.082031);
            cr.LineTo(48.417969, 10.832031);
            cr.LineTo(45.667969, 10.832031);
            cr.ClosePath();
            cr.MoveTo(45.667969, 8.082031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(45.667969, 12.082031);
            cr.LineTo(48.417969, 12.082031);
            cr.LineTo(48.417969, 14.832031);
            cr.LineTo(45.667969, 14.832031);
            cr.ClosePath();
            cr.MoveTo(45.667969, 12.082031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.667969, 14.875);
            cr.LineTo(33.417969, 14.875);
            cr.LineTo(33.417969, 17.625);
            cr.LineTo(30.667969, 17.625);
            cr.ClosePath();
            cr.MoveTo(30.667969, 14.875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(40.832031, 18.625);
            cr.LineTo(43.582031, 18.625);
            cr.LineTo(43.582031, 21.375);
            cr.LineTo(40.832031, 21.375);
            cr.ClosePath();
            cr.MoveTo(40.832031, 18.625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(45.667969, 16.457031);
            cr.LineTo(48.417969, 16.457031);
            cr.LineTo(48.417969, 19.207031);
            cr.LineTo(45.667969, 19.207031);
            cr.ClosePath();
            cr.MoveTo(45.667969, 16.457031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(45.585938, 20.789063);
            cr.LineTo(48.335938, 20.789063);
            cr.LineTo(48.335938, 23.539063);
            cr.LineTo(45.585938, 23.539063);
            cr.ClosePath();
            cr.MoveTo(45.585938, 20.789063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(31.75, 55.144531);
            cr.CurveTo(31.75, 56.796875, 30.398438, 58.144531, 28.75, 58.144531);
            cr.LineTo(11.75, 58.144531);
            cr.CurveTo(10.101563, 58.144531, 8.75, 56.796875, 8.75, 55.144531);
            cr.LineTo(8.75, 44.789063);
            cr.CurveTo(8.75, 43.140625, 10.101563, 41.789063, 11.75, 41.789063);
            cr.LineTo(28.75, 41.789063);
            cr.CurveTo(30.398438, 41.789063, 31.75, 43.140625, 31.75, 44.789063);
            cr.ClosePath();
            cr.MoveTo(31.75, 55.144531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void Drawbrush_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 512;
            float h = 512;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(37.367188, 467.015625);
            cr.CurveTo(81.277344, 466.667969, 86.75, 456.09375, 101.058594, 412.011719);
            cr.CurveTo(127.453125, 342.445313, 246.34375, 430.214844, 172.667969, 460.699219);
            cr.CurveTo(98.996094, 491.179688, -6.542969, 467.359375, 37.367188, 467.015625);
            cr.ClosePath();
            cr.MoveTo(37.367188, 467.015625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(458.855469, 46.902344);
            cr.CurveTo(439.441406, 31.253906, 407.265625, 38.964844, 386.992188, 64.121094);
            cr.LineTo(181.644531, 355.851563);
            cr.CurveTo(161.371094, 381.003906, 161.472656, 376.195313, 180.890625, 391.839844);
            cr.CurveTo(200.308594, 407.488281, 195.628906, 408.613281, 215.902344, 383.453125);
            cr.LineTo(457.308594, 120.785156);
            cr.CurveTo(477.582031, 95.628906, 478.273438, 62.550781, 458.855469, 46.902344);
            cr.ClosePath();
            cr.MoveTo(406.066406, 81.824219);
            cr.CurveTo(406.066406, 81.824219, 403.066406, 78.324219, 393.066406, 70.324219);
            cr.CurveTo(408.066406, 45.824219, 437.566406, 50.324219, 437.566406, 50.324219);
            cr.CurveTo(409.566406, 66.824219, 406.066406, 81.824219, 406.066406, 81.824219);
            cr.ClosePath();
            cr.MoveTo(406.066406, 81.824219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Draweraser_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 118;
            float h = 102;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 5;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(61.082031, 83.007813);
            cr.CurveTo(62.734375, 83.007813, 65.058594, 82.074219, 66.25, 80.929688);
            cr.LineTo(79.65625, 68.082031);
            cr.CurveTo(80.847656, 66.9375, 82.804688, 65.078125, 84.007813, 63.949219);
            cr.LineTo(97.136719, 51.605469);
            cr.CurveTo(98.339844, 50.472656, 100.304688, 48.625, 101.507813, 47.492188);
            cr.LineTo(114.636719, 35.148438);
            cr.CurveTo(115.839844, 34.019531, 115.867188, 32.140625, 114.703125, 30.972656);
            cr.LineTo(110.988281, 27.261719);
            cr.CurveTo(109.824219, 26.09375, 107.914063, 24.183594, 106.746094, 23.019531);
            cr.LineTo(103.035156, 19.304688);
            cr.CurveTo(101.867188, 18.140625, 99.957031, 16.230469, 98.792969, 15.0625);
            cr.LineTo(95.078125, 11.351563);
            cr.CurveTo(93.910156, 10.183594, 92.003906, 8.273438, 90.835938, 7.109375);
            cr.LineTo(87.125, 3.394531);
            cr.CurveTo(85.957031, 2.230469, 84.019531, 2.199219, 82.816406, 3.328125);
            cr.LineTo(69.6875, 15.675781);
            cr.CurveTo(68.484375, 16.804688, 66.519531, 18.652344, 65.316406, 19.785156);
            cr.LineTo(52.1875, 32.128906);
            cr.CurveTo(50.984375, 33.257813, 49.019531, 35.109375, 47.816406, 36.238281);
            cr.LineTo(34.6875, 48.585938);
            cr.CurveTo(33.484375, 49.714844, 31.515625, 51.5625, 30.316406, 52.695313);
            cr.LineTo(17.183594, 65.039063);
            cr.CurveTo(15.984375, 66.171875, 15.953125, 68.050781, 17.121094, 69.214844);
            cr.LineTo(20.832031, 72.929688);
            cr.CurveTo(22, 74.097656, 23.9375, 75.976563, 25.140625, 77.105469);
            cr.LineTo(29.230469, 80.953125);
            cr.CurveTo(30.433594, 82.082031, 32.765625, 83.007813, 34.414063, 83.007813);
            cr.ClosePath();
            cr.MoveTo(61.082031, 83.007813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -10.167, -7.743);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(54.402344, 30.050781);
            cr.CurveTo(53.199219, 31.179688, 53.199219, 33.027344, 54.402344, 34.15625);
            cr.LineTo(82.898438, 60.882813);
            cr.CurveTo(84.101563, 62.011719, 86.070313, 62.007813, 87.273438, 60.878906);
            cr.LineTo(114.636719, 35.148438);
            cr.CurveTo(115.839844, 34.019531, 115.867188, 32.140625, 114.699219, 30.972656);
            cr.LineTo(87.125, 3.394531);
            cr.CurveTo(85.957031, 2.226563, 84.019531, 2.199219, 82.816406, 3.328125);
            cr.ClosePath();
            cr.MoveTo(54.402344, 30.050781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(19.332031, 81.882813);
            cr.CurveTo(19.332031, 82.867188, 18.433594, 83.675781, 17.332031, 83.675781);
            cr.LineTo(2, 83.675781);
            cr.CurveTo(0.898438, 83.675781, 0, 82.867188, 0, 81.882813);
            cr.CurveTo(0, 80.898438, 0.898438, 80.089844, 2, 80.089844);
            cr.LineTo(17.332031, 80.089844);
            cr.CurveTo(18.433594, 80.089844, 19.332031, 80.898438, 19.332031, 81.882813);
            cr.ClosePath();
            cr.MoveTo(19.332031, 81.882813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(68.332031, 91.964844);
            cr.CurveTo(68.332031, 93.042969, 67.433594, 93.921875, 66.332031, 93.921875);
            cr.LineTo(13.5, 93.921875);
            cr.CurveTo(12.398438, 93.921875, 11.5, 93.042969, 11.5, 91.964844);
            cr.CurveTo(11.5, 90.886719, 12.398438, 90.007813, 13.5, 90.007813);
            cr.LineTo(66.332031, 90.007813);
            cr.CurveTo(67.433594, 90.007813, 68.332031, 90.886719, 68.332031, 91.964844);
            cr.ClosePath();
            cr.MoveTo(68.332031, 91.964844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(39.914063, 99.925781);
            cr.CurveTo(39.914063, 101.023438, 39.015625, 101.925781, 37.914063, 101.925781);
            cr.LineTo(7.164063, 101.925781);
            cr.CurveTo(6.066406, 101.925781, 5.164063, 101.023438, 5.164063, 99.925781);
            cr.LineTo(5.164063, 99.839844);
            cr.CurveTo(5.164063, 98.742188, 6.066406, 97.839844, 7.164063, 97.839844);
            cr.LineTo(37.914063, 97.839844);
            cr.CurveTo(39.015625, 97.839844, 39.914063, 98.742188, 39.914063, 99.839844);
            cr.ClosePath();
            cr.MoveTo(39.914063, 99.925781);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void Drawerode_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 92;
            float h = 82;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 5;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(2.5, 16.53125);
            cr.LineTo(2.5, 79.53125);
            cr.LineTo(65.5, 79.53125);
            cr.LineTo(65.5, 61.53125);
            cr.LineTo(56.5, 61.53125);
            cr.LineTo(56.582031, 52.570313);
            cr.LineTo(38.5, 52.53125);
            cr.LineTo(38.5, 34.53125);
            cr.LineTo(20.5, 34.53125);
            cr.LineTo(20.5, 16.53125);
            cr.ClosePath();
            cr.MoveTo(2.5, 16.53125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -25, -10.97);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(37.414063, 0);
            cr.LineTo(47.113281, 5.558594);
            cr.LineTo(41.554688, 15.253906);
            cr.LineTo(31.855469, 9.699219);
            cr.ClosePath();
            cr.MoveTo(37.414063, 0);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(52.515625, 11.632813);
            cr.LineTo(66.203125, 14.175781);
            cr.LineTo(63.664063, 27.863281);
            cr.LineTo(49.976563, 25.320313);
            cr.ClosePath();
            cr.MoveTo(52.515625, 11.632813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(68.050781, 31.734375);
            cr.LineTo(79.046875, 27.585938);
            cr.LineTo(83.191406, 38.578125);
            cr.LineTo(72.199219, 42.726563);
            cr.ClosePath();
            cr.MoveTo(68.050781, 31.734375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.945313, 17.234375);
            cr.LineTo(92, 70.03125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 4;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(30.945313, 17.234375);
            cr.LineTo(82.871094, 62.132813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -25, -10.97);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(92, 70.03125);
            cr.CurveTo(86.324219, 67.910156, 78.644531, 66.019531, 72.847656, 66.042969);
            cr.LineTo(81.671875, 61.097656);
            cr.LineTo(85.289063, 51.65625);
            cr.CurveTo(86.101563, 57.394531, 89.082031, 64.722656, 92, 70.03125);
            cr.ClosePath();
            cr.MoveTo(92, 70.03125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawfloodfill_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 131;
            float h = 132;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(58.710938, 130.265625);
            cr.CurveTo(56.765625, 132.210938, 53.585938, 132.210938, 51.640625, 130.265625);
            cr.LineTo(1.460938, 80.085938);
            cr.CurveTo(-0.484375, 78.140625, -0.484375, 74.957031, 1.460938, 73.011719);
            cr.LineTo(63.738281, 10.734375);
            cr.CurveTo(65.683594, 8.789063, 68.863281, 8.789063, 70.808594, 10.734375);
            cr.LineTo(120.988281, 60.914063);
            cr.CurveTo(122.933594, 62.859375, 122.933594, 66.039063, 120.988281, 67.984375);
            cr.ClosePath();
            cr.MoveTo(58.710938, 130.265625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(67.527344, 6.921875);
            cr.LineTo(124.800781, 64.195313);
            cr.LineTo(119.5, 69.5);
            cr.LineTo(62.222656, 12.222656);
            cr.ClosePath();
            cr.MoveTo(67.527344, 6.921875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125.316406, 70.5);
            cr.LineTo(125.316406, 119.25);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(125.816406, 70.5);
            cr.CurveTo(126.605469, 74.5625, 127.203125, 78.625, 127.816406, 82.6875);
            cr.LineTo(128.640625, 88.78125);
            cr.LineTo(129.316406, 94.875);
            cr.CurveTo(129.519531, 96.90625, 129.941406, 98.9375, 130.265625, 100.96875);
            cr.CurveTo(130.585938, 103, 130.613281, 105.03125, 130.316406, 107.0625);
            cr.CurveTo(130.023438, 109.09375, 129.542969, 111.128906, 128.847656, 113.15625);
            cr.CurveTo(128.132813, 115.1875, 127.246094, 117.222656, 125.816406, 119.253906);
            cr.LineTo(124.816406, 119.253906);
            cr.CurveTo(123.382813, 117.222656, 122.496094, 115.1875, 121.78125, 113.15625);
            cr.CurveTo(121.085938, 111.128906, 120.605469, 109.09375, 120.316406, 107.0625);
            cr.CurveTo(120.015625, 105.03125, 120.042969, 103, 120.363281, 100.96875);
            cr.CurveTo(120.6875, 98.9375, 121.113281, 96.90625, 121.316406, 94.875);
            cr.LineTo(121.988281, 88.78125);
            cr.LineTo(122.816406, 82.6875);
            cr.CurveTo(123.429688, 78.625, 124.023438, 74.5625, 124.816406, 70.5);
            cr.ClosePath();
            cr.MoveTo(125.816406, 70.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(31.691406, 5.9375);
            cr.CurveTo(31.691406, 9.21875, 29.03125, 11.875, 25.753906, 11.875);
            cr.CurveTo(22.472656, 11.875, 19.816406, 9.21875, 19.816406, 5.9375);
            cr.CurveTo(19.816406, 2.660156, 22.472656, 0, 25.753906, 0);
            cr.CurveTo(29.03125, 0, 31.691406, 2.660156, 31.691406, 5.9375);
            cr.ClosePath();
            cr.MoveTo(31.691406, 5.9375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(48.257813, 25.5);
            cr.LineTo(77, 54.246094);
            cr.CurveTo(78.167969, 55.410156, 78.363281, 57.121094, 77.441406, 58.046875);
            cr.CurveTo(76.519531, 58.96875, 74.808594, 58.769531, 73.640625, 57.601563);
            cr.LineTo(44.898438, 28.859375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 6;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(1, 1, 1, 1);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(48.257813, 25.5);
            cr.LineTo(77, 54.246094);
            cr.CurveTo(78.167969, 55.410156, 78.363281, 57.121094, 77.441406, 58.046875);
            cr.CurveTo(76.519531, 58.96875, 74.808594, 58.769531, 73.640625, 57.601563);
            cr.LineTo(44.898438, 28.859375);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -5.185, -13.25);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(75.53125, 56.078125);
            cr.CurveTo(74.605469, 57, 72.894531, 56.800781, 71.730469, 55.636719);
            cr.LineTo(25.542969, 9.449219);
            cr.CurveTo(24.375, 8.28125, 24.179688, 6.570313, 25.101563, 5.648438);
            cr.CurveTo(26.023438, 4.722656, 27.734375, 4.921875, 28.902344, 6.089844);
            cr.LineTo(75.089844, 52.277344);
            cr.CurveTo(76.253906, 53.441406, 76.453125, 55.152344, 75.53125, 56.078125);
            cr.ClosePath();
            cr.MoveTo(75.53125, 56.078125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void Drawgrowshrink_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 96;
            float h = 104;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(0, 38.515625);
            cr.LineTo(18, 38.515625);
            cr.LineTo(18, 56.515625);
            cr.LineTo(0, 56.515625);
            cr.ClosePath();
            cr.MoveTo(0, 38.515625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(51, 26.515625);
            cr.LineTo(96, 26.515625);
            cr.LineTo(96, 71.515625);
            cr.LineTo(51, 71.515625);
            cr.ClosePath();
            cr.MoveTo(51, 26.515625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 7;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(18.859375, 18.917969);
            cr.CurveTo(22.28125, 16.34375, 26.917969, 13.433594, 33.167969, 10.183594);
            cr.CurveTo(70.335938, -9.152344, 72.335938, 19.621094, 72.335938, 19.621094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -18.5, -6.985);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(10.335938, 33.515625);
            cr.CurveTo(17.078125, 28.375, 26.609375, 22.730469, 34.386719, 20.417969);
            cr.LineTo(20.558594, 17.363281);
            cr.LineTo(11.886719, 6.171875);
            cr.CurveTo(13.121094, 14.191406, 12.097656, 25.21875, 10.335938, 33.515625);
            cr.ClosePath();
            cr.MoveTo(10.335938, 33.515625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 7;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(61.070313, 89.789063);
            cr.CurveTo(57.769531, 91.515625, 53.597656, 93.421875, 48.332031, 95.515625);
            cr.CurveTo(9.402344, 110.996094, 9.601563, 66.0625, 9.601563, 66.0625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -18.5, -6.985);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(72.332031, 77.183594);
            cr.CurveTo(64.710938, 80.898438, 54.257813, 84.5625, 46.179688, 85.304688);
            cr.LineTo(59.136719, 91.011719);
            cr.LineTo(65.445313, 103.6875);
            cr.CurveTo(65.808594, 95.582031, 68.976563, 84.96875, 72.332031, 77.183594);
            cr.ClosePath();
            cr.MoveTo(72.332031, 77.183594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawimport_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 89;
            float h = 95;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 5;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(65.5, 56.5);
            cr.LineTo(65.5, 23.5);
            cr.CurveTo(65.5, 21.851563, 64.546875, 19.546875, 63.378906, 18.378906);
            cr.LineTo(49.621094, 4.621094);
            cr.CurveTo(48.453125, 3.453125, 46.148438, 2.5, 44.5, 2.5);
            cr.LineTo(5.5, 2.5);
            cr.CurveTo(3.851563, 2.5, 2.5, 3.851563, 2.5, 5.5);
            cr.LineTo(2.5, 89.5);
            cr.CurveTo(2.5, 91.148438, 3.851563, 92.5, 5.5, 92.5);
            cr.LineTo(62.5, 92.5);
            cr.CurveTo(64.148438, 92.5, 65.5, 91.148438, 65.5, 89.5);
            cr.LineTo(65.5, 74.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -25, -16);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(47.5, 17.5);
            cr.CurveTo(47.5, 19.148438, 48.851563, 20.5, 50.5, 20.5);
            cr.LineTo(62.5, 20.5);
            cr.CurveTo(64.148438, 20.5, 64.546875, 19.546875, 63.378906, 18.378906);
            cr.LineTo(49.621094, 4.621094);
            cr.CurveTo(48.453125, 3.453125, 47.5, 3.851563, 47.5, 5.5);
            cr.ClosePath();
            cr.MoveTo(47.5, 17.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 7;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(88.5, 65.5);
            cr.LineTo(51.398438, 65.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -25, -16);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(34.5, 65.5);
            cr.CurveTo(42.453125, 62.550781, 52.316406, 57.515625, 58.433594, 52.183594);
            cr.LineTo(53.617188, 65.5);
            cr.LineTo(58.433594, 78.8125);
            cr.CurveTo(52.316406, 73.480469, 42.453125, 68.449219, 34.5, 65.5);
            cr.ClosePath();
            cr.MoveTo(34.5, 65.5);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawline_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 74;
            float h = 74;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(70.878906, 73);
            cr.CurveTo(69.710938, 74.167969, 67.800781, 74.167969, 66.636719, 73);
            cr.LineTo(0.875, 7.238281);
            cr.CurveTo(-0.292969, 6.070313, -0.292969, 4.164063, 0.875, 2.996094);
            cr.LineTo(2.996094, 0.875);
            cr.CurveTo(4.164063, -0.289063, 6.074219, -0.289063, 7.238281, 0.875);
            cr.LineTo(73, 66.636719);
            cr.CurveTo(74.167969, 67.804688, 74.167969, 69.714844, 73, 70.878906);
            cr.ClosePath();
            cr.MoveTo(70.878906, 73);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        public void Drawraiselower_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 106;
            float h = 60;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 9;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(2.976563, 55);
            cr.CurveTo(15.3125, 55, 14.976563, 55, 23.3125, 55);
            cr.CurveTo(31.644531, 55, 41.144531, 4.667969, 52.976563, 4.5);
            cr.CurveTo(64.8125, 4.335938, 74.644531, 55, 81.3125, 55);
            cr.CurveTo(87.976563, 55, 101.3125, 55, 101.3125, 55);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -3.022, -6.869);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 5;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(14.3125, 39.167969);
            cr.LineTo(14.3125, 10.296875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -3.022, -6.869);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(28.625, 21.894531);
            cr.LineTo(26.0625, 24.28125);
            cr.LineTo(14.316406, 11.640625);
            cr.LineTo(2.5625, 24.28125);
            cr.LineTo(0, 21.894531);
            cr.LineTo(14.316406, 6.5);
            cr.ClosePath();
            cr.MoveTo(28.625, 21.894531);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 5;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(91.976563, 6.5);
            cr.LineTo(91.976563, 35.371094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -3.022, -6.869);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(77.664063, 23.773438);
            cr.LineTo(80.226563, 21.386719);
            cr.LineTo(91.976563, 34.027344);
            cr.LineTo(103.726563, 21.386719);
            cr.LineTo(106.289063, 23.773438);
            cr.LineTo(91.976563, 39.167969);
            cr.ClosePath();
            cr.MoveTo(77.664063, 23.773438);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawtree_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 305;
            float h = 295;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(230.652344, 282.183594);
            cr.CurveTo(230.652344, 299.933594, 65.875, 299.933594, 65.875, 282.183594);
            cr.CurveTo(65.875, 264.433594, 230.652344, 264.433594, 230.652344, 282.183594);
            cr.CurveTo(230.652344, 288.71875, 230.652344, 275.652344, 230.652344, 282.183594);
            cr.ClosePath();
            cr.MoveTo(230.652344, 282.183594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(21.265625, 146.5);
            cr.CurveTo(-5.78125, 147.738281, -7.691406, 105.898438, 19.355469, 104.660156);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(47.710938, 103.683594);
            cr.CurveTo(48.945313, 130.730469, 7.109375, 132.644531, 5.871094, 105.597656);
            cr.CurveTo(4.636719, 78.550781, 46.472656, 76.636719, 47.710938, 103.683594);
            cr.CurveTo(48.238281, 115.238281, 47.183594, 92.128906, 47.710938, 103.683594);
            cr.ClosePath();
            cr.MoveTo(47.710938, 103.683594);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(48.6875, 101.203125);
            cr.CurveTo(21.640625, 102.4375, 19.730469, 60.601563, 46.773438, 59.363281);
            cr.CurveTo(59.386719, 58.789063, 61.296875, 100.625, 48.6875, 101.203125);
            cr.CurveTo(37.136719, 101.730469, 60.238281, 100.675781, 48.6875, 101.203125);
            cr.ClosePath();
            cr.MoveTo(48.6875, 101.203125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(220.941406, 177.257813);
            cr.CurveTo(222.378906, 208.652344, 173.8125, 210.871094, 172.375, 179.476563);
            cr.CurveTo(170.941406, 148.078125, 219.507813, 145.855469, 220.941406, 177.257813);
            cr.CurveTo(221.554688, 190.667969, 220.328125, 163.84375, 220.941406, 177.257813);
            cr.ClosePath();
            cr.MoveTo(220.941406, 177.257813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(243.316406, 168.066406);
            cr.CurveTo(244.753906, 199.460938, 196.191406, 201.683594, 194.753906, 170.289063);
            cr.CurveTo(193.316406, 138.890625, 241.882813, 136.667969, 243.316406, 168.066406);
            cr.CurveTo(243.929688, 181.476563, 242.703125, 154.65625, 243.316406, 168.066406);
            cr.ClosePath();
            cr.MoveTo(243.316406, 168.066406);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(131.382813, 182.949219);
            cr.CurveTo(132.773438, 213.417969, 85.644531, 215.570313, 84.253906, 185.101563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(103.300781, 189.160156);
            cr.CurveTo(104.535156, 216.207031, 62.699219, 218.121094, 61.460938, 191.070313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(92.757813, 172.855469);
            cr.CurveTo(94.300781, 206.578125, 42.136719, 208.960938, 40.597656, 175.238281);
            cr.CurveTo(39.054688, 141.515625, 91.21875, 139.132813, 92.757813, 172.855469);
            cr.CurveTo(93.414063, 187.261719, 92.101563, 158.449219, 92.757813, 172.855469);
            cr.ClosePath();
            cr.MoveTo(92.757813, 172.855469);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(117.089844, 200.132813);
            cr.CurveTo(112.445313, 199.582031, 92.417969, 190.117188, 92.417969, 190.117188);
            cr.LineTo(92.714844, 193.960938);
            cr.CurveTo(100.015625, 194.109375, 109.546875, 202.789063, 117.089844, 200.132813);
            cr.CurveTo(112.445313, 199.582031, 109.546875, 202.789063, 117.089844, 200.132813);
            cr.ClosePath();
            cr.MoveTo(117.089844, 200.132813);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(100.535156, 165.839844);
            cr.CurveTo(98.722656, 164.628906, 92.71875, 164.046875, 90.710938, 165.683594);
            cr.CurveTo(97.425781, 174.339844, 124.175781, 196.335938, 134.488281, 199.730469);
            cr.CurveTo(130.492188, 195.660156, 104.09375, 168.210938, 100.535156, 165.839844);
            cr.CurveTo(98.722656, 164.628906, 104.09375, 168.210938, 100.535156, 165.839844);
            cr.ClosePath();
            cr.MoveTo(100.535156, 165.839844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(200.875, 165.527344);
            cr.CurveTo(195.300781, 160.796875, 171.339844, 186.871094, 168.167969, 191.253906);
            cr.CurveTo(168.871094, 192.1875, 190.503906, 173.355469, 200.875, 165.527344);
            cr.CurveTo(200.136719, 164.902344, 190.503906, 173.355469, 200.875, 165.527344);
            cr.ClosePath();
            cr.MoveTo(200.875, 165.527344);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(167.59375, 243.351563);
            cr.CurveTo(165.253906, 221.414063, 176.074219, 210.59375, 176.074219, 210.59375);
            cr.CurveTo(195.8125, 190, 220.917969, 174.925781, 249.496094, 173.152344);
            cr.LineTo(239.84375, 165.839844);
            cr.LineTo(238.964844, 165.839844);
            cr.CurveTo(215.1875, 172.660156, 197.972656, 188.539063, 175.34375, 196.476563);
            cr.CurveTo(155.769531, 213.996094, 155.421875, 260.695313, 149.605469, 282.152344);
            cr.CurveTo(161.636719, 282.160156, 175.808594, 284.78125, 187.484375, 282.183594);
            cr.CurveTo(187.484375, 282.183594, 169.933594, 265.292969, 167.59375, 243.351563);
            cr.CurveTo(165.253906, 221.414063, 169.933594, 265.292969, 167.59375, 243.351563);
            cr.ClosePath();
            cr.MoveTo(167.59375, 243.351563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(82.382813, 191.496094);
            cr.LineTo(82.605469, 194.375);
            cr.CurveTo(82.605469, 194.375, 115.417969, 205.308594, 122.296875, 211.132813);
            cr.CurveTo(115.621094, 202.613281, 102.808594, 200.335938, 82.382813, 191.496094);
            cr.CurveTo(82.382813, 191.496094, 102.808594, 200.335938, 82.382813, 191.496094);
            cr.ClosePath();
            cr.MoveTo(82.382813, 191.496094);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(89.589844, 184.660156);
            cr.CurveTo(90.828125, 211.710938, 48.988281, 213.621094, 47.753906, 186.574219);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(29.523438, 153.667969);
            cr.CurveTo(2.476563, 154.90625, 0.5625, 113.066406, 27.609375, 111.832031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(72.421875, 38.109375);
            cr.CurveTo(71.007813, 7.265625, 118.71875, 5.085938, 120.125, 35.929688);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(163.363281, 29.957031);
            cr.CurveTo(164.949219, 64.546875, 111.441406, 66.992188, 109.863281, 32.402344);
            cr.CurveTo(108.28125, -2.1875, 161.78125, -4.632813, 163.363281, 29.957031);
            cr.CurveTo(164.039063, 44.730469, 162.6875, 15.183594, 163.363281, 29.957031);
            cr.ClosePath();
            cr.MoveTo(163.363281, 29.957031);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(196.636719, 36.0625);
            cr.CurveTo(197.871094, 63.109375, 156.035156, 65.019531, 154.800781, 37.972656);
            cr.CurveTo(153.5625, 10.925781, 195.398438, 9.015625, 196.636719, 36.0625);
            cr.CurveTo(197.164063, 47.617188, 196.109375, 24.507813, 196.636719, 36.0625);
            cr.ClosePath();
            cr.MoveTo(196.636719, 36.0625);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(248.203125, 41.085938);
            cr.CurveTo(250.035156, 81.214844, 187.964844, 84.054688, 186.128906, 43.921875);
            cr.CurveTo(184.296875, 3.792969, 246.367188, 0.957031, 248.203125, 41.085938);
            cr.CurveTo(248.988281, 58.226563, 247.417969, 23.945313, 248.203125, 41.085938);
            cr.ClosePath();
            cr.MoveTo(248.203125, 41.085938);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(67.957031, 161.660156);
            cr.CurveTo(69.789063, 201.789063, 7.71875, 204.628906, 5.882813, 164.496094);
            cr.CurveTo(4.046875, 124.363281, 66.121094, 121.527344, 67.957031, 161.660156);
            cr.CurveTo(68.742188, 178.800781, 67.171875, 144.515625, 67.957031, 161.660156);
            cr.ClosePath();
            cr.MoveTo(67.957031, 161.660156);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(176.074219, 210.59375);
            cr.CurveTo(162.578125, 235.402344, 166.925781, 261.84375, 187.484375, 282.183594);
            cr.CurveTo(158.914063, 282.089844, 130.347656, 281.96875, 101.777344, 281.96875);
            cr.CurveTo(154.269531, 256.492188, 134.300781, 200.855469, 83.875, 194.492188);
            cr.LineTo(136.03125, 199.453125);
            cr.LineTo(40.601563, 66.527344);
            cr.CurveTo(38.769531, 26.398438, 100.839844, 23.5625, 102.675781, 63.691406);
            cr.LineTo(113.171875, 38.761719);
            cr.CurveTo(110.921875, -10.535156, 187.167969, -14.019531, 189.425781, 35.277344);
            cr.LineTo(162.746094, 48.148438);
            cr.CurveTo(160.496094, -1.152344, 236.742188, -4.636719, 239, 44.660156);
            cr.LineTo(272.996094, 101.710938);
            cr.CurveTo(313.125, 99.878906, 315.964844, 161.953125, 275.835938, 163.785156);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(265.316406, 66.386719);
            cr.CurveTo(266.550781, 93.433594, 224.714844, 95.347656, 223.476563, 68.300781);
            cr.CurveTo(222.242188, 41.253906, 264.078125, 39.339844, 265.316406, 66.386719);
            cr.CurveTo(265.84375, 77.941406, 264.785156, 54.835938, 265.316406, 66.386719);
            cr.ClosePath();
            cr.MoveTo(265.316406, 66.386719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(261.355469, 69.949219);
            cr.CurveTo(288.402344, 68.710938, 290.316406, 110.550781, 263.265625, 111.785156);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(286.308594, 156.167969);
            cr.CurveTo(287.539063, 183.214844, 245.703125, 185.128906, 244.46875, 158.078125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(257.949219, 162.121094);
            cr.CurveTo(259.1875, 189.171875, 217.347656, 191.078125, 216.113281, 164.03125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(158.535156, 18.441406);
            cr.CurveTo(115.734375, 22.976563, 57.8125, 31.160156, 40.050781, 79.074219);
            cr.CurveTo(25.320313, 118.820313, 13.320313, 125.984375, 45.117188, 155.101563);
            cr.CurveTo(81.160156, 188.101563, 135.976563, 220.003906, 182.4375, 185.460938);
            cr.CurveTo(231.675781, 148.851563, 287.3125, 182.660156, 273.789063, 108.484375);
            cr.CurveTo(261.925781, 43.40625, 213.207031, 33.464844, 158.535156, 18.441406);
            cr.ClosePath();
            cr.MoveTo(158.535156, 18.441406);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(99.386719, 18.914063);
            cr.CurveTo(105.695313, 18.625, 111.589844, 20.566406, 116.3125, 24.039063);
            cr.CurveTo(99.703125, -1.554688, 64.566406, 21.65625, 74.054688, 45.765625);
            cr.CurveTo(73.871094, 31.503906, 85.03125, 19.570313, 99.386719, 18.914063);
            cr.CurveTo(105.695313, 18.625, 85.03125, 19.570313, 99.386719, 18.914063);
            cr.ClosePath();
            cr.MoveTo(99.386719, 18.914063);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(198.441406, 31.046875);
            cr.CurveTo(202.023438, 33.125, 204.589844, 36.292969, 205.960938, 39.882813);
            cr.CurveTo(205.878906, 20.175781, 177.984375, 21.492188, 174.925781, 36.867188);
            cr.CurveTo(179.890625, 28.933594, 190.285156, 26.308594, 198.441406, 31.046875);
            cr.CurveTo(202.023438, 33.125, 190.285156, 26.308594, 198.441406, 31.046875);
            cr.ClosePath();
            cr.MoveTo(198.441406, 31.046875);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(265.316406, 66.386719);
            cr.CurveTo(264.441406, 47.261719, 239.152344, 38.988281, 227.46875, 55.023438);
            cr.CurveTo(246.089844, 42.648438, 269.464844, 66.28125, 255.96875, 84.796875);
            cr.CurveTo(261.890625, 80.863281, 265.664063, 74.007813, 265.316406, 66.386719);
            cr.CurveTo(264.785156, 54.835938, 265.664063, 74.007813, 265.316406, 66.386719);
            cr.ClosePath();
            cr.MoveTo(265.316406, 66.386719);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }


        public void Drawnone_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 117;
            float h = 117;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            cr.LineWidth = 14;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(110.394531, 58.695313);
            cr.CurveTo(110.394531, 87.25, 87.25, 110.394531, 58.699219, 110.394531);
            cr.CurveTo(30.144531, 110.394531, 7, 87.25, 7, 58.695313);
            cr.CurveTo(7, 30.144531, 30.144531, 7, 58.699219, 7);
            cr.CurveTo(87.25, 7, 110.394531, 30.144531, 110.394531, 58.695313);
            cr.ClosePath();
            cr.MoveTo(110.394531, 58.695313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -3.073, -2.648);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            cr.LineWidth = 14;
            cr.MiterLimit = 10;
            cr.LineCap = LineCap.Butt;
            cr.LineJoin = LineJoin.Miter;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(22.410156, 95.515625);
            cr.LineTo(95.511719, 22.417969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            matrix = new Matrix(1, 0, 0, 1, -3.073, -2.648);
            pattern.Matrix = matrix;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }



        public void Drawlake_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 152;
            float h = 124;
            float scale = Math.Min(width / w, height / h);
            matrix.Translate(x + Math.Max(0, (width - w * scale) / 2), y + Math.Max(0, (height - h * scale) / 2));
            matrix.Scale(scale, scale);
            cr.Matrix = matrix;

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(146.953125, 34.613281);
            cr.CurveTo(141.585938, 34.613281, 136.488281, 32.273438, 132.972656, 28.195313);
            cr.CurveTo(132.027344, 27.101563, 130.652344, 26.46875, 129.207031, 26.46875);
            cr.CurveTo(127.761719, 26.46875, 126.386719, 27.101563, 125.441406, 28.195313);
            cr.CurveTo(121.925781, 32.273438, 116.828125, 34.613281, 111.460938, 34.613281);
            cr.CurveTo(106.089844, 34.613281, 100.996094, 32.273438, 97.476563, 28.195313);
            cr.CurveTo(96.535156, 27.101563, 95.15625, 26.46875, 93.710938, 26.46875);
            cr.CurveTo(92.265625, 26.46875, 90.890625, 27.101563, 89.945313, 28.195313);
            cr.CurveTo(86.429688, 32.273438, 81.335938, 34.613281, 75.964844, 34.613281);
            cr.CurveTo(70.597656, 34.613281, 65.5, 32.273438, 61.984375, 28.195313);
            cr.CurveTo(61.039063, 27.101563, 59.664063, 26.46875, 58.21875, 26.46875);
            cr.CurveTo(56.769531, 26.46875, 55.398438, 27.101563, 54.453125, 28.195313);
            cr.CurveTo(50.933594, 32.273438, 45.839844, 34.613281, 40.46875, 34.613281);
            cr.CurveTo(35.101563, 34.613281, 30.003906, 32.273438, 26.488281, 28.195313);
            cr.CurveTo(25.542969, 27.101563, 24.167969, 26.46875, 22.722656, 26.46875);
            cr.CurveTo(21.277344, 26.46875, 19.902344, 27.101563, 18.957031, 28.195313);
            cr.CurveTo(15.441406, 32.273438, 10.34375, 34.613281, 4.976563, 34.613281);
            cr.CurveTo(2.226563, 34.613281, 0.00390625, 36.839844, 0.00390625, 39.585938);
            cr.CurveTo(0.00390625, 42.332031, 2.230469, 44.558594, 4.976563, 44.558594);
            cr.CurveTo(10.757813, 44.558594, 16.335938, 42.792969, 21.015625, 39.585938);
            cr.CurveTo(21.597656, 39.1875, 22.167969, 38.769531, 22.722656, 38.324219);
            cr.CurveTo(23.277344, 38.769531, 23.847656, 39.1875, 24.429688, 39.585938);
            cr.CurveTo(29.109375, 42.792969, 34.6875, 44.558594, 40.46875, 44.558594);
            cr.CurveTo(46.253906, 44.558594, 51.828125, 42.792969, 56.511719, 39.585938);
            cr.CurveTo(57.09375, 39.1875, 57.664063, 38.769531, 58.21875, 38.324219);
            cr.CurveTo(58.773438, 38.769531, 59.34375, 39.1875, 59.925781, 39.585938);
            cr.CurveTo(64.605469, 42.792969, 70.179688, 44.558594, 75.964844, 44.558594);
            cr.CurveTo(81.75, 44.558594, 87.324219, 42.792969, 92.003906, 39.585938);
            cr.CurveTo(92.585938, 39.1875, 93.15625, 38.769531, 93.710938, 38.324219);
            cr.CurveTo(94.265625, 38.769531, 94.835938, 39.1875, 95.421875, 39.585938);
            cr.CurveTo(100.101563, 42.792969, 105.675781, 44.558594, 111.460938, 44.558594);
            cr.CurveTo(117.246094, 44.558594, 122.820313, 42.792969, 127.5, 39.585938);
            cr.CurveTo(128.082031, 39.1875, 128.652344, 38.769531, 129.207031, 38.324219);
            cr.CurveTo(129.761719, 38.769531, 130.332031, 39.1875, 130.914063, 39.585938);
            cr.CurveTo(135.597656, 42.792969, 141.171875, 44.558594, 146.953125, 44.558594);
            cr.CurveTo(149.699219, 44.558594, 151.929688, 42.332031, 151.929688, 39.585938);
            cr.CurveTo(151.929688, 36.839844, 149.699219, 34.613281, 146.953125, 34.613281);
            cr.ClosePath();
            cr.MoveTo(146.953125, 34.613281);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4.972656, 18.089844);
            cr.CurveTo(10.757813, 18.089844, 16.332031, 16.324219, 21.011719, 13.117188);
            cr.CurveTo(21.597656, 12.71875, 22.167969, 12.296875, 22.722656, 11.855469);
            cr.CurveTo(23.273438, 12.296875, 23.847656, 12.714844, 24.429688, 13.117188);
            cr.CurveTo(29.109375, 16.324219, 34.683594, 18.089844, 40.46875, 18.089844);
            cr.CurveTo(46.253906, 18.089844, 51.828125, 16.324219, 56.507813, 13.117188);
            cr.CurveTo(57.089844, 12.71875, 57.660156, 12.296875, 58.214844, 11.855469);
            cr.CurveTo(58.769531, 12.296875, 59.339844, 12.714844, 59.925781, 13.117188);
            cr.CurveTo(64.605469, 16.324219, 70.179688, 18.089844, 75.964844, 18.089844);
            cr.CurveTo(81.746094, 18.089844, 87.320313, 16.324219, 92.003906, 13.117188);
            cr.CurveTo(92.585938, 12.71875, 93.15625, 12.296875, 93.710938, 11.855469);
            cr.CurveTo(94.261719, 12.296875, 94.835938, 12.714844, 95.417969, 13.117188);
            cr.CurveTo(100.097656, 16.324219, 105.671875, 18.089844, 111.457031, 18.089844);
            cr.CurveTo(117.242188, 18.089844, 122.816406, 16.324219, 127.496094, 13.117188);
            cr.CurveTo(128.078125, 12.71875, 128.652344, 12.296875, 129.203125, 11.855469);
            cr.CurveTo(129.757813, 12.296875, 130.328125, 12.714844, 130.914063, 13.117188);
            cr.CurveTo(135.59375, 16.324219, 141.167969, 18.089844, 146.953125, 18.089844);
            cr.CurveTo(149.699219, 18.089844, 151.925781, 15.863281, 151.925781, 13.117188);
            cr.CurveTo(151.925781, 10.367188, 149.699219, 8.144531, 146.953125, 8.144531);
            cr.CurveTo(141.585938, 8.144531, 136.488281, 5.804688, 132.972656, 1.726563);
            cr.CurveTo(132.027344, 0.632813, 130.652344, 0, 129.203125, 0);
            cr.CurveTo(127.757813, 0, 126.382813, 0.628906, 125.4375, 1.726563);
            cr.CurveTo(121.921875, 5.804688, 116.828125, 8.144531, 111.457031, 8.144531);
            cr.CurveTo(106.085938, 8.144531, 100.992188, 5.804688, 97.476563, 1.726563);
            cr.CurveTo(96.53125, 0.632813, 95.15625, 0, 93.710938, 0);
            cr.CurveTo(92.261719, 0, 90.886719, 0.628906, 89.941406, 1.726563);
            cr.CurveTo(86.425781, 5.804688, 81.332031, 8.144531, 75.960938, 8.144531);
            cr.CurveTo(70.59375, 8.144531, 65.5, 5.804688, 61.984375, 1.726563);
            cr.CurveTo(61.039063, 0.628906, 59.664063, 0, 58.21875, 0);
            cr.CurveTo(56.769531, 0, 55.394531, 0.628906, 54.449219, 1.726563);
            cr.CurveTo(50.933594, 5.804688, 45.839844, 8.144531, 40.46875, 8.144531);
            cr.CurveTo(35.101563, 8.144531, 30.003906, 5.804688, 26.488281, 1.726563);
            cr.CurveTo(25.542969, 0.628906, 24.167969, 0, 22.722656, 0);
            cr.CurveTo(21.273438, 0, 19.898438, 0.628906, 18.953125, 1.726563);
            cr.CurveTo(15.4375, 5.804688, 10.34375, 8.144531, 4.972656, 8.144531);
            cr.CurveTo(2.226563, 8.144531, 0, 10.371094, 0, 13.117188);
            cr.CurveTo(0, 15.863281, 2.226563, 18.089844, 4.972656, 18.089844);
            cr.ClosePath();
            cr.MoveTo(4.972656, 18.089844);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(146.953125, 113.945313);
            cr.CurveTo(141.585938, 113.945313, 136.488281, 111.605469, 132.972656, 107.527344);
            cr.CurveTo(132.027344, 106.433594, 130.652344, 105.804688, 129.207031, 105.804688);
            cr.CurveTo(127.761719, 105.804688, 126.386719, 106.433594, 125.441406, 107.527344);
            cr.CurveTo(121.925781, 111.605469, 116.828125, 113.945313, 111.460938, 113.945313);
            cr.CurveTo(106.089844, 113.945313, 100.996094, 111.609375, 97.476563, 107.527344);
            cr.CurveTo(96.535156, 106.433594, 95.15625, 105.804688, 93.710938, 105.804688);
            cr.CurveTo(92.265625, 105.804688, 90.890625, 106.433594, 89.945313, 107.527344);
            cr.CurveTo(86.429688, 111.605469, 81.335938, 113.945313, 75.964844, 113.945313);
            cr.CurveTo(70.597656, 113.945313, 65.5, 111.605469, 61.984375, 107.527344);
            cr.CurveTo(61.039063, 106.433594, 59.664063, 105.804688, 58.21875, 105.804688);
            cr.CurveTo(56.769531, 105.804688, 55.398438, 106.433594, 54.453125, 107.527344);
            cr.CurveTo(50.933594, 111.609375, 45.839844, 113.945313, 40.46875, 113.945313);
            cr.CurveTo(35.101563, 113.945313, 30.003906, 111.609375, 26.488281, 107.527344);
            cr.CurveTo(25.542969, 106.433594, 24.167969, 105.804688, 22.722656, 105.804688);
            cr.CurveTo(21.277344, 105.804688, 19.902344, 106.433594, 18.957031, 107.527344);
            cr.CurveTo(15.441406, 111.609375, 10.34375, 113.945313, 4.976563, 113.945313);
            cr.CurveTo(2.226563, 113.945313, 0.00390625, 116.171875, 0.00390625, 118.917969);
            cr.CurveTo(0.00390625, 121.664063, 2.230469, 123.890625, 4.976563, 123.890625);
            cr.CurveTo(10.757813, 123.890625, 16.335938, 122.128906, 21.015625, 118.917969);
            cr.CurveTo(21.597656, 118.519531, 22.171875, 118.101563, 22.722656, 117.65625);
            cr.CurveTo(23.277344, 118.101563, 23.847656, 118.519531, 24.429688, 118.917969);
            cr.CurveTo(29.109375, 122.128906, 34.6875, 123.890625, 40.46875, 123.890625);
            cr.CurveTo(46.253906, 123.890625, 51.828125, 122.128906, 56.511719, 118.917969);
            cr.CurveTo(57.09375, 118.519531, 57.664063, 118.101563, 58.21875, 117.65625);
            cr.CurveTo(58.773438, 118.101563, 59.34375, 118.519531, 59.925781, 118.917969);
            cr.CurveTo(64.605469, 122.128906, 70.179688, 123.890625, 75.964844, 123.890625);
            cr.CurveTo(81.75, 123.890625, 87.324219, 122.128906, 92.003906, 118.917969);
            cr.CurveTo(92.585938, 118.519531, 93.15625, 118.101563, 93.710938, 117.65625);
            cr.CurveTo(94.265625, 118.101563, 94.835938, 118.519531, 95.421875, 118.917969);
            cr.CurveTo(100.101563, 122.128906, 105.675781, 123.890625, 111.460938, 123.890625);
            cr.CurveTo(117.246094, 123.890625, 122.820313, 122.128906, 127.5, 118.917969);
            cr.CurveTo(128.082031, 118.519531, 128.652344, 118.101563, 129.207031, 117.65625);
            cr.CurveTo(129.761719, 118.101563, 130.332031, 118.519531, 130.914063, 118.917969);
            cr.CurveTo(135.59375, 122.128906, 141.171875, 123.890625, 146.953125, 123.890625);
            cr.CurveTo(149.699219, 123.890625, 151.929688, 121.664063, 151.929688, 118.917969);
            cr.CurveTo(151.929688, 116.171875, 149.699219, 113.945313, 146.953125, 113.945313);
            cr.ClosePath();
            cr.MoveTo(146.953125, 113.945313);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(146.953125, 87.476563);
            cr.CurveTo(141.585938, 87.476563, 136.488281, 85.140625, 132.972656, 81.0625);
            cr.CurveTo(132.027344, 79.964844, 130.652344, 79.335938, 129.207031, 79.335938);
            cr.CurveTo(127.761719, 79.335938, 126.386719, 79.964844, 125.441406, 81.0625);
            cr.CurveTo(121.925781, 85.140625, 116.828125, 87.476563, 111.460938, 87.476563);
            cr.CurveTo(106.089844, 87.476563, 100.996094, 85.140625, 97.476563, 81.0625);
            cr.CurveTo(96.535156, 79.964844, 95.15625, 79.335938, 93.710938, 79.335938);
            cr.CurveTo(92.265625, 79.335938, 90.890625, 79.964844, 89.945313, 81.0625);
            cr.CurveTo(86.429688, 85.140625, 81.335938, 87.476563, 75.964844, 87.476563);
            cr.CurveTo(70.597656, 87.476563, 65.5, 85.140625, 61.984375, 81.0625);
            cr.CurveTo(61.039063, 79.964844, 59.664063, 79.335938, 58.21875, 79.335938);
            cr.CurveTo(56.769531, 79.335938, 55.398438, 79.964844, 54.453125, 81.0625);
            cr.CurveTo(50.933594, 85.140625, 45.839844, 87.476563, 40.46875, 87.476563);
            cr.CurveTo(35.101563, 87.476563, 30.003906, 85.140625, 26.488281, 81.0625);
            cr.CurveTo(25.542969, 79.964844, 24.167969, 79.335938, 22.722656, 79.335938);
            cr.CurveTo(21.277344, 79.335938, 19.902344, 79.964844, 18.957031, 81.0625);
            cr.CurveTo(15.441406, 85.140625, 10.34375, 87.476563, 4.976563, 87.476563);
            cr.CurveTo(2.226563, 87.476563, 0.00390625, 89.703125, 0.00390625, 92.449219);
            cr.CurveTo(0.00390625, 95.199219, 2.230469, 97.425781, 4.976563, 97.425781);
            cr.CurveTo(10.757813, 97.425781, 16.335938, 95.660156, 21.015625, 92.449219);
            cr.CurveTo(21.597656, 92.050781, 22.167969, 91.632813, 22.722656, 91.191406);
            cr.CurveTo(23.277344, 91.632813, 23.847656, 92.050781, 24.429688, 92.449219);
            cr.CurveTo(29.109375, 95.660156, 34.6875, 97.425781, 40.46875, 97.425781);
            cr.CurveTo(46.253906, 97.425781, 51.828125, 95.660156, 56.511719, 92.449219);
            cr.CurveTo(57.09375, 92.050781, 57.664063, 91.632813, 58.21875, 91.191406);
            cr.CurveTo(58.773438, 91.632813, 59.34375, 92.050781, 59.925781, 92.449219);
            cr.CurveTo(64.605469, 95.660156, 70.179688, 97.425781, 75.964844, 97.425781);
            cr.CurveTo(81.75, 97.425781, 87.324219, 95.660156, 92.003906, 92.449219);
            cr.CurveTo(92.585938, 92.050781, 93.15625, 91.632813, 93.710938, 91.191406);
            cr.CurveTo(94.265625, 91.632813, 94.835938, 92.050781, 95.421875, 92.449219);
            cr.CurveTo(100.101563, 95.660156, 105.675781, 97.425781, 111.460938, 97.425781);
            cr.CurveTo(117.246094, 97.425781, 122.820313, 95.660156, 127.5, 92.449219);
            cr.CurveTo(128.082031, 92.050781, 128.652344, 91.632813, 129.207031, 91.191406);
            cr.CurveTo(129.761719, 91.632813, 130.332031, 92.050781, 130.914063, 92.449219);
            cr.CurveTo(135.597656, 95.660156, 141.171875, 97.425781, 146.953125, 97.425781);
            cr.CurveTo(149.699219, 97.425781, 151.929688, 95.199219, 151.929688, 92.449219);
            cr.CurveTo(151.929688, 89.703125, 149.699219, 87.476563, 146.953125, 87.476563);
            cr.ClosePath();
            cr.MoveTo(146.953125, 87.476563);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(4.972656, 70.953125);
            cr.CurveTo(10.757813, 70.953125, 16.332031, 69.1875, 21.011719, 65.980469);
            cr.CurveTo(21.597656, 65.582031, 22.167969, 65.164063, 22.722656, 64.71875);
            cr.CurveTo(23.273438, 65.164063, 23.847656, 65.582031, 24.429688, 65.980469);
            cr.CurveTo(29.109375, 69.1875, 34.683594, 70.953125, 40.46875, 70.953125);
            cr.CurveTo(46.253906, 70.953125, 51.828125, 69.1875, 56.507813, 65.980469);
            cr.CurveTo(57.089844, 65.582031, 57.660156, 65.164063, 58.214844, 64.71875);
            cr.CurveTo(58.769531, 65.164063, 59.339844, 65.582031, 59.925781, 65.980469);
            cr.CurveTo(64.605469, 69.1875, 70.179688, 70.953125, 75.964844, 70.953125);
            cr.CurveTo(81.746094, 70.953125, 87.320313, 69.1875, 92.003906, 65.980469);
            cr.CurveTo(92.585938, 65.582031, 93.15625, 65.164063, 93.710938, 64.71875);
            cr.CurveTo(94.261719, 65.164063, 94.835938, 65.582031, 95.417969, 65.980469);
            cr.CurveTo(100.097656, 69.1875, 105.671875, 70.953125, 111.457031, 70.953125);
            cr.CurveTo(117.242188, 70.953125, 122.816406, 69.1875, 127.496094, 65.980469);
            cr.CurveTo(128.078125, 65.582031, 128.652344, 65.164063, 129.203125, 64.71875);
            cr.CurveTo(129.757813, 65.164063, 130.328125, 65.582031, 130.914063, 65.980469);
            cr.CurveTo(135.59375, 69.1875, 141.167969, 70.953125, 146.953125, 70.953125);
            cr.CurveTo(149.699219, 70.953125, 151.925781, 68.726563, 151.925781, 65.980469);
            cr.CurveTo(151.925781, 63.234375, 149.699219, 61.007813, 146.953125, 61.007813);
            cr.CurveTo(141.585938, 61.007813, 136.488281, 58.667969, 132.972656, 54.589844);
            cr.CurveTo(132.027344, 53.496094, 130.652344, 52.863281, 129.203125, 52.863281);
            cr.CurveTo(127.757813, 52.863281, 126.382813, 53.496094, 125.4375, 54.589844);
            cr.CurveTo(121.921875, 58.667969, 116.828125, 61.007813, 111.457031, 61.007813);
            cr.CurveTo(106.085938, 61.007813, 100.992188, 58.667969, 97.476563, 54.589844);
            cr.CurveTo(96.53125, 53.496094, 95.15625, 52.863281, 93.710938, 52.863281);
            cr.CurveTo(92.261719, 52.863281, 90.886719, 53.496094, 89.941406, 54.589844);
            cr.CurveTo(86.425781, 58.667969, 81.332031, 61.007813, 75.960938, 61.007813);
            cr.CurveTo(70.59375, 61.007813, 65.496094, 58.667969, 61.980469, 54.589844);
            cr.CurveTo(61.035156, 53.496094, 59.660156, 52.863281, 58.214844, 52.863281);
            cr.CurveTo(56.769531, 52.863281, 55.394531, 53.496094, 54.449219, 54.589844);
            cr.CurveTo(50.933594, 58.667969, 45.835938, 61.007813, 40.46875, 61.007813);
            cr.CurveTo(35.097656, 61.007813, 30.003906, 58.667969, 26.484375, 54.589844);
            cr.CurveTo(25.539063, 53.496094, 24.164063, 52.863281, 22.71875, 52.863281);
            cr.CurveTo(21.273438, 52.863281, 19.898438, 53.496094, 18.953125, 54.589844);
            cr.CurveTo(15.4375, 58.667969, 10.339844, 61.007813, 4.972656, 61.007813);
            cr.CurveTo(2.226563, 61.007813, 0, 63.234375, 0, 65.980469);
            cr.CurveTo(0, 68.726563, 2.226563, 70.953125, 4.972656, 70.953125);
            cr.ClosePath();
            cr.MoveTo(4.972656, 70.953125);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            if (pattern != null) pattern.Dispose();

            cr.Restore();
        }

        



        #endregion
    }
}
