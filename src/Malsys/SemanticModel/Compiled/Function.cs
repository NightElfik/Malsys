using Malsys.Expressions;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Function : IBindable {

		public readonly ImmutableList<OptionalParameter> Parameters;

		public readonly ImmutableList<Binding> Bindings;

		public readonly IExpression ReturnExpression;

		public readonly Ast.Binding AstNode;


		public Function(ImmutableList<OptionalParameter> prms, ImmutableList<Binding> binds, IExpression expr, Ast.Binding astNode) {

			Parameters = prms;
			Bindings = binds;
			ReturnExpression = expr;
		}
	}
}
