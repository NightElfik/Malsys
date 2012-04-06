
namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class BaseLsystem {

		public readonly string Name;
		public readonly ImmutableList<IExpression> Arguments;

		public readonly Ast.BaseLsystem AstNode;


		public BaseLsystem(string name, ImmutableList<IExpression> args, Ast.BaseLsystem astNode = null) {
			Name = name;
			Arguments = args;

			AstNode = astNode;
		}

	}
}
