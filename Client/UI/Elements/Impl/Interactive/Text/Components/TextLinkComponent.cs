using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class TextLinkComponent : TextComponent
    {
        internal string url;

        /// <summary>
        /// A text component with an embedded link.
        /// </summary>
        /// <param name="text">The text of the Text.</param>
        /// <param name="url">The link in the text.</param>
        public TextLinkComponent(string text, string url) : base(text)
        {
            this.url = url;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface, CairoFont withFont)
        {
            withFont.SetupContext(ctx);
            ctx.SetSourceRGBA(ElementGeometrics.LinkTextColor);

            DrawText(ctx, text, Bounds.drawX, Bounds.drawY);
        }
    }
}
