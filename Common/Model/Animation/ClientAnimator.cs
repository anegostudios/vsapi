using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

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
        [ThreadStatic]
        static float[] ReusableIdentityMatrix;
        [ThreadStatic]
        static float[] localTransformMatrix;
        [ThreadStatic]
        static float[] tmpMatrix;
        public Dictionary<int, AnimationJoint> jointsById;
        protected HashSet<int> jointsDone = new HashSet<int>();

        public static int MaxConcurrentAnimations = 16;
        int maxDepth;
        List<ElementPose>[][] frameByDepthByAnimation;
        List<ElementPose>[][] nextFrameTransformsByAnimation;
        ShapeElementWeights[][][] weightsByAnimationAndElement;


        Action<string, AnimationSound> onShouldPlaySoundListener = null;

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
            }
            else
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
            Action<string, AnimationSound> onShouldPlaySoundListener = null
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
            Action<string, AnimationSound> onShouldPlaySoundListener = null
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
            Action<string, AnimationSound> onShouldPlaySoundListener = null
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
            for (int i = 0; i < defaultPoseMatrices.Length; i += 16)
            {
                Buffer.BlockCopy(identMat, 0, defaultPoseMatrices, i * sizeof(float), 16 * sizeof(float));
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
                        AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose()
                        {
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
                        AttachmentPointByCode[apoint.Code] = new AttachmentPointAndPose()
                        {
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

                if (anim.meta.AnimationSounds != null && anim.meta.AnimationSounds.Length > 0 && onShouldPlaySoundListener != null)
                {
                    if (anim.SoundPlayedAtIteration == null) anim.SoundPlayedAtIteration = new int[anim.meta.AnimationSounds.Length].Fill(-1);

                    for (int i = 0; i < anim.meta.AnimationSounds.Length; i++)
                    {
                        var animsound = anim.meta.AnimationSounds[i];
                        if (anim.CurrentFrame >= animsound.Frame && anim.SoundPlayedAtIteration[i] != anim.Iterations && anim.Active)
                        {
                            onShouldPlaySoundListener(anim.meta.Code, animsound);
                            anim.SoundPlayedAtIteration[i] = anim.Iterations;
                        }
                    }
                }
            }

            base.OnFrame(activeAnimationsByAnimCode, dt);
        }

        // Override to calculate transformation matrices on each frame
        protected override void calculateMatrices(float dt)
        {
            // If matrix calculation is disabled, exit the method
            if (!CalculateMatrices)
                return;

            // Clear the set of processed joints
            jointsDone.Clear();

            // Initialize animation version (used for caching transformations)
            int animVersion = 0;

            // Limit the number of animations to process
            int safeAnimCount = Math.Min(activeAnimCount, MaxConcurrentAnimations);

            // Process each active animation
            for (int j = 0; j < safeAnimCount; j++)
            {
                RunningAnimation anim = CurAnims[j];

                // Set element weights for the current animation
                weightsByAnimationAndElement[0][j] = anim.ElementWeights;

                // Track maximum animation version among active ones
                animVersion = Math.Max(animVersion, anim.Animation.Version);

                // Get previous and next keyframes for current time
                AnimationFrame[] prevNextFrame = anim.Animation.PrevNextKeyFrameByFrame[(int)anim.CurrentFrame % anim.Animation.QuantityFrames];
                frameByDepthByAnimation[0][j] = prevNextFrame[0].RootElementTransforms;
                prevFrame[j] = prevNextFrame[0].FrameNumber;

                // Determine next frame based on animation end handling type
                if (anim.Animation.OnAnimationEnd == EnumEntityAnimationEndHandling.Hold && (int)anim.CurrentFrame + 1 == anim.Animation.QuantityFrames)
                {
                    // If animation should hold on the last frame
                    nextFrameTransformsByAnimation[0][j] = prevNextFrame[0].RootElementTransforms;
                    nextFrame[j] = prevNextFrame[0].FrameNumber;
                }
                else
                {
                    // Otherwise move to next frame for interpolation
                    nextFrameTransformsByAnimation[0][j] = prevNextFrame[1].RootElementTransforms;
                    nextFrame[j] = prevNextFrame[1].FrameNumber;
                }
            }

            // Recursively calculate transformation matrices for all hierarchy elements
            calculateMatrices(
                animVersion,
                dt,
                RootPoses,
                weightsByAnimationAndElement[0],
                ReusableIdentityMatrix ??= Mat4f.Create(),
                frameByDepthByAnimation[0],
                nextFrameTransformsByAnimation[0],
                0
            );

            // Initialize transformation matrices for joints not used in animations
            var TransformationMatrices = this.TransformationMatrices;
            if (TransformationMatrices != null)
            {
                // Iterate through joint matrices (each matrix takes 16 elements)
                for (int jointidMul16 = 0; jointidMul16 < TransformationMatrices.Length; jointidMul16 += 16)
                {
                    // Skip joints that are active in animation
                    if (jointsById.ContainsKey(jointidMul16 / 16))
                        continue;

                    // Copy identity matrix for unused joints
                    Buffer.BlockCopy(identMat, 0, TransformationMatrices, jointidMul16 * sizeof(float), 16 * sizeof(float));
                }
            }

            // Synchronize model matrices for attachment points
            foreach (var val in AttachmentPointByCode)
            {
                var cachedMatrix = val.Value.CachedPose.AnimModelMatrix;
                var animMatrix = val.Value.AnimModelMatrix;
                // Copy current transformation matrix to attachment point animation matrix
                Buffer.BlockCopy(cachedMatrix, 0, animMatrix, 0, 16 * sizeof(float));
            }
        }

        // Recursive calculation of transformation matrices for hierarchy elements
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
            // Increment recursion depth
            depth++;
            List<ElementPose>[] nowChildKeyFrameByAnimation = this.frameByDepthByAnimation[depth];
            List<ElementPose>[] nextChildKeyFrameByAnimation = this.nextFrameTransformsByAnimation[depth];
            ShapeElementWeights[][] childWeightsByAnimationAndElement = this.weightsByAnimationAndElement[depth];
            localTransformMatrix ??= Mat4f.Create();

            // Process each child element in current pose
            for (int childPoseIndex = 0; childPoseIndex < outFrame.Count; childPoseIndex++)
            {
                ElementPose outFramePose = outFrame[childPoseIndex];
                ShapeElement elem = outFramePose.ForElement;
                // Set model matrix for current element
                outFramePose.SetMat(modelMatrix);
                // Initialize local transformation matrix with identity matrix
                Mat4f.Identity(localTransformMatrix);
                // Clear pose before calculating weighted transformations
                outFramePose.Clear();
                float weightSum = 0f;

#if DEBUG
                    // Debug information about element weights
                    StringBuilder sb = null;
                    if (EleWeightDebug) sb = new StringBuilder();
#endif

                // Limit the number of animations to process
                int safeAnimCount = Math.Min(activeAnimCount, MaxConcurrentAnimations);

                // First pass: calculate total weight of all influencing animations
                for (int animIndex = 0; animIndex < safeAnimCount; animIndex++)
                {
                    if (childPoseIndex >= weightsByAnimationAndElement[animIndex].Length)
                        continue;

                    RunningAnimation anim = CurAnims[animIndex];
                    ShapeElementWeights sew = weightsByAnimationAndElement[animIndex][childPoseIndex];
                    // Only blending modes other than "Add" participate in weight normalization
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

                // Second pass: blend transformations of all active animations
                for (int animIndex = 0; animIndex < safeAnimCount; animIndex++)
                {
                    if (childPoseIndex >= weightsByAnimationAndElement[animIndex].Length)
                        continue;

                    RunningAnimation anim = CurAnims[animIndex];
                    ShapeElementWeights sew = weightsByAnimationAndElement[animIndex][childPoseIndex];
                    // Calculate final weighted coefficient for current animation
                    anim.CalcBlendedWeight(weightSum / sew.Weight, sew.BlendMode);

                    ElementPose nowFramePose = nowKeyFrameByAnimation[animIndex][childPoseIndex];
                    ElementPose nextFramePose = nextInKeyFrameByAnimation[animIndex][childPoseIndex];
                    int prevFrame = this.prevFrame[animIndex];
                    int nextFrame = this.nextFrame[animIndex];

                    // Calculate distance between keyframes (accounting for cyclic animations)
                    float keyFrameDist = nextFrame > prevFrame ? (nextFrame - prevFrame) : (anim.Animation.QuantityFrames - prevFrame + nextFrame);
                    // Calculate distance from previous frame to current animation frame
                    float curFrameDist = anim.CurrentFrame >= prevFrame ? (anim.CurrentFrame - prevFrame) : (anim.Animation.QuantityFrames - prevFrame + anim.CurrentFrame);
                    // Calculate interpolation coefficient (0-1) between keyframes
                    float lerp = keyFrameDist < 1e-6f ? 0f : curFrameDist / keyFrameDist;

                    // Linear interpolation between current and next frame with animation weight applied
                    outFramePose.Add(nowFramePose, nextFramePose, lerp, anim.BlendedWeight);

                    // Prepare data for recursive child element processing
                    nowChildKeyFrameByAnimation[animIndex] = nowFramePose.ChildElementPoses;
                    childWeightsByAnimationAndElement[animIndex] = sew.ChildElements;
                    nextChildKeyFrameByAnimation[animIndex] = nextFramePose.ChildElementPoses;
                }

                // Get and apply local transformation matrix for element
                elem.GetLocalTransformMatrix(animVersion, localTransformMatrix, outFramePose);
                Mat4f.Mul(outFramePose.AnimModelMatrix, outFramePose.AnimModelMatrix, localTransformMatrix);

                // If this is client (joint matrices are calculated), update joint transformation matrices
                if (TransformationMatrices != null)
                {
                    // If element has a joint and this joint hasn't been processed yet
                    if (elem.JointId > 0 && !jointsDone.Contains(elem.JointId))
                    {
                        var tmpMatrixLocal = tmpMatrix ??= Mat4f.Create();
                        // Apply inverse model transformation to get final joint matrix
                        Mat4f.Mul(tmpMatrixLocal, outFramePose.AnimModelMatrix, elem.inverseModelTransform);
                        int index = 16 * elem.JointId;
                        var transformationMatrices = TransformationMatrices;

                        // Check if there's space in the matrix array (in case a mod extended the hierarchy)
                        if (index + 16 > transformationMatrices.Length)
                        {
                            var transformationMatricesDefault = this.TransformationMatricesDefaultPose;
                            // Re-initialize matrices with larger size
                            initMatrices(elem.JointId + 1);
                            // Copy previously calculated matrices to new array
                            Buffer.BlockCopy(transformationMatrices, 0, this.TransformationMatrices, 0, transformationMatrices.Length * sizeof(float));
                            Buffer.BlockCopy(transformationMatricesDefault, 0, this.TransformationMatricesDefaultPose, 0, transformationMatricesDefault.Length * sizeof(float));
                            transformationMatrices = this.TransformationMatrices;
                        }

                        // Copy joint matrix to global transformation matrices array
                        Buffer.BlockCopy(tmpMatrix, 0, transformationMatrices, index * sizeof(float), 16 * sizeof(float));
                        // Mark joint as processed
                        jointsDone.Add(elem.JointId);
                    }
                }

                // Recursively process child elements
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

#if DEBUG
        static bool EleWeightDebug=false;
#endif
        Dictionary<string, string> eleWeights = new Dictionary<string, string>();
        public override string DumpCurrentState()
        {
#if DEBUG
            EleWeightDebug = true;
#endif
            eleWeights.Clear();
            calculateMatrices(1 / 60f);
#if DEBUG
            EleWeightDebug = false;
#endif

            return base.DumpCurrentState() + "\nElement weights:\n" + string.Join("\n", eleWeights.Select(x => x.Key + ": " + x.Value));
        }

    }
}
