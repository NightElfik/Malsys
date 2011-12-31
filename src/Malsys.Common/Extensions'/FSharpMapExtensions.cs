using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Malsys {
	public static class FSharpMapExtensions {

		public static FSharpMap<TKey, TValue> Add<TKey, TValue>(this FSharpMap<TKey, TValue> map, TKey key, TValue value, out FSharpOption<TValue> oldValue) {

			oldValue = map.TryFind(key);
			return map.Add(key, value);
		}

		public static FSharpMap<TKey, TValue> Add<TKey, TValue>(this FSharpMap<TKey, TValue> map, TKey key, TValue value, out bool oldValueWasSet) {

			oldValueWasSet = OptionModule.IsSome(map.TryFind(key));
			return map.Add(key, value);
		}

		public static FSharpMap<TKey, TValue> AddRange<TKey, TValue>(this FSharpMap<TKey, TValue> map, IEnumerable<KeyValuePair<TKey, TValue>> values) {

			foreach (var keyValue in values) {
				map = map.Add(keyValue.Key, keyValue.Value);
			}

			return map;
		}

		public static FSharpMap<TKey, TValue> AddRange<TKey, TValue>(this FSharpMap<TKey, TValue> map, FSharpMap<TKey, TValue> values) {

			if (map.Count == 0) {
				return values;
			}
			if (values.Count == 0) {
				return map;
			}

			foreach (var keyValue in values) {
				map = map.Add(keyValue.Key, keyValue.Value);
			}

			return map;
		}

	}
}
