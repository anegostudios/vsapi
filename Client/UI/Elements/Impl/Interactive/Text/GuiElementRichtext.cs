using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class GuiElementRichtext : GuiElement
    {
        RichTextComponentBase[] Components;
        TextFlowPath[] flowPath;

        public bool Debug = false;
        

        public GuiElementRichtext(ICoreClientAPI capi, RichTextComponentBase[] components, ElementBounds bounds) : base(capi, bounds)
        {
            this.Components = components;
        }


        public void CalcHeightAndPositions()
        {
            Bounds.CalcWorldBounds();

            double posX = 0;
            double posY = 0;

            List<int> currentLine = new List<int>();
            List<TextFlowPath> flowPathList = new List<TextFlowPath>();

            flowPathList.Add(new TextFlowPath(Bounds.InnerWidth));

            double lineHeight = 0;
            double ascentHeight = 0;
            
            for (int i = 0; i < Components.Length; i++)
            {
                RichTextComponentBase comp = Components[i];

                bool didLineBreak = comp.CalcBounds(flowPathList.ToArray(), lineHeight, posX, posY);

                posX += comp.PaddingLeft;

                if (comp.Float == EnumFloat.None)
                {
                    posX = 0;
                    posY += Math.Max(lineHeight, comp.BoundsPerLine[0].Height) + comp.MarginTop;
                    currentLine.Clear();
                    lineHeight = 0;
                    ascentHeight = 0;
                    continue;
                }

                if (didLineBreak)
                {
                    lineHeight = Math.Max(lineHeight, comp.BoundsPerLine[0].Height);
                    ascentHeight = Math.Max(ascentHeight, comp.BoundsPerLine[0].AscentOrHeight);

                    // All previous elements in this line might need to have their Y pos adjusted due to a larger element in the line
                    foreach (int index in currentLine)
                    {
                        RichTextComponentBase lineComp = Components[index];
                        Rectangled lastLineBounds = lineComp.BoundsPerLine[lineComp.BoundsPerLine.Length - 1];
                        
                        if (lineComp.VerticalAlign == EnumVerticalAlign.Bottom)
                        {
                            lastLineBounds.Y += ascentHeight - lineComp.BoundsPerLine[lineComp.BoundsPerLine.Length - 1].AscentOrHeight;
                        }
                        if (lineComp.VerticalAlign == EnumVerticalAlign.Middle)
                        {
                            lastLineBounds.Y += ascentHeight - lineComp.BoundsPerLine[lineComp.BoundsPerLine.Length - 1].AscentOrHeight / 2;
                        }
                    }

                    // The current element that was still on the same line as well
                    if (comp.VerticalAlign == EnumVerticalAlign.Bottom)
                    {
                        foreach (var val in comp.BoundsPerLine) val.Y += ascentHeight - comp.BoundsPerLine[0].AscentOrHeight;

                    }
                    if (comp.VerticalAlign == EnumVerticalAlign.Middle)
                    {
                        foreach (var val in comp.BoundsPerLine) val.Y += ascentHeight - comp.BoundsPerLine[0].AscentOrHeight / 2;
                    }
                    
                    currentLine.Clear();
                    currentLine.Add(i);

                    posY += lineHeight;
                    for (int k = 1; k < comp.BoundsPerLine.Length - 1; k++) posY += comp.BoundsPerLine[k].Height;

                    posX = comp.BoundsPerLine[comp.BoundsPerLine.Length - 1].Width;
                    if (comp.BoundsPerLine[comp.BoundsPerLine.Length - 1].Width > 0)
                    {
                        lineHeight = comp.BoundsPerLine[comp.BoundsPerLine.Length - 1].Height;
                        ascentHeight = comp.BoundsPerLine[comp.BoundsPerLine.Length - 1].AscentOrHeight;
                    } else
                    {
                        lineHeight = 0;
                        ascentHeight = 0;
                    }

                } else
                {
                    if (comp.Float == EnumFloat.Inline && comp.BoundsPerLine.Length > 0)
                    {
                        posX += comp.BoundsPerLine[0].Width;
                        lineHeight = Math.Max(comp.BoundsPerLine[0].Height, lineHeight);
                        ascentHeight = Math.Max(comp.BoundsPerLine[0].AscentOrHeight, ascentHeight);                        
                        currentLine.Add(i);
                    }
                }

                if (comp.Float != EnumFloat.Inline)
                {
                    ConstrainTextFlowPath(flowPathList, posY, comp.BoundsPerLine[0], comp.Float);
                }
            }



            if (Components.Length > 0)
            {
                Bounds.fixedHeight = (posY + lineHeight) / RuntimeEnv.GUIScale;
            }
            

            foreach (int index in currentLine)
            {
                RichTextComponentBase lineComp = Components[index];
                Rectangled lastLineBounds = lineComp.BoundsPerLine[lineComp.BoundsPerLine.Length - 1];

                if (lineComp.VerticalAlign == EnumVerticalAlign.Bottom)
                {
                    lastLineBounds.Y += ascentHeight - lineComp.BoundsPerLine[lineComp.BoundsPerLine.Length - 1].AscentOrHeight;
                }
                if (lineComp.VerticalAlign == EnumVerticalAlign.Middle)
                {
                    lastLineBounds.Y += ascentHeight - lineComp.BoundsPerLine[lineComp.BoundsPerLine.Length - 1].AscentOrHeight / 2;
                }
            }

            this.flowPath = flowPathList.ToArray();
        }



        private double getLeftIndentAt(List<TextFlowPath> flowPath, double posY)
        {
            for (int i = 0; i < flowPath.Count; i++)
            {
                TextFlowPath tfp = flowPath[i];
                if (tfp.Y1 <= posY && tfp.Y2 >= posY) return tfp.X1;
            }

            return 0;
        }


        private void ConstrainTextFlowPath(List<TextFlowPath> flowPath, double posY, Rectangled rect, EnumFloat elementFloat)
        {
            double x1 = elementFloat == EnumFloat.Left ? rect.Width : 0;
            double x2 = elementFloat == EnumFloat.Right ? Bounds.InnerWidth - rect.Width : Bounds.InnerWidth;

            double remainingHeight = rect.Height;

            for (int i = 0; i < flowPath.Count; i++)
            {
                TextFlowPath tfp = flowPath[i];

                if (tfp.Y2 <= posY) continue; // we already passed this one

                double hereX1 = Math.Max(x1, tfp.X1);
                double hereX2 = Math.Min(x2, tfp.X2);


                // Current bounds are taller, let's make a split and insert ours
                if (tfp.Y2 > posY + rect.Height)
                {
                    // Already more contrained, don't touch
                    if (x1 <= tfp.X1 && x2 >= tfp.X2) continue;

                    if (i == 0)
                    {
                        // We're at the begining, so don't need a "before" element
                        flowPath[i] = new TextFlowPath(hereX1, posY, hereX2, posY + rect.Height);
                        flowPath.Insert(i + 1, new TextFlowPath(tfp.X1, posY + rect.Height, tfp.X2, tfp.Y2));
                    } else
                    {
                        flowPath[i] = new TextFlowPath(tfp.X1, tfp.Y1, tfp.X2, posY);
                        flowPath.Insert(i + 1, new TextFlowPath(tfp.X1, posY + rect.Height, tfp.X2, tfp.Y2));
                        flowPath.Insert(i, new TextFlowPath(hereX1, posY, hereX2, posY + rect.Height));
                    }
                    
                    remainingHeight = 0;
                    break;
                } else

                // Current bounds are shorter, let's update it
                {
                    flowPath[i].X1 = hereX1;
                    flowPath[i].X2 = hereX2;
                    remainingHeight -= tfp.Y2 - posY;
                }
            }

            if (remainingHeight > 0)
            {
                flowPath.Add(new TextFlowPath(x1, posY, x2, posY + remainingHeight));
            }
        }



        public override void BeforeCalcBounds()
        {
            CalcHeightAndPositions();
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctx.Save();
            Matrix m = ctx.Matrix;
            m.Translate(Bounds.drawX, Bounds.drawY);
            ctx.Matrix = m;

            for (int i = 0; i < Components.Length; i++)
            { 
                Components[i].ComposeElements(ctx, surface);

                if (Debug)
                {
                    ctx.LineWidth = 1f;
                    if (Components[i] is ClearFloatTextComponent)
                    {
                        ctx.SetSourceRGBA(0, 0, 1, 0.5);
                    } else
                    {
                        ctx.SetSourceRGBA(0, 0, 0, 0.5);
                    }

                    foreach (var val in Components[i].BoundsPerLine)
                    {
                        ctx.Rectangle(val.X, val.Y, val.Width, val.Height);
                        ctx.Stroke();
                    }
                } 
            }

            ctx.Restore();

            //surface.WriteToPng("richtext.png");
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            bool found = false;
            int relx = (int)(api.Input.MouseX - Bounds.absX);
            int rely = (int)(api.Input.MouseY - Bounds.absY);

            for (int i = 0; i < Components.Length; i++) {
                RichTextComponentBase comp = Components[i];

                comp.RenderInteractiveElements(api, deltaTime, Bounds.renderX, Bounds.renderY);

                for (int j = 0; !found && j < comp.BoundsPerLine.Length; j++)
                {
                    LineRectangled rec = comp.BoundsPerLine[j];

                    if (rec.PointInside(relx, rely))
                    {
                        MouseOverCursor = comp.MouseOverCursor;
                        found = true;
                    }
                }   
            }
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            MouseEvent relArgs = new MouseEvent((int)(args.X - Bounds.absX), (int)(args.Y - Bounds.absY), args.Button);

            for (int i = 0; i < Components.Length; i++)
            {
                Components[i].OnMouseMove(api, relArgs);
                if (relArgs.Handled) break;
            }

            args.Handled = relArgs.Handled;
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            MouseEvent relArgs = new MouseEvent((int)(args.X - Bounds.absX), (int)(args.Y - Bounds.absY), args.Button);

            for (int i = 0; i < Components.Length; i++)
            {
                Components[i].OnMouseDown(api, relArgs);
                if (relArgs.Handled) break;
            }

            args.Handled = relArgs.Handled;
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            MouseEvent relArgs = new MouseEvent((int)(args.X - Bounds.absX), (int)(args.Y - Bounds.absY), args.Button);

            for (int i = 0; i < Components.Length; i++)
            {
                Components[i].OnMouseUp(api, relArgs);
                if (relArgs.Handled) break;
            }

            args.Handled = relArgs.Handled;

            base.OnMouseUp(api, args);
        }


        public override void Dispose()
        {
            base.Dispose();

            foreach (var val in Components)
            {
                val.Dispose();
            }
        }
    }




    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a chat input to the GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="OnTextChanged">The event fired when the text is changed.</param>
        /// <param name="key">The name of this chat component.</param>
        public static GuiComposer AddRichtext(this GuiComposer composer, string vtmlCode, CairoFont baseFont, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementRichtext(composer.Api, VtmlUtil.Richtextify(vtmlCode, baseFont), bounds), key);
            }

            return composer;
        }

        /// <summary>
        /// Adds a chat input to the GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="OnTextChanged">The event fired when the text is changed.</param>
        /// <param name="key">The name of this chat component.</param>
        public static GuiComposer AddRichtext(this GuiComposer composer, RichTextComponentBase[] components, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementRichtext(composer.Api, components, bounds), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the chat input by name.
        /// </summary>
        /// <param name="key">The name of the chat input component.</param>
        /// <returns>The named component.</returns>
        public static GuiElementRichtext GetRichtext(this GuiComposer composer, string key)
        {
            return (GuiElementRichtext)composer.GetElement(key);
        }



    }
}
