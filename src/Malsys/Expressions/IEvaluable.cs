
namespace Malsys.Expressions {
	public delegate IValue EvaluateDelegate(IValue[] parameters);

	public interface IEvaluable : IPostfixExpressionMember {
		byte Arity { get; }

		IValue Evaluate(params IValue[] args);
	}
}
