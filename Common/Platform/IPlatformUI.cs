using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public interface IXPlatFormsUI
    {
        void SetClipboardText(string text);
        string GetClipboardText();
        void ShowMessageBox(string title, string text);

        Size2i GetScreenSize();
    }

}
