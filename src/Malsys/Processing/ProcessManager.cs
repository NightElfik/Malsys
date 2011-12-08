using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using System;
using Malsys.SemanticModel.Compiled;
using System.Collections.Generic;
using Microsoft.FSharp.Core;

namespace Malsys.Processing {
	public class ProcessManager {

		private ProcessStatement defaultProcessStatement = new ProcessStatement("",
			ProcessConfigurations.PrintSymbolsConfig.Name, ImmutableList<ProcessComponentAssignment>.Empty);


		public TimeSpan Timeout { get; set; }

		public ProcessManager() {
			Timeout = new TimeSpan(0, 0, 10);
		}


		public void RenderLsystems(string src, FilesManager fm, MessageLogger logger, IComponentResolver componentResolver) {

			var compiler = new CompilersContainer();
			var inCompiled = compiler.CompileInput(src, "webInput", logger);

			if (logger.ErrorOcured) {
				return;
			}

			var evaluator = new EvaluatorsContainer();
			var inBlockEvaled = evaluator.EvaluateInput(inCompiled);

			foreach (var lsystemKvp in inBlockEvaled.Lsystems) {
				var lsysEvaled = evaluator.EvaluateLsystem(lsystemKvp.Value, ImmutableList<IValue>.Empty,
					inBlockEvaled.GlobalConstants, inBlockEvaled.GlobalFunctions);

				var context = new ProcessContext(lsysEvaled, fm, inBlockEvaled, evaluator, logger);

				IEnumerable<ProcessStatement> processStatements;
				if(lsysEvaled.ProcessStatements.Length != 0){
					processStatements = lsysEvaled.ProcessStatements;
				}
				else{
					processStatements = new ProcessStatement[]{ defaultProcessStatement };
				}

				var processConfigsMap = inBlockEvaled.ProcessConfigurations;
				processConfigsMap = processConfigsMap.Add(ProcessConfigurations.PrintSymbolsConfig.Name, ProcessConfigurations.PrintSymbolsConfig);

				foreach (var processStat in processStatements) {

					var maybeConfig = processConfigsMap.TryFind(processStat.ProcessConfiName);
					if (OptionModule.IsNone(maybeConfig)) {
						logger.LogError("UnknownProcessConfig", Position.Unknown, "Unknown process configuration `{0}`.".Fmt(processStat.ProcessConfiName));
						return;
					}
					var configMgr = new ProcessConfigurationManager();

					configMgr.BuildConfiguration(maybeConfig.Value, processStat.ComponentAssignments, componentResolver, context);

					configMgr.StarterComponent.Start(configMgr.RequiresMeasure, Timeout);

					configMgr.ClearComponents();
				}

				fm.TryDeleteAllTempFiles();
			}

		}

	}
}
