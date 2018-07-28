using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;

namespace Vintagestory.API.Common
{
    public class ServerEntityAnimator
    {
        protected RunningAnimation[] anims;
        protected Entity entity;
        protected EntityAgent entityAgent;

        protected int curAnimCount = 0;
        public RunningAnimation[] curAnims = new RunningAnimation[20];


        public int ActiveAnimationCount
        {
            get { return curAnimCount; }
        }


        public ServerEntityAnimator(Entity entity, Animation[] Animations)
        {
            this.entity = entity;

            if (entity is EntityAgent)
            {
                entityAgent = entity as EntityAgent;
            }

            anims = new RunningAnimation[Animations == null ? 0 : Animations.Length];

            for (int i = 0; i < anims.Length; i++)
            {
                anims[i] = new RunningAnimation()
                {
                    Active = false,
                    Running = false,
                    Animation = Animations[i],
                    CurrentFrame = 0
                };
            }
        }


        public virtual void OnFrame(float dt)
        {
            curAnimCount = 0;
            double walkSpeed = entityAgent == null ? 1f : entityAgent.Controls.MovespeedMultiplier * entityAgent.GetWalkSpeedMultiplier(0.3);

            for (int i = 0; i < anims.Length; i++)
            {
                RunningAnimation anim = anims[i];

                AnimationMetaData animData = null;
                entity.ActiveAnimationsByAnimCode.TryGetValue(anim.Animation.Code, out animData);

                bool wasActive = anim.Active;
                anim.Active = animData != null;

                // Animation got started
                if (!wasActive && anim.Active)
                {
                    AnimNowActive(anim, animData);
                }

                // Animation got stopped
                if (wasActive && !anim.Active)
                {
                    if (anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.Rewind)
                    {
                        anim.ShouldRewind = true;
                    }

                    if (anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.Stop)
                    {
                        anim.Stop();
                        entity.ActiveAnimationsByAnimCode.Remove(anim.Animation.Code);
                    }

                    if (anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.PlayTillEnd)
                    {
                        anim.ShouldPlayTillEnd = true;
                    }
                }

                if (!anim.Running) continue;

                bool shouldStop =
                    (anim.Iterations > 0 && anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Stop) ||
                    (anim.Iterations > 0 && !anim.Active && anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.PlayTillEnd && anim.EasingFactor < 0.01f) ||
                    (anim.Iterations < 0 && !anim.Active && anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.Rewind && anim.EasingFactor < 0.01f)
                ;
                if (shouldStop)
                {
                    anim.Stop();
                    if (anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Stop)
                    {
                        entity.ActiveAnimationsByAnimCode.Remove(anim.Animation.Code);
                    }
                    continue;
                }

                //debug += anim.Animation.Code + ",";

                curAnims[curAnimCount] = anim;

                if (anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Hold && anim.Iterations != 0 && !anim.Active)
                {
                    anim.EaseOut(dt);
                }

                anim.Progress(dt, (float)walkSpeed);

                curAnimCount++;
            }
        }


        protected virtual void AnimNowActive(RunningAnimation anim, AnimationMetaData animData)
        {
            anim.Running = true;
            anim.Active = true;
            anim.meta = animData;
            anim.ShouldRewind = false;
            anim.ShouldPlayTillEnd = false;
        }

    }
}
