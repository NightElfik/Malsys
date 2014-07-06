// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class RewriteRuleReplacement : IAstNode {

		public ListPos<LsystemSymbol> Replacement;
		public Expression Weight;

		public PositionRange Position { get; private set; }


		public RewriteRuleReplacement(ListPos<LsystemSymbol> replac, Expression wei, PositionRange pos) {

			Replacement = replac;
			Weight = wei;

			Position = pos;
		}

	}
}
