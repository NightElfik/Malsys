// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class RewriteRuleReplacement : IAstNode {

		public readonly ImmutableListPos<LsystemSymbol> Replacement;
		public readonly Expression Weight;


		public RewriteRuleReplacement(ImmutableListPos<LsystemSymbol> replac, Expression wei, PositionRange pos) {

			Replacement = replac;
			Weight = wei;

			Position = pos;
		}


		public PositionRange Position { get; private set; }

	}
}
