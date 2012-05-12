/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys {
	public static class Swap {

		public static void Them<T>(ref T x, ref T y) where T : class {
			T t = x;
			x = y;
			y = t;
		}

	}
}
