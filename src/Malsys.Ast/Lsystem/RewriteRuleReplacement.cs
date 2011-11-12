
namespace Malsys.Ast {
	public class RewriteRuleReplacement : IToken {

		public readonly ImmutableListPos<LsystemSymbol> Replacement;
		public readonly Expression Weight;


		public RewriteRuleReplacement(ImmutableListPos<LsystemSymbol> replac, Expression wei, Position pos) {

			Replacement = replac;
			Weight = wei;

			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

	}
}
