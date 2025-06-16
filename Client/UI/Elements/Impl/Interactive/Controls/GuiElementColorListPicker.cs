using Cairo;
using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Creates a toggle button for the GUI.
    /// </summary>
    public class GuiElementColorListPicker : GuiElementElementListPickerBase<int>
    {
        public GuiElementColorListPicker(ICoreClientAPI capi, int elem, ElementBounds bounds) : base(capi, elem, bounds)
        {
        }

        public override void DrawElement(int color, Context ctx, ImageSurface surface)
        {
            double[] dcolor = ColorUtil.ToRGBADoubles(color);
            ctx.SetSourceRGBA(dcolor[0], dcolor[1], dcolor[2], 1);
            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctx.Fill();
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Returns a previously added color list picker element
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GuiElementColorListPicker GetColorListPicker(this GuiComposer composer, string key)
        {
            return (GuiElementColorListPicker)composer.GetElement(key);
        }


        /// <summary>
        /// Selects one of the colors from a color list picker
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key"></param>
        /// <param name="selectedIndex"></param>
        public static void ColorListPickerSetValue(this GuiComposer composer, string key, int selectedIndex)
        {
            int i = 0;
            GuiElementColorListPicker btn;
            while ((btn = composer.GetColorListPicker(key + "-" + i)) != null)
            {
                btn.SetValue(i == selectedIndex);
                i++;
            }
        }


        /// <summary>
        /// Adds a range of clickable colors
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="colors"></param>
        /// <param name="onToggle"></param>
        /// <param name="startBounds"></param>
        /// <param name="maxLineWidth"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GuiComposer AddColorListPicker(this GuiComposer composer, int[] colors, Action<int> onToggle, ElementBounds startBounds, int maxLineWidth, string key = null)
        {
            return AddElementListPicker<int>(composer, typeof(GuiElementColorListPicker), colors, onToggle, startBounds, maxLineWidth, key);
        }




    }

}
