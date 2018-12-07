namespace Vintagestory.API.Client
{
    /// <summary>
    /// Base class for Hud Elements.
    /// </summary>
    public abstract class HudElement : GuiDialog
    {
        /// <summary>
        /// Creates a new Hud Element.
        /// </summary>
        /// <param name="capi">The Client API</param>
        public HudElement(ICoreClientAPI capi) : base(capi)
        {
        }

        public override EnumDialogType DialogType => EnumDialogType.HUD;

        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }
        

        public override bool RequiresUngrabbedMouse()
        {
            return false;
        }

        public override void OnRender2D(float deltaTime)
        {
            capi.Render.GlPushMatrix();
            capi.Render.GlTranslate(0, 0, -150);
            base.OnRender2D(deltaTime);
            capi.Render.GlPopMatrix();
        }
    }
}
