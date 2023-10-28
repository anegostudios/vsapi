using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AnimatableRenderer : IRenderer
    {
        public double RenderOrder => 1;
        public int RenderRange => 99;

        protected Vec3d pos;
        protected ICoreClientAPI capi;
        protected AnimatorBase animator;
        protected Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode = new Dictionary<string, AnimationMetaData>();

        public MeshRef meshref;
        public int textureId;

        public float[] ModelMat = Mat4f.Create();

        public bool ShouldRender;
        public bool StabilityAffected = true;
        public bool LightAffected = true;
        public float FogAffectedness = 1f;
        public Vec3f rotationDeg;

        public float ScaleX = 1f;
        public float ScaleY = 1f;
        public float ScaleZ = 1f;

        public Vec4f renderColor = ColorUtil.WhiteArgbVec.Clone();

        public bool backfaceCulling = true;

        public AnimatableRenderer(ICoreClientAPI capi, Vec3d pos, Vec3f rotationDeg, AnimatorBase animator, Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, MeshRef meshref, int textureId, EnumRenderStage renderStage = EnumRenderStage.Opaque)
        {
            this.pos = pos;
            this.capi = capi;
            this.animator = animator;
            this.activeAnimationsByAnimCode = activeAnimationsByAnimCode;
            this.meshref = meshref;
            this.rotationDeg = rotationDeg;
            this.textureId = textureId;
            if (rotationDeg == null) this.rotationDeg = new Vec3f();

            capi.Event.EnqueueMainThreadTask(() =>
            {
                capi.Event.RegisterRenderer(this, renderStage, "animatable");
                capi.Event.RegisterRenderer(this, EnumRenderStage.ShadowFar, "animatable");
                capi.Event.RegisterRenderer(this, EnumRenderStage.ShadowNear, "animatable");
            }, "registerrenderers");
        }

        public void OnRenderFrame(float dt, EnumRenderStage stage)
        {
            if (!ShouldRender || meshref.Disposed || !meshref.Initialized) return;

            bool shadowPass = stage != EnumRenderStage.Opaque;
            
            EntityPlayer entityPlayer = capi.World.Player.Entity;

            Mat4f.Identity(ModelMat);
            Mat4f.Translate(ModelMat, ModelMat, (float)(pos.X - entityPlayer.CameraPos.X), (float)(pos.Y - entityPlayer.CameraPos.Y), (float)(pos.Z - entityPlayer.CameraPos.Z));

            Mat4f.Translate(ModelMat, ModelMat, 0.5f, 0, 0.5f);
            Mat4f.Scale(ModelMat, ModelMat, ScaleX, ScaleY, ScaleZ);
            Mat4f.RotateY(ModelMat, ModelMat, rotationDeg.Y * GameMath.DEG2RAD);
            Mat4f.Translate(ModelMat, ModelMat, -0.5f, 0, -0.5f);

            IRenderAPI rpi = capi.Render;
            IShaderProgram prevProg = rpi.CurrentActiveShader;
            prevProg?.Stop();

            capi.Render.GlDisableCullFace();

            IShaderProgram prog = rpi.GetEngineShader(shadowPass ? EnumShaderProgram.Shadowmapentityanimated : EnumShaderProgram.Entityanimated);
            prog.Use();
            Vec4f lightrgbs = LightAffected ? capi.World.BlockAccessor.GetLightRGBs((int)pos.X, (int)pos.Y, (int)pos.Z) : ColorUtil.WhiteArgbVec;
            rpi.GlToggleBlend(true, EnumBlendMode.Standard);

            if (!shadowPass)
            {
                prog.Uniform("extraGlow", (int)0);
                prog.Uniform("rgbaAmbientIn", rpi.AmbientColor);
                prog.Uniform("rgbaFogIn", rpi.FogColor);
                prog.Uniform("fogMinIn", rpi.FogMin * FogAffectedness);
                prog.Uniform("fogDensityIn", rpi.FogDensity * FogAffectedness);
                prog.Uniform("rgbaLightIn", lightrgbs);
                prog.Uniform("renderColor", renderColor);
                prog.Uniform("alphaTest", 0.1f);
                prog.UniformMatrix("modelMatrix", ModelMat);
                prog.UniformMatrix("viewMatrix", rpi.CameraMatrixOriginf);
                prog.Uniform("windWaveIntensity", (float)0);
                prog.Uniform("glitchEffectStrength", 0f);
                prog.Uniform("frostAlpha", 0f);
                if (!StabilityAffected)
                {
                    prog.Uniform("globalWarpIntensity", 0f);
                    prog.Uniform("glitchWaviness", 0f);
                }
            } else
            {
                prog.UniformMatrix("modelViewMatrix", Mat4f.Mul(new float[16], capi.Render.CurrentModelviewMatrix, ModelMat));
            }

            prog.BindTexture2D("entityTex", textureId, 0);
            prog.UniformMatrix("projectionMatrix", rpi.CurrentProjectionMatrix);
            prog.Uniform("addRenderFlags", 0);

            
            prog.UniformMatrices4x3(
                "elementTransforms",
                GlobalConstants.MaxAnimatedElements,
                animator.Matrices4x3
            );

            if ((stage == EnumRenderStage.Opaque || stage == EnumRenderStage.ShadowNear) && !backfaceCulling) capi.Render.GlDisableCullFace();

            capi.Render.RenderMesh(meshref);

            if ((stage == EnumRenderStage.Opaque || stage == EnumRenderStage.ShadowNear) && !backfaceCulling) capi.Render.GlEnableCullFace();

            prog.Stop();
            prevProg?.Use();
        }


        public void Dispose()
        {
            capi.Event.UnregisterRenderer(this, EnumRenderStage.Opaque);
            capi.Event.UnregisterRenderer(this, EnumRenderStage.ShadowFar);
            capi.Event.UnregisterRenderer(this, EnumRenderStage.ShadowNear);
            meshref?.Dispose();
        }

    }
}
