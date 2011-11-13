using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionEvaled {

		public readonly ImmutableList<OptionalParameterEvaled> Parameters;

		public readonly ImmutableList<Binding> Bindings;

		public readonly IExpression ReturnExpression;

		public readonly Ast.Binding AstNode;


		public FunctionEvaled(ImmutableList<OptionalParameterEvaled> prms, ImmutableList<Binding> binds, IExpression expr) {

			Parameters = prms;
			Bindings = binds;
			ReturnExpression = expr;
		}

		public string BindedName { get { return AstNode != null ? AstNode.NameId.Name : null; } }
	}
}
