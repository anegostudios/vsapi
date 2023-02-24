using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class DrunkPerceptionEffect : PerceptionEffect
    {
        NormalizedSimplexNoise noisegen = NormalizedSimplexNoise.FromDefaultOctaves(4, 1, 0.9, 123);

        public DrunkPerceptionEffect(ICoreClientAPI capi) : base(capi)
        {

        }

        float accum;
        float accum1s;
        float targetIntensity;

        public override void OnBeforeGameRender(float dt)
        {
            if (capi.IsGamePaused) return;

            capi.Render.ShaderUniforms.PerceptionEffectIntensity = Intensity;

            accum1s += dt;
            if (accum1s > 1)
            {
                accum1s = 0;
                targetIntensity = capi.World.Player.Entity.WatchedAttributes.GetFloat("intoxication");
            }

            Intensity += (targetIntensity - Intensity) * dt / 3;
            
            accum = (float)((capi.InWorldEllapsedMilliseconds / 3000.0) % 100 * Math.PI);

            float f = Intensity / 250f;
            float dp = (float)(Math.Cos(accum / 1.15) + Math.Cos(accum / 1.35f)) * f / 2;

            capi.World.Player.Entity.Pos.Pitch += dp;
            capi.Input.MousePitch += dp;
            capi.Input.MouseYaw += (float)(Math.Sin(accum / 1.1) + Math.Sin(accum / 1.5f) + Math.Sin(accum / 5f) * 0.2f) * f;

            var hc = capi.World.Player.Entity.AnimManager.HeadController;

            hc.yawOffset = (float)(Math.Cos(accum / 1.12) + Math.Cos(accum / 1.2f) + Math.Cos(accum / 4f) * 0.2f) * f * 60f;
            accum /= 2;
            hc.pitchOffset = (float)(Math.Sin(accum / 1.12) + Math.Sin(accum / 1.2f) + Math.Sin(accum / 4f) * 0.2f) * f * 30f;
        }

        public override void ApplyToFpHand(Matrixf modelMat)
        {
            float f = Intensity / 10f;

            modelMat.Translate(GameMath.Sin(accum) * f, GameMath.Sin(accum) * 1.2 * f, 0);
            modelMat.RotateX(GameMath.Cos(accum * 0.8f) * f);
            modelMat.RotateZ(GameMath.Cos(accum * 1.1f) * f);
        }

        public override void ApplyToTpPlayer(EntityPlayer entityPlr, float[] modelMatrix, float? playerIntensity = null)
        {
            float inten = playerIntensity == null ? Intensity : (float)playerIntensity;

            Mat4f.RotateX(modelMatrix, modelMatrix, GameMath.Sin(accum) / 6f * inten);
            Mat4f.RotateZ(modelMatrix, modelMatrix, GameMath.Sin(accum * 1.2f) / 6f * inten);
        }

        public override void NowActive(float intensity)
        {
            base.NowActive(intensity);

            capi.Render.ShaderUniforms.PerceptionEffectId = 2;
            
        }


    }
}
