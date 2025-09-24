using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public static class TreeAttributeUtil
    {
        public static Vec3i GetVec3i(this ITreeAttribute tree, string code, Vec3i defaultValue = null)
        {
            if (!(tree.TryGetAttribute(code + "X", out var attr) && attr is IntAttribute codeX)) return defaultValue;
            return new Vec3i(codeX.value, tree.GetInt(code + "Y"), tree.GetInt(code + "Z"));
        }
        public static BlockPos GetBlockPos(this ITreeAttribute tree, string code, BlockPos defaultValue = null)
        {
            if (!(tree.TryGetAttribute(code + "X", out var attr) && attr is IntAttribute codeX)) return defaultValue;
            return new BlockPos(codeX.value, tree.GetInt(code + "Y"), tree.GetInt(code + "Z"));
        }


        public static void SetVec3i(this ITreeAttribute tree, string code, Vec3i value)
        {
            tree.SetInt(code + "X", value.X);
            tree.SetInt(code + "Y", value.Y);
            tree.SetInt(code + "Z", value.Z);
        }

        public static void SetBlockPos(this ITreeAttribute tree, string code, BlockPos value)
        {
            tree.SetInt(code + "X", value.X);
            tree.SetInt(code + "Y", value.Y);
            tree.SetInt(code + "Z", value.Z);
        }

        public static Vec3i[] GetVec3is(this ITreeAttribute tree, string code, Vec3i[] defaultValue = null)
        {
            if (!(tree.TryGetAttribute(code + "X", out var attrX) && attrX is IntArrayAttribute codeX)) return defaultValue;
            if (!(tree.TryGetAttribute(code + "Y", out var attrY) && attrY is IntArrayAttribute codeY)) return defaultValue;
            if (!(tree.TryGetAttribute(code + "Z", out var attrZ) && attrZ is IntArrayAttribute codeZ)) return defaultValue;

            int[] x = codeX.value;
            int[] y = codeY.value;
            int[] z = codeZ.value;

            Vec3i[] values = new Vec3i[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                values[i] = new Vec3i(x[i], y[i], z[i]);
            }

            return values;
        }


        public static void SetVec3is(this ITreeAttribute tree, string code, Vec3i[] value)
        {
            int[] x = new int[value.Length];
            int[] y = new int[value.Length];
            int[] z = new int[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                var v = value[i];
                x[i] = v.X;
                y[i] = v.Y;
                z[i] = v.Z;
            }

            tree[code + "X"] = new IntArrayAttribute(x);
            tree[code + "Y"] = new IntArrayAttribute(y);
            tree[code + "Z"] = new IntArrayAttribute(z);
        }

    }


    /// <summary>
    /// A datastructure to hold generic data for most primitives (int, string, float, etc.). But can also hold generic data using the ByteArrayAttribute + class serialization
    /// </summary>
    public class TreeAttribute : ITreeAttribute
    {
        private const int AttributeID = 6;
        public static Dictionary<int, Type> AttributeIdMapping = new Dictionary<int, Type>();

        protected int depth = 0;
        internal IDictionary<string, IAttribute> attributes = new Vintagestory.Common.ConcurrentSmallDictionary<string, IAttribute>(0);

        /// <summary>
        /// Will return null if given attribute does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IAttribute this[string key]
        {
            get
            {
                return attributes.TryGetValue(key);
            }

            set
            {
                attributes[key] = value;
            }
        }

        /// <summary>
        /// Amount of elements in this Tree attribute
        /// </summary>
        public int Count
        {
            get { return attributes.Count; }
        }

        /// <summary>
        /// Returns all values inside this tree attributes
        /// </summary>
        public IAttribute[] Values
        {
            get { return (IAttribute[])attributes.Values; }
        }

        public string[] Keys
        {
            get { return (string[])attributes.Keys; }
        }


        static TreeAttribute()
        {
            RegisterAttribute(1, typeof(IntAttribute));
            RegisterAttribute(2, typeof(LongAttribute));
            RegisterAttribute(3, typeof(DoubleAttribute));
            RegisterAttribute(4, typeof(FloatAttribute));
            RegisterAttribute(5, typeof(StringAttribute));
            RegisterAttribute(6, typeof(TreeAttribute));
            RegisterAttribute(7, typeof(ItemstackAttribute));
            RegisterAttribute(8, typeof(ByteArrayAttribute));
            RegisterAttribute(9, typeof(BoolAttribute));

            RegisterAttribute(10, typeof(StringArrayAttribute));
            RegisterAttribute(11, typeof(IntArrayAttribute));
            RegisterAttribute(12, typeof(FloatArrayAttribute));
            RegisterAttribute(13, typeof(DoubleArrayAttribute));
            RegisterAttribute(14, typeof(TreeArrayAttribute));
            RegisterAttribute(15, typeof(LongArrayAttribute));
            RegisterAttribute(16, typeof(BoolArrayAttribute));
        }

        public static void RegisterAttribute(int attrId, Type type)
        {
            AttributeIdMapping[attrId] = type;
        }

        public TreeAttribute()
        {

        }

        public static TreeAttribute CreateFromBytes(byte[] blockEntityData)
        {
            TreeAttribute tree = new TreeAttribute();
            using (MemoryStream ms = new MemoryStream(blockEntityData))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    tree.FromBytes(reader);
                }
            }

            return tree;
        }



        public virtual void FromBytes(BinaryReader stream)
        {
            if (depth > 30)
            {
                Console.WriteLine("Can't fully decode AttributeTree, beyond 30 depth limit");
                return;
            }

            attributes.Clear();

            byte attrId;
            while ((attrId = stream.ReadByte()) != 0)
            {
                string key = stream.ReadString();

                Type t = AttributeIdMapping[attrId];
                IAttribute attr = (IAttribute)Activator.CreateInstance(t);

                if (attr is TreeAttribute)
                {
                    ((TreeAttribute)attr).depth = depth + 1;
                }

                attr.FromBytes(stream);

                attributes[key] = attr;
            }
        }

        public virtual byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    ToBytes(writer);
                }

                return ms.ToArray();
            }
        }

        public virtual void FromBytes(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    FromBytes(reader);
                }
            }
        }


        public virtual void ToBytes(BinaryWriter stream)
        {
            foreach (var val in attributes)
            {
                // attrid
                stream.Write((byte)val.Value.GetAttributeId());
                // key
                stream.Write(val.Key);
                // value
                val.Value.ToBytes(stream);
            }

            TerminateWrite(stream);
        }

        public int GetAttributeId()
        {
            return AttributeID;
        }


        [Obsolete("May not return consistent results if the TreeAttribute changes between calls")]
        public int IndexOf(string key)
        {
            var keys = attributes.Keys;
            int i = 0;
            foreach (string testkey in attributes.Keys)
            {
                if (testkey == key)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }


        public IEnumerator<KeyValuePair<string, IAttribute>> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, IAttribute>> IEnumerable<KeyValuePair<string, IAttribute>>.GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        public void Clear()
        {
            attributes.Clear();
        }

        /// <summary>
        /// Set a value. Returns itself for method chaining
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TreeAttribute Set(string key, IAttribute value)
        {
            attributes[key] = value;
            return this;
        }


        public IAttribute GetAttribute(string key)
        {
            return attributes.TryGetValue(key);
        }

        /// <summary>
        /// True if this attribute exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasAttribute(string key)
        {
            return attributes.ContainsKey(key);
        }

        public bool TryGetAttribute(string key, out IAttribute value)
        {
            return attributes.TryGetValue(key, out value);
        }


        #region Quick access methods

        public IAttribute GetAttributeByPath(string path)
        {
            if (!path.Contains('/')) return this[path];

            string[] parts = path.Split('/');

            ITreeAttribute treeAttr = this;

            // Traverse down the tree hierarchy
            for (int i = 0; i < parts.Length - 1; i++)
            {
                IAttribute attr = treeAttr[parts[i]];

                if (attr is ITreeAttribute)
                {
                    treeAttr = (ITreeAttribute)attr;
                }
                else
                {
                    return null;
                }
            }

            return treeAttr[parts[parts.Length - 1]];
        }


        public void DeleteAttributeByPath(string path)
        {
            string[] parts = path.Split('/');

            ITreeAttribute treeAttr = this;

            // Traverse down the tree hierarchy
            for (int i = 0; i < parts.Length - 1; i++)
            {
                IAttribute attr = treeAttr[parts[i]];

                if (attr is ITreeAttribute)
                {
                    attr = (ITreeAttribute)attr;
                }
                else
                {
                    return;
                }
            }

            treeAttr?.RemoveAttribute(parts[parts.Length - 1]);
        }


        /// <summary>
        /// Removes an attribute
        /// </summary>
        /// <param name="key"></param>
        public virtual void RemoveAttribute(string key)
        {
            attributes.Remove(key);
        }

        /// <summary>
        /// Creates a bool attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetBool(string key, bool value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<bool> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new BoolAttribute(value);
        }

        /// <summary>
        /// Creates an int attribute with given key and value<br/>
        /// Side note: If you need this attribute to be compatible with deserialized json - use SetLong()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetInt(string key, int value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<int> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new IntAttribute(value);
        }

        /// <summary>
        /// Creates a long attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetLong(string key, long value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<long> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new LongAttribute(value);
        }

        /// <summary>
        /// Creates a double attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetDouble(string key, double value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<double> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new DoubleAttribute(value);
        }

        /// <summary>
        /// Creates a float attribute with given key and value<br/>
        /// Side note: If you need this attribute to be compatible with deserialized json - use SetDouble()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetFloat(string key, float value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<float> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new FloatAttribute(value);
        }

        public virtual void SetString(string key, string value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<string> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new StringAttribute(value);
        }

        public virtual void SetStringArray(string key, string[] values)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<string[]> sattr)
            {
                sattr.SetValue(values);
                return;
            }
            attributes[key] = new StringArrayAttribute(values);
        }

        /// <summary>
        /// Creates a byte[] attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetBytes(string key, byte[] value)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ScalarAttribute<byte[]> sattr)
            {
                sattr.SetValue(value);
                return;
            }
            attributes[key] = new ByteArrayAttribute(value);
        }

        public virtual void SetAttribute(string key, IAttribute value)
        {
            attributes[key] = value;
        }

        /// <summary>
        /// Sets given item stack with given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemstack"></param>
        public void SetItemstack(string key, ItemStack itemstack)
        {
            if (attributes.TryGetValue(key, out IAttribute attr) && attr is ItemstackAttribute sattr)
            {
                sattr.SetValue(itemstack);
                return;
            }
            attributes[key] = new ItemstackAttribute(itemstack);
        }

        /// <summary>
        /// Retrieves a bool or null if the key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool? TryGetBool(string key)
        {
            return (attributes.TryGetValue(key) as BoolAttribute)?.value;
        }

        public virtual int? TryGetInt(string key)
        {
            return (attributes.TryGetValue(key) as IntAttribute)?.value;
        }

        /// <summary>
        /// Retrieves a double or null if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual double? TryGetDouble(string key)
        {
            return (attributes.TryGetValue(key) as DoubleAttribute)?.value;
        }

        /// <summary>
        /// Retrieves a float or null if the key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual float? TryGetFloat(string key)
        {
            return (attributes.TryGetValue(key) as FloatAttribute)?.value;
        }

        /// <summary>
        /// Retrieves a bool or default value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetBool(string key, bool defaultValue = false)
        {
            BoolAttribute attr = attributes.TryGetValue(key) as BoolAttribute;
            return attr == null ? defaultValue : attr.value;
        }

        /// <summary>
        /// Retrieves an int or default value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetInt(string key, int defaultValue = 0)
        {
            IntAttribute attr = attributes.TryGetValue(key) as IntAttribute;
            return attr == null ? defaultValue : attr.value;
        }

        /// <summary>
        /// Same as (int)GetDecimal(key, defValue);
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetAsInt(string key, int defaultValue = 0)
        {
            return (int)GetDecimal(key, defaultValue);
        }

        public virtual bool GetAsBool(string key, bool defaultValue = false)
        {
            IAttribute attr = attributes.TryGetValue(key);
            if (attr is IntAttribute) return (int)attr.GetValue() > 0;
            if (attr is FloatAttribute) return (float)attr.GetValue() > 0;
            if (attr is DoubleAttribute) return (double)attr.GetValue() > 0;
            if (attr is LongAttribute) return (long)attr.GetValue() > 0;
            if (attr is StringAttribute) return (string)attr.GetValue() == "true" || (string)attr.GetValue() == "1";
            if (attr is BoolAttribute) return (bool)attr.GetValue();
            return defaultValue;
        }

        /// <summary>
        /// Retrieves an int, float, long or double value. Whatever attribute is found for given key, in aformentioned order. If its a string its converted to double
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual double GetDecimal(string key, double defaultValue = 0)
        {
            IAttribute attr = attributes.TryGetValue(key);
            if (attr is IntAttribute) return (int)attr.GetValue();
            if (attr is FloatAttribute) return (float)attr.GetValue();
            if (attr is DoubleAttribute) return (double)attr.GetValue();
            if (attr is LongAttribute) return (long)attr.GetValue();
            if (attr is StringAttribute) return ((string)attr.GetValue()).ToDouble();

            return defaultValue;
        }

        /// <summary>
        /// Retrieves a double or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual double GetDouble(string key, double defaultValue = 0)
        {
            DoubleAttribute attr = attributes.TryGetValue(key) as DoubleAttribute;
            return attr == null ? defaultValue : attr.value;
        }

        /// <summary>
        /// Retrieves a float or defaultvalue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual float GetFloat(string key, float defaultValue = 0)
        {
            FloatAttribute attr = attributes.TryGetValue(key) as FloatAttribute;
            return attr == null ? defaultValue : attr.value;
        }


        /// <summary>
        /// Retrieves a string attribute or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetString(string key, string defaultValue = null)
        {
            string val = (attributes.TryGetValue(key) as StringAttribute)?.value;
            return val == null ? defaultValue : val;
        }

        /// <summary>
        /// Retrieves the value of given attribute, independent of attribute type
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetAsString(string key, string defaultValue = null)
        {
            string val = attributes.TryGetValue(key)?.GetValue().ToString();
            return val == null ? defaultValue : val;
        }

        /// <summary>
        /// Retrieves a string or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string[] GetStringArray(string key, string[] defaultValue = null)
        {
            string[] val = (attributes.TryGetValue(key) as StringArrayAttribute)?.value;
            return val == null ? defaultValue : val;
        }

        /// <summary>
        /// Retrieves a byte array or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>

        public virtual byte[] GetBytes(string key, byte[] defaultValue = null)
        {
            byte[] val = (attributes.TryGetValue(key) as ByteArrayAttribute)?.value;
            return val == null ? defaultValue : val;
        }

        /// <summary>
        /// Retrieves an attribute tree or null if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual ITreeAttribute GetTreeAttribute(string key)
        {
            return attributes.TryGetValue(key) as ITreeAttribute;
        }

        /// <summary>
        /// Retrieves an attribute tree or adds it if key is not found.
        /// Throws an exception if the key does exist but is not a tree.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual ITreeAttribute GetOrAddTreeAttribute(string key)
        {
            var attr = attributes.TryGetValue(key);
            if (attr == null)
            {
                var result = new TreeAttribute();
                SetAttribute(key, result);
                return result;
            }
            else if (attr is ITreeAttribute result)
            {
                return result;
            }
            else throw new InvalidOperationException(
                $"The attribute with key '{ key }' is a { attr.GetType().Name }, not TreeAttribute.");
        }

        /// <summary>
        /// Retrieves an itemstack or defaultValue if key is not found. Be sure to call stack.ResolveBlockOrItem() after retrieving it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public ItemStack GetItemstack(string key, ItemStack defaultValue = null)
        {
            ItemStack stack = ((ItemstackAttribute)attributes.TryGetValue(key))?.value;
            return stack == null ? defaultValue : stack;
        }

        /// <summary>
        /// Retrieves a long or default value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual long GetLong(string key, long defaultValue = 0)
        {
            LongAttribute attr = (LongAttribute)attributes.TryGetValue(key);
            return attr == null ? defaultValue : attr.value;
        }

        /// <summary>
        /// Retrieves a long or null value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual long? TryGetLong(string key)
        {
            return ((LongAttribute)attributes.TryGetValue(key))?.value;
        }

        public virtual ModelTransform GetModelTransform(string key)
        {
            ITreeAttribute attr = GetTreeAttribute(key);
            if (attr == null) return null;

            ITreeAttribute torigin = attr.GetTreeAttribute("origin");
            ITreeAttribute trotation = attr.GetTreeAttribute("rotation");
            ITreeAttribute ttranslation = attr.GetTreeAttribute("translation");
            float scale = attr.GetFloat("scale", 1);

            FastVec3f origin = new FastVec3f(0.5f, 0.5f, 0.5f);
            if (torigin != null)
            {
                origin.X = torigin.GetFloat("x");
                origin.Y = torigin.GetFloat("y");
                origin.Z = torigin.GetFloat("z");
            }

            FastVec3f rotation = new FastVec3f();
            if (trotation != null)
            {
                rotation.X = trotation.GetFloat("x");
                rotation.Y = trotation.GetFloat("y");
                rotation.Z = trotation.GetFloat("z");
            }

            FastVec3f translation = new FastVec3f();
            if (ttranslation != null)
            {
                translation.X = ttranslation.GetFloat("x");
                translation.Y = ttranslation.GetFloat("y");
                translation.Z = ttranslation.GetFloat("z");
            }

            return new ModelTransform()
            {
                Scale = scale,
                Origin = origin,
                Translation = translation,
                Rotation = rotation
            };
        }




        public object GetValue()
        {
            return this;
        }


        #endregion





        /// <summary>
        /// Creates a deep copy of the attribute tree
        /// </summary>
        /// <returns></returns>
        public virtual ITreeAttribute Clone()
        {
            TreeAttribute tree = new TreeAttribute();

            foreach (var val in attributes)
            {
                tree[val.Key] = val.Value.Clone();
            }

            return tree;
        }


        IAttribute IAttribute.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Returns true if given tree contains all of elements of this one, but given tree may contain also more elements. Individual node values are exactly matched.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSubSetOf(IWorldAccessor worldForResolve, IAttribute other)
        {
            if (!(other is TreeAttribute)) return false;
            TreeAttribute otherTree = (TreeAttribute)other;

            if (attributes.Count > otherTree.attributes.Count) return false;

            foreach (var val in attributes)
            {
                if (GlobalConstants.IgnoredStackAttributes.Contains(val.Key)) continue;
                if (!otherTree.attributes.TryGetValue(val.Key, out IAttribute otherAttribute)) return false;

                if (val.Value is TreeAttribute)
                {
                    if (!(otherAttribute as TreeAttribute).IsSubSetOf(worldForResolve, val.Value)) return false;
                } else
                {
                    if (!otherAttribute.Equals(worldForResolve, val.Value)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if given tree exactly matches this one
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IWorldAccessor worldForResolve, IAttribute other)
        {
            if (!(other is TreeAttribute)) return false;
            TreeAttribute otherTree = (TreeAttribute)other;

            if (attributes.Count != otherTree.attributes.Count) return false;

            foreach (var val in attributes)
            {
                if (!otherTree.attributes.ContainsKey(val.Key)) return false;
                if (!otherTree.attributes[val.Key].Equals(worldForResolve, val.Value)) return false;
            }


            return true;
        }

        public bool Equals(IWorldAccessor worldForResolve, IAttribute other, params string[] ignorePaths)
        {
            return Equals(worldForResolve, other, "", ignorePaths);
        }

        public bool Equals(IWorldAccessor worldForResolve, IAttribute other, string currentPath, params string[] ignorePaths)
        {
            if (!(other is TreeAttribute)) return false;
            TreeAttribute otherTree = (TreeAttribute)other;

            if ((ignorePaths == null || ignorePaths.Length == 0) && attributes.Count != otherTree.attributes.Count) return false;
            
            // Test 1 and 2: 
            // - Check for exists in a, but missing in b
            // - Check for a != b
            foreach (var val in attributes)
            {
                string curPath = currentPath + (currentPath.Length > 0 ? "/" : "") + val.Key;
                if (ignorePaths != null && ignorePaths.Contains(curPath)) continue;
                if (!otherTree.attributes.ContainsKey(val.Key)) return false;

                IAttribute otherAttr = otherTree.attributes[val.Key];

                if (otherAttr is TreeAttribute)
                {
                    if (!((TreeAttribute)otherAttr).Equals(worldForResolve, val.Value, currentPath, ignorePaths)) return false;
                } else
                {
                    if (otherAttr is ItemstackAttribute)
                    {
                        if (!(otherAttr as ItemstackAttribute).Equals(worldForResolve, val.Value, ignorePaths))
                        {
                            return false;
                        }
                    } else
                    {
                        if (!otherAttr.Equals(worldForResolve, val.Value)) return false;
                    }
                    
                }
            }

            // Test 3: 
            // - Check for exists in b, but missing in a
            foreach (var val in otherTree.attributes)
            {
                string curPath = currentPath + (currentPath.Length > 0 ? "/" : "") + val.Key;
                if (ignorePaths != null && ignorePaths.Contains(curPath)) continue;
                if (!attributes.ContainsKey(val.Key)) return false;
            }


            return true;
        }




        public string ToJsonToken()
        {
            return ToJsonToken(attributes);
        }

        public static IAttribute FromJson(string json)
        {
            return new JsonObject(JToken.Parse(json)).ToAttribute();
        }

        public static string ToJsonToken(IEnumerable<KeyValuePair<string, IAttribute>> attributes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            int i = 0;

            foreach (var val in attributes)
            {
                if (i > 0) sb.Append(", ");
                i++;

                sb.Append("\"" + val.Key + "\": " + val.Value.ToJsonToken());
            }

            sb.Append(" }");

            return sb.ToString();
        }

        /// <summary>
        /// Merges the sourceTree into the current one
        /// </summary>
        /// <param name="sourceTree"></param>
        public virtual void MergeTree(ITreeAttribute sourceTree)
        {
            if (sourceTree is TreeAttribute srcTree)
            {
                MergeTree(this, srcTree);
            } else
            {
                throw new ArgumentException("Expected TreeAttribute but got " + sourceTree.GetType().Name + "! " + sourceTree.ToString() + "");
            }
        }

        protected static void MergeTree(TreeAttribute dstTree, TreeAttribute srcTree)
        {
            foreach (var srcVal in srcTree.attributes)
            {
                MergeAttribute(dstTree, srcVal.Key, srcVal.Value);
            }
        }

        protected static void MergeAttribute(TreeAttribute dstTree, string srcKey, IAttribute srcAttr)
        {
            IAttribute dstAttr;
            dstAttr = dstTree.attributes.TryGetValue(srcKey);

            if (dstAttr == null)
            {
                dstTree.attributes[srcKey] = srcAttr.Clone();
                return;
            }

            if (dstAttr.GetAttributeId() != srcAttr.GetAttributeId())
            {
                throw new Exception("Cannot merge attributes! Expected attributeId " + dstAttr.GetAttributeId().ToString() + " instead of " + srcAttr.GetAttributeId().ToString() + "! Existing: " + dstAttr.ToString() + ", new: " + srcAttr.ToString());
            }

            if (srcAttr is ITreeAttribute)
            {
                MergeTree(dstAttr as TreeAttribute, srcAttr as TreeAttribute);
            }
            else
            {
                dstTree.attributes[srcKey] = srcAttr.Clone();
            }
        }

        public OrderedDictionary<string, IAttribute> SortedCopy(bool recursive = false)
        {
            var sorted = attributes.OrderBy(x => x.Key);
            var sortedTree = new OrderedDictionary<string, IAttribute>();
            foreach (var entry in sorted)
            {
                var attribute = entry.Value;
                if (attribute is TreeAttribute tree && recursive) attribute = tree.ConsistentlyOrderedCopy();
                sortedTree.Add(entry.Key, attribute);
            }
            return sortedTree;
        }

        private IAttribute ConsistentlyOrderedCopy()
        {
            var sorted = attributes.OrderBy(x => x.Key).ToDictionary(pair => pair.Key,pair => pair.Value);
            foreach (var (key, attribute) in sorted)
            {
                if (attribute is TreeAttribute tree)
                {
                    sorted[key] = tree.ConsistentlyOrderedCopy();
                }
            }
            var sortedTree = new TreeAttribute();
            sortedTree.attributes.AddRange(sorted);
            return sortedTree;
        }

        public override int GetHashCode()
        {
            return GetHashCode(null);
        }

        public int GetHashCode(string[] ignoredAttributes)
        {
            int hashcode = 0;
            int i = 0;
            foreach (var val in attributes)
            {
                if (ignoredAttributes?.Contains(val.Key) == true) continue;

                var tree = val.Value as ITreeAttribute;
                if (tree != null)
                {
                    if (i == 0)
                    {
                        hashcode = val.Key.GetHashCode() ^ tree.GetHashCode(ignoredAttributes);
                    }
                    else
                    {
                        hashcode ^= val.Key.GetHashCode() ^ tree.GetHashCode(ignoredAttributes);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        hashcode = val.Key.GetHashCode() ^ val.Value.GetHashCode();
                    }
                    else
                    {
                        hashcode ^= val.Key.GetHashCode() ^ val.Value.GetHashCode();
                    }
                }

                i++;
            }

            return hashcode;
        }

        public static void BeginDirectWrite(BinaryWriter writer, string key)
        {
            writer.Write((byte)AttributeID);
            writer.Write(key);
        }

        public static void TerminateWrite(BinaryWriter writer)
        {
            writer.Write((byte)0);
        }
    }

    public static class DictionaryExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            source.TryGetValue(key, out TValue val);
            return val;
        }
    }
}
