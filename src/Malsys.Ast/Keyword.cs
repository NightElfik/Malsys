
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Keyword : IToken, IAstVisitable {

		public Keyword(Position pos) {
			Position = pos;
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
