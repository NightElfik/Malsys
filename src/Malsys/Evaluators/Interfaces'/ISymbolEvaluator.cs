using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {

	public interface ISymbolEvaluator {

		Symbol<IValue> Evaluate(Symbol<IExpression> symbol, ConstsMap consts, FunsMap funs);

	}


	public static class ISymbolEvaluatorExtensions {

		public static ImmutableList<Symbol<IValue>> EvaluateList(this ISymbolEvaluator evaluator, ImmutableList<Symbol<IExpression>> symbols,
				ConstsMap consts, FunsMap funs) {

			var result = new Symbol<IValue>[symbols.Length];

			for (int i = 0; i < symbols.Length; i++) {
				result[i] = evaluator.Evaluate(symbols[i], consts, funs);
			}

			return new ImmutableList<Symbol<IValue>>(result, true);
		}

	}

}
