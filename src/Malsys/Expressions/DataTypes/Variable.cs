
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Variable : IExpression, IExpressionVisitable {
		public readonly string Name;

		public Variable(string name) {
			Name = name;
		}


		public override string ToString() {
			return Name;
		}

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
