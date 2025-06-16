using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class DamagedPerceptionEffect : PerceptionEffect
    {
        private long damageVignettingUntil;
        private float strength;
        private int duration;

        private readonly NormalizedSimplexNoise noiseGenerator;

        public DamagedPerceptionEffect(ICoreClientAPI capi) : base(capi)
        {
            noiseGenerator = NormalizedSimplexNoise.FromDefaultOctaves(4, 1, 0.9, 123);
        }

        public override void OnOwnPlayerDataReceived(EntityPlayer player)
        {
            player.WatchedAttributes.RegisterModifiedListener("onHurt", OnHurt);
        }

        private void OnHurt()
        {
            var player = capi.World.Player.Entity;
            strength = player.WatchedAttributes.GetFloat("onHurt");

            if (strength == 0 || capi.World.Player.Entity.RemainingActivityTime("invulnerable") <= 0) return;

            duration = GameMath.Clamp(200 + (int)(strength * 10), 200, 600) * 3;
            damageVignettingUntil = capi.ElapsedMilliseconds + duration;

            var angle = player.WatchedAttributes.GetFloat("onHurtDir");
            if (angle < -99)
            {
                capi.Render.ShaderUniforms.DamageVignettingSide = 0;
            }
            else
            {
                float angleDist = GameMath.AngleRadDistance(player.Pos.Yaw - GameMath.PIHALF, angle);
                capi.Render.ShaderUniforms.DamageVignettingSide = GameMath.Clamp(angleDist / GameMath.PIHALF, -1, 1);
            }
        }

        public override void OnBeforeGameRender(float dt)
        {
            if (capi.IsGamePaused) return;
            HandleDamageEffects(Math.Min(dt, 1));
        }

        private void HandleDamageEffects(float dt)
        {
            if (!capi.World.Player.Entity.Alive)
            {
                ApplyDeathEffects(dt);
                return;
            }

            var healthThreshold = CalculateHealthThreshold();
            var elapsedSeconds = capi.InWorldEllapsedMilliseconds / 1000f;

            ApplyDamageSepiaEffect(healthThreshold, elapsedSeconds);
            ApplyDamageVignette(elapsedSeconds, healthThreshold);
            ApplyMotionEffects(healthThreshold, elapsedSeconds);
        }

        private void ApplyDeathEffects(float dt)
        {
            capi.Render.ShaderUniforms.ExtraSepia +=
                (2f - capi.Render.ShaderUniforms.ExtraSepia) * Math.Min(1, dt * 5);

            capi.Render.ShaderUniforms.DamageVignetting +=
                (1.25f - capi.Render.ShaderUniforms.DamageVignetting) * Math.Min(1, dt * 5);

            capi.Render.ShaderUniforms.DamageVignettingSide +=
                (0f - capi.Render.ShaderUniforms.DamageVignettingSide) * Math.Min(1, dt * 5);
        }

        private float CalculateHealthThreshold()
        {
            var healthTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("health");
            var percentageHealthLeft = healthTree?.GetFloat("currenthealth") / healthTree?.GetFloat("maxhealth") ?? 1f;
            return Math.Max(0f, (0.23f - percentageHealthLeft) * 1f / 0.18f);
        }

        private void ApplyDamageSepiaEffect(float healthThreshold, float elapsedSeconds)
        {
            capi.Render.ShaderUniforms.ExtraSepia = healthThreshold <= 0 ? 0 : GameMath.Clamp(healthThreshold * (float)noiseGenerator.Noise(0, elapsedSeconds / 3) * 1.2f, 0, 1.2f);
        }

        private void ApplyDamageVignette(float elapsedSeconds, float healthThreshold)
        {
            var val = GameMath.Clamp((int)(damageVignettingUntil - capi.ElapsedMilliseconds), 0, duration);
            var noise = (float)noiseGenerator.Noise(12412, elapsedSeconds / 2) * 0.5f + (float)Math.Pow(Math.Abs(GameMath.Sin(elapsedSeconds * 1 / 0.7f)), 30) * 0.5f;

            var lowHealth = Math.Min(healthThreshold * 1.5f, 1) * (noise * 0.75f + 0.5f);

            capi.Render.ShaderUniforms.DamageVignetting = GameMath.Clamp(
                GameMath.Clamp(strength / 2, 0.5f, 3.5f) * ((float)val / Math.Max(1, duration)) + lowHealth, 
                0, 1.5f
            );
        }

        private void ApplyMotionEffects(float healthThreshold, float elapsedSeconds)
        {
            if (healthThreshold <= 0) return;

            if (capi.World.Rand.NextDouble() < 0.01)
            {
                capi.World.AddCameraShake(0.15f * healthThreshold);
            }
            capi.Input.MouseYaw += healthThreshold * (float)(noiseGenerator.Noise(76, elapsedSeconds / 50) - 0.5f) * 0.003f;
            var dp = healthThreshold * (float)(noiseGenerator.Noise(elapsedSeconds / 50, 987) - 0.5f) * 0.003f;
            capi.World.Player.Entity.Pos.Pitch += dp;
            capi.Input.MousePitch += dp;
        }
    }
}
