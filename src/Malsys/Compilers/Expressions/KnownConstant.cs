using System;

namespace Malsys.Compilers.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class KnownConstant {

		#region Known constants definitions

		public static readonly KnownConstant True = new KnownConstant("true", 1.0);
		public static readonly KnownConstant False = new KnownConstant("false", 0.0);

		public static readonly KnownConstant Nan = new KnownConstant("nan", double.NaN);

		public static readonly KnownConstant Infinity = new KnownConstant("infinity", double.PositiveInfinity);
		public static readonly KnownConstant Infty = Infinity.ChangeNameTo("infty");
		public static readonly KnownConstant InfinityUnicode = Infinity.ChangeNameTo(CharHelper.Infinity.ToString());

		public static readonly KnownConstant Pi = new KnownConstant("pi", Math.PI);
		public static readonly KnownConstant PiUnicode = Pi.ChangeNameTo(CharHelper.Pi.ToString());

		public static readonly KnownConstant Tau = new KnownConstant("tau", 2 * Math.PI);
		public static readonly KnownConstant TauUnicode = Tau.ChangeNameTo(CharHelper.Tau.ToString());

		public static readonly KnownConstant E = new KnownConstant("e", Math.E);

		public static readonly KnownConstant GoldenRatio = new KnownConstant("golden_ratio", (1 + Math.Sqrt(5)) / 2);
		public static readonly KnownConstant GoldenRatioCamel = GoldenRatio.ChangeNameTo("goldenRatio");
		public static readonly KnownConstant GoldenRatioPhi = GoldenRatio.ChangeNameTo(CharHelper.Phi.ToString());
		public static readonly KnownConstant GoldenRatioVarPhi = GoldenRatio.ChangeNameTo(CharHelper.VarPhi.ToString());

		#endregion


		public readonly string Name;
		public readonly double Value;


		public KnownConstant(string name, double value) {
			Name = name;
			Value = value;
		}


		public KnownConstant ChangeNameTo(string newName) {
			return new KnownConstant(newName, Value);
		}
	}
}
