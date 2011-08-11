using System.Collections.Generic;
using System;

namespace Malsys {
	public static class LinqExtensions {

		public static IEnumerable<Tuple<int, int>> GetEqualValuesIndices<T>(this IList<T> source, Func<T, T, bool> eqFunc) {

			for (int i = 0; i < source.Count; i++) {
				for (int j = 0; j < i; j++) {
					if (eqFunc(source[i], source[j])) {
						yield return new Tuple<int, int>(j, i);
					}
				}
			}
		}


		public static ImmutableList<T> ToImmutableList<T>(this IEnumerable<T> source) {
			return new ImmutableList<T>(source);
		}
	}
}
