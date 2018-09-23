using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    
    /// <summary>
    /// A class to play the right animations at the right time.
    /// Must be able to:
    /// - Automatically run animations based on each animations EnumEntityActivity
    /// - Manually be feed by an Entity to play additional animations
    /// Definition of a running animation:
    /// - The transformationMatrices for a given frame. Has to be interpolated from
    ///   - Current 30-fps frame to next frame, depending on the current dt
    ///   - All currently active animations
    /// 
    /// </summary>
    public class FastEntityAnimator
    {
        public RunningAnimation[] anims;
        protected Entity entity;
        protected EntityAgent entityAgent;



        public float[] Matrices
        {
            get { return curAnimCount > 0 ? TransformationMatrices : TransformationMatricesIdentity; }
        }

        public int ActiveAnimationCount
        {
            get { return curAnimCount; }
        }


        public float[] HeadGlobalMatrix = null;
        public float[] HeadGlobalMatrixInverted = null;
        public float[] HeadLocalMatrix = null;
        
        public float[] tmpMatrix = Mat4f.Create();
        public float[] IdentityMatrix = Mat4f.Create();

        public ShapeElement HeadElement;
        public float HeadPitch;
        public float HeadYaw;

        public float[] TransformationMatrices = new float[16 * GlobalConstants.MaxAnimatedElements];
        public static float[] TransformationMatricesIdentityStatic = new float[16 * GlobalConstants.MaxAnimatedElements];

        public float[] TransformationMatricesIdentity;


        protected int curAnimCount = 0;
        public RunningAnimation[] curAnims = new RunningAnimation[20];

        static FastEntityAnimator()
        {
            float[] mat = Mat4f.Create();
            int k = 0;
            for (int i = 0; i < GlobalConstants.MaxAnimatedElements; i++)
            {
                for (int j = 0; j < 16; j++) TransformationMatricesIdentityStatic[k++] = mat[j];
            }
        }

        public FastEntityAnimator(Entity entity, Animation[] Animations, ShapeElement headElement = null)
        {
            this.entity = entity;
            this.HeadElement = headElement;

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
            
            if (HeadElement != null)
            {
                HeadGlobalMatrix = Mat4f.Create();
                HeadGlobalMatrixInverted = Mat4f.Create();
                HeadLocalMatrix = Mat4f.Create();

                List<ShapeElement> elems = HeadElement.GetParentPath();
                
                for (int i = 0; i < elems.Count; i++)
                {
                    ShapeElement elem = elems[i];
                    float[] localTransform = elem.GetLocalTransformMatrix();
                    Mat4f.Mul(HeadGlobalMatrix, HeadGlobalMatrix, localTransform);
                }

                Mat4f.Mul(HeadGlobalMatrix, HeadGlobalMatrix, HeadElement.GetLocalTransformMatrix());

                Mat4f.Invert(HeadGlobalMatrixInverted, HeadGlobalMatrix);

                TransformationMatricesIdentity = (float[])TransformationMatricesIdentityStatic.Clone();
            } else
            {
                TransformationMatricesIdentity = TransformationMatricesIdentityStatic;
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
                    (anim.Iterations > 0 && !anim.Active && (anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.PlayTillEnd || anim.Animation.OnActivityStopped == EnumEntityActivityStoppedHandling.EaseOut) && anim.EasingFactor < 0.01f) ||
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

               // debug += anim.Animation.Code + "("+anim.BlendedWeight.ToString("#.##")+"),";

                curAnims[curAnimCount] = anim;

                if (anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Hold && anim.Iterations != 0 && !anim.Active)
                {
                    anim.EaseOut(dt);
                }

                anim.Progress(dt, (float)walkSpeed);

                curAnimCount++;

            }


            calculateMatrices(dt);
          //  Console.WriteLine(debug);



            if (HeadElement != null && (HeadYaw != 0 || HeadPitch != 0))
            {
                Mat4f.Identity(HeadLocalMatrix);
                Mat4f.RotateY(HeadLocalMatrix, HeadLocalMatrix, HeadYaw);
                Mat4f.RotateZ(HeadLocalMatrix, HeadLocalMatrix, HeadPitch);

                ApplyHeadTransform(HeadLocalMatrix, new ShapeElement[] { HeadElement }, HeadElement.JointId);
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

        protected virtual void calculateMatrices(float dt)
        {
            bool first = true;

            for (int i = 0; i < curAnimCount; i++)
            {
                RunningAnimation anim = curAnims[i];

                lerpAndAddMatrices(anim, first);
                first = false;
            }
        }


        protected void lerpAndAddMatrices(RunningAnimation anim, bool first)
        {
            try
            {
                AnimationFrame curFrame = anim.Animation.AllFrames[(int)anim.CurrentFrame % anim.Animation.AllFrames.Length];
                AnimationFrame nextFrame = anim.Animation.AllFrames[((int)anim.CurrentFrame + 1) % anim.Animation.AllFrames.Length];

                float l = anim.CurrentFrame - (int)anim.CurrentFrame;

                if (first)
                {
                    for (int i = 0; i < TransformationMatrices.Length; i++)
                    {
                        TransformationMatrices[i] = curFrame.transformationMatrices[i] * (1 - l) + nextFrame.transformationMatrices[i] * (l);
                    }
                }
                else
                {
                    for (int i = 0; i < TransformationMatrices.Length; i++)
                    {
                        TransformationMatrices[i] += curFrame.transformationMatrices[i] * (1 - l) + nextFrame.transformationMatrices[i] * (l);
                    }
                }
            } catch (Exception e)
            {
                string str = string.Format("Something crashed while trying to calculate an animation frame for {0}. AllFrames.Length={1}, currframee={2}, tf mats length={3}. Exception: {4}", entity?.Code, anim.Animation.AllFrames.Length,  anim.CurrentFrame, TransformationMatrices.Length, e);
                throw new Exception(str);
            }
        }



        protected void ApplyHeadTransform(float[] matrix, ShapeElement[] forElems, int headJointId)
        {
            for (int k = 0; k < forElems.Length; k++)
            {
                ShapeElement elem = forElems[k];

                if (elem == HeadElement || elem.JointId != headJointId)
                {

                    for (int i = 0; i < 16; i++)
                    {
                        tmpMatrix[i] = ActiveAnimationCount == 0 ? IdentityMatrix[i] : TransformationMatrices[16 * elem.JointId + i];
                    }

                    float[] origin = new float[] { (float)HeadElement.RotationOrigin[0] / 16f, (float)HeadElement.RotationOrigin[1] / 16f, (float)HeadElement.RotationOrigin[2] / 16f };

                    Mat4f.Mul(tmpMatrix, tmpMatrix, HeadGlobalMatrix);
                    Mat4f.Translate(tmpMatrix, tmpMatrix, origin);
                    Mat4f.Mul(tmpMatrix, tmpMatrix, matrix);
                    origin[0] = -origin[0];
                    origin[1] = -origin[1];
                    origin[2] = -origin[2];
                    Mat4f.Translate(tmpMatrix, tmpMatrix, origin);
                    Mat4f.Mul(tmpMatrix, tmpMatrix, HeadGlobalMatrixInverted);

                    for (int i = 0; i < 16; i++)
                    {
                        TransformationMatrices[16 * elem.JointId + i] = tmpMatrix[i];
                        TransformationMatricesIdentity[16 * elem.JointId + i] = tmpMatrix[i];
                    }
                }

                if (elem.Children != null)
                {
                    ApplyHeadTransform(matrix, elem.Children, headJointId);
                }
            }
        }


    }
}
