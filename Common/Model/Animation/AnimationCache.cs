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
        public Animation[] Animations;
        public ShapeElement[] RootElems;
        public List<ElementPose> RootPoses;
    }

    public static class AnimationCache
    {
        public static void ClearCache(ICoreAPI api)
        {
            api.ObjectCache["animCache"] = null;
        }

        public static IAnimationManager InitManager(ICoreAPI api, IAnimationManager manager, Entity entity, Shape entityShape)
        {
            if (entityShape == null)
            {
                return new NoAnimationManager();
            }

            string dictKey = entity.Code + "-" + entity.Properties.Client.Shape.Base.ToString();

            object animCacheObj;
            Dictionary<string, AnimCacheEntry> animCache = null;
            entity.Api.ObjectCache.TryGetValue("animCache", out animCacheObj);
            animCache = animCacheObj as Dictionary<string, AnimCacheEntry>;
            if (animCache == null)
            {
                entity.Api.ObjectCache["animCache"] = animCache = new Dictionary<string, AnimCacheEntry>();
            }

            IAnimator animator;

            AnimCacheEntry cacheObj = null;
            if (animCache.TryGetValue(dictKey, out cacheObj))
            {
                manager.Init(entity.Api, entity, entityShape);

                animator = api.Side == EnumAppSide.Client ? 
                    new ClientAnimator(entity, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, entityShape.JointsById) :
                    new ServerAnimator(entity, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, entityShape.JointsById)
                ;

                manager.Animator = animator;

            } else {

                manager.Init(entity.Api, entity, entityShape);

                for (int i = 0; entityShape.Animations != null && i < entityShape.Animations.Length; i++)
                {
                    entityShape.Animations[i].GenerateAllFrames(entityShape.Elements, entityShape.JointsById);
                }

                IAnimator animatorbase = api.Side == EnumAppSide.Client ?
                    new ClientAnimator(entity, entityShape.Animations, entityShape.Elements, entityShape.JointsById) :
                    new ServerAnimator(entity, entityShape.Animations, entityShape.Elements, entityShape.JointsById)
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
