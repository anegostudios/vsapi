using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AnimationKeyFrame
    {
        /// <summary>
        /// The ID of the keyframe.
        /// </summary>
        [JsonProperty]
        public int Frame;

        /// <summary>
        /// The elements of the keyframe.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, AnimationKeyFrameElement> Elements;


        IDictionary<ShapeElement, AnimationKeyFrameElement> ElementsByShapeElement;

        /// <summary>
        /// Resolves the keyframe animation for which elements are important.
        /// </summary>
        /// <param name="allElements"></param>
        [System.Obsolete("Use the overload taking a Dictionary argument instead for higher performance on large sets")]
        public void Resolve(ShapeElement[] allElements)
        {
            if (Elements == null) return;

            foreach (var val in Elements)
            {
                val.Value.Frame = Frame;
            }

            foreach (ShapeElement elem in allElements)
            {
                if (elem == null) continue;
                if (Elements.TryGetValue(elem.Name, out AnimationKeyFrameElement kelem)) ElementsByShapeElement[elem] = kelem;
            }
        }

        /// <summary>
        /// Resolves the keyframe animation for which elements are important.
        /// </summary>
        /// <param name="allElements"></param>
        public void Resolve(Dictionary<string, ShapeElement> allElements)
        {
            if (Elements == null) return;

            ElementsByShapeElement = new FastSmallDictionary<ShapeElement, AnimationKeyFrameElement>(Elements.Count);
            foreach (var val in Elements)
            {
                AnimationKeyFrameElement kelem = val.Value;
                kelem.Frame = Frame;
                allElements.TryGetValue(val.Key, out ShapeElement elem);
                if (elem != null) ElementsByShapeElement[elem] = kelem;
            }
        }

        internal AnimationKeyFrameElement GetKeyFrameElement(ShapeElement forElem)
        {
            if (forElem == null) return null;
            ElementsByShapeElement.TryGetValue(forElem, out AnimationKeyFrameElement kelem);
            return kelem;
        }

        public AnimationKeyFrame Clone()
        {
            return new AnimationKeyFrame()
            {
                Elements = Elements == null ? null : new Dictionary<string, AnimationKeyFrameElement>(Elements),
                Frame = Frame
            };

        }
    }
}
