using System.Linq;
using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class KeywordPos : IToken {

		public static readonly KeywordPos Unknown = new KeywordPos(Keyword.Unknown, Position.Unknown);

		public static ImmutableList<KeywordPos> CreateListKnownOnly(params KeywordPos[] keywords) {
			return new ImmutableList<KeywordPos>(keywords.Where(k => !k.IsEmpty));
		}


		public readonly Keyword Keyword;

		public KeywordPos(Keyword kw, Position pos) {
			Keyword = kw;
			Position = pos;
		}


		public override string ToString() {
			return Keyword.ToString();
		}


		public string ToKeyword() {
			return EnumHelper.GetStringVal(Keyword);
		}

		public bool IsEmpty { get { return Position.IsUnknown; } }


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
