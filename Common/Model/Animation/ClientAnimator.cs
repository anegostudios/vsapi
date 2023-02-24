using System;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{

    /// <summary>
    /// Syncs every frame with entity.ActiveAnimationsByAnimCode, starts and stops animations when necessary 
    /// and does recursive interpolation on the rotation, position and scale value for each frame, for each element and for each active element
    /// this produces always correctly blended animations but is significantly more costly for the cpu when compared to the technique used by the <see cref="AnimatorBase"/>.
    /// </summary>
    public class ClientAnimator : AnimatorBase
    {
        public ShapeElement[] rootElements;
        public List<ElementPose> RootPoses;

        protected HashSet<int> jointsDone = new HashSet<int>();
        public Dictionary<int, AnimationJoint> jointsById;

        public static int MaxConcurrentAnimations = 16;
        int maxDepth;
        List<ElementPose>[][] transformsByAnimation;
        List<ElementPose>[][] nextFrameTransformsByAnimation;
        ShapeElementWeights[][][] weightsByAnimationAndElement;

        float[] localTransformMatrix = Mat4f.Create();
        float[] tmpMatrix = Mat4f.Create();
        
        public static ClientAnimator CreateForEntity(Entity entity, List<ElementPose> rootPoses, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById)
        {
            if (entity is EntityAgent)
            {
                EntityAgent entityag = entity as EntityAgent;
                return new ClientAnimator(
                    () => entityag.Controls.MovespeedMultiplier * entityag.GetWalkSpeedMultiplier(0.3),
                    rootPoses,
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            } else
            {
                return new ClientAnimator(
                    () => 1,
                    rootPoses,
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            }
        }

        public static ClientAnimator CreateForEntity(Entity entity, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById)
        {
            if (entity is EntityAgent)
            {
                EntityAgent entityag = entity as EntityAgent;

                return new ClientAnimator(
                    () => entityag.Controls.MovespeedMultiplier * entityag.GetWalkSpeedMultiplier(0.3),
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            } else
            {
                return new ClientAnimator(
                    () => 1,
                    animations,
                    rootElements,
                    jointsById,
                    (code) => entity.AnimManager.OnAnimationStopped(code)
                );
            }
        }

        public ClientAnimator(WalkSpeedSupplierDelegate walkSpeedSupplier, Animation[] animations, Action<string> onAnimationStoppedListener = null) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            initFields();
        }

        public ClientAnimator(WalkSpeedSupplierDelegate walkSpeedSupplier, List<ElementPose> rootPoses, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, Action<string> onAnimationStoppedListener = null) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.rootElements = rootElements;
            this.jointsById = jointsById;
            this.RootPoses = rootPoses;
            LoadAttachmentPoints(RootPoses);
            initFields();
        }

        public ClientAnimator(WalkSpeedSupplierDelegate walkSpeedSupplier, Animation[] animations, ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, Action<string> onAnimationStoppedListener = null) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.rootElements = rootElements;
            this.jointsById = jointsById;

            RootPoses = new List<ElementPose>();
            LoadPosesAndAttachmentPoints(rootElements, RootPoses);
            initFields();
        }

        protected virtual void initFields()
        {
            maxDepth = 2 + (RootPoses == null ? 0 : getMaxDepth(RootPoses, 1));

            transformsByAnimation = new List<ElementPose>[maxDepth][];
            nextFrameTransformsByAnimation = new List<ElementPose>[maxDepth][];
            weightsByAnimationAndElement = new ShapeElementWeights[maxDepth][][];

            for (int i = 0; i < maxDepth; i++)
            {
                transformsByAnimation[i] = new List<ElementPose>[MaxConcurrentAnimations];
                nextFrameTransformsByAnimation[i] = new List<ElementPose>[MaxConcurrentAnimations];
                weightsByAnimationAndElement[i] = new ShapeElementWeights[MaxConcurrentAnimations][];
            }
        }

        protected virtual void LoadAttachmentPoints(List<ElementPose> cachedPoses)
        {
            for (int i = 0; i < cachedPoses.Count; i++)
            {
                ElementPose elem = cachedPoses[i];

                if (elem.ForElement.AttachmentPoints != null)
                {
                    for (int j = 0; j < elem.ForElement.AttachmentPoints.Length; j++)
                    {
                        AttachmentPoint apoint = elem.ForElement.AttachmentPoints[j];
                        AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose() {
                            AttachPoint = apoint,
                            CachedPose = elem
                        };
                    }
                }

                if (elem.ChildElementPoses != null)
                {
                    LoadAttachmentPoints(elem.ChildElementPoses);
                }
            }
        }

        protected virtual void LoadPosesAndAttachmentPoints(ShapeElement[] elements, List<ElementPose> intoPoses)
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
                        AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose() {
                            AttachPoint = apoint,
                            CachedPose = pose
                        };
                    }
                }

                if (elem.Children != null)
                {
                    pose.ChildElementPoses = new List<ElementPose>(elem.Children.Length);
                    LoadPosesAndAttachmentPoints(elem.Children, pose.ChildElementPoses);
                }
            }
        }

        private int getMaxDepth(List<ElementPose> poses, int depth)
        {
            for (int i = 0; i < poses.Count; i++)
            {
                var pose = poses[i];

                if (pose.ChildElementPoses != null)
                {
                    depth = getMaxDepth(pose.ChildElementPoses, depth);
                }
            }

            return depth + 1;
        }

        public override ElementPose GetPosebyName(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return getPosebyName(RootPoses, name);
        }

        private ElementPose getPosebyName(List<ElementPose> poses, string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            for (int i = 0; i < poses.Count; i++)
            {
                var pose = poses[i];
                if (pose.ForElement.Name.Equals(name, stringComparison)) return pose;

                if (pose.ChildElementPoses != null)
                {
                    var foundPose = getPosebyName(pose.ChildElementPoses, name);
                    if (foundPose != null) return foundPose;
                }
            }

            return null;
        }

        protected override void AnimNowActive(RunningAnimation anim, AnimationMetaData animData)
        {
            base.AnimNowActive(anim, animData);

            if (anim.Animation.PrevNextKeyFrameByFrame == null)
            {
                anim.Animation.GenerateAllFrames(rootElements, jointsById);
            }

            anim.LoadWeights(rootElements);
        }


        

        int[] prevFrame = new int[MaxConcurrentAnimations];
        int[] nextFrame = new int[MaxConcurrentAnimations];


        protected override void calculateMatrices(float dt)
        {
            if (!CalculateMatrices) return;
            try
            {
                jointsDone.Clear();

                for (int j = 0; j < curAnimCount; j++)
                {
                    RunningAnimation anim = CurAnims[j];
                    weightsByAnimationAndElement[0][j] = anim.ElementWeights;

                    AnimationFrame[] prevNextFrame = anim.Animation.PrevNextKeyFrameByFrame[(int)anim.CurrentFrame % anim.Animation.QuantityFrames];
                    transformsByAnimation[0][j] = prevNextFrame[0].RootElementTransforms;
                    prevFrame[j] = prevNextFrame[0].FrameNumber;

                    if (anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Hold && (int)anim.CurrentFrame + 1 == anim.Animation.QuantityFrames)
                    {
                        nextFrameTransformsByAnimation[0][j] = prevNextFrame[0].RootElementTransforms;
                        nextFrame[j] = prevNextFrame[0].FrameNumber;
                    }
                    else
                    {
                        nextFrameTransformsByAnimation[0][j] = prevNextFrame[1].RootElementTransforms;
                        nextFrame[j] = prevNextFrame[1].FrameNumber;
                    }
                }

                calculateMatrices(
                    dt,
                    RootPoses,
                    weightsByAnimationAndElement[0],
                    Mat4f.Create(),
                    transformsByAnimation[0],
                    nextFrameTransformsByAnimation[0],
                    0
                );


                for (int jointid = 0; jointid < GlobalConstants.MaxAnimatedElements; jointid++)
                {
                    if (jointsById.ContainsKey(jointid)) continue;

                    for (int j = 0; j < 12; j++)
                    {
                        TransformationMatrices4x3[jointid * 12 + j] = identMat4x3[j];
                    }
                }

                foreach (var val in AttachmentPointByCode)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        val.Value.AnimModelMatrix[i] = val.Value.CachedPose.AnimModelMatrix[i];
                    }
                }

            } catch (Exception)
            {
                //entity.World.Logger.Fatal("Animation system crash. Please report this bug. curanimcount: {3}, tm-l:{0}, jbi-c: {1}, abc-c: {2}\nException: {4}", TransformationMatrices.Length, jointsById.Count, AttachmentPointByCode.Count, curAnimCount, e);
            }
        }



        // Careful when changing around stuff in here, this is a recursively called method
        private void calculateMatrices(
            float dt,
            List<ElementPose> currentPoses,
            ShapeElementWeights[][] weightsByAnimationAndElement,
            float[] modelMatrix,
            List<ElementPose>[] transformsByAnimation,
            List<ElementPose>[] nextFrameTransformsByAnimation,
            int depth
        )
        {
            depth++;
            List<ElementPose>[] childTransformsByAnimation = this.transformsByAnimation[depth];
            List<ElementPose>[] nextFrameChildTransformsByAnimation = this.nextFrameTransformsByAnimation[depth];
            ShapeElementWeights[][] childWeightsByAnimationAndElement = this.weightsByAnimationAndElement[depth];


            for (int i = 0; i < currentPoses.Count; i++)
            {
                ElementPose currentPose = currentPoses[i];
                ShapeElement elem = currentPose.ForElement;

                currentPose.SetMat(modelMatrix);
                Mat4f.Identity(localTransformMatrix);

                currentPose.Clear();

                float weightSum = 0f;
                for (int j = 0; j < curAnimCount; j++)
                {
                    RunningAnimation anim = CurAnims[j];
                    ShapeElementWeights sew = weightsByAnimationAndElement[j][i];

                    if (sew.BlendMode != EnumAnimationBlendMode.Add)
                    {
                        weightSum += sew.Weight * anim.EasingFactor;
                    }
                }

                for (int j = 0; j < curAnimCount; j++)
                {
                    RunningAnimation anim = CurAnims[j];
                    ShapeElementWeights sew = weightsByAnimationAndElement[j][i];
                    anim.CalcBlendedWeight(weightSum / sew.Weight, sew.BlendMode);

                    ElementPose prevFramePose = transformsByAnimation[j][i];
                    ElementPose nextFramePose = nextFrameTransformsByAnimation[j][i];

                    int prevFrame = this.prevFrame[j];
                    int nextFrame = this.nextFrame[j];

                    // May loop around, so nextFrame can be smaller than prevFrame
                    float keyFrameDist = nextFrame > prevFrame ? (nextFrame - prevFrame) : (anim.Animation.QuantityFrames - prevFrame + nextFrame);
                    float curFrameDist = anim.CurrentFrame >= prevFrame ? (anim.CurrentFrame - prevFrame) : (anim.Animation.QuantityFrames - prevFrame + anim.CurrentFrame);

                    float lerp = curFrameDist / keyFrameDist;

                    currentPose.Add(prevFramePose, nextFramePose, lerp, anim.BlendedWeight);

                    childTransformsByAnimation[j] = prevFramePose.ChildElementPoses;
                    childWeightsByAnimationAndElement[j] = sew.ChildElements;

                    nextFrameChildTransformsByAnimation[j] = nextFramePose.ChildElementPoses;
                }

                elem.GetLocalTransformMatrix(localTransformMatrix, currentPose);
                Mat4f.Mul(currentPose.AnimModelMatrix, currentPose.AnimModelMatrix, localTransformMatrix);

                if (elem.JointId > 0 && !jointsDone.Contains(elem.JointId))
                {
                    Mat4f.Mul(tmpMatrix, currentPose.AnimModelMatrix, elem.inverseModelTransform);

                    // https://stackoverflow.com/questions/32565827/whats-the-purpose-of-magic-4-of-last-row-in-matrix-4x4-for-3d-graphics
                    // We skip the last row
                    // 0 4 8  12
                    // 1 5 9  13
                    // 2 6 10 14
                    // 3 7 11 15
                    // =>
                    // 0 3 6 9
                    // 1 4 7 10
                    // 2 5 8 11
                    int index = 12 * elem.JointId;
                    TransformationMatrices4x3[index++] = tmpMatrix[0];
                    TransformationMatrices4x3[index++] = tmpMatrix[1];
                    TransformationMatrices4x3[index++] = tmpMatrix[2];
                    TransformationMatrices4x3[index++] = tmpMatrix[4];
                    TransformationMatrices4x3[index++] = tmpMatrix[5];
                    TransformationMatrices4x3[index++] = tmpMatrix[6];
                    TransformationMatrices4x3[index++] = tmpMatrix[8];
                    TransformationMatrices4x3[index++] = tmpMatrix[9];
                    TransformationMatrices4x3[index++] = tmpMatrix[10];
                    TransformationMatrices4x3[index++] = tmpMatrix[12];
                    TransformationMatrices4x3[index++] = tmpMatrix[13];
                    TransformationMatrices4x3[index++] = tmpMatrix[14];

                    /*for (int l = 0; l < 12; l++)
                    {
                        TransformationMatrices[12 * elem.JointId + l] = tmpMatrix[l];
                    }*/

                    jointsDone.Add(elem.JointId);
                }

                if (currentPose.ChildElementPoses != null)
                {
                    calculateMatrices(
                        dt,
                        currentPose.ChildElementPoses,
                        childWeightsByAnimationAndElement,
                        currentPose.AnimModelMatrix,
                        childTransformsByAnimation,
                        nextFrameChildTransformsByAnimation,
                        depth
                    );
                }
            }
        }


    }
}