using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{

    /// <summary>
    /// Syncs every frame with entity.ActiveAnimationsByAnimCode, starts and stops animations when necessary
    /// and does recursive interpolation on the rotation, position and scale value for each frame, for each element and for each active element
    /// this produces always correctly blended animations but is significantly more costly for the cpu when compared to the technique used by the <see cref="AnimatorBase"/>.
    /// </summary>
    public class ClientAnimator : AnimatorBase
    {
        public Dictionary<int, AnimationJoint> jointsById;
        protected HashSet<int> jointsDone = new HashSet<int>();

        public static int MaxConcurrentAnimations = 16;
        int maxDepth;
        List<ElementPose>[][] frameByDepthByAnimation;
        List<ElementPose>[][] nextFrameTransformsByAnimation;
        ShapeElementWeights[][][] weightsByAnimationAndElement;

        float[] localTransformMatrix = Mat4f.Create();
        float[] tmpMatrix = Mat4f.Create();

        Action<AnimationSound> onShouldPlaySoundListener = null;

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
                    entity.AnimManager.TriggerAnimationStopped,
                    entity.AnimManager.ShouldPlaySound
                );
            } else
            {
                return new ClientAnimator(
                    () => 1,
                    rootPoses,
                    animations,
                    rootElements,
                    jointsById,
                    entity.AnimManager.TriggerAnimationStopped,
                    entity.AnimManager.ShouldPlaySound
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
                    entity.AnimManager.TriggerAnimationStopped,
                    entity.AnimManager.ShouldPlaySound
                );
            }
            else
            {
                return new ClientAnimator(
                    () => 1,
                    animations,
                    rootElements,
                    jointsById,
                    entity.AnimManager.TriggerAnimationStopped,
                    entity.AnimManager.ShouldPlaySound
                );
            }
        }

        public ClientAnimator(                  // This constructor is called only on the server side, by ServerAnimator constructors.  Note, no call to initMatrices, to save RAM on a server
            WalkSpeedSupplierDelegate walkSpeedSupplier,
            Animation[] animations,
            Action<string> onAnimationStoppedListener = null,
            Action<AnimationSound> onShouldPlaySoundListener = null
        ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.onShouldPlaySoundListener = onShouldPlaySoundListener;
            initFields();
        }

        public ClientAnimator(                  // This constructor is called only on the client side
            WalkSpeedSupplierDelegate walkSpeedSupplier,
            List<ElementPose> rootPoses,
            Animation[] animations,
            ShapeElement[] rootElements,
            Dictionary<int, AnimationJoint> jointsById,
            Action<string> onAnimationStoppedListener = null,
            Action<AnimationSound> onShouldPlaySoundListener = null
        ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.RootElements = rootElements;
            this.jointsById = jointsById;
            this.RootPoses = rootPoses;
            this.onShouldPlaySoundListener = onShouldPlaySoundListener;
            LoadAttachmentPoints(RootPoses);
            initFields();
            initMatrices(MaxJointId);
        }

        public ClientAnimator(                  // This constructor is called only on the client side
            WalkSpeedSupplierDelegate walkSpeedSupplier,
            Animation[] animations,
            ShapeElement[] rootElements,
            Dictionary<int, AnimationJoint> jointsById,
            Action<string> onAnimationStoppedListener = null,
            Action<AnimationSound> onShouldPlaySoundListener = null
        ) : base(walkSpeedSupplier, animations, onAnimationStoppedListener)
        {
            this.RootElements = rootElements;
            this.jointsById = jointsById;

            RootPoses = new List<ElementPose>();
            LoadPosesAndAttachmentPoints(rootElements, RootPoses);
            this.onShouldPlaySoundListener = onShouldPlaySoundListener;
            initFields();
            initMatrices(MaxJointId);
        }

        protected virtual void initFields()
        {
            maxDepth = 2 + (RootPoses == null ? 0 : getMaxDepth(RootPoses, 1));

            frameByDepthByAnimation = new List<ElementPose>[maxDepth][];
            nextFrameTransformsByAnimation = new List<ElementPose>[maxDepth][];
            weightsByAnimationAndElement = new ShapeElementWeights[maxDepth][][];

            for (int i = 0; i < maxDepth; i++)
            {
                frameByDepthByAnimation[i] = new List<ElementPose>[MaxConcurrentAnimations];
                nextFrameTransformsByAnimation[i] = new List<ElementPose>[MaxConcurrentAnimations];
                weightsByAnimationAndElement[i] = new ShapeElementWeights[MaxConcurrentAnimations][];
            }
        }

        protected virtual void initMatrices(int maxJointId)
        {
            // Matrices are only required on client side; and from 1.20.5 limited to the MaxJointId (x16) in length to avoid wasting RAM
            var identMat = ClientAnimator.identMat;
            var defaultPoseMatrices = new float[16 * maxJointId];
            for (int i = 0; i < defaultPoseMatrices.Length; i++)
            {
                defaultPoseMatrices[i] = identMat[i % 16];
            }
            TransformationMatricesDefaultPose = defaultPoseMatrices;
            TransformationMatrices = new float[defaultPoseMatrices.Length];
        }

        public override void ReloadAttachmentPoints()
        {
            LoadAttachmentPoints(RootPoses);
        }

        protected virtual void LoadAttachmentPoints(List<ElementPose> cachedPoses)
        {
            for (int i = 0; i < cachedPoses.Count; i++)
            {
                ElementPose elem = cachedPoses[i];

                var attachmentPoints = elem.ForElement.AttachmentPoints;
                if (attachmentPoints != null)
                {
                    for (int j = 0; j < attachmentPoints.Length; j++)
                    {
                        AttachmentPoint apoint = attachmentPoints[j];
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
                anim.Animation.GenerateAllFrames(RootElements, jointsById);
            }

            anim.LoadWeights(RootElements);
        }




        int[] prevFrame = new int[MaxConcurrentAnimations];
        int[] nextFrame = new int[MaxConcurrentAnimations];

        public override int MaxJointId => jointsById.Count + 1;

        public override void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt)
        {
            for (int j = 0; j < activeAnimCount; j++)
            {
                RunningAnimation anim = CurAnims[j];
                if (anim.Animation.PrevNextKeyFrameByFrame == null && anim.Animation.KeyFrames.Length > 0)
                {
                    anim.Animation.GenerateAllFrames(RootElements, jointsById);
                }

                if (anim.meta.AnimationSound != null && onShouldPlaySoundListener != null)
                {
                    if (anim.CurrentFrame >= anim.meta.AnimationSound.Frame && anim.SoundPlayedAtIteration != anim.Iterations && anim.Active)
                    {
                        onShouldPlaySoundListener(anim.meta.AnimationSound);
                        anim.SoundPlayedAtIteration = anim.Iterations;
                    }
                }
            }

            base.OnFrame(activeAnimationsByAnimCode, dt);
        }

        protected override void calculateMatrices(float dt)
        {
            if (!CalculateMatrices) return;

            jointsDone.Clear();

            int animVersion = 0;

            for (int j = 0; j < activeAnimCount; j++)
            {
                RunningAnimation anim = CurAnims[j];

                weightsByAnimationAndElement[0][j] = anim.ElementWeights;

                animVersion = Math.Max(animVersion, anim.Animation.Version);

                AnimationFrame[] prevNextFrame = anim.Animation.PrevNextKeyFrameByFrame[(int)anim.CurrentFrame % anim.Animation.QuantityFrames];
                frameByDepthByAnimation[0][j] = prevNextFrame[0].RootElementTransforms;
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
                animVersion,
                dt,
                RootPoses,
                weightsByAnimationAndElement[0],
                Mat4f.Create(),
                frameByDepthByAnimation[0],
                nextFrameTransformsByAnimation[0],
                0
            );

            var TransformationMatrices = this.TransformationMatrices;
            if (TransformationMatrices != null)
            {
                for (int jointidMul16 = 0; jointidMul16 < TransformationMatrices.Length; jointidMul16 += 16)    // radfast 20.2.25:  jointIdMul16 is (jointId * 16); I think this is the highest performance way to loop through this, taking account of the many array bounds checks
                {
                    if (jointsById.ContainsKey(jointidMul16 / 16)) continue;

                for (int j = 0; j < 16; j++)
                {
                        TransformationMatrices[jointidMul16 + j] = identMat[j];
                    }
                }
            }

            foreach (var val in AttachmentPointByCode)
            {
                var cachedMatrix = val.Value.CachedPose.AnimModelMatrix;
                var animMatrix = val.Value.AnimModelMatrix;
                for (int i = 0; i < 16; i++)
                {
                    animMatrix[i] = cachedMatrix[i];
                }
            }
        }



        // Careful when changing around stuff in here, this is a recursively called method
        private void calculateMatrices(
            int animVersion,
            float dt,
            List<ElementPose> outFrame,
            ShapeElementWeights[][] weightsByAnimationAndElement,
            float[] modelMatrix,
            List<ElementPose>[] nowKeyFrameByAnimation,
            List<ElementPose>[] nextInKeyFrameByAnimation,
            int depth
        )
        {
            depth++;
            List<ElementPose>[] nowChildKeyFrameByAnimation = this.frameByDepthByAnimation[depth];
            List<ElementPose>[] nextChildKeyFrameByAnimation = this.nextFrameTransformsByAnimation[depth];
            ShapeElementWeights[][] childWeightsByAnimationAndElement = this.weightsByAnimationAndElement[depth];


            for (int childPoseIndex = 0; childPoseIndex < outFrame.Count; childPoseIndex++)
            {
                ElementPose outFramePose = outFrame[childPoseIndex];
                ShapeElement elem = outFramePose.ForElement;

                outFramePose.SetMat(modelMatrix);
                Mat4f.Identity(localTransformMatrix);

                outFramePose.Clear();

                float weightSum = 0f;
#if DEBUG
                StringBuilder sb = null;
                if (EleWeightDebug) sb = new StringBuilder();
#endif
                for (int animIndex = 0; animIndex < activeAnimCount; animIndex++)
                {
                    RunningAnimation anim = CurAnims[animIndex];

                    ShapeElementWeights sew = weightsByAnimationAndElement[animIndex][childPoseIndex];

                    if (sew.BlendMode != EnumAnimationBlendMode.Add)
                    {
                        weightSum += sew.Weight * anim.EasingFactor;
                    }

#if DEBUG
                    if (EleWeightDebug) sb.Append(string.Format("{0:0.0} from {1} (blendmode {2}), ", sew.Weight * anim.EasingFactor, anim.Animation.Code, sew.BlendMode));
#endif
                }
#if DEBUG
                if (EleWeightDebug)
                {
                    if (eleWeights.ContainsKey(elem.Name)) eleWeights[elem.Name] += sb.ToString();
                    else eleWeights[elem.Name] = sb.ToString();
                }
#endif

                for (int animIndex = 0; animIndex < activeAnimCount; animIndex++)
                {
                    RunningAnimation anim = CurAnims[animIndex];
                    ShapeElementWeights sew = weightsByAnimationAndElement[animIndex][childPoseIndex];
                    anim.CalcBlendedWeight(weightSum / sew.Weight, sew.BlendMode);

                    ElementPose nowFramePose = nowKeyFrameByAnimation[animIndex][childPoseIndex];
                    ElementPose nextFramePose = nextInKeyFrameByAnimation[animIndex][childPoseIndex];

                    int prevFrame = this.prevFrame[animIndex];
                    int nextFrame = this.nextFrame[animIndex];

                    // May loop around, so nextFrame can be smaller than prevFrame
                    float keyFrameDist = nextFrame > prevFrame ? (nextFrame - prevFrame) : (anim.Animation.QuantityFrames - prevFrame + nextFrame);
                    float curFrameDist = anim.CurrentFrame >= prevFrame ? (anim.CurrentFrame - prevFrame) : (anim.Animation.QuantityFrames - prevFrame + anim.CurrentFrame);

                    float lerp = curFrameDist / keyFrameDist;

                    outFramePose.Add(nowFramePose, nextFramePose, lerp, anim.BlendedWeight);

                    nowChildKeyFrameByAnimation[animIndex] = nowFramePose.ChildElementPoses;
                    childWeightsByAnimationAndElement[animIndex] = sew.ChildElements;

                    nextChildKeyFrameByAnimation[animIndex] = nextFramePose.ChildElementPoses;
                }

                elem.GetLocalTransformMatrix(animVersion, localTransformMatrix, outFramePose);
                Mat4f.Mul(outFramePose.AnimModelMatrix, outFramePose.AnimModelMatrix, localTransformMatrix);

                if (TransformationMatrices != null)     // It is null on a server, non-null on a client
                {
                if (elem.JointId > 0 && !jointsDone.Contains(elem.JointId))
                {
                    Mat4f.Mul(tmpMatrix, outFramePose.AnimModelMatrix, elem.inverseModelTransform);

                    int index = 16 * elem.JointId;
                        var transformationMatrices = TransformationMatrices;
                        var tmpMatrixLocal = tmpMatrix;
                        if (index + 16 > transformationMatrices.Length)     // Check we have space for this joint: we normally should, if MaxJointId was correct, but a mod could have modified the shape or something
                        {
                            var transformationMatricesDefault = this.TransformationMatricesDefaultPose;
                            initMatrices(elem.JointId + 1);    // This replaces the matrices in both fields, but our local references are still the old populated matrices
                            Array.Copy(transformationMatrices, this.TransformationMatrices, transformationMatrices.Length);
                            Array.Copy(transformationMatricesDefault, this.TransformationMatricesDefaultPose, transformationMatricesDefault.Length);
                            transformationMatrices = this.TransformationMatrices;
                        }
                    for (int i = 0; i < 16; i++)
                    {
                            transformationMatrices[index + i] = tmpMatrixLocal[i];
                    }

                    jointsDone.Add(elem.JointId);
                }
                }

                if (outFramePose.ChildElementPoses != null)
                {
                    calculateMatrices(
                        animVersion,
                        dt,
                        outFramePose.ChildElementPoses,
                        childWeightsByAnimationAndElement,
                        outFramePose.AnimModelMatrix,
                        nowChildKeyFrameByAnimation,
                        nextChildKeyFrameByAnimation,
                        depth
                    );
                }
            }
        }


        static bool EleWeightDebug=false;
        Dictionary<string, string> eleWeights = new Dictionary<string, string>();
        public override string DumpCurrentState()
        {
            EleWeightDebug = true;
            eleWeights.Clear();
            calculateMatrices(1 / 60f);
            EleWeightDebug = false;

            return base.DumpCurrentState() + "\nElement weights:\n" + string.Join("\n", eleWeights.Select(x => x.Key + ": " + x.Value));
        }

    }
}
