
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class EmptyStatement : IInputStatement, ILsystemStatement {

		public readonly bool Hidden;


		public EmptyStatement(Position pos) {
			Position = pos;
			Hidden = false;
		}

		public EmptyStatement(Position pos, bool hidden) {
			Position = pos;
			Hidden = hidden;
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
