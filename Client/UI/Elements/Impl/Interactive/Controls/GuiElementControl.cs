
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// An element that allows input and can be disabled
    /// </summary>
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
        public virtual bool Enabled { get { return this.enabled; } set { enabled = value; } }

    }

}
