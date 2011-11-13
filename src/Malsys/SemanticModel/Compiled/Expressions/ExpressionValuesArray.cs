using System.Collections.Generic;

namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionValuesArray : ImmutableList<IExpression>, IExpression, IExpressionVisitable {

		public ExpressionValuesArray()
			: base(ImmutableList<IExpression>.Empty) { }

		public ExpressionValuesArray(ImmutableList<IExpression> values)
			: base(values) { }



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
