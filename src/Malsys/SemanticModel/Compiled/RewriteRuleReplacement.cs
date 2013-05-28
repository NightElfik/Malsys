// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class RewriteRuleReplacement {

		public static readonly RewriteRuleReplacement Empty
			= new RewriteRuleReplacement(ImmutableList<Symbol<IExpression>>.Empty, Constant.One);


		public readonly ImmutableList<Symbol<IExpression>> Replacement;
		public readonly IExpression Weight;

		public RewriteRuleReplacement(ImmutableList<Symbol<IExpression>> replac, IExpression wei) {
			Replacement = replac;
			Weight = wei;
		}
	}
}
