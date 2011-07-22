using System.Collections.Generic;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionValuesArray : IExpression, IExpressionVisitable {

		public IExpression this[int i] { get { return values[i]; } }
		public int Length { get { return values.Length; } }

		private IExpression[] values;


		public ExpressionValuesArray(IList<IExpression> values) {
			this.values = new IExpression[values.Count];
			for (int i = 0; i < values.Count; i++) {
				this.values[i] = values[i];
			}
		}

		/// <summary>
		/// Faster version of constructing ExpressionValuesArray.
		/// Use only if no other reference will exist on given array.
		/// </summary>
		internal ExpressionValuesArray(IExpression[] immutableValues) {
			values = immutableValues;
		}

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
