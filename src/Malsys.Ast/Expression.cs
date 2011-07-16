using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;

namespace Malsys.Ast {
	public class Expression : Token, IAstVisitable {
		public readonly ReadOnlyCollection<object> Members;

		public Expression(IList<object> members, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Members = new ReadOnlyCollection<object>(members);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public class ExpressionBuilder {
		private List<object> members = new List<object>();


		public void AddConstant(FloatConstant value) {
			members.Add(value);
		}

		public void AddVariable(Identificator id) {
			members.Add(id);
		}

		public void AddOperator(Identificator id, byte arity) {
			members.Add(new ExpressionFunction(id, arity, true, id.BeginLine, id.BeginColumn, id.EndLine, id.EndColumn));
		}

		public void AddFunction(Identificator syntax, IList<Expression> args, int beginLine, int beginColumn, int endLine, int endColumn) {
			members.Add(new ExpressionFunction(syntax, (byte)args.Count, false, beginLine, beginColumn, endLine, endColumn));
			for (int i = 0; i < args.Count; i++) {
				if (i != 0) {
					members.Add(ExpressionSpecOp.NextArg);
				}
				members.AddRange(args[i].Members);
			}
			members.Add(ExpressionSpecOp.EndFunc);
		}

		public void AddExpression(ExpressionBuilder expr) {
			members.Add(ExpressionSpecOp.OpenParen);
			members.AddRange(expr.members);
			members.Add(ExpressionSpecOp.CloseParen);
		}

		public Expression ToExpression(int beginLine, int beginColumn, int endLine, int endColumn) {
			return new Expression(members, beginLine, beginColumn, endLine, endColumn);
		}

	}

	public class ExpressionFunction : Token, IAstVisitable {
		public readonly Identificator Syntax;
		public readonly byte Arity;
		public readonly bool IsOperator;

		public ExpressionFunction(Identificator syntax, byte arity, bool isOperator, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Syntax = syntax;
			Arity = arity;
			IsOperator = isOperator;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public enum ExpressionSpecOp {
		/// <summary>
		/// Left parenthesis.
		/// </summary>
		OpenParen,
		/// <summary>
		/// Right parenthesis.
		/// </summary>
		CloseParen,
		/// <summary>
		/// Function argumetn delimiter.
		/// </summary>
		NextArg,
		/// <summary>
		/// End of function (after last arg).
		/// </summary>
		EndFunc
	}
}
