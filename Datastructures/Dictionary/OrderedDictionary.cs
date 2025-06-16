using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Datastructures
{

    /// <summary>
    /// Same as your normal C# Dictionary but ensures that the order in which the items are added is remembered. That way you can iterate over the dictionary with the insert order intact or set/get elements by index.
    /// Taken from http://www.codeproject.com/Articles/18615/OrderedDictionary-T-A-generic-implementation-of-IO
    /// Please be aware that this is not a very efficient implementation, recommed use only for small sets of data.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
	[ProtoContract]
	public class OrderedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>
	{
		private static readonly string _keyTypeName = typeof(TKey).FullName;
		private static readonly string _valueTypeName = typeof(TValue).FullName;
		private static readonly bool _valueTypeIsReferenceType = !typeof(ValueType).IsAssignableFrom(typeof(TValue));

		private Dictionary<TKey, TValue> _dictionary;
		private List<KeyValuePair<TKey, TValue>> _list;
		private IEqualityComparer<TKey> _comparer;
		private int _initialCapacity;

		[ProtoMember(1)]
		private List<KeyValuePair<TKey, TValue>> listSer;


		public Dictionary<TKey, TValue> InternalDictionary => Dictionary;

		public OrderedDictionary() : this(0, null)
		{
		}
		public OrderedDictionary(int capacity) : this(capacity, null)
		{
		}
		public OrderedDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
		{
		}

		public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if(0 > capacity)
				throw new ArgumentOutOfRangeException("capacity", "'capacity' must be non-negative");

			_initialCapacity = capacity;
			_comparer = comparer;
		}

        public OrderedDictionary(OrderedDictionary<TKey, TValue> initialData)
        {
            foreach (var val in initialData)
            {
                this[val.Key] = val.Value;
            }
        }

		[OnDeserialized]
		protected void OnDeserializedMethod(StreamingContext context)
		{
			if (listSer == null) return;

			foreach (var val in listSer)
            {
				this[val.Key] = val.Value;
            }
			listSer = null;
		}

		[ProtoBeforeSerialization]
		protected void BeforeSerialization()
        {
			listSer = new List<KeyValuePair<TKey, TValue>>();
			listSer.AddRange(_list);
        }

		public OrderedDictionary(Dictionary<TKey, TValue> initialData)
        {
			foreach (var val in initialData)
			{
				this[val.Key] = val.Value;
			}
		}

        private Dictionary<TKey, TValue> Dictionary
		{
			get
			{
				if(null == _dictionary)
				{
					_dictionary = new Dictionary<TKey, TValue>(_initialCapacity, _comparer);
				}
				return _dictionary;
			}
		}

		private List<KeyValuePair<TKey, TValue>> List
		{
			get
			{
				if (_list == null)
				{
					_list = new List<KeyValuePair<TKey, TValue>>(_initialCapacity);
				}
				return _list;
			}
		}

	

		public void Insert(int index, TKey key, TValue value)
		{
			if(index > Count || index < 0)
				throw new ArgumentOutOfRangeException("index");

			Dictionary.Add(key, value);
			List.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
		}

        public void InsertBefore(TKey Atkey, TKey key, TValue value)
        {
            for (int index = 0; index < List.Count; index++)
            {
                if (List[index].Key.Equals(Atkey))
                {
                    Insert(index, key, value);
                    return;
                }
            }
        }
        

        public void RemoveAt(int index)
		{
			if(index >= Count || index < 0)
				throw new ArgumentOutOfRangeException("index", "'index' must be non-negative and less than the size of the collection");

			TKey key = List[index].Key;

			List.RemoveAt(index);
			Dictionary.Remove(key);
		}

        public TValue GetValueAtIndex(int index)
        {
            return List[index].Value;
        }

        public TKey GetKeyAtIndex(int index)
        {
            return List[index].Key;
        }

        public void SetAtIndex(int index, TValue value)
        {
			if(index >= Count || index < 0)
				throw new ArgumentOutOfRangeException("index", "'index' must be non-negative and less than the size of the collection");

			TKey key = List[index].Key;

			List[index] = new KeyValuePair<TKey, TValue>(key, value);
			Dictionary[key] = value;
		}

        


        public int Add(TKey key, TValue value)
		{
			Dictionary.Add(key, value);
			List.Add(new KeyValuePair<TKey,TValue>(key, value));
			return Count - 1;
		}

		public void Clear()
		{
			Dictionary.Clear();
			List.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}


		/// <summary>
		/// Returns -1 if the key was not found
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int IndexOfKey(TKey key)
		{
            if (null == key)
            {
                throw new ArgumentNullException("key");
            }

			for (int index = 0; index < List.Count; index++)
			{
				KeyValuePair<TKey, TValue> entry = List[index];
				TKey next = entry.Key;
				if (null != _comparer)
				{
					if (_comparer.Equals(next, key))
					{
						return index;
					}
				}
				else if(next.Equals(key))
				{
					return index;
				}
			}

			return -1;
		}

	
		public bool Remove(TKey key)
		{
            if (null == key)
            {
                throw new ArgumentNullException("key");
            }

			int index = IndexOfKey(key);
			if (index >= 0)
			{
				if (Dictionary.Remove(key))
				{
					List.RemoveAt(index);
					return true;
				}
			}

			return false;
		}

		
		public TValue this[TKey key]
		{
			get
			{
				return Dictionary[key];
			}
			set
			{
				if (Dictionary.ContainsKey(key))
				{
					Dictionary[key] = value;    // expensive
					List[IndexOfKey(key)] = new KeyValuePair<TKey, TValue>(key, value);
				}
				else
				{
					Add(key, value);
				}
			}
		}


		void CopyTo(Array array, int index)
		{
			((ICollection)List).CopyTo(array, index);
		}

		public int Count
		{
			get
			{
				return List.Count;
			}
		}


		public ICollection<TKey> Keys
		{
			get
			{
				return Dictionary.Keys;
			}
		}

        public IEnumerable<TValue> ValuesOrdered
        {
            get
            {
                return List.Select(val => val.Value);
            }
        }

	
		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

        public TValue TryGetValue(TKey key)
        {
            Dictionary.TryGetValue(key, out TValue val);
            return val;
        }


		/// <summary>
		/// Important: these are NOT ordered.   The ordered values is obtained by .ValuesOrdered
		/// </summary>
		public ICollection<TValue> Values
		{
			get
			{
				return Dictionary.Values;
			}
		}

        public bool IsReadOnly => false;

        void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}


		bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey,TValue>>)Dictionary).Contains(item);
		}

		void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey,TValue>>)Dictionary).CopyTo(array, arrayIndex);
		}

		
		bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }


        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.Add(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int i = 0;
            foreach (var val in _dictionary)
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(val.Key, val.Value);
                i++;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool ContainsValue(TValue value)
        {
			return Dictionary.ContainsValue(value);
        }

        /// <summary>
        /// Adds values, but for performance does not replace any existing values which have the same key; that's fine for asset loading
        /// </summary>
        /// <param name="src"></param>
        /// <param name="logger"></param>
        internal void AddRange(Dictionary<TKey, TValue> src, ILogger logger)
		{
			foreach (KeyValuePair<TKey, TValue> val in src)
			{
				TKey key = val.Key;
				if (!Dictionary.ContainsKey(key))
				{
					Dictionary.Add(key, val.Value);
					List.Add(val);
				}
				//else logger.Warning("Duplicate key in shapes dictionary: " + key.ToString());
			}
		}
	}
}

