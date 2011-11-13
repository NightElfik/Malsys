using Malsys.Expressions;
using Malsys.SemanticModel;

namespace Malsys {
	public static class IntExtensions {
		public static Constant ToConst(this int i) {
			return new Constant(i);
		}
	}
}
