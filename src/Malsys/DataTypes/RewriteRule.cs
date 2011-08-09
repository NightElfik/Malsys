using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule {

		public readonly Symbol<string> SymbolPattern;

		public readonly SymbolsList<string> LeftContext;
		public readonly SymbolsList<string> RightContext;

		public readonly IExpression Condition;

		public readonly IExpression Weight;

		public readonly ImmutableList<VariableDefinition<IExpression>> LocalVariables;
		public readonly SymbolsList<IExpression> Replacement;


		public RewriteRule(Symbol<string> symbolPtrn, SymbolsList<string> lCtxt, SymbolsList<string> rCtxt, IExpression cond,
			   IExpression wei, ImmutableList<VariableDefinition<IExpression>> locVars, SymbolsList<IExpression> replac) {

			SymbolPattern = symbolPtrn;
			LeftContext = lCtxt;
			RightContext = rCtxt;
			Condition = cond;
			Weight = wei;
			LocalVariables = locVars;
			Replacement = replac;
		}
	}

}
