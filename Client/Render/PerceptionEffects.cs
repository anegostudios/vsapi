using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public abstract class PerceptionEffect
    {
        public string Code;
        public int PerceptionEffectId;
        public float Intensity;

        public double DurationHours;

        protected ICoreClientAPI capi;

        public PerceptionEffect(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        public virtual void OnBeforeGameRender(float dt)
        {
            if (PerceptionEffectId > 0)
            {
                capi.Render.ShaderUniforms.PerceptionEffectId = PerceptionEffectId;
                capi.Render.ShaderUniforms.PerceptionEffectIntensity = Intensity;
            }
        }

        public virtual void OnOwnPlayerDataReceived(EntityPlayer eplr)
        {

        }

        public virtual void ApplyToFpHand(Matrixf modelMat)
        {

        }

        public virtual void ApplyToTpPlayer(EntityPlayer entityPlr, float[] modelMatrix, float? playerIntensity = null)
        {

        }

        public virtual void NowDisabled()
        {
            
        }

        public virtual void NowActive(float intensity)
        {
            this.Intensity = intensity;
        }
    }

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

            hc.dy = (float)(Math.Cos(accum / 1.12) + Math.Cos(accum / 1.2f) + Math.Cos(accum / 4f) * 0.2f) * f * 60f;
            accum /= 2;
            hc.dp = (float)(Math.Sin(accum / 1.12) + Math.Sin(accum / 1.2f) + Math.Sin(accum / 4f) * 0.2f) * f * 30f;
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

    public class DamagedAndFreezingPerceptionEffect : PerceptionEffect
    {
        internal long damangeVignettingUntil;
        internal float strength;
        internal int duration;
        float curFreezingVal;

        NormalizedSimplexNoise noisegen = NormalizedSimplexNoise.FromDefaultOctaves(4, 1, 0.9, 123);

        public DamagedAndFreezingPerceptionEffect(ICoreClientAPI capi) : base(capi)
        {
        }

        public override void OnOwnPlayerDataReceived(EntityPlayer eplr)
        {
            eplr.WatchedAttributes.RegisterModifiedListener("onHurt", onHurt);
        }

        public override void OnBeforeGameRender(float dt)
        {
            dt = Math.Min(dt, 1);

            var eplr = capi.World.Player;

            if (!eplr.Entity.Alive)
            {
                capi.Render.ShaderUniforms.ExtraSepia += (2f - capi.Render.ShaderUniforms.ExtraSepia) * Math.Min(1, dt * 5);
                capi.Render.ShaderUniforms.DamageVignetting += (1.25f - capi.Render.ShaderUniforms.DamageVignetting) * Math.Min(1, dt * 5);
                capi.Render.ShaderUniforms.DamageVignettingSide += (0f - capi.Render.ShaderUniforms.DamageVignettingSide) * Math.Min(1, dt * 5);
            } else {

                if (!capi.IsGamePaused)
                {
                    ITreeAttribute healthTree = eplr.Entity.WatchedAttributes.GetTreeAttribute("health");
                    float healthRel = healthTree == null ? 1 : healthTree.GetFloat("currenthealth") / healthTree.GetFloat("maxhealth");

                    float f = Math.Max(0, (0.23f - healthRel) * 1 / 0.18f);
                    float lowHealthness = 0;

                    if (f > 0)
                    {
                        float ellapseSec = (float)(capi.InWorldEllapsedMilliseconds / 1000.0);

                        float bla = (float)noisegen.Noise(12412, ellapseSec / 2) * 0.5f + (float)Math.Pow(Math.Abs(GameMath.Sin(ellapseSec * 1 / 0.7f)), 30) * 0.5f;
                        lowHealthness = Math.Min(f * 1.5f, 1) * (bla * 0.75f + 0.5f);

                        if (eplr.Entity.Alive)
                        {
                            capi.Render.ShaderUniforms.ExtraSepia = GameMath.Clamp(f * (float)noisegen.Noise(0, ellapseSec / 3) * 1.2f, 0, 1.2f);
                            if (capi.World.Rand.NextDouble() < 0.01)
                            {
                                capi.World.AddCameraShake(0.15f * f);
                            }

                            capi.Input.MouseYaw += f * (float)(noisegen.Noise(76, ellapseSec / 50) - 0.5f) * 0.003f;

                            float dp = f * (float)(noisegen.Noise(ellapseSec / 50, 987) - 0.5f) * 0.003f; ;

                            eplr.Entity.Pos.Pitch += dp;
                            capi.Input.MousePitch += dp;
                        }
                    }
                    else
                    {
                        capi.Render.ShaderUniforms.ExtraSepia = 0;
                    }

                    int val = GameMath.Clamp((int)(damangeVignettingUntil - capi.ElapsedMilliseconds), 0, duration);

                    capi.Render.ShaderUniforms.DamageVignetting = GameMath.Clamp(GameMath.Clamp(strength / 2, 0.5f, 3.5f) * ((float)val / Math.Max(1, duration)) + lowHealthness, 0, 1.5f);
                }


                float freezing = eplr.Entity.WatchedAttributes.GetFloat("freezingEffectStrength", 0);

                curFreezingVal += (freezing - curFreezingVal) * dt;

                if (curFreezingVal > 0.1 && eplr.CameraMode == EnumCameraMode.FirstPerson)
                {
                    float ellapseSec = (float)(capi.InWorldEllapsedMilliseconds / 1000.0);
                    capi.Input.MouseYaw += capi.Settings.Float["cameraShakeStrength"] * (float)(Math.Max(0, noisegen.Noise(ellapseSec, 12) - 0.4f) * Math.Sin(ellapseSec * 90) * 0.01) * GameMath.Clamp(curFreezingVal * 3, 0, 1);
                }

                capi.Render.ShaderUniforms.FrostVignetting = curFreezingVal;
            }
        }

        private void onHurt()
        {
            var eplr = capi.World.Player.Entity;
            strength = eplr.WatchedAttributes.GetFloat("onHurt");
            
            if (strength == 0 || capi.World.Player.Entity.RemainingActivityTime("invulnerable") <= 0) return;

            duration = GameMath.Clamp(200 + (int)(strength * 10), 200, 600) * 3;
            damangeVignettingUntil = capi.ElapsedMilliseconds + duration;
                        
            float angle = eplr.WatchedAttributes.GetFloat("onHurtDir");
            if (angle < -99)
            {
                capi.Render.ShaderUniforms.DamageVignettingSide = 0;
            }
            else
            {
                float angleDist = GameMath.AngleRadDistance(eplr.Pos.Yaw - GameMath.PIHALF, angle);
                capi.Render.ShaderUniforms.DamageVignettingSide = GameMath.Clamp(angleDist / GameMath.PIHALF, -1, 1);
            }
        }

    }


    public class PerceptionEffects
    {
        private ICoreClientAPI capi;
        private int nextPerceptionEffectId=1;
        Dictionary<string, PerceptionEffect> registeredPerceptionEffects = new Dictionary<string, PerceptionEffect>();

        Dictionary<string, PerceptionEffect> activePerceptionEffects = new Dictionary<string, PerceptionEffect>();

        public PerceptionEffects(ICoreClientAPI capi)
        {
            this.capi = capi;

            RegisterPerceptionEffect(new DamagedAndFreezingPerceptionEffect(capi), "damagedfreezing");
            RegisterPerceptionEffect(new DrunkPerceptionEffect(capi), "drunk");
        }

        public void RegisterPerceptionEffect(PerceptionEffect effect, string code)
        {
            effect.PerceptionEffectId = nextPerceptionEffectId++;
            effect.Code = code;
            registeredPerceptionEffects[code] = effect;
        }


        public void TriggerEffect(string code, float intensity, bool? on = null)
        {
            bool ison = (on == null) ? !activePerceptionEffects.ContainsKey(code) : (bool)on;

            if (ison)
            {
                activePerceptionEffects[code] = registeredPerceptionEffects[code];
                activePerceptionEffects[code].NowActive(intensity);
            } else
            {
                if (activePerceptionEffects.ContainsKey(code))
                {
                    activePerceptionEffects[code].NowDisabled();
                    activePerceptionEffects.Remove(code);

                    capi.Render.ShaderUniforms.PerceptionEffectId = 0;
                }

                
            }
            
        }


        public void OnBeforeGameRender(float dt)
        {
            foreach (var effect in activePerceptionEffects)
            {
                effect.Value.OnBeforeGameRender(dt);
            }
        }


        public void OnOwnPlayerDataReceived(EntityPlayer eplr)
        {
            TriggerEffect("damagedfreezing", 1, true);
            TriggerEffect("drunk", 0, true);

            foreach (var effect in activePerceptionEffects)
            {
                effect.Value.OnOwnPlayerDataReceived(eplr);
            }
        }


        public void ApplyToFpHand(Matrixf modelMat)
        {
            foreach (var effect in activePerceptionEffects)
            {
                effect.Value.ApplyToFpHand(modelMat);
            }
        }

        public void ApplyToTpPlayer(EntityPlayer entityPlr, float[] modelMatrix, float? playerIntensity = null)
        {
            foreach (var effect in activePerceptionEffects)
            {
                effect.Value.ApplyToTpPlayer(entityPlr, modelMatrix, playerIntensity);
            }
        }


    }
}
