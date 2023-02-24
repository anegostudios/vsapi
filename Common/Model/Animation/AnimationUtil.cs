using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AnimationUtil : IRenderer
    {
        public AnimatorBase animator;
        public AnimatableRenderer renderer;
        public Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode = new Dictionary<string, AnimationMetaData>();
        protected ICoreAPI api;
        protected bool stopRenderTriggered = false;
        protected ICoreClientAPI capi;
        protected Vec3d position;
        protected virtual int RenderTextureId => capi.BlockTextureAtlas.AtlasTextures[0].TextureId;

        public double RenderOrder => 1;
        public int RenderRange => 99;

        public AnimationUtil(ICoreAPI api, Vec3d position)
        {
            this.api = api;
            this.position = position;
            capi = api as ICoreClientAPI;
            (api as ICoreClientAPI)?.Event.RegisterRenderer(this, EnumRenderStage.Opaque, "beanimutil");
        }

        public virtual void InitializeShapeAndAnimator(string cacheDictKey, Shape shape, ITexPositionSource texSource, Vec3f rotation, out MeshData meshdata)
        {
            shape.ResolveReferences(api.World.Logger, cacheDictKey);
            CacheInvTransforms(shape.Elements);
            shape.ResolveAndLoadJoints();

            TesselationMetaData meta = new TesselationMetaData()
            {
                //QuantityElements = compositeShape.QuantityElements,
                //SelectiveElements = compositeShape.SelectiveElements,
                TexSource = texSource,
                WithJointIds = true,
                WithDamageEffect = true,
                TypeForLogging = cacheDictKey,
                Rotation = rotation
            };

            capi.Tesselator.TesselateShape(meta, shape, out var mesh);
            meshdata = mesh;

            InitializeAnimator(cacheDictKey, meshdata, shape, rotation);
        }

        public virtual void InitializeAnimator(string cacheDictKey, MeshData meshdata, Shape shape, Vec3f rotation, EnumRenderStage renderStage = EnumRenderStage.Opaque) 
        {
            if (meshdata == null)
            {
                throw new ArgumentException("meshdata cannot be null");
            }

            animator = GetAnimator(api, cacheDictKey, shape);
            renderer?.Dispose();

            if (RuntimeEnv.MainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                renderer = new AnimatableRenderer(capi, position, rotation, animator, activeAnimationsByAnimCode, capi.Render.UploadMesh(meshdata), RenderTextureId, renderStage);
            } else
            {
                renderer = new AnimatableRenderer(capi, position, rotation, animator, activeAnimationsByAnimCode, null, RenderTextureId, renderStage);
                capi.Event.EnqueueMainThreadTask(() => {
                    renderer.meshref = capi.Render.UploadMesh(meshdata);
                }, "uploadmesh");
            }
        }


        public virtual void InitializeAnimator(string cacheDictKey, MeshRef meshref, Shape blockShape, Vec3f rotation, EnumRenderStage renderStage = EnumRenderStage.Opaque)
        {
            if (api.Side != EnumAppSide.Client) throw new NotImplementedException("Server side animation system not implemented yet.");

            animator = GetAnimator(api, cacheDictKey, blockShape);
            renderer = new AnimatableRenderer(capi, position, rotation, animator, activeAnimationsByAnimCode, meshref, RenderTextureId, renderStage);
        }

        public virtual void InitializeAnimatorServer(string cacheDictKey, Shape blockShape)
        {
            animator = GetAnimator(api, cacheDictKey, blockShape);
        }




        public void AnimationTickServer(float deltaTime)
        {
            if (animator == null) return; // not initialized yet

            if (activeAnimationsByAnimCode.Count > 0 || animator.ActiveAnimationCount > 0)
            {
                animator.OnFrame(activeAnimationsByAnimCode, deltaTime);
            }
        }

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            if (animator == null || renderer == null) return; // not initialized yet

            if (activeAnimationsByAnimCode.Count > 0 || animator.ActiveAnimationCount > 0)
            {
                animator.OnFrame(activeAnimationsByAnimCode, deltaTime);
            }

            if (activeAnimationsByAnimCode.Count == 0 && animator.ActiveAnimationCount == 0 && renderer.ShouldRender && !stopRenderTriggered)
            {
                stopRenderTriggered = true;
                OnAnimationsStateChange(false);
            }
        }

        public virtual bool StartAnimation(AnimationMetaData meta)
        {
            if (!activeAnimationsByAnimCode.ContainsKey(meta.Code))
            {
                stopRenderTriggered = false;
                activeAnimationsByAnimCode[meta.Code] = meta;
                OnAnimationsStateChange(true);
                return true;
            }

            return false;
        }

        protected virtual void OnAnimationsStateChange(bool animsNowActive)
        {

        }


        public void StopAnimation(string code)
        {
            activeAnimationsByAnimCode.Remove(code);
        }


        public static AnimatorBase GetAnimator(ICoreAPI api, string cacheDictKey, Shape shape)
        {
            if (shape == null)
            {
                return null;
            }

            cacheDictKey = "animutil-" + cacheDictKey;

            object animCacheObj;
            Dictionary<string, AnimCacheEntry> animCache = null;
            api.ObjectCache.TryGetValue("animUtil-animCache", out animCacheObj);
            animCache = animCacheObj as Dictionary<string, AnimCacheEntry>;
            if (animCache == null)
            {
                api.ObjectCache["animUtil-animCache"] = animCache = new Dictionary<string, AnimCacheEntry>();
            }

            AnimatorBase animator;

            AnimCacheEntry cacheObj = null;
            if (animCache.TryGetValue(cacheDictKey, out cacheObj))
            {
                animator = api.Side == EnumAppSide.Client ?
                    new ClientAnimator(() => 1, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, shape.JointsById) :
                    new ServerAnimator(() => 1, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, shape.JointsById)
                ;
            }
            else
            {
                for (int i = 0; shape.Animations != null && i < shape.Animations.Length; i++)
                {
                    shape.Animations[i].GenerateAllFrames(shape.Elements, shape.JointsById);
                }

                animator = api.Side == EnumAppSide.Client ?
                    new ClientAnimator(() => 1, shape.Animations, shape.Elements, shape.JointsById) :
                    new ServerAnimator(() => 1, shape.Animations, shape.Elements, shape.JointsById)
                ;

                animCache[cacheDictKey] = new AnimCacheEntry()
                {
                    Animations = shape.Animations,
                    RootElems = (animator as ClientAnimator).rootElements,
                    RootPoses = (animator as ClientAnimator).RootPoses
                };
            }


            return animator;
        }


        public static void CacheInvTransforms(ShapeElement[] elements)
        {
            if (elements == null) return;

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].CacheInverseTransformMatrix();
                CacheInvTransforms(elements[i].Children);
            }
        }

        public void Dispose()
        {
            renderer?.Dispose();
            (api as ICoreClientAPI)?.Event.UnregisterRenderer(this, EnumRenderStage.Opaque);
        }


    }
}
