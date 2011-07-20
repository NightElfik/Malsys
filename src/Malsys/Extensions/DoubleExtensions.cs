using Malsys.Expressions;

namespace Malsys {
	public static class DoubleExtensions {
		public static Constant ToConst(this double d) {
			return new Constant(d);
		}
	}
}
