using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Expressions {
	public interface IValue : IComparable<IValue> {
		bool IsConstant { get; }
		bool IsArray { get; }
		IArithmeticValueType Type { get; }
	}

	public enum IArithmeticValueType {
		Constant,
		Array,
	}
}
