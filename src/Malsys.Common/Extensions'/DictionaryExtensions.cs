using System.Collections.Generic;

namespace Malsys {
	public static class DictionaryExtensions {

		/// <summary>
		/// Adds given key-value pair to this dictionary and returns True if key
		/// was already in dictionary and value was updated.
		/// </summary>
		/// <remarks>
		/// This method really SHOULD be implemented by Microsoft.
		/// This implementation is far less effective than they could do (2 searches vs. 1).
		/// </remarks>
		/// <returns>
		/// True if key was already in dictionary and value was updated. False if key was added to dictionary
		/// </returns>
		public static bool AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {

			bool wasInDict = dict.ContainsKey(key);
			dict[key] = value;
			return wasInDict;

		}

		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> values) {
			foreach (var kvp in values) {
				dict.Add(kvp.Key, kvp.Value);
			}
		}

		public static bool ContainsValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {

			TValue val;

			if (dict.TryGetValue(key, out val)) {
				return value != null ? value.Equals(val) : val == null;
			}
			else {
				return false;
			}

		}

		public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, TValue defaultValue = default(TValue)) {

			TValue val;
			if (map.TryGetValue(key, out val)) {
				return val;
			}
			else {
				return defaultValue;
			}

		}


	}
}
