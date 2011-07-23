using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RewriteRule : IToken, ILsystemStatement {

		public readonly SymbolPattern Pattern;

		public readonly SymbolPatternsList LeftContext;
		public readonly SymbolPatternsList RightContext;

		public readonly RichExpression Condition;

		public readonly RichExpression Probability;

		public readonly int LocalVariableDefsCount;
		public readonly int ReplacementSymbolsCount;

		private VariableDefinition[] variableDefs;
		private SymbolWithArgs[] replacement;


		public RewriteRule(SymbolPatternsList lctxt, SymbolPattern pattern, SymbolPatternsList rctxt, RichExpression cond, RichExpression probab,
				IEnumerable<VariableDefinition> varDefs, IEnumerable<SymbolWithArgs> replac, Position pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			Probability = probab;
			variableDefs = varDefs.ToArray();
			replacement = replac.ToArray();
			Position = pos;

			LocalVariableDefsCount = variableDefs.Length;
			ReplacementSymbolsCount = replacement.Length;
		}

		public VariableDefinition GetVariableDefinition(int i) {
			return variableDefs[i];
		}

		public SymbolWithArgs GetReplacSumbol(int i) {
			return replacement[i];
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
