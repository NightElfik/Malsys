
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
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



		public Position Position { get; private set; }


		public LsystemStatementType StatementType {
			get { return LsystemStatementType.RewriteRule; }
		}

	}
}
