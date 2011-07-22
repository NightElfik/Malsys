
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UserFunction : IExpression, IExpressionVisitable {

		public IExpression this[int i] { get { return arguments[i]; } }

		public readonly string Name;
		public readonly int ArgumentsCount;

		private IExpression[] arguments;


		public UserFunction(string name, IExpression[] args) {
			Name = name;
			arguments = args;

			ArgumentsCount = arguments.Length;
		}


		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
