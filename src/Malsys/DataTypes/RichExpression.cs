using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RichExpression {

		public readonly IExpression Expression;
		public readonly ImmutableList<VariableDefinition> VariableDefinitions;


		public RichExpression(IEnumerable<VariableDefinition> varDefs, IExpression expr) {
			VariableDefinitions = ImmutableList<VariableDefinition>.Empty;
			Expression = expr;
		}

		public RichExpression(ImmutableList<VariableDefinition> varDefs, IExpression expr) {
			VariableDefinitions = varDefs;
			Expression = expr;
		}
	}
}
