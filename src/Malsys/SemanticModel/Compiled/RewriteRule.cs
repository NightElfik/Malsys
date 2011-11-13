using Malsys.Expressions;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule : ILsystemStatement {

		public readonly Symbol<string> SymbolPattern;

		public readonly SymbolsList<string> LeftContext;
		public readonly SymbolsList<string> RightContext;

		public readonly ImmutableList<Binding> LocalBindings;

		public readonly IExpression Condition;

		public readonly ImmutableList<RewriteRuleReplacement> Replacements;



		public RewriteRule(Symbol<string> symbolPtrn, SymbolsList<string> lCtxt, SymbolsList<string> rCtxt,
				ImmutableList<Binding> localBinds, IExpression cond,
				ImmutableList<RewriteRuleReplacement> replacs) {

			SymbolPattern = symbolPtrn;
			LeftContext = lCtxt;
			RightContext = rCtxt;
			LocalBindings = localBinds;
			Condition = cond;
			Replacements = replacs;
		}

		#region ILsystemStatement Members

		public LsystemStatementType StatementType { get { return LsystemStatementType.RewriteRule; } }

		#endregion
	}

}
