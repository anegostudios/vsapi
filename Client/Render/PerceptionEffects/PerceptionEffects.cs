using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    public class PerceptionEffects
    {
        private ICoreClientAPI capi;
        private int nextPerceptionEffectId=1;
        Dictionary<string, PerceptionEffect> registeredPerceptionEffects = new Dictionary<string, PerceptionEffect>();

        Dictionary<string, PerceptionEffect> activePerceptionEffects = new Dictionary<string, PerceptionEffect>();

        public PerceptionEffects(ICoreClientAPI capi)
        {
            this.capi = capi;

            RegisterPerceptionEffect(new DamagedPerceptionEffect(capi), "damaged");
            RegisterPerceptionEffect(new FreezingPerceptionEffect(capi), "freezing");
            RegisterPerceptionEffect(new DrunkPerceptionEffect(capi), "drunk");
        }

        public void RegisterPerceptionEffect(PerceptionEffect effect, string code)
        {
            effect.PerceptionEffectId = nextPerceptionEffectId++;
            effect.Code = code;
            registeredPerceptionEffects[code] = effect;
            
        }

        public ICollection<string> RegisteredEffects => registeredPerceptionEffects.Keys;

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
            TriggerEffect("damaged", 1, true);
            TriggerEffect("freezing", 1, true);
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
