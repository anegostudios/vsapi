using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public abstract class GuiDialogCharacterBase : GuiDialog
    {

        public abstract List<GuiTab> Tabs { get; }
        public abstract List<API.Common.Action<GuiComposer>> RenderTabHandlers { get; }


        public GuiDialogCharacterBase(ICoreClientAPI capi) : base(capi) { }


        public virtual void OnTitleBarClose()
        {
            TryClose();
        }


        public abstract event Common.Action ComposeExtraGuis;

        public abstract event Common.Action<int> TabClicked;
    }
}
