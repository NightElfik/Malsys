
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ExpressionValuesArray : ImmutableList<IExpression>, IExpression {

		public ExpressionValuesArray()
			: base(ImmutableList<IExpression>.Empty) { }

		public ExpressionValuesArray(ImmutableList<IExpression> values)
			: base(values) { }



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
