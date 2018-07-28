using System;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public delegate void DrawDelegate(Context ctx, ImageSurface surface);

    public class IconUtil
    {
        ICoreClientAPI capi;

        public IconUtil(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

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
                textureId = textureId,
                width = width,
                height = height
            };
        }

        public void DrawIcon(Context cr, string type, double x, double y, double width, double height, double[] rgba)
        {
            DrawIconInt(cr, type, (int)x, (int)y, (float)width, (float)height, rgba);
        }

        public void DrawIconInt(Context cr, string type, int x, int y, float width, float height, double[] rgba)
        {

            switch (type)
            {
                case "none":
                    Drawnone_svg(cr, x, y, width, height, rgba);
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

                case "copy":
                    Drawduplicate_svg(cr, x, y, width, height, rgba);
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

                case "left":
                    Drawleft_svg(cr, x, y, width, height, rgba);
                    break;

                case "right":
                    Drawright_svg(cr, x, y, width, height, rgba);
                    break;
            }
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

            cr.LineWidth = 28;
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

            cr.LineWidth = 28;
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

            cr.LineWidth = 28;
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



        public void DrawMapPlayer(Context cr, int x, int y, float width, float height, double[] rgba)
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
            //cr.FillRule = FillRule.Winding;
            //cr.FillPreserve();
            cr.Stroke();
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

            cr.Restore();
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

            surface.Blur(blurRadius);

            ctx.Operator = Operator.DestOver;
            ctx.Rectangle(x, y, (int)size + 1, (int)size + 1);
            ctx.Fill();

            blurCtx.Dispose();
            surface.Dispose();

            ctx.Operator = Operator.Over;   
        }



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


        public void Drawduplicate_svg(Context cr, int x, int y, float width, float height, double[] rgba)
        {
            Pattern pattern = null;
            Matrix matrix = cr.Matrix;

            cr.Save();
            float w = 129;
            float h = 129;
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
            cr.MoveTo(71.328125, 66.042969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
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
            cr.MoveTo(71.328125, 46.078125);
            cr.LineTo(71.328125, 30.828125);
            cr.CurveTo(71.328125, 29.691406, 70.667969, 28.097656, 69.863281, 27.292969);
            cr.LineTo(60.363281, 17.792969);
            cr.CurveTo(59.558594, 16.988281, 57.96875, 16.328125, 56.828125, 16.328125);
            cr.LineTo(29.898438, 16.328125);
            cr.CurveTo(28.761719, 16.328125, 27.828125, 17.261719, 27.828125, 18.398438);
            cr.LineTo(27.828125, 76.398438);
            cr.CurveTo(27.828125, 77.539063, 28.761719, 78.472656, 29.898438, 78.472656);
            cr.LineTo(50.828125, 78.472656);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(58.035156, 27.6875);
            cr.CurveTo(58.035156, 28.828125, 58.96875, 29.757813, 60.109375, 29.757813);
            cr.LineTo(68.394531, 29.757813);
            cr.CurveTo(69.535156, 29.757813, 69.804688, 29.097656, 69, 28.292969);
            cr.LineTo(59.5, 18.796875);
            cr.CurveTo(58.695313, 17.988281, 58.035156, 18.261719, 58.035156, 19.402344);
            cr.ClosePath();
            cr.MoveTo(58.035156, 27.6875);
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
            cr.MoveTo(94.328125, 95.792969);
            cr.LineTo(94.328125, 60.578125);
            cr.CurveTo(94.328125, 59.441406, 93.667969, 57.847656, 92.863281, 57.042969);
            cr.LineTo(83.363281, 47.542969);
            cr.CurveTo(82.558594, 46.738281, 80.96875, 46.078125, 79.828125, 46.078125);
            cr.LineTo(52.898438, 46.078125);
            cr.CurveTo(51.761719, 46.078125, 50.828125, 47.011719, 50.828125, 48.148438);
            cr.LineTo(50.828125, 106.148438);
            cr.CurveTo(50.828125, 107.289063, 51.761719, 108.222656, 52.898438, 108.222656);
            cr.LineTo(92.257813, 108.222656);
            cr.CurveTo(93.398438, 108.222656, 94.328125, 107.289063, 94.328125, 106.148438);
            cr.LineTo(94.328125, 95.792969);
            cr.Tolerance = 0.1;
            cr.Antialias = Antialias.Default;
            cr.StrokePreserve();
            if (pattern != null) pattern.Dispose();

            cr.Operator = Operator.Over;
            pattern = new SolidPattern(rgba[0], rgba[1], rgba[2], rgba[3]);
            cr.SetSource(pattern);

            cr.NewPath();
            cr.MoveTo(81.035156, 57.4375);
            cr.CurveTo(81.035156, 58.578125, 81.96875, 59.507813, 83.109375, 59.507813);
            cr.LineTo(91.394531, 59.507813);
            cr.CurveTo(92.535156, 59.507813, 92.804688, 58.847656, 92, 58.042969);
            cr.LineTo(82.5, 48.546875);
            cr.CurveTo(81.695313, 47.738281, 81.035156, 48.011719, 81.035156, 49.152344);
            cr.ClosePath();
            cr.MoveTo(81.035156, 57.4375);
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
