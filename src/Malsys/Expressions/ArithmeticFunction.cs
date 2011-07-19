using System;

namespace Malsys.Expressions {
	public class ArithmeticFunction {
		public string Syntax { get; set; }
		public byte Arity { get; set; }

		public PostfixExpression[] Arguments { get; set; }

		public ArithmeticFunction(string syntax, byte arity, PostfixExpression[] arguments) {
			Syntax = syntax;
			Arity = arity;
			Arguments = arguments;
		}

		public override string ToString() {
			return Syntax + "'" + Arity;
		}
	}
}
