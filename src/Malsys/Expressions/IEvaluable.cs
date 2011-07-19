
namespace Malsys.Expressions {
	public interface IEvaluable {
		byte Arity { get; }

		double Evaluate(params double[] args);
	}
}
