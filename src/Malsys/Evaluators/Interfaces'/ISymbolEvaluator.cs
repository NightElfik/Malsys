using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {

	public interface ISymbolEvaluator {

		Symbol<IValue> Evaluate(Symbol<IExpression> symbol, IExpressionEvaluatorContext exprEvalCtxt);

	}


	public static class ISymbolEvaluatorExtensions {

		public static ImmutableList<Symbol<IValue>> EvaluateList(this ISymbolEvaluator evaluator, ImmutableList<Symbol<IExpression>> symbols,
				IExpressionEvaluatorContext exprEvalCtxt) {

			var result = new Symbol<IValue>[symbols.Length];

			for (int i = 0; i < symbols.Length; i++) {
				result[i] = evaluator.Evaluate(symbols[i], exprEvalCtxt);
			}

			return new ImmutableList<Symbol<IValue>>(result, true);
		}

	}

}
