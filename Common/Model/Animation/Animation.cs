using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public enum EnumEntityActivityStoppedHandling
    {
        PlayTillEnd = 0,
        Rewind = 1,
        Stop = 2,
        EaseOut = 3
    }

    public enum EnumEntityAnimationEndHandling
    {
        Repeat = 0,
        Hold = 1,
        Stop = 2
    }




    /// <summary>
    /// Represents a shape animation and can calculate the transformation matrices for each frame to be sent to the shader
    /// Process
    /// 1. For each frame, for each root element, calculate the transformation matrix. Curent model matrix is identy mat.
    /// 1.1. Get previous and next key frame. Apply translation, rotation and scale to model matrix.
    /// 1.2. Store this matrix as animationmatrix in list
    /// 1.3. For each child element
    /// 1.3.1. Multiply local transformation matrix with the animation matrix. This matrix is now the curent model matrix. Go to 1 with child elements as root elems
    /// 
    /// 2. For each frame, for each joint
    /// 2.1. Calculate the inverse model matrix 
    /// 2.2. Multiply stored animationmatrix with the inverse model matrix
    /// 
    /// 3. done
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Animation
    {
        [JsonProperty]
        public int QuantityFrames;
    
        [JsonProperty]
        public string Name;

        [JsonProperty]
        public string Code;

        [JsonProperty]
        public AnimationKeyFrame[] KeyFrames;

        [JsonProperty]
        public EnumEntityActivityStoppedHandling OnActivityStopped = EnumEntityActivityStoppedHandling.Rewind;

        [JsonProperty]
        public EnumEntityAnimationEndHandling OnAnimationEnd = EnumEntityAnimationEndHandling.Repeat;



        public uint CodeCrc32;

        public AnimationFrame[][] PrevNextKeyFrameByFrame;


        protected HashSet<int> jointsDone = new HashSet<int>();


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Code == null) Code = Name;
            CodeCrc32 = AnimationMetaData.GetCrc32(Code);
        }


        /// <summary>
        /// Compiles the animation into a bunch of matrices, 31 matrices per frame.
        /// </summary>
        /// <param name="rootElements"></param>
        /// <param name="jointsById"></param>
        /// <param name="recursive">When false, will only do root elements</param>
        public void GenerateAllFrames(ShapeElement[] rootElements, Dictionary<int, AnimationJoint> jointsById, bool recursive = true)
        {
            AnimationFrame[] resolvedKeyFrames = new AnimationFrame[KeyFrames.Length];

            for (int i = 0; i < resolvedKeyFrames.Length; i++)
            {
                resolvedKeyFrames[i] = new AnimationFrame() { FrameNumber = KeyFrames[i].Frame } ;
            }

            if (KeyFrames.Length == 0) return;

            if (jointsById.Count > GlobalConstants.MaxAnimatedElements)
            {
                throw new Exception("Max joint cap of "+ GlobalConstants.MaxAnimatedElements + " reached. Sorry, you'll have to simplify your model if you want it to be animated. (until some programmer finds another solution to pass on more joint data through shader uniforms)");
            }

            for (int i = 0; i < resolvedKeyFrames.Length; i++)
            {
                jointsDone.Clear();
                GenerateFrame(i, resolvedKeyFrames, rootElements, jointsById, Mat4f.Create(), resolvedKeyFrames[i].RootElementTransforms, recursive);
            }

            for (int i = 0; i < resolvedKeyFrames.Length; i++)
            {
                resolvedKeyFrames[i].FinalizeMatrices(jointsById);
            }

            PrevNextKeyFrameByFrame = new AnimationFrame[QuantityFrames][];
            for (int i = 0; i < QuantityFrames; i++)
            {
                AnimationFrame left, right;
                getLeftRightResolvedFrame(i, resolvedKeyFrames, out left, out right);

                PrevNextKeyFrameByFrame[i] = new AnimationFrame[] { left, right };
            }
        }



        protected void GenerateFrame(int indexNumber, AnimationFrame[] resKeyFrames, ShapeElement[] elements, Dictionary<int, AnimationJoint> jointsById, float[] modelMatrix, List<ElementPose> transforms, bool recursive = true)
        {
            int frameNumber = resKeyFrames[indexNumber].FrameNumber;

            for (int i = 0; i < elements.Length; i++)
            {
                ShapeElement element = elements[i];

                ElementPose animTransform = new ElementPose();
                animTransform.ForElement = element;

                GenerateFrameForElement(frameNumber, element, ref animTransform);
                transforms.Add(animTransform);

                float[] animModelMatrix = Mat4f.CloneIt(modelMatrix);
                Mat4f.Mul(animModelMatrix, animModelMatrix, element.GetLocalTransformMatrix(null, animTransform));

                if (element.JointId > 0 && !jointsDone.Contains(element.JointId))
                {
                    resKeyFrames[indexNumber].SetTransform(element.JointId, animModelMatrix);
                    jointsDone.Add(element.JointId);
                }

                if (recursive && element.Children != null)
                {
                    GenerateFrame(indexNumber, resKeyFrames, element.Children, jointsById, animModelMatrix, animTransform.ChildElementPoses);
                }



            }
        }



        protected void GenerateFrameForElement(int frameNumber, ShapeElement element, ref ElementPose transform)
        {
            for (int flag = 0; flag < 3; flag++)
            {
                AnimationKeyFrameElement curKelem, nextKelem;

                getTwoKeyFramesElementForFlag(frameNumber, element, flag, out curKelem, out nextKelem);

                if (curKelem == null) continue;


                float t;

                if (nextKelem == null || curKelem == nextKelem)
                {
                    nextKelem = curKelem;
                    t = 0;
                }
                else
                {
                    if (nextKelem.Frame < curKelem.Frame)
                    {
                        int quantity = nextKelem.Frame + (QuantityFrames - curKelem.Frame);
                        int framePos = GameMath.Mod(frameNumber - curKelem.Frame, QuantityFrames);
                        
                        t = (float)framePos / quantity;
                    }
                    else
                    {
                        t = (float)(frameNumber - curKelem.Frame) / (nextKelem.Frame - curKelem.Frame);
                    }
                }


                lerpKeyFrameElement(curKelem, nextKelem, flag, t, ref transform);
            }
        }


        protected void lerpKeyFrameElement(AnimationKeyFrameElement prev, AnimationKeyFrameElement next, int forFlag, float t, ref ElementPose transform)
        {
            if (prev == null && next == null) return;

            t = GameMath.SmoothStep(t);

            // Applies the transforms in model space
            if (forFlag == 0)
            {
                transform.translateX = GameMath.Lerp((float)prev.OffsetX / 16f, (float)next.OffsetX / 16f, t);
                transform.translateY = GameMath.Lerp((float)prev.OffsetY / 16f, (float)next.OffsetY / 16f, t);
                transform.translateZ = GameMath.Lerp((float)prev.OffsetZ / 16f, (float)next.OffsetZ / 16f, t);
            }
            else if (forFlag == 1)
            {
                transform.degX = GameMath.Lerp((float)prev.RotationX, (float)next.RotationX, t);
                transform.degY = GameMath.Lerp((float)prev.RotationY, (float)next.RotationY, t);
                transform.degZ = GameMath.Lerp((float)prev.RotationZ, (float)next.RotationZ, t);
            }
            else
            {
                transform.scaleX = GameMath.Lerp((float)prev.StretchX, (float)next.StretchX, t);
                transform.scaleY = GameMath.Lerp((float)prev.StretchY, (float)next.StretchY, t);
                transform.scaleZ = GameMath.Lerp((float)prev.StretchZ, (float)next.StretchZ, t);
            }
        }


        

        protected void getTwoKeyFramesElementForFlag(int frameNumber, ShapeElement forElement, int forFlag, out AnimationKeyFrameElement left, out AnimationKeyFrameElement right)
        {
            left = null;
            right = null;

            int rightKfIndex = seekRightKeyFrame(frameNumber, forElement, forFlag);
            if (rightKfIndex == -1) return;

            right = KeyFrames[rightKfIndex].GetKeyFrameElement(forElement);

            int leftKfIndex = seekLeftKeyFrame(rightKfIndex, forElement, forFlag);
            if (leftKfIndex == -1)
            {
                left = right;
                return;
            }

            left = KeyFrames[leftKfIndex].GetKeyFrameElement(forElement);
        }


        private int seekRightKeyFrame(int aboveFrameNumber, ShapeElement forElement, int forFlag)
        {
            int firstIndex = -1;

            for (int i = 0; i < KeyFrames.Length; i++)
            {
                AnimationKeyFrame keyframe = KeyFrames[i];
                
                AnimationKeyFrameElement kelem = keyframe.GetKeyFrameElement(forElement);
                if (kelem != null && kelem.IsSet(forFlag))
                {
                    if (firstIndex == -1) firstIndex = i;
                    if (keyframe.Frame <= aboveFrameNumber) continue;

                    return i;
                }
            }

            return firstIndex;
        }

        private int seekLeftKeyFrame(int leftOfKeyFrameIndex, ShapeElement forElement, int forFlag)
        {
            for (int i = 0; i < KeyFrames.Length; i++)
            {
                int index = GameMath.Mod(leftOfKeyFrameIndex - i - 1, KeyFrames.Length);
                AnimationKeyFrame keyframe = KeyFrames[index];

                AnimationKeyFrameElement kelem = keyframe.GetKeyFrameElement(forElement);
                if (kelem != null && kelem.IsSet(forFlag))
                {
                    return index;
                }
            }

            return -1;
        }


        protected void getLeftRightResolvedFrame(int frameNumber, AnimationFrame[] frames, out AnimationFrame left, out AnimationFrame right)
        {
            left = null;
            right = null;

            // Go left of frameNumber until we hit the first keyframe
            int keyframeIndex = frames.Length - 1;
            bool loopAround = false;

            while (keyframeIndex >= -1)
            {
                AnimationFrame keyframe = frames[GameMath.Mod(keyframeIndex, frames.Length)];
                keyframeIndex--;

                if (keyframe.FrameNumber <= frameNumber || loopAround)
                {
                    left = keyframe;
                    break;
                }

                if (keyframeIndex == -1) loopAround = true;
            }


            keyframeIndex += 2;
            AnimationFrame nextkeyframe = frames[GameMath.Mod(keyframeIndex, frames.Length)];
            right = nextkeyframe;
            return;
        }
    }
}
