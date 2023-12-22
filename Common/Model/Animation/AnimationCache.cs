using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public class AnimCacheEntry
    {
        /// <summary>
        /// Animations of this cache.
        /// </summary>
        public Animation[] Animations;

        /// <summary>
        /// The root elements of this cache.
        /// </summary>
        public ShapeElement[] RootElems;

        /// <summary>
        /// The poses of this cache
        /// </summary>
        public List<ElementPose> RootPoses;
    }

    public static class AnimationCache
    {
        /// <summary>
        /// Clears the animation cache.
        /// </summary>
        /// <param name="api"></param>
        public static void ClearCache(ICoreAPI api)
        {
            api.ObjectCache["animCache"] = null;
        }

        /// <summary>
        /// Clears the animation cache.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="entity"></param>
        public static void ClearCache(ICoreAPI api, Entity entity)
        {
            var animCache = ObjectCacheUtil.GetOrCreate(api, "animCache", () => new Dictionary<string, AnimCacheEntry>());
            string dictKey = entity.Code + "-" + entity.Properties.Client.ShapeForEntity.Base.ToString();
            animCache.Remove(dictKey);
        }

        /// <summary>
        /// Initializes the cache to the Animation Manager then spits it back out.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="manager"></param>
        /// <param name="entity"></param>
        /// <param name="entityShape"></param>
        /// <param name="copyOverAnims"></param>
        /// <param name="requireJointsForElements"></param>
        /// <returns></returns>
        public static IAnimationManager InitManager(ICoreAPI api, IAnimationManager manager, Entity entity, Shape entityShape, RunningAnimation[] copyOverAnims, params string[] requireJointsForElements)
        {
            if (entityShape == null)
            {
                return new NoAnimationManager();
            }

            string dictKey = entity.Code + "-" + entity.Properties.Client.ShapeForEntity.Base.ToString();

            var animCache = ObjectCacheUtil.GetOrCreate(api, "animCache", () => new Dictionary<string, AnimCacheEntry>());

            IAnimator animator = null;

            AnimCacheEntry cacheObj;
            if (animCache.TryGetValue(dictKey, out cacheObj))
            {
                manager.Init(entity.Api, entity);
                
                animator = api.Side == EnumAppSide.Client ? 
                    ClientAnimator.CreateForEntity(entity, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, entityShape.JointsById) :
                    ServerAnimator.CreateForEntity(entity, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, entityShape.JointsById)
                ;

                manager.Animator = animator;

            } else {

                entityShape.ResolveAndFindJoints(api.Logger, entity.Properties.Client.ShapeForEntity.Base.ToString(), requireJointsForElements);

                manager.Init(entity.Api, entity);

                IAnimator animatorbase = api.Side == EnumAppSide.Client ?
                    ClientAnimator.CreateForEntity(entity, entityShape.Animations, entityShape.Elements, entityShape.JointsById) :
                    ServerAnimator.CreateForEntity(entity, entityShape.Animations, entityShape.Elements, entityShape.JointsById)
                ;

                manager.Animator = animator = animatorbase;


                animCache[dictKey] = new AnimCacheEntry()
                {
                    Animations = entityShape.Animations,
                    RootElems = (animatorbase as ClientAnimator).rootElements,
                    RootPoses = (animatorbase as ClientAnimator).RootPoses
                };
            }

            if (copyOverAnims != null && animator != null)
            {
                for (int i = 0; i < copyOverAnims.Length; i++)
                {
                    var sourceAnim = copyOverAnims[i];
                    if (sourceAnim != null && sourceAnim.Active)
                    {
                        manager.ActiveAnimationsByAnimCode.TryGetValue(sourceAnim.Animation.Code, out var meta);
                        if (meta != null)
                        {
                            meta.StartFrameOnce = sourceAnim.CurrentFrame;
                        }
                    }
                }
            }
            
            return manager;
        }

    }
}
