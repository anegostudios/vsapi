using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

#nullable disable

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

        public static IAnimationManager LoadAnimatorCached(this IAnimationManager manager, ICoreAPI api, Entity entity, Shape entityShape, RunningAnimation[] copyOverAnims, bool requirePosesOnServer, params string[] requireJointsForElements)
        {
            return InitManager(api, manager, entity, entityShape, copyOverAnims, requirePosesOnServer, requireJointsForElements);
        }

        /// <summary>
        /// Initializes the cache to the Animation Manager then spits it back out.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="manager"></param>
        /// <param name="entity"></param>
        /// <param name="entityShape"></param>
        /// <param name="copyOverAnims"></param>
        /// <param name="requirePosesOnServer"></param>
        /// <param name="requireJointsForElements"></param>
        /// <returns></returns>
        [Obsolete("Use manager.LoadAnimator() or manager.LoadAnimatorCached() instead")]
        public static IAnimationManager InitManager(ICoreAPI api, IAnimationManager manager, Entity entity, Shape entityShape, RunningAnimation[] copyOverAnims, bool requirePosesOnServer, params string[] requireJointsForElements)
        {
            if (entityShape == null)
            {
                return new NoAnimationManager();
            }

            string dictKey = entity.Code + "-" + entity.Properties.Client.ShapeForEntity.Base.ToString();

            var animCache = ObjectCacheUtil.GetOrCreate(api, "animCache", () => new Dictionary<string, AnimCacheEntry>());

            entityShape.InitForAnimations(api.Logger, entity.Properties.Client.ShapeForEntity.Base.ToString(), requireJointsForElements);
            IAnimator animator = null;

            if (animCache.TryGetValue(dictKey, out AnimCacheEntry cacheObj))
            {
                manager.Init(entity.Api, entity);

                animator = api.Side == EnumAppSide.Client ?
                    ClientAnimator.CreateForEntity(entity, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, entityShape.JointsById) :
                    ServerAnimator.CreateForEntity(entity, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, entityShape.JointsById, requirePosesOnServer)
                ;

                manager.Animator = animator;
                manager.CopyOverAnimStates(copyOverAnims, animator);
            }
            else
            {
                animator = manager.LoadAnimator(api, entity, entityShape, copyOverAnims, requirePosesOnServer, requireJointsForElements);

                animCache[dictKey] = new AnimCacheEntry()
                {
                    Animations = entityShape.Animations,
                    RootElems = (animator as AnimatorBase).RootElements,
                    RootPoses = (animator as AnimatorBase).RootPoses
                };
            }

            return manager;
        }



    }
}
