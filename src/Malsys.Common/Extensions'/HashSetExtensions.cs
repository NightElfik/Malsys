﻿using System.Collections.Generic;

namespace Malsys {
	public static class HashSetExtensions {


		public static void AddRange<T>(this HashSet<T> hs, IEnumerable<T> items) {

			foreach (var item in items) {
				hs.Add(item);
			}

		}


	}
}
