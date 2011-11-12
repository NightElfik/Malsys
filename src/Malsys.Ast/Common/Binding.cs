
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Binding : IInputStatement, ILsystemStatement {

		public readonly Identificator NameId;
		public readonly IBindable Value;


		public Binding(Identificator name, IBindable value, Position pos) {

			NameId = name;
			Value = value;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstInputVisitable Members

		public void Accept(IAstInputVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion

		#region IAstLsystemVisitable Members

		public void Accept(IAstLsystemVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
