using Malsys.Expressions;

namespace Malsys {
	public class RewriteRule {
		public SymbolPattern SymbolPattern { get; set; }

		public SymbolPattern[] LeftContext { get; set; }
		public SymbolPattern[] RightContext { get; set; }

		public VariableDefinition[] PreConditionVars { get; set; }
		public IExpression Condition { get; set; }

		public VariableDefinition[] PreProbabilityVars { get; set; }
		public IExpression ProbabilityWeight { get; set; }

		public VariableDefinition[] ReplacementVars { get; set; }
		public Symbol[] Replacement { get; set; }
	}

}
