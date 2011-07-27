using System.Collections.Generic;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionValuesArray : ImmutableList<IExpression>, IExpression, IExpressionVisitable {

		public ExpressionValuesArray()
			: base(ImmutableList<IExpression>.Empty) { }

		public ExpressionValuesArray(IEnumerable<IExpression> values)
			: base(values) { }

		public ExpressionValuesArray(ImmutableList<IExpression> values)
			: base(values) { }


		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
