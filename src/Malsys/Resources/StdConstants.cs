
namespace Malsys.Resources {
	[MalsysConstants]
	public static class StdConstants {

		/// <summary>
		/// True.
		/// </summary>
		/// <group>
		/// Boolean constants
		/// </group>
		[AccessName("true")]
		public static readonly double True = 1.0;

		/// <summary>
		/// False.
		/// </summary>
		/// <group>
		/// Boolean constants
		/// </group>
		[AccessName("false")]
		public static readonly double False = 0.0;


		/// <summary>
		/// Not a number.
		/// </summary>
		/// <group>
		/// Special constants
		/// </group>
		[AccessName("NaN", "nan")]
		public static readonly double NaN = double.NaN;

		/// <summary>
		/// Positive infinity.
		/// </summary>
		/// <group>
		/// Special constants
		/// </group>
		[AccessName("infinity", "\u221E")]
		public static readonly double Infinity = double.PositiveInfinity;

	}
}
