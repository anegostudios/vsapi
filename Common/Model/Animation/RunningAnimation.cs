using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class ShapeElementWeights
    {
        public float Weight = 1f;
        public EnumAnimationBlendMode BlendMode = EnumAnimationBlendMode.AddAverage;
        public ShapeElementWeights[] ChildElements;
    }

    public class RunningAnimation
    {
        public AnimationMetaData meta;

        public float CurrentFrame;
        public Animation Animation;
        public bool Active;
        public bool Running;
        public int Iterations;

        public bool ShouldRewind = false;
        public bool ShouldPlayTillEnd = false;

        public float EasingFactor;
        public float BlendedWeight;

        public ShapeElementWeights[] ElementWeights;

        public void LoadWeights(ShapeElement[] rootElements)
        {
            ElementWeights = new ShapeElementWeights[rootElements.Length];
            LoadWeights(rootElements, ElementWeights, meta.ElementWeight, meta.ElementBlendMode);
        }

        private void LoadWeights(ShapeElement[] elements, ShapeElementWeights[] intoList, Dictionary<string, float> elementWeight, Dictionary<string, EnumAnimationBlendMode> elementBlendMode)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                ShapeElement elem = elements[i];
                intoList[i] = new ShapeElementWeights();
                
                float w;
                if (elementWeight.TryGetValue(elem.Name, out w))
                {
                    intoList[i].Weight = w;
                }
                else
                {
                    intoList[i].Weight = meta.Weight;
                }
                
                EnumAnimationBlendMode blendMode;
                if (elementBlendMode.TryGetValue(elem.Name, out blendMode))
                {
                    intoList[i].BlendMode = blendMode;
                } else
                {
                    intoList[i].BlendMode = meta.BlendMode;
                }


                if (elem.Children != null)
                {
                    intoList[i].ChildElements = new ShapeElementWeights[elem.Children.Length];
                    LoadWeights(elem.Children, intoList[i].ChildElements, elementWeight, elementBlendMode);
                }
            }
        }

        internal void CalcBlendedWeight(float weightSum, EnumAnimationBlendMode blendMode)
        {
            if (weightSum == 0)
            {
                BlendedWeight = EasingFactor;
                return;
            }
            BlendedWeight = GameMath.Clamp(blendMode != EnumAnimationBlendMode.Average ? EasingFactor : EasingFactor / weightSum, 0, 1);
        }

        public void Progress(float dt, float walkspeed)
        {
            dt *= meta.GetCurrentAnimationSpeed(walkspeed);

            
            if (Active || (Iterations == 0 && Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.PlayTillEnd))
            {
                EasingFactor = Math.Min(1f, EasingFactor + (1f - EasingFactor) * dt * meta.EaseInSpeed);
            }
            else
            {
                EasingFactor = Math.Max(0, EasingFactor - (EasingFactor - 0) * dt * meta.EaseOutSpeed);
            }
            
            float newFrame = (CurrentFrame + 30 * (ShouldRewind ? -dt : dt));

            if (!Active && Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.PlayTillEnd && (Iterations >= 1 || newFrame >= Animation.QuantityFrames - 1))
            {
                EasingFactor = 0;
                CurrentFrame = Animation.QuantityFrames - 1;
                return;
            }

            if (Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Hold && newFrame >= Animation.QuantityFrames - 1)
            {
                Iterations = 1;
                CurrentFrame = Animation.QuantityFrames - 1;
                return;
            }


            if (newFrame <= 0)
            {
                Iterations--;
                CurrentFrame = 0;
                return;
            }

            CurrentFrame = newFrame;

            if (CurrentFrame >= Animation.QuantityFrames - 1)
            {
                Iterations++;
                CurrentFrame = GameMath.Mod(newFrame, Animation.QuantityFrames - 1);
            }

            
            if (Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Stop && Iterations > 0)
            {
                CurrentFrame = Animation.QuantityFrames - 1;
            }
        }


        public void Stop()
        {
            Active = false;
            Running = false;
            CurrentFrame = 0;
            Iterations = 0;
            EasingFactor = 0;
        }

        public void EaseOut(float dt)
        {
            EasingFactor = Math.Max(0, EasingFactor - (EasingFactor - 0) * dt * meta.EaseOutSpeed);
        }
    }
}
