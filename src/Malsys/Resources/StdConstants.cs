using System;
using Malsys.Compilers;

namespace Malsys.Resources {
	public static class StdConstants {

		/// <summary>
		/// True
		/// </summary>
		/// <docGroup>
		/// Boolean constants
		/// </docGroup>
		[Alias("true")]
		[CompilerConstant]
		public static readonly double True = 1.0;

		/// <summary>
		/// False
		/// </summary>
		/// <docGroup>
		/// Boolean constants
		/// </docGroup>
		[Alias("false")]
		[CompilerConstant]
		public static readonly double False = 0.0;


		/// <summary>
		/// Not a number
		/// </summary>
		/// <docGroup>
		/// Special constants
		/// </docGroup>
		[Alias("Nan", "nan")]
		[CompilerConstant]
		public static readonly double NaN = double.NaN;

		/// <summary>
		/// Not a number
		/// </summary>
		/// <docGroup>
		/// Special constants
		/// </docGroup>
		[Alias("infinity", "\u221E")]
		[CompilerConstant]
		public static readonly double Infinity = double.PositiveInfinity;

	}
}
