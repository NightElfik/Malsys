
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Keyword : IToken {

		public Keyword(Position pos) {
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
