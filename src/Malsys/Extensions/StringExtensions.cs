using Malsys.Expressions;

namespace Malsys {
	public static class StringExtensions {
		public static Variable ToVar(this string name) {
			return new Variable(name);
		}
	}
}
