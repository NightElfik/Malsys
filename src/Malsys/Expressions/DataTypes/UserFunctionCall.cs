using System.Collections.Generic;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UserFunctionCall : IExpression, IExpressionVisitable {

		public readonly string Name;
		public readonly ImmutableList<IExpression> Arguments;


		public UserFunctionCall(string name, IEnumerable<IExpression> args) {
			Name = name;
			Arguments = new ImmutableList<IExpression>(args);
		}

		public UserFunctionCall(string name, ImmutableList<IExpression> args) {
			Name = name;
			Arguments = args;
		}


		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
