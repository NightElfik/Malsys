﻿using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule : ILsystemStatement {

		public readonly LsystemSymbol Pattern;
		public readonly ImmutableListPos<LsystemSymbol> LeftContext;
		public readonly ImmutableListPos<LsystemSymbol> RightContext;

		public readonly ImmutableListPos<ConstantDefinition> LocalConstDefs;

		public readonly Expression Condition;

		public readonly ImmutableListPos<RewriteRuleReplacement> Replacements;


		public RewriteRule(LsystemSymbol pattern, ImmutableListPos<LsystemSymbol> lctxt, ImmutableListPos<LsystemSymbol> rctxt,
				ImmutableListPos<ConstantDefinition> localConsts, Expression cond, ImmutableListPos<RewriteRuleReplacement> replacs, Position pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			LocalConstDefs = localConsts;
			Replacements = replacs;

			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstLsystemVisitable Members

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
