
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessContainer : IProcessConfigStatement {

		public readonly Identificator NameId;
		public readonly Identificator TypeNameId;
		public readonly Identificator DefaultTypeNameId;


		public ProcessContainer(Identificator name, Identificator typeName, Identificator defaultTypeName, Position pos) {

			NameId = name;
			TypeNameId = typeName;
			DefaultTypeNameId = defaultTypeName;

			Position = pos;
		}


		public Position Position { get; private set; }


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessContainer; }
		}

	}
}
