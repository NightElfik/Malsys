using System;

namespace Malsys.SemanticModel.Evaluated {

	public interface IValue : IComparable<IValue> {

		bool IsConstant { get; }

		bool IsArray { get; }

		bool IsNaN { get; }

		ExpressionValueType Type { get; }

		Position AstPosition { get; }

	}

	[Flags]
	public enum ExpressionValueType {
		Constant = 0x1,
		Array = 0x2,
		Any = Constant | Array,
	}

	public static class ExpressionValueTypeExtensions {
		public static string ToTypeString(this ExpressionValueType type) {
			switch (type) {
				case ExpressionValueType.Constant: return "value";
				case ExpressionValueType.Array: return "array";
				case ExpressionValueType.Any: return "value or array";
				default: return "unknown";
			}
		}

		public static string ToTypeStringOneWord(this ExpressionValueType type) {
			switch (type) {
				case ExpressionValueType.Constant: return "value";
				case ExpressionValueType.Array: return "array";
				case ExpressionValueType.Any: return "valueOrArray";
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

	}

}
