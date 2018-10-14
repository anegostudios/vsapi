using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public abstract class GuiElementControl : GuiElement
    {
        protected bool enabled = true;

        /// <summary>
        /// Constructor for the element.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="bounds">the bounds of the element.</param>
        public GuiElementControl(ICoreClientAPI capi, ElementBounds bounds) : base(capi, bounds)
        {
        }

        /// <summary>
        /// Enables/disables the given element (default is enabled)
        /// </summary>
        public bool Enabled { get { return this.enabled; } set { enabled = value; } }

    }


    public abstract class GuiElementTextControl : GuiElementTextBase
    {
        protected bool enabled = true;

        /// <summary>
        /// Constructor for the text element.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="text">The text value of the element.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">the bounds of the element.</param>
        public GuiElementTextControl(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds) : base(capi, text, font, bounds)
        {
        }

        /// <summary>
        /// Enables/disables the given element (default is enabled)
        /// </summary>
        public bool Enabled { get { return this.enabled; } set { enabled = value; } }

    }
}
