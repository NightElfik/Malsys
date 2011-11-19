
namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
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

		#region ILsystemStatement Members

		public LsystemStatementType StatementType { get { return LsystemStatementType.RewriteRule; } }

		#endregion
	}

}
