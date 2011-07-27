using Malsys.Expressions;

namespace Malsys {
	public static class StringExtensions {
		public static ExprVariable ToVar(this string name) {
			return new ExprVariable(name);
		}
	}
}
