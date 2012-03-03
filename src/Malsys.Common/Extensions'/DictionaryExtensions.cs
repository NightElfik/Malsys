using System.Collections.Generic;

namespace Malsys {
	public static class DictionaryExtensions {

		/// <summary>
		/// Adds given key-value pair to this dictionary and returns True if key
		/// was already in dictionary and value was updated.
		/// </summary>
		/// <remarks>
		/// This method really SHOULD be implemented by M$soft.
		/// This implementation is far less effective than they could do (2 searches vs. 1).
		/// </remarks>
		/// <returns>
		/// True if key was already in dictionary and value was updated. False if key was added to dictionary
		/// </returns>
		public static bool AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value){

			bool wasInDict = dict.ContainsKey(key);
			dict[key] = value;
			return wasInDict;

		}


	}
}
