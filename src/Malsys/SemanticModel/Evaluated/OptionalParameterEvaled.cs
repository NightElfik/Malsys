
namespace Malsys.SemanticModel.Evaluated {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameterEvaled {

		public readonly string Name;
		public readonly IValue DefaultValue;

		public readonly Ast.OptionalParameter AstNode;


		public OptionalParameterEvaled(string name, Ast.OptionalParameter astNode = null) {

			Name = name;
			DefaultValue = null;

			AstNode = astNode;

		}

		public OptionalParameterEvaled(string name, IValue defaultValue, Ast.OptionalParameter astNode = null) {

			Name = name;
			DefaultValue = defaultValue;

			AstNode = astNode;
		}


		public bool IsOptional { get { return DefaultValue != null; } }


	}
}
