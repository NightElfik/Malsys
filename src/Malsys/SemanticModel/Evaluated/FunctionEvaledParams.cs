using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionEvaledParams {

		public readonly ImmutableList<OptionalParameterEvaled> Parameters;

		public readonly ImmutableList<Binding> Bindings;

		public readonly IExpression ReturnExpression;

		public readonly Ast.Binding AstNode;


		public FunctionEvaledParams(ImmutableList<OptionalParameterEvaled> prms, ImmutableList<Binding> binds, IExpression expr, Ast.Binding astNode) {

			Parameters = prms;
			Bindings = binds;
			ReturnExpression = expr;
			AstNode = astNode;
		}

		public string BindedName { get { return AstNode != null ? AstNode.NameId.Name : null; } }
	}
}
