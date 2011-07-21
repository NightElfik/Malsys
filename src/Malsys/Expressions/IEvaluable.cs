
namespace Malsys.Expressions {
	public delegate IValue EvaluateDelegate(ArgsStorage args);

	public interface IEvaluable : IPostfixExpressionMember {
		int Arity { get; }
		string Name { get; }
		string Syntax { get; }

		IValue Evaluate(ArgsStorage args);
	}
}
