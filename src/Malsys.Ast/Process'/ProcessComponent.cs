
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ProcessComponent : IProcessConfigStatement {

		public readonly Identificator NameId;
		public readonly Identificator TypeNameId;


		public ProcessComponent(Identificator name, Identificator typeName, Position pos) {

			NameId = name;
			TypeNameId = typeName;

			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IProcessConfigVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
