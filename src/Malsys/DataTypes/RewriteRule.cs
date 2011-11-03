using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule {

		public readonly Symbol<string> SymbolPattern;

		public readonly SymbolsList<string> LeftContext;
		public readonly SymbolsList<string> RightContext;

		public readonly ImmutableList<VariableDefinition<IExpression>> LocalVariables;

		public readonly IExpression Condition;

		public readonly ImmutableList<RewriteRuleReplacement> Replacements;



		public RewriteRule(Symbol<string> symbolPtrn, SymbolsList<string> lCtxt, SymbolsList<string> rCtxt,
			ImmutableList<VariableDefinition<IExpression>> locVars, IExpression cond,
			   ImmutableList<RewriteRuleReplacement> replacs) {

			SymbolPattern = symbolPtrn;
			LeftContext = lCtxt;
			RightContext = rCtxt;
			LocalVariables = locVars;
			Condition = cond;
			Replacements = replacs;
		}
	}

}
