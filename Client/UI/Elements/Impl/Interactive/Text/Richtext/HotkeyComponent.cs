using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{

    public class HotkeyComponent : RichTextComponent
    {
        public HotkeyComponent(ICoreClientAPI api, string hotkeycode, CairoFont font) : base(api, hotkeycode, font)
        {
            if (api.Input.HotKeys.TryGetValue(hotkeycode.ToLowerInvariant(), out var hotkey))
            {
                displayText = hotkey.CurrentMapping.ToString();
            }

            init();
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY)
        {
            base.RenderInteractiveElements(deltaTime, renderX, renderY);

            for (int i = 0; i < lines.Length; i++)
            {
                var bounds = lines[i].Bounds;
                api.Render.RenderRectangle((float)renderX + (float)bounds.X - 3, (float)renderY + (float)bounds.Y - 3, 50, (float)bounds.Width + 4, (float)bounds.Height + 2, ColorUtil.ColorFromRgba(200, 255, 255, 128));
            }
        }


    }
}
