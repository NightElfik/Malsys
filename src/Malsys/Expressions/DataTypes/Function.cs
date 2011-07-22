
namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Function : IExpression, IExpressionVisitable {

		public IExpression this[int i] { get { return arguments[i]; } }


		public readonly string Name;
		public readonly int ArgumentsCount;

		private IExpression[] arguments;
		private ExpressionValueType[] paramsTypes;

		public readonly EvalDelegate Evaluate;


		public Function(string name, EvalDelegate evalFunc, IExpression[] args, ExpressionValueType[] paramsTypes) {

			Name = name;
			ArgumentsCount = args.Length;
			Evaluate = evalFunc;
			arguments = args;
			this.paramsTypes = paramsTypes;
		}

		public ExpressionValueType GetValueType(int i) {
			return paramsTypes[i % paramsTypes.Length];
		}

		#region IExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
