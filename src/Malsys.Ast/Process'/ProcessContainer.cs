
namespace Malsys.Ast {
	public class ProcessContainer : IProcessConfigStatement {

		public Identifier NameId;
		public Identifier TypeNameId;
		public Identifier DefaultTypeNameId;

		public PositionRange Position { get; private set; }


		public ProcessContainer(Identifier name, Identifier typeName, Identifier defaultTypeName, PositionRange pos) {

			NameId = name;
			TypeNameId = typeName;
			DefaultTypeNameId = defaultTypeName;

			Position = pos;
		}


		public ProcessConfigStatementType StatementType {
			get { return ProcessConfigStatementType.ProcessContainer; }
		}

	}
}
