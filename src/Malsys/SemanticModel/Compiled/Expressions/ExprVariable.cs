
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExprVariable : IExpression {

		public readonly string Name;

		public readonly Ast.Identificator AstNode;


		public ExprVariable(string name, Ast.Identificator astNode) {
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
