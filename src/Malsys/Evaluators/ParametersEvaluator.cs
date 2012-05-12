﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe.
	/// </remarks>
	public class ParametersEvaluator : IParametersEvaluator {


		public ImmutableList<OptionalParameterEvaled> Evaluate(ImmutableList<OptionalParameter> optPrms, IExpressionEvaluatorContext exprEvalCtxt) {

			var result = new OptionalParameterEvaled[optPrms.Length];

			for (int i = 0; i < result.Length; i++) {
				var currParam = optPrms[i];

				if (currParam.IsOptional) {
					result[i] = new OptionalParameterEvaled(currParam.Name, exprEvalCtxt.Evaluate(currParam.DefaultValue), currParam.AstNode);
				}
				else {
					result[i] = new OptionalParameterEvaled(currParam.Name, currParam.AstNode);
				}
			}

			return new ImmutableList<OptionalParameterEvaled>(result, true);
		}


	}
}
