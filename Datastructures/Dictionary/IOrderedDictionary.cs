using System.Collections.Generic;
using System.Collections.Specialized;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Represents a generic collection of key/value pairs that are ordered independently of the key and value.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    public interface IOrderedDictionary<TKey, TValue> : IOrderedDictionary, IDictionary<TKey, TValue>
	{
		
		new int Add(TKey key, TValue value);

		
		void Insert(int index, TKey key, TValue value);

        TValue GetValueAtIndex(int index);

        void SetAtIndex(int index, TValue value);
		
	}
}
