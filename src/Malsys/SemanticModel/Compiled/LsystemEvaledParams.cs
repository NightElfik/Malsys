
namespace Malsys.SemanticModel.Compiled {
	public class LsystemEvaledParams {

		public readonly string Name;
		public readonly ImmutableList<OptionalParameterEvaled> Parameters;
		public readonly ImmutableList<ILsystemStatement> Statements;
		public readonly Ast.LsystemDefinition AstNode;


		public LsystemEvaledParams(string name, ImmutableList<OptionalParameterEvaled> prms,
				ImmutableList<ILsystemStatement> statements, Ast.LsystemDefinition astNode) {

			Name = name;
			Parameters = prms;
			Statements = statements;
			AstNode = astNode;
		}

	}
}
