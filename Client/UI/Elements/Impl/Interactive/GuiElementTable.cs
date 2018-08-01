using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public interface IGuiElementCell
    {
        ElementBounds Bounds { get; }

        void OnRenderInteractiveElements(ICoreClientAPI api, ElementBounds bounds, float deltaTime);
        void UpdateCellHeight();
        void ComposeElements(Context ctx, ImageSurface surface);
        void CreateDynamicParts();
    }

    public delegate IGuiElementCell OnRequireCell(TableCell cell, ElementBounds bounds);

    public class GuiElementTable : GuiElement
    {
        public List<IGuiElementCell> elementCells = new List<IGuiElementCell>();

        public int unscaledCellSpacing = 10;
        public int UnscaledCellVerPadding = 2;
        public int UnscaledCellHorPadding = 6;


        public API.Common.Action<int> leftPartClick;
        public API.Common.Action<int> rightPartClick;

        int tableTextureId;

        ElementBounds insideBounds;

        OnRequireCell cellcreator;

        public GuiElementTable(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<int> OnMouseDownOnCellLeft, API.Common.Action<int> OnMouseDownOnCellRight, OnRequireCell cellCreator, List<TableCell> cells = null) : base(capi, bounds)
        {
            this.cellcreator = cellCreator;

            insideBounds = new ElementBounds().WithFixedPadding(unscaledCellSpacing).WithEmptyParent();
            insideBounds.CalcWorldBounds();

            leftPartClick = OnMouseDownOnCellLeft;
            rightPartClick = OnMouseDownOnCellRight;

            if (cells != null)
            {
                foreach (TableCell cell in cells)
                {
                    AddCell(cell);
                }
            }

            CalcTotalHeight();
        }

        internal void ReloadCells(List<TableCell> cells)
        {
            elementCells.Clear();

            foreach (TableCell cell in cells)
            {
                AddCell(cell);
            }

            CalcTotalHeight();
            ComposeTable();
        }


        public void CalcTotalHeight()
        {
            double height = 0;
            foreach (IGuiElementCell cell in elementCells)
            {
                cell.UpdateCellHeight();
                height += cell.Bounds.fixedHeight + unscaledCellSpacing + 2 * UnscaledCellVerPadding;
            }

            Bounds.fixedHeight = height + unscaledCellSpacing;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            insideBounds = new ElementBounds().WithFixedPadding(unscaledCellSpacing).WithEmptyParent();
            insideBounds.CalcWorldBounds();

            Bounds.CalcWorldBounds();
            ComposeTable();
        }

        

        void ComposeTable()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctx = genContext(surface);

            
            CalcTotalHeight();
            Bounds.CalcWorldBounds();
            

            double unscaledHeight = 0;
            int i = 0;
            foreach (IGuiElementCell cell in elementCells)
            {
                cell.Bounds.WithFixedAlignmentOffset(0, unscaledHeight);
                cell.ComposeElements(ctx, surface);
                cell.CreateDynamicParts();
                i++;

                unscaledHeight += cell.Bounds.OuterHeight / RuntimeEnv.GUIScale + unscaledCellSpacing;
            }

            //surface.WriteToPng("table.png");
            generateTexture(surface, ref tableTextureId);

            ctx.Dispose();
            surface.Dispose();
        }

        public void AddCell(TableCell cell, int afterPosition = -1)
        {
            ElementBounds cellBounds = new ElementBounds()
            {
                fixedPaddingX = UnscaledCellHorPadding,
                fixedPaddingY = UnscaledCellVerPadding,
                fixedWidth = Bounds.fixedWidth - 2 * UnscaledCellHorPadding - 2 * unscaledCellSpacing,
                fixedHeight = 0,
                BothSizing = ElementSizing.Fixed,
            }.WithParent(insideBounds);

            IGuiElementCell cellElem = this.cellcreator(cell, cellBounds);

            //GuiElementCell cellElement = new GuiElementCell(cell, cellBounds);

            if (afterPosition == -1)
            {    
                elementCells.Add(cellElem);
            } else
            {
                elementCells.Insert(afterPosition, cellElem);
            }
        }


        public void RemoveCell(int position)
        {
            elementCells.RemoveAt(position);
        }


        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;

            int i = 0;

            int dx = api.Input.MouseX - (int)Bounds.absX;
            int dy = api.Input.MouseY - (int)Bounds.absY;


            foreach (IGuiElementCell element in elementCells)
            {
                Vec2d pos = element.Bounds.PositionInside(dx, dy);

                if (pos != null)
                {
                    api.Gui.PlaySound("menubutton_press");

                    if (pos.X > element.Bounds.InnerWidth - scaled(GuiElementCell.unscaledRightBoxWidth))
                    {
                        rightPartClick?.Invoke(i);
                        args.Handled = true;
                        return;
                    }
                    else
                    {
                        leftPartClick?.Invoke(i);
                        args.Handled = true;
                        return;
                    }
                }

                i++;
            }
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(tableTextureId, Bounds);

            int i = 0;

            int dx = api.Input.MouseX - (int)Bounds.absX;
            int dy = api.Input.MouseY - (int)Bounds.absY;

            foreach (IGuiElementCell element in elementCells)
            {
                element.OnRenderInteractiveElements(api, Bounds, deltaTime);
                i++;
            }
        }

    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddTable(this GuiComposer composer, ElementBounds bounds, OnRequireCell creallCreator, API.Common.Action<int> OnMouseDownOnCellLeft = null, API.Common.Action<int> OnMouseDownOnCellRight = null, List<TableCell> cells = null, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementTable(composer.Api, bounds, OnMouseDownOnCellLeft, OnMouseDownOnCellRight, creallCreator, cells), key);
            }

            return composer;
        }

        public static GuiElementTable GetTable(this GuiComposer composer, string key)
        {
            return (GuiElementTable)composer.GetElement(key);
        }
    }

}
