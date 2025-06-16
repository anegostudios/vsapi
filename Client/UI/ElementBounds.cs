using System;
using System.Collections.Generic;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Box sizing model comparable to the box sizing model of cascading style sheets using "position:relative;"
    /// Each element has a position, size, padding and margin
    /// Padding is counted towards the size of the box, whereas margin is not
    /// </summary>
    public class ElementBounds
    {
        public ElementBounds ParentBounds;
        public ElementBounds LeftOfBounds;

        public List<ElementBounds> ChildBounds = new List<ElementBounds>();

        protected bool IsWindowBounds;

        /// <summary>
        /// For debugging purposes only
        /// </summary>
        public string Code;

        public EnumDialogArea Alignment;

        /// <summary>
        /// Set the vertical and horizontal sizing, see also <see cref="ElementSizing"/>. Setting this is equal to calling <see cref="WithSizing(ElementSizing)"/>
        /// </summary>
        public ElementSizing BothSizing
        {
            set { verticalSizing = value; horizontalSizing = value; }
        }

        public ElementSizing verticalSizing;
        public ElementSizing horizontalSizing;

        // Sizing = Percentual
        //public double percentMarginX;
        //public double percentMarginY;
        public double percentPaddingX;
        public double percentPaddingY;
        public double percentX;
        public double percentY;
        public double percentWidth;
        public double percentHeight;

        // Sizing = Fixed
        public double fixedMarginX;
        public double fixedMarginY;
        public double fixedPaddingX;
        public double fixedPaddingY;
        public double fixedX;
        public double fixedY;
        public double fixedWidth;
        public double fixedHeight;
        public double fixedOffsetX;
        public double fixedOffsetY;


        // Sizing = Fit to text
        // Takes values from element


        public double absPaddingX;
        public double absPaddingY;
        public double absMarginX;
        public double absMarginY;
        public double absOffsetX;
        public double absOffsetY;

        public double absFixedX;
        public double absFixedY;
        public double absInnerWidth;
        public double absInnerHeight;

        public string Name;

        public bool AllowNoChildren;
        public bool Initialized;

        /// <summary>
        /// If set, bgDrawX/Y will be relative, instead of absolute
        /// </summary>
        public bool IsDrawingSurface;


        private bool requiresrelculation = true;
        public virtual bool RequiresRecalculation { get { return requiresrelculation; } }

        /// <summary>
        /// Position relative to it's parent element plus margin
        /// </summary>
        public virtual double relX { get { return absFixedX + absMarginX + absOffsetX; } }
        public virtual double relY { get { return absFixedY + absMarginY + absOffsetY; } }


        /// <summary>
        /// Absolute position of the element plus margin. Same as renderX but without padding
        /// </summary>
        public virtual double absX { get { return absFixedX + absMarginX + absOffsetX + ParentBounds.absPaddingX + ParentBounds.absX; } }
        public virtual double absY { get { return absFixedY + absMarginY + absOffsetY + ParentBounds.absPaddingY + ParentBounds.absY; } }

        /// <summary>
        /// Width including padding
        /// </summary>
        public virtual double OuterWidth { get { return absInnerWidth + 2 * absPaddingX; } }
        /// <summary>
        /// Height including padding
        /// </summary>
        public virtual double OuterHeight { get { return absInnerHeight + 2 * absPaddingY; } }

        public virtual int OuterWidthInt { get { return (int)OuterWidth; } }
        public virtual int OuterHeightInt { get { return (int)OuterHeight; } }

        public virtual double InnerWidth { get { return absInnerWidth; } }
        public virtual double InnerHeight { get { return absInnerHeight; } }


        /// <summary>
        /// Position where the element has to be drawn. This is a position relative to it's parent element plus margin plus padding. 
        /// </summary>
        public virtual double drawX
        {
            get { return bgDrawX + absPaddingX; }
        }
        public virtual double drawY
        {
            get { return bgDrawY + absPaddingY; }
        }

        /// <summary>
        /// Position where the background has to be drawn, this encompasses the elements padding
        /// </summary>
        public virtual double bgDrawX
        {
            get
            {
                return absFixedX + absMarginX + absOffsetX + (ParentBounds.IsDrawingSurface ? ParentBounds.absPaddingX : ParentBounds.drawX);
            }
        }
        public virtual double bgDrawY
        {
            get
            {
                return absFixedY + absMarginY + absOffsetY + (ParentBounds.IsDrawingSurface ? ParentBounds.absPaddingY : ParentBounds.drawY);
            }
        }


        public virtual double renderX { get { return absFixedX + absMarginX + absOffsetX + ParentBounds.absPaddingX + ParentBounds.renderX + renderOffsetX; } }
        public virtual double renderY { get { return absFixedY + absMarginY + absOffsetY + ParentBounds.absPaddingY + ParentBounds.renderY + renderOffsetY; } }


        public double renderOffsetX, renderOffsetY;

        public ElementBounds()
        {

        }


        public void MarkDirtyRecursive()
        {
            Initialized = false;

            foreach (ElementBounds child in ChildBounds)
            {
                if (ParentBounds == child) continue;
                if (this == child)
                {
                    throw new Exception(string.Format("Fatal: Element bounds {0} self reference itself in child bounds, this would cause a stack overflow.", this));
                }

                child.MarkDirtyRecursive();
            }
        }


        public virtual void CalcWorldBounds()
        {
            requiresrelculation = false;

            absOffsetX = scaled(fixedOffsetX);
            absOffsetY = scaled(fixedOffsetY);

            if (horizontalSizing == ElementSizing.FitToChildren && verticalSizing == ElementSizing.FitToChildren)
            {
                absFixedX = scaled(fixedX);
                absFixedY = scaled(fixedY);

                absPaddingX = scaled(fixedPaddingX);
                absPaddingY = scaled(fixedPaddingY);

                buildBoundsFromChildren();
            }
            else
            {
                switch (horizontalSizing)
                {
                    case ElementSizing.Fixed:
                        absFixedX = scaled(fixedX);
                        if (LeftOfBounds != null) absFixedX += LeftOfBounds.absFixedX + LeftOfBounds.OuterWidth;

                        absInnerWidth = scaled(fixedWidth);
                        absPaddingX = scaled(fixedPaddingX);
                        break;

                    case ElementSizing.Percentual:
                    case ElementSizing.PercentualSubstractFixed:
                        absFixedX = percentX * ParentBounds.OuterWidth;
                        absInnerWidth = percentWidth * ParentBounds.OuterWidth;
                        absPaddingX = scaled(fixedPaddingX) + percentPaddingX * ParentBounds.OuterWidth;

                        if (horizontalSizing == ElementSizing.PercentualSubstractFixed)
                        {
                            absInnerWidth -= scaled(fixedWidth);
                        }
                        break;

                    case ElementSizing.FitToChildren:
                        absFixedX = scaled(fixedX);
                        absPaddingX = scaled(fixedPaddingX);

                        buildBoundsFromChildren();
                        break;
                }

                switch (verticalSizing)
                {
                    case ElementSizing.Fixed:
                        absFixedY = scaled(fixedY);
                        absInnerHeight = scaled(fixedHeight);
                        absPaddingY = scaled(fixedPaddingY);
                        break;

                    case ElementSizing.Percentual:
                    case ElementSizing.PercentualSubstractFixed:
                        absFixedY = percentY * ParentBounds.OuterHeight;
                        absInnerHeight = percentHeight * ParentBounds.OuterHeight;
                        absPaddingY = scaled(fixedPaddingY) + percentPaddingY * ParentBounds.OuterHeight;

                        if (horizontalSizing == ElementSizing.PercentualSubstractFixed)
                        {
                            absInnerHeight -= scaled(fixedHeight);
                        }

                        break;

                    case ElementSizing.FitToChildren:
                        absFixedY = scaled(fixedY);
                        absPaddingY = scaled(fixedPaddingY);

                        buildBoundsFromChildren();
                        break;
                }
            }


            // Only if the parent element has been initialized already
            if (ParentBounds?.Initialized == true)
            {
                calcMarginFromAlignment(ParentBounds.InnerWidth, ParentBounds.InnerHeight);
            }

            Initialized = true;

            foreach (ElementBounds child in ChildBounds)
            {
                if (!child.Initialized)
                {
                    child.CalcWorldBounds();
                }
            }
        }

        void calcMarginFromAlignment(double dialogWidth, double dialogHeight)
        {
            int leftMar = 0;
            int rightMar = 0;
            if (ParentBounds?.IsWindowBounds == true)
            {
                leftMar = GuiStyle.LeftDialogMargin;
                rightMar = GuiStyle.RightDialogMargin;
            }

            switch (Alignment)
            {
                case EnumDialogArea.FixedTop:
                    break;
                case EnumDialogArea.FixedMiddle:
                    absMarginY = dialogHeight / 2 - OuterHeight / 2;
                    break;
                case EnumDialogArea.FixedBottom:
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case EnumDialogArea.CenterFixed:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    break;

                case EnumDialogArea.CenterBottom:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case EnumDialogArea.CenterMiddle:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    absMarginY = dialogHeight / 2 - OuterHeight / 2;
                    break;

                case EnumDialogArea.CenterTop:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    break;

                case EnumDialogArea.LeftBottom:
                    absMarginX = leftMar;
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case EnumDialogArea.LeftMiddle:
                    absMarginX = leftMar;
                    absMarginY = dialogHeight / 2 - absInnerHeight / 2;
                    break;
                case EnumDialogArea.LeftTop:
                    absMarginX = leftMar;
                    absMarginY = 0;
                    break;
                case EnumDialogArea.LeftFixed:
                    absMarginX = leftMar;
                    break;


                case EnumDialogArea.RightBottom:
                    absMarginX = dialogWidth - OuterWidth - rightMar;
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case EnumDialogArea.RightMiddle:
                    absMarginX = dialogWidth - OuterWidth - rightMar;
                    absMarginY = dialogHeight / 2 - OuterHeight / 2;
                    break;
                case EnumDialogArea.RightTop:
                    absMarginX = dialogWidth - OuterWidth - rightMar;
                    absMarginY = 0;
                    break;

                case EnumDialogArea.RightFixed:
                    absMarginX = dialogWidth - OuterWidth - rightMar;
                    break;

            }
        }

        void buildBoundsFromChildren()
        {
            if (ChildBounds == null || ChildBounds.Count == 0)
            {
                if (AllowNoChildren) return;
                throw new Exception("Cant build bounds from children elements, there are no children!");
            }

            double width = 0;
            double height = 0;

            foreach (ElementBounds bounds in ChildBounds)
            {
                if (bounds == this)
                {
                    throw new Exception("Endless loop detected. Bounds instance is contained itself in its ChildBounds List. Fix your code please :P");
                }

                // Alignment can only happen once the max size is known, so ignore it for now
                EnumDialogArea prevAlign = bounds.Alignment;
                bounds.Alignment = EnumDialogArea.None;

                bounds.CalcWorldBounds();

                if (bounds.horizontalSizing != ElementSizing.Percentual)
                {
                    width = Math.Max(width, bounds.OuterWidth + bounds.relX);
                }
                if (bounds.verticalSizing != ElementSizing.Percentual)
                {
                    height = Math.Max(height, bounds.OuterHeight + bounds.relY);
                }

                // Reassign actual alignment, now as we can calculate the alignment
                bounds.Alignment = prevAlign;
            }

            if (width == 0 || height == 0)
            {
                throw new Exception("Couldn't build bounds from children, there were probably no child elements using fixed sizing! (or they were size 0)");
            }

            if (horizontalSizing != ElementSizing.Fixed)
            {
                this.absInnerWidth = width;
            }

            if (verticalSizing != ElementSizing.Fixed)
            {
                this.absInnerHeight = height;
            }
        }


        public static double scaled(double value)
        {
            return value * RuntimeEnv.GUIScale;
        }


        public ElementBounds WithScale(double factor)
        {
            fixedX *= factor;
            fixedY *= factor;
            fixedWidth *= factor;
            fixedHeight *= factor;
            absPaddingX *= factor;
            absPaddingY *= factor;
            absMarginX *= factor;
            absMarginY *= factor;

            percentPaddingX *= factor;
            percentPaddingY *= factor;
            percentX *= factor;
            percentY *= factor;
            percentWidth *= factor;
            percentHeight *= factor;


            return this;
        }

        public ElementBounds WithChildren(params ElementBounds[] bounds)
        {
            foreach (ElementBounds bound in bounds)
            {
                WithChild(bound);
            }
            return this;
        }

        public ElementBounds WithChild(ElementBounds bounds)
        {
            if (!ChildBounds.Contains(bounds))
            {
                ChildBounds.Add(bounds);
            }


            if (bounds.ParentBounds == null)
            {
                bounds.ParentBounds = this;
            }

            return this;
        }

        public ElementBounds RightOf(ElementBounds leftBounds, double leftMargin = 0)
        {
            this.LeftOfBounds = leftBounds;
            this.fixedX = leftMargin;
            return this;
        }


        /// <summary>
        /// Returns the relative coordinate if supplied coordinate is inside the bounds, otherwise null
        /// </summary>
        /// <param name="absPointX"></param>
        /// <param name="absPointY"></param>
        /// <returns></returns>
        public Vec2d PositionInside(int absPointX, int absPointY)
        {
            if (PointInside(absPointX, absPointY))
            {
                return new Vec2d(absPointX - absX, absPointY - absY);
            }

            return null;
        }

        /// <summary>
        /// Returns true if supplied coordinate is inside the bounds
        /// </summary>
        /// <param name="absPointX"></param>
        /// <param name="absPointY"></param>
        /// <returns></returns>
        public bool PointInside(int absPointX, int absPointY)
        {
            return
                absPointX >= absX &&
                absPointX <= absX + OuterWidth &&
                absPointY >= absY &&
                absPointY <= absY + OuterHeight
            ;
        }


        /// <summary>
        /// Returns true if supplied coordinate is inside the bounds
        /// </summary>
        /// <param name="absPointX"></param>
        /// <param name="absPointY"></param>
        /// <returns></returns>
        public bool PointInside(double absPointX, double absPointY)
        {
            return
                absPointX >= absX &&
                absPointX <= absX + OuterWidth &&
                absPointY >= absY &&
                absPointY <= absY + OuterHeight
            ;
        }


        /// <summary>
        /// Checks if the bounds is at least partially inside it's parent bounds by checking if any of the 4 corner points is inside
        /// </summary>
        /// <returns></returns>
        public bool PartiallyInside(ElementBounds boundingBounds)
        {
            return
                boundingBounds.PointInside(absX, absY) ||
                boundingBounds.PointInside(absX + OuterWidth, absY) ||
                boundingBounds.PointInside(absX, absY + OuterHeight) ||
                boundingBounds.PointInside(absX + OuterWidth, absY + OuterHeight)
            ;
        }



        /// <summary>
        /// Makes a copy of the current bounds but leaves the position and 0. Sets the parent to the calling bounds
        /// </summary>
        /// <returns></returns>
        public ElementBounds CopyOnlySize()
        {
            return new ElementBounds()
            {
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedHeight = fixedHeight,
                fixedWidth = fixedWidth,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                ParentBounds = Empty.WithSizing(ElementSizing.FitToChildren)
            };
        }

        /// <summary>
        /// Makes a copy of the current bounds but leaves the position and padding at 0. Sets the same parent as the current one.
        /// </summary>
        /// <returns></returns>
        public ElementBounds CopyOffsetedSibling(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX,
                fixedY = fixedY + fixedDeltaY,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                ParentBounds = ParentBounds
            };
        }

        /// <summary>
        /// Makes a copy of the current bounds but leaves the position and padding at 0. Sets the same parent as the current one.
        /// </summary>
        /// <returns></returns>
        public ElementBounds BelowCopy(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                percentX = percentX,
                percentY = percentY = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX,
                fixedY = fixedY + fixedDeltaY + fixedHeight + fixedPaddingY * 2,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                ParentBounds = ParentBounds,
            };
        }

        /// <summary>
        /// Create a flat copy of the element with a fixed position offset that causes it to be right of the original element
        /// </summary>
        /// <param name="fixedDeltaX"></param>
        /// <param name="fixedDeltaY"></param>
        /// <param name="fixedDeltaWidth"></param>
        /// <param name="fixedDeltaHeight"></param>
        /// <returns></returns>
        public ElementBounds RightCopy(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                percentX = percentX,
                percentY = percentY = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX + fixedWidth + fixedPaddingX * 2,
                fixedY = fixedY + fixedDeltaY,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                ParentBounds = ParentBounds,
            };
        }



        /// <summary>
        /// Creates a clone of the bounds but without child elements
        /// </summary>
        /// <returns></returns>
        public ElementBounds FlatCopy()
        {
            return new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = fixedWidth,
                fixedHeight = fixedHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                ParentBounds = ParentBounds
            };
        }





        public ElementBounds ForkChild()
        {
            return ForkChildOffseted();
        }

        public ElementBounds ForkChildOffseted(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX,
                fixedY = fixedY + fixedDeltaY,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                ParentBounds = this
            };

        }

        /// <summary>
        /// Creates a new elements bounds which acts as the parent bounds of the current bounds. It will also arrange the fixedX/Y and Width/Height coords of both bounds so that the parent bounds surrounds the child bounds with given spacings. Uses fixed coords only!
        /// </summary>
        /// <param name="leftSpacing"></param>
        /// <param name="topSpacing"></param>
        /// <param name="rightSpacing"></param>
        /// <param name="bottomSpacing"></param>
        /// <returns></returns>
        public ElementBounds ForkBoundingParent(double leftSpacing = 0, double topSpacing = 0, double rightSpacing = 0, double bottomSpacing = 0)
        {
            ElementBounds bounds = new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedWidth = fixedWidth + 2 * fixedPaddingX + leftSpacing + rightSpacing,
                fixedHeight = fixedHeight + 2 * fixedPaddingY + topSpacing + bottomSpacing,
                fixedX = fixedX,
                fixedY = fixedY,
                percentHeight = percentHeight,
                percentWidth = percentWidth
            };

            fixedX = leftSpacing;
            fixedY = topSpacing;
            percentWidth = 1;
            percentHeight = 1;

            ParentBounds = bounds;

            return bounds;
        }



        /// <summary>
        /// Creates a new elements bounds which acts as the child bounds of the current bounds. It will also arrange the fixedX/Y and Width/Height coords of both bounds so that the parent bounds surrounds the child bounds with given spacings. Uses fixed coords only!
        /// </summary>
        /// <param name="leftSpacing"></param>
        /// <param name="topSpacing"></param>
        /// <param name="rightSpacing"></param>
        /// <param name="bottomSpacing"></param>
        /// <returns></returns>
        public ElementBounds ForkContainingChild(double leftSpacing = 0, double topSpacing = 0, double rightSpacing = 0, double bottomSpacing = 0)
        {
            ElementBounds bounds = new ElementBounds()
            {
                Alignment = Alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedWidth = fixedWidth - 2 * fixedPaddingX - leftSpacing - rightSpacing,
                fixedHeight = fixedHeight - 2 * fixedPaddingY - topSpacing - bottomSpacing,
                fixedX = fixedX,
                fixedY = fixedY,
                percentHeight = percentHeight,
                percentWidth = percentWidth
            };

            bounds.fixedX = leftSpacing;
            bounds.fixedY = topSpacing;
            percentWidth = 1;
            percentHeight = 1;

            ChildBounds.Add(bounds);
            bounds.ParentBounds = this;

            return bounds;
        }



        public override string ToString()
        {
            return absX + "/" + absY + " -> " + (absX + OuterWidth) + " / " + (absY + OuterHeight);
        }

        /// <summary>
        /// Set the fixed y-position to "refBounds.fixedY + refBounds.fixedHeight + spacing" so that the bounds will be under the reference bounds
        /// </summary>
        /// <param name="refBounds"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        public ElementBounds FixedUnder(ElementBounds refBounds, double spacing = 0)
        {
            fixedY += refBounds.fixedY + refBounds.fixedHeight + spacing;
            return this;
        }

        /// <summary>
        /// Set the fixed x-position to "refBounds.fixedX + refBounds.fixedWidth + leftSpacing" so that the bounds will be right of reference bounds
        /// </summary>
        /// <param name="refBounds"></param>
        /// <param name="leftSpacing"></param>
        /// <returns></returns>
        public ElementBounds FixedRightOf(ElementBounds refBounds, double leftSpacing = 0)
        {
            fixedX = refBounds.fixedX + refBounds.fixedWidth + leftSpacing;
            return this;
        }

        /// <summary>
        /// Set the fixed x-position to "refBounds.fixedX - fixedWith - rightSpacing" so that the element will be left of reference bounds
        /// </summary>
        /// <param name="refBounds"></param>
        /// <param name="rightSpacing"></param>
        /// <returns></returns>
        public ElementBounds FixedLeftOf(ElementBounds refBounds, double rightSpacing = 0)
        {
            fixedX = refBounds.fixedX - fixedWidth - rightSpacing;
            return this;
        }

        /// <summary>
        /// Set the fixed width and fixed height values
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public ElementBounds WithFixedSize(double width, double height)
        {
            fixedWidth = width;
            fixedHeight = height;
            return this;
        }

        /// <summary>
        /// Set the width property
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public ElementBounds WithFixedWidth(double width)
        {
            fixedWidth = width;
            return this;
        }


        /// <summary>
        /// Set the height property
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public ElementBounds WithFixedHeight(double height)
        {
            fixedHeight = height;
            return this;
        }

        /// <summary>
        /// Set the alignment property
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public ElementBounds WithAlignment(EnumDialogArea alignment)
        {
            this.Alignment = alignment;
            return this;
        }

        /// <summary>
        /// Set the vertical and horizontal sizing property to the same value. See also <seealso cref="ElementSizing"/>.
        /// </summary>
        /// <param name="sizing"></param>
        /// <returns></returns>
        public ElementBounds WithSizing(ElementSizing sizing)
        {
            this.verticalSizing = sizing;
            this.horizontalSizing = sizing;
            return this;
        }

        /// <summary>
        /// Set the vertical and horizontal sizing properties individually. See also <seealso cref="ElementSizing"/>.
        /// </summary>
        /// <param name="horizontalSizing"></param>
        /// <param name="verticalSizing"></param>
        /// <returns></returns>
        public ElementBounds WithSizing(ElementSizing horizontalSizing, ElementSizing verticalSizing)
        {
            this.verticalSizing = verticalSizing;
            this.horizontalSizing = horizontalSizing;
            return this;
        }

        /// <summary>
        /// Sets a new fixed margin (pad = top/right/down/left margin)
        /// </summary>
        /// <param name="pad"></param>
        /// <returns></returns>
        public ElementBounds WithFixedMargin(double pad)
        {
            this.fixedMarginX = pad;
            this.fixedMarginY = pad;
            return this;
        }

        /// <summary>
        /// Sets a new fixed margin (pad = top/right/down/left margin)
        /// </summary>
        /// <param name="padH"></param>
        /// <param name="padV"></param>
        /// <returns></returns>
        public ElementBounds WithFixedMargin(double padH, double padV)
        {
            this.fixedMarginX = padH;
            this.fixedMarginY = padV;
            return this;
        }



        /// <summary>
        /// Sets a new fixed padding (pad = top/right/down/left padding)
        /// </summary>
        /// <param name="pad"></param>
        /// <returns></returns>
        public ElementBounds WithFixedPadding(double pad)
        {
            this.fixedPaddingX = pad;
            this.fixedPaddingY = pad;
            return this;
        }

        /// <summary>
        /// Sets a new fixed padding (x = left/right, y = top/down padding)
        /// </summary>
        /// <param name="leftRight"></param>
        /// <param name="upDown"></param>
        /// <returns></returns>
        public ElementBounds WithFixedPadding(double leftRight, double upDown)
        {
            this.fixedPaddingX = leftRight;
            this.fixedPaddingY = upDown;
            return this;
        }


        /// <summary>
        /// Sets a new fixed offset that is applied after element alignment. So you could i.e. horizontally center an element and then offset in x direction  from there using this method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ElementBounds WithFixedAlignmentOffset(double x, double y)
        {
            this.fixedOffsetX = x;
            this.fixedOffsetY = y;
            return this;
        }


        /// <summary>
        /// Sets a new fixed offset that is used during element alignment.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ElementBounds WithFixedPosition(double x, double y)
        {
            this.fixedX = x;
            this.fixedY = y;
            return this;
        }

        /// <summary>
        /// Sets a new fixed offset that is used during element alignment.
        /// </summary>
        /// <param name="offx"></param>
        /// <param name="offy"></param>
        /// <returns></returns>
        public ElementBounds WithFixedOffset(double offx, double offy)
        {
            this.fixedX += offx;
            this.fixedY += offy;
            return this;
        }


        /// <summary>
        /// Shrinks the current width/height by a fixed value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ElementBounds FixedShrink(double amount)
        {
            fixedWidth -= amount;
            fixedHeight -= amount;

            return this;
        }

        /// <summary>
        /// Grows the current width/height by a fixed value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ElementBounds FixedGrow(double amount)
        {
            fixedWidth += amount;
            fixedHeight += amount;

            return this;
        }

        /// <summary>
        /// Grows the current width/height by a fixed value
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public ElementBounds FixedGrow(double width, double height)
        {
            fixedWidth += width;
            fixedHeight += height;

            return this;
        }


        /// <summary>
        /// Sets the parent of the bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public ElementBounds WithParent(ElementBounds bounds)
        {
            ParentBounds = bounds;
            return this;
        }

        /// <summary>
        /// Creates a new bounds using FitToChildren and sets that as bound parent. This is usefull if you want to draw elements that are not part of the dialog
        /// </summary>
        /// <returns></returns>
        public ElementBounds WithEmptyParent()
        {
            ParentBounds = Empty;
            return this;
        }


        /// <summary>
        /// Create a new ElementBounds instance with given fixed x/y position and width/height 0
        /// </summary>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <returns></returns>
        public static ElementBounds Fixed(int fixedX, int fixedY)
        {
            return Fixed(fixedX, fixedY, 0, 0);
        }

        /// <summary>
        /// Quick Method to create a new ElementBounds instance that fills 100% of its parent bounds. Useful for backgrounds.
        /// </summary>
        public static ElementBounds Fill
        {
            get
            {
                return new ElementBounds() { Alignment = EnumDialogArea.None, BothSizing = ElementSizing.Percentual, percentWidth = 1, percentHeight = 1 };
            }
        }


        public static ElementBounds FixedPos(EnumDialogArea alignment, double fixedX, double fixedY)
        {
            return new ElementBounds() { Alignment = alignment, BothSizing = ElementSizing.Fixed, fixedX = fixedX, fixedY = fixedY };
        }





        /// <summary>
        /// Quick method to create a new ElementBounds instance that uses fixed element sizing. The X/Y Coordinates are left at 0. 
        /// </summary>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds FixedSize(double fixedWidth, double fixedHeight)
        {
            return new ElementBounds() { Alignment = EnumDialogArea.None, fixedWidth = fixedWidth, fixedHeight = fixedHeight, BothSizing = ElementSizing.Fixed };
        }

        /// <summary>
        /// Quick method to create a new ElementBounds instance that uses fixed element sizing. The X/Y Coordinates are left at 0. 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds FixedSize(EnumDialogArea alignment, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds() { Alignment = alignment, fixedWidth = fixedWidth, fixedHeight = fixedHeight, BothSizing = ElementSizing.Fixed };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses fixed element sizing.
        /// </summary>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds Fixed(double fixedX, double fixedY, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds() { fixedX = fixedX, fixedY = fixedY, fixedWidth = fixedWidth, fixedHeight = fixedHeight, BothSizing = ElementSizing.Fixed };
        }


        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses fixed element sizing.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedOffsetX"></param>
        /// <param name="fixedOffsetY"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds FixedOffseted(EnumDialogArea alignment, double fixedOffsetX, double fixedOffsetY, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedWidth = fixedWidth,
                fixedHeight = fixedHeight,
                BothSizing = ElementSizing.Fixed
            };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses fixed element sizing.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds Fixed(EnumDialogArea alignment, double fixedX, double fixedY, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = fixedWidth,
                fixedHeight = fixedHeight,
                BothSizing = ElementSizing.Fixed
            };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses percentual element sizing, e.g. setting percentWidth to 0.5 will set the width of the bounds to 50% of its parent width
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="percentWidth"></param>
        /// <param name="percentHeight"></param>
        /// <returns></returns>
        public static ElementBounds Percentual(EnumDialogArea alignment, double percentWidth, double percentHeight)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                percentWidth = percentWidth,
                percentHeight = percentHeight,
                BothSizing = ElementSizing.Percentual
            };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses percentual element sizing, e.g. setting percentWidth to 0.5 will set the width of the bounds to 50% of its parent width
        /// </summary>
        /// <param name="percentX"></param>
        /// <param name="percentY"></param>
        /// <param name="percentWidth"></param>
        /// <param name="percentHeight"></param>
        /// <returns></returns>
        public static ElementBounds Percentual(double percentX, double percentY, double percentWidth, double percentHeight)
        {
            return new ElementBounds()
            {
                percentX = percentX,
                percentY = percentY,
                percentWidth = percentWidth,
                percentHeight = percentHeight,
                BothSizing = ElementSizing.Percentual
            };
        }


        /// <summary>
        /// Create a special instance of type <see cref="ElementEmptyBounds"/> whose position is 0 and size 1. It's often used for other bounds that need a static, unchanging parent bounds
        /// </summary>
        public static ElementBounds Empty
        {
            get
            {
                return new ElementEmptyBounds();
            }
        }

    }
}
