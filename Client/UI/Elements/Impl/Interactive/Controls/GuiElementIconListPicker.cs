using Cairo;
using System;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Creates a toggle button for the GUI.
    /// </summary>
    public class GuiElementIconListPicker : GuiElementElementListPickerBase<string>
    {
        public GuiElementIconListPicker(ICoreClientAPI capi, string elem, ElementBounds bounds) : base(capi, elem, bounds)
        {
        }

        public override void DrawElement(string icon, Context ctx, ImageSurface surface)
        {
            ctx.SetSourceRGBA(1,1,1,0.2);
            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctx.Fill();

            api.Gui.Icons.DrawIcon(ctx, "wp" + icon.UcFirst(), Bounds.drawX + 2, Bounds.drawY + 2, Bounds.InnerWidth - 4, Bounds.InnerHeight - 4, new double[] { 1,1,1,1 });
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Returns the icon list picker
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GuiElementIconListPicker GetIconListPicker(this GuiComposer composer, string key)
        {
            return (GuiElementIconListPicker)composer.GetElement(key);
        }


        /// <summary>
        /// Selects one of the clickable icons
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key"></param>
        /// <param name="selectedIndex"></param>
        public static void IconListPickerSetValue(this GuiComposer composer, string key, int selectedIndex)
        {
            int i = 0;
            GuiElementIconListPicker  btn;
            while ((btn = composer.GetIconListPicker(key + "-" + i)) != null)
            {
                btn.SetValue(i == selectedIndex);
                i++;
            }
        }


        /// <summary>
        /// Adds multiple clickable icons 
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="icons"></param>
        /// <param name="onToggle"></param>
        /// <param name="startBounds"></param>
        /// <param name="maxLineWidth"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GuiComposer AddIconListPicker(this GuiComposer composer, string[] icons, Action<int> onToggle, ElementBounds startBounds, int maxLineWidth, string key = null)
        {
            return AddElementListPicker<string>(composer, typeof(GuiElementIconListPicker), icons, onToggle, startBounds, maxLineWidth, key);
        }




    }

}
