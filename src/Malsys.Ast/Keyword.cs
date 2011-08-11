using System.Linq;
using System.Collections.Generic;

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

	public static class KeywordExtensions {
		public static ImmutableList<Keyword> WithoutEmpty(this IEnumerable<Keyword> keywords) {
			return new ImmutableList<Keyword>(keywords.Where(k => !k.IsEmpty));
		}
	}
}
