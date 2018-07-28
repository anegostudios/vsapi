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

        public GuiElementControl(ICoreClientAPI capi, ElementBounds bounds) : base(capi, bounds)
        {
        }

        public bool Enabled { get { return this.enabled; } set { enabled = value; } }

    }


    public abstract class GuiElementTextControl : GuiElementTextBase
    {
        protected bool enabled = true;

        public GuiElementTextControl(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds) : base(capi, text, font, bounds)
        {
        }

        public bool Enabled { get { return this.enabled; } set { enabled = value; } }

    }
}
