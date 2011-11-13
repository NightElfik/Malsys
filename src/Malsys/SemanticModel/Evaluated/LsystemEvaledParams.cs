using Malsys.SemanticModel.Compiled;

namespace Malsys.SemanticModel.Evaluated {
	public class LsystemEvaledParams {

		public readonly ImmutableList<OptionalParameterEvaled> Parameters;

		public readonly ImmutableList<ILsystemStatement> Statements;

		public readonly Ast.Binding AstNode;


		public LsystemEvaledParams(ImmutableList<OptionalParameterEvaled> prms, ImmutableList<ILsystemStatement> statements, Ast.Binding astNode) {

			Parameters = prms;
			Statements = statements;
			AstNode = astNode;
		}

		public string BindedName { get { return AstNode != null ? AstNode.NameId.Name : null; } }
	}
}
