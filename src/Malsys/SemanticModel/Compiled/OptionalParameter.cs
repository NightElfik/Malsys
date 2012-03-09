using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class OptionalParameter {

		public readonly string Name;
		public readonly IExpression DefaultValue;

		public readonly Ast.OptionalParameter AstNode;


		public OptionalParameter(string name, Ast.OptionalParameter astNode = null) {

			Name = name;
			DefaultValue = EmptyExpression.Instance;

			AstNode = astNode;
		}

		public OptionalParameter(string name, IExpression defaultValue, Ast.OptionalParameter astNode = null) {

			Name = name;
			DefaultValue = defaultValue;

			AstNode = astNode;
		}


		public bool IsOptional { get { return DefaultValue.ExpressionType != ExpressionType.EmptyExpression; } }

	}
}
