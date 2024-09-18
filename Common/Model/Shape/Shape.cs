using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using Vintagestory.API.Util;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The base shape for all json objects.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Shape
    {
        /// <summary>
        /// The collection of textures in the shape.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, AssetLocation> Textures;

        /// <summary>
        /// The elements of the shape.
        /// </summary>
        [JsonProperty]
        public ShapeElement[] Elements;

        /// <summary>
        /// The animations for the shape.
        /// </summary>
        [JsonProperty]
        public Animation[] Animations;

        public Dictionary<uint, Animation> AnimationsByCrc32 = new Dictionary<uint, Animation>();

        /// <summary>
        /// The width of the texture. (default: 16)
        /// </summary>
        [JsonProperty]
        public int TextureWidth = 16;

        /// <summary>
        /// The height of the texture (default: 16) 
        /// </summary>
        [JsonProperty]
        public int TextureHeight = 16;

        [JsonProperty]
        public Dictionary<string, int[]> TextureSizes = new Dictionary<string, int[]>();


        public Dictionary<int, AnimationJoint> JointsById = new Dictionary<int, AnimationJoint>();

        // This is never used anywhere. AnimatorBase.cs loads those separatly anyway
        //public Dictionary<string, AttachmentPoint> AttachmentPointsByCode = new Dictionary<string, AttachmentPoint>();


        [OnDeserialized]
        public void TrimTextureNamesAndResolveFaces(StreamingContext context)
        {
            foreach (ShapeElement el in Elements) el.TrimTextureNamesAndResolveFaces();
        }

        /// <summary>
        /// Attempts to resolve all references within the shape. Logs missing references them to the errorLogger
        /// </summary>
        /// <param name="errorLogger"></param>
        /// <param name="shapeNameForLogging"></param>
        public void ResolveReferences(ILogger errorLogger, string shapeNameForLogging)
        {
            Dictionary<string, ShapeElement> elementsByName = new Dictionary<string, ShapeElement>();
            CollectElements(Elements, elementsByName);

            for (int i = 0; Animations != null && i < Animations.Length; i++)
            {
                Animation anim = Animations[i];
                for (int j = 0; j < anim.KeyFrames.Length; j++)
                {
                    AnimationKeyFrame keyframe = anim.KeyFrames[j];
                    ResolveReferences(errorLogger, shapeNameForLogging, elementsByName, keyframe);

                    foreach (AnimationKeyFrameElement kelem in keyframe.Elements.Values)
                    {
                        kelem.Frame = keyframe.Frame;
                    }
                }

                if (anim.Code == null || anim.Code.Length == 0)
                {
                    anim.Code = anim.Name.ToLowerInvariant().Replace(" ", "");
                }

                AnimationsByCrc32[AnimationMetaData.GetCrc32(anim.Code)] = anim;
            }

            for (int i = 0; i < Elements.Length; i++)
            {
                ShapeElement elem = Elements[i];
                elem.ResolveRefernces();
            }
        }



        /// <summary>
        /// Prefixes texturePrefixCode to all textures in this shape. Required pre-step for stepparenting. The long arguments StepParentShape() calls this method.
        /// </summary>
        /// <param name="texturePrefixCode"></param>
        /// <param name="damageEffect"></param>
        /// <returns></returns>
        public bool SubclassForStepParenting(string texturePrefixCode, float damageEffect = 0f)
        {
            HashSet<string> textureCodes = new HashSet<string>();

            foreach (var childElem in Elements)
            {
                childElem.WalkRecursive((el) =>
                {
                    el.Name = texturePrefixCode + el.Name;

                    el.DamageEffect = damageEffect;

                    foreach (var face in el.FacesResolved)
                    {
                        if (face == null || !face.Enabled) continue;
                        textureCodes.Add(face.Texture);

                        face.Texture = texturePrefixCode + face.Texture;
                    }
                });
            }

            if (Textures != null)
            {
                var texturesizes = TextureSizes.ToArray();
                TextureSizes.Clear();
                foreach (var val in texturesizes)
                {
                    TextureSizes[texturePrefixCode + val.Key] = val.Value;
                    textureCodes.Remove(val.Key);
                }

                // Set default texturewidth/height for those not in the TextureSizes dict
                foreach (var code in textureCodes)
                {
                    TextureSizes[texturePrefixCode + code] = new int[] { TextureWidth, TextureHeight };
                }
            }

            if (this.Animations != null)
            {
                foreach (var anim in Animations)
                {
                    foreach (var kf in anim.KeyFrames)
                    {
                        Dictionary<string, AnimationKeyFrameElement> scElements = new Dictionary<string, AnimationKeyFrameElement>();
                        foreach (var kelem in kf.Elements)
                        {
                            scElements[texturePrefixCode + kelem.Key] = kelem.Value;
                        }

                        kf.Elements = scElements;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a step parented shape to this shape. If you plan to cache the childShape use the shorter argument method and call SubclassForStepParenting() only once on it
        /// </summary>
        /// <param name="childShape"></param>
        /// <param name="texturePrefixCode"></param>
        /// <param name="childLocationForLogging"></param>
        /// <param name="parentLocationForLogging"></param>
        /// <param name="logger"></param>
        /// <param name="onTexture"></param>
        /// <param name="damageEffect"></param>
        /// <returns></returns>
        public bool StepParentShape(Shape childShape, string texturePrefixCode, string childLocationForLogging, string parentLocationForLogging, ILogger logger, Action<string, AssetLocation> onTexture, float damageEffect = 0)
        {
            childShape.SubclassForStepParenting(texturePrefixCode, damageEffect);
            return StepParentShape(null, childShape.Elements, childShape, childLocationForLogging, parentLocationForLogging, logger, onTexture);
        }

        /// <summary>
        /// Adds a step parented shape to this shape, does not call the required pre-step SubclassForStepParenting()
        /// </summary>
        /// <param name="childShape"></param>
        /// <param name="childLocationForLogging"></param>
        /// <param name="parentLocationForLogging"></param>
        /// <param name="logger"></param>
        /// <param name="onTexture"></param>
        /// <returns></returns>
        public bool StepParentShape(Shape childShape, string childLocationForLogging, string parentLocationForLogging, ILogger logger, Action<string, AssetLocation> onTexture)
        {
            return StepParentShape(null, childShape.Elements, childShape, childLocationForLogging, parentLocationForLogging, logger, onTexture);
        }

        private bool StepParentShape(ShapeElement parentElem, ShapeElement[] elements, Shape childShape, string childLocationForLogging, string parentLocationForLogging, ILogger logger, Action<string, AssetLocation> onTexture)
        {
            bool anyElementAdded = false;
            foreach (var childElem in elements)
            {
                ShapeElement stepparentElem;

                if (childElem.Children != null)
                {
                    bool added = StepParentShape(childElem, childElem.Children, childShape, childLocationForLogging, parentLocationForLogging, logger, onTexture); ;
                    anyElementAdded |= added;
                }

                if (childElem.StepParentName != null)
                {
                    stepparentElem = GetElementByName(childElem.StepParentName, StringComparison.InvariantCultureIgnoreCase);
                    if (stepparentElem == null)
                    {
                        logger.Warning("Step parented shape {0} requires step parent element with name {1}, but no such element was found in parent shape {2}. Will not be visible.", childLocationForLogging, childElem.StepParentName, parentLocationForLogging);
                        continue;
                    }
                }
                else
                {
                    if (parentElem == null)
                    {
                        logger.Warning("Step parented shape {0} did not define a step parent element for parent shape {1}. Will not be visible.", childLocationForLogging, parentLocationForLogging);
                    }
                    continue;
                }

                if (parentElem != null)
                {
                    parentElem.Children = parentElem.Children.Remove(childElem);
                }

                if (stepparentElem.Children == null)
                {
                    stepparentElem.Children = new ShapeElement[] { childElem };
                }
                else
                {
                    stepparentElem.Children = stepparentElem.Children.Append(childElem);
                }

                childElem.ParentElement = stepparentElem;

                childElem.SetJointIdRecursive(stepparentElem.JointId);

                anyElementAdded = true;
            }

            if (!anyElementAdded) return false;


            if (childShape.Animations != null && Animations != null)
            {
                foreach (var gearAnim in childShape.Animations)
                {
                    var entityAnim = Animations.FirstOrDefault(anim => anim.Code == gearAnim.Code);
                    if (entityAnim == null) continue;

                    for (int gi = 0; gi < gearAnim.KeyFrames.Length; gi++)
                    {
                        var gearKeyFrame = gearAnim.KeyFrames[gi];
                        var entityKeyFrame = getOrCreateKeyFrame(entityAnim, gearKeyFrame.Frame);

                        foreach (var val in gearKeyFrame.Elements)
                        {
                            entityKeyFrame.Elements[val.Key] = val.Value;
                        }
                    }
                }
            }

            if (childShape.Textures != null)
            {
                foreach (var val in childShape.Textures)
                {
                    onTexture(val.Key, val.Value);
                }

                foreach (var val in childShape.TextureSizes)
                {
                    TextureSizes[val.Key] = val.Value;
                }

                if (childShape.Textures.Count > 0 && childShape.TextureSizes.Count == 0)
                {
                    foreach (var val in childShape.Textures)
                    {
                        TextureSizes[val.Key] = new int[] { childShape.TextureWidth, childShape.TextureHeight };
                    }
                }
            }

            return anyElementAdded;
        }

        private AnimationKeyFrame getOrCreateKeyFrame(Animation entityAnim, int frame)
        {
            for (int ei = 0; ei < entityAnim.KeyFrames.Length; ei++)
            {
                var entityKeyFrame = entityAnim.KeyFrames[ei];
                if (entityKeyFrame.Frame == frame)
                {
                    return entityKeyFrame;
                }
            }

            for (int ei = 0; ei < entityAnim.KeyFrames.Length; ei++)
            {
                var entityKeyFrame = entityAnim.KeyFrames[ei];
                if (entityKeyFrame.Frame > frame)
                {
                    var kfm = new AnimationKeyFrame() { Frame = frame, Elements = new Dictionary<string, AnimationKeyFrameElement>() };
                    entityAnim.KeyFrames = entityAnim.KeyFrames.InsertAt(kfm, ei);
                    return kfm;
                }
            }

            var kf = new AnimationKeyFrame() { Frame = frame, Elements = new Dictionary<string, AnimationKeyFrameElement>() };
            entityAnim.KeyFrames = entityAnim.KeyFrames.InsertAt(kf, 0);
            return kf;
        }



        /// <summary>
        /// Collects all the elements in the shape recursively.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementsByName"></param>
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

        [Obsolete("Must call ResolveAndFindJoints(errorLogger, shapeName, joints) instead")]
        public void ResolveAndLoadJoints(params string[] requireJointsForElements)
        {
            ResolveAndFindJoints(null, null, requireJointsForElements);
        }

        /// <summary>
        /// Resolves all joints and loads them.
        /// </summary>
        /// <param name="shapeName"></param>
        /// <param name="requireJointsForElements"></param>
        /// <param name="errorLogger"></param>
        public void ResolveAndFindJoints(ILogger errorLogger, string shapeName, params string[] requireJointsForElements)
        {
            if (Animations == null) return;

            Dictionary<string, ShapeElement> elementsByName = new Dictionary<string, ShapeElement>();
            CollectElements(Elements, elementsByName);

            ShapeElement[] allElements = elementsByName.Values.ToArray();
            
            int jointCount = 0;

            HashSet<string> AnimatedElements = new HashSet<string>();

            HashSet<string> animationCodes = new HashSet<string>();

            int version = -1;
            bool errorLogged = false;

            for (int i = 0; i < Animations.Length; i++)
            {
                Animation anim = Animations[i];

                if (animationCodes.Contains(anim.Code))
                {
                    errorLogger?.Warning("Shape {0}: Two or more animations use the same code '{1}'. This will lead to undefined behavior.", shapeName, anim.Code);
                }
                animationCodes.Add(anim.Code);

                if (version == -1) version = anim.Version;
                else if (version != anim.Version)
                {
                    if (!errorLogged) errorLogger?.Error("Shape {0} has mixed animation versions. This will cause incorrect animation blending.", shapeName);
                    errorLogged = true;
                }

                for (int j = 0; j < anim.KeyFrames.Length; j++)
                {
                    AnimationKeyFrame kf = anim.KeyFrames[j];
                    AnimatedElements.AddRange(kf.Elements.Keys.ToArray());

                    kf.Resolve(allElements);
                }
            }

            foreach (ShapeElement elem in elementsByName.Values)
            {
                elem.JointId = 0;
            }

            int maxDepth = 0;

            foreach (string code in AnimatedElements)
            {
                ShapeElement elem;
                elementsByName.TryGetValue(code, out elem);
                if (elem == null) continue;
                AnimationJoint joint = new AnimationJoint() { JointId = ++jointCount, Element = elem };
                JointsById[joint.JointId] = joint;
                
                maxDepth = Math.Max(maxDepth, elem.GetParentPath().Count);
            }

            // Currently used to require a joint for the head for head control, but not really used because
            // the player head also happens to be using in animations so it has a joint anyway
            foreach (string elemName in requireJointsForElements)
            {
                if (!AnimatedElements.Contains(elemName))
                {
                    ShapeElement elem = GetElementByName(elemName);
                    if (elem == null) continue;

                    AnimationJoint joint = new AnimationJoint() { JointId = ++jointCount, Element = elem };
                    JointsById[joint.JointId] = joint;
                    maxDepth = Math.Max(maxDepth, elem.GetParentPath().Count);
                }
            }
            


            // Iteratively and recursively assign the lowest depth to highest depth joints to all elements
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

        /// <summary>
        /// Tries to load the shape from the specified JSON file, with error logging
        /// <br/>Returns null if the file could not be found, or if there was an error
        /// </summary>
        /// <param name="api"></param>
        /// <param name="shapePath"></param>
        /// <returns></returns>
        public static Shape TryGet(ICoreAPI api, string shapePath)
        {
            ShapeElement.locationForLogging = shapePath;
            try
            {
                return api.Assets.TryGet(shapePath)?.ToObject<Shape>();
            }
            catch (Exception e)
            {
                api.World.Logger.Error("Exception thrown when trying to load shape file {0}", shapePath);
                api.World.Logger.Error(e);
                return null;
            }
        }

        /// <summary>
        /// Tries to load the shape from the specified JSON file, with error logging
        /// <br/>Returns null if the file could not be found, or if there was an error
        /// </summary>
        /// <param name="api"></param>
        /// <param name="shapePath"></param>
        /// <returns></returns>
        public static Shape TryGet(ICoreAPI api, AssetLocation shapePath)
        {
            ShapeElement.locationForLogging = shapePath;
            try
            {
                return api.Assets.TryGet(shapePath)?.ToObject<Shape>();
            }
            catch (Exception e)
            {
                api.World.Logger.Error("Exception thrown when trying to load shape file {0}\n{1}", shapePath, e.Message);
                return null;
            }
        }

        public void WalkElements(string wildcardpath, Action<ShapeElement> onElement)
        {
            walkElements(Elements, wildcardpath, onElement);
        }

        private void walkElements(ShapeElement[] elements, string wildcardpath, Action<ShapeElement> onElement)
        {
            if (elements == null) return;

            string pathElem;
            string subPath;

            int slashIndex = wildcardpath.IndexOf('/');
            if (slashIndex >= 0) {
                pathElem = wildcardpath.Substring(0, slashIndex);
                subPath = wildcardpath.Substring(slashIndex + 1);
            } else
            {
                pathElem = wildcardpath;
                subPath = "";
                if (pathElem == "*") subPath = "*";
            }

            foreach (ShapeElement elem in elements)
            {
                if (pathElem == "*" || elem.Name.Equals(pathElem, StringComparison.InvariantCultureIgnoreCase))
                {
                    onElement(elem);
                    if (elem.Children != null)
                    {
                        walkElements(elem.Children, subPath, onElement);
                    }
                }
            }
        }

        /// <summary>
        /// Recursively searches the element by name from the shape.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <param name="stringComparison"></param>
        /// <returns>The shape element or null if none was found</returns>
        public ShapeElement GetElementByName(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return GetElementByName(name, Elements, stringComparison);
        }

        ShapeElement GetElementByName(string name, ShapeElement[] elems, StringComparison stringComparison)
        {
            if (elems == null) return null;

            foreach (ShapeElement elem in elems)
            {
                if (elem.Name.Equals(name, stringComparison)) return elem;
                if (elem.Children != null)
                {
                    ShapeElement foundElem = GetElementByName(name, elem.Children, stringComparison);
                    if (foundElem != null) return foundElem;
                }
            }

            return null;
        }

        public void RemoveElements(string[] elementNames)
        {
            if (elementNames == null) return;
            
            foreach (var val in elementNames)
            {
                RemoveElementByName(val);
                RemoveElementByName("skinpart-" + val);
            }
            
        }

        /// <summary>
        /// Removes *all* elements with given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public bool RemoveElementByName(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return RemoveElementByName(name, ref Elements, stringComparison);
        }


        bool RemoveElementByName(string name, ref ShapeElement[] elems, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (elems == null) return false;

            bool removed=false;

            for (int i = 0; i < elems.Length; i++)
            {
                if (elems[i].Name.Equals(name, stringComparison))
                {
                    elems = elems.RemoveEntry(i);
                    removed = true;
                    i--;
                    continue;
                }

                if (RemoveElementByName(name, ref elems[i].Children, stringComparison))
                {
                    removed = true;
                }
            }

            return removed;
        }

        public ShapeElement[] CloneElements()
        {
            if (Elements == null) return null;

            ShapeElement[] elems = new ShapeElement[Elements.Length];

            for (int i = 0; i < elems.Length; i++)
            {
                elems[i] = Elements[i].Clone();
            }

            return elems;
        }

        public Animation[] CloneAnimations()
        {
            if (Animations == null) return null;

            Animation[] elems = new Animation[Animations.Length];

            for (int i = 0; i < Animations.Length; i++)
            {
                elems[i] = Animations[i].Clone();
            }

            return elems;
        }

        public void CacheInvTransforms() => CacheInvTransforms(Elements);

        public static void CacheInvTransforms(ShapeElement[] elements)
        {
            if (elements == null) return;

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].CacheInverseTransformMatrix();
                CacheInvTransforms(elements[i].Children);
            }
        }


        /// <summary>
        /// Creates a deep copy of the shape. If the shape has animations, then it also resolves references and joints to ensure the cloned shape is fully initialized
        /// </summary>
        /// <returns></returns>
        public Shape Clone()
        {
            var shape = new Shape()
            {
                Elements = CloneElements(),
                Animations = CloneAnimations(),
                TextureWidth = TextureWidth,
                TextureHeight = TextureHeight,
                TextureSizes = TextureSizes,
                Textures = Textures,
            };

            for (int i = 0; i < shape.Elements.Length; i++)
            {
                shape.Elements[i].ResolveRefernces();
            }

            return shape;
        }

        public void InitForAnimations(ILogger logger, string shapeNameForLogging, params string[] requireJointsForElements)
        {
            CacheInvTransforms();
            ResolveReferences(logger, shapeNameForLogging);
            ResolveAndFindJoints(logger, shapeNameForLogging, requireJointsForElements);
        }

        private void ResolveReferences(ILogger errorLogger, string shapeName, Dictionary<string, ShapeElement> elementsByName, AnimationKeyFrame kf)
        {
            if (kf?.Elements == null) return;

            foreach (var val in kf.Elements)
            {
                ShapeElement elem;
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

    }
}
