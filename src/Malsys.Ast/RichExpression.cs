using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Expression with local variable defnitions.
	/// Immutable.
	/// </summary>
	public class RichExpression : ImmutableList<VariableDefinition>, IAstVisitable {

		public readonly Expression Expression;
		public readonly int LocalVariableDefsCount;

		private readonly new int Length;

		public RichExpression(IEnumerable<VariableDefinition> varDefs, Expression expr, Position pos) : base(varDefs, pos) {
			Expression = expr;
			Length = LocalVariableDefsCount = base.Length;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
