using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

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
        /// Initializes the cache to the Animation Manager then spits it back out.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="manager"></param>
        /// <param name="entity"></param>
        /// <param name="entityShape"></param>
        /// <param name="requireJointsForElements"></param>
        /// <returns></returns>
        public static IAnimationManager InitManager(ICoreAPI api, IAnimationManager manager, Entity entity, Shape entityShape, params string[] requireJointsForElements)
        {
            if (entityShape == null)
            {
                return new NoAnimationManager();
            }

            string dictKey = entity.Code + "-" + entity.Properties.Client.ShapeForEntity.Base.ToString();

            object animCacheObj;
            Dictionary<string, AnimCacheEntry> animCache;
            entity.Api.ObjectCache.TryGetValue("animCache", out animCacheObj);
            animCache = animCacheObj as Dictionary<string, AnimCacheEntry>;
            if (animCache == null)
            {
                entity.Api.ObjectCache["animCache"] = animCache = new Dictionary<string, AnimCacheEntry>();
            }

            IAnimator animator;

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

                entityShape.ResolveAndLoadJoints(requireJointsForElements);

                manager.Init(entity.Api, entity);

                IAnimator animatorbase = api.Side == EnumAppSide.Client ?
                    ClientAnimator.CreateForEntity(entity, entityShape.Animations, entityShape.Elements, entityShape.JointsById) :
                    ServerAnimator.CreateForEntity(entity, entityShape.Animations, entityShape.Elements, entityShape.JointsById)
                ;

                manager.Animator = animatorbase;


                animCache[dictKey] = new AnimCacheEntry()
                {
                    Animations = entityShape.Animations,
                    RootElems = (animatorbase as ClientAnimator).rootElements,
                    RootPoses = (animatorbase as ClientAnimator).RootPoses
                };
            }
            
            return manager;
        }

    }
}
