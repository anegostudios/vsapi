using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public static partial class ElementStdBounds
    {
        /// <summary>
        /// Quick method to create a new ElementBounds instance that uses fixed element sizing. The X/Y Coordinates are left at 0. 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        internal static ElementBounds Statbar( ElementAlignment alignment, double width)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                fixedWidth = width,
                fixedHeight = GuiElementStatbar.DefaultHeight,
                bothSizing = ElementSizing.Fixed
            };
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance that is positioned at the screen center and sized accordingly to fit all it's child elements
        /// </summary>
        public static ElementBounds AutosizedMainDialog
        {
            get
            {
                return new ElementBounds() { alignment = ElementAlignment.CenterMiddle, bothSizing = ElementSizing.FitToChildren };
            }
        }


        public static int mainMenuUnscaledLogoSize = 280;
        public static int mainMenuUnscaledLogoHorPadding = 30;
        public static int mainMenuUnscaledLogoVerPadding = 10;
        public static int mainMenuUnscaledWoodPlankWidth = 13;

        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds MainScreenRightPart()
        {
            ElementBounds bounds = ElementBounds.Percentual(ElementAlignment.RightMiddle, 1, 1);
            bounds.horizontalSizing = ElementSizing.PercentualSubstractFixed;
            bounds.fixedWidth = mainMenuUnscaledLogoSize + mainMenuUnscaledLogoHorPadding * 2 + mainMenuUnscaledWoodPlankWidth;
            return bounds;
        }



        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds AutosizedMainDialogAtPos(double fixedY)
        {
            return new ElementBounds().WithSizing(ElementSizing.FitToChildren).WithAlignment(ElementAlignment.CenterFixed).WithFixedPosition(0, fixedY);
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance that draws a background for a dialog
        /// </summary>
        public static ElementBounds DialogBackground()
        {
            return new ElementBounds().WithSizing(ElementSizing.FitToChildren).WithFixedPadding(ElementGeometrics.ElementToDialogPadding);
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
        /// <returns></returns>
        public static ElementBounds MenuButton(float rowIndex, ElementAlignment alignment = ElementAlignment.CenterFixed)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                bothSizing = ElementSizing.Fixed,
                fixedY = 80 * rowIndex,
                fixedPaddingX = 2,
                fixedPaddingY = 2
            };
        }


        /// <summary>
        /// Quick Method to create a new ElementBounds instance to create a menu consiting of one ore more vertically arranged and horizontally centered buttons in a grid. The y position is calculated using rowIndex * 80. 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static ElementBounds Rowed(float rowIndex, double padding, ElementAlignment alignment = ElementAlignment.None)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                bothSizing = ElementSizing.Fixed,
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
                alignment = ElementAlignment.None,
                bothSizing = ElementSizing.Fixed,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = fixedWith,
                fixedHeight = fixedHeight
            };
        }


        public static ElementBounds Lever(double x, double y)
        {
            return new ElementBounds()
            {
                alignment = ElementAlignment.None,
                bothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = GuiElementLever.unscaledLeverWidth,
                fixedHeight = GuiElementLever.unscaledLeverHeight + GuiElementLever.unscaledLampYOffset + 20
            };
        }

        public static ElementBounds Slider(double x, double y, double width)
        {
            return new ElementBounds()
            {
                alignment = ElementAlignment.None,
                bothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = width,
                fixedHeight = GuiElementSlider.unscaledHeight
            };
        }

        /// <summary>
        /// Creates a scrollbar right of given element bounds, requires the left element to be using fixed element positioning
        /// </summary>
        /// <param name="leftBounds"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ElementBounds VerticalScrollbar(ElementBounds leftElement)
        {
            return new ElementBounds()
            {
                alignment = leftElement.alignment,
                bothSizing = ElementSizing.Fixed,
                fixedOffsetX = leftElement.fixedX + leftElement.fixedWidth + 3,
                fixedOffsetY = leftElement.fixedY,
                fixedPaddingX = GuiElementScrollbar.scrollbarPadding,
                fixedWidth = GuiElementScrollbar.scrollbarWidth,
                fixedHeight = leftElement.fixedHeight,
                percentHeight = leftElement.percentHeight
            };
        }

        public static ElementBounds Slot(double x = 0, double y = 0)
        {
            return new ElementBounds()
            {
                alignment = ElementAlignment.None,
                bothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = GuiElementPassiveItemSlot.unscaledSlotSize,
                fixedHeight = GuiElementPassiveItemSlot.unscaledSlotSize
            };
        }


        public static ElementBounds SlotGrid(ElementAlignment alignment, double x, double y, int cols, int rows)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                bothSizing = ElementSizing.Fixed,
                fixedX = x,
                fixedY = y,
                fixedWidth = cols * (GuiElementPassiveItemSlot.unscaledSlotSize + GuiElementItemSlotGrid.unscaledSlotPadding),
                fixedHeight = rows * (GuiElementPassiveItemSlot.unscaledSlotSize + GuiElementItemSlotGrid.unscaledSlotPadding)
            };
        }

        internal static ElementBounds ToggleButton(double fixedX, double fixedY, double width, double height)
        {
            return new ElementBounds()
            {
                alignment = ElementAlignment.None,
                bothSizing = ElementSizing.Fixed,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = width,
                fixedHeight = height
            };
        }


        internal static ElementBounds TitleBar()
        {
            return new ElementBounds()
            {
                alignment = ElementAlignment.None,
                verticalSizing = ElementSizing.Fixed,
                horizontalSizing = ElementSizing.Percentual,
                percentWidth = 1,
                fixedHeight = 30
            };
        }
    }
}
