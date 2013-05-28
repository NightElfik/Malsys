﻿// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class RewriteRule : ILsystemStatement {

		public readonly Symbol<string> SymbolPattern;

		public readonly SymbolsList<string> LeftContext;
		public readonly SymbolsList<string> RightContext;

		public readonly ImmutableList<ConstantDefinition> LocalConstantDefs;

		public readonly IExpression Condition;

		public readonly ImmutableList<RewriteRuleReplacement> Replacements;



		public RewriteRule(Symbol<string> symbolPtrn, SymbolsList<string> lCtxt, SymbolsList<string> rCtxt,
				ImmutableList<ConstantDefinition> locConsts, IExpression cond,
				ImmutableList<RewriteRuleReplacement> replacs) {

			SymbolPattern = symbolPtrn;
			LeftContext = lCtxt;
			RightContext = rCtxt;
			LocalConstantDefs = locConsts;
			Condition = cond;
			Replacements = replacs;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.RewriteRule; }
		}

	}

}
