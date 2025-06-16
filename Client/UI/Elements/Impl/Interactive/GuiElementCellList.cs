using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Config;
using System;

#nullable disable

namespace Vintagestory.API.Client
{
    public interface IGuiElementCell : IDisposable
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

        void OnMouseUpOnElement(MouseEvent args, int elementIndex);

        void OnMouseDownOnElement(MouseEvent args, int elementIndex);

        void OnMouseMoveOnElement(MouseEvent args, int elementIndex);

        string MouseOverCursor { get; }
    }

    public delegate IGuiElementCell OnRequireCell<T>(T cell, ElementBounds bounds);

    public class GuiElementCellList<T> : GuiElement
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

        

        Func<IGuiElementCell, bool> cellFilter;
        OnRequireCell<T> cellcreator;
        bool didInitialize;

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

        IEnumerable<T> cellsTmp = null;


        /// <summary>
        /// Creates a new list in the current GUI.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="bounds">The bounds of the list.</param>
        /// <param name="cellCreator">The event fired when a cell is requested by the gui</param>
        /// <param name="cells">The array of cells initialized with the list.</param>
        public GuiElementCellList(ICoreClientAPI capi, ElementBounds bounds, OnRequireCell<T> cellCreator, IEnumerable<T> cells = null) : base(capi, bounds)
        {
            this.cellcreator = cellCreator;
            this.cellsTmp = cells;
            Bounds.IsDrawingSurface = true;
        }

        void Initialize()
        {
            if (cellsTmp != null)
            {
                foreach (T cell in cellsTmp)
                {
                    AddCell(cell);
                }

                visibleCells.Clear();
                visibleCells.AddRange(elementCells);
            }

            CalcTotalHeight();
            didInitialize = true;
        }

        public void ReloadCells(IEnumerable<T> cells)
        {
            
            foreach (var val in elementCells)
            {
                val?.Dispose();
            }

            elementCells.Clear();

            foreach (T cell in cells)
            {
                AddCell(cell);
            }

            visibleCells.Clear();
            visibleCells.AddRange(elementCells);

            CalcTotalHeight();
        }

        public override void BeforeCalcBounds()
        {
            if (!didInitialize) Initialize();
            else CalcTotalHeight();
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
            Bounds.CalcWorldBounds();
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
        }


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
        protected void AddCell(T cell, int afterPosition = -1)
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

            int mousex = api.Input.MouseX;
            int mousey = api.Input.MouseY;

            foreach (IGuiElementCell element in visibleCells)
            {
                Vec2d pos = element.Bounds.PositionInside(mousex, mousey);

                if (pos != null)
                {
                    element.OnMouseUpOnElement(args, elementCells.IndexOf(element));
                }
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;

            int mousex = api.Input.MouseX;
            int mousey = api.Input.MouseY;

            foreach (IGuiElementCell element in visibleCells)
            {
                Vec2d pos = element.Bounds.PositionInside(mousex, mousey);

                if (pos != null)
                {
                    element.OnMouseDownOnElement(args, elementCells.IndexOf(element));
                }
            }
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;

            int mousex = api.Input.MouseX;
            int mousey = api.Input.MouseY;

            foreach (IGuiElementCell element in visibleCells)
            {
                Vec2d pos = element.Bounds.PositionInside(mousex, mousey);

                if (pos != null)
                {
                    element.OnMouseMoveOnElement(args, elementCells.IndexOf(element));
                }
            }
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            MouseOverCursor = null;

            foreach (IGuiElementCell element in visibleCells)
            {
                if (element.Bounds.PartiallyInside(Bounds.ParentBounds))
                {
                    element.OnRenderInteractiveElements(api, deltaTime);
                    if (element.MouseOverCursor != null)
                    {
                        MouseOverCursor = element.MouseOverCursor;
                    }
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
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="cellCreator">the event fired when the cell is requested by the GUI</param>
        /// <param name="cells">The cells of the list.</param>
        /// <param name="key">The identifier for the list.</param>
        public static GuiComposer AddCellList<T>(this GuiComposer composer, ElementBounds bounds, OnRequireCell<T> cellCreator, IEnumerable<T> cells = null, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementCellList<T>(composer.Api, bounds, cellCreator, cells), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the list by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the list to get.</param>
        /// <returns></returns>
        public static GuiElementCellList<T> GetCellList<T>(this GuiComposer composer, string key)
        {
            return (GuiElementCellList<T>)composer.GetElement(key);
        }
    }

}
