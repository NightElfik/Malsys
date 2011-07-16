using System;

namespace Malsys.Expressions {
	public delegate double EvaluateDelegate(double[] parameters);

	/// <summary>
	/// Immutable arithmetic function.
	/// </summary>
	public class ArithmeticFunction {
		public readonly string Syntax;
		public readonly byte Arity;

		private readonly EvaluateDelegate evalFunction;

		public ArithmeticFunction(string syntax, byte arity, EvaluateDelegate evalFunc) {
			Syntax = syntax;
			Arity = arity;
			evalFunction = evalFunc;
		}

		public double Evaluate(params double[] values) {
#if DEBUG
			if (values.Length < Arity) {
				throw new ArgumentException("Failed to evaluate function `{0}'{1}` with only {2} argument(s).".Fmt(Syntax, Arity, values.Length));
			}
#endif
			return evalFunction.Invoke(values);
		}

		public override string ToString() {
			return Syntax + "'" + Arity;
		}
	}
}
