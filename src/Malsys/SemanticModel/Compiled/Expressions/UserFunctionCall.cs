
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UserFunctionCall : IExpression {

		public readonly string Name;
		public readonly ImmutableList<IExpression> Arguments;


		public UserFunctionCall(string name, ImmutableList<IExpression> args) {
			Name = name;
			Arguments = args;
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
