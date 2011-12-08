
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class EmptyExpression : IExpression {

		public static readonly EmptyExpression Instance = new EmptyExpression();

		/// <summary>
		/// Use static instance instead.
		/// </summary>
		private EmptyExpression() {

		}


		public bool IsEmptyExpression { get { return true; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
