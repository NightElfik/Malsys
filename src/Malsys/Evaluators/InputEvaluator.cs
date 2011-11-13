using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using System.Collections.Generic;

namespace Malsys.Evaluators {
	public class InputEvaluator {

		private BindingsEvaluator bindEvaluator;


		public InputEvaluator(BindingsEvaluator bindEval) {
			bindEvaluator = bindEval;
		}


		public InputBlock Evaluate(IEnumerable<IInputStatement> inStatements) {

			BindingMaps maps = new BindingMaps(BindingType.Expression | BindingType.Function | BindingType.Lsystem);

			foreach (var stat in inStatements) {
				switch (stat.StatementType) {
					case InputStatementType.Binding:
						bindEvaluator.Evaluate((Binding)stat, maps);
						break;
					default:
						throw new EvalException("Unknown Input statement type.");
				}
			}

			return new InputBlock(maps.Variables, maps.Functions, maps.Lsystems);
		}

	}
}
