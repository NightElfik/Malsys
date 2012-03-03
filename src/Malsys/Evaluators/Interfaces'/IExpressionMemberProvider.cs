using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	public interface IExpressionMemberProvider {

		bool TryGetVariableValue(string name, out IValue value);

		bool TryEvaluateFunction(string name, ArgsStorage args, out IValue value);

	}
}
