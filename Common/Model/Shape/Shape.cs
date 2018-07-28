using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Shape
    {
        [JsonProperty]
        public Dictionary<string, AssetLocation> Textures;

        [JsonProperty]
        public ShapeElement[] Elements;

        [JsonProperty]
        public Animation[] Animations;

        [JsonProperty]
        public int TextureWidth = 16;

        [JsonProperty]
        public int TextureHeight = 16;

        public Dictionary<int, AnimationJoint> JointsById = new Dictionary<int, AnimationJoint>();
        public Dictionary<string, AttachmentPoint> AttachmentPointsByCode = new Dictionary<string, AttachmentPoint>();

        public void ResolveReferences(ILogger errorLogger, string shapeName)
        {
            Dictionary<string, ShapeElement> elementsByName = new Dictionary<string, ShapeElement>();
            CollectElements(Elements, elementsByName);

            for (int i = 0; Animations != null && i < Animations.Length; i++)
            {
                Animation anim = Animations[i];
                for (int j = 0; j < anim.KeyFrames.Length; j++)
                {
                    AnimationKeyFrame keyframe = anim.KeyFrames[j];
                    ResolveReferences(errorLogger, shapeName, elementsByName, keyframe);

                    foreach (AnimationKeyFrameElement kelem in keyframe.Elements.Values)
                    {
                        kelem.Frame = keyframe.Frame;
                    }
                }

                if (anim.Code == null || anim.Code.Length == 0)
                {
                    anim.Code = anim.Name.ToLowerInvariant().Replace(" ", "");
                }
            }

            for (int i = 0; i < Elements.Length; i++)
            {
                ShapeElement elem = Elements[i];
                elem.ResolveRefernces();

                CollectAttachmentPoints(elem);
            }

            
        }

        private void CollectAttachmentPoints(ShapeElement elem)
        {
            for (int j = 0; elem.AttachmentPoints != null && j < elem.AttachmentPoints.Length; j++)
            {
                AttachmentPointsByCode[elem.AttachmentPoints[j].Code] = elem.AttachmentPoints[j];
            }

            for (int j = 0; elem.Children != null && j < elem.Children.Length; j++)
            {
                CollectAttachmentPoints(elem.Children[j]);
            }
        }

        private void ResolveReferences(ILogger errorLogger, string shapeName, Dictionary<string, ShapeElement> elementsByName, AnimationKeyFrame kf)
        {
            if (kf == null) return;

            foreach (var val in kf.Elements)
            {
                ShapeElement elem = null;
                elementsByName.TryGetValue(val.Key, out elem);

                if (elem == null)
                {
                    errorLogger.Error("Shape {0} has a key frame elmenent for which the referencing shape element {1} cannot be found.", shapeName, val.Key);

                    val.Value.ForElement = new ShapeElement();
                    continue;
                }

                val.Value.ForElement = elem;
            }
        }


        public void CollectElements(ShapeElement[] elements, Dictionary<string, ShapeElement> elementsByName)
        {
            if (elements == null) return;

            for (int i = 0; i < elements.Length; i++)
            {
                ShapeElement elem = elements[i];

                elementsByName[elem.Name] = elem;

                CollectElements(elem.Children, elementsByName);
            }
        }

        public void ResolveAndLoadJoints(ShapeElement headElement = null)
        {
            if (Animations == null) return;

            Dictionary<string, ShapeElement> elementsByName = new Dictionary<string, ShapeElement>();
            CollectElements(Elements, elementsByName);

            ShapeElement[] allElements = elementsByName.Values.ToArray();
            
            int jointCount = 0;

            HashSet<string> AnimatedElements = new HashSet<string>();

            for (int i = 0; i < Animations.Length; i++)
            {
                Animation anim = Animations[i];

                for (int j = 0; j < anim.KeyFrames.Length; j++)
                {
                    AnimationKeyFrame kf = anim.KeyFrames[j];
                    AnimatedElements.AddRange(kf.Elements.Keys.ToArray());

                    kf.Resolve(anim, allElements);
                }
            }

            foreach (ShapeElement elem in elementsByName.Values)
            {
                elem.JointId = 0;
            }

            int maxDepth = 0;

            foreach (string code in AnimatedElements)
            {
                ShapeElement elem = elementsByName[code];
                AnimationJoint joint = new AnimationJoint() { JointId = ++jointCount, Element = elem };
                JointsById[joint.JointId] = joint;
                
                maxDepth = Math.Max(maxDepth, elem.GetParentPath().Count);
            }

            // Ensure that the head gets a jointid
            if (headElement != null && !AnimatedElements.Contains(headElement.Name))
            {
                AnimationJoint joint = new AnimationJoint() { JointId = ++jointCount, Element = headElement };
                JointsById[joint.JointId] = joint;
                maxDepth = Math.Max(maxDepth, headElement.GetParentPath().Count);
            }


            // Iteratively and recursively assigns the lowest depth to highest depth joints to all elements
            // prevents that we overwrite a child joint id with a parent joint id
            for (int depth = 0; depth <= maxDepth; depth++)
            {
                foreach (AnimationJoint joint in JointsById.Values)
                {
                    if (joint.Element.GetParentPath().Count != depth) continue;

                    joint.Element.SetJointId(joint.JointId);
                }
            }
            
        }
    }
}
