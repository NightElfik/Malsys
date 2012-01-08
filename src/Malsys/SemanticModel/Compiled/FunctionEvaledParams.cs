using Malsys.SemanticModel.Evaluated;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionEvaledParams {

		public readonly string Name;
		public readonly ImmutableList<OptionalParameterEvaled> Parameters;
		public readonly ImmutableList<IFunctionStatement> Statements;
		public readonly Ast.FunctionDefinition AstNode;


		public FunctionEvaledParams(string name, ImmutableList<OptionalParameterEvaled> prms,
				ImmutableList<IFunctionStatement> stats, Ast.FunctionDefinition astNode) {

			Name = name;
			Parameters = prms;
			Statements = stats;
			AstNode = astNode;
		}

	}
}
