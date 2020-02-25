using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            foreach (var val in OnModified)
            {
                if (val.listener == listener)
                {
                    OnModified.Remove(val);
                    return;
                }
            }
        }


        /// <summary>
        /// Marks the whole attribute tree as dirty, so that it will be resent to all connected clients. Does not trigger modified listeners (because it makes no sense and breaks things)
        /// </summary>
        public void MarkAllDirty()
        {
            this.allDirty = true;

            /*foreach (TreeModifiedListener listener in OnModified)
            {
                listener.listener();
            }*/
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
            foreach (TreeModifiedListener listener in OnModified)
            {
                if (listener.path == null || path.StartsWith(listener.path))
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

        public IAttribute GetAttributeByPath(string path)
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
                } else
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



        public override void SetInt(string key, int value)
        {
            base.SetInt(key, value);
            MarkPathDirty(key);
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

        internal new SyncedTreeAttribute Clone()
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

        public string[] DirtyPaths()
        {
            return attributePathsDirty.ToArray();
        }

        public byte[][] DirtyData()
        {
            byte[][] dirtydata = new byte[attributePathsDirty.Count][];

            int i = 0;
            foreach (string path in attributePathsDirty)
            {
                IAttribute attr = GetAttributeByPath(path);
                if (attr == null) continue;

                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);

                writer.Write((byte)attr.GetAttributeId());
                attr.ToBytes(writer);

                dirtydata[i++] = ms.ToArray();
            }

            return dirtydata;
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
                //SetAttribute(path, attr); - cant use this, causes a mark dirty and triggers modified listeners too early
            }

            attr.FromBytes(reader);

            foreach (TreeModifiedListener listener in OnModified)
            {
                if (listener.path == null || path.StartsWith(listener.path))
                {
                    listener.listener();
                }
            }
        }

    }
}
