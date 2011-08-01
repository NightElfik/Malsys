using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Expression with local variable defnitions.
	/// Immutable.
	/// </summary>
	public class RichExpression : IToken, IAstVisitable {

		public readonly Expression Expression;
		public readonly ImmutableList<VariableDefinition> VariableDefinitions;


		public RichExpression(Position pos) {
			Expression = new Expression(pos);
			VariableDefinitions = ImmutableList<VariableDefinition>.Empty;
			Position = pos;
		}

		public RichExpression(IEnumerable<VariableDefinition> varDefs, Expression expr, Position pos) {
			Expression = expr;
			VariableDefinitions = new ImmutableList<VariableDefinition>(varDefs);
			Position = pos;
		}

		public RichExpression(RichExpression rExpr, Position pos) {
			Expression = rExpr.Expression;
			VariableDefinitions = rExpr.VariableDefinitions;
			Position = pos;
		}


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
