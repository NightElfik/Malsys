
namespace Malsys.Ast {
	public class EmptyStatement : IToken, IInputStatement, ILsystemStatement, IExprInteractiveStatement {

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

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
