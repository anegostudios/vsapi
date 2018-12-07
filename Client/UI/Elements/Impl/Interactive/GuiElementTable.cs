using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public interface IGuiElementCell
    {
        /// <summary>
        /// The bounds of the cell.
        /// </summary>
        ElementBounds Bounds { get; }

        /// <summary>
        /// The event fired when the cell is rendered.
        /// </summary>
        /// <param name="api">The Client API</param>
        /// <param name="bounds">The bounds of the cell</param>
        /// <param name="deltaTime">The change in time.</param>
        void OnRenderInteractiveElements(ICoreClientAPI api, ElementBounds bounds, float deltaTime);

        /// <summary>
        /// Called when the cell is modified and needs to be updated.
        /// </summary>
        void UpdateCellHeight();

        /// <summary>
        /// Composes the elements of the cell.
        /// </summary>
        /// <param name="ctx">The context for the cell</param>
        /// <param name="surface">The surface of the cell area.</param>
        void ComposeElements(Context ctx, ImageSurface surface);

        /// <summary>
        /// This builds the dynamic parts for the cell.
        /// </summary>
        void CreateDynamicParts();

        /// <summary>
        /// cleans up and gets rid of the cell in a neat and orderly fashion.
        /// </summary>
        void Dispose();
    }

    public delegate IGuiElementCell OnRequireCell(TableCell cell, ElementBounds bounds);

    public class GuiElementTable : GuiElement
    {
        /// <summary>
        /// The cells in the table.  See IGuiElementCell for how it's supposed to function.
        /// </summary>
        public List<IGuiElementCell> elementCells = new List<IGuiElementCell>();

        /// <summary>
        /// the space between the cells.  Default: 10
        /// </summary>
        public int unscaledCellSpacing = 10;

        /// <summary>
        /// The padding on the vertical axis of the cell.  Default: 2
        /// </summary>
        public int UnscaledCellVerPadding = 2;

        /// <summary>
        /// The padding on the horizontal axis of the cell.  Default: 6
        /// </summary>
        public int UnscaledCellHorPadding = 6;

        /// <summary>
        /// The delegate fired when the left part of the cell is clicked.
        /// </summary>
        public API.Common.Action<int> leftPartClick;

        /// <summary>
        /// The delegate fired when the right part of the cell is clicked.
        /// </summary>
        public API.Common.Action<int> rightPartClick;

        LoadedTexture tableTexture;

        ElementBounds insideBounds;

        OnRequireCell cellcreator;

        /// <summary>
        /// Creates a new table in the current GUI.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="bounds">The bounds of the table.</param>
        /// <param name="OnMouseDownOnCellLeft">The function fired when the cell is clicked on the left side.</param>
        /// <param name="OnMouseDownOnCellRight">The function fired when the cell is clicked on the right side.</param>
        /// <param name="cellCreator">The event fired when a cell is requested by the gui</param>
        /// <param name="cells">The array of cells initialized with the table.</param>
        public GuiElementTable(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<int> OnMouseDownOnCellLeft, API.Common.Action<int> OnMouseDownOnCellRight, OnRequireCell cellCreator, List<TableCell> cells = null) : base(capi, bounds)
        {
            tableTexture = new LoadedTexture(capi);
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
            foreach (var val in elementCells)
            {
                val?.Dispose();
            }

            elementCells.Clear();

            foreach (TableCell cell in cells)
            {
                AddCell(cell);
            }

            CalcTotalHeight();
            ComposeTable();
        }

        /// <summary>
        /// Calculates the total height for the table.
        /// </summary>
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
            generateTexture(surface, ref tableTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Adds a cell to the table.
        /// </summary>
        /// <param name="cell">The cell to add.</param>
        /// <param name="afterPosition">The position of the cell to add after.  (Default: -1)</param>
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

        /// <summary>
        /// Removes a cell at a specified position.
        /// </summary>
        /// <param name="position">The position of the cell to remove.</param>
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
            api.Render.Render2DTexturePremultipliedAlpha(tableTexture.TextureId, Bounds);

            int i = 0;

            int dx = api.Input.MouseX - (int)Bounds.absX;
            int dy = api.Input.MouseY - (int)Bounds.absY;

            foreach (IGuiElementCell element in elementCells)
            {
                element.OnRenderInteractiveElements(api, Bounds, deltaTime);
                i++;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            tableTexture.Dispose();

            foreach (var val in elementCells)
            {
                val.Dispose();
            }
        }

    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a Table to the current GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="creallCreator">the event fired when the cell is requested by the GUI</param>
        /// <param name="OnMouseDownOnCellLeft">The event fired when the player clicks on the lefthand side of the cell.</param>
        /// <param name="OnMouseDownOnCellRight">The event fired when the player clicks on the righthand side of the cell.</param>
        /// <param name="cells">The cells of the table.</param>
        /// <param name="key">The identifier for the table.</param>
        public static GuiComposer AddTable(this GuiComposer composer, ElementBounds bounds, OnRequireCell creallCreator, API.Common.Action<int> OnMouseDownOnCellLeft = null, API.Common.Action<int> OnMouseDownOnCellRight = null, List<TableCell> cells = null, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementTable(composer.Api, bounds, OnMouseDownOnCellLeft, OnMouseDownOnCellRight, creallCreator, cells), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the table by name.
        /// </summary>
        /// <param name="key">The name of the table to get.</param>
        /// <returns></returns>
        public static GuiElementTable GetTable(this GuiComposer composer, string key)
        {
            return (GuiElementTable)composer.GetElement(key);
        }
    }

}
