
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessConfigurationDefinition : IInputStatement {

		public readonly Identificator NameId;

		public readonly ImmutableListPos<IProcessConfigStatement> Statements;


		public ProcessConfigurationDefinition(Identificator name, ImmutableListPos<IProcessConfigStatement> statements, Position pos) {

			NameId = name;
			Statements = statements;

			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
