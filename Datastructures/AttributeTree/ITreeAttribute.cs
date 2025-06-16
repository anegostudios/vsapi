using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Represents a List of nestable Attributes
    /// </summary>
    public interface ITreeAttribute : IAttribute, IEnumerable<KeyValuePair<string, IAttribute>>
    {
        /// <summary>
        /// Will return null if given attribute does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IAttribute this[string key] { get; set; }

        /// <summary>
        /// Amount of elements in this Tree attribute
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns all values inside this tree attributes
        /// </summary>
        IAttribute[] Values { get; }

        /// <summary>
        /// True if this attribute exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool HasAttribute(string key);

        /// <summary>
        /// Similar to TryGetValue for a Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetAttribute(string key, out IAttribute value);


        /// <summary>
        /// Removes an attribute
        /// </summary>
        /// <param name="key"></param>
        void RemoveAttribute(string key);

        /// <summary>
        /// Creates a bool attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetBool(string key, bool value);

        /// <summary>
        /// Creates an int attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetInt(string key, int value);

        /// <summary>
        /// Creates a long attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetLong(string key, long value);

        /// <summary>
        /// Creates a double attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetDouble(string key, double value);
        

        /// <summary>
        /// Creates a float attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetFloat(string key, float value);

        /// <summary>
        /// Creates a string attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetString(string key, string value);

        /// <summary>
        /// Creates a byte[] attribute with given key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetBytes(string key, byte[] value);

        /// <summary>
        /// Sets given item stack with given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemstack"></param>
        void SetItemstack(string key, ItemStack itemstack);

        /// <summary>
        /// Retrieves a bool or null if the key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool? TryGetBool(string key);

        /// <summary>
        /// Retrieves a bool or default value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>
        /// Retrieves an int or null if the key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int? TryGetInt(string key);

        /// <summary>
        /// Retrieves an int or default value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>
        /// Same as (int)<see cref="GetDecimal(string, double)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        int GetAsInt(string key, int defaultValue = 0);

        /// <summary>
        /// Returns true/false, for whatever type of attribute is found for given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool GetAsBool(string key, bool defaultValue = false);

        /// <summary>
        /// Retrieves an int, float, long or double value. Whatever attribute is found for given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        double GetDecimal(string key, double defaultValue = 0);

        /// <summary>
        /// Retrieves the value of given attribute, independent of attribute type
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string GetAsString(string key, string defaultValue = null);

        /// <summary>
        /// Retrieves a long or null value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long? TryGetLong(string key);

        /// <summary>
        /// Retrieves a long or default value if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        long GetLong(string key, long defaultValue = 0);

        /// <summary>
        /// Retrieves a float or null if the key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        float? TryGetFloat(string key);

        /// <summary>
        /// Retrieves a float or defaultvalue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        float GetFloat(string key, float defaultValue = 0);

        /// <summary>
        /// Retrieves a double or null if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        double? TryGetDouble(string key);

        /// <summary>
        /// Retrieves a double or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        double GetDouble(string key, double defaultValue = 0);

        /// <summary>
        /// Retrieves a string or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string GetString(string key, string defaultValue = null);

        /// <summary>
        /// Retrieves a byte array or defaultValue if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        byte[] GetBytes(string key, byte[] defaultValue = null);

        /// <summary>
        /// Retrieves an itemstack or defaultValue if key is not found. Be sure to call stack.ResolveBlockOrItem() after retrieving it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        ItemStack GetItemstack(string key, ItemStack defaultValue = null);
        
        /// <summary>
        /// Retrieves an attribute tree or null if key is not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ITreeAttribute GetTreeAttribute(string key);
        
        /// <summary>
        /// Retrieves an attribute tree or adds it if key is not found.
        /// Throws an exception if the key does exist but is not a tree.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ITreeAttribute GetOrAddTreeAttribute(string key);

        /// <summary>
        /// Creates a deep copy of the attribute tree
        /// </summary>
        /// <returns></returns>
        new ITreeAttribute Clone();

        /// <summary>
        /// Merges trees (it will overwrite existing values)
        /// </summary>
        /// <param name="tree"></param>
        void MergeTree(ITreeAttribute tree);

        /// <summary>
        /// Returns a ITreeAttribute sorted alphabetically by key. Does not modify the existing ITreeAttribute
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        OrderedDictionary<string, IAttribute> SortedCopy(bool recursive = false);

        bool Equals(IWorldAccessor worldForResolve, IAttribute attr, params string[] ignoreSubTrees);
        bool IsSubSetOf(IWorldAccessor worldForResolve, IAttribute other);

        int GetHashCode(string[] ignoredAttributes);
    }
}
