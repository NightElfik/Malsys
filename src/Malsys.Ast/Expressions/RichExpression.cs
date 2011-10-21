using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Expression with local variable defnitions.
	/// Immutable.
	/// </summary>
	public class RichExpression : IToken {

		public readonly Expression Expression;
		public readonly ImmutableList<VariableDefinition> VariableDefinitions;
		public readonly Position BeginSeparator;
		public readonly Position EndSeparator;


		#region Constructors

		public RichExpression(Position pos)
			: this(Position.Unknown, Position.Unknown, pos) {
		}

		public RichExpression(Position beginSep, Position endSep, Position pos)
			: this(ImmutableList<VariableDefinition>.Empty, new Expression(pos), beginSep, endSep, pos) {
		}

		public RichExpression(IEnumerable<VariableDefinition> varDefs, Expression expr, Position pos)
			: this(varDefs, expr, Position.Unknown, Position.Unknown, pos) {
		}

		public RichExpression(IEnumerable<VariableDefinition> varDefs, Expression expr, Position beginSep, Position endSep, Position pos)
			: this(varDefs.ToImmutableList(),expr, beginSep, endSep, pos) {
		}

		public RichExpression(ImmutableList<VariableDefinition> varDefs, Expression expr, Position beginSep, Position endSep, Position pos) {
			Expression = expr;
			VariableDefinitions = varDefs;
			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		public RichExpression(RichExpression rExpr, Position pos) {
			Expression = rExpr.Expression;
			VariableDefinitions = rExpr.VariableDefinitions;
			Position = pos;
		}

		#endregion


		public bool IsEmpty { get { return Expression.IsEmpty && VariableDefinitions.IsEmpty; } }


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
