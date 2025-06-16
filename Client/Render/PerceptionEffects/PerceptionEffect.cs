using Vintagestory.API.Common;

#nullable disable

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
}
