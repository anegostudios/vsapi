using Cairo;
using System;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Creates a toggle button for the GUI.
    /// </summary>
    public class GuiElementToggleButton : GuiElementTextBase
    {
        Action<bool> handler;

        /// <summary>
        /// Is this button toggleable?
        /// </summary>
        public bool Toggleable = false;

        /// <summary>
        /// Is this button on?
        /// </summary>
        public bool On;

        LoadedTexture releasedTexture;
        LoadedTexture pressedTexture;
        LoadedTexture hoverTexture;

        int unscaledDepth = 4;

        string icon;

        double pressedYOffset;
        double nonPressedYOffset;

        /// <summary>
        /// Is this element capable of being in the focus?
        /// </summary>
        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Constructor for the button
        /// </summary>
        /// <param name="capi">The core client API.</param>
        /// <param name="icon">The icon name</param>
        /// <param name="text">The text for the button.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="OnToggled">The action that happens when the button is toggled.</param>
        /// <param name="bounds">The bounding box of the button.</param>
        /// <param name="toggleable">Can the button be toggled on or off?</param>
        public GuiElementToggleButton(ICoreClientAPI capi, string icon, string text, CairoFont font, Action<bool> OnToggled, ElementBounds bounds, bool toggleable = false) : base(capi, text, font, bounds)
        {
            releasedTexture = new LoadedTexture(capi);
            pressedTexture = new LoadedTexture(capi);
            hoverTexture = new LoadedTexture(capi);

            handler = OnToggled;
            Toggleable = toggleable;
            this.icon = icon;
        }

        /// <summary>
        /// Composes the element in both the pressed, and released states.
        /// </summary>
        /// <param name="ctx">The context of the element.</param>
        /// <param name="surface">The surface of the element.</param>
        /// <remarks>Neither the context, nor the surface is used in this function.</remarks>
        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ComposeReleasedButton();
            ComposePressedButton();
        }

        void ComposeReleasedButton()
        {
            double depth = scaled(unscaledDepth);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(Bounds.OuterWidth), (int)(Bounds.OuterHeight));
            Context ctx = genContext(surface);


            ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
            RoundRectangle(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, GuiStyle.ElementBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(1, 1, 1, 0.1);
            ctx.Fill();


            EmbossRoundRectangleElement(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, false, (int)depth);

            // Pretty elaborate way of vertically centering the text. Le sigh.
            double textexYBearing = Font.GetTextExtents(GetText()).YBearing;
            nonPressedYOffset = (-Font.GetFontExtents().Ascent - textexYBearing + (Bounds.InnerHeight + textexYBearing) / 2 - 2);

            DrawMultilineTextAt(ctx, Bounds.absPaddingX, Bounds.absPaddingY + nonPressedYOffset, EnumTextOrientation.Center);

            if (icon != null && icon.Length > 0)
            {
                api.Gui.Icons.DrawIcon(ctx, icon, Bounds.absPaddingX + scaled(4), Bounds.absPaddingY + scaled(4), Bounds.InnerWidth - scaled(9), Bounds.InnerHeight - scaled(9), Font.Color);
            }

            generateTexture(surface, ref releasedTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        void ComposePressedButton()
        {
            double depth = scaled(unscaledDepth);

            // 1. Pressed
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(Bounds.OuterWidth), (int)(Bounds.OuterHeight));
            Context ctx = genContext(surface);
            ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
            RoundRectangle(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, GuiStyle.ElementBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(0, 0, 0, 0.1);
            ctx.Fill();


            EmbossRoundRectangleElement(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, true, (int)depth);

            // Pretty elaborate way of vertically centering the text. Le sigh.
            double textexYBearing = Font.GetTextExtents(GetText()).YBearing;
            pressedYOffset = (-Font.GetFontExtents().Ascent - textexYBearing + (Bounds.InnerHeight + textexYBearing) / 2);

            DrawMultilineTextAt(ctx, Bounds.absPaddingX, Bounds.absPaddingY + pressedYOffset, EnumTextOrientation.Center);

            if (icon != null && icon.Length > 0)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);
                api.Gui.Icons.DrawIcon(ctx, icon, Bounds.absPaddingX + scaled(4), Bounds.absPaddingY + scaled(4), Bounds.InnerWidth - scaled(8), Bounds.InnerHeight - scaled(8), GuiStyle.DialogDefaultTextColor);
            }

            generateTexture(surface, ref pressedTexture);

            ctx.Dispose();
            surface.Dispose();

            // 2. Hover
            surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Fill();

            double[] prevcolor = this.Font.Color;
            Font.Color = GuiStyle.ActiveButtonTextColor;

            DrawMultilineTextAt(ctx, Bounds.absPaddingX + pressedYOffset + scaled(4), 0, EnumTextOrientation.Center);

            if (icon != null && icon.Length > 0)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);
                api.Gui.Icons.DrawIcon(ctx, icon, Bounds.absPaddingX + scaled(4), Bounds.absPaddingY + scaled(4), Bounds.InnerWidth - scaled(8), Bounds.InnerHeight - scaled(8), GuiStyle.DialogDefaultTextColor);
            }

            this.Font.Color = prevcolor;

            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();

        }

        /// <summary>
        /// Renders the button.
        /// </summary>
        /// <param name="deltaTime">The time elapsed.</param>
        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(On ? pressedTexture.TextureId : releasedTexture.TextureId, Bounds);

            if (icon == null && Bounds.PointInside(api.Input.MouseX, api.Input.MouseY))
            {
                api.Render.Render2DTexturePremultipliedAlpha(
                    hoverTexture.TextureId,
                    Bounds.renderX,
                    Bounds.renderY + (On ? pressedYOffset : nonPressedYOffset),
                    Bounds.OuterWidthInt,
                    Bounds.OuterHeightInt
                );
            }
        }

        /// <summary>
        /// Handles the mouse button press while the mouse is on this button.
        /// </summary>
        /// <param name="api">The client API</param>
        /// <param name="args">The mouse event arguments.</param>
        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            On = !On;
            handler?.Invoke(On);
            api.Gui.PlaySound("toggleswitch");
        }

        /// <summary>
        /// Handles the mouse button release while the mouse is on this button.
        /// </summary>
        /// <param name="api">The client API</param>
        /// <param name="args">The mouse event arguments</param>
        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Toggleable) On = false;
        }

        /// <summary>
        /// Handles the event fired when the mouse is released.
        /// </summary>
        /// <param name="api">The client API</param>
        /// <param name="args">Mouse event arguments</param>
        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            if (!Toggleable) On = false;
            base.OnMouseUp(api, args);
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            if (args.KeyCode == (int)GlKeys.Enter)
            {
                args.Handled = true;
                On = !On;
                handler?.Invoke(On);
                api.Gui.PlaySound("toggleswitch");
            }
        }



        /// <summary>
        /// Sets the value of the button.
        /// </summary>
        /// <param name="on">Am I on or off?</param>
        public void SetValue(bool on)
        {
            On = on;
        }

        /// <summary>
        /// Disposes of the button.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            releasedTexture.Dispose();
            pressedTexture.Dispose();
            hoverTexture.Dispose();
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Gets the toggle button by name in the GUIComposer.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the button.</param>
        /// <returns>A button.</returns>
        public static GuiElementToggleButton GetToggleButton(this GuiComposer composer, string key)
        {
            return (GuiElementToggleButton)composer.GetElement(key);
        }


        /// <summary>
        /// Creates a toggle button with the given parameters.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the button.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="onToggle">The event that happens once the button is toggled.</param>
        /// <param name="bounds">The bounding box of the button.</param>
        /// <param name="key">The name of the button for easy access.</param>
        public static GuiComposer AddToggleButton(this GuiComposer composer, string text, CairoFont font, Action<bool> onToggle, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementToggleButton(composer.Api, "", text, font, onToggle, bounds, true), key);
            }
            return composer;
        }

        /// <summary>
        /// Adds an icon button.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="icon">The name of the icon.</param>
        /// <param name="onToggle">The event that happens once the button is toggled.</param>
        /// <param name="bounds">The bounding box of the button.</param>
        /// <param name="key">The name of the button for easy access.</param>
        public static GuiComposer AddIconButton(this GuiComposer composer, string icon, Action<bool> onToggle, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementToggleButton(composer.Api, icon, "", CairoFont.WhiteDetailText(), onToggle, bounds, false), key);
            }
            return composer;
        }

        public static GuiComposer AddIconButton(this GuiComposer composer, string icon, CairoFont font, Action<bool> onToggle, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementToggleButton(composer.Api, icon, "", font, onToggle, bounds, false), key);
            }
            return composer;
        }

        /// <summary>
        /// Toggles the given button.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the button that was set.</param>
        /// <param name="selectedIndex">the index of the button.</param>
        public static void ToggleButtonsSetValue(this GuiComposer composer, string key, int selectedIndex)
        {
            int i = 0;
            GuiElementToggleButton btn;
            while ((btn = composer.GetToggleButton(key + "-" + i)) != null)
            {
                btn.SetValue(i == selectedIndex);
                i++;
            }
        }

        /// <summary>
        /// Adds multiple buttons with icons.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="icons">The collection of icons for the buttons.</param>
        /// <param name="font">The font for the buttons.</param>
        /// <param name="onToggle">The event called when the buttons are pressed.</param>
        /// <param name="bounds">The bounds of the buttons.</param>
        /// <param name="key">The key given to the bundle of buttons.</param>
        public static GuiComposer AddIconToggleButtons(this GuiComposer composer, string[] icons, CairoFont font, Action<int> onToggle, ElementBounds[] bounds, string key = null)
        {
            if (!composer.Composed)
            {
                int quantityButtons = icons.Length;

                for (int i = 0; i < icons.Length; i++)
                {
                    int index = i;

                    composer.AddInteractiveElement(
                        new GuiElementToggleButton(composer.Api, icons[i], "", font, (on) => {
                            if (on)
                            {
                                onToggle(index);
                                for (int j = 0; j < quantityButtons; j++)
                                {
                                    if (j == index) continue;
                                    composer.GetToggleButton(key + "-" + j).SetValue(false);
                                }
                            }
                            else
                            {
                                composer.GetToggleButton(key + "-" + index).SetValue(true);
                            }
                        }, bounds[i], true), 
                        key + "-" + i
                    );
                }
            }
            return composer;
        }

        /// <summary>
        /// Adds multiple buttons with Text.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="texts">The texts on all the buttons.</param>
        /// <param name="font">The font for the buttons</param>
        /// <param name="onToggle">The event fired when the button is pressed.</param>
        /// <param name="bounds">The bounds of the buttons.</param>
        /// <param name="key">The key given to the bundle of buttons.</param>
        public static GuiComposer AddTextToggleButtons(this GuiComposer composer, string[] texts, CairoFont font, Action<int> onToggle, ElementBounds[] bounds, string key = null)
        {
            if (!composer.Composed)
            {
                int quantityButtons = texts.Length;

                for (int i = 0; i < texts.Length; i++)
                {
                    int index = i;

                    composer.AddInteractiveElement(
                        new GuiElementToggleButton(composer.Api, "", texts[i], font, (on) => {
                            if (on)
                            {
                                onToggle(index);
                                for (int j = 0; j < quantityButtons; j++)
                                {
                                    if (j == index) continue;
                                    composer.GetToggleButton(key + "-" + j).SetValue(false);
                                }
                            }
                            else
                            {
                                composer.GetToggleButton(key + "-" + index).SetValue(true);
                            }
                        }, bounds[i], true),
                        key + "-" + i
                    );
                }
            }
            return composer;
        }




    }

}
