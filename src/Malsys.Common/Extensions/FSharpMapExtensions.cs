using System;
using System.Collections.Generic;
using System.Text;
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

		public static FSharpMap<TKey, TValue> AddRange<TKey, TValue>(this FSharpMap<TKey, TValue> map, FSharpMap<TKey, TValue> anotherMap) {

			foreach (var keyValue in anotherMap) {
				map = map.Add(keyValue.Key, keyValue.Value);
			}

			return map;
		}

	}
}
