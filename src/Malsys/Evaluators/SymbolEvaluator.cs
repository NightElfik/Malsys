using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Evaluators {
	internal class SymbolEvaluator : ISymbolEvaluator {

		private IExpressionEvaluator exprEvaluator;


		public SymbolEvaluator(IExpressionEvaluator expressionEvaluator) {
			exprEvaluator = expressionEvaluator;
		}


		public Symbol<IValue> Evaluate(Symbol<IExpression> symbol, ConstsMap consts, FunsMap funs) {
			return new Symbol<IValue>(symbol.Name, exprEvaluator.EvaluateList(symbol.Arguments, consts, funs));
		}


	}
}
