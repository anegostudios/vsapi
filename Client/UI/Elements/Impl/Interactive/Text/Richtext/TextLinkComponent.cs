using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class LinkTextComponent : RichTextComponent
    {
        Common.Action<LinkTextComponent> onLinkClicked;

        public string Href;


        /// <summary>
        /// A text component with an embedded link.
        /// </summary>
        /// <param name="displayText">The text of the Text.</param>
        /// <param name="url">The link in the text.</param>
        public LinkTextComponent(string displayText, CairoFont font, Common.Action<LinkTextComponent> onLinkClicked) : base(displayText, font)
        {
            this.onLinkClicked = onLinkClicked;
            MouseOverCursor = "linkselect";

            this.font = this.font.Clone().WithColor(GuiStyle.LightBrownHoverTextColor);
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            textUtil.DrawMultilineText(ctx, font, lines, EnumTextOrientation.Left);

            ctx.LineWidth = 1;
            ctx.SetSourceRGBA(font.Color);

            PointD pos = ctx.CurrentPoint;

            for (int i = 0; i < lines.Length; i++)
            {
                TextLine line = lines[i];
                ctx.MoveTo(line.Bounds.X + line.PaddingLeft, line.Bounds.Y + line.Bounds.AscentOrHeight + 2);
                ctx.LineTo(line.Bounds.X + line.PaddingLeft - line.PaddingRight + line.Bounds.Width, line.Bounds.Y + line.Bounds.AscentOrHeight + 2);
                ctx.Stroke();
            }

        }

        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            foreach (var val in BoundsPerLine)
            {
                if (val.PointInside(args.X, args.Y))
                {
                    args.Handled = true;
                    if (onLinkClicked == null)
                    {
                        if (Href.StartsWith("hotkey://"))
                        {
                            api.Input.GetHotKeyByCode(Href.Substring("hotkey://".Length))?.Handler?.Invoke(null);
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(Href);
                        }
                    } else
                    {
                        onLinkClicked.Invoke(this);
                    }
                    
                }
            }
        }
    }
}
