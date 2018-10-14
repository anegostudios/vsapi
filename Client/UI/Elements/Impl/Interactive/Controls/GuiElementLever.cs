using System;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    /// <remarks>
    /// PULL THE LEVER KRONK!
    /// </remarks>
    public class GuiElementLever : GuiElement
    {
        internal bool leverOn;
        ActionConsumable<bool> onLeverNewState;

        // TODO: Create atlas for those
        int handleTextureId;
        int handleShadowTextureId;
        int shaftTextureId;
        int glowingRedlampTextureId;
        int glowingGreenlampTextureId;

        // Can only scale those during compose elements
        internal const int unscaledLeverWidth = 30;
        internal const int unscaledLeverHeight = 60;

        internal const int unscaledHandleWidth = 52;
        internal const int unscaledHandleHeight = 10;

        internal const int unscaledLeverWalkDistance = 40;
        internal const int unscaledLampYOffset = 20;


        // Then store scaled values for rendering, so we don't have to scale them every render frame
        double leverWalkDistance;
        double lampYOffset;
        double leverWidth;
        double leverHeight;
        double handleWidth;
        double handleHeight;

        /* Lever Animation stuff */
        double leverPosition = 0;

        // Delay and transition times are measured in seconds
        float leverTransitionTime = 0.4f;
        float lampDelay = 0.2f;

        float currentTimePassed = 0f;
        float currentLampDelayLeft = 0f;

        bool transitioning = false;


        /// <summary>
        /// Creates the Lever with the specified bounds.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="onLeverNewState">The event fired when the lever is flipped.</param>
        /// <param name="bounds"></param>
        public GuiElementLever(ICoreClientAPI capi, ActionConsumable<bool> onLeverNewState, ElementBounds bounds) : base(capi, bounds)
        {
            this.onLeverNewState = onLeverNewState;
        }


        // TODO: Somehow clean up this mess of code
        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            lampYOffset = scaled(unscaledLampYOffset);
            leverWidth = scaled(unscaledLeverWidth);
            leverHeight = scaled(unscaledLeverHeight);
            handleWidth = scaled(unscaledHandleWidth);
            handleHeight = scaled(unscaledHandleHeight);
            leverWalkDistance = scaled(unscaledLeverWalkDistance);


            Bounds.CalcWorldBounds();


            /*** 1. Panel ***/

            // Wood bg
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY + lampYOffset, Bounds.InnerWidth, leverHeight, ElementGeometrics.ElementBGRadius);
            fillWithPattern(api, ctxStatic, woodTextureName);

            EmbossRoundRectangleElement(ctxStatic, Bounds.drawX, Bounds.drawY + lampYOffset, leverWidth, leverHeight);

            // Wood Inset 
            ctxStatic.SetSourceRGBA(0, 0, 0, 0.6);
            RoundRectangle(ctxStatic, Bounds.drawX + scaled(8), Bounds.drawY + lampYOffset + scaled(8), leverWidth - scaled(16), leverHeight - scaled(16), ElementGeometrics.ElementBGRadius);
            ctxStatic.Fill();
            EmbossRoundRectangleElement(ctxStatic, Bounds.drawX + scaled(8), Bounds.drawY + lampYOffset + scaled(8), leverWidth - scaled(16), leverHeight - scaled(16), true);



            // Nails
            ctxStatic.Operator = Operator.Over;
            ctxStatic.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + scaled(3)), (int)(Bounds.drawY + lampYOffset + scaled(3)));
            ctxStatic.Paint();
            ctxStatic.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + leverWidth - scaled(7.5)), (int)(Bounds.drawY + lampYOffset + leverHeight - scaled(7.5)));
            ctxStatic.Paint();
            ctxStatic.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + leverWidth - scaled(7.5)), (int)(Bounds.drawY + lampYOffset + scaled(3)));
            ctxStatic.Paint();
            ctxStatic.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + scaled(3)), (int)(Bounds.drawY + lampYOffset + leverHeight - scaled(7.5)));
            ctxStatic.Paint();

            // Green lamp
            Lamp(ctxStatic, Bounds.drawX + Bounds.OuterWidth / 2 - scaled(10 / 2), Bounds.drawY + scaled(6), new float[] { 0, 0.21f, 0 });

            // Red lamp
            Lamp(ctxStatic, Bounds.drawX + Bounds.OuterWidth / 2 - scaled(10 / 2), Bounds.drawY + Bounds.OuterHeight - lampYOffset + scaled(6), new float[] { 0.21f, 0, 0 });


            /*** 2. Lit lamps ***/

            // Green
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)scaled(10), (int)scaled(10));
            Context ctx = genContext(surface);
            Lamp(ctx, 0, 0, new float[] { 0.1f, 0.71f, 0.1f });
            generateTexture(surface, ref glowingGreenlampTextureId);

            ctx.Dispose();
            surface.Dispose();

            // Red
            surface = new ImageSurface(Format.Argb32, (int)scaled(10), (int)scaled(10));
            ctx = genContext(surface);
            Lamp(ctx, 0, 0, new float[] { 0.71f, 0.1f, 0.1f });
            generateTexture(surface, ref glowingRedlampTextureId);
            ctx.Dispose();
            surface.Dispose();


            /*** 3. Handle ***/
            surface = new ImageSurface(Format.Argb32, (int)handleWidth + 10, (int)handleHeight + 10);
            ctx = genContext(surface);

            // Shadow
            ImageSurface insetShadowSurface = new ImageSurface(Format.Argb32, (int)handleWidth + 10, (int)handleHeight + 10);
            Context ctxInsetShadow = new Context(insetShadowSurface);

            ctxInsetShadow.SetSourceRGBA(255, 255, 255, 0);
            ctxInsetShadow.Paint();
            ctxInsetShadow.SetSourceRGBA(0, 0, 0, 0.5);
            RoundRectangle(ctxInsetShadow, 0, 0, handleWidth, handleHeight, ElementGeometrics.ElementBGRadius);
            ctxInsetShadow.Fill();
            insetShadowSurface.Blur(3);

            generateTexture(insetShadowSurface, ref handleShadowTextureId);

            ctxInsetShadow.Dispose();
            insetShadowSurface.Dispose();


            RoundRectangle(ctx, 0, 0, handleWidth, handleHeight, ElementGeometrics.ElementBGRadius);
            fillWithPattern(api, ctx, woodTextureName);

            EmbossRoundRectangleElement(ctx, 0, 0, handleWidth, handleHeight);

            generateTexture(surface, ref handleTextureId);
            ctx.Dispose();
            surface.Dispose();


            /*** 4. Shaft ***/
            double shaftHeight = (leverHeight - scaled(16)) / 2; 
            surface = new ImageSurface(Format.Argb32, (int)scaled(8), (int)shaftHeight);
            ctx = genContext(surface);

            paintWithPattern(api, ctx, noisyMetalTextureName);

            ctx.NewPath();
            ctx.SetSourceRGBA(255, 255, 255, 0.3f);
            ctx.LineWidth = 2f;
            ctx.MoveTo(scaled(1), 0);
            ctx.LineTo(scaled(1), shaftHeight);
            ctx.Stroke();

            ctx.NewPath();
            ctx.SetSourceRGBA(0, 0, 0, 0.5f);
            ctx.LineWidth = 2f;
            ctx.MoveTo(scaled(8 - 1), 0);
            ctx.LineTo(scaled(8 - 1), shaftHeight);
            ctx.Stroke();


            generateTexture(surface, ref shaftTextureId);
            ctx.Dispose();
            surface.Dispose();
        }



        public override void RenderInteractiveElements(float deltaTime)
        {
            double offsetY = Bounds.absY + lampYOffset;

            double handleZOffset = 0f;

            if (transitioning)
            {
                currentTimePassed += deltaTime;

                float sineConst = 1.5723f;

                if (leverOn)
                {
                    leverPosition -= leverWalkDistance * deltaTime / leverTransitionTime * sineConst * Math.Sin(currentTimePassed / leverTransitionTime * Math.PI);
                    
                } else
                {
                    leverPosition += leverWalkDistance * deltaTime / leverTransitionTime * sineConst * Math.Sin(currentTimePassed / leverTransitionTime * Math.PI);
                }

                leverPosition = Math.Max(0, Math.Min(leverWalkDistance, leverPosition));

                if (currentTimePassed >= leverTransitionTime)
                {
                    transitioning = false;
                    currentLampDelayLeft = lampDelay;
                }


                handleZOffset = scaled(12) * Math.Sin(currentTimePassed / leverTransitionTime * Math.PI);
            }

            if (currentLampDelayLeft > 0)
            {
                currentLampDelayLeft -= deltaTime;
            } else
            {
                if (!transitioning)
                {
                    api.Render.RenderTexture(leverOn ? glowingGreenlampTextureId : glowingRedlampTextureId, Bounds.absX + leverWidth / 2 - scaled(10 / 2), Bounds.absY + scaled(6) + (leverOn ? 0 : leverHeight + lampYOffset), scaled(10), scaled(10));
                }
            }


            if (leverPosition - handleZOffset < leverWalkDistance / 2)
            {
                double shaftHeight = (leverHeight - scaled(16)) / 2 * (leverPosition - handleZOffset < leverWalkDistance / 2 ? 1 - 2 * (leverPosition - handleZOffset) / leverWalkDistance : 0);
                double shaftOffset = handleZOffset - handleHeight / 2;

                api.Render.RenderTexture(shaftTextureId, Bounds.absX + scaled(leverWidth) / 2 - scaled(8 / 2) - scaled(0.5), offsetY + leverPosition + scaled(5) - shaftOffset, scaled(8), shaftHeight);
            }
            else
            {
                double shaftHeight = (leverHeight - scaled(16)) / 2 * (2 * (leverPosition - handleZOffset) / leverWalkDistance - 1);

                api.Render.RenderTexture(shaftTextureId, Bounds.absX + scaled(leverWidth) / 2 - scaled(8 / 2) - scaled(0.5), offsetY + leverWalkDistance / 2 + scaled(5) + handleHeight / 2, scaled(8), shaftHeight);

            }



            api.Render.RenderTexture(handleShadowTextureId, Bounds.absX - handleWidth / 4 + scaled(4), offsetY + leverPosition + scaled(3) + scaled(5), handleWidth + 10, handleHeight + 10);

            api.Render.RenderTexture(handleTextureId, Bounds.absX - handleWidth / 4 + scaled(1), offsetY + leverPosition + scaled(5) - handleZOffset, handleWidth+10, handleHeight+9);

        }


        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            leverTransitionTime = 0.4f;

            transitioning = true;
            if (leverOn)
            {
                leverPosition = 0; 
            } else
            {
                leverPosition = leverWalkDistance;
            }

            leverOn = !leverOn;
            currentTimePassed = 0;

            if (onLeverNewState != null)
            {
                args.Handled = onLeverNewState(leverOn);
            }
        }

        /// <summary>
        /// Sets the lever state.
        /// </summary>
        /// <param name="leverOn">On or off?</param>
        /// <remarks>WRONG LEVERRRRRRRRRR!</remarks>
        public void SetLeverState(bool leverOn)
        {
            this.leverOn = leverOn;

            leverPosition = leverOn ? 0 : leverWalkDistance;
        }
    }


    public static partial class GuiComposerHelpers
    {

        /// <summary>
        /// Adds a lever to the current GUI.
        /// </summary>
        /// <param name="onLeverNewState">The event that fires when the lever is flipped.</param>
        /// <param name="bounds">The bounds of the lever.</param>
        /// <param name="key">The name for the lever.</param>
        public static GuiComposer AddLever(this GuiComposer composer, ActionConsumable<bool> onLeverNewState, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementLever(composer.Api, onLeverNewState, bounds), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the lever by name.
        /// </summary>
        /// <param name="key">Wrong Lever.</param>
        /// <returns>The lever.</returns>
        public static GuiElementLever GetLever(this GuiComposer composer, string key)
        {
            return (GuiElementLever)composer.GetElement(key);
        }
    }

}
