
namespace Malsys.Ast {
	public class EmptyStatement : IToken, IInputStatement, ILsystemStatement, IExprInteractiveStatement {

		public EmptyStatement(Position pos) {
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
