
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
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


		public void Accept(IProcessConfigVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
