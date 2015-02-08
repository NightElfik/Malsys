
namespace Malsys.Ast {
	public class RewriteRule : ILsystemStatement {

		public LsystemSymbol Pattern;
		public ListPos<LsystemSymbol> LeftContext;
		public ListPos<LsystemSymbol> RightContext;

		public ListPos<ConstantDefinition> LocalConstDefs;
		public Expression Condition;
		public ListPos<RewriteRuleReplacement> Replacements;

		public PositionRange Position { get; private set; }


		public RewriteRule(LsystemSymbol pattern, ListPos<LsystemSymbol> lctxt, ListPos<LsystemSymbol> rctxt,
				ListPos<ConstantDefinition> localConsts, Expression cond, ListPos<RewriteRuleReplacement> replacs,
				PositionRange pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			LocalConstDefs = localConsts;
			Replacements = replacs;

			Position = pos;
		}


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.RewriteRule; }
		}

	}
}
