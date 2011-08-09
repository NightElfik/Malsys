using System.Collections.Generic;
using System;

namespace Malsys {
	public static class LinqExtensions {

		public static bool TryGetEqualValuesIndices<T>(this IList<T> source, out int nonUniqueIndex1, out int nonUniqueIndex2) where T : IEquatable<T> {
			for (int i = 0; i < source.Count; i++) {
				for (int j = 0; j < i; j++) {
					if (source[i].Equals(source[j])) {
						nonUniqueIndex1 = j;
						nonUniqueIndex2 = i;
						return true;
					}
				}
			}

			nonUniqueIndex1 = -1;
			nonUniqueIndex2 = -1;
			return false;
		}


		public static ImmutableList<T> ToImmutableList<T>(this IEnumerable<T> source) {
			return new ImmutableList<T>(source);
		}
	}
}
