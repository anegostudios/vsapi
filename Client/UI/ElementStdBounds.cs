
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// Just some default sizings for various ui elements
    /// </summary>
    public static partial class ElementStdBounds
    {
        public static int mainMenuUnscaledLogoSize = 230;
        public static int mainMenuUnscaledLogoHorPadding = 30;
        public static int mainMenuUnscaledLogoVerPadding = 10;
        public static int mainMenuUnscaledWoodPlankWidth = 13;

        /// <summary>
        /// Quick method to create a new ElementBounds instance that uses fixed element sizing. The X/Y Coordinates are left at 0. 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static ElementBounds Statbar(EnumDialogArea alignment, double width)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                fixedWidth = width,
                fixedHeight = GuiElementStatbar.DefaultHeight,
                BothSizing = ElementSizing.Fixed
            };
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance that is positioned at the screen center and sized accordingly to fit all it's child elements
        /// </summary>
        public static ElementBounds AutosizedMainDialog
        {
            get
            {
                return new ElementBounds() { Alignment = EnumDialogArea.CenterMiddle, BothSizing = ElementSizing.FitToChildren };
            }
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds MainScreenRightPart()
        {
            ElementBounds bounds = ElementBounds.Percentual(EnumDialogArea.RightMiddle, 1, 1);
            bounds.horizontalSizing = ElementSizing.PercentualSubstractFixed;
            bounds.fixedWidth = mainMenuUnscaledLogoSize + mainMenuUnscaledLogoHorPadding * 2 + mainMenuUnscaledWoodPlankWidth;
            return bounds;
        }



        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds AutosizedMainDialogAtPos(double fixedY)
        {
            return new ElementBounds().WithSizing(ElementSizing.FitToChildren).WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0, fixedY);
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds DialogBackground()
        {
            return new ElementBounds().WithSizing(ElementSizing.FitToChildren).WithFixedPadding(GuiStyle.ElementToDialogPadding);
        }

        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds DialogBackground(double horPadding, double verPadding)
        {
            return new ElementBounds().WithSizing(ElementSizing.FitToChildren).WithFixedPadding(horPadding, verPadding);
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance to create a menu consiting of one ore more vertically arranged and horizontally centered buttons in a grid. The y position is calculated using rowIndex * 80. 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static ElementBounds MenuButton(float rowIndex, EnumDialogArea alignment = EnumDialogArea.CenterFixed)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                BothSizing = ElementSizing.Fixed,
                fixedY = 80 * rowIndex,
                fixedPaddingX = 2,
                fixedPaddingY = 2
            };
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance to create a menu consiting of one ore more vertically arranged and horizontally centered buttons in a grid. The y position is calculated using rowIndex * 80. 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="padding"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static ElementBounds Rowed(float rowIndex, double padding, EnumDialogArea alignment = EnumDialogArea.None)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                BothSizing = ElementSizing.Fixed,
                fixedY = 70 * rowIndex,
                fixedPaddingX = padding,
                fixedPaddingY = padding
            };
        }




        /// <summary>
        /// Quick Method to create a new ElementBounds instance that is currently used for Signs (e.g. graphics options)
        /// </summary>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <param name="fixedWith"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds Sign(double fixedX, double fixedY, double fixedWith, double fixedHeight = 80)
        {
            return new ElementBounds()
            {
                Alignment = EnumDialogArea.None,
                BothSizing = ElementSizing.Fixed,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = fixedWith,
                fixedHeight = fixedHeight
            };
        }


        public static ElementBounds Slider(double x, double y, double width)
        {
            return new ElementBounds()
            {
                Alignment = EnumDialogArea.None,
                BothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = width,
                fixedHeight = GuiElementSlider.unscaledHeight
            };
        }

        /// <summary>
        /// Creates a scrollbar right of given element bounds, requires the left element to be using fixed element positioning
        /// </summary>
        /// <param name="leftElement"></param>
        /// <returns></returns>
        public static ElementBounds VerticalScrollbar(ElementBounds leftElement)
        {
            return new ElementBounds()
            {
                Alignment = leftElement.Alignment,
                BothSizing = ElementSizing.Fixed,
                fixedOffsetX = leftElement.fixedX + leftElement.fixedWidth + 3,
                fixedOffsetY = leftElement.fixedY,
                fixedPaddingX = GuiElementScrollbar.DeafultScrollbarPadding,
                fixedWidth = GuiElementScrollbar.DefaultScrollbarWidth,
                fixedHeight = leftElement.fixedHeight,
                percentHeight = leftElement.percentHeight
            };
        }

        public static ElementBounds Slot(double x = 0, double y = 0)
        {
            return new ElementBounds()
            {
                Alignment = EnumDialogArea.None,
                BothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = GuiElementPassiveItemSlot.unscaledSlotSize,
                fixedHeight = GuiElementPassiveItemSlot.unscaledSlotSize
            };
        }


        public static ElementBounds SlotGrid(EnumDialogArea alignment, double x, double y, int cols, int rows)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                BothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = cols * (GuiElementPassiveItemSlot.unscaledSlotSize + GuiElementItemSlotGrid.unscaledSlotPadding),
                fixedHeight = rows * (GuiElementPassiveItemSlot.unscaledSlotSize + GuiElementItemSlotGrid.unscaledSlotPadding)
            };
        }

        public static ElementBounds ToggleButton(double fixedX, double fixedY, double width, double height)
        {
            return new ElementBounds()
            {
                Alignment = EnumDialogArea.None,
                BothSizing = ElementSizing.Fixed,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = width,
                fixedHeight = height
            };
        }


        public static ElementBounds TitleBar()
        {
            return new ElementBounds()
            {
                Alignment = EnumDialogArea.None,
                verticalSizing = ElementSizing.Fixed,
                horizontalSizing = ElementSizing.Percentual,
                percentWidth = 1,
                fixedHeight = (float)GuiStyle.TitleBarHeight
            };
        }
    }
}
