using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule : IToken, ILsystemStatement {

		public readonly SymbolPattern Pattern;

		public readonly ImmutableListPos<SymbolPattern> LeftContext;
		public readonly ImmutableListPos<SymbolPattern> RightContext;

		public readonly RichExpression Condition;

		public readonly RichExpression Probability;

		public readonly ImmutableListPos<VariableDefinition> VariableDefs;
		public readonly ImmutableList<SymbolExprArgs> Replacement;


		public RewriteRule(SymbolPattern pattern, ImmutableListPos<SymbolPattern> lctxt, ImmutableListPos<SymbolPattern> rctxt, RichExpression cond, RichExpression probab,
				ImmutableListPos<VariableDefinition> varDefs, IEnumerable<SymbolExprArgs> replac, Position pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			Probability = probab;
			VariableDefs = varDefs;
			Replacement = new ImmutableList<SymbolExprArgs>(replac);
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
