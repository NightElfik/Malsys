using System.Collections.Generic;

namespace Malsys {
	public static class LinqExtensions {

		public static bool TryGetEqualValuesIndices<TSource>(this IList<TSource> source, out int nonUniqueIndex1, out int nonUniqueIndex2) {
			for (int i = 0; i < source.Count; i++) {
				for (int j = 0; j < i; j++) {
					if (source[i].Equals(source[j])) {
						nonUniqueIndex1 = i;
						nonUniqueIndex2 = j;
						return false;
					}
				}
			}

			nonUniqueIndex1 = -1;
			nonUniqueIndex2 = -1;
			return true;
		}

	}
}
