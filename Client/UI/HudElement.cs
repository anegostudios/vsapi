namespace Vintagestory.API.Client
{
    public abstract class HudElement : GuiDialog
    {
        public HudElement(ICoreClientAPI capi) : base(capi)
        {
        }

        public override EnumDialogType DialogType => EnumDialogType.HUD;

        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }
        

        public override bool DisableWorldInteract()
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
