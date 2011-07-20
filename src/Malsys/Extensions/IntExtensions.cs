using Malsys.Expressions;

namespace Malsys {
	public static class IntExtensions {
		public static Constant ToConst(this int i) {
			return new Constant(i);
		}
	}
}
