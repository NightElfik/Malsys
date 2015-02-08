
namespace Malsys.SemanticModel.Evaluated {
	public class OptionalParameterEvaled {

		public string Name;
		public IValue DefaultValue;

		public readonly Ast.OptionalParameter AstNode;


		public OptionalParameterEvaled(Ast.OptionalParameter astNode) {
			AstNode = astNode;
		}

		public bool IsOptional { get { return DefaultValue != null; } }


	}
}
