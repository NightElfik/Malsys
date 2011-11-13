using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UserFunctionCall : IExpression, IExpressionVisitable {

		public readonly string Name;
		public readonly ImmutableList<IExpression> Arguments;


		public UserFunctionCall(string name, ImmutableList<IExpression> args) {
			Name = name;
			Arguments = args;
		}


		#region IExpression Members

		public bool IsEmpty { get { return false; } }

		#endregion

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
