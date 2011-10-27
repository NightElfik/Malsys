using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule : IToken, ILsystemStatement {

		public readonly ImmutableList<KeywordPos> Keywords;

		public readonly Symbol<Identificator> Pattern;
		public readonly SymbolsListPos<Identificator> LeftContext;
		public readonly SymbolsListPos<Identificator> RightContext;

		public readonly ImmutableListPos<VariableDefinition> LocalVariables;

		public readonly Expression Condition;

		public readonly ImmutableListPos<RewriteRuleReplacement> Replacements;


		public RewriteRule(Symbol<Identificator> pattern, SymbolsListPos<Identificator> lctxt, SymbolsListPos<Identificator> rctxt,
				ImmutableListPos<VariableDefinition> locVars, Expression cond, ImmutableListPos<RewriteRuleReplacement> replacs,
				ImmutableList<KeywordPos> keywords, Position pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			LocalVariables = locVars;
			Replacements = replacs;
			Keywords = keywords.WithoutEmpty();

			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
