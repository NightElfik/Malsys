using System;

namespace Malsys.Expressions {
	public interface IValue : IComparable<IValue> {
		bool IsConstant { get; }
		bool IsArray { get; }
		bool IsNaN { get; }
		ExpressionValueType Type { get; }
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
	}
}
