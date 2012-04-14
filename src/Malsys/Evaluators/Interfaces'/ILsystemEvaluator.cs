using System.Collections.Generic;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface ILsystemEvaluator {

		LsystemEvaled Evaluate(LsystemEvaledParams lsystem, IList<IValue> arguments, IExpressionEvaluatorContext exprEvalCtxt,
				IBaseLsystemResolver baseResolver, IMessageLogger logger);

		LsystemEvaled EvaluateAdditionalStatements(LsystemEvaled lsystem, IEnumerable<ILsystemStatement> additionalStatements, IMessageLogger logger);

	}
}
