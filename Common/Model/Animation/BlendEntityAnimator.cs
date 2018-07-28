using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AttachmentPointAndPose {
        public ElementPose Pose;
        public AttachmentPoint AttachPoint;
    }

    public class BlendEntityAnimator : FastEntityAnimator
    {
        ShapeElement[] rootElements;
        List<ElementPose> RootPoses;
        protected HashSet<int> jointsDone = new HashSet<int>();
        Dictionary<int, AnimationJoint> jointsById;

        float[] localTransformMatrix = Mat4f.Create();
        float[] identMat = Mat4f.Create();

        public Dictionary<string, AttachmentPointAndPose> AttachmentPointByCode = new Dictionary<string, AttachmentPointAndPose>();

        public bool FastMode;


        public BlendEntityAnimator(Entity entity, Animation[] Animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, ShapeElement headElement = null) : base(entity, Animations, headElement)
        {
            this.rootElements = rootElements;
            this.jointsById = jointsById;

            RootPoses = new List<ElementPose>();
            LoadedPoses(rootElements, RootPoses);
        }

        void LoadedPoses(ShapeElement[] elements, List<ElementPose> intoPoses)
        {
            ElementPose pose;
            for (int i = 0; i < elements.Length; i++)
            {
                ShapeElement elem = elements[i];

                intoPoses.Add(pose = new ElementPose());
                pose.AnimModelMatrix = Mat4f.Create();
                pose.ForElement = elem;

                if (elem.AttachmentPoints != null)
                {
                    for (int j = 0; j < elem.AttachmentPoints.Length; j++)
                    {
                        AttachmentPoint apoint = elem.AttachmentPoints[j];
                        AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose() { AttachPoint = apoint, Pose = pose };
                    }
                }

                if (elem.Children != null)
                {
                    pose.ChildElementPoses = new List<ElementPose>(elem.Children.Length);
                    LoadedPoses(elem.Children, pose.ChildElementPoses);
                }
            }
        }


        protected override void AnimNowActive(RunningAnimation anim, AnimationMetaData animData)
        {
            base.AnimNowActive(anim, animData);
            anim.LoadWeights(rootElements);
        }

       
        protected override void calculateMatrices(float dt)
        {
            if (FastMode)
            {
                base.calculateMatrices(dt);
                return;
            }

            List<List<ElementPose>> transformsByAnimation = new List<List<ElementPose>>();
            List<List<ElementPose>> nextFrameTransformsByAnimation = new List<List<ElementPose>>();

            List<ShapeElementWeights[]> weightsByAnimationAndElement = new List<ShapeElementWeights[]>();
            
            jointsDone.Clear();

            for (int j = 0; j < curAnimCount; j++)
            {
                RunningAnimation anim = curAnims[j];
                weightsByAnimationAndElement.Add(anim.ElementWeights);

                AnimationFrame curFrame = anim.Animation.AllFrames[(int)anim.CurrentFrame % anim.Animation.AllFrames.Length];
                transformsByAnimation.Add(curFrame.RootElementTransforms);

                AnimationFrame nextFrame = anim.Animation.AllFrames[((int)anim.CurrentFrame + 1) % anim.Animation.AllFrames.Length];
                if (anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Hold) nextFrame = curFrame;

                nextFrameTransformsByAnimation.Add(nextFrame.RootElementTransforms);
            }

            
            calculateMatrices(dt, RootPoses, weightsByAnimationAndElement, Mat4f.Create(), transformsByAnimation, nextFrameTransformsByAnimation);

            for (int jointid = 0; jointid < GlobalConstants.MaxAnimatedElements; jointid++)
            {
                if (jointsById.ContainsKey(jointid)) continue;
                
                for (int j = 0; j < 16; j++)
                {
                    TransformationMatrices[jointid * 16 + j] = identMat[j];
                }
            }
        }


        // Recursive method
        private void calculateMatrices(float dt, List<ElementPose> currentPoses, List<ShapeElementWeights[]> weightsByAnimationAndElement, float[] modelMatrix, List<List<ElementPose>> transformsByAnimation, List<List<ElementPose>> nextFrameTransformsByAnimation)
        {
            List<List<ElementPose>> childTransformsByAnimation = new List<List<ElementPose>>();
            List<List<ElementPose>> nextFrameChildTransformsByAnimation = new List<List<ElementPose>>();
            List<ShapeElementWeights[]> childWeightsByAnimationAndElement = new List<ShapeElementWeights[]>();

            for (int i = 0; i < currentPoses.Count; i++)
            {
                ElementPose currentPose = currentPoses[i];
                ShapeElement elem = currentPose.ForElement;

                currentPose.SetMat(modelMatrix);
                Mat4f.Identity(localTransformMatrix);

                currentPose.Clear();
                childTransformsByAnimation.Clear();
                nextFrameChildTransformsByAnimation.Clear();
                childWeightsByAnimationAndElement.Clear();

                float weightSum = 0f;
                for (int j = 0; j < transformsByAnimation.Count; j++)
                {
                    RunningAnimation anim = curAnims[j];
                    ShapeElementWeights sew = weightsByAnimationAndElement[j][i];

                    if (sew.BlendMode != EnumAnimationBlendMode.Add)
                    {
                        weightSum += sew.Weight * anim.EasingFactor;
                    }
                }

                for (int j = 0; j < transformsByAnimation.Count; j++)
                {
                    RunningAnimation anim = curAnims[j];
                    ShapeElementWeights sew = weightsByAnimationAndElement[j][i];
                    anim.CalcBlendedWeight(weightSum, sew.BlendMode);

                    ElementPose tf = transformsByAnimation[j][i];
                    ElementPose nextFrameTf = nextFrameTransformsByAnimation[j][i];
                    
                    float l = anim.CurrentFrame - (int)anim.CurrentFrame;

                    currentPose.Add(tf, nextFrameTf, l, anim.BlendedWeight);                    
                    childTransformsByAnimation.Add(tf.ChildElementPoses);
                    childWeightsByAnimationAndElement.Add(sew.ChildElements);

                    nextFrameChildTransformsByAnimation.Add(nextFrameTf.ChildElementPoses);
                }

                elem.GetLocalTransformMatrix(localTransformMatrix, currentPose);
                Mat4f.Mul(currentPose.AnimModelMatrix, currentPose.AnimModelMatrix, localTransformMatrix);

                if (elem.JointId > 0 && !jointsDone.Contains(elem.JointId))
                {
                    Mat4f.Mul(tmpMatrix, currentPose.AnimModelMatrix, elem.inverseModelTransform);
                    for (int l = 0; l < 16; l++)
                    {
                        TransformationMatrices[16 * elem.JointId + l] = tmpMatrix[l];
                    }
                    jointsDone.Add(elem.JointId);
                }
                
                if (currentPose.ChildElementPoses != null)
                {
                    calculateMatrices(dt, currentPose.ChildElementPoses, childWeightsByAnimationAndElement, currentPose.AnimModelMatrix, childTransformsByAnimation, nextFrameChildTransformsByAnimation);
                }
            }
        }
    }
}
