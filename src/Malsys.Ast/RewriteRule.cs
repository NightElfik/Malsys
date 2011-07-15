using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public class RewriteRule : Token, ILsystemStatement {
		public readonly RrContext LeftContext;
		public readonly SymbolPattern Pattern;
		public readonly RrContext RightContext;
		public readonly RrCondition Condition;
		public readonly RrProbability Probability;
		public readonly ReadOnlyCollection<SymbolWithParams> Replacement;

		public RewriteRule(RrContext lctxt, SymbolPattern pattern, RrContext rctxt, RrCondition cond, RrProbability probab,
				IList<SymbolWithParams> replac, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			Probability = probab;
			Replacement = new ReadOnlyCollection<SymbolWithParams>(replac);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public class RrContext : Token, IAstVisitable {
		public readonly ReadOnlyCollection<SymbolPattern> Patterns;

		public RrContext(IList<SymbolPattern> patterns, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Patterns = new ReadOnlyCollection<SymbolPattern>(patterns);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public class RrCondition : Token, IAstVisitable {
		public readonly ReadOnlyCollection<VariableDefinition> VariableDefinitions;
		public readonly Expression Expression;

		public RrCondition(IList<VariableDefinition> varDefs, Expression expr, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			VariableDefinitions = new ReadOnlyCollection<VariableDefinition>(varDefs);
			Expression = expr;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public class RrProbability : Token, IAstVisitable {
		public readonly ReadOnlyCollection<VariableDefinition> VariableDefinitions;
		public readonly Expression Expression;

		public RrProbability(IList<VariableDefinition> varDefs, Expression expr, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			VariableDefinitions = new ReadOnlyCollection<VariableDefinition>(varDefs);
			Expression = expr;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
