using Malsys.SemanticModel.Compiled.Expressions;

namespace Malsys.SemanticModel.Compiled {

	/// <summary>
	/// All classes implementing this interface should be immutable.
	/// </summary>
	public interface IExpression : IExpressionVisitable, IBindable {
	}
}
