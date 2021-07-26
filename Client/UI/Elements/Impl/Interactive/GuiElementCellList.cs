using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using System;

namespace Vintagestory.API.Client
{
    public interface IGuiElementCell
    {
        ElementBounds InsideClipBounds { get; set; }

        /// <summary>
        /// The bounds of the cell.
        /// </summary>
        ElementBounds Bounds { get; }

        /// <summary>
        /// The event fired when the cell is rendered.
        /// </summary>
        /// <param name="api">The Client API</param>
        /// <param name="deltaTime">The change in time.</param>
        void OnRenderInteractiveElements(ICoreClientAPI api, float deltaTime);

        /// <summary>
        /// Called when the cell is modified and needs to be updated.
        /// </summary>
        void UpdateCellHeight();

        /// <summary>
        /// cleans up and gets rid of the cell in a neat and orderly fashion.
        /// </summary>
        void Dispose();
    }

    public delegate IGuiElementCell OnRequireCell(ListCellEntry cell, ElementBounds bounds);

    public class GuiElementCellList : GuiElement
    {
        /// <summary>
        /// The cells in the list.  See IGuiElementCell for how it's supposed to function.
        /// </summary>
        public List<IGuiElementCell> elementCells = new List<IGuiElementCell>();

        List<IGuiElementCell> visibleCells = new List<IGuiElementCell>();

        /// <summary>
        /// the space between the cells.  Default: 10
        /// </summary>
        public int unscaledCellSpacing = 10;

        /// <summary>
        /// The padding on the vertical axis of the cell.  Default: 2
        /// </summary>
        public int UnscaledCellVerPadding = 4;

        /// <summary>
        /// The padding on the horizontal axis of the cell.  Default: 7
        /// </summary>
        public int UnscaledCellHorPadding = 7;

        /// <summary>
        /// The delegate fired when the left part of the cell is clicked.
        /// </summary>
        public API.Common.Action<int> leftPartClick;

        /// <summary>
        /// The delegate fired when the right part of the cell is clicked.
        /// </summary>
        public API.Common.Action<int> rightPartClick;

        

        //ElementBounds insideBounds;

        OnRequireCell cellcreator;

        public override ElementBounds InsideClipBounds { 
            get => base.InsideClipBounds; 
            set {
                base.InsideClipBounds = value;

                foreach (var val in elementCells)
                {
                    val.InsideClipBounds = InsideClipBounds;
                }
            }
        }


        /// <summary>
        /// Creates a new list in the current GUI.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="bounds">The bounds of the list.</param>
        /// <param name="OnMouseDownOnCellLeft">The function fired when the cell is clicked on the left side.</param>
        /// <param name="OnMouseDownOnCellRight">The function fired when the cell is clicked on the right side.</param>
        /// <param name="cellCreator">The event fired when a cell is requested by the gui</param>
        /// <param name="cells">The array of cells initialized with the list.</param>
        public GuiElementCellList(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<int> OnMouseDownOnCellLeft, API.Common.Action<int> OnMouseDownOnCellRight, OnRequireCell cellCreator, List<ListCellEntry> cells = null) : base(capi, bounds)
        {
            this.cellcreator = cellCreator;

            Bounds.IsDrawingSurface = true;

            leftPartClick = OnMouseDownOnCellLeft;
            rightPartClick = OnMouseDownOnCellRight;

            if (cells != null)
            {
                foreach (ListCellEntry cell in cells)
                {
                    AddCell(cell);
                }

                visibleCells.Clear();
                visibleCells.AddRange(elementCells);
            }

            CalcTotalHeight();
        }

        public void ReloadCells(List<ListCellEntry> cells)
        {
            foreach (var val in elementCells)
            {
                val?.Dispose();
            }

            elementCells.Clear();

            foreach (ListCellEntry cell in cells)
            {
                AddCell(cell);
            }

            visibleCells.Clear();
            visibleCells.AddRange(elementCells);

            CalcTotalHeight();
        }

        public override void BeforeCalcBounds()
        {
            CalcTotalHeight();
        }

