using System;
using Malsys.Compilers.Expressions;

namespace Malsys.Resources {
	public static class StdConstants {


		private const string boolGroup = "Boolean constants";

		public static readonly KnownConstant True = new KnownConstant("True", boolGroup, "true", 1.0);

		public static readonly KnownConstant False = new KnownConstant("False", boolGroup, "false", 0.0);



		private const string specGroup = "Special constants";

		public static readonly KnownConstant Nan = new KnownConstant("not a number", specGroup, "nan", double.NaN);

		public static readonly KnownConstant Infinity
			= new KnownConstant("Infinity", specGroup, "infinity", double.PositiveInfinity);
		public static readonly KnownConstant Infty = Infinity.ChangeNameTo("infty");
		public static readonly KnownConstant InfinityUnicode = Infinity.ChangeNameTo(CharHelper.Infinity.ToString());



		private const string wellKnownGroup = "Well known constants";

		public static readonly KnownConstant Pi = new KnownConstant("Pi", wellKnownGroup, "pi", Math.PI);
		public static readonly KnownConstant PiUnicode = Pi.ChangeNameTo(CharHelper.Pi.ToString());

		public static readonly KnownConstant Tau = new KnownConstant("Tau (2 pi)", wellKnownGroup, "tau", 2 * Math.PI);
		public static readonly KnownConstant TauUnicode = Tau.ChangeNameTo(CharHelper.Tau.ToString());

		public static readonly KnownConstant E = new KnownConstant("Euler's number", wellKnownGroup, "e", Math.E);

		public static readonly KnownConstant GoldenRatio
			= new KnownConstant("Golden ratio", wellKnownGroup, "golden_ratio", (1 + Math.Sqrt(5)) / 2);
		public static readonly KnownConstant GoldenRatioCamel = GoldenRatio.ChangeNameTo("goldenRatio");


	}
}
