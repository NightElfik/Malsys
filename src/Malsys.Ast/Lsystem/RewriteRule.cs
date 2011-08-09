using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule : IToken, ILsystemStatement {

		public readonly ImmutableList<Keyword> Keywords;

		public readonly SymbolPattern Pattern;
		public readonly ImmutableListPos<SymbolPattern> LeftContext;
		public readonly ImmutableListPos<SymbolPattern> RightContext;

		public readonly ImmutableListPos<VariableDefinition> LocalVariables;

		public readonly Expression Condition;

		public readonly ImmutableListPos<SymbolExprArgs> Replacement;

		public readonly Expression Weight;


		public RewriteRule(SymbolPattern pattern, ImmutableListPos<SymbolPattern> lctxt, ImmutableListPos<SymbolPattern> rctxt,
				ImmutableListPos<VariableDefinition> locVars, Expression cond, ImmutableListPos<SymbolExprArgs> replac, Expression wei,
				ImmutableList<Keyword> keywords, Position pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			Weight = wei;
			LocalVariables = locVars;
			Replacement = replac;
			Keywords = keywords;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
