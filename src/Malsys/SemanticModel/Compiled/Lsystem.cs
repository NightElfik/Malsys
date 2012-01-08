
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IInputStatement {

		public readonly string Name;
		public readonly ImmutableList<OptionalParameter> Parameters;
		public readonly ImmutableList<ILsystemStatement> Statements;

		public readonly Ast.LsystemDefinition AstNode;


		public Lsystem(string name, ImmutableList<OptionalParameter> prms, ImmutableList<ILsystemStatement> statements,
				Ast.LsystemDefinition astNode) {

			Name = name;
			Parameters = prms;
			Statements = statements;

			AstNode = astNode;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.Lsystem; }
		}

	}
}
