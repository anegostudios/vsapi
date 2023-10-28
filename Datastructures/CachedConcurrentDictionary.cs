using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Exactly like ConcurrentDictionary except that Values is cached for performance, instead of building a new List each time Values is accessed
    /// <br/><br/>The cache is only guaranteed up to date if CachedConcurrentDictionary.TryAdd, .TryRemove or [] methods are used to modify the Dictionary
    /// otherwise the cache will be updated next time when one of those methods is used
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CachingConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        ICollection<TValue> valuesCached;

        public new ICollection<TValue> Values
        {
            get { var vc = valuesCached;  return vc ?? (valuesCached = base.Values); }
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            bool result = base.TryAdd(key, value);
            if (result) valuesCached = null;
            return result;
        }

        public new bool TryRemove(TKey key, out TValue value)
        {
            bool result = base.TryRemove(key, out value);
            if (result) valuesCached = null;
            return result;
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException();
                }
                return value;
            }
            set
            {
                if (key == null) throw new ArgumentNullException("key");
                TryAdd(key, value);
            }
        }
    }
}
