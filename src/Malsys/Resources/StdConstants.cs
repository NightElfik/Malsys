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


	}
}
