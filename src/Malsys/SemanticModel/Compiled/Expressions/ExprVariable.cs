// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ExprVariable : IExpression {

		public readonly string Name;

		public readonly Ast.Identifier AstNode;


		public ExprVariable(string name, Ast.Identifier astNode) {
			Name = name;
			AstNode = astNode;
		}


		public override string ToString() {
			return Name;
		}


		public ExpressionType ExpressionType { get { return ExpressionType.ExprVariable; } }

	}


	public static class ExprVariableExtensions {

		public static ExprVariable ToVar(this string name) {
			return new ExprVariable(name, null);
		}

	}
}
