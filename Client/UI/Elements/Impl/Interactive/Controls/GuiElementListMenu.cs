using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    // Contains quite some ugly hacks to make the scrollbar work :<
    public class GuiElementListMenu : GuiElementTextControl
    {
        internal string[] values;
        internal string[] names;

        int maxHeight = 350;
        double expandedBoxWidth = 0;
        double expandedBoxHeight = 0;


        internal int selectedIndex;
        internal int hoveredIndex;

        API.Common.Action<string> onSelectionChanged;

        LoadedTexture hoverTexture;
        LoadedTexture dropDownTexture;
        LoadedTexture scrollbarTexture;

        bool expanded;

        double scrollOffY = 0;

        GuiElementCompactScrollbar scrollbar;

        ElementBounds visibleBounds;

        public bool IsOpened
        {
            get { return expanded; }
        }

        public override double DrawOrder
        {
            get { return 0.5; }
        }

        public override bool Focusable
        {
            get { return true; }
        }

        public GuiElementListMenu(ICoreClientAPI capi, string[] values, string[] names, int selectedIndex, API.Common.Action<string> onSelectionChanged, ElementBounds bounds) : base(capi, "", CairoFont.WhiteSmallText(), bounds)
        {
            hoverTexture = new LoadedTexture(capi);
            dropDownTexture = new LoadedTexture(capi);
            scrollbarTexture = new LoadedTexture(capi);

            this.values = values;
            this.names = names;
            this.selectedIndex = selectedIndex;
            this.onSelectionChanged = onSelectionChanged;
            hoveredIndex = selectedIndex;



            ElementBounds scrollbarBounds = ElementBounds.Fixed(0, 0, 0, 0).WithEmptyParent();

            scrollbar = new GuiElementCompactScrollbar(api, onNewScrollbarValue, scrollbarBounds);
        }

        private void onNewScrollbarValue(float offY)
        {
            scrollOffY = (int)(offY / (30 * Scale)) * 30 * Scale;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ComposeDynamicElements();
        }


        public void ComposeDynamicElements()
        {
            Bounds.CalcWorldBounds();

            // Expandable box with list of names
            expandedBoxWidth = Bounds.InnerWidth;
            expandedBoxHeight = (10 + values.Length * 30) * Scale;
            

            for (int i = 0; i < values.Length; i++)
            {
                expandedBoxWidth = Math.Max(expandedBoxWidth, Font.GetTextExtents(names[i]).Width);
            }

            expandedBoxWidth += 5 * Scale;


            ImageSurface surface = new ImageSurface(Format.Argb32, (int)expandedBoxWidth, (int)expandedBoxHeight);
            Context ctx = genContext(surface);

            visibleBounds = Bounds.FlatCopy();
            visibleBounds.fixedHeight = Math.Min(maxHeight, expandedBoxHeight);
            visibleBounds.fixedWidth = expandedBoxWidth / RuntimeEnv.GUIScale;
            visibleBounds.fixedY += Bounds.InnerHeight / RuntimeEnv.GUIScale;
            visibleBounds.CalcWorldBounds();

            Font.SetupContext(ctx);

            ctx.SetSourceRGBA(ElementGeometrics.DialogDefaultBgColor);
            RoundRectangle(ctx, 0, 0, expandedBoxWidth, expandedBoxHeight, 1);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(0,0,0,0.5);
            ctx.Stroke();

            double height = Font.GetFontExtents().Height;
            double offy = (30 - height) / 2;

            ctx.SetSourceRGBA(ElementGeometrics.DialogDefaultTextColor);

            for (int i = 0; i < values.Length; i++)
            {
                ctx.MoveTo(5 * Scale, ((int)offy + i * 30) * Scale);
                ShowTextCorrectly(ctx, names[i], 0, 0);
            }

            generateTexture(surface, ref dropDownTexture);

            ctx.Dispose();
            surface.Dispose();


            // Scrollbar static stuff
            scrollbar.Bounds.WithFixedSize(10, visibleBounds.fixedHeight - 4).WithFixedPosition(expandedBoxWidth / RuntimeEnv.GUIScale - 10, 0).WithFixedPadding(0, 2);
            scrollbar.Bounds.WithEmptyParent();
            scrollbar.Bounds.CalcWorldBounds();

            surface = new ImageSurface(Format.Argb32, (int)expandedBoxWidth, (int)scrollbar.Bounds.OuterHeight);
            ctx = genContext(surface);
            
            scrollbar.ComposeElements(ctx, surface);
            scrollbar.SetHeights((int)visibleBounds.InnerHeight / RuntimeEnv.GUIScale, (int)expandedBoxHeight);

            generateTexture(surface, ref scrollbarTexture);

            ctx.Dispose();
            surface.Dispose();


            // Hover bar

            surface = new ImageSurface(Format.Argb32, (int)expandedBoxWidth, (int)(30 * Scale));
            ctx = genContext(surface);

            double[] col = ElementGeometrics.DialogHighlightColor;
            col[3] = 0.5;
            ctx.SetSourceRGBA(col);
            RoundRectangle(ctx, 0, 0, expandedBoxWidth, 30*Scale, 0);
            ctx.Fill();
            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
            
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (expanded)
            {
                api.Render.BeginScissor(visibleBounds);

                api.Render.Render2DTexturePremultipliedAlpha(
                    dropDownTexture.TextureId, 
                    (int)Bounds.renderX,
                    (int)Bounds.renderY + (int)Bounds.InnerHeight - (int)scrollOffY, 
                    (int)expandedBoxWidth, 
                    (int)expandedBoxHeight,
                    60
                );

                if (hoveredIndex >= 0)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(
                        hoverTexture.TextureId, 
                        (int)Bounds.renderX, 
                        (int)(Bounds.renderY + Bounds.InnerHeight + 30 * Scale * hoveredIndex), 
                        (int)expandedBoxWidth - scaled(14), 
                        (int)30 * Scale,
                        61
                    );
                }

                api.Render.EndScissor();

                api.Render.Render2DTexturePremultipliedAlpha(
                    scrollbarTexture.TextureId,
                    (int)visibleBounds.renderX,
                    (int)visibleBounds.renderY,
                    (int)visibleBounds.OuterWidth,
                    (int)visibleBounds.OuterHeight,
                    66
                );

                scrollbar.Bounds.WithParent(Bounds);
                scrollbar.Bounds.absFixedY = Bounds.InnerHeight;
                scrollbar.RenderInteractiveElements(deltaTime);
            }

        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!hasFocus) return;

            if (args.KeyCode == (int)GlKeys.Enter && expanded)
            {
                expanded = false;
                selectedIndex = hoveredIndex;
                onSelectionChanged?.Invoke(values[selectedIndex]);
                args.Handled = true;
                return;
            }

            if (args.KeyCode == (int)GlKeys.Up || args.KeyCode == (int)GlKeys.Down)
            {
                args.Handled = true;

                if (!expanded)
                {
                    expanded = true;
                    hoveredIndex = selectedIndex;
                    return;
                }

                if (args.KeyCode == (int)GlKeys.Up)
                {
                    hoveredIndex = GameMath.Mod((hoveredIndex - 1), values.Length);
                }
                else
                {
                    hoveredIndex = GameMath.Mod((hoveredIndex + 1), values.Length);
                }
            }
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (!expanded) return;

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            if (mouseX >= Bounds.renderX && mouseX <= Bounds.renderX + expandedBoxWidth)
            {
                int num = (int)((mouseY - Bounds.renderY - 30 * Scale) / (30 * Scale));
                if (num >= 0 && num < values.Length)
                {
                    hoveredIndex = num;
                    args.Handled = true;
                }
            }
        }


        public void Open()
        {
            expanded = true;
        }

        internal void Close()
        {
            expanded = false;
        }

        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            if (!expanded) return;

            if (expanded && args.X >= Bounds.renderX && args.X <= Bounds.renderX + expandedBoxWidth)
            {
                int selectedElement = (int)((args.Y - Bounds.renderY - 30 * Scale + scrollOffY) / (30 * Scale));

                if (selectedElement >= 0 && selectedElement < values.Length)
                {
                    selectedIndex = selectedElement;
                    onSelectionChanged?.Invoke(values[selectedIndex]);
                    expanded = false;
                    args.Handled = true;
                }
            }
        }

        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (expanded && visibleBounds.PointInside(api.Input.MouseX, api.Input.MouseY))
            {
                scrollbar.OnMouseWheel(api, args);
            }
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();
            expanded = false;
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            this.selectedIndex = selectedIndex;
        }

        public void SetList(string[] values, string[] names)
        {
            this.values = values;
            this.names = names;
            ComposeDynamicElements();
        }


        public override void Dispose()
        {
            base.Dispose();

            hoverTexture.Dispose();
            dropDownTexture.Dispose();
            scrollbarTexture.Dispose();

            scrollbar?.Dispose();
        }


    }
}
