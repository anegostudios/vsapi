using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        Dictionary<ShapeElement, AnimationKeyFrameElement> ElementsByShapeElement = new Dictionary<ShapeElement, AnimationKeyFrameElement>();

        /// <summary>
        /// Resolves the keyframe animation for which elements are important.
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="allElements"></param>
        public void Resolve(Animation anim, ShapeElement[] allElements)
        {
            if (Elements == null) return;

            foreach (var val in Elements)
            {
                val.Value.Frame = Frame;
            }

            foreach (ShapeElement elem in allElements)
            {
                AnimationKeyFrameElement kelem = FindKeyFrameElement(elem);
                if (kelem != null) ElementsByShapeElement[elem] = kelem;
            }
        }

        AnimationKeyFrameElement FindKeyFrameElement(ShapeElement forElem)
        {
            if (forElem == null) return null;

            foreach (var val in Elements)
            {
                if (forElem.Name == val.Key)
                {
                    return val.Value;
                }
            }

            return null;
        }

        internal AnimationKeyFrameElement GetKeyFrameElement(ShapeElement forElem)
        {
            if (forElem == null) return null;
            AnimationKeyFrameElement kelem = null;
            ElementsByShapeElement.TryGetValue(forElem, out kelem);
            return kelem;
        }
    }
}