        /// <summary>
        /// Calculates the total height for the list.
        /// </summary>
        public void CalcTotalHeight()
        {
            Bounds.CalcWorldBounds();
            double height = 0;
            double unscaledHeight = 0;

            foreach (IGuiElementCell cell in visibleCells)
            {
                cell.UpdateCellHeight();
                cell.Bounds.WithFixedPosition(0, unscaledHeight);
                cell.Bounds.CalcWorldBounds();

                height += cell.Bounds.fixedHeight + unscaledCellSpacing + 2 * UnscaledCellVerPadding;

                unscaledHeight += cell.Bounds.OuterHeight / RuntimeEnv.GUIScale + unscaledCellSpacing;
            }

            Bounds.fixedHeight = height + unscaledCellSpacing;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
        }


        Func<IGuiElementCell, bool> cellFilter;


        internal void FilterCells(Func<IGuiElementCell, bool> onFilter)
        {
            this.cellFilter = onFilter;

            visibleCells.Clear();
            foreach (IGuiElementCell elem in elementCells)
            {
                if (cellFilter(elem))
                {
                    visibleCells.Add(elem);
                }
            }


            CalcTotalHeight();
        }

        /// <summary>
        /// Adds a cell to the list.
        /// </summary>
        /// <param name="cell">The cell to add.</param>
        /// <param name="afterPosition">The position of the cell to add after.  (Default: -1)</param>
        protected void AddCell(ListCellEntry cell, int afterPosition = -1)
        {
            ElementBounds cellBounds = new ElementBounds()
            {
                fixedPaddingX = UnscaledCellHorPadding,
                fixedPaddingY = UnscaledCellVerPadding,
                fixedWidth = Bounds.fixedWidth - 2 * Bounds.fixedPaddingX - 2 * UnscaledCellHorPadding,
                fixedHeight = 0,
                BothSizing = ElementSizing.Fixed,
            }.WithParent(Bounds);



            IGuiElementCell cellElem = this.cellcreator(cell, cellBounds);
            cellElem.InsideClipBounds = InsideClipBounds;

            if (afterPosition == -1)
            {
                elementCells.Add(cellElem);
            }
            else
            {
                elementCells.Insert(afterPosition, cellElem);
            }
        }

        /// <summary>
        /// Removes a cell at a specified position.
        /// </summary>
        /// <param name="position">The position of the cell to remove.</param>
        protected void RemoveCell(int position)
        {
            elementCells.RemoveAt(position);
        }


        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;

            int i = 0;

            int mousex = api.Input.MouseX;
            int mousey = api.Input.MouseY;


            foreach (IGuiElementCell element in visibleCells)
            {
                Vec2d pos = element.Bounds.PositionInside(mousex, mousey);

                if (pos != null)
                {
                    api.Gui.PlaySound("menubutton_press");

                    if (pos.X > element.Bounds.InnerWidth - scaled(GuiElementCell.unscaledRightBoxWidth))
                    {
                        rightPartClick?.Invoke(elementCells.IndexOf(element));
                        args.Handled = true;
                        return;
                    }
                    else
                    {
                        leftPartClick?.Invoke(elementCells.IndexOf(element));
                        args.Handled = true;
                        return;
                    }
                }

                i++;
            }
        }

        public override void RenderInteractiveElements(float deltaTime)
        {            
            foreach (IGuiElementCell element in visibleCells)
            {
                if (element.Bounds.PartiallyInside(Bounds.ParentBounds))
                {
                    element.OnRenderInteractiveElements(api, deltaTime);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var val in elementCells)
            {
                val.Dispose();
            }
        }

    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a List to the current GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="creallCreator">the event fired when the cell is requested by the GUI</param>
        /// <param name="OnMouseDownOnCellLeft">The event fired when the player clicks on the lefthand side of the cell.</param>
        /// <param name="OnMouseDownOnCellRight">The event fired when the player clicks on the righthand side of the cell.</param>
        /// <param name="cells">The cells of the list.</param>
        /// <param name="key">The identifier for the list.</param>
        public static GuiComposer AddCellList(this GuiComposer composer, ElementBounds bounds, OnRequireCell creallCreator, API.Common.Action<int> OnMouseDownOnCellLeft = null, API.Common.Action<int> OnMouseDownOnCellRight = null, List<ListCellEntry> cells = null, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementCellList(composer.Api, bounds, OnMouseDownOnCellLeft, OnMouseDownOnCellRight, creallCreator, cells), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the list by name.
        /// </summary>
        /// <param name="key">The name of the list to get.</param>
        /// <returns></returns>
        public static GuiElementCellList GetCellList(this GuiComposer composer, string key)
        {
            return (GuiElementCellList)composer.GetElement(key);
        }
    }

}
