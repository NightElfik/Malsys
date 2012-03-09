
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessConfigurationDefinition : IInputStatement {

		public readonly Identificator NameId;

		public readonly ImmutableListPos<IProcessConfigStatement> Statements;


		public ProcessConfigurationDefinition(Identificator name, ImmutableListPos<IProcessConfigStatement> statements, Position pos) {

			NameId = name;
			Statements = statements;

			Position = pos;
		}


		public Position Position { get; private set; }


		public InputStatementType StatementType {
			get { return InputStatementType.ProcessConfigurationDefinition; }
		}

	}
}
