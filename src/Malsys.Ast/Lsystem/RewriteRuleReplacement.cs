
namespace Malsys.Ast {
	public class RewriteRuleReplacement : IToken {

		public readonly ImmutableList<Keyword> Keywords;

		public readonly SymbolsListPos<Expression> Replacement;
		public readonly Expression Weight;


		public RewriteRuleReplacement(SymbolsListPos<Expression> replac, Expression wei, ImmutableList<Keyword> keywords, Position pos) {

			Replacement = replac;
			Weight = wei;
			Keywords = keywords.WithoutEmpty();

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
