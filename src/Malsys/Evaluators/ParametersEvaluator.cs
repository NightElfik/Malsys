using System.Collections.Generic;
using System.Linq;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <remarks>
	/// All public members are thread safe.
	/// </remarks>
	public class ParametersEvaluator : IParametersEvaluator {

		public List<OptionalParameterEvaled> Evaluate(IEnumerable<OptionalParameter> optPrms, IExpressionEvaluatorContext exprEvalCtxt) {
			return optPrms.Select(p =>
				new OptionalParameterEvaled(p.AstNode){
					Name = p.Name,
					DefaultValue = p.IsOptional ? exprEvalCtxt.Evaluate(p.DefaultValue) : null,
				}
			).ToList();
		}

	}
}
