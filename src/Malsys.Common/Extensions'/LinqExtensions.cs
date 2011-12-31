using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
