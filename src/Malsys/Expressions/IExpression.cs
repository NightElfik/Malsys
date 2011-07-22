
namespace Malsys.Expressions {
	public delegate IValue EvalDelegate(ArgsStorage args);

	public interface IExpression : IExpressionVisitable { }
}
