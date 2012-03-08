using Malsys.Evaluators;
using Malsys.SemanticModel.Compiled;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.LsystemEvaledParams>;
using ProcConfsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.ProcessConfigurationStatement>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlockEvaled {

		public readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;
		public readonly LsysMap Lsystems;
		public readonly ProcConfsMap ProcessConfigurations;
		public readonly ImmutableList<ProcessStatement> ProcessStatements;

		public string SourceName;


		public InputBlockEvaled(IExpressionEvaluatorContext exprEvalCtxt, LsysMap lsystems,
				ProcConfsMap procConfigs, ImmutableList<ProcessStatement> procStats, string sourceName) {

			ExpressionEvaluatorContext = exprEvalCtxt;
			Lsystems = lsystems;
			ProcessConfigurations = procConfigs;
			ProcessStatements = procStats;

			SourceName = sourceName;
		}


		public InputBlockEvaled JoinWith(InputBlockEvaled inputBlock) {

			return new InputBlockEvaled(
				ExpressionEvaluatorContext.MergeWith(inputBlock.ExpressionEvaluatorContext),
				Lsystems.AddRange(inputBlock.Lsystems),
				ProcessConfigurations.AddRange(inputBlock.ProcessConfigurations),
				ProcessStatements.AddRange(inputBlock.ProcessStatements),
				inputBlock.SourceName);

		}


	}
}
