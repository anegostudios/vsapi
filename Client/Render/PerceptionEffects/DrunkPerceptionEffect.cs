using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

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
            if (capi.IsGamePaused || capi.World.Player.Entity.AnimManager.HeadController == null) return;

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

            // When mouse is in gui mode we need to manually update the players yaw, otherwise the fp hands dont rotate with the camera
            if (!capi.Input.MouseGrabbed)
            {
                capi.World.Player.Entity.Pos.Yaw = capi.Input.MouseYaw;
            }

            var hc = capi.World.Player.Entity.AnimManager.HeadController;

            hc.yawOffset = (float)(Math.Cos(accum / 1.12) + Math.Cos(accum / 1.2f) + Math.Cos(accum / 4f) * 0.2f) * f * 60f;
            accum /= 2;
            hc.pitchOffset = (float)(Math.Sin(accum / 1.12) + Math.Sin(accum / 1.2f) + Math.Sin(accum / 4f) * 0.2f) * f * 30f;

            hc.pitchOffset = (float)(Math.Sin(accum / 1.12) + Math.Sin(accum / 1.2f) + Math.Sin(accum / 4f) * 0.2f) * f * 30f;


            double accum2 = (float)((capi.InWorldEllapsedMilliseconds / 9000.0) % 100 * Math.PI);
            float intox = capi.Render.ShaderUniforms.PerceptionEffectIntensity;
            capi.Render.ShaderUniforms.AmbientBloomLevelAdd[DefaultShaderUniforms.BloomAddDrunkIndex] = GameMath.Clamp((float)Math.Abs(Math.Cos(accum2 / 1.12) + Math.Sin(accum2 / 2.2) + Math.Cos(accum2 * 2.3)) * intox * 2, intox / 3f, 1.8f);
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
            var rplr = entityPlr.Player as IClientPlayer;
            if (rplr == null || entityPlr.AnimManager.Animator == null || (rplr.CameraMode == EnumCameraMode.FirstPerson && !rplr.ImmersiveFpMode)) return;

            float inten = playerIntensity == null ? Intensity : (float)playerIntensity;

            var pos = entityPlr.AnimManager.Animator.GetPosebyName("root");
            pos.degOffX = GameMath.Sin(accum) / 5f * inten * GameMath.RAD2DEG;
            pos.degOffZ = GameMath.Sin(accum * 1.2f) / 5f * inten * GameMath.RAD2DEG;
        }

        public override void NowActive(float intensity)
        {
            base.NowActive(intensity);

            capi.Render.ShaderUniforms.PerceptionEffectId = 2;
        }


    }
}
