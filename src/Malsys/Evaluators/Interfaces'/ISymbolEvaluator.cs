// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Linq;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {

	public interface ISymbolEvaluator {

		Symbol<IValue> Evaluate(Symbol<IExpression> symbol, IExpressionEvaluatorContext exprEvalCtxt);

	}


	public static class ISymbolEvaluatorExtensions {

		public static List<Symbol<IValue>> EvaluateList(this ISymbolEvaluator evaluator, List<Symbol<IExpression>> symbols,
				IExpressionEvaluatorContext exprEvalCtxt) {
	
			return symbols.Select(s => evaluator.Evaluate(s, exprEvalCtxt)).ToList();
		}

	}

}
