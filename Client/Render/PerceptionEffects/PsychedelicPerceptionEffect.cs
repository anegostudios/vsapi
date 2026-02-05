#nullable disable

using System;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class PsychedelicPerceptionEffect : DrunkPerceptionEffect
    {
        public PsychedelicPerceptionEffect(ICoreClientAPI capi) : base(capi) { }

        float accum;
        float accum1s;
        float targetIntensity;

        public override void OnBeforeGameRender(float dt)
        {
            if (capi.IsGamePaused || capi.World.Player.Entity.AnimManager.HeadController == null) return;

            capi.Render.ShaderUniforms.PerceptionEffectIntensity = Intensity;
            capi.Render.ShaderUniforms.PsychedelicStrength = Intensity * 2f;
            capi.World.Player.Entity.HeadBobbingAmplitude = GameMath.Clamp(1 - Intensity * 5, 0, 1);

            accum1s += dt;
            if (accum1s > 1)
            {
                accum1s = 0;
                targetIntensity = capi.World.Player.Entity.WatchedAttributes.GetFloat("psychedelic") / 2f;
            }

            Intensity += (targetIntensity - Intensity) * dt / 3;

            accum = (float)((capi.InWorldEllapsedMilliseconds / 3000.0) % 100 * Math.PI);

            float f = Intensity / 250f / 2f / 4f;

            float dizzinessAccum = accum / 4;

            float dp = (float)(Math.Cos(dizzinessAccum / 1.15) + Math.Cos(dizzinessAccum / 1.35f)) * f;
            capi.World.Player.Entity.Pos.Pitch += dp;
            capi.Input.MousePitch += dp;
            capi.Input.MouseYaw += (float)(Math.Sin(dizzinessAccum / 1.1) + Math.Sin(dizzinessAccum / 1.5f) + Math.Sin(dizzinessAccum / 5f) * 0.2f) * f;

            // When mouse is in gui mode we need to manually update the players yaw, otherwise the fp hands dont rotate with the camera
            if (!capi.Input.MouseGrabbed)
            {
                capi.World.Player.Entity.Pos.Yaw = capi.Input.MouseYaw;
            }

            var hc = capi.World.Player.Entity.AnimManager.HeadController;

            hc.YawOffset = (float)(Math.Cos(dizzinessAccum / 1.12) + Math.Cos(dizzinessAccum / 1.2f) + Math.Cos(dizzinessAccum / 4f) * 0.2f) * f * 60f;
            dizzinessAccum /= 2;
            hc.PitchOffset = (float)(Math.Sin(dizzinessAccum / 1.12) + Math.Sin(dizzinessAccum / 1.2f) + Math.Sin(dizzinessAccum / 4f) * 0.2f) * f * 30f;


            double accum2 = (float)((capi.InWorldEllapsedMilliseconds / 9000.0) % 100 * Math.PI);
            float intox = capi.Render.ShaderUniforms.PerceptionEffectIntensity;

            float b = GameMath.Clamp((0.4f + (float)Math.Abs(Math.Cos(accum2 / 1.12) + Math.Sin(accum2 / 2.2) + Math.Cos(accum2 * 2.3))) * intox, 0f, 1.5f);
            capi.Render.ShaderUniforms.AmbientBloomLevelAdd[DefaultShaderUniforms.BloomAddPsychedelicIndex] = b;

        }
    }
}
