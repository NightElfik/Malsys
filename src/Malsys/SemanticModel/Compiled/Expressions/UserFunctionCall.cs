
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UserFunctionCall : IExpression {


		public readonly string Name;

		public readonly ImmutableList<IExpression> Arguments;

		public readonly Ast.ExpressionFunction AstNode;


		public UserFunctionCall(string name, ImmutableList<IExpression> args, Ast.ExpressionFunction astNode) {

			Name = name;
			Arguments = args;

			AstNode = astNode;
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
