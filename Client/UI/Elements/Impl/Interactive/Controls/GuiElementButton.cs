using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementToggleButton : GuiElementTextControl
    {
        API.Common.Action<bool> handler;

        public bool Toggleable = false;
        public bool On;

        LoadedTexture releasedTexture;
        LoadedTexture pressedTexture;

        int unscaledDepth = 4;

        string icon;

        public override bool Focusable { get { return true; } }

        public GuiElementToggleButton(ICoreClientAPI capi, string icon, string text, CairoFont font, API.Common.Action<bool> OnToggled, ElementBounds bounds, bool toggleable = false) : base(capi, text, font, bounds)
        {
            releasedTexture = new LoadedTexture(capi);
            pressedTexture = new LoadedTexture(capi);

            handler = OnToggled;
            this.Toggleable = toggleable;
            this.icon = icon;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ComposeReleasedButton();
            ComposePressedButton();
        }


        void ComposeReleasedButton()
        {
            double depth = scaled(unscaledDepth);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(Bounds.OuterWidth), (int)(Bounds.OuterHeight));
            Context ctx = genContext(surface);


            ctx.SetSourceRGB(ElementGeometrics.DialogDefaultBgColor[0], ElementGeometrics.DialogDefaultBgColor[1], ElementGeometrics.DialogDefaultBgColor[2]);
            RoundRectangle(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, ElementGeometrics.ElementBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(1, 1, 1, 0.1);
            ctx.Fill();


            EmbossRoundRectangleElement(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, false, (int)depth);

            double height = GetMultilineTextHeight(text, Bounds.InnerWidth);
            ShowMultilineText(ctx, text, Bounds.absPaddingX, (Bounds.InnerHeight - height) / 2 - depth/2, Bounds.InnerWidth, EnumTextOrientation.Center);

            if (icon != null && icon.Length > 0)
            {
                api.Gui.Icons.DrawIcon(ctx, icon, Bounds.absPaddingX + 3, Bounds.absPaddingY + 3, Bounds.InnerWidth - 6, Bounds.InnerHeight - 6, ElementGeometrics.DialogDefaultTextColor);
            }

            generateTexture(surface, ref releasedTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        void ComposePressedButton()
        {
            double depth = scaled(unscaledDepth);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(Bounds.OuterWidth), (int)(Bounds.OuterHeight));
            Context ctx = genContext(surface);

            
            ctx.SetSourceRGB(ElementGeometrics.DialogDefaultBgColor[0], ElementGeometrics.DialogDefaultBgColor[1], ElementGeometrics.DialogDefaultBgColor[2]);
            RoundRectangle(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, ElementGeometrics.ElementBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(0, 0, 0, 0.1);
            ctx.Fill();


            EmbossRoundRectangleElement(ctx, 0, 0, Bounds.OuterWidth, Bounds.OuterHeight, true, (int)depth);

            double height = GetMultilineTextHeight(text, Bounds.InnerWidth);

            ShowMultilineText(ctx, text, Bounds.absPaddingX, (Bounds.InnerHeight - height)/2 + depth / 2, Bounds.InnerWidth, EnumTextOrientation.Center);

            if (icon != null && icon.Length > 0)
            {
                ctx.SetSourceRGBA(ElementGeometrics.DialogDefaultTextColor);
                api.Gui.Icons.DrawIcon(ctx, icon, Bounds.absPaddingX + scaled(4), Bounds.absPaddingY + scaled(4), Bounds.InnerWidth - scaled(8), Bounds.InnerHeight - scaled(8), ElementGeometrics.DialogDefaultTextColor);
            }

            generateTexture(surface, ref pressedTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(On ? pressedTexture.TextureId : releasedTexture.TextureId, Bounds);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            On = !On;
            if (handler != null) handler(On);
            api.Gui.PlaySound("toggleswitch");
        }

        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Toggleable) On = false;
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            if (!Toggleable) On = false;
            base.OnMouseUp(api, args);
        }

        public void SetValue(bool on)
        {
            this.On = on;
        }

        public override void Dispose()
        {
            base.Dispose();

            releasedTexture.Dispose();
            pressedTexture.Dispose();
        }
    }


    public static partial class GuiComposerHelpers
    {
        public static GuiElementToggleButton GetToggleButton(this GuiComposer composer, string key)
        {
            return (GuiElementToggleButton)composer.GetElement(key);
        }



        public static GuiComposer AddToggleButton(this GuiComposer composer, string text, CairoFont font, API.Common.Action<bool> onToggle, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementToggleButton(composer.Api, "", text, font, onToggle, bounds, true), key);
            }
            return composer;
        }

        public static GuiComposer AddIconButton(this GuiComposer composer, string icon, API.Common.Action<bool> onToggle, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementToggleButton(composer.Api, icon, "", CairoFont.WhiteDetailText(), onToggle, bounds, false), key);
            }
            return composer;
        }


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


        public static GuiComposer AddIconToggleButtons(this GuiComposer composer, string[] icons, CairoFont font, API.Common.Action<int> onToggle, ElementBounds[] bounds, string key = null)
        {
            if (!composer.composed)
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

        public static GuiComposer AddTextToggleButtons(this GuiComposer composer, string[] texts, CairoFont font, API.Common.Action<int> onToggle, ElementBounds[] bounds, string key = null)
        {
            if (!composer.composed)
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
