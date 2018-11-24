using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Alessio.Base.Models
{
	public class AutoDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		#region Fields
		private Dictionary<TKey, TValue> _dictionary;
		private Func<TValue, TKey> _extractor;
		private bool _addIfAccessedEmpty;
		#endregion

		#region Constructors
		public AutoDictionary(Func<TValue, TKey> extractor, bool addIfAccessedEmpty = false)
		{
			_dictionary = new Dictionary<TKey, TValue>();
			_extractor = extractor;
			_addIfAccessedEmpty = addIfAccessedEmpty;
		}
		#endregion

		#region Methods
		public TValue Add(TValue value)
		{
			_dictionary.Add(_extractor(value), value);
			return value;
		}

		public void RemoveKey(TKey key)
		{
			_dictionary.Remove(key);
		}

		public void Remove(TValue value)
		{
			RemoveKey(_extractor(value));
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool Contains(TValue value)
		{
			return ContainsKey(_extractor(value));
		}
		#endregion

		#region Operators
		public TValue this[TKey key]
		{
			get
			{
				if (!_dictionary.ContainsKey(key))
				{
					if (_addIfAccessedEmpty) _dictionary.Add(key, ReflectUtilities.NewOrDefault<TValue>());
					else throw new KeyNotFoundException($"The given key was not found in the dictionary: {key}");
				}
				return _dictionary[key];
			}
			set
			{
				if (!_dictionary.ContainsKey(key))
				{
					if (_addIfAccessedEmpty) _dictionary.Add(key, value);
					else throw new KeyNotFoundException($"The given key was not found in the dictionary: {key}");
				}
				else _dictionary[key] = value;
			}
		}
		#endregion

		#region IEnumerable
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
