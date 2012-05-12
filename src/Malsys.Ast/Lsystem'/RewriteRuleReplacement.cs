/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class RewriteRuleReplacement : IToken {

		public readonly ImmutableListPos<LsystemSymbol> Replacement;
		public readonly Expression Weight;


		public RewriteRuleReplacement(ImmutableListPos<LsystemSymbol> replac, Expression wei, Position pos) {

			Replacement = replac;
			Weight = wei;

			Position = pos;
		}


		public Position Position { get; private set; }

	}
}
