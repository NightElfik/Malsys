
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class EmptyStatement : IStatement, IInputStatement, ILsystemStatement, IProcessConfigStatement {


		public EmptyStatement(Position pos) {
			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

		public void Accept(IProcessConfigVisitor visitor) {
			visitor.Visit(this);
		}
	}
}
