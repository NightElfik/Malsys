using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule {
		public readonly Symbol<string> SymbolPattern;

		public readonly SymbolsList<string> LeftContext;
		public readonly SymbolsList<string> RightContext;

		public readonly RichExpression Condition;

		public readonly RichExpression ProbabilityWeight;

		public readonly ImmutableList<VariableDefinition> ReplacementVars;
		public readonly SymbolsList<IExpression> Replacement;


		public RewriteRule(Symbol<string> symbolPtrn, SymbolsList<string> lCtxt, SymbolsList<string> rCtxt, RichExpression cond,
			   RichExpression probab, ImmutableList<VariableDefinition> replacVars, SymbolsList<IExpression> replac) {

			SymbolPattern = symbolPtrn;
			LeftContext = lCtxt;
			RightContext = rCtxt;
			Condition = cond;
			ProbabilityWeight = probab;
			ReplacementVars = replacVars;
			Replacement = replac;
		}
	}

}
