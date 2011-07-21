using System;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Indexer : IEvaluable, IPostfixExpressionMember {

		#region IEvaluable Members

		public int Arity { get { return 2; } }
		public string Name { get { return "indexer"; } }
		public string Syntax { get { return "[]"; } }

		public IValue Evaluate(ArgsStorage args) {
			if (args.ArgsCount != 2) {
				throw new EvalException("Failed to evaluate indexer with {0} argument(s), it needs 2.".Fmt(args.ArgsCount));
			}

			if (!args[0].IsArray) {
				throw new EvalException("Failed to evaluate indexer. Excpected first argument as array, but it is value.");
			}

			if (!args[1].IsConstant) {
				throw new EvalException("Failed to evaluate indexer. Excpected second argument as value, but it is array.");
			}

			Constant index = (Constant)args[1];
			if (index.Value < 0) {
				throw new EvalException("Failed to evaluate indexer, index out of range. Index is zero-based but negative value `{0}` was given.".Fmt(index.Value));
			}

			ValuesArray arr = (ValuesArray)args[0];
			if (index.Value >= arr.Length) {
				throw new EvalException("Failed to evaluate indexer, index out of range. Cannot index array of length {0} with zero-based index {1}.".Fmt(arr.Length, index.Value));
			}

			return arr[(int)Math.Round(index.Value)];
		}

		#endregion

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsEvaluable { get { return true; } }
		public bool IsUnknownFunction { get { return false; } }

		#endregion
	}
}
