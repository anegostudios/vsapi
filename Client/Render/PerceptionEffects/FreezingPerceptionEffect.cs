using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class FreezingPerceptionEffect : PerceptionEffect
    {
        private float currentStrength;
        private readonly NormalizedSimplexNoise noiseGenerator;

        public FreezingPerceptionEffect(ICoreClientAPI capi) : base(capi)
        {
            noiseGenerator = NormalizedSimplexNoise.FromDefaultOctaves(4, 1, 0.9, 123);
        }

        public override void OnBeforeGameRender(float dt)
        {
            if (capi.IsGamePaused) return;
            HandleFreezingEffects(Math.Min(dt, 1));
        }

        private void HandleFreezingEffects(float dt)
        {
            var strength = capi.World.Player.Entity.WatchedAttributes.GetFloat("freezingEffectStrength");
            currentStrength += (strength - currentStrength) * dt;

            ApplyFrostVignette(currentStrength);

            if (!(currentStrength > 0.1) || capi.World.Player.CameraMode != EnumCameraMode.FirstPerson) return;

            ApplyMotionEffects(currentStrength);
        }

        private void ApplyFrostVignette(float strength)
        {
            capi.Render.ShaderUniforms.FrostVignetting = strength;
        }

        private void ApplyMotionEffects(float strength)
        {
            var elapsedSeconds = capi.InWorldEllapsedMilliseconds / 1000f;
            capi.Input.MouseYaw +=
                capi.Settings.Float["cameraShakeStrength"] *
                (float)(Math.Max(0, noiseGenerator.Noise(elapsedSeconds, 12) - 0.4f) *
                        Math.Sin(elapsedSeconds * 90) * 0.01) *
                GameMath.Clamp(strength * 3, 0, 1);
        }
    }
}
