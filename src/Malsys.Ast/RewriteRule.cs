using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public class RewriteRule : IToken, ILsystemStatement {
		public readonly RrContext LeftContext;
		public readonly SymbolPattern Pattern;
		public readonly RrContext RightContext;
		public readonly RrCondition Condition;
		public readonly RrProbability Probability;
		public readonly ReadOnlyCollection<SymbolWithParams> Replacement;

		public RewriteRule(RrContext lctxt, SymbolPattern pattern, RrContext rctxt, RrCondition cond, RrProbability probab,
				IList<SymbolWithParams> replac, Position pos) {

			LeftContext = lctxt;
			Pattern = pattern;
			RightContext = rctxt;
			Condition = cond;
			Probability = probab;
			Replacement = new ReadOnlyCollection<SymbolWithParams>(replac);
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

	public class RrContext : IToken, IAstVisitable {
		public readonly ReadOnlyCollection<SymbolPattern> Patterns;

		public RrContext(IList<SymbolPattern> patterns, Position pos) {
			Patterns = new ReadOnlyCollection<SymbolPattern>(patterns);
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

	public class RrCondition : IToken, IAstVisitable {
		public readonly ReadOnlyCollection<VariableDefinition> VariableDefinitions;
		public readonly Expression Expression;

		public RrCondition(IList<VariableDefinition> varDefs, Expression expr, Position pos) {
			VariableDefinitions = new ReadOnlyCollection<VariableDefinition>(varDefs);
			Expression = expr;
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

	public class RrProbability : IToken, IAstVisitable {
		public readonly ReadOnlyCollection<VariableDefinition> VariableDefinitions;
		public readonly Expression Expression;

		public RrProbability(IList<VariableDefinition> varDefs, Expression expr, Position pos) {
			VariableDefinitions = new ReadOnlyCollection<VariableDefinition>(varDefs);
			Expression = expr;
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
