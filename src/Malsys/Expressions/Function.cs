using System;

namespace Malsys.Expressions {
	public class Function : IPostfixExpressionMember {
		public string Syntax { get; set; }
		public byte Arity { get; set; }

		public PostfixExpression[] Arguments { get; set; }

		public Function(string syntax, byte arity, PostfixExpression[] arguments) {
			Syntax = syntax;
			Arity = arity;
			Arguments = arguments;
		}

		public override string ToString() {
			return Syntax + "'" + Arity;
		}

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsEvaluable { get { return false; } }

		#endregion
	}
}
