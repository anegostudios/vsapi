using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class TreeModifiedListener
    {
        public Action listener;
        public string path;
    }

    public class SyncedTreeAttribute : TreeAttribute
    {
        /// <summary>
        /// Whole tree will be resent
        /// </summary>
        bool allDirty;
        
        /// <summary>
        /// Subtrees will be resent
        /// </summary>
        HashSet<string> attributePathsDirty = new HashSet<string>();


        public List<TreeModifiedListener> OnModified = new List<TreeModifiedListener>();

        public void RegisterModifiedListener(string path, Action listener)
        {
            OnModified.Add(new TreeModifiedListener() { path = path, listener = listener });
        }

        public void UnregisterListener(Action listener)
        {
            // Iterate over a copy, we might have the same listener on several paths
            foreach (var val in new List<TreeModifiedListener>(OnModified))
            {
                if (val.listener == listener)
                {
                    OnModified.Remove(val);
                }
            }
        }


        /// <summary>
        /// Marks the whole attribute tree as dirty, so that it will be resent to all connected clients. Does not trigger modified listeners (because it makes no sense and breaks things)
        /// </summary>
        public void MarkAllDirty()
        {
            this.allDirty = true;
        }


        public bool AllDirty
        {
            get { return allDirty; }
        }

        public bool PartialDirty
        {
            get { return attributePathsDirty.Count > 0; }
        }

        public void MarkClean()
        {
            allDirty = false;
            attributePathsDirty.Clear();
        }


        /// <summary>
        /// Marks part of the attribute tree as dirty, allowing for a partial update of the attribute tree.
        /// Has no effect it the whole tree is already marked dirty. If more than 5 paths are marked dirty it will wipe the list of dirty paths and just marked the whole tree as dirty
        /// </summary>
        /// <param name="path"></param>
        public void MarkPathDirty(string path)
        {
            var OnModified = this.OnModified;
            for (int i = 0; i < OnModified.Count; i++)       // Iterating through the list "manually" guards against rare CMEs
            {
                TreeModifiedListener listener = OnModified[i];
                if (listener == null) continue;
                if (listener.path == null || path.StartsWithOrdinal(listener.path))
                {
                    listener.listener();
                }
            }

            if (allDirty) return;

            if (attributePathsDirty.Count >= 10)
            {
                attributePathsDirty.Clear();
                allDirty = true;
            }


            attributePathsDirty.Add(path);
        }


        #region Quick access methods




        public override void SetInt(string key, int value)
        {
            base.SetInt(key, value);
            MarkPathDirty(key);
        }

        /// <summary>
        /// Equivalent to i++ within the TreeAttribute, i.e. it increases the stored value but returns the current read value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetIntAndIncrement(string key, int defaultValue = 0)
        {
            IntAttribute attr = attributes.TryGetValue(key) as IntAttribute;
            if (attr == null) attributes[key] = attr = new IntAttribute(defaultValue);
            int value = attr.value;
            attr.SetValue(value + 1);
            MarkPathDirty(key);
            return value;
        }

        public override void SetLong(string key, long value)
        {
            base.SetLong(key, value);
            MarkPathDirty(key);
        }

        public override void SetFloat(string key, float value)
        {
            base.SetFloat(key, value);
            MarkPathDirty(key);
        }

        public override void SetBool(string key, bool value)
        {
            base.SetBool(key, value);
            MarkPathDirty(key);
        }

        public override void SetBytes(string key, byte[] value)
        {
            base.SetBytes(key, value);
            MarkPathDirty(key);
        }

        public override void SetDouble(string key, double value)
        {
            base.SetDouble(key, value);
            MarkPathDirty(key);
        }

        public override void SetString(string key, string value)
        {
            base.SetString(key, value);
            MarkPathDirty(key);
        }

        public override void SetAttribute(string key, IAttribute value)
        {
            base.SetAttribute(key, value);
            MarkPathDirty(key);
        }

        public override void RemoveAttribute(string key)
        {
            base.RemoveAttribute(key);
            MarkAllDirty();
        }

        #endregion

        public override SyncedTreeAttribute Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            ToBytes(writer);
            ms.Position = 0;

            BinaryReader reader = new BinaryReader(ms);
            SyncedTreeAttribute tree = new SyncedTreeAttribute();
            tree.FromBytes(reader);
            return tree;
        }

        public void GetDirtyPathData(out string[] paths, out byte[][] dirtydata)
        {
            FastMemoryStream ms = new FastMemoryStream();
            GetDirtyPathData(ms, out paths, out dirtydata);
        }

        public void GetDirtyPathData(FastMemoryStream ms, out string[] paths, out byte[][] dirtydata)
        {
            try
            {
            paths = attributePathsDirty.ToArray();
            }
            catch
            {
                paths = attributePathsDirty.ToArray();                  // Retry once: extremely rarely, attributePathsDirty.ToArray() can throw a CME or "Exception: Destination array is not long enough to copy all the items in the collection." due to another thread adding an entry.  In future if still a problem we can use a ConcurrentDictioanary where our set values are now the keys of the Dictionary, and the values are unimportant
            }

            dirtydata = new byte[paths.Length][];
            BinaryWriter writer = new BinaryWriter(ms);

            for (int i = 0; i < paths.Length; i++)
            {
                string path;
                if ((path = paths[i]) == null) continue;
                IAttribute attr;
                if ((attr = GetAttributeByPath(path)) == null) continue;

                ms.Reset();
                writer.Write((byte)attr.GetAttributeId());
                attr.ToBytes(writer);
                dirtydata[i] = ms.ToArray();
            }
        }


        public override void FromBytes(BinaryReader stream)
        {
            base.FromBytes(stream);

            foreach (TreeModifiedListener listener in OnModified)
            {
                listener.listener();
            }
        }

        public void PartialUpdate(string path, byte[] data)
        {
            IAttribute attr = GetAttributeByPath(path);
            
            if (data == null)
            {
                DeleteAttributeByPath(path);
                return;
            }

            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);

            int attrId = reader.ReadByte();

            if (attr == null)
            {
                Type t = AttributeIdMapping[attrId];
                attr = (IAttribute)Activator.CreateInstance(t);
                attributes[path] = attr;
            }

            attr.FromBytes(reader);

            foreach (TreeModifiedListener listener in OnModified)
            {
                if (listener.path == null || path.StartsWithOrdinal(listener.path))
                {
                    listener.listener();
                }
            }
        }

    }
}
