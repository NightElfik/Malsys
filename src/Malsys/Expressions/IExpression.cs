
namespace Malsys.Expressions {
	public delegate IValue EvalDelegate(ArgsStorage args);

	/// <summary>
	/// All classes implementing this interface should be immutable.
	/// </summary>
	public interface IExpression : IExpressionVisitable { }
}
