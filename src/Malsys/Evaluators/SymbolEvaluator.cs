// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe.
	/// </remarks>
	public class SymbolEvaluator : ISymbolEvaluator {

		public Symbol<IValue> Evaluate(Symbol<IExpression> symbol, IExpressionEvaluatorContext exprEvalCtxt) {
			return new Symbol<IValue>(symbol.AstNode) {
				Name = symbol.Name,
				Arguments = exprEvalCtxt.EvaluateList(symbol.Arguments),
			};
		}

	}
}
