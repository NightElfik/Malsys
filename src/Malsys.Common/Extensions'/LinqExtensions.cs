using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.FSharp.Collections;

namespace Malsys {
	public static class LinqExtensions {

		/// <summary>
		/// Finds all indices of pairs with equal value.
		/// </summary>
		public static IEnumerable<Tuple<int, int>> GetEqualValuesIndices<T>(this IList<T> source, Func<T, T, bool> eqFunc) {

			for (int i = 0; i < source.Count; i++) {
				for (int j = 0; j < i; j++) {
					if (eqFunc(source[i], source[j])) {
						yield return new Tuple<int, int>(j, i);
					}
				}
			}
		}


		public static string JoinToString<T>(this IEnumerable<T> source, string separator = "") {

			bool first = true;
			StringBuilder sb = new StringBuilder();

			foreach (var item in source) {
				if (first) {
					first = false;
				}
				else {
					sb.Append(separator);
				}
				sb.Append(item.ToString());
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts this enumerable to dictionary according to specified key selector and element selector functions.
		/// Values with the same key are overwritten.
		/// </summary>
		public static Dictionary<TKey, TElement> ToDictionaryOverwrite<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) {

			var dict = new Dictionary<TKey, TElement>();

			foreach (var item in source) {
				dict[keySelector(item)] = elementSelector(item);
			}

			return dict;

		}

		/// <summary>
		/// Converts this enumerable to F# map according to specified key selector and element selector functions.
		/// </summary>
		public static FSharpMap<TKey, TElement> ToFsharpMap<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) {

			return source.Aggregate(MapModule.Empty<TKey, TElement>(), (acc, x) => acc.Add(keySelector(x), elementSelector(x)));

		}


		public static T RandomOrDefault<T>(this IQueryable<T> source) {

			int count = source.Count();

			if (count == 0) {
				return default(T);
			}

			int index = new Random().Next(count);

			return source.Skip(index).First();
		}

	}
}
