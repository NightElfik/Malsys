using System;

namespace Malsys.SemanticModel.Evaluated {

	public interface IValue : IComparable<IValue> {

		bool IsConstant { get; }

		bool IsArray { get; }

		bool IsNaN { get; }

		ExpressionValueType Type { get; }

		PositionRange AstPosition { get; }

	}

	/// <remarks>
	/// ExpressionValueType must be subset of ExpressionValueTypeFlags.
	/// </remarks>
	public enum ExpressionValueType {

		Constant = 0x1,
		Array = 0x2,

	}

	[Flags]
	public enum ExpressionValueTypeFlags {

		Unknown = 0x0,
		Constant = 0x1,
		Array = 0x2,
		Any = Constant | Array,

	}

	public static class ExpressionValueTypeExtensions {

		public static string ToTypeString(this ExpressionValueType type) {
			switch (type) {
				case ExpressionValueType.Constant: return "constant";
				case ExpressionValueType.Array: return "array";
				default: return "unknown";
			}
		}

		public static bool IsCompatibleWith(this ExpressionValueType type1, ExpressionValueType type2) {
			return type1 == type2;
		}

		public static bool IsCompatibleWith(this ExpressionValueType type1, ExpressionValueTypeFlags type2) {
			return ((int)type1 & (int)type2) != 0;
		}

		public static ExpressionValueTypeFlags ToFlags(this ExpressionValueType type) {
			switch (type) {
				case ExpressionValueType.Constant: return ExpressionValueTypeFlags.Constant;
				case ExpressionValueType.Array: return ExpressionValueTypeFlags.Array;
				default: return ExpressionValueTypeFlags.Unknown;
			}
		}

		public static string ToTypeString(this ExpressionValueTypeFlags type) {
			switch (type) {
				case ExpressionValueTypeFlags.Constant: return "constant";
				case ExpressionValueTypeFlags.Array: return "array";
				case ExpressionValueTypeFlags.Any: return "constant or array";
				default: return "unknown";
			}
		}

		public static string ToTypeStringOneWord(this ExpressionValueTypeFlags type) {
			switch (type) {
				case ExpressionValueTypeFlags.Constant: return "constant";
				case ExpressionValueTypeFlags.Array: return "array";
				case ExpressionValueTypeFlags.Any: return "constantOrArray";
				default: return "unknown";
			}
		}

	}


	public static class IValueExtensions {

		public static double ConstOrDefault(this IValue val, double defaultValue = 0) {

			if (val.IsConstant) {
				return ((Constant)val).Value;
			}
			else {
				return defaultValue;
			}
		}

		public static bool IsConstArray(this IValue val) {

			if (!val.IsArray) {
				return false;
			}

			var arr = (ValuesArray)val;

			for (int i = 0; i < arr.Length; i++) {
				if (!arr[i].IsConstant) {
					return false;
				}
			}

			return true;
		}

		public static bool IsConstArrayOfLength(this IValue val, int length) {

			if (!val.IsArray) {
				return false;
			}

			var arr = (ValuesArray)val;

			if (arr.Length != length) {
				return false;
			}

			for (int i = 0; i < arr.Length; i++) {
				if (!arr[i].IsConstant) {
					return false;
				}
			}

			return true;

		}


		public static ExpressionValueTypeFlags IValueTypeToEnum(Type t) {

			if (t.Equals(typeof(Constant))) {
				return ExpressionValueTypeFlags.Constant;
			}
			else if (t.Equals(typeof(ValuesArray))) {
				return ExpressionValueTypeFlags.Array;
			}
			else if (t.Equals(typeof(IValue))) {
				return ExpressionValueTypeFlags.Any;
			}
			else {
				return ExpressionValueTypeFlags.Unknown;
			}

		}


	}

}
