
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class EmptyExpression : IExpression, IExpressionVisitable {

		public static readonly EmptyExpression Instance = new EmptyExpression();

		/// <summary>
		/// Use static instance instead.
		/// </summary>
		private EmptyExpression() {

		}

		#region IExpression Members

		public bool IsEmpty { get { return true; } }

		#endregion

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
