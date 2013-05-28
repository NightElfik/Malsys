// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.Evaluators;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.LsystemEvaledParams>;
using ProcConfsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.ProcessConfigurationStatement>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlockEvaled {

		public readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;
		public readonly LsysMap Lsystems;
		public readonly ProcConfsMap ProcessConfigurations;
		public readonly ImmutableList<ProcessStatementEvaled> ProcessStatements;

		public string SourceName;


		public InputBlockEvaled(IExpressionEvaluatorContext exprEvalCtxt, LsysMap lsystems,
				ProcConfsMap procConfigs, ImmutableList<ProcessStatementEvaled> procStats, string sourceName) {

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
