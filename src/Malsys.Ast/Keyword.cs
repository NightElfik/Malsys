
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Keyword : IToken {

		public static readonly Keyword Empty = new Keyword(Position.Unknown);


		public Keyword(Position pos) {
			Position = pos;
		}


		public bool IsEmpty { get { return Position.IsUnknown; } }


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
